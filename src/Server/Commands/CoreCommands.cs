namespace Mint.Server.Commands;

internal static class CoreCommands
{
    internal static void Register()
    {
        Log.Information("CoreCommands -> Register().");

        var section = MintServer.Commands.CreateSection("mint.core", 1);
        section.ImportFrom(typeof(CoreCommands));
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

        Account? foundAccount = MintServer.AccountsCollection.Get(ctx.Sender.Name);
        if (foundAccount != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Account", "Account with that name already exists!");
            return;
        }

        Account newAccount = new Account(ctx.Sender.Name, Guid.NewGuid().ToString(), "user", null, null, new Dictionary<string, string>());
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

        Account? foundAccount = MintServer.AccountsCollection.Get(ctx.Sender.Name);
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

        ctx.Sender.Authorize(foundAccount);

        ctx.Messenger.Send(MessageMark.OK, "Account", "Welcome back, {0}!", foundAccount.Name);
    }

    [StaticCommand("logout", "logout from account", null)]
    public static void Logout(CommandInvokeContext ctx, string password)
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
}