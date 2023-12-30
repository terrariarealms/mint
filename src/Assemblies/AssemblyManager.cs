using System.Reflection;
using System.Runtime.Loader;
using Mint.Assemblies.Modules;

namespace Mint.Assemblies;

internal class AssemblyManager
{
    internal const string WorkingDirectory = "modules";

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

        var assembly = Assembly.LoadFrom(array[0]);
        return assembly;
    }

    private Assembly? ResolveAssembly(object? sender, ResolveEventArgs sargs)
    {
        var name = new AssemblyName(sargs.Name).Name;
        if (name == null)
            return null;

        if (modules != null)
        {
            var moduleAssembly = modules.Find((p) => p.Assembly?.GetName().Name == name);
            if (moduleAssembly?.Assembly != null)
                return moduleAssembly.Assembly;
        }

        var assembly = FindAssembly(name, "dll") ?? FindAssembly(name, "exe");
        if (assembly == null) LogResolveFail(name);

        return assembly;
    }

    private void LogResolveFail(string name)
    {
        Log.Error("AssemblyManager -> failed to resolve {Name}", name);
    }

    internal void LoadModules()
    {
        Log.Information("AssemblyManager -> LoadModules()");

        if (!Directory.Exists(WorkingDirectory))
            Directory.CreateDirectory(WorkingDirectory);

        modules = new List<ModuleAssembly>();

        LoadVia(new SourceLoader());
        LoadVia(new BinaryLoader());

        modules = modules.OrderBy(p => p.Module.Priority).ToList();

        CheckDependencies();
    }

    internal void LoadVia(IModuleLoader loader)
    {
        Log.Information("AssemblyManager -> LoadVia(IModuleLoader)");

        loader.Initialize(WorkingDirectory);
        modules?.AddRange(loader.LoadModules());
    }

    internal void InvokeSetup()
    {
        Log.Information("AssemblyManager -> InvokeSetup()");

        if (modules == null) return;

        foreach (var asm in modules)
            asm.Module?.Setup();
    }

    internal void InvokeInitialize()
    {
        Log.Information("AssemblyManager -> InvokeInitialize()");

        if (modules == null) return;

        foreach (var asm in modules)
            asm.Module?.Initialize();
    }

    private void CheckDependencies()
    {
        Log.Information("AssemblyManager -> CheckDependencies()");

        if (modules == null) return;

        var sleep = false;
        modules.ForEach((m) =>
        {
            if (m.Module != null) CheckDependenciesFor(m.Module, ref sleep);
        });

        if (sleep)
        {
            Log.Information("AssemblyManager -> Cannot start server: update your modules and read all above.");
            Thread.Sleep(-1);
        }
    }

    private void CheckDependenciesFor(MintModule module, ref bool sleep)
    {
        if (module?.ModuleReferences == null) return;

        string[] refs = module.ModuleReferences;
        for (var i = 0; i < refs.Length; i++)
            if (!DependencyExists(refs[i]))
            {
                Log.Error("AssemblyManager -> CheckDependenciesFor() for {Name} -> failed to resolve {Dependency}",
                    module.ModuleName, refs[i]);

                sleep = true;
            }
    }

    private bool DependencyExists(string name)
    {
        return modules?.Find((m) => m.Module?.ModuleName + m.Module?.ModuleArchitecture == name) != null;
    }
}