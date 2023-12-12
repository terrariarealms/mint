namespace Mint.Server;

// this will be super long if i named that "CharacterOperationType"
public enum CharacterOperation : byte
{
    /// <summary>
    /// Means that server recieved packet from player and requested change.
    /// </summary>
    RequestedByPlayer,

    /// <summary>
    /// Means that Mint wants to process that operation.
    /// </summary>
    RequestedByServer,

    /// <summary>
    /// Means that module wants to process that operation.
    /// </summary>
    RequestedByThirdparty,

    /// <summary>
    /// Means that request was silent and ICharacter will not invoke ICharacter events.
    /// </summary>
    SilentRequest
}