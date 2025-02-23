using MusicMage.Backend.Helpers;

namespace MusicMage.Backend.Tests;

public class NoteHelperTest
{
    [Fact]
    public void GetNoteFrequency_ShouldReturnCorrectFrequency()
    { 
        Dictionary<string, float> noteXFrequency = new()
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
        foreach (var note in noteXFrequency)
        {
            NoteHelper.GetNoteFrequency(note.Key);
            Assert.Equal(note.Value, noteXFrequency[note.Key]);

            const int octave = 1;
            NoteHelper.GetNoteFrequency(note.Key, octave);
            Assert.Equal(MathF.Pow(note.Value, octave), noteXFrequency[note.Key]);
        }
    }
}