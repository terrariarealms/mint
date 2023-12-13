namespace Mint.Server;

public class DynamicMessenger : PlayerMessenger
{
    public DynamicMessenger(string name, bool outputToConsole)
    {
        Name = name;
        OutputToConsole = outputToConsole;
    }

    public string Name { get;}
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

    public override void Send(MessageMark mark, string? source, string message, params object?[] objects)
    {
        message = string.Format(message, objects);

        messages?.Add(new DynamicMessage(mark, source, message));
        
        if (OutputToConsole)
            Console.WriteLine($"{Name}: {(source == null ? "" : "<" + source + "> ")}{message}");
    }
}