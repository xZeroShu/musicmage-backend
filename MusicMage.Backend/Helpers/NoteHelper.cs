using MusicMage.Backend.Enums;

namespace MusicMage.Backend.Helpers;

public static class NoteHelper
{
    public const int SampleRate = 44100; // Standard audio sample rate
    private const double FadeOutDuration = 0.02;
    private const short Amplitude = 16383; // short.MaxValue / 2
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

    public static byte[] GetWaveBuffer(double frequency, float durationMs, WaveType waveTypes, double dutyCycle = 0.5, bool fade = true)
    {
        var noteSampleCount = (int)(SampleRate * durationMs / 1000.0);
        var noteBuffer = new short[noteSampleCount];
        var samplesPerCycle = (int)(SampleRate / frequency);

        var angleIncrement = 2.0 * Math.PI * frequency / SampleRate;

        var selectedWaves = WaveTypeXWaveMethod
            .Where(w => waveTypes.HasFlag(w.Key))
            .Select(w => w.Value)
            .ToList();

        for (var i = 0; i < noteSampleCount; i++)
        {
            noteBuffer[i] = (short)selectedWaves.Sum(func => func(i, angleIncrement, samplesPerCycle, dutyCycle));

            if (!fade) continue;
            if (i < noteSampleCount - FadeOutSamples) continue;
            noteBuffer[i] = AddFade(noteSampleCount, i, noteBuffer[i]);
        }

        return noteBuffer.SelectMany(BitConverter.GetBytes).ToArray();
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