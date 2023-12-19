namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    static void OnPlayerLife(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();
        
        reader.ReadByte();

        short current = reader.ReadInt16();
        short max = reader.ReadInt16();

        if (!player.IgnoreAnticheat && (current > max || max % 20 != 0 || max > 500))
        {
            Log.Error("{Name} cheat detect in {Where}", player.Account?.Name, "OnPlayerLife");
            player.CloseConnection();
            return;
        }

        player.TPlayer.statLife = current;
        player.TPlayer.statLifeMax = max;
        player.TPlayer.statLifeMax2 = max;

        player.Character.SetLife(current, CharacterOperation.RequestedByPlayer);
    }
}