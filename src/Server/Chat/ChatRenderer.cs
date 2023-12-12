namespace Mint.Server.Chat;

public class ChatRenderer
{
    internal ChatRenderer(){}

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

    private RenderList _prefixList = new RenderList();
    private RenderList _nameList = new RenderList();
    private RenderList _suffixList = new RenderList();

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

    string RenderPrefix(Player sender, ChatMessage message) => sender?.Group?.Presence.GetPrefix() ?? "";
    string RenderName(Player sender, ChatMessage message) => sender?.Name ?? "<!unknown>";
    string RenderSuffix(Player sender, ChatMessage message) => sender?.Group?.Presence.GetSuffix() ?? "";

    /// <summary>
    /// Renders message.
    /// </summary>
    /// <param name="message">Chat message</param>
    /// <returns>Rendered text</returns>
    public string RenderMessage(ChatMessage message)
    {
        string prefix = PrefixRender.Render(message);
        string name = NameRender.Render(message);
        string suffix = SuffixRender.Render(message);

        return prefix + name + suffix + ": " + message.Text;
    }
}