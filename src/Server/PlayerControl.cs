namespace Mint.Server;

[Flags]
public enum PlayerControl : byte
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    Jump = 16,
    UseItem = 32,
    SetDirection = 64
}