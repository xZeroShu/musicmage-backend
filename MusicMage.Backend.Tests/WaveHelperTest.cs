using MusicMage.Backend.Enums;
using MusicMage.Backend.Helpers;

namespace MusicMage.Backend.Tests;

public class WaveHelperTest
{
    [Fact]
    public void GetSilenceWaveBuffer_ShouldReturnCorrectLength()
    {
        const int pauseMs = 500;
        const int expectedLength = (int)(WaveHelper.SampleRate * pauseMs / 1000.0) * sizeof(short);

        var result = WaveHelper.GetSilenceWaveBuffer(pauseMs);

        Assert.NotNull(result);
        Assert.Equal(expectedLength, result.Length);
    }
    
    [Fact]
    public void GetWaveBuffer_ShouldReturnNonEmptyBuffer()
    {
        var channelSettings = new Dictionary<int, (double frequency, float durationMs, WaveType waveType, double dutyCycle)>
        {
            { 0, (440.0, 500.0f, WaveType.Sine, 0.5) },
            { 1, (880.0, 500.0f, WaveType.Square, 0.5) }
        };

        var result = WaveHelper.GetWaveBuffer(channelSettings, fade: true);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetWaveBuffer_ShouldHandleSingleChannel()
    {
        var channelSettings = new Dictionary<int, (double frequency, float durationMs, WaveType waveType, double dutyCycle)>
        {
            { 0, (440.0, 500.0f, WaveType.Sine, 0.5) }
        };

        var result = WaveHelper.GetWaveBuffer(channelSettings, fade: false);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetWaveBuffer_ShouldReturnDifferentResultsForFade()
    {
        var channelSettings = new Dictionary<int, (double frequency, float durationMs, WaveType waveType, double dutyCycle)>
        {
            { 0, (440.0, 500.0f, WaveType.Sine, 0.5) }
        };

        var bufferWithFade = WaveHelper.GetWaveBuffer(channelSettings, fade: true);
        var bufferWithoutFade = WaveHelper.GetWaveBuffer(channelSettings, fade: false);

        Assert.NotEqual(bufferWithFade, bufferWithoutFade);
    }
}