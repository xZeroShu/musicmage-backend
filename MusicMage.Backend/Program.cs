using MusicMage.Backend.Enums;
using MusicMage.Backend.Helpers;
using NAudio.Wave;

var filePath = Path.Combine(Environment.CurrentDirectory, "notes.wav");
using var fileStream = new FileStream(filePath, FileMode.Create);
using var writer = new WaveFileWriter(fileStream, new WaveFormat(NoteHelper.SampleRate, 16, 2));

List<Dictionary<int, Note>> notes =
[
    new()
    {
        { 0, new Note("E", 4, 1/8f, 120)},
        { 1, new Note("C", 4, 1/8f, 120)},
    },
    new()
    {
        { 0, new Note("E", 4, 1/8f, 120)},
        { 1, new Note("C", 4, 1/8f, 120)},
    },
    new()
    {
        { 0, new Note("E", 4, 1/8f, 120)},
        { 1, new Note("C", 4, 1/8f, 120)},
    },
    new()
    {
        { 0, new Note("C", 4, 1/8f, 120)},
        { 1, new Note("G", 3, 1/8f, 120)},
    },
    new()
    {
        { 0, new Note("E", 4, 1/8f, 120)},
        { 1, new Note("C", 4, 1/8f, 120)},
    },
    new()
    {
        { 0, new Note("G", 4, 1/4F, 120)},
        { 1, new Note("E", 4, 1/4F, 120)},
    },
    new()
    {
        { 0, new Note("G", 3, 1/4F, 120)},
        { 1, new Note("E", 3, 1/4F, 120)},
    },
];

foreach (var note in notes)
{
    Dictionary<int, (double frequency, float durationMs, WaveType waveType, double dutyCycle)> channelSettings = new();
    foreach (var channel in note.Keys)
    {
        var noteFrequency = NoteHelper.GetNoteFrequency(note[channel].NoteName, note[channel].Octave);
        channelSettings[channel] = (noteFrequency, note[channel].DurationMs, WaveType.Square, 0.5);
    }
    var waveBuffer = NoteHelper.GetWaveBuffer(channelSettings);
    // var silenceBuffer = NoteHelper.GetSilenceWaveBuffer(20);
    writer.Write(waveBuffer, 0, waveBuffer.Length);
    // writer.Write(silenceBuffer, 0, silenceBuffer.Length);
}

public class Note(string noteName, int octave, float beats, int beatsPerMinute)
{
    public string NoteName { get; } = noteName;
    public int Octave { get; } = octave;
    public float DurationMs { get; } = GetDurationInMilliseconds(beats, beatsPerMinute);

    private static float GetDurationInMilliseconds(float beats, int beatsPerMinute)
    {
        return beats * 4 * 60 * 1000 / beatsPerMinute;
    }
}