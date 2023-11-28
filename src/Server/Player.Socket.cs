using Terraria.Localization;
using Terraria;
using Terraria.Net.Sockets;

namespace Mint.Server;

public partial class Player
{
    /// <summary>
    /// Player's socket.
    /// </summary>
    public ISocket Socket => Netplay.Clients[Index].Socket;

    /// <summary>
    /// Send packet bytes to client.
    /// </summary>
    /// <param name="bytes">Packet bytes</param>
    public void SendBytes(byte[] bytes)
    {
		Socket.AsyncSend(bytes, 0, bytes.Length, RemoteClient.ServerWriteCallBack);
    }

    public void SendPacket(int packetId, NetworkText? text, )
    {

    }
}