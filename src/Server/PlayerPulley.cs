namespace Mint.Server;

[Flags]
public enum PlayerPulley : byte
{
    None = 0,
    Enabled = 1,
    SetDirection = 2,
    SetVelocity = 4,
    SetVortexStealth = 8,
    SetGravityDirection = 16,
    SetShieldRaised = 32,
    SetGhost = 64
}