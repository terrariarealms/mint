
namespace Mint.Server;

public partial class Player
{
    /// <summary>
    /// Does player have 'ignore anticheat' permission.
    /// </summary>
    public virtual bool IgnoreAnticheat => Group?.HasPermission("mint.admin.ignoreanticheat") == true;

    /// <summary>
    /// Player character.
    /// </summary>
    public virtual ICharacter Character { get; internal set; }  

    /// <summary>
    /// Is player dead.
    /// </summary>
    public virtual bool Dead => TPlayer.dead;

    /// <summary>
    /// Player current life. (not max life)
    /// </summary>
    public virtual int Life => TPlayer.statLife;

    /// <summary>
    /// Player current mana. (not max mana)
    /// </summary>
    public virtual int Mana => TPlayer.statMana;

    /// <summary>
    /// Player controls.
    /// </summary>
    public virtual PlayerControl Controls { get; internal set; }

    /// <summary>
    /// Player misc 1.
    /// </summary>
    public virtual PlayerMiscFirst MiscFirst { get; internal set; }

    /// <summary>
    /// Player misc 2.
    /// </summary>
    public virtual PlayerMiscSecond MiscSecond { get; internal set; }

    /// <summary>
    /// Player pulley.
    /// </summary>
    public virtual PlayerPulley Pulley { get; internal set; }
}