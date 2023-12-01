namespace Mint.Assemblies.Modules;

public abstract class MintModule
{
    /// <summary>
    /// Name of module. Used in displaying module in console and resolving dependencies
    /// Valid name will be "mcpa" (Original name is Mint Core Packet API)
    /// </summary>
    public abstract string ModuleName { get; }

    /// <summary>
    /// Version of module. Used in displaying module in console. (not used in resolving dependecies)
    /// It can be '0.1-beta' '6.42.4-stable'.
    /// </summary>
    public abstract string ModuleVersion { get; }

    /// <summary>
    /// Names of modules that module is using.
    /// </summary>
    public abstract string[]? ModuleReferences { get; }

    /// <summary>
    /// Version of module architecture. 
    /// If you have changes that change syntax of interacting with module then you need to increase value.
    /// </summary>
    public abstract int ModuleArchitecture { get; }

    /// <summary>
    /// Setup invoking in async module loading.
    /// Can be used only for initializing without reference to other modules
    /// </summary>
    public abstract void Setup();

    /// <summary>
    /// Initialize invoking when ALL modules was handled Setup();
    /// It can be used if you need to work with other modules.
    /// </summary>
    public abstract void Initialize();
}