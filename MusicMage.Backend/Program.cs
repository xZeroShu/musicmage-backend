using MusicMage.Backend.Enums;
using MusicMage.Backend.Helpers;
using NAudio.Wave;

List<string> noteList = ["C", "D", "E", "F", "G", "A", "B"];
var filePath = Path.Combine(Environment.CurrentDirectory, "notes.wav");
using var fileStream = new FileStream(filePath, FileMode.Create);
using var writer = new WaveFileWriter(fileStream, new WaveFormat(NoteHelper.SampleRate, 16, 1));
// for (var i = 21; i < noteList.Count * 5; i++)
// {
//     var noteFrequency = NoteHelper.GetNoteFrequency(noteList[i % noteList.Count], (i / noteList.Count));
//     var waveBuffer = NoteHelper.GetWaveBuffer(noteFrequency, 300, WaveType.Square | WaveType.Triangle | WaveType.Sine);
//     var silenceBuffer = NoteHelper.GetSilenceWaveBuffer(20);
//     writer.Write(waveBuffer, 0, waveBuffer.Length);
//     writer.Write(silenceBuffer, 0, silenceBuffer.Length);
// }

List<Note> superMarioNoteList =
[
    new("E", 4, 1/8f, 120),
    new("E", 4, 1/8f, 120),
    new("E", 4, 1/8f, 120),
    new("C", 4, 1/8f, 120),
    new("E", 4, 1/8f, 120),
    new("G", 4, 1/4f, 120),
    new("G", 3, 1/4f, 120)
];
foreach (var note in superMarioNoteList)
{
    var noteFrequency = NoteHelper.GetNoteFrequency(note.NoteName, note.Octave);
    Console.WriteLine($"Note Frequency: {noteFrequency}");
    Console.WriteLine($"DurationMs: {note.DurationMs}");
    var waveBuffer = NoteHelper.GetWaveBuffer(noteFrequency, note.DurationMs, WaveType.Square | WaveType.Triangle, 1);
    var silenceBuffer = NoteHelper.GetSilenceWaveBuffer(20);
    writer.Write(waveBuffer, 0, waveBuffer.Length);
    writer.Write(silenceBuffer, 0, silenceBuffer.Length);
}

public class Note(string noteName, int octave, float beats, int beatsPerMinute)
{
    public string NoteName { get; private set; } = noteName;
    public int Octave { get; private set; } = octave;
    public float DurationMs { get; private set; } = GetDurationInMilliseconds(beats, beatsPerMinute);

    private static float GetDurationInMilliseconds(float beats, int beatsPerMinute)
    {
        return beats * 4 * 60 * 1000 / beatsPerMinute;
    }
}