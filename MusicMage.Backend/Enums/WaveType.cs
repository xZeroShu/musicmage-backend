namespace MusicMage.Backend.Enums;

[Flags]
public enum WaveType
{
    None = 0,
    Sine = 1,
    Square = 2,
    Triangle = 4,
    All = Sine | Square | Triangle
}