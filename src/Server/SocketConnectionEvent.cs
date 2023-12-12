using System.Net.Sockets;
using Terraria.Net.Sockets;

namespace Mint.Server;

public delegate void SocketConnectionEvent(ref ISocket socket, TcpClient tcpClient, ref bool handled);