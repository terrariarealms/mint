namespace Mint.Server;

public interface ICharacter
{    
    /// <summary>
    /// Invokes when someone requests slot change operation.
    /// </summary>
    public static event CharacterSlotChangeEvent? OnSlotChange;
    protected static void InvokeSlotChange(ICharacter character, ref int slot, ref NetItem item, CharacterOperation operationType, ref bool ignore)
    {
        if (character != character.BasePlayer.Character)
            return;

        OnSlotChange?.Invoke(character, ref slot, ref item, operationType, ref ignore);
    }

    /// <summary>
    /// Invokes when someone requests stats change operation.
    /// </summary>
    public static event CharacterStatsChangeEvent? OnStatsChange;
    protected static void InvokeStatsChange(ICharacter character, ref CharacterStats stats, CharacterOperation operationType, ref bool ignore)
    {
        if (character != character.BasePlayer.Character)
            return;

        OnStatsChange?.Invoke(character, ref stats, operationType, ref ignore);
    }

    /// <summary>
    /// Invokes when someone requests life change operation.
    /// </summary>
    public static event CharacterLifeChangeEvent? OnLifeChange;
    protected static void InvokeLifeChange(ICharacter character, ref int life, CharacterOperation operationType, ref bool ignore)
    {
        if (character != character.BasePlayer.Character)
            return;

        OnLifeChange?.Invoke(character, ref life, operationType, ref ignore);
    }

    /// <summary>
    /// Invokes when someone requests mana change operation.
    /// </summary>
    public static event CharacterManaChangeEvent? OnManaChange;
    protected static void InvokeManaChange(ICharacter character, ref int mana, CharacterOperation operationType, ref bool ignore)
    {
        if (character != character.BasePlayer.Character)
            return;

        OnManaChange?.Invoke(character, ref mana, operationType, ref ignore);
    }

    public Player BasePlayer { get; }

    /// <summary>
    /// IEnumerable of slots.
    /// </summary>
    public IEnumerable<NetItem> Slots { get; }
    
    /// <summary>
    /// Character stats. (difficulty, visuals and other)
    /// </summary>
    public CharacterStats Stats { get; }

    /// <summary>
    /// Character max life.
    /// </summary>
    public int MaxLife { get; }

    /// <summary>
    /// Character max mana.
    /// </summary>
    public int MaxMana { get; }

    /// <summary>
    /// Set slot on target index.
    /// </summary>
    /// <param name="slot">Slot index</param>
    /// <param name="item">Item</param>
    /// <param name="operationType">Operation type</param>
    public void SetSlot(int slot, NetItem item, CharacterOperation operationType);

    /// <summary>
    /// Set character stats.
    /// </summary>
    /// <param name="stats">Stats</param>
    /// <param name="operationType">Operation type</param>
    public void SetStats(CharacterStats stats, CharacterOperation operationType);

    /// <summary>
    /// Set max life.
    /// </summary>
    /// <param name="maxLife">Max life</param>
    /// <param name="operationType">Operation type</param>
    public void SetLife(int maxLife, CharacterOperation operationType);

    /// <summary>
    /// Set max mana
    /// </summary>
    /// <param name="maxMana">Max mana</param>
    /// <param name="operationType">Operation type</param>
    public void SetMana(int maxMana, CharacterOperation operationType);
}