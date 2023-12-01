
namespace Mint.Network;

public class NetworkHandler
{
    public IDataHandler NetModules => netModules;
    public IDataHandler Packets => packets;

    internal NetModuleHandler netModules = new NetModuleHandler();
    internal PacketHandler packets = new PacketHandler();

    internal void Initialize()
    {
        On.Terraria.MessageBuffer.GetData += OnGetData;
    }

    private void OnGetData(On.Terraria.MessageBuffer.orig_GetData orig, Terraria.MessageBuffer self, int start, int length, out int messageType)
    {
        bool handled = false;

        messageType = self.readBuffer[start];

        if (messageType < 0 || messageType > Terraria.ID.MessageID.Count)
            return;

        Player? player = MintServer.Players?.players[self.whoAmI];
        if (player == null)
            return;

        Packet packet = new Packet((byte)messageType, (byte)self.whoAmI, start, length);

        if (messageType == 82) netModules.Invoke(player, packet, ref handled);
        else packets.Invoke(player, packet, ref handled);

        if (handled)
            return;

        orig(self, start, length, out messageType);
    }
}