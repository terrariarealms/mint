namespace Mint.Server.Chat;

public class RenderList
{
    internal RenderList(){}

    /// <summary>
    /// 'Pre' rendering layer. This is first layer of rendering.
    /// </summary>
    public List<ChatRenderDelegate> Pre => _pre;

    /// <summary>
    /// 'Post' rendering layer. This is last layer of rendering.
    /// </summary>
    public List<ChatRenderDelegate> Post => _post;

    private List<ChatRenderDelegate> _pre = new List<ChatRenderDelegate>(16);
    private List<ChatRenderDelegate> _post = new List<ChatRenderDelegate>(16);

    /// <summary>
    /// Renders text from Pre & Post layers.
    /// </summary>
    /// <param name="message">Target message</param>
    /// <returns>Rendered text</returns>
    public string Render(ChatMessage message)
    {
        return string.Join(" ", RenderAll(message));
    }

    /// <summary>
    /// Clear all layers.
    /// </summary>
    public void Clear()
    {
        _pre.Clear();
        _post.Clear();
    }

    private IEnumerable<string> RenderAll(ChatMessage message)
    {
        foreach (ChatRenderDelegate renderDelegate in _pre) if (renderDelegate != null)
            yield return renderDelegate(message.Sender, message);

        foreach (ChatRenderDelegate renderDelegate in _post) if (renderDelegate != null)
            yield return renderDelegate(message.Sender, message);
    }
}