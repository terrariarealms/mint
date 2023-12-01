
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

        foreach (string file in Directory.EnumerateFiles(_workingDirectory, "*.dll"))
        {
            Console.WriteLine($"BINARY_LOAD -> {file}");
            ModuleAssembly? assembly = LoaderUtils.TryLoadFrom(file);
            if (assembly != null)
                yield return assembly;
        }
    }
}