using Terraria.Localization;
using Terraria;
using Terraria.Net.Sockets;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Mint.Server;

public partial class Player
{
    /// <summary>   
    /// Player socket.
    /// </summary>
    public virtual ISocket Socket => Netplay.Clients[Index].Socket;

    /// <summary>
    /// Outcoming packets blocking collection. Using for async packet sending.
    /// You can add OutcomingPacket here for directly packet sending.
    /// </summary>
    public virtual BlockingCollection<OutcomingPacket> OutcomingPackets { get; } = new BlockingCollection<OutcomingPacket>();

    /// <summary>
    /// Sent packets id array from player.
    /// </summary>
    public virtual bool[] SentPackets { get; } = new bool[PacketID.Count];

    /// <summary>
    /// Player IP Address.
    /// You can change it if your server using Dimensions or something.
    /// </summary>
    public virtual string IP 
    {
        get => _ip ?? Socket.GetRemoteAddress()?.ToString()?.Split(':')[0] ?? "0.0.0.0";
        set => _ip = value;
    }

    public virtual void CloseConnection()
    {
        Messenger.Send(MessageMark.Error, "System", "Your connection with server was closed.");
        StopPacketHandler();
        StopCommandHandler();
        Socket.Close();
    }

    private string? _ip;

    internal CancellationTokenSource? netTokenSource;
    internal CancellationToken? netToken;
    internal Task? packetHandlerTask;

    /// <summary>
    /// Send packet bytes to client.
    /// </summary>
    /// <param name="bytes">Packet bytes</param>
    public virtual void SendBytes(byte[] bytes)
    {
        Socket.AsyncSend(bytes, 0, bytes.Length, RemoteClient.ServerWriteCallBack);
    }

    /// <summary>
    /// Send packet to client.
    /// </summary>
    /// <param name="packetId">Packet ID</param>
    /// <param name="text">Network text</param>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <param name="num3"></param>
    /// <param name="num4"></param>
    /// <param name="num5"></param>
    /// <param name="num6"></param>
    /// <param name="num7"></param>
    public virtual void SendPacket(int packetId, NetworkText? text, int num1 = 0, float num2 = 0f, float num3 = 0f, float num4 = 0f, int num5 = 0, int num6 = 0, int num7 = 0)
    {
        Net.SendData(packetId, Index, -1, text ?? NetworkText.Empty, num1, num2, num3, num4, num5, num6, num7);
    }

    internal void StartPacketHandler()
    {
        netTokenSource = new CancellationTokenSource();
        netToken = netTokenSource.Token;

        packetHandlerTask = new Task(PacketHandler);
        packetHandlerTask.Start();
    }

    internal void StopPacketHandler()
    {
        netTokenSource?.Cancel();
    }

    // ctrl c + ctrl v issue
    private void PacketHandler()
    {
        if (netToken == null)
            return;

        while (!netToken.Value.IsCancellationRequested)
        {
            try
            {
                bool ignore = false;

                OutcomingPacket packet = OutcomingPackets.Take(netToken.Value);

                List<NetworkBindDelegate<OutcomingPacket>>? binds = MintServer.Network.OutcomingPackets.binds[packet.PacketID];
                if (binds != null)
                {
                    foreach (NetworkBindDelegate<OutcomingPacket> bind in binds)
                    {
                        bind(this, packet, ref ignore);
                    }
                }

                if (ignore)
                    continue;

                NetworkBindDelegate<OutcomingPacket>? hijackDelegate = MintServer.Network.OutcomingHijack[packet.PacketID];

                if (hijackDelegate != null) hijackDelegate?.Invoke(this, packet, ref ignore);
                else packet.Original(packet.PacketID, 
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
                Console.WriteLine("Exception in SendData: " + ex.ToString());
            }
        }
    }
}