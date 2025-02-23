using MusicMage.Backend.Enums;

namespace MusicMage.Backend.Helpers;

public static class NoteHelper
{
    public const int SampleRate = 44100; // Standard audio sample rate
    private const double FadeOutDuration = 0.02;
    private const short Amplitude = 16383; // short.MaxValue / 2
    private const float DecayMs = 20;
    private const float SustainLevel = 0.7f;
    private const short ReleaseMs = 50;
    private const int FadeOutSamples = (int)(FadeOutDuration * SampleRate);
    private static readonly Dictionary<string, float> NoteXFrequency = new()
    {
        ["C"] = 16.35f,
        ["C#"] = 17.32f,
        ["D"] = 18.35f,
        ["D#"] = 19.45f,
        ["E"] = 20.6f,
        ["F"] = 21.83f,
        ["F#"] = 23.12f,
        ["G"] = 24.5f,
        ["G#"] = 25.96f,
        ["A"] = 27.5f,
        ["A#"] = 29.14f,
        ["B"] = 30.87f
    };

    private static readonly Dictionary<WaveType, Func<int, double, int, double, short>> WaveTypeXWaveMethod = new()
    {
        { WaveType.Sine, (i, angle, _, _) => GetSineWaveBuffer(i, angle) },
        { WaveType.Square, (i, _, samples, dutyCycle) => GetSquareWaveBuffer(i, samples, dutyCycle) },
        { WaveType.Triangle, (i, _, samples, _) => GetTriangleWaveBuffer(i, samples) }
    };

    public static float GetNoteFrequency(string note, int octave = 0)
    {
        var noteToReturn = NoteXFrequency.GetValueOrDefault(note, 0);
        return noteToReturn * MathF.Pow(2, octave);
    }
    
    public static byte[] GetWaveBuffer(
        Dictionary<int, (double frequency, float durationMs, WaveType waveType, double dutyCycle)> channelSettings,
        bool fade = true)
    {
        var channels = channelSettings.Count;
        var channelSampleCounts = channelSettings.ToDictionary(
            c => c.Key,
            c => (int)(SampleRate * c.Value.durationMs / 1000.0)
        );

        var maxSampleCount = channelSampleCounts.Values.Max();

        var noteBuffer = new short[maxSampleCount, channels];

        foreach (var channel in channelSettings.Keys)
        {
            var frequency = channelSettings[channel].frequency;
            var durationMs = channelSettings[channel].durationMs;
            var waveType = channelSettings[channel].waveType;

            var noteSampleCount = (int)(SampleRate * durationMs / 1000.0);
            var samplesPerCycle = (int)(SampleRate / frequency);
            var angleIncrement = 2.0 * Math.PI * frequency / SampleRate;

            const int decaySamples = (int)(SampleRate * DecayMs / 1000.0);
            const int releaseSamples = (int)(SampleRate * ReleaseMs / 1000.0);
            
            var selectedWaves = WaveTypeXWaveMethod
                .Where(w => waveType.HasFlag(w.Key))
                .Select(w => w.Value)
                .ToList();

            for (var i = 0; i < noteSampleCount; i++)
            {
                var amplitude = GetAmplitudeForEnvelopeAdsr(i, decaySamples, noteSampleCount, releaseSamples);

                noteBuffer[i, channel] = (short)(selectedWaves.Sum(func => func(i, angleIncrement, samplesPerCycle, channelSettings[channel].dutyCycle)) * amplitude);

                if (fade && i >= noteSampleCount - FadeOutSamples)
                {
                    noteBuffer[i, channel] = AddFade(noteSampleCount, i, noteBuffer[i, channel]);
                }
            }

            for (var i = noteSampleCount; i < maxSampleCount; i++)
            {
                noteBuffer[i, channel] = 0;
            }
        }

        var interleavedBuffer = CreateInterleavedBuffer(maxSampleCount, channels, channelSampleCounts, noteBuffer);

        return interleavedBuffer.ToArray();
    }

    private static byte[] CreateInterleavedBuffer(int maxSampleCount, int channels, Dictionary<int, int> channelSampleCounts,
        short[,] noteBuffer)
    {
        var interleavedBuffer = new byte[maxSampleCount * channels * sizeof(short)];

        var index = 0;
        for (var i = 0; i < maxSampleCount; i++)
        {
            for (var ch = 0; ch < channels; ch++)
            {
                var sample = (i < channelSampleCounts[ch]) ? noteBuffer[i, ch] : (short)0;
                BitConverter.GetBytes(sample).CopyTo(interleavedBuffer, index);
                index += sizeof(short);
            }
        }

        return interleavedBuffer;
    }

    private static double GetAmplitudeForEnvelopeAdsr(int i, int decaySamples, int noteSampleCount, int releaseSamples)
    {
        double amplitude;
        if (i < decaySamples)
        {
            amplitude = 1.0 - ((1.0 - SustainLevel) * (i / (double)decaySamples));
        }
        else if (i < noteSampleCount - releaseSamples)
        {
            amplitude = SustainLevel;
        }
        else
        {
            amplitude = SustainLevel * (1.0 - ((i - (noteSampleCount - releaseSamples)) / (double)releaseSamples));
        }

        return amplitude;
    }

    private static short GetSineWaveBuffer(int i, double angleIncrement)
    {
        return (short)(Amplitude * Math.Sin(i * angleIncrement));
    }

    private static short GetSquareWaveBuffer(int i, int samplesPerCycle, double dutyCycle)
    {
        var threshold = (int)(samplesPerCycle * dutyCycle);
        return (i % samplesPerCycle < threshold) ? Amplitude : (short)-Amplitude;
    }

    private static short GetTriangleWaveBuffer(int i, int samplesPerCycle)
    {
        var cyclePosition = (double)(i % samplesPerCycle) / samplesPerCycle;

        if (cyclePosition < 0.5)
            return (short)(2 * Amplitude * cyclePosition - Amplitude);
        else
            return (short)(Amplitude - 2 * Amplitude * (cyclePosition - 0.5));
    }

    public static byte[] GetSilenceWaveBuffer(int pauseMs)
    {
        var pauseSampleCount = (int)((SampleRate * pauseMs) / 1000.0);
        var silenceBuffer = new short[pauseSampleCount];
        return silenceBuffer.SelectMany(BitConverter.GetBytes).ToArray();
    }

    private static short AddFade(int noteSampleCount, int i, short note)
    {
        var fadeFactor = (double)(noteSampleCount - i) / FadeOutSamples;
        return (short)(note * fadeFactor);
    }
}