namespace Mint.Server.Commands;

public interface ICommand
{
    /// <summary>
    /// Command invoke event. Invokes when someone invoking command.
    /// </summary>
    public static event CommandInvokeEvent? OnCommandInvoked;

    protected static void InvokeOnCommand(Player sender, ICommand command, ref CommandInvokeContext ctx,
        ref bool ignore)
    {
        OnCommandInvoked?.Invoke(sender, command, ref ctx, ref ignore);
    }

    /// <summary>
    /// Command name. Example: "register".
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Command description. Example: "register new account".
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Command syntax. Example: "<password>".
    /// <argument> - shows that argument is required.
    /// [argument] - shows that argument is optional.
    /// </summary>
    public string? Syntax { get; }

    /// <summary>
    /// Command permission. Example: "mint.register"
    /// </summary>
    public string? Permission { get; }

    /// <summary>
    /// Command flags.
    /// </summary>
    public CommandFlags Flags { get; }

    public void Invoke(CommandInvokeContext ctx);
}