namespace Mint.Core;

internal static class MintEntryPoint
{
    static void Main(string[] args)
    {
        AssemblyManager assemblyManager = new AssemblyManager();
        // assemblyManager.SetupResolving();
        StartMint(args, assemblyManager);
    }
    static void StartMint(string[] args, AssemblyManager assemblyManager)
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