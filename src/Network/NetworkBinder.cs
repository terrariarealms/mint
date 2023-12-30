namespace Mint.Network;

public sealed class NetworkBinder<TPacket> where TPacket : struct
{
    internal NetworkBinder(byte maxBinds)
    {
        this.maxBinds = maxBinds;
        binds = new List<NetworkBindDelegate<TPacket>>?[maxBinds];
    }

    internal byte maxBinds;
    internal List<NetworkBindDelegate<TPacket>>?[] binds;

    /// <summary>
    /// Add packet bind.
    /// </summary>
    /// <param name="packetId">Packet ID</param>
    /// <param name="packetBind">Bind delegate.</param>
    /// <returns>Is packet bind was added</returns>
    public bool Add(byte packetId, NetworkBindDelegate<TPacket> packetBind)
    {
        if (packetId < 0 || packetId > binds.Length - 1)
            return false;

        if (binds[packetId] == null)
        {
            binds[packetId] = new List<NetworkBindDelegate<TPacket>>()
            {
                packetBind
            };
            return true;
        }

        if (binds[packetId]?.Contains(packetBind) == true)
            return false;

        binds[packetId]?.Add(packetBind);
        return true;
    }

    /// <summary>
    /// Remove packet bind.
    /// </summary>
    /// <param name="packetId">Packet ID</param>
    /// <param name="packetBind">Bind delegate.</param>
    /// <returns>Is packet bind was removed</returns>
    public bool Remove(byte packetId, NetworkBindDelegate<TPacket> packetBind)
    {
        if (packetId < 0 || packetId > binds.Length - 1)
            return false;

        if (binds[packetId] == null)
            return true;

        return binds[packetId]?.Remove(packetBind) ?? false;
    }

    internal void Reset()
    {
        binds = new List<NetworkBindDelegate<TPacket>>?[maxBinds];
    }
}