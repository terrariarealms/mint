namespace Mint.Server.Commands;

public class CommandPermissionAttribute : Attribute
{
    public readonly string Permission;

    public CommandPermissionAttribute(string permission)
    {
        Permission = permission;
    }
}