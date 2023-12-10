using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Mint.Server;

public class PlayerSlotCollection
{
    public PlayerSlotCollection(Player player)
    {
        this.player = player;
        slots = new NetItem[350];
    }
    public PlayerSlotCollection(Player player, NetItem[] slots)
    {
        this.player = player;
        this.slots = slots;
    }

    internal Player player;
    internal NetItem[] slots;

    /// <summary>
    /// Get or set slot.
    /// </summary>
    /// <param name="index">Slot index</param>
    /// <returns>NetItem</returns>
    public NetItem this[int index]
    {
        get => slots[index];
        set => SetSlot(index, value); 
    }

    /// <summary>
    /// Sets slot.
    /// </summary>
    /// <param name="slot">Slot index</param>
    /// <param name="netItem">NetItem</param>
    /// <param name="remoteClient">Remote client</param>
    /// <param name="ignoreClient">Ignore client</param>
    public void SetSlot(int slot, NetItem netItem, int remoteClient = -1, int ignoreClient = -1)
    {
        slots[slot] = netItem;

        Item item = new Item();
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

        NetMessage.TrySendData(5, remoteClient, ignoreClient, NetworkText.FromLiteral(item.Name), player.Index,
            slot,
            item.prefix);
    }
}