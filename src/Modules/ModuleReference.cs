namespace Mint.Modules;

public struct ModuleReference
{
    public ModuleReference(bool required, string moduleName, int moduleArch)
    {
        Required = required;
        ModuleName = moduleName;
        ModuleArchitecture = moduleArch;
    }

    public readonly bool Required;
    public readonly string ModuleName;
    public readonly int ModuleArchitecture;
}