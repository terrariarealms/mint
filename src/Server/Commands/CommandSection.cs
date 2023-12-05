namespace Mint.Server.Commands;

public sealed class CommandSection
{
    internal CommandSection(string name, int capacity)
    {
        Name = name;
        Commands = new List<ICommand>(capacity);
    }
    internal CommandSection(string name, List<ICommand> wrappedCommands)
    {
        Name = name;
        Commands = wrappedCommands;
    }

    /// <summary>
    /// Command section name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Commands list.
    /// </summary>
    internal List<ICommand> Commands { get; }
}