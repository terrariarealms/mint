using Terraria;

namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    private static void OnPlayerSlot(Server.Player player, IncomingPacket packet, ref bool ignore)
    {
        var reader = packet.GetReader();

        reader.ReadByte();

        var slotId = reader.ReadInt16();
        var stack = reader.ReadInt16();
        var prefix = reader.ReadByte();
        var netId = reader.ReadInt16();

        var item = new Item();
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