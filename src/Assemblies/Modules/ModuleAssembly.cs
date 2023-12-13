using System.Reflection;

namespace Mint.Assemblies.Modules;

public class ModuleAssembly
{
    internal ModuleAssembly(string path, Assembly assembly, MintModule module)
    {
        Path = path;
        Assembly = assembly;
        Module = module;
    }

    public readonly string Path;

    internal Assembly Assembly;
    internal MintModule Module;
}