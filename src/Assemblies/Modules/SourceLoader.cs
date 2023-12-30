namespace Mint.Assemblies.Modules;

public class SourceLoader : IModuleLoader
{
    private string? _workingDirectory;
    private AssemblyCompiler? _compiler;

    public void Initialize(string workingDirectory)
    {
        _workingDirectory = workingDirectory;
        _compiler = new AssemblyCompiler(_workingDirectory,
            new CompilationTargets("src", "bin", "net7.0", "-c Release"));
    }

    public IEnumerable<ModuleAssembly> LoadModules()
    {
        if (_workingDirectory == null || _compiler == null)
            throw new InvalidOperationException("SourceLoader was not initialized.");

        foreach (var info in _compiler.FindCandidates())
        {
            var path = _compiler.CompileDll(info);
            if (path == null)
            {
                Log.Error("SourceLoader: failed -> {path}", info.WorkingPath);
                continue;
            }

            Log.Information("SourceLoader: load -> {File}", path);
            var assembly = LoaderUtils.TryLoadFrom(path);
            if (assembly != null)
                yield return assembly;
        }
    }
}