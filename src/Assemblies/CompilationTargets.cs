namespace Mint.Assemblies;

public struct CompilationTargets
{
    public CompilationTargets(string? srcDirectory, string binDirectory, string? framework, string? customArgs)
    {
        SourceDirectory = srcDirectory;
        BinaryDirectory = binDirectory;
        Framework = framework;
        CustomArguments = customArgs;
    }


    /// <summary>
    /// Default project compilation directory. Example: "src"
    /// </summary>
    public string? SourceDirectory;

    /// <summary>
    /// Default project compilation directory. Example: "bin"
    /// </summary>
    public string BinaryDirectory;

    /// <summary>
    /// Default project framework. Example: "net6.0"
    /// </summary>
    public string? Framework;

    /// <summary>
    /// Custom compilation arguments.
    /// </summary>
    public string? CustomArguments;
}