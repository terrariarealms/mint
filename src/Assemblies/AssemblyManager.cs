using System.Reflection;
using Mint.Assemblies.Modules;

namespace Mint.Assemblies;

internal class AssemblyManager
{
    internal const string WorkingDirectory = "modules";

    internal List<Assembly> dependencies = new List<Assembly>();
    internal List<ModuleAssembly>? modules;

    internal void SetupResolving()
    {
        AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
    }

    private Assembly? FindAssembly(string fileName, string extension)
    {
        var array = Directory.EnumerateFiles("deps", $"*{fileName}.{extension}", SearchOption.AllDirectories)
            .ToArray();
        if (array.Length == 0) return null;

        Assembly assembly = Assembly.LoadFrom(array[0]);
        dependencies.Add(assembly);
        return assembly;
    }

    private Assembly? ResolveAssembly(object? sender, ResolveEventArgs sargs)
    {
        var name = new AssemblyName(sargs.Name).Name;
        if (name == null)
            return null;

        if (modules != null)
        {
            ModuleAssembly? moduleAssembly = modules.Find((p) => p.Assembly?.GetName().Name == name);
            if (moduleAssembly?.Assembly != null)
                return moduleAssembly.Assembly;
        }

        Assembly? cachedAssembly = dependencies.Find((p) => p.GetName().Name == name);
        if (cachedAssembly != null)
            return cachedAssembly;

        Assembly? assembly = FindAssembly(name, "dll") ?? FindAssembly(name, "exe");
        if (assembly == null)
            Console.WriteLine($"\x1b[31mCannot resolve assembly {name}.\x1b[0m");

        return assembly;
    }
    
    internal void LoadModules()
    {
        modules = new List<ModuleAssembly>();

        LoadVia(new SourceLoader());
        LoadVia(new BinaryLoader());

        CheckDependencies();
    }

    internal void LoadVia(IModuleLoader loader)
    {
        loader.Initialize(WorkingDirectory);
        modules?.AddRange(loader.LoadModules());
    }

    internal void InvokeSetup()
    {
        if (modules == null) return;

        foreach (ModuleAssembly asm in modules)
            asm.Module?.Setup();
    }

    internal void InvokeInitialize()
    {
        if (modules == null) return;

        foreach (ModuleAssembly asm in modules)
            asm.Module?.Initialize();
    }

    private void CheckDependencies()
    {
        if (modules == null) return;

        //foreach (ModuleAssembly asm in modules)

        bool sleep = false;
        modules.ForEach((m) => CheckDependenciesFor(m.Module, ref sleep));

        if (sleep)
        {
            Console.WriteLine("");
            Console.WriteLine("requested server sleep by dependencies issues.");
            Console.WriteLine("please fix all problems above and try again.");
            Thread.Sleep(-1);
        }
    }

    private void CheckDependenciesFor(MintModule? module, ref bool sleep)
    {
        if (module?.ModuleReferences == null) return;

        string[] refs = module.ModuleReferences;
        for (int i = 0 ; i < refs.Length; i++)
        {
            if (!DependencyExists(refs[i]))
            {
                Console.WriteLine($"\x1b[31mcritical\x1b[0m {module?.ModuleName}: \x1b[31mmodule ref'{refs[i]}' not found!\x1b[0m");
            
                sleep = true;
            }
        }
    }

    private bool DependencyExists(string name)
    {
        return modules?.Find((m) => m.Module?.ModuleName + m.Module?.ModuleArchitecture == name) != null;
    }
}