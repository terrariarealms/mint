namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    private static void OnPlayerUUID(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (player.SentPackets[68])
            return;

        player.SentPackets[68] = true;

        var reader = packet.GetReader();
        var uuid = reader.ReadString();

        player.UUID = uuid;

        if (!player.Authorized) player.AutoAuthorize();
    }
}