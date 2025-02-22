using MusicMage.Backend.Enums;
using MusicMage.Backend.Helpers;
using NAudio.Wave;

List<string> noteList = ["C", "D", "E", "F", "G", "A", "B"];
var filePath = Path.Combine(Environment.CurrentDirectory, "notes.wav");
using var fileStream = new FileStream(filePath, FileMode.Create);
using var writer = new WaveFileWriter(fileStream, new WaveFormat(NoteHelper.SampleRate, 16, 1));
for (var i = 21; i < noteList.Count * 5; i++)
{
    var noteFrequency = NoteHelper.GetNoteFrequency(noteList[i % noteList.Count], (i / noteList.Count));
    var waveBuffer = NoteHelper.GetWaveBuffer(noteFrequency, 300, WaveType.Sine);
    var silenceBuffer = NoteHelper.GetSilenceWaveBuffer(20);
    writer.Write(waveBuffer, 0, waveBuffer.Length);
    writer.Write(silenceBuffer, 0, silenceBuffer.Length);
}