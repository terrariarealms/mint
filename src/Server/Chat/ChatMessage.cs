namespace Mint.Server.Chat;

public struct ChatMessage
{
    public ChatMessage(Player sender, string text, DateTime time)
    {
        Sender = sender;
        Text = text;
        Time = time;
    }

    /// <summary>
    /// Message sender.
    /// </summary>
    public readonly Player Sender;

    /// <summary>
    /// Text of message.
    /// </summary>
    public string Text;

    /// <summary>
    /// Time of message creation.
    /// </summary>
    public readonly DateTime Time;
}