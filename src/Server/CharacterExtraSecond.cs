namespace Mint.Server;

[Flags]
public enum CharacterExtraSecond : byte
{
    UsedAegisCrystal = 0,
    UsedAegisFruit = 1,
    UsedArcaneCrystal = 2,
    UsedGalaxyPearl = 4,
    UsedGummyWorm = 8,
    UsedAmbrosia = 16,
    AteArtisanBread = 32
}