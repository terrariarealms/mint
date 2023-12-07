namespace Mint.Server.Commands;

public class CommandFlagsAttribute : Attribute
{
    public readonly CommandFlags Flags;

    public CommandFlagsAttribute(CommandFlags flags)
    {
        Flags = flags;
    }
}