
namespace Mint.Server.Commands;

public class DynamicCommand : ICommand
{
    public DynamicCommand(string name, string description, string? syntax, string? permission, CommandFlags flags, DynamicCommandDelegate @delegate)
    {
        Name = name;
        Description = description;
        Syntax = syntax;
        Permission = permission;
        Flags = flags;
        CommandDelegate = @delegate;
    }

    public string Name { get; }

    public string Description { get; }

    public string? Syntax { get; }

    public string? Permission { get; }

    public CommandFlags Flags { get; }

    public DynamicCommandDelegate CommandDelegate { get; }

    public void Invoke(CommandInvokeContext ctx)
    {
        bool ignore = false;
        ICommand.InvokeOnCommand(ctx.Sender, this, ref ctx, ref ignore);
        if (ignore) return;

        CommandDelegate(ctx);
    }
}