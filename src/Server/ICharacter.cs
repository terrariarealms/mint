namespace Mint.Server;

public interface ICharacter
{
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