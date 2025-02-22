using MusicMage.Backend.Enums;
using MusicMage.Backend.Helpers;
using NAudio.Wave;

List<string> noteList = ["C", "D", "E", "F", "G", "A", "B"];
var filePath = Path.Combine(Environment.CurrentDirectory, "notes.wav");
using var fileStream = new FileStream(filePath, FileMode.Create);
using var writer = new WaveFileWriter(fileStream, new WaveFormat(NoteHelper.SampleRate, 16, 2));
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
    new("E", "A", 4, 1/8f, 120),
    new("E", "B", 4, 1/8f, 120),
    new("E", "C", 4, 1/8f, 120),
    new("C", "B", 4, 1/8f, 120),
    new("E", "C", 4, 1/8f, 120),
    new("G", "A", 4, 1/4f, 120),
    new("G", "B", 3, 1/4f, 120)
];
foreach (var note in superMarioNoteList)
{
    var noteFrequencyCh1 = NoteHelper.GetNoteFrequency(note.NoteNameCh1, note.Octave);
    var noteFrequencyCh2 = NoteHelper.GetNoteFrequency(note.NoteNameCh2, note.Octave);
    Console.WriteLine($"Note Frequency: {noteFrequencyCh1}");
    Console.WriteLine($"DurationMs: {note.DurationMs}");
    // var waveBuffer = NoteHelper.GetWaveBuffer(noteFrequency, note.DurationMs, WaveType.Square | WaveType.Triangle, 1);
    Dictionary<int, (double frequency, float durationMs, WaveType waveType, double dutyCycle)> channelSettings = new()
    {
        { 0, (noteFrequencyCh1, note.DurationMs, WaveType.Square | WaveType.Triangle, 1) },
        { 1, (noteFrequencyCh2, note.DurationMs, WaveType.Square, 0.5) },
    };
    var waveBuffer = NoteHelper.GetWaveBuffer(channelSettings);
    var silenceBuffer = NoteHelper.GetSilenceWaveBuffer(20);
    writer.Write(waveBuffer, 0, waveBuffer.Length);
    writer.Write(silenceBuffer, 0, silenceBuffer.Length);
}

public class Note(string noteNameCh1, string noteNameCh2, int octave, float beats, int beatsPerMinute)
{
    public string NoteNameCh1 { get; private set; } = noteNameCh1;
    public string NoteNameCh2 { get; private set; } = noteNameCh2;
    public int Octave { get; private set; } = octave;
    public float DurationMs { get; private set; } = GetDurationInMilliseconds(beats, beatsPerMinute);

    private static float GetDurationInMilliseconds(float beats, int beatsPerMinute)
    {
        return beats * 4 * 60 * 1000 / beatsPerMinute;
    }
}