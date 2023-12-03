using System.Collections.Concurrent;
using Terraria.Localization;

namespace Mint.Network;

public class NetworkHandler
{
    /// <summary>
    /// Incoming packets binder.
    /// </summary>
    public readonly NetworkBinder<IncomingPacket> IncomingPackets = new NetworkBinder<IncomingPacket>(255);

    /// <summary>
    /// Incoming net modules binder.
    /// </summary>
    public readonly NetworkBinder<IncomingPacket> IncomingNetModules = new NetworkBinder<IncomingPacket>(32);

    /// <summary>
    /// Outcoming packets hijacks.
    /// </summary>
    public readonly NetworkBindDelegate<OutcomingPacket>?[] OutcomingHijack = new NetworkBindDelegate<OutcomingPacket>?[255];

    /// <summary>
    /// Packets that requested to broadcast.
    /// </summary>
    public readonly BlockingCollection<OutcomingPacket> BroadcastPackets = new BlockingCollection<OutcomingPacket>();

    internal Task? PacketHandlerTask;

    internal void Initialize()
    {
        PacketHandlerTask = new Task(PacketHandler);
        PacketHandlerTask.Start();

        ModSend.SendData += OnSendData;
    }

    private void OnSendData(ModSend.orig_SendData orig, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        OutcomingPacket packet = new OutcomingPacket(msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7, orig);

        if (remoteClient == -1) BroadcastPackets.Add(packet);
        else MintServer.Players[remoteClient].OutcomingPackets.Add(packet);
    }

    // ctrl c + ctrl v issue
    private void PacketHandler()
    {
        while (true)
        {
            try
            {
                bool ignore = false;

                OutcomingPacket packet = BroadcastPackets.Take();
                NetworkBindDelegate<OutcomingPacket>? hijackDelegate = MintServer.Network.OutcomingHijack[packet.PacketID];
                hijackDelegate?.Invoke(null, packet, ref ignore);

                if (!ignore)
                    packet.Original(packet.PacketID, 
                                    packet.RemoteClient, 
                                    packet.IgnoreClient, 
                                    packet.Text, 
                                    packet.Number0, 
                                    packet.Number1,
                                    packet.Number2, 
                                    packet.Number3, 
                                    packet.Number4, 
                                    packet.Number5, 
                                    packet.Number6);
                                    
            }
            // when NetToken was cancelled we got operation canceled exception from blocking collection.
            catch (OperationCanceledException)
            {}
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Player.Network: PacketHandler:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}