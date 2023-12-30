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
        var assembly = Assembly.LoadFrom(path);

        if (assembly == null)
            return null;

        return LoadFrom(assembly, path);
    }

    internal static ModuleAssembly? LoadFrom(Assembly assembly, string path)
    {
        var moduleType = typeof(MintModule);
        foreach (var type in assembly.GetTypes())
            if (type.IsSubclassOf(moduleType))
            {
                var objInstance = Activator.CreateInstance(type);
                if (objInstance == null) return null;

                var instance = (MintModule)objInstance;

                var moduleAssembly = new ModuleAssembly(path, assembly, instance);

                return moduleAssembly;
            }

        return null;
    }
}