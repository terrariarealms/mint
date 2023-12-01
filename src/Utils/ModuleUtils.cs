namespace Mint.Utils;

public static class ModuleUtils
{
    public static bool IsModuleLoaded(string name, int? architecture)
    {
        if (MintServer.AssemblyManager?.modules == null)
            return false;

        foreach (ModuleAssembly asm in MintServer.AssemblyManager.modules)
        { 
            bool validArch = (architecture != null && asm.Module?.ModuleArchitecture == architecture) || architecture == null;
            if (asm.Module?.ModuleName == name && validArch)
                return true;
        }

        return false;
    }
}