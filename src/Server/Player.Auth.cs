namespace Mint.Server;

public partial class Player
{
    /// <summary>
    /// Player's account.
    /// </summary>
    public Account? Account { get; set; }

    /// <summary>
    /// Player's group
    /// </summary>
    public Group? Group => Account?.GetGroup();

    /// <summary>
    /// Player's authorized state.
    /// </summary>
    public bool Authorized { get; set; }

    /// <summary>
    /// Find account candidate by Name & UUID & IP.
    /// </summary>
    /// <returns>Found account</returns>
    public Account? FindAccountCandidate()
    {
        if (string.IsNullOrEmpty(Name) || UUID == null)
            return null;

        Account? account = MintServer.AccountsCollection?.Get(Name);
        if (account != null && account.VerifyToken(UUID, IP))
            return account;

        return null;
    }
}