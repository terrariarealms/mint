using System.Diagnostics;

namespace Mint.Assemblies;

public class AssemblyCompiler
{
    public AssemblyCompiler(string workingDirectory, CompilationTargets targets)
    {
        _workingDirectory = workingDirectory;
        _targets = targets;
    }

    public string WorkingDirectory => _workingDirectory;
    public CompilationTargets CompilationTargets => _targets;

    private string _workingDirectory;
    private CompilationTargets _targets;

    /// <summary>
    /// Checks for having candidates in working directory.
    /// </summary>
    /// <returns>Candidates</returns>
    public IEnumerable<CompilationInfo> FindCandidates()
    {
        foreach (string directory in Directory.EnumerateDirectories(WorkingDirectory))
        {
            // gets name of module
            string? name = directory.Replace('\\', '/').Split('/').Last();
            if (name == null)
            {
                Console.WriteLine($"Skip {directory}: name is NULL: {name}");
            }

            // source code directory path
            string? srcPath = null;

            if (_targets.SourceDirectory != null)
            {
                srcPath = Path.Combine(directory, _targets.SourceDirectory);
                if (!Directory.Exists(srcPath)) 
                {
                    Console.WriteLine($"Skip {directory}: srcPath not exists: {srcPath}");
                    continue;
                }

                // sets root/src directory as src directory (if target src directory is not null (else skip that))
            }
            // sets root directory as src directory
            else srcPath = directory;

            // check for existing .csproj file with name of directory.
            string csprojPath = Path.Combine(srcPath, $"{name}.csproj");
            if (!File.Exists(csprojPath))
            {
                Console.WriteLine($"Skip {directory}: csprojPath not exists: {csprojPath}");
                continue;
            }

            CompilationInfo compilationInfo = new CompilationInfo(name, directory, srcPath, _targets.BinaryDirectory);
            yield return compilationInfo;
        }
    }

    /// <summary>
    /// Compiles project as DLL.
    /// </summary>
    /// <param name="info">Compilation info</param>
    /// <returns>Assembly path</returns>
    public string? CompileDll(CompilationInfo info)
    {
        string binPath = Path.Combine(info.WorkingPath, info.BinaryDirectory);
        if (Directory.Exists(binPath))
            Directory.Delete(binPath, true);

        string binFilePath = Path.Combine(binPath, info.Name + ".dll");

        string command = BuildCommand(info);
        if (RunCommand(command, false))
        {
            if (File.Exists(binFilePath))
                return binFilePath;
            else Console.WriteLine($"Skip {info.WorkingPath}: binFilePath not exists: {binFilePath}");
        }

        return null;
    } 

    /// <summary>
    /// Builds dotnet command.
    /// </summary>
    /// <param name="info">CompilationInfo</param>
    /// <returns>Dotnet command</returns>
    public string BuildCommand(CompilationInfo info)
    {
        string binPath = Path.Combine(info.WorkingPath, info.BinaryDirectory);

        string command = $"build {info.SourcePath}/ -o {binPath}";

        if (_targets.Framework != null) command += $" -f " + _targets.Framework;
        if (_targets.CustomArguments != null) command += " " + _targets.CustomArguments;

        return command;
    }

    /// <summary>
    /// Runs dotnet command.
    /// </summary>
    /// <param name="command">Target command</param>
    /// <param name="redirectOutput">RedirectStandardOutput</param>
    /// <returns>Command's running success</returns>
    public bool RunCommand(string command, bool redirectOutput = true)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("dotnet", command)
        {
            RedirectStandardOutput = redirectOutput
        };
        Process? process = Process.Start(startInfo);
        process?.WaitForExit();

        return process != null;
    }
}