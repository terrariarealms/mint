namespace Mint.Server;

public delegate void CharacterManaChangeEvent(ICharacter character, ref int mana, CharacterOperation operationType, ref bool ignore);