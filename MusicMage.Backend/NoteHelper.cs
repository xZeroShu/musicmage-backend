namespace MusicMage.Backend;

public static class NoteHelper
{
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
}