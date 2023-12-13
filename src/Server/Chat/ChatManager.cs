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
        Log.Information("Chat renderer -> Initialize()");
        ChatRenderer.Setup();
    }

    public event EventChatDelegate? OnChat;

    /// <summary>
    /// Handle message. You can use it for displaying messages of player.
    /// If your message starts with '/' it will be invoke command.
    /// </summary>
    /// <param name="message">Chat message</param>
    public void HandleMessage(ChatMessage message)
    {
        bool isCommand = message.Text.StartsWith('/');
        if (!isCommand)
        {
            bool ignore = false;
            OnChat?.Invoke(message.Sender, ref message, ref ignore);
            if (ignore) return;
        }

        bool block = false;
        string text = message.Text;

        foreach (ChatFilterDelegate filterDelegate in ChatFilters)
            filterDelegate(message.Sender, ref text, ref block);

        if (text.StartsWith('/'))
        {
            string validCommand = (text.StartsWith("/register") || text.StartsWith("/login")) ? "[/register OR /login]" : text;

            Log.Information("{Group}@{Name} used -> {Command}", message.Sender.Group?.Name ?? "unauthorized", message.Sender.Name, validCommand);

            CommandResult result = MintServer.Commands.InvokeCommand(message.Sender, text);
            DisplayResult(message.Sender, result);
        }
        else
        {
            Log.Information("{Group}@{Name} says -> {Command}", message.Sender.Group?.Name ?? "unauthorized", message.Sender.Name, text);

            string rendered = ChatRenderer.RenderMessage(new ChatMessage(message.Sender, text, message.Time));
            BroadcastMessage(rendered, Color.White);
        }
    }

    public event EventDisplayResultDelegate? OnDisplayResult;

    /// <summary>
    /// Displays command result to player.
    /// </summary>
    /// <param name="player">Target player</param>
    /// <param name="result">Command result</param>
    public void DisplayResult(Player player, CommandResult result)
    {
        bool ignore = false;
        OnDisplayResult?.Invoke(player, result, ref ignore);
        if (ignore) return;

        switch (result)
        {
            case CommandResult.CommandDisabled:
                player.Messenger.Send(MessageMark.Error, "Commands", "Command is disabled.");
                break;
            case CommandResult.NoPermission:
                player.Messenger.Send(MessageMark.Error, "Commands", "You do not have permission for using this command.");
                break;
            case CommandResult.Error:
                player.Messenger.Send(MessageMark.Error, "Commands", "Command was failed!");
                break;
            case CommandResult.CommandNotFound:
                player.Messenger.Send(MessageMark.Error, "Commands", "Command not found. Use /help for viewing all available commands.");
                break;
        }
    }

    internal void SystemBroadcast(string text)
    {
        string sysPrefix = $"[c/1f9162:[][c/20d492:{MintServer.Localization.Translate("System")}][c/1f9162:]] [c/595959:»] ";
        BroadcastMessage(sysPrefix + text, new Color(130, 255, 203));

        Log.Information("System Broadcast: {Text}", text);
    }

    public event EventBroadcastDelegate? OnBroadcast;

    /// <summary>
    /// Broadcast chat message to all connected players.
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="color">Message color</param>
    public void BroadcastMessage(string text, Color color)
    {
        bool ignore = false;
        OnBroadcast?.Invoke(ref text, ref color, ref ignore);
        if (ignore) return;

        Log.Information("Broadcast: {Text}", text);
        Terraria.Chat.ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral(text), color);
    }
}