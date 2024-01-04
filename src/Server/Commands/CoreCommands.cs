using On.Terraria.GameContent.Personalities;

namespace Mint.Server.Commands;

internal static class CoreCommands
{
    internal static void Register()
    {
        Log.Information("CoreCommands -> Register().");

        var section = MintServer.Commands.CreateSection("mint.core", 1);
        section.ImportFrom(typeof(CoreCommands));
    }

    [StaticCommand("rootfix", "fix root group permissions", null)]
    [CommandFlags(CommandFlags.Special)]
    public static void Rootfix(CommandInvokeContext ctx)
    {
        var root = MintServer.GroupsCollection.Get("root");
        if (root == null)
        {
            root = new Group("root", true, null, new GroupPresence(null, null, null), new List<DatabaseObject>(),
                new List<string>());
            root.Save();
        }
        else
        {
            root.RootPermissions = true;
            root.Save();
        }

        ctx.Messenger.Send(MessageMark.OK, "System", "Fixed.");
    }

    [StaticCommand("help", "view available commands list", "[page]")]
    public static void Help(CommandInvokeContext ctx, int page = 1)
    {
        ctx.Messenger.SendPage("Commands (total: {2}):", new List<string>(BuildHelpLines(ctx.Sender)), page, null,
            "Next page: /help {3}");
    }

    internal static IEnumerable<string> BuildHelpLines(Player plr)
    {
        foreach (var section in MintServer.Commands.commands)
        foreach (var command in section.Commands)
            if (command.Permission == null || (plr.Group?.HasPermission(command.Permission) == true
                                               && !command.Flags.HasFlag(CommandFlags.Hidden) &&
                                               !command.Flags.HasFlag(CommandFlags.Special)))
            {
                if (plr.Group?.RootPermissions == false && command.Flags.HasFlag(CommandFlags.RootOnly))
                    continue;

                var syntax = command.Syntax == null
                    ? ""
                    : " " + MintServer.Localization.Translate(command.Syntax, plr.Account?.LanguageID);
                var description = MintServer.Localization.Translate(command.Description, plr.Account?.LanguageID);
                var commandText = $"{command.Name}{syntax} - {description}";
                yield return commandText;
            }
    }

    [StaticCommand("register", "register account", "<password>")]
    public static void Register(CommandInvokeContext ctx, string password)
    {
        if (ctx.Sender.Name == null || ctx.Sender.UUID == null) return;

        if (ctx.Sender.Account != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "You have already registered account!");
            return;
        }

        var foundAccount = MintServer.AccountsCollection.Get(ctx.Sender.Name);
        if (foundAccount != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "Account with that name already exists!");
            return;
        }

        var newAccount = new Account(ctx.Sender.Name, Guid.NewGuid().ToString(), "user", null, null, ctx.Sender.IP, ctx.Sender.UUID,
            new Dictionary<string, string>());
        newAccount.SetPassword(password);
        newAccount.SetToken(ctx.Sender.UUID, ctx.Sender.IP);
        newAccount.LanguageID = MintServer.Config.LanguageID;

        MintServer.AccountsCollection.Push(newAccount.Name, newAccount);

        ctx.Sender.Authorize(newAccount);

        ctx.Messenger.Send(MessageMark.OK, "Account", "Account successfully created!");
    }

    [StaticCommand("login", "login account", "<password>")]
    public static void Login(CommandInvokeContext ctx, string password)
    {
        if (ctx.Sender.Name == null || ctx.Sender.UUID == null) return;

        if (ctx.Sender.Account != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "You have already logged in!");
            return;
        }

        var foundAccount = MintServer.AccountsCollection.Get(ctx.Sender.Name);
        if (foundAccount == null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "Account with that name is not exists!");
            return;
        }

        if (!foundAccount.VerifyPassword(password))
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "Invalid password!");
            return;
        }

        foundAccount.IP = ctx.Sender.IP;
        foundAccount.UUID = ctx.Sender.UUID;
        foundAccount.SetToken(ctx.Sender.UUID, ctx.Sender.IP);
        foundAccount.Save();

        ctx.Sender.Authorize(foundAccount);

        ctx.Messenger.Send(MessageMark.OK, "Account", "Welcome back, {0}!", foundAccount.Name);
    }

    [StaticCommand("logout", "logout from account", null)]
    public static void Logout(CommandInvokeContext ctx)
    {
        if (ctx.Sender.Name == null || ctx.Sender.UUID == null) return;

        if (ctx.Sender.Account == null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "You are not logged in!");
            return;
        }

        ctx.Sender.Account.Token = null;
        ctx.Sender.Account.Save();

        ctx.Sender.Logout();
        ctx.Messenger.Send(MessageMark.OK, "Account", "You have logged out from your account.");
    }

    [StaticCommand("lang russian", "установить русский язык (для себя)", null)]
    public static void LangRussian(CommandInvokeContext ctx)
    {
        if (ctx.Sender.Account == null)
        {
            ctx.Messenger.CleanSend(MessageMark.Error, "Язык", "Вы не вошли в аккаунт!");
            return;
        }

        ctx.Sender.Account.LanguageID = LanguageID.Russian;
        ctx.Sender.Account.Save();

        ctx.Messenger.CleanSend(MessageMark.OK, "Язык", "Язык успешно изменен на русский.");
    }

    [StaticCommand("lang english", "set language to english (for self)", null)]
    public static void LangEnglish(CommandInvokeContext ctx)
    {
        if (ctx.Sender.Account == null)
        {
            ctx.Messenger.CleanSend(MessageMark.Error, "Language", "You are not logged in!");
            return;
        }

        ctx.Sender.Account.LanguageID = LanguageID.English;
        ctx.Sender.Account.Save();

        ctx.Messenger.CleanSend(MessageMark.OK, "Language", "Language successfully changed to english.");
    }

    #region Groups Management

    [StaticCommand("group list", "view group list", "[page]")]
    [CommandPermission("mint.groups.list")]
    public static void GroupList(CommandInvokeContext ctx, int page = 1)
    {
        IEnumerable<string> lines = MintServer.GroupsCollection.GetAll()
            .Select(p =>
                p.Name + ": " + p.Presence.GetPrefix() + ctx.Sender.Name ?? "unknown" + p.Presence.GetSuffix());
        ctx.Messenger.SendPage("Registered groups (total: {2}):", new List<string>(lines), page, null,
            "Next page: /group list {3}");
    }

    [StaticCommand("group add", "create new group",
        "<name> [permissions;with;that;symbol] [parent group] [root: true/false]")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupAdd(CommandInvokeContext ctx, string name, string? permissions = null, Group? parent = null,
        bool root = false)
    {
        if (MintServer.GroupsCollection.Get(name) != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Groups", "Group with that name already exists!");
            return;
        }

        List<string> permsList = permissions?.Split(';').ToList() ?? new List<string>();
        var group = new Group(name, root, parent?.Name, new GroupPresence(null, null, null), new List<DatabaseObject>(),
            permsList);
        group.Save();
        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully created!");
    }

    [StaticCommand("group del", "delete group", "<name>")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupDelete(CommandInvokeContext ctx, Group group)
    {
        MintServer.GroupsCollection.Pop(group.Name);
        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully deleted!");
    }

    [StaticCommand("group setroot", "set root permissions for group", "<name> <isRoot: true/false>")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupSetRoot(CommandInvokeContext ctx, Group group, bool isRoot)
    {
        group.RootPermissions = isRoot;
        group.Save();

        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully updated!");
    }

    [StaticCommand("group addperm", "add permission to group", "<name> <permission>")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupAddPerm(CommandInvokeContext ctx, Group group, string permission)
    {
        group.Permissions.Add(permission);
        group.Save();

        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully updated!");
    }

    [StaticCommand("group delperm", "delete permission of group", "<name> <permission>")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupDelPerm(CommandInvokeContext ctx, Group group, string permission)
    {
        group.Permissions.Remove(permission);
        group.Save();

        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully updated!");
    }

    [StaticCommand("group listperm", "view permissions of group", "<name> [page]")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupListPerm(CommandInvokeContext ctx, Group group, int page = 1)
    {
        ctx.Messenger.SendPage("Permissions (total: {2}):", group.Permissions, page, null,
            "Next page: /group listperm <name> {3}");
    }

    [StaticCommand("group parent", "set parent for group", "<name> <parent>")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupParent(CommandInvokeContext ctx, Group group, Group parent)
    {
        group.ParentName = parent?.Name ?? null;
        group.Save();

        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully updated!");
    }

    [StaticCommand("group noparent", "remove parent for group", "<name>")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupNoParent(CommandInvokeContext ctx, Group group)
    {
        group.ParentName = null;
        group.Save();

        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully updated!");
    }

    [StaticCommand("group prefix", "set prefix for group", "<name> <prefix>")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupPrefix(CommandInvokeContext ctx, Group group, string prefix)
    {
        group.Presence.Prefix = prefix;
        group.Save();

        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully updated!");
    }

    [StaticCommand("group suffix", "set suffix for group", "<name> <suffix>")]
    [CommandPermission("mint.groups.manage")]
    public static void GroupSuffix(CommandInvokeContext ctx, Group group, string suffix)
    {
        group.Presence.Suffix = suffix;
        group.Save();

        ctx.Messenger.Send(MessageMark.OK, "Groups", "Group successfully updated!");
    }

    #endregion

    #region Accounts management

    [StaticCommand("account setgroup", "set group for account", "<account> <group>")]
    [CommandPermission("mint.accounts.manage")]
    public static void AccountSetGroup(CommandInvokeContext ctx, Account account, Group group)
    {
        account.GroupName = group.Name;
        account.Save();

        ctx.Messenger.Send(MessageMark.OK, "Account", "Account successfully updated.");
    }

    [StaticCommand("account setpassword", "set password for account", "<account> <password>")]
    [CommandPermission("mint.accounts.manage")]
    public static void AccountSetPassword(CommandInvokeContext ctx, Account account, string password)
    {
        account.SetPassword(password);
        account.Save();

        ctx.Messenger.Send(MessageMark.OK, "Account", "Account successfully updated.");
    }

    [StaticCommand("account killtoken", "kill auto-auth token for account", "<account>")]
    [CommandPermission("mint.accounts.manage")]
    public static void AccountKillToken(CommandInvokeContext ctx, Account account)
    {
        account.Token = null;
        account.Save();

        ctx.Messenger.Send(MessageMark.OK, "Account", "Account successfully updated.");
    }

    [StaticCommand("account add", "add new account", "<account> <group>")]
    [CommandPermission("mint.accounts.manage")]
    public static void AccountAdd(CommandInvokeContext ctx, string accountName, Group group)
    {
        if (MintServer.AccountsCollection.Get(accountName) != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "Account with that name already exists!");
            return;
        }

        var account = new Account(accountName, Guid.NewGuid().ToString(), group.Name, null, null, "*.*.*.*", null,
            new Dictionary<string, string>());
        account.Save();

        ctx.Messenger.Send(MessageMark.OK, "Account", "Account successfully created.");
    }

    [StaticCommand("account del", "delete account", "<account>")]
    [CommandPermission("mint.accounts.manage")]
    public static void AccountAdd(CommandInvokeContext ctx, string accountName)
    {
        if (MintServer.AccountsCollection.Get(accountName) == null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "Account with that name is not exists!");
            return;
        }

        MintServer.AccountsCollection.Pop(accountName);
        ctx.Messenger.Send(MessageMark.OK, "Account", "Account successfully deleted.");
    }

    #endregion
}