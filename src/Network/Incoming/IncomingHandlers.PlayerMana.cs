namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    static void OnPlayerMana(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();
        
        reader.ReadByte();

        short current = reader.ReadInt16();
        short max = reader.ReadInt16();

        if (!player.IgnoreAnticheat && (current > max || max % 20 != 0 || max > 200))
        {
            Log.Error("{Name} cheat detect in {Where}", player.Account?.Name, "OnPlayerMana");
            player.CloseConnection();
            return;
        }

        player.TPlayer.statMana = current;
        player.TPlayer.statManaMax = max;
        player.TPlayer.statManaMax2 = max;

        player.Character.SetMana(current, CharacterOperation.RequestedByPlayer);
    }
}