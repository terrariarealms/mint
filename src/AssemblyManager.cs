using System.Reflection;
using Mint.Modules;

namespace Mint.Core;

public class AssemblyManager
{
    internal List<Assembly> dependencies = new List<Assembly>();
    internal List<ModuleAssembly>? modules;

    private ModulesLoader? _moduleLoader;


#region Assembly resolving
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
#endregion

#region Modules
    internal void LoadModules()
    {
        _moduleLoader = new ModulesLoader();
        modules = new List<ModuleAssembly>();

        var binModules = _moduleLoader.FindBinaryModules("modules");
        LoadProjects(_moduleLoader.FindProjectModules("modules"), CountOf(binModules));

        foreach (string binPath in binModules)
        {
            LoadModule(binPath);
        }

        _moduleLoader.Wait();

        CheckDependencies();
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


    private void LoadProjects(IEnumerable<string> modules, int binCount)
    {
        _moduleLoader?.SetupCompiler(CountOf(modules) + binCount + 2);
        foreach (string projPath in modules)
        {
            _moduleLoader?.CompileProject(projPath, CompileCallback);
        }
    }

    private void CompileCallback(byte result, CompileContext ctx, string? binDir, string? binFile, string? name)
    {
        if (result != CompileCallbackID.Compiled || binFile == null)
            return;

        LoadModule(binFile);
    }


    private void LoadModule(string binFile)
    {        
        (MintModule?, Assembly)? data = _moduleLoader?.LoadFrom(binFile);

        if (data?.Item1 == null)
        {
            Console.WriteLine($"loadModule {binFile}: \x1b[31mfailed.\x1b[0m");
            return;
        }

        ModuleAssembly moduleAssembly = new ModuleAssembly(binFile)
        {
            Module = data?.Item1,
            Assembly = data?.Item2
        };
        modules?.Add(moduleAssembly);
    }
#endregion

    private int CountOf<T>(IEnumerable<T> enumerable)
    {
        int count = 0;
        var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext())
            count++;

        return count;
    }

    private bool DependencyExists(string name)
    {
        return modules?.Find((m) => m.Module?.ModuleName + m.Module?.ModuleArchitecture == name) != null;
    }
}