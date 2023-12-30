namespace Mint.Server;

public delegate void CharacterSlotChangeEvent(ICharacter character, ref int slot, ref NetItem item,
    CharacterOperation operationType, ref bool ignore);