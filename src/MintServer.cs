using Terraria;
using Terraria.Initializers;
using Terraria.Localization;

namespace Mint.Core;

public static class MintServer
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal static AssemblyManager AssemblyManager;

    internal static MintConfig Config;
    internal static ConfigUtils ConfigUtils = new ConfigUtils("core");

    public static DatabaseUtils DatabaseUtils { get; private set; }

    public static CommandsManager Commands { get; private set; } = new CommandsManager();
    public static NetworkHandler Network { get; private set; } = new NetworkHandler();
    public static PlayersManager Players { get; private set; } = new PlayersManager();
    public static ChatManager Chat { get; private set; } = new ChatManager();

    public static DynamicPlayer ServerPlayer { get; } = new DynamicPlayer("root", new Account("root", "0", "root", null, null, new Dictionary<string, string>()), new DynamicMessenger("root", true));

    public static DatabaseCollection<Account> AccountsCollection { get; private set; }
    public static DatabaseCollection<Group> GroupsCollection { get; private set; }

    internal static ReplEngine ReplEngine = new ReplEngine();

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    static void Main(string[] args)
    {
        ReplEngine.Initialize();

        AssemblyManager = new AssemblyManager();
        AssemblyManager.SetupResolving();
        AssemblyManager.LoadModules();

        if (!Directory.Exists("data"))
            Directory.CreateDirectory("data");

        Config = ConfigUtils.GetConfig<MintConfig>();
        DatabaseUtils = new DatabaseUtils();

        AccountsCollection = DatabaseUtils.GetDatabase<Account>();       
        GroupsCollection = DatabaseUtils.GetDatabase<Group>();
        InsertDefaultGroups();

        Prepare(args, true);

        Commands.InitializeParsers();

        CoreCommands.Register();
        
        Players.Initialize();
        Network.Initialize();

        Chat.Initialize();

        // TileFix removes caching 
        TileFix.Fix();

        AssemblyManager.InvokeSetup();

        AssemblyManager.InvokeInitialize();
        StartServer();
    }

    static void InsertDefaultGroups()
    {
        if (GroupsCollection.Get("unauthorized") == null)
        {
            Group unauthorized = new Group("unauthorized", false, null, new GroupPresence(null, null, new MintColor(85, 85, 85)), new List<DatabaseObject>(), new List<string>()
            {
                "mint.register",
                "mint.login"
            });

            GroupsCollection.Add(unauthorized);
        }

        if (GroupsCollection.Get("user") == null)
        {
            Group user = new Group("user", false, null, new GroupPresence(null, null, new MintColor(85, 85, 85)), new List<DatabaseObject>(), new List<string>()
            {
                "mint.logout"
            });

            GroupsCollection.Add(user);
        }

        if (GroupsCollection.Get("root") == null)
        {
            Group root = new Group("root", true, null, new GroupPresence(null, null, new MintColor(85, 85, 85)), new List<DatabaseObject>(), new List<string>());

            GroupsCollection.Add(root);
        }
    }

    static void CliReader()
    {
        while (true)
        {
            string? command = Console.ReadLine();
            if (command == null) continue;

            if (command.StartsWith("/"))
            {
                Commands.InvokeCommand(ServerPlayer, command);
                continue;
            }

            ReplEngine.RunCode(command);    
        }
    }

#region Terraria Server Startup
    static void Prepare(string[] args, bool monoArgs = true)
    {
        // 🌿🌿🌿🌿🌿🌿🌿🌿
        //     |
        //   \ | /    04:20
        //  __\|/__
        // 🌿🌿🌿🌿🌿🌿🌿🌿

        Thread.CurrentThread.Name = "Main Thread";
        if (monoArgs) args = Terraria.Utils.ConvertMonoArgsToDotNet(args);
        Program.LaunchParameters = Terraria.Utils.ParseArguements(args);
        Program.SavePath = Path.Combine("data");
        ThreadPool.SetMinThreads(8, 8);
        Program.InitializeConsoleOutput();
        Program.SetupLogging();
        //Platform.Get<IWindowService>().SetQuickEditEnabled(false);
        Terraria.Main.dedServ = true;
        LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture);
    }

    static void StartServer()
    {
        using var main = new Terraria.Main();
        Lang.InitializeLegacyLocalization();
        LaunchInitializer.LoadParameters(main);

        if (Config.Game.UseSSC) Terraria.Main.ServerSideCharacter = true;

        On.Terraria.Main.startDedInputCallBack += (x) => CliReader();
        main.DedServ();

        main.Run();
    }
#endregion
}