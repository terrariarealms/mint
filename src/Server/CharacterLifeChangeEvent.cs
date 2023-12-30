namespace Mint.Server;

public delegate void CharacterLifeChangeEvent(ICharacter character, ref int life, CharacterOperation operationType,
    ref bool ignore);