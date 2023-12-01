namespace Mint.Assemblies.Modules;

public interface IModuleLoader
{
    public void Initialize(string workingDirectory);

    public IEnumerable<ModuleAssembly> LoadModules();
}