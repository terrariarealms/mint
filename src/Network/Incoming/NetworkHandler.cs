
namespace Mint.Network.Incoming;

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
        bool ignore = false;

        messageType = self.readBuffer[start];

        if (messageType < 0 || messageType > Terraria.ID.MessageID.Count)
            return;

        Player? player = MintServer.Players?.players[self.whoAmI];
        if (player == null)
            return;

        Packet packet = new Packet((byte)messageType, (byte)self.whoAmI, start, length);

        NetEvents.InvokeGetData(player, packet, ref ignore);

        if (messageType == 82) netModules.Invoke(player, packet, ref ignore);
        else packets.Invoke(player, packet, ref ignore);

        if (ignore)
            return;

        orig(self, start, length, out messageType);
    }
}