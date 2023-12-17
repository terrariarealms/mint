using Extensions;

namespace Mint.Server.Commands;

public sealed class CommandsManager
{
    internal CommandsManager(){}

    internal List<CommandSection> commands = new List<CommandSection>();
    internal Dictionary<Type, Func<string, object>> parsers = new Dictionary<Type, Func<string, object>>();
    
    internal void InitializeParsers()
    {
        parsers.Add(typeof(string), (arg) => arg);
        parsers.Add(typeof(bool), (arg) =>
        {
            if (bool.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        parsers.Add(typeof(byte), (arg) =>
        {
            if (byte.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        parsers.Add(typeof(int), (arg) =>
        {
            if (int.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        parsers.Add(typeof(float), (arg) =>
        {
            if (float.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        parsers.Add(typeof(short), (arg) =>
        {
            if (short.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        parsers.Add(typeof(long), (arg) =>
        {
            if (long.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        parsers.Add(typeof(ulong), (arg) =>
        {
            if (ulong.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        parsers.Add(typeof(Player), (arg) =>
        {
            Player? plr = MintServer.Players.First((p) => p.PlayerState == PlayerState.Joined && p.Name == arg);
            if (plr != null)
                return plr;

            plr = MintServer.Players.First((p) => p.PlayerState == PlayerState.Joined && p.Name?.ToLower() == arg.ToLower());
            if (plr != null)
                return plr;

            plr = MintServer.Players.First((p) => p.PlayerState == PlayerState.Joined && p.Name?.ToLower().StartsWith(arg) == true);
            if (plr != null)
                return plr;

            return new InvalidParameterValue(arg);
        });
        parsers.Add(typeof(Account), (arg) =>
        {
            var account = MintServer.AccountsCollection.Get(arg);
            if (account == null)
                return new InvalidParameterValue(arg);

            return account;
        });
        parsers.Add(typeof(Group), (arg) =>
        {
            var group = MintServer.GroupsCollection.Get(arg);
            if (group == null)
                return new InvalidParameterValue(arg);

            return group;
        });
    }

    /// <summary>
    /// Add custom parser.
    /// There is no DeleteCustomParser because that method was designed for modules only.
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="func">Func</param>
    public void AddCustomParser(Type type, Func<string, object> func)
    {
        parsers.Add(type, func);
    }

    internal ParseResult TryParse(Type type, string input, out object value)
    {
        if (!parsers.ContainsKey(type))
        {        
            Log.Error("Failed to pare type {Type} from {Input}", type.Name, input);
            value = new InvalidParameterValue(input);
            return ParseResult.ParserNotFound;
        }

        value = parsers[type](input);
        if (value is InvalidParameterValue) 
            return ParseResult.InvalidArgument;

        return ParseResult.Success;
    }

    public CommandResult InvokeCommand(Player sender, string text, bool special = false)
    {
        if (text.StartsWith("/"))
            text = text.Substring(1); // removes '/' from start.

        List<string> args = Parsing.SplitArguments(text); // splits arguments
        if (args.Count == 0) return CommandResult.CommandNotFound;

        // omg
        foreach (CommandSection section in commands)
        {
            IEnumerable<ICommand> sortedCommands = section.Commands.OrderByDescending(p => p.Name.Length);
            foreach (ICommand command in sortedCommands)
            {
                if (text.StartsWith(command.Name))
                {
                    args = new List<string>(args.Skip(command.Name.Split(' ').Length));

                    if (command != null)
                    {
                        Group? playerGroup = sender.Account?.GetGroup();
                        bool rootUser = playerGroup != null && playerGroup.RootPermissions;

                        if (command.Flags.HasFlag(CommandFlags.Disabled))
                            return CommandResult.CommandDisabled;

                        if (command.Flags.HasFlag(CommandFlags.Special) && !special)
                            return CommandResult.CommandNotFound;

                        if (command.Flags.HasFlag(CommandFlags.RootOnly) && !rootUser)
                            return CommandResult.NoPermission;

                        if (command.Permission != null && !rootUser)
                        {
                            if (playerGroup == null) return CommandResult.NoPermission;
                            else if (!playerGroup.HasPermission(command.Permission)) return CommandResult.NoPermission;
                        }

                        try
                        {
                            CommandInvokeContext ctx = new CommandInvokeContext(sender, command, args.AsReadOnly());
                            command.Invoke(ctx);

                            return CommandResult.Successfully;
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Commands: Exception in handling command {Command} from {Section}:", command.Name, section.Name);
                            Log.Error("Commands: {Exception}", ex.ToString());

                            return CommandResult.Error;
                        }
                    }
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

        Log.Information("Created command section {Name} with capacity {Capacity}.", name, capacity);

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

        Log.Information("Created command section {Name} from List<ICommand>.", name);

        return section;
    }

    /// <summary>
    /// Destroy command section.
    /// </summary>
    /// <param name="section">Section instance.</param>
    public void DestroySection(CommandSection section)
    {
        Log.Information("Destroyed command section {Name}.", section.Name);

        commands.Remove(section);
        RequestGC();
    }

    /// <summary>
    /// Destroy command section.
    /// </summary>
    /// <param name="section">Section name.</param>
    public void DestroySection(string section)
    {
        Log.Information("Destroyed command section {Name}.", section);

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