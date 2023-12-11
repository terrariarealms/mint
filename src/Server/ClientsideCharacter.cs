
using MongoDB.Driver.Linq;

namespace Mint.Server;

public class ClientsideCharacter : ICharacter
{
    public ClientsideCharacter(Player player)
    {
        _basePlayer = player;
        _slots = new NetItem[350];
        _stats = new CharacterStats();
        _maxHp = 100;
        _maxMp = 20;
    }

    public ClientsideCharacter(Player player, NetItem[] slots, CharacterStats stats, int maxLife, int maxMana)
    {
        _basePlayer = player;
        _slots = slots;
        _stats = stats;
        _maxHp = maxLife;
        _maxMp = maxMana;
    }

    private Player _basePlayer;
    private NetItem[] _slots;
    private CharacterStats _stats;
    private int _maxHp;
    private int _maxMp;


    public IEnumerable<NetItem> Slots => _slots;

    public CharacterStats Stats => _stats;

    public int MaxLife => _maxHp;

    public int MaxMana => _maxMp;

    public void SetLife(int maxLife, CharacterOperation operationType)
    {
        _maxHp = maxLife;
        CharacterUtils.SetLife(_basePlayer, maxLife, false, operationType == CharacterOperation.RequestedByServer ? -1 : _basePlayer.Index);
    }

    public void SetMana(int maxMana, CharacterOperation operationType)
    {
        _maxMp = maxMana;
        CharacterUtils.SetMana(_basePlayer, maxMana, false, operationType == CharacterOperation.RequestedByServer ? -1 : _basePlayer.Index);
    }

    public void SetSlot(int slot, NetItem item, CharacterOperation operationType)
    {
        _slots[slot] = item;
        CharacterUtils.SetSlot(_basePlayer, slot, item, false, operationType == CharacterOperation.RequestedByServer ? -1 : _basePlayer.Index);
    }

    public void SetStats(CharacterStats stats, CharacterOperation operationType)
    {
        _stats = stats;
        CharacterUtils.SetDifficulty(_basePlayer, stats.Difficulty, true);
        CharacterUtils.SetExtraFirst(_basePlayer, stats.ExtraFirst, true);
        CharacterUtils.SetExtraSecond(_basePlayer, stats.ExtraSecond, true);
        CharacterUtils.SetVisuals(_basePlayer, stats.Visuals, false, operationType == CharacterOperation.RequestedByServer ? -1 : _basePlayer.Index);
    }
}