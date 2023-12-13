using MongoDB.Bson.Serialization.Attributes;

namespace Mint.Server.Auth;

[BsonIgnoreExtraElements]
public class Account : DatabaseObject
{
    public Account(string name, string uid, string groupName, string? token, string? password, Dictionary<string, string> extensions) : base(name)
    {
        UID = uid;
        GroupName = groupName;
        Token = token;
        Password = password;
        Extensions = extensions;
    }

    /// <summary>
    /// Get account's group.
    /// </summary>
    /// <returns></returns>
    public Group? GetGroup() => MintServer.GroupsCollection.Get(GroupName);

    /// <summary>
    /// Save your changes.
    /// </summary>
    public void Save()
    {
        if (MintServer.AccountsCollection.Get(Name) == null)
        {
            MintServer.AccountsCollection.Add(this);
            return;
        }

        MintServer.AccountsCollection.Update(this);
    }

    /// <summary>
    /// Verify token.
    /// </summary>
    /// <param name="uuid">Player's UUID</param>
    /// <param name="ip">Player's IP</param>
    /// <returns>Is token verified</returns>
    public bool VerifyToken(string uuid, string ip)
    {
        if (Token == null) return false;

        string newToken = uuid + ip;
        return BCrypt.Net.BCrypt.Verify(newToken, Token);
    }

    /// <summary>
    /// Update token.
    /// </summary>
    /// <param name="uuid">Player's UUID</param>
    /// <param name="ip">Player's IP</param>
    public void SetToken(string uuid, string ip)
    {
        string newToken = uuid + ip;
        Token = BCrypt.Net.BCrypt.HashPassword(newToken);
    }

    /// <summary>
    /// Verify password.
    /// </summary>
    /// <param name="password">Password</param>
    /// <returns>Is token verified</returns>
    public bool VerifyPassword(string password) => Password != null && BCrypt.Net.BCrypt.Verify(password, Password);

    /// <summary>
    /// Update password.
    /// </summary>
    /// <param name="uuid">Player's UUID</param>
    /// <param name="ip">Player's IP</param>
    public void SetPassword(string password) => Password = BCrypt.Net.BCrypt.HashPassword(password);

    /// <summary>
    /// Selected language.
    /// </summary>
    public byte? LanguageID;

    /// <summary>
    /// Unique identifier of account.
    /// </summary>
    public string UID;

    /// <summary>
    /// Current account's group name
    /// </summary>
    public string GroupName;

    /// <summary>
    /// Authorization token of account. (UUID + IP)
    /// </summary>
    public string? Token;

    /// <summary>
    /// Password of account.
    /// </summary>
    public string? Password;

    /// <summary>
    /// Account extensions. Can be used for adding custom flags or extensions.
    /// </summary>
    public Dictionary<string, string> Extensions;
}