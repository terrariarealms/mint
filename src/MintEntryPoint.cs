namespace Mint.Core;

internal static class MintEntryPoint
{
    private static void Main(string[] args)
    {
        var assemblyManager = new AssemblyManager();
        // assemblyManager.SetupResolving();
        StartMint(args, assemblyManager);
    }

    private static void StartMint(string[] args, AssemblyManager assemblyManager)
    {
        MintServer.AssemblyManager = assemblyManager;
        try
        {
            MintServer.Initialize(args);
        }
        catch (Exception ex)
        {
            MintServer.DisplayException(ex);
        }
    }
}