namespace Mint.Server.Commands;

public enum CommandResult
{
    /// <summary>
    /// Means that command invoke was successfully.
    /// </summary>
    Successfully,

    /// <summary>
    /// Means that command was not found.
    /// </summary>
    CommandNotFound,

    /// <summary>
    /// Means that command was disabled.
    /// </summary>
    CommandDisabled,

    /// <summary>
    /// Means that command is not invoked due not having permission.
    /// </summary>
    NoPermission,

    /// <summary>
    /// Means that command was throwed exception.
    /// </summary>
    Error
}