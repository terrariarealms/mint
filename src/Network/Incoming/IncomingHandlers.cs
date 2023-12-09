namespace Mint.Network.Incoming;

public static class IncomingHandlers
{
    internal static void Initialize()
    {
        MintServer.Network.IncomingNetModules.Add(1, OnChatNetModule);
    }

    internal static void OnChatNetModule(Player? player, IncomingPacket packet, ref bool ignore)
    {
        ignore = true;
        if (player == null) return;

        BinaryReader reader = packet.GetReader();

        string command = reader.ReadString();
        string text = reader.ReadString();

        MintServer.Chat.HandleMessage(new ChatMessage(player, command == "Say" ? text : $"/{command.ToLower()} {text}", DateTime.UtcNow));
    }
}