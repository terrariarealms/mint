namespace Mint.Assemblies.Modules;

public class BinaryLoader : IModuleLoader
{
    private string? _workingDirectory;

    public void Initialize(string workingDirectory)
    {
        _workingDirectory = workingDirectory;
    }

    public IEnumerable<ModuleAssembly> LoadModules()
    {
        if (_workingDirectory == null)
            throw new InvalidOperationException("BinaryLoader was not initialized.");

        foreach (var file in Directory.EnumerateFiles(_workingDirectory, "*.dll"))
        {
            Log.Information("BinaryLoader: load -> {File}", file);
            var assembly = LoaderUtils.TryLoadFrom(file);
            if (assembly != null)
                yield return assembly;
        }
    }
}