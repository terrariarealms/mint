using Terraria.Localization;
using Terraria;
using Terraria.Net.Sockets;

namespace Mint.Server;

public partial class Player
{
    /// <summary>   
    /// Player socket.
    /// </summary>
    public ISocket Socket => Netplay.Clients[Index].Socket;

    /// <summary>
    /// Player IP Address.
    /// You can change it if your server using Dimensions or something.
    /// </summary>
    public string IP 
    {
        get => _ip ?? Socket.GetRemoteAddress()?.ToString()?.Split(':')[0] ?? "0.0.0.0";
        set => _ip = value;
    }

    private string? _ip;

    /// <summary>
    /// Send packet bytes to client.
    /// </summary>
    /// <param name="bytes">Packet bytes</param>
    public void SendBytes(byte[] bytes)
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
    public void SendPacket(int packetId, NetworkText? text, int num1 = 0, float num2 = 0f, float num3 = 0f, float num4 = 0f, int num5 = 0, int num6 = 0, int num7 = 0)
    {
        Net.SendData(packetId, Index, -1, text ?? NetworkText.Empty, num1, num2, num3, num4, num5, num6, num7);
    }
}