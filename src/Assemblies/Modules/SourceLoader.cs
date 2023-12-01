
namespace Mint.Assemblies.Modules;

public class SourceLoader : IModuleLoader
{
    private string? _workingDirectory;
    private AssemblyCompiler? _compiler;

    public void Initialize(string workingDirectory)
    {
        _workingDirectory = workingDirectory;
        _compiler = new AssemblyCompiler(_workingDirectory, new CompilationTargets("src", "bin", "net6.0", null));
    }

    public IEnumerable<ModuleAssembly> LoadModules()
    {
        if (_workingDirectory == null || _compiler == null)
            throw new InvalidOperationException("SourceLoader was not initialized.");

        foreach (CompilationInfo info in _compiler.FindCandidates())
        {
            string? path = _compiler.CompileDll(info);
            if (path == null)
            {
                Console.WriteLine("SOURCE_LOAD >>> FAIL: " + info.WorkingPath);
                continue;
            }

            Console.WriteLine($"SOURCE_LOAD -> {path}");
            ModuleAssembly? assembly = LoaderUtils.TryLoadFrom(path);
            if (assembly != null)
                yield return assembly;
        }
    }
}