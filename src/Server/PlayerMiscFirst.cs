namespace Mint.Server;

[Flags]
public enum PlayerMiscFirst : byte
{
    None = 0,
    SetHoveringUp = 1,
    SetVoidVaultActive = 2,
    SetSitting = 4,
    SetDD2Victory = 8,
    SetInteractionAnimal = 16,
    SetInteractionSmallAnimal = 32,
    SetReturnPotion = 64,
    SetHoveringDown = 128
}