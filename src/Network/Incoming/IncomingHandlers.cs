namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    internal static void Initialize()
    {
        Log.Information("IncomingHandlers -> Initialize()");

        MintServer.Network.IncomingModulesHijack[1] = OnChatNetModule;

        MintServer.Network.IncomingHijack[PacketID.PlayerUUID] = OnPlayerUUID;
        MintServer.Network.IncomingHijack[PacketID.PlayerUpdate] = OnPlayerUpdate;
        MintServer.Network.IncomingHijack[PacketID.PlayerPresence] = OnPlayerPresence;
        MintServer.Network.IncomingHijack[PacketID.PlayerSlot] = OnPlayerSlot;
        MintServer.Network.IncomingHijack[PacketID.PlayerLife] = OnPlayerLife;
        MintServer.Network.IncomingHijack[PacketID.PlayerMana] = OnPlayerMana;
    }

    private static void ReadAccessoryVisibility(BinaryReader reader, bool[] hideVisibleAccessory)
    {
        var num = reader.ReadUInt16();
        for (var i = 0; i < hideVisibleAccessory.Length; i++) hideVisibleAccessory[i] = (num & (1 << i)) != 0;
    }
}