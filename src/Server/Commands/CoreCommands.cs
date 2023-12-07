namespace Mint.Server.Commands;

internal static class CoreCommands
{
    internal static void Register()
    {
        var section = MintServer.Commands.CreateSection("mint.sex", 1);
        section.ImportFrom(typeof(CoreCommands));
    }

    internal static void Invoke()
    {
        Console.WriteLine(MintServer.Commands.InvokeCommand(MintServer.ServerPlayer, "/sex"));
        Console.WriteLine(MintServer.Commands.InvokeCommand(MintServer.ServerPlayer, "/sex super 2"));
    }

    [StaticCommand("sex", "allows sex", "<int>")]
    [CommandPermission("mint.sex")]
    [CommandFlags(CommandFlags.RootOnly | CommandFlags.Hidden)]
    public static void Sex(CommandInvokeContext ctx)
    {
        ctx.Messenger.Send(MessageMark.OK, "SEX", "ky");
    }

    [StaticCommand("sex super 2", "allows sex", "<int>")]
    [CommandPermission("mint.sex")]
    [CommandFlags(CommandFlags.RootOnly | CommandFlags.Hidden)]
    public static void Sex2(CommandInvokeContext ctx, int val1, int val2 = 4356)
    {
        ctx.Messenger.Send(MessageMark.OK, "SEX", $"Test: val1={val1}&val2={val2};");
    }
}