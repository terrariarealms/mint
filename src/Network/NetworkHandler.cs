
namespace Mint.Network;

internal class NetworkHandler
{
    internal List<PacketBindDelegate>?[] packetBinds = new List<PacketBindDelegate>?[Terraria.ID.MessageID.Count];

    internal void Initialize()
    {
        On.Terraria.MessageBuffer.GetData += OnGetData;
    }

    private void OnGetData(On.Terraria.MessageBuffer.orig_GetData orig, Terraria.MessageBuffer self, int start, int length, out int messageType)
    {
        bool handled = false;

        messageType = self.readBuffer[start];

        if (messageType > packetBinds.Length)
            return;
        
        Player? player = PlayersAPI.GetPlayer(self.whoAmI);
        if (player == null)
            return;

        Packet packet = new Packet((byte)messageType, (byte)self.whoAmI, start, length);

        List<PacketBindDelegate>? binds = packetBinds[messageType];
        binds?.ForEach((PacketBindDelegate @delegate) => @delegate(player, packet, ref handled));

        if (handled)
            return;

        orig(self, start, length, out messageType);
    }


    internal void Register(int packetId, PacketBindDelegate bind)
    {
        if (packetId > packetBinds.Length)
            throw new InvalidOperationException("Cannot register PacketBind: invalid Packet ID!");

        if (packetBinds[packetId] == null) 
            packetBinds[packetId] = new List<PacketBindDelegate>();

        packetBinds[packetId]?.Add(bind);
    }

    internal void Unregister(int packetId, PacketBindDelegate bind)
    {
        if (packetId > packetBinds.Length)
            throw new InvalidOperationException("Cannot register PacketBind: invalid Packet ID!");

        packetBinds[packetId]?.Remove(bind);
    }
}