namespace Mint.API;

public static class ModuleUtils
{
    /// <summary>
    /// Check for having target module loaded.
    /// </summary>
    /// <param name="name">Module name</param>
    /// <param name="architecture">Module architecture</param>
    /// <returns>Does that module exist and loaded.</returns>
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