namespace Mint.Server;

[Flags]
public enum CharacterExtraFirst : byte
{
    BiomeTorches = 0,
    HappyFunTorchTime = 1,
    UnlockedBiomeTorches = 2,
    UnlockedSuperCart = 4,
    UsedSuperCart = 8
}