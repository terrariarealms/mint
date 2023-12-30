namespace Mint.Server;

public partial class Player
{
    /// <summary>
    /// Player's account.
    /// </summary>
    public virtual Account? Account { get; set; }

    /// <summary>
    /// Player's group
    /// </summary>
    public virtual Group? Group => Account?.GetGroup() ?? MintServer.GroupsCollection.Get("unauthorized");

    /// <summary>
    /// Player's authorized state.
    /// </summary>
    public virtual bool Authorized { get; set; }

    /// <summary>
    /// Invokes when server is requesting auto-authorization for player.
    /// </summary>
    public static event AutoAuthorizeEvent? OnAutoAuthorize;

    /// <summary>
    /// Authorize player automatically.
    /// </summary>
    public virtual void AutoAuthorize()
    {
        var account = FindAccountCandidate();
        var ignore = false;
        OnAutoAuthorize?.Invoke(this, ref account, ref ignore);
        if (ignore) return;

        if (account != null) Authorize(account);
    }

    /// <summary>
    /// Invokes when server is requesting find account candidate operation.
    /// </summary>
    public static event FindAccountEvent? OnFindAccountCandidate;

    /// <summary>
    /// Find account candidate by Name & UUID & IP.
    /// </summary>
    /// <returns>Found account</returns>
    public virtual Account? FindAccountCandidate()
    {
        if (string.IsNullOrEmpty(Name) || UUID == null)
            return null;

        var account = MintServer.AccountsCollection.Get(Name);

        var ignore = false;
        OnFindAccountCandidate?.Invoke(this, ref account, ref ignore);
        if (ignore) return null;

        if (account != null && account.VerifyToken(UUID, IP))
            return account;

        return null;
    }

    /// <summary>
    /// Invokes when server/player is requesting authorize operation for player.
    /// </summary>
    public static event AuthorizeEvent? OnAuthorize;

    /// <summary>
    /// Authorize player as target account.
    /// </summary>
    /// <param name="account">Target account</param>
    public virtual void Authorize(Account account, bool tokenChange = true)
    {
        var ignore = false;
        OnAuthorize?.Invoke(this, ref account, ref tokenChange, ref ignore);
        if (ignore) return;

        Log.Information("Player {Name} was authorized as {Account}.", Name, account.Name);

        if (tokenChange && UUID != null)
        {
            account.SetToken(UUID, IP);
            account.Save();
        }

        Account = account;
        Authorized = true;
    }

    /// <summary>
    /// Invokes when server/player is requesting logout operation.
    /// </summary>
    public static event LogoutEvent? OnLogout;

    /// <summary>
    /// Logout from account.
    /// </summary>
    public virtual void Logout()
    {
        var ignore = false;
        OnLogout?.Invoke(this, ref ignore);
        if (ignore) return;

        Log.Information("Player {Name} was logged out from as {Account}.", Name, Account?.Name ?? "unknown");

        Account = null;
        Authorized = false;
    }
}