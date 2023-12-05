using Extensions;

namespace Mint.Server.Commands;

public sealed class CommandsManager
{
    internal List<CommandSection> commands = new List<CommandSection>();

    public CommandResult InvokeCommand(Player sender, string text)
    {
        if (text.StartsWith("/"))
            text = text.Substring(1); // removes '/' from start.

        List<string> args = Parsing.SplitArguments(text); // splits arguments
        if (args.Count == 0) return CommandResult.CommandNotFound;

        string commandName = args[0];

        args = new List<string>(args.Skip(1));

        foreach (CommandSection section in commands)
        {
            ICommand? command = section.Commands.Find((p) => p.Name == commandName);
            if (command != null)
            {
                if (command.Flags.HasFlag(CommandFlags.Disabled))
                    return CommandResult.CommandDisabled;

                try
                {
                    CommandInvokeContext ctx = new CommandInvokeContext(sender, command, args.AsReadOnly());
                    command.Invoke(ctx);

                    return CommandResult.Successfully;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in handling command {command.Name} from {section.Name}:");
                    Console.WriteLine(ex.ToString());

                    return CommandResult.Error;
                }
            }
        }

        return CommandResult.CommandNotFound;
    }

    /// <summary>
    /// Create new commands section.
    /// </summary>
    /// <param name="name">Section name</param>
    /// <param name="capacity">Section capacity</param>
    /// <returns>CommandSection instance</returns>
    public CommandSection CreateSection(string name, int capacity)
    {
        CommandSection section = new CommandSection(name, capacity);
        commands.Add(section);

        return section;
    }

    /// <summary>
    /// Create new commands section.
    /// </summary>
    /// <param name="name">Section name</param>
    /// <param name="wrappedCommands">Commands list.</param>
    /// <returns>CommandSection instance</returns>
    public CommandSection CreateSection(string name, List<ICommand> wrappedCommands)
    {
        CommandSection section = new CommandSection(name, wrappedCommands);
        commands.Add(section);

        return section;
    }

    /// <summary>
    /// Destroy command section.
    /// </summary>
    /// <param name="section">Section instance.</param>
    public void DestroySection(CommandSection section)
    {
        commands.Remove(section);
        RequestGC();
    }

    /// <summary>
    /// Destroy command section.
    /// </summary>
    /// <param name="section">Section name.</param>
    public void DestroySection(string section)
    {
        commands.RemoveAll((p) => p.Name == section);
        RequestGC();
    }


    private void RequestGC()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }


    private static class Parsing
    {
        internal static List<string> SplitArguments(string text)
        {
            var validArguments = new List<string>()
            {
                ""
            };
            var argumentIndex = 0;
            var blockNoSplitting = false;
            var noSplitting = false;

            foreach (var c in text)
                switch (c)
                {
                    case ' ':
                        if (!noSplitting)
                        {
                            argumentIndex++;
                            validArguments.Add("");
                        }

                        blockNoSplitting = false;
                        break;

                    case '"':
                        if (!blockNoSplitting)
                            noSplitting = !noSplitting;

                        blockNoSplitting = false;
                        break;

                    case '\\':
                        blockNoSplitting = true;
                        break;

                    default:
                        validArguments[argumentIndex] += c;
                        blockNoSplitting = false;
                        break;
                }

            return validArguments;
        }
    }
}