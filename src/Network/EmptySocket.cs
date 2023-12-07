using Terraria.Net;
using Terraria.Net.Sockets;

namespace Mint.Network;

public class EmptySocket : ISocket
{
    public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object? state = null)
    {
    }

    public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object? state = null)
    {
    }

    public void Close()
    {
    }

    public void Connect(RemoteAddress address)
    {
    }

    public RemoteAddress GetRemoteAddress()
    {
        return new LocalAddress();
    }

    public bool IsConnected()
    {
        return false;
    }

    public bool IsDataAvailable()
    {
        return false;
    }

    public void SendQueuedPackets()
    {
    }

    public bool StartListening(SocketConnectionAccepted callback)
    {
        return false;
    }

    public void StopListening()
    {
    }

    class LocalAddress : RemoteAddress
    {
        public override string GetFriendlyName()
        {
            return "mint";
        }

        public override string GetIdentifier()
        {
            return "0.0.0.0";
        }

        public override bool IsLocalHost()
        {
            return true;
        }
    }
}
