namespace Mint.Network.Incoming;

public static class IncomingHandlers
{
    internal static void Initialize()
    {
        MintServer.Network.IncomingModulesHijack[1] = OnChatNetModule;

        MintServer.Network.IncomingHijack[PacketID.PlayerPresence] = OnPlayerPresence;
    }

    static void OnChatNetModule(Player? player, IncomingPacket packet, ref bool ignore)
    {
        ignore = true;
        if (player == null) return;

        BinaryReader reader = packet.GetReader();

        string command = reader.ReadString();
        string text = reader.ReadString();

        MintServer.Chat.HandleMessage(new ChatMessage(player, command == "Say" ? text : $"/{command.ToLower()} {text}", DateTime.UtcNow));
    }

    static void OnPlayerPresence(Player? player, IncomingPacket packet, ref bool ignore)
    {
        ignore = true;
        if (player == null) return;

        BinaryReader reader = packet.GetReader();
        reader.ReadByte();

        byte skinVariant = reader.ReadByte();
        byte hair = reader.ReadByte();
        string name = reader.ReadString();


        TPlayer tplr = player.TPlayer;
        tplr.hideMisc = 5;
    }

	static void ReadAccessoryVisibility(BinaryReader reader, bool[] hideVisibleAccessory)
	{
		ushort num = reader.ReadUInt16();
		for (int i = 0; i < hideVisibleAccessory.Length; i++)
		{
			hideVisibleAccessory[i] = (num & (1 << i)) != 0;
		}
	}
}