using System.Net;
using Serilog;
using Serilog.Sinks.SpectreConsole;
using Serilog.Events;
using Terraria;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.Net.Sockets;
using Terraria.Utilities;
using Mint.Localization;

namespace Mint.Core;

public static class MintServer
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal static AssemblyManager AssemblyManager;

    internal static MintConfig Config;

#if REPL
    internal static ReplEngine ReplEngine = new ReplEngine();
#endif

    public static CommandsManager Commands { get; private set; } = new();
    public static NetworkHandler Network { get; private set; } = new();
    public static PlayersManager Players { get; private set; } = new();
    public static ChatManager Chat { get; private set; } = new();

    public static LocalizationManager Localization { get; } = new();

    public static DynamicPlayer ServerPlayer { get; } = new("root",
        new Account("root", "0", "root", null, null, new Dictionary<string, string>()),
        new DynamicMessenger("root", true));

    public static DatabaseStorage<Account> AccountsCollection { get; private set; }
    public static DatabaseStorage<Group> GroupsCollection { get; private set; }

    public static ISocket ServerSocket { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    internal static void DisplayException(Exception ex)
    {
        Log.Error("Error in server initialization: ");
        Log.Error("{Exception}", ex.ToString());
    }

    internal static void Initialize(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("mint.log", LogEventLevel.Verbose,
                "[{Timestamp:HH:mm:ss:ff} | {Level:u4}]: {Message:lj}{NewLine}{Exception}")
            .WriteTo.SpectreConsole("[{Timestamp:HH:mm:ss:ff} | {Level:u4}]: {Message:lj}{NewLine}{Exception}",
                LogEventLevel.Verbose)
            .MinimumLevel.Verbose()
            .CreateLogger();

        var russianLang = new LocalizationContainer();
        russianLang.ImportFrom(File.ReadAllText("mint_localization_russian.json"), false, true);

        Localization.AddContainer(LanguageID.Russian, russianLang);

#if REPL
        ReplEngine.Initialize();
#endif

        AssemblyManager.LoadModules();

        if (!Directory.Exists("data"))
            Directory.CreateDirectory("data");

        Config = Config<MintConfig>.GetData();

        AccountsCollection = MongoDatabase.Get<Account>();
        GroupsCollection = MongoDatabase.Get<Group>();
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

        ServerSocket = new RemadeTcpSocket();

        AssemblyManager.InvokeInitialize();
        MainHooks.AttachHooks();
        ItemHooks.AttachHooks();
        StartServer();
    }

    private static void InsertDefaultGroups()
    {
        if (GroupsCollection.Get("unauthorized") == null)
        {
            Log.Information("Registering group {Name}", "unauthorized");

            var unauthorized = new Group("unauthorized", false, null,
                new GroupPresence(null, null, new MintColor(85, 85, 85)), new List<DatabaseObject>(), new List<string>()
                {
                    "mint.register",
                    "mint.login"
                });

            GroupsCollection.Push("unauthorized", unauthorized);
        }

        if (GroupsCollection.Get("user") == null)
        {
            Log.Information("Registering group {Name}", "user");

            var user = new Group("user", false, null, new GroupPresence(null, null, new MintColor(85, 85, 85)),
                new List<DatabaseObject>(), new List<string>()
                {
                    "mint.logout"
                });

            GroupsCollection.Push("user", user);
        }

        if (GroupsCollection.Get("root") == null)
        {
            Log.Information("Registering group {Name}", "root");

            var root = new Group("root", true, null, new GroupPresence(null, null, new MintColor(85, 85, 85)),
                new List<DatabaseObject>(), new List<string>());

            GroupsCollection.Push("root", root);
        }
    }

    private static void CliReader()
    {
        while (true)
        {
            var command = Console.ReadLine();
            if (command == null) continue;

            if (command.StartsWith("/"))
            {
                Commands.InvokeCommand(ServerPlayer, command, true);
                continue;
            }

#if REPL
            ReplEngine.RunCode(command);
#endif
        }
    }

    #region Terraria Server Startup

    private static void Prepare(string[] args, bool monoArgs = true)
    {
        // 🌿🌿🌿🌿🌿🌿🌿🌿
        //     |
        //   \ | /    04:20
        //  __\|/__
        // 🌿🌿🌿🌿🌿🌿🌿🌿

        Thread.CurrentThread.Name = "Main Thread";
        if (monoArgs) args = Utils.ConvertMonoArgsToDotNet(args);
        Program.LaunchParameters = Utils.ParseArguements(args);
        Program.SavePath = Path.Combine("data");
        ThreadPool.SetMinThreads(8, 8);
        Program.InitializeConsoleOutput();
        Program.SetupLogging();
        //Platform.Get<IWindowService>().SetQuickEditEnabled(false);
        Main.dedServ = true;
        LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture);
    }

    private static void StartServer()
    {
        using var main = new Main();
        Lang.InitializeLegacyLocalization();
        LaunchInitializer.LoadParameters(main);

        On.Terraria.Main.startDedInputCallBack += (x) => CliReader();
        On.Terraria.Netplay.InitializeServer += (x) =>
        {
            Log.Information("NetplayHijack -> InitializeServer()");
            Netplay.Connection.ResetSpecialFlags();
            Netplay.ResetNetDiag();
            if (Main.rand == null) Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
            Main.myPlayer = 255;
            Netplay.ServerIP = IPAddress.Any;
            Main.menuMode = 14;
            Main.netMode = 2;
            Netplay.Disconnect = false;
            for (var i = 0; i < 256; i++)
            {
                Netplay.Clients[i] = new RemoteClient();
                Netplay.Clients[i].Reset();
                Netplay.Clients[i].Id = i;
                Netplay.Clients[i].ReadBuffer = new byte[1024];
            }

            Netplay.TcpListener = ServerSocket;
            Log.Information("NetplayHijack -> using {Name} socket.", ServerSocket.GetType().FullName);
            if (!Netplay.Disconnect)
            {
                if (!Netplay.StartListening())
                {
                    Log.Error("NetplayHijack -> Cannot start listening -> port already used.");
                    Netplay.SaveOnServerExit = false;
                    Netplay.Disconnect = true;
                }

                Log.Information("NetplayHijack -> Server started.");
            }
        };
        main.DedServ();

        main.Run();
    }

    #endregion
}