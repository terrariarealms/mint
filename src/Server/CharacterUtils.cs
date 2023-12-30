using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Mint.Server;

public static class CharacterUtils
{
    /// <summary>
    /// Set slot for player.
    /// </summary>
    /// <param name="slot">Slot index</param>
    /// <param name="netItem">NetItem</param>
    /// <param name="remoteClient">Remote client</param>
    /// <param name="ignoreClient">Ignore client</param>
    public static void SetSlot(Player player, int slot, NetItem netItem, bool quiet = false, int remoteClient = -1,
        int ignoreClient = -1)
    {
        var item = new Item();
        item.SetDefaults(netItem.ItemID);
        item.stack = netItem.ItemStack;
        item.prefix = netItem.ItemPrefix;

        if (slot >= PlayerItemSlotID.Loadout3_Dye_0)
            player.TPlayer.Loadouts[2].Dye[slot - PlayerItemSlotID.Loadout3_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout3_Armor_0)
            player.TPlayer.Loadouts[2].Armor[slot - PlayerItemSlotID.Loadout3_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout2_Dye_0)
            player.TPlayer.Loadouts[1].Dye[slot - PlayerItemSlotID.Loadout2_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout2_Armor_0)
            player.TPlayer.Loadouts[1].Armor[slot - PlayerItemSlotID.Loadout2_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout1_Dye_0)
            player.TPlayer.Loadouts[0].Dye[slot - PlayerItemSlotID.Loadout1_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout1_Armor_0)
            player.TPlayer.Loadouts[0].Armor[slot - PlayerItemSlotID.Loadout1_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Bank4_0)
            player.TPlayer.bank4.item[slot - PlayerItemSlotID.Bank4_0] = item;
        else if (slot >= PlayerItemSlotID.Bank3_0)
            player.TPlayer.bank3.item[slot - PlayerItemSlotID.Bank3_0] = item;
        else if (slot >= PlayerItemSlotID.TrashItem)
            player.TPlayer.trashItem = item;
        else if (slot >= PlayerItemSlotID.Bank2_0)
            player.TPlayer.bank2.item[slot - PlayerItemSlotID.Bank2_0] = item;
        else if (slot >= PlayerItemSlotID.Bank1_0)
            player.TPlayer.bank.item[slot - PlayerItemSlotID.Bank1_0] = item;
        else if (slot >= PlayerItemSlotID.MiscDye0)
            player.TPlayer.miscDyes[slot - PlayerItemSlotID.MiscDye0] = item;
        else if (slot >= PlayerItemSlotID.Misc0)
            player.TPlayer.miscEquips[slot - PlayerItemSlotID.Misc0] = item;
        else if (slot >= PlayerItemSlotID.Dye0)
            player.TPlayer.dye[slot - PlayerItemSlotID.Dye0] = item;
        else if (slot >= PlayerItemSlotID.Armor0)
            player.TPlayer.armor[slot - PlayerItemSlotID.Armor0] = item;
        else
            player.TPlayer.inventory[slot - PlayerItemSlotID.Inventory0] = item;

        if (!quiet)
            Net.TrySendData(5, remoteClient, ignoreClient, NetworkText.FromLiteral(item.Name), player.Index,
                slot,
                item.prefix);
    }

    /// <summary>
    /// Set max life for player.
    /// </summary>
    /// <param name="life"></param>
    public static void SetLife(Player plr, int life, bool quiet = false, int ignoreClient = -1)
    {
        plr.TPlayer.statLifeMax = life;
        plr.TPlayer.statLifeMax2 = life;

        if (!quiet)
            Net.SendData(16, -1, ignoreClient, NetworkText.Empty, plr.Index);
    }

    /// <summary>
    /// Set max mana for player.
    /// </summary>
    /// <param name="mana"></param>
    public static void SetMana(Player plr, int mana, bool quiet = false, int ignoreClient = -1)
    {
        plr.TPlayer.statManaMax = mana;
        plr.TPlayer.statManaMax = mana;

        if (!quiet)
            Net.SendData(42, -1, ignoreClient, NetworkText.Empty, plr.Index);
    }

    /// <summary>
    /// Set max mana for player.
    /// </summary>
    /// <param name="mana"></param>
    public static void SetVisuals(Player plr, CharacterVisuals visuals, bool quiet = false, int ignoreClient = -1)
    {
        plr.TPlayer.skinVariant = visuals.SkinVariant;
        plr.TPlayer.hair = visuals.Hair;
        plr.TPlayer.hairDye = visuals.HairDye;
        plr.TPlayer.hideVisibleAccessory = visuals.HideAccessories;
        plr.TPlayer.hideMisc = visuals.HideMisc;
        plr.TPlayer.hairColor = visuals.HairColor;
        plr.TPlayer.skinColor = visuals.SkinColor;
        plr.TPlayer.eyeColor = visuals.EyesColor;
        plr.TPlayer.shirtColor = visuals.ShirtColor;
        plr.TPlayer.underShirtColor = visuals.UndershirtColor;
        plr.TPlayer.pantsColor = visuals.PantsColor;
        plr.TPlayer.shoeColor = visuals.ShoesColor;

        if (!quiet)
            Net.SendData(4, -1, ignoreClient, NetworkText.Empty, plr.Index);
    }

    /// <summary>
    /// Set difficulty.
    /// </summary>
    /// <param name="flags">CharacterDifficulty</param>
    public static void SetDifficulty(Player plr, CharacterDifficulty flags, bool quiet = false, int ignoreClient = -1)
    {
        if (flags.HasFlag(CharacterDifficulty.Classic))
            plr.TPlayer.difficulty = 0;
        else if (flags.HasFlag(CharacterDifficulty.Medium))
            plr.TPlayer.difficulty = 1;
        else if (flags.HasFlag(CharacterDifficulty.Hard))
            plr.TPlayer.difficulty = 2;
        else if (flags.HasFlag(CharacterDifficulty.Journey))
            plr.TPlayer.difficulty = 3;

        plr.TPlayer.extraAccessory = flags.HasFlag(CharacterDifficulty.ExtraAccessory);

        if (!quiet)
            Net.SendData(4, -1, ignoreClient, NetworkText.Empty, plr.Index);
    }

    /// <summary>
    /// Set extra first.
    /// </summary>
    /// <param name="flags">CharacterExtraFirst</param>
    public static void SetExtraFirst(Player plr, CharacterExtraFirst flags, bool quiet = false, int ignoreClient = -1)
    {
        plr.TPlayer.UsingBiomeTorches = flags.HasFlag(CharacterExtraFirst.BiomeTorches);
        plr.TPlayer.happyFunTorchTime = flags.HasFlag(CharacterExtraFirst.HappyFunTorchTime);
        plr.TPlayer.unlockedBiomeTorches = flags.HasFlag(CharacterExtraFirst.UnlockedBiomeTorches);
        plr.TPlayer.unlockedSuperCart = flags.HasFlag(CharacterExtraFirst.UnlockedSuperCart);
        plr.TPlayer.enabledSuperCart = flags.HasFlag(CharacterExtraFirst.UsedSuperCart);

        if (!quiet)
            Net.SendData(4, -1, ignoreClient, NetworkText.Empty, plr.Index);
    }

    /// <summary>
    /// Set extra second.
    /// </summary>
    /// <param name="flags">CharacterExtraSecond</param>
    public static void SetExtraSecond(Player plr, CharacterExtraSecond flags, bool quiet = false, int ignoreClient = -1)
    {
        plr.TPlayer.usedAegisCrystal = flags.HasFlag(CharacterExtraSecond.UsedAegisCrystal);
        plr.TPlayer.usedAegisFruit = flags.HasFlag(CharacterExtraSecond.UsedAegisFruit);
        plr.TPlayer.usedArcaneCrystal = flags.HasFlag(CharacterExtraSecond.UsedArcaneCrystal);
        plr.TPlayer.usedGalaxyPearl = flags.HasFlag(CharacterExtraSecond.UsedGalaxyPearl);
        plr.TPlayer.usedGummyWorm = flags.HasFlag(CharacterExtraSecond.UsedGummyWorm);
        plr.TPlayer.usedAmbrosia = flags.HasFlag(CharacterExtraSecond.UsedAmbrosia);
        plr.TPlayer.ateArtisanBread = flags.HasFlag(CharacterExtraSecond.AteArtisanBread);

        if (!quiet)
            Net.SendData(4, -1, ignoreClient, NetworkText.Empty, plr.Index);
    }
}