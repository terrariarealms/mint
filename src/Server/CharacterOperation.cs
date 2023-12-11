namespace Mint.Server;

// this will be super long if i named that "CharacterOperationType"
public enum CharacterOperation : byte
{
    /// <summary>
    /// Means that server recieved packet from player and requested change.
    /// </summary>
    RequestedByPlayer,

    /// <summary>
    /// Means that Mint or modules wants to process that operation.
    /// </summary>
    RequestedByServer
}