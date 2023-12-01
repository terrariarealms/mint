using System.Reflection;

namespace Mint.Assemblies.Modules;

public class ModuleAssembly
{
    internal ModuleAssembly(string path)
    {
        Path = path;
    }

    public readonly string Path;

    internal Assembly? Assembly;
    internal MintModule? Module;
}