
using Terraria.Net;

namespace Mint.Network.Incoming;

public class NetModuleHandler : IDataHandler
{
    internal List<PacketBindDelegate>?[] packetBinds = new List<PacketBindDelegate>?[32];

    public void Invoke(Player sender, Packet packet, ref bool ignore)
    {
        try
        {
            ushort packetId = BitConverter.ToUInt16(Net.buffer[packet.Sender].readBuffer, packet.Start + 1);

            if (packetId > packetBinds.Length - 1)
            {
                ignore = true;
                return;
            }

            Packet netPacket = new Packet((byte)packetId, packet.Sender, packet.Start + 2, packet.Length - 2);

            List<PacketBindDelegate>? binds = packetBinds[netPacket.PacketID];
            if (binds == null) return;

            foreach (PacketBindDelegate @delegate in binds)
                @delegate(sender, netPacket, ref ignore);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Exception in PacketHandler: ");
            Console.WriteLine(ex.ToString());

            Console.WriteLine("=== Additional information ===");
            Console.WriteLine($"PacketID={packet.PacketID}; Author={sender.Name ?? "UNKNOWN"} (ID: {sender.Index}); AuthorState={sender.PlayerState}");
            Console.ResetColor();

            ignore = true;
        }
    }

    public void Register(int packetId, PacketBindDelegate bind)
    {
        if (packetId > packetBinds.Length)
            throw new InvalidOperationException($"Cannot register PacketBind: invalid Packet ID! ({packetId})");

        if (packetBinds[packetId] == null) 
        {
            packetBinds[packetId] = new List<PacketBindDelegate>()
            {
                bind
            };
            return;
        }

        packetBinds[packetId]?.Add(bind);
    }

    public void Unregister(int packetId, PacketBindDelegate bind)
    {
        if (packetId > packetBinds.Length)
            throw new InvalidOperationException($"Cannot unregister PacketBind: invalid Packet ID! ({packetId})");

        packetBinds[packetId]?.Remove(bind);
    }
}