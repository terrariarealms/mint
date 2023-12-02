
namespace Mint.Network.Incoming;

public class PacketHandler : IDataHandler
{
    internal List<PacketBindDelegate>?[] packetBinds = new List<PacketBindDelegate>?[Terraria.ID.MessageID.Count];

    public void Invoke(Player sender, Packet packet, ref bool ignore)
    {
        try
        {
            List<PacketBindDelegate>? binds = packetBinds[packet.PacketID];
            if (binds == null) return;

            foreach (PacketBindDelegate @delegate in binds)
                @delegate(sender, packet, ref ignore);
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