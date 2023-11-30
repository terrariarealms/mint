namespace Mint.API;

public static class NetworkAPI
{
    /// <summary>
    /// Add packet bind.
    /// </summary>
    /// <param name="packetId">Target packet ID</param>
    /// <param name="bindDelegate">Packet bind delegate</param>
    public static void AddBind(int packetId, PacketBindDelegate bindDelegate)
    {
        MintServer.Network?.Register(packetId, bindDelegate);
    }   

    /// <summary>
    /// Remove packet bind.
    /// </summary>
    /// <param name="packetId">Target packet ID</param>
    /// <param name="bindDelegate">Packet bind delegate</param>
    public static void RemoveBind(int packetId, PacketBindDelegate bindDelegate)
    {
        MintServer.Network?.Unregister(packetId, bindDelegate);
    }   
}