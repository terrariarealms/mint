namespace Mint.Server;

public class DynamicMessenger : PlayerMessenger
{
    public DynamicMessenger(string name, bool outputToConsole)
    {
        Name = name;
        OutputToConsole = outputToConsole;
    }

    public string Name { get; }
    public IReadOnlyList<DynamicMessage>? Messages => messages?.AsReadOnly();
    public bool OutputToConsole { get; set; }

    internal List<DynamicMessage>? messages;

    public override void Begin()
    {
        messages = new List<DynamicMessage>();
    }

    public override void End()
    {
        base.End();
    }

    private void PrivateSend(MessageMark mark, string? source, string message, object?[] objects)
    {
        message = string.Format(message, objects);

        messages?.Add(new DynamicMessage(mark, source, message));

        if (OutputToConsole)
            Log.Information("{Name}: {Text}", Name, $"{(source == null ? "" : "<" + source + "> ")}{message}");
    }

    public override void Send(MessageMark mark, string? source, string message, params object?[] objects)
    {
        PrivateSend(mark, source, message, objects);
    }

    public override void CleanSend(MessageMark mark, string? source, string message, params object?[] objects)
    {
        PrivateSend(mark, source, message, objects);
    }
}