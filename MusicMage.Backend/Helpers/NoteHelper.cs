namespace MusicMage.Backend.Helpers;

public static class NoteHelper
{
    public const int SampleRate = 44100; // Standard audio sample rate
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
        var noteSampleCount = (int)((SampleRate * durationMs) / 1000.0);
        
        var noteBuffer = new short[noteSampleCount];
        
        const double amplitude = 16383d; // short.MaxValue / 2
        var angleIncrement = 2.0 * Math.PI * frequency / SampleRate;

        for (var i = 0; i < noteSampleCount; i++)
        {
            noteBuffer[i] = (short)(amplitude * Math.Sin(i * angleIncrement));
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