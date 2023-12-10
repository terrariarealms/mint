using MongoDB.Driver.Linq;
using Terraria.Localization;

namespace Mint.Server;

public class PlayerCharacter
{
    public PlayerCharacter(Player basePlayer, int maxLife, int maxMana, NetItem[] slots)
    {
        BasePlayer = basePlayer;
        MaxLife = maxLife;
        MaxMana = maxMana;
        Slots = new PlayerSlotCollection(basePlayer, slots);
    }

    public PlayerCharacter(Player basePlayer)
    {
        BasePlayer = basePlayer;
        MaxLife = 100;
        MaxMana = 100;
        Slots = new PlayerSlotCollection(basePlayer);
    }

    internal bool PendingSave;

    /// <summary>
    /// Base player.
    /// </summary>
    public Player BasePlayer { get; }

    /// <summary>
    /// Player max life count.
    /// </summary>
    public int MaxLife { get; private set; }

    /// <summary>
    /// Player max mana count.
    /// </summary>
    public int MaxMana  { get; private set; }

    /// <summary>
    /// Player slots.
    /// </summary>
    public PlayerSlotCollection Slots { get; }

    /// <summary>
    /// Player visuals.
    /// </summary>
    public PlayerVisuals Visuals { get; private set; }

    /// <summary>
    /// Set max life for player.
    /// </summary>
    /// <param name="life"></param>
    public void SetLife(int life, bool quiet = false)
    {
        PendingSave = true;

        MaxLife = life;

        BasePlayer.TPlayer.statLifeMax = life;
        BasePlayer.TPlayer.statLifeMax2 = life;

        if (!quiet)
        Net.SendData(16, -1, -1, NetworkText.Empty, BasePlayer.Index);
    }

    /// <summary>
    /// Set max mana for player.
    /// </summary>
    /// <param name="mana"></param>
    public void SetMana(int mana, bool quiet = false)
    {
        PendingSave = true;

        MaxMana = mana;

        BasePlayer.TPlayer.statManaMax = mana;
        BasePlayer.TPlayer.statManaMax = mana;

        if (!quiet)
        Net.SendData(42, -1, -1, NetworkText.Empty, BasePlayer.Index);
    }

    /// <summary>
    /// Set max mana for player.
    /// </summary>
    /// <param name="mana"></param>
    public void SetVisuals(PlayerVisuals visuals, bool quiet = false)
    {
        PendingSave = true;

        Visuals = visuals;

        BasePlayer.TPlayer.skinVariant = visuals.SkinVariant;
        BasePlayer.TPlayer.hair = visuals.Hair;
        BasePlayer.TPlayer.hairDye = visuals.HairDye;
        BasePlayer.TPlayer.hideVisibleAccessory = visuals.HideAccessories;
        BasePlayer.TPlayer.hideMisc = visuals.HideMisc;
        BasePlayer.TPlayer.hairColor = visuals.HairColor;
        BasePlayer.TPlayer.skinColor = visuals.SkinColor;
        BasePlayer.TPlayer.eyeColor = visuals.EyesColor;
        BasePlayer.TPlayer.shirtColor = visuals.ShirtColor;
        BasePlayer.TPlayer.underShirtColor = visuals.UndershirtColor;
        BasePlayer.TPlayer.pantsColor = visuals.PantsColor;
        BasePlayer.TPlayer.shoeColor = visuals.ShoesColor;

        if (!quiet)
        Net.SendData(4, -1, -1, NetworkText.Empty, BasePlayer.Index);
    }

    /// <summary>
    /// Set difficulty.
    /// </summary>
    /// <param name="flags">CharacterDifficulty</param>
    public void SetDifficulty(CharacterDifficulty flags, bool quiet = false)
    {
        if (flags.HasFlag(CharacterDifficulty.Classic))
            BasePlayer.TPlayer.difficulty = 0;
        else if (flags.HasFlag(CharacterDifficulty.Medium))
            BasePlayer.TPlayer.difficulty = 1;
        else if (flags.HasFlag(CharacterDifficulty.Hard))
            BasePlayer.TPlayer.difficulty = 2;
        else if (flags.HasFlag(CharacterDifficulty.Journey))
            BasePlayer.TPlayer.difficulty = 3;

        BasePlayer.TPlayer.extraAccessory = flags.HasFlag(CharacterDifficulty.ExtraAccessory);

        if (!quiet)
        Net.SendData(4, -1, -1, NetworkText.Empty, BasePlayer.Index);
    }

    /// <summary>
    /// Set extra first.
    /// </summary>
    /// <param name="flags">CharacterExtraFirst</param>
    public void SetExtraFirst(CharacterExtraFirst flags, bool quiet = false)
    {
        BasePlayer.TPlayer.UsingBiomeTorches = flags.HasFlag(CharacterExtraFirst.BiomeTorches);
        BasePlayer.TPlayer.happyFunTorchTime = flags.HasFlag(CharacterExtraFirst.HappyFunTorchTime);
        BasePlayer.TPlayer.unlockedBiomeTorches = flags.HasFlag(CharacterExtraFirst.UnlockedBiomeTorches);
        BasePlayer.TPlayer.unlockedSuperCart = flags.HasFlag(CharacterExtraFirst.UnlockedSuperCart);
        BasePlayer.TPlayer.enabledSuperCart = flags.HasFlag(CharacterExtraFirst.UsedSuperCart);

        if (!quiet)
        Net.SendData(4, -1, -1, NetworkText.Empty, BasePlayer.Index);
    }

    /// <summary>
    /// Set extra second.
    /// </summary>
    /// <param name="flags">CharacterExtraSecond</param>
    public void SetExtraSecond(CharacterExtraSecond flags, bool quiet = false)
    {
        BasePlayer.TPlayer.usedAegisCrystal = flags.HasFlag(CharacterExtraSecond.UsedAegisCrystal);
        BasePlayer.TPlayer.usedAegisFruit = flags.HasFlag(CharacterExtraSecond.UsedAegisFruit);
        BasePlayer.TPlayer.usedArcaneCrystal = flags.HasFlag(CharacterExtraSecond.UsedArcaneCrystal);
        BasePlayer.TPlayer.usedGalaxyPearl = flags.HasFlag(CharacterExtraSecond.UsedGalaxyPearl);
        BasePlayer.TPlayer.usedGummyWorm = flags.HasFlag(CharacterExtraSecond.UsedGummyWorm);
        BasePlayer.TPlayer.usedAmbrosia = flags.HasFlag(CharacterExtraSecond.UsedAmbrosia);
        BasePlayer.TPlayer.ateArtisanBread = flags.HasFlag(CharacterExtraSecond.AteArtisanBread);

        if (!quiet)
        Net.SendData(4, -1, -1, NetworkText.Empty, BasePlayer.Index);
    }
}