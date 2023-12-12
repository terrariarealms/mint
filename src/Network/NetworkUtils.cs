namespace Mint.Network;

public static class NetworkUtils
{
    /// <summary>
    /// Broadcast packet to all players.
    /// </summary>
    /// <param name="bytes"></param>
    public static void SendPacket(byte[] bytes, int remoteClient = -1, int ignoreClient = -1)
    {
        if (remoteClient != -1)
        {
            MintServer.Players[remoteClient].SendBytes(bytes);
            return; 
        }

        MintServer.Players.QuickForEach((p) => 
        {
            if (p.Index != ignoreClient) 
                p.SendBytes(bytes);
        });
    }
}