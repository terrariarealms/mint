namespace Mint.Server;

[Flags]
public enum PlayerMiscSecond : byte
{
    Sleeping = 1,
    AutoReuseAllWeapons = 2,
    ControlDownHold = 4,
    OperatingAnotherEntity = 8,
    ControlUseTile = 8
}