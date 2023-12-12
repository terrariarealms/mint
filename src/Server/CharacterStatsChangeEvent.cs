namespace Mint.Server;

public delegate void CharacterStatsChangeEvent(ICharacter character, ref CharacterStats stats, CharacterOperation operationType, ref bool ignore);