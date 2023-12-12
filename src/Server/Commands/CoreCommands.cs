namespace Mint.Server.Commands;

internal static class CoreCommands
{
    internal static void Register()
    {
        Log.Information("CoreCommands -> Register().");

        var section = MintServer.Commands.CreateSection("mint.core", 1);
        section.ImportFrom(typeof(CoreCommands));
    }
    

    [StaticCommand("register", "зарегистрировать аккаунт", "<пароль>")]
    public static void Register(CommandInvokeContext ctx, string password)
    {
        if (ctx.Sender.Name == null || ctx.Sender.UUID == null) return;

        if (ctx.Sender.Account != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Аккаунт", "Вы уже зарегистрировали аккаунт!");
            return;
        }

        Account? foundAccount = MintServer.AccountsCollection.Get(ctx.Sender.Name);
        if (foundAccount != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Аккаунт", "Аккаунт с таким именем уже зарегистрирован!");
            return;
        }

        Account newAccount = new Account(ctx.Sender.Name, Guid.NewGuid().ToString(), "user", null, null, new Dictionary<string, string>());
        newAccount.SetPassword(password);
        newAccount.SetToken(ctx.Sender.UUID, ctx.Sender.IP);

        MintServer.AccountsCollection.Add(newAccount);

        ctx.Sender.Authorize(newAccount);

        ctx.Messenger.Send(MessageMark.OK, "Аккаунт", "Аккаунт создан!");
    }

    [StaticCommand("login", "войти в аккаунт", "<пароль>")]
    public static void Login(CommandInvokeContext ctx, string password)
    {
        if (ctx.Sender.Name == null || ctx.Sender.UUID == null) return;

        if (ctx.Sender.Account != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Аккаунт", "Вы уже вошли в аккаунт!");
            return;
        }

        Account? foundAccount = MintServer.AccountsCollection.Get(ctx.Sender.Name);
        if (foundAccount == null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Аккаунт", "Аккаунт с таким именем не зарегистрирован!");
            return;
        }

        if (!foundAccount.VerifyPassword(password))
        {
            ctx.Messenger.Send(MessageMark.Error, "Аккаунт", "Неверный пароль!");
            return;
        }

        ctx.Sender.Authorize(foundAccount);

        ctx.Messenger.Send(MessageMark.OK, "Аккаунт", $"С возвращением, {foundAccount.Name}!");
    }

    [StaticCommand("logout", "выйти из аккаунта", null)]
    public static void Logout(CommandInvokeContext ctx, string password)
    {
        if (ctx.Sender.Name == null || ctx.Sender.UUID == null) return;

        if (ctx.Sender.Account != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Аккаунт", "Вы уже вошли в аккаунт!");
            return;
        }

        Account? foundAccount = MintServer.AccountsCollection.Get(ctx.Sender.Name);
        if (foundAccount == null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Аккаунт", "Аккаунт с таким именем не зарегистрирован!");
            return;
        }

        if (!foundAccount.VerifyPassword(password))
        {
            ctx.Messenger.Send(MessageMark.Error, "Аккаунт", "Неверный пароль!");
            return;
        }

        ctx.Sender.Authorize(foundAccount);

        ctx.Messenger.Send(MessageMark.OK, "Аккаунт", $"С возвращением, {foundAccount.Name}!");
    }
}