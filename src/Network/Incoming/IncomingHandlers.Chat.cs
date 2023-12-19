namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    static void OnChatNetModule(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();

        string command = reader.ReadString();
        string text = reader.ReadString();

        if (text.Length > 300 && !player.IgnoreAnticheat)
        {
            Log.Error("{Name} cheat detect in {Where}", player.Account?.Name, "OnChatNetModule");
            player.CloseConnection();
            return;
        }

        if (player.PlayerState != PlayerState.Joined)
        {
            Log.Error("{Name} bot detect in {Where}", player.Account?.Name, "OnChatNetModule");
            player.CloseConnection();
            return;
        }

        MintServer.Chat.HandleMessage(new ChatMessage(player, command == "Say" ? text : $"/{command.ToLower()} {text}", DateTime.UtcNow));
    }
}