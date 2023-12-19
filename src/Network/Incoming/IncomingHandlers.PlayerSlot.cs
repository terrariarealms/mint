using Terraria;

namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    static void OnPlayerSlot(Server.Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();
        
        reader.ReadByte();

        short slotId = reader.ReadInt16();
        short stack = reader.ReadInt16();
        byte prefix = reader.ReadByte();
        short netId = reader.ReadInt16();
        
        Item item = new Item();
        item.netDefaults(netId);
        
        if (stack > item.maxStack)
        {
            Log.Error("{Name} stack cheat detect in {Where}", player.Account?.Name, "OnPlayerSlot");
            player.Character.SetSlot(slotId, new NetItem(0, 0, 0), CharacterOperation.RequestedByServer);
            return;
        }

        player.Character.SetSlot(slotId, new NetItem(netId, stack, prefix), CharacterOperation.RequestedByPlayer);
    }
}