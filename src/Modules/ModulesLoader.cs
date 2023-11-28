using System.Reflection;
using Mint.Modules;

namespace Mint.Modules;

public class ModulesLoader
{
    internal const string ModulesDirectory = "modules";

    private AssemblyCompiler? _compiler;

    internal void Destroy() => _compiler = null;

    internal void Wait() => _compiler?.Wait();

    public IEnumerable<string> FindProjectModules(string path)
    {
        IEnumerable<string> dirs = Directory.EnumerateDirectories(path);

        foreach (string dir in dirs)
        {
            string csprojName = $"{dir.Substring(path.Trim('/').Trim('\\').Length + 1)}.csproj";
            string csprojPath = Path.Combine(dir, "src", csprojName);

            if (!File.Exists(csprojPath))
            {
                Console.WriteLine($"findBin {path} ->  \x1b[31mskipped. '{csprojPath}' not found!\x1b[0m");
            }
            else
            {
                Console.WriteLine($"findBin {path} -> \x1b[34m{csprojPath}.\x1b[0m");
            
                yield return dir;
            }
        }
    }

    public IEnumerable<string> FindBinaryModules(string path)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(path, "*.dll", SearchOption.TopDirectoryOnly);

        foreach (string file in files)
        {
            Console.WriteLine($"findBin {path} -> \x1b[34m{file}.\x1b[0m");
            yield return file;
        }
    }

    public void SetupCompiler(int count)
    {
        if (_compiler != null)
        {
            Console.WriteLine($"\x1b[31msetup compiler fail: _compiler != null.\x1b[0m");
            return;
        }
        _compiler = new AssemblyCompiler(count);
    }

    public void CompileProject(string name, CompileCallback callback)
    {
        string fullPath = name.StartsWith(ModulesDirectory) ? name : Path.Combine(ModulesDirectory, name);
        string objPath = Path.Combine(fullPath, "src", "obj");

        if (!Directory.Exists(fullPath))
        {
            Console.WriteLine($"[Load] {name}: \x1b[31mCannot load module from project: directory {name} does not exists.\x1b[0m");
            return;
        }

        if (_compiler == null)
        {
            Console.WriteLine("\x1b[31mcompilation fail: _compiler == null.\x1b[0m");
            return;
        }

        Console.WriteLine($"[Load] {name}: \x1b[34mCompiling {name}...\x1b[0m");
            
        string? customArgs = Directory.Exists(objPath) ? "--no-restore" : null;
        CompileContext ctx = new CompileContext(fullPath, "net6.0", customArgs, callback);
        _compiler.Compile(ctx);
    }

    internal (MintModule?, Assembly)? LoadFrom(string filePath)
    {
        try
        {
            Assembly assembly = Assembly.LoadFrom(filePath);

            Type moduleType = typeof(MintModule);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(moduleType))
                {
                    object? instance = Activator.CreateInstance(type);
                    MintModule? module = instance is MintModule ? (MintModule)instance : null;
                    Console.WriteLine($"loadFrom {filePath}: \x1b[32mload was successfully: '{module?.ModuleName}' v{module?.ModuleVersion.ToString()} arch_{module?.ModuleArchitecture}\x1b[0m");
                    return (module, assembly);
                }
            }

            Console.WriteLine($"loadFrom {filePath}: \x1b[31mskipped: no MintModule.\x1b[0m");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"loadFrom {filePath}: \x1b[31mskipped by exception: {ex.Message}.\x1b[0m");
            Console.WriteLine($"\x1b[31m{ex.ToString()}\x1b[0m");
            return null;
        }
    }
}