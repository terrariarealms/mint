namespace Mint.Server;

public struct DynamicMessage
{
    public DynamicMessage(MessageMark mark, string? source, string text)
    {
        Mark = mark;
        Source = source;
        Text = text;
    }

    public MessageMark Mark;
    public string? Source;
    public string Text;
}