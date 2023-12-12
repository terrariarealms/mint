using System.Reflection;

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
    public List<ICommand> Commands { get; }

    /// <summary>
    /// Import commands from type.
    /// </summary>
    /// <param name="type">Target type</param>
    public void ImportFrom(Type type)
    {
        foreach (MethodInfo method in type.GetMethods())
        {
            if (!method.IsStatic) continue;

            StaticCommandAttribute? commandAttribute = method.GetCustomAttribute<StaticCommandAttribute>();
            if (commandAttribute == null) continue;

            CommandPermissionAttribute? permissionAttribute = method.GetCustomAttribute<CommandPermissionAttribute>();
            CommandFlagsAttribute? flagsAttribute = method.GetCustomAttribute<CommandFlagsAttribute>();

            ICommand command = new StaticCommand(method, commandAttribute.Name, commandAttribute.Description, commandAttribute.Syntax, permissionAttribute?.Permission, flagsAttribute?.Flags ?? CommandFlags.None);
            
            Log.Information("CommandSection -> Registered command {Name} from {Method}.", command.Name, method.Name);

            Commands.Add(command);
        }
    }
}