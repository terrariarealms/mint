namespace Mint.Assemblies;

public struct CompilationInfo
{
    // srcPath path: path/to/fucking/modules/directory/and/module/src
    // workingPath path: path/to/fucking/modules/directory/and/module
    // binDirectory: bin

    public CompilationInfo(string name, string workingPath, string srcPath, string binDirectory)
    {
        Name = name;
        WorkingPath = workingPath;
        SourcePath = srcPath;
        BinaryDirectory = binDirectory;
    }

    /// <summary>
    /// Name of target project name.
    /// </summary>
    public string Name;

    /// <summary>
    /// Source code directory.
    /// </summary>
    public string WorkingPath;

    /// <summary>
    /// Source code directory.
    /// </summary>
    public string SourcePath;

    /// <summary>
    /// Binary directory.
    /// </summary>
    public string BinaryDirectory;
}