namespace Mint.Core;

[Flags]
public enum ModuleType : byte
{
    None,

    /// <summary>
    /// API module type provides API utilities like: authorization (/register /login).
    /// Requires auto-update by finding new module updates with same module architecture.
    /// </summary>
    API = 1,

    /// <summary>
    /// Worker module type provides utilities like: anticheat, server optimization.
    /// </summary>
    Worker = 2
}