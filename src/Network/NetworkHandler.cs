using System.Collections.Concurrent;
using Terraria.Localization;

namespace Mint.Network;

public sealed class NetworkHandler
{
    internal NetworkHandler(){}

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
    public readonly NetworkBindDelegate<IncomingPacket>?[] IncomingHijack = new NetworkBindDelegate<IncomingPacket>?[255];

    /// <summary>
    /// Outcoming packets hijacks.
    /// </summary>
    public readonly NetworkBindDelegate<IncomingPacket>?[] IncomingModulesHijack = new NetworkBindDelegate<IncomingPacket>?[32];

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
        ModGet.GetData += OnGetData;

        IncomingHandlers.Initialize();
    }

    private void OnGetData(ModGet.orig_GetData orig, Terraria.MessageBuffer self, int start, int length, out int messageType)
    {
        messageType = self.readBuffer[start];

        Player? player = MintServer.Players.players[self.whoAmI];
        if (player == null)
            return;

        if (messageType == 82) // net module
        {
            ushort netModuleId = BitConverter.ToUInt16(self.readBuffer, start + 1); // id
            start += 3;
            length -= 3;

            IncomingPacket packet = new IncomingPacket((byte)netModuleId, (byte)self.whoAmI, start, length);
            packet.CreateReader();
            HandleNetModule(orig, self, player, packet);
            packet.DisposeReader();
        }
        else // default packet
        {
            start += 1;
            length -= 1;

            IncomingPacket packet = new IncomingPacket((byte)messageType, (byte)self.whoAmI, start, length);
            packet.CreateReader();
            HandlePacket(orig, self, player, packet);
            packet.DisposeReader();
        }
    }

    private bool HandleNetModule(ModGet.orig_GetData orig, Terraria.MessageBuffer self, Player player, IncomingPacket packet)
    {
        bool handled = false;
        IncomingNetModules.binds[packet.PacketID]?.ForEach((p) => p?.Invoke(player, packet, ref handled));

        if (handled) return handled;

        var hijack = IncomingModulesHijack[packet.PacketID];

        if (hijack != null) hijack(player, packet, ref handled);
        else orig(self, packet.Start - 3, packet.Length + 3, out _);

        return handled;
    }

    private bool HandlePacket(ModGet.orig_GetData orig, Terraria.MessageBuffer self, Player player, IncomingPacket packet)
    {
        bool handled = false;
        IncomingPackets.binds[packet.PacketID]?.ForEach((p) => p?.Invoke(player, packet, ref handled));

        if (handled) return handled;
        
        var hijack = IncomingHijack[packet.PacketID];

        if (hijack != null) hijack(player, packet, ref handled);
        else orig(self, packet.Start - 1, packet.Length + 1, out _);

        return handled;
    }

    private void OnSendData(ModSend.orig_SendData orig, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        OutcomingPacket packet = new OutcomingPacket(msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7, orig);

        if (remoteClient == -1) BroadcastPackets.Add(packet);
        else MintServer.Players[remoteClient].OutcomingPackets.Add(packet);
    }

    private Player _nonePlayer = new DynamicPlayer();

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
                hijackDelegate?.Invoke(packet.RemoteClient != -1 ? MintServer.Players.players[packet.RemoteClient] : _nonePlayer, packet, ref ignore);

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