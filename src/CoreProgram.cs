using Terraria;
using Terraria.Initializers;
using Terraria.Localization;

namespace Mint.Core;

internal static class CoreProgram
{
    public static AssemblyManager? AssemblyManager;

    private static void Main(string[] args)
    {
        AssemblyManager = new AssemblyManager();
        AssemblyManager.SetupResolving();
        AssemblyManager.LoadModules();

        Prepare(args, true);

        AssemblyManager.InvokeSetup();

        AssemblyManager.InvokeInitialize();
        StartServer();
    }

    static void Prepare(string[] args, bool monoArgs = true)
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
        Terraria.Main.dedServ = true;
        LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture);
    }

    static void StartServer()
    {
        using var main = new Terraria.Main();
        Lang.InitializeLegacyLocalization();
        LaunchInitializer.LoadParameters(main);
        main.DedServ();

        main.Run();
    }
}