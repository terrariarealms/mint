namespace Mint.Server.Commands;

[Flags]
public enum CommandFlags : byte
{
    /// <summary>
    /// Default command flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// Means that only player with root permissions can invoke that command.
    /// </summary>
    RootOnly = 1,

    /// <summary>
    /// Means that command cannot be invoked.
    /// </summary>
    Disabled = 2,

    /// <summary>
    /// Means that command will be not shown in /help.
    /// </summary>
    Hidden = 4,

    /// <summary>
    /// Means that command is can be invoked only by thirdparty assemblies.
    /// </summary>
    Special = 8
}