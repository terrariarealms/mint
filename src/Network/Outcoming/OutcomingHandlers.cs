namespace Mint.Network.Outcoming;

public static partial class OutcomingHandlers
{
    internal static void Initialize()
    {
        Log.Information("OutcomingHandlers -> Initialize()");

        MintServer.Network.OutcomingHijack[PacketID.WorldData] = OnWorldData;
    }
}