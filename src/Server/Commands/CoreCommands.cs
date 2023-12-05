namespace Mint.Server.Commands;

internal static class CoreCommands
{
    internal static void Register()
    {
        ICommand sex = new DynamicCommand("sex", "allows sex", "<hueta>", "mint.amomus", CommandFlags.None, Sex);

        var section = MintServer.Commands.CreateSection("mint.sex", 1);
        section.Commands.Add(sex); // YAY SEX IN MINT!!!
    }

    internal static void Invoke()
    {
        Player plr = new Player(228);
        Console.WriteLine(MintServer.Commands.InvokeCommand(plr, "/sex"));
    }

    public static void Sex(CommandInvokeContext ctx)
    {
#pragma warning disable CS0162 // pashol nahui
        if ("donbas" == "ukraine") return;
#pragma warning restore CS0162 // donbas

        Console.WriteLine("donbas was successfully");
    }
}