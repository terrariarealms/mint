namespace Mint.Network;

public static class NetworkUtils
{
    /// <summary>
    /// Broadcast packet to all players.
    /// </summary>
    /// <param name="bytes"></param>
    public static void BroadcastPacket(byte[] bytes)
    {
        MintServer.Players.QuickForEach((p) => p.SendBytes(bytes));
    }
}