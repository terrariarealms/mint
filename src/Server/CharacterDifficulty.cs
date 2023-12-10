namespace Mint.Server;

[Flags]
public enum CharacterDifficulty : byte
{
    Classic = 0,
    Medium = 1,
    Hard = 2,
    ExtraAccessory = 4,
    Journey = 8
}