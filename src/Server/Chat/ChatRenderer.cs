namespace Mint.Server.Chat;

public class ChatRenderer
{
    internal ChatRenderer()
    {
    }

    /// <summary>
    /// Prefix render list. You can modify prefix rendering via this property.
    /// </summary>
    public RenderList PrefixRender => _prefixList;

    /// <summary>
    /// Name render list. You can modify name rendering via this property.
    /// </summary>
    public RenderList NameRender => _nameList;

    /// <summary>
    /// Suffix render list. You can modify suffix rendering via this property.
    /// </summary>
    public RenderList SuffixRender => _suffixList;

    private RenderList _prefixList = new();
    private RenderList _nameList = new();
    private RenderList _suffixList = new();

    internal void Reset()
    {
        _prefixList = new RenderList();
        _nameList = new RenderList();
        _suffixList = new RenderList();
    }

    internal void Setup()
    {
        _prefixList.Post.Insert(0, RenderPrefix);
        _nameList.Post.Insert(0, RenderName);
        _suffixList.Post.Insert(0, RenderSuffix);
    }

    private string RenderPrefix(Player sender, ChatMessage message)
    {
        return sender?.Group?.Presence.GetPrefix() ?? "";
    }

    private string RenderName(Player sender, ChatMessage message)
    {
        return sender?.Name ?? "<!unknown>";
    }

    private string RenderSuffix(Player sender, ChatMessage message)
    {
        return sender?.Group?.Presence.GetSuffix() ?? "";
    }

    public static event EventRenderedChatDelegate? OnRenderedChat;

    /// <summary>
    /// Renders message.
    /// </summary>
    /// <param name="message">Chat message</param>
    /// <returns>Rendered text</returns>
    public string RenderMessage(ChatMessage message)
    {
        var prefix = PrefixRender.Render(message);
        var name = NameRender.Render(message);
        var suffix = SuffixRender.Render(message);

        var rendered = prefix + name + suffix + ": " + message.Text;
        OnRenderedChat?.Invoke(message.Sender, rendered);
        return rendered;
    }
}