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

    public static float GetNoteFrequency(string note, int octave = 0)
    {
        var noteToReturn = NoteXFrequency.GetValueOrDefault(note, 0);
        return noteToReturn * MathF.Pow(2, octave);
    }
    
    public static byte[] GetSineWaveBuffer(double frequency, int durationMs)
    {
        var noteSampleCount = (int)(SampleRate * durationMs / 1000.0);
        var noteBuffer = new short[noteSampleCount];
        
        var angleIncrement = 2.0 * Math.PI * frequency / SampleRate;

        for (var i = 0; i < noteSampleCount; i++)
        {
            noteBuffer[i] = (short)(Amplitude * Math.Sin(i * angleIncrement));

            if (i < noteSampleCount - FadeOutSamples) continue;
            
            var fadeFactor = (double)(noteSampleCount - i) / FadeOutSamples;
            noteBuffer[i] = (short)(noteBuffer[i] * fadeFactor);
        }

        return noteBuffer.SelectMany(BitConverter.GetBytes).ToArray();
    }
    
    public static byte[] GetSquareWaveBuffer(double frequency, int durationMs)
    {
        var noteSampleCount = (int)(SampleRate * durationMs / 1000.0);
        var noteBuffer = new short[noteSampleCount];
        var samplesPerCycle = (int)(SampleRate / frequency);

        for (var i = 0; i < noteSampleCount; i++)
        {
            noteBuffer[i] = (i % samplesPerCycle < samplesPerCycle / 2) ? Amplitude : (short)-Amplitude;

            if (i < noteSampleCount - FadeOutSamples) continue;
            
            var fadeFactor = (double)(noteSampleCount - i) / FadeOutSamples;
            noteBuffer[i] = (short)(noteBuffer[i] * fadeFactor);
        }

        return noteBuffer.SelectMany(BitConverter.GetBytes).ToArray();
    }
    
    public static byte[] GetTriangleWaveBuffer(double frequency, int durationMs)
    {
        var noteSampleCount = (int)(SampleRate * durationMs / 1000.0);
        var noteBuffer = new short[noteSampleCount];
        var samplesPerCycle = (int)(SampleRate / frequency);

        for (var i = 0; i < noteSampleCount; i++)
        {
            var cyclePosition = (double)(i % samplesPerCycle) / samplesPerCycle;
        
            if (cyclePosition < 0.5)
                noteBuffer[i] = (short)(2 * Amplitude * cyclePosition - Amplitude);
            else
                noteBuffer[i] = (short)(Amplitude - 2 * Amplitude * (cyclePosition - 0.5));
            
            if (i < noteSampleCount - FadeOutSamples) continue;
            
            var fadeFactor = (double)(noteSampleCount - i) / FadeOutSamples;
            noteBuffer[i] = (short)(noteBuffer[i] * fadeFactor);
        }

        return noteBuffer.SelectMany(BitConverter.GetBytes).ToArray();
    }

    public static byte[] GetSilenceWaveBuffer(int pauseMs)
    {
        var pauseSampleCount = (int)((SampleRate * pauseMs) / 1000.0);
        var silenceBuffer = new short[pauseSampleCount];
        return silenceBuffer.SelectMany(BitConverter.GetBytes).ToArray();
    }
}