using System.Reflection;

namespace Mint.Assemblies.Modules;

internal static class LoaderUtils
{
    internal static ModuleAssembly? TryLoadFrom(string path)
    {
        try
        {
            return LoadFrom(path);
        }
        catch
        {
            return null;
        }
    }

    internal static ModuleAssembly? LoadFrom(string path)
    {
        Assembly assembly = Assembly.LoadFrom(path);

        if (assembly == null)
            return null;

        return LoadFrom(assembly, path);
    }

    internal static ModuleAssembly? LoadFrom(Assembly assembly, string path)
    {
        Type moduleType = typeof(MintModule);
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsSubclassOf(moduleType))
            {
                object? objInstance = Activator.CreateInstance(type);
                if (objInstance == null) return null;
                
                MintModule instance = (MintModule)objInstance;

                ModuleAssembly moduleAssembly = new ModuleAssembly(path)
                {
                    Assembly = assembly,
                    Module = instance
                };

                return moduleAssembly;
            }
        }

        return null;
    }
}