using Microsoft.Xna.Framework;
using Terraria.UI.Chat;

namespace Mint.Server.Chat;

public class ChatManager 
{
    internal ChatManager(){}

    /// <summary>
    /// Chat renderer: prefix, name, suffix.
    /// </summary>
    public ChatRenderer ChatRenderer { get; } = new ChatRenderer();

    /// <summary>
    /// Chat filters. You can use it for creating ban-words.
    /// You can block or modify message content via chat filter.
    /// </summary>
    public List<ChatFilterDelegate> ChatFilters { get; } = new List<ChatFilterDelegate>();

    internal void Initialize()
    {
        ChatRenderer.Setup();
    }

    /// <summary>
    /// Handle message. You can use it for displaying messages of player.
    /// If your message starts with '/' it will be invoke command.
    /// </summary>
    /// <param name="message">Chat message</param>
    public void HandleMessage(ChatMessage message)
    {
        bool block = false;
        string text = message.Text;

        foreach (ChatFilterDelegate filterDelegate in ChatFilters)
            filterDelegate(message.Sender, ref text, ref block);

        if (text.StartsWith('/'))
        {
            CommandResult result = MintServer.Commands.InvokeCommand(message.Sender, text);
            DisplayResult(message.Sender, result);
        }
        else
        {
            string rendered = ChatRenderer.RenderMessage(new ChatMessage(message.Sender, text, message.Time));
            BroadcastMessage(rendered, Color.White);
        }
    }

    /// <summary>
    /// Displays command result to player.
    /// </summary>
    /// <param name="player">Target player</param>
    /// <param name="result">Command result</param>
    public void DisplayResult(Player player, CommandResult result)
    {
        switch (result)
        {
            case CommandResult.CommandDisabled:
                player.Messenger.Send(MessageMark.Error, "Команды", "Команда отключена.");
                break;
            case CommandResult.NoPermission:
                player.Messenger.Send(MessageMark.Error, "Команды", "У вас недостаточно прав для использования этой команды.");
                break;
            case CommandResult.Error:
                player.Messenger.Send(MessageMark.Error, "Команды", "При выполнении этой команды произошла ошибка.");
                break;
            case CommandResult.CommandNotFound:
                player.Messenger.Send(MessageMark.Error, "Команды", "Команда не найдена. Используйте /help для просмотра списка команд.");
                break;
        }
    }

    /// <summary>
    /// Broadcast chat message to all connected players.
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="color">Message color</param>
    public void BroadcastMessage(string text, Color color)
    {
        Console.WriteLine("@bc: " + text);
        Terraria.Chat.ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral(text), color);
    }
}