using MongoDB.Bson.Serialization.Attributes;

namespace Mint.Server.Auth;

[BsonIgnoreExtraElements]
public class Group : DatabaseObject
{
    /// <summary>
    /// Create clean group without presence, permissions and extensions.
    /// </summary>
    /// <param name="name">Group name</param>
    /// <returns>Created group</returns>
    public static Group CreateClean(string name)
    {
        return new Group(name, false, null, new GroupPresence(null, null, null), new List<DatabaseObject>(),
            new List<string>());
    }

    public Group(string name, bool root, string? parent, GroupPresence presence, List<DatabaseObject> extensions,
        List<string> permissions) : base(name)
    {
        RootPermissions = root;
        ParentName = parent;
        Presence = presence;
        Extensions = extensions;
        Permissions = permissions;
    }

    /// <summary>
    /// Check for having permission.
    /// </summary>
    /// <param name="permission">Target permission</param>
    /// <returns>Having permission</returns>
    public bool HasPermission(string permission)
    {
        // root permission is cool
        if (RootPermissions)
            return true;

        // if permissions is null we are calling parent
        if (Permissions == null)
            return GetParent()?.HasPermission(permission) ?? false;

        // permissions like !my.cool.perm will override all other permissions and return false. 
        if (Permissions.Contains("!" + permission))
            return false;

        foreach (var perm in Permissions)
        {
            if (perm == permission)
                return true;

            string[] array = perm.Split('.');
            if (HasPartedPermission(array))
                return true;
        }

        return false;
    }

    // this is parted permission check like "my", "cool", "perm".
    private bool HasPartedPermission(string[] array)
    {
        foreach (var part in array)
        {
            // permissions like !my.cool.* will return false. 
            if (Permissions.Contains("!" + part + ".*"))
                return false;

            // permissions like my.cool.* will return true. 
            if (Permissions.Contains(part + ".*"))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Get group's parent.
    /// </summary>
    /// <returns>Parent group</returns>
    public Group? GetParent()
    {
        return ParentName == null ? null : MintServer.GroupsCollection.Get(ParentName);
    }

    /// <summary>
    /// Save your changes.
    /// </summary>
    public void Save()
    {
        MintServer.GroupsCollection.Push(Name, this);
    }

    /// <summary>
    /// Indicates does that group has root permissions. This is the replacement of '*' permission.
    /// Group with root permissions have all permissions.
    /// </summary>
    public bool RootPermissions;

    /// <summary>
    /// Group's parent name.
    /// </summary>
    public string? ParentName;

    /// <summary>
    /// Group presence. (Prefix, Suffix, Color)
    /// </summary>
    public GroupPresence Presence;

    /// <summary>
    /// Group extensions. Can be used for adding custom flags or extensions.
    /// </summary>
    public List<DatabaseObject> Extensions;

    /// <summary>
    /// Group permissions.
    /// </summary>
    public List<string> Permissions;
}