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
        foreach (var directory in Directory.EnumerateDirectories(WorkingDirectory))
        {
            // gets name of module
            var name = directory.Replace('\\', '/').Split('/').Last();
            if (name == null)
            {
                Log.Error("AssemblyCompiler for {Directory}: name is null.", directory);
                continue;
            }

            // source code directory path
            string? srcPath = null;

            if (_targets.SourceDirectory != null)
            {
                srcPath = Path.Combine(directory, _targets.SourceDirectory);
                if (!Directory.Exists(srcPath))
                {
                    Log.Error("AssemblyCompiler for {Directory}: source code directory path not exists -> {Path}",
                        directory, srcPath);
                    continue;
                }

                // sets root/src directory as src directory (if target src directory is not null (else skip that))
            }
            // sets root directory as src directory
            else
            {
                srcPath = directory;
            }

            // check for existing .csproj file with name of directory.
            var csprojPath = Path.Combine(srcPath, $"{name}.csproj");
            if (!File.Exists(csprojPath))
            {
                Log.Error("AssemblyCompiler for {Directory}: .csproj file path not exists -> {Path}", directory,
                    csprojPath);
                continue;
            }

            var compilationInfo = new CompilationInfo(name, directory, srcPath, _targets.BinaryDirectory);
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
        var binPath = Path.Combine(info.WorkingPath, info.BinaryDirectory);
        var objPath = Path.Combine(info.WorkingPath, info.SourcePath, "obj");
        if (Directory.Exists(binPath))
            Directory.Delete(binPath, true);

        var ignoreRestoring = Directory.Exists(objPath);

        var binFilePath = Path.Combine(binPath, info.Name + ".dll");

        var command = BuildCommand(info);
        if (ignoreRestoring)
            command += "--no-restore";

        if (RunCommand(command, true))
        {
            if (File.Exists(binFilePath))
                return binFilePath;
            else
                Log.Error("AssemblyCompiler for {Directory}: binary files directory path not exists -> {Path}",
                    info.WorkingPath, binFilePath);
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
        var binPath = Path.Combine(info.WorkingPath, info.BinaryDirectory);

        var command = $"build {info.SourcePath}/ -o {binPath}";

        if (_targets.Framework != null) command += $" -f " + _targets.Framework;
        if (_targets.CustomArguments != null) command += " " + _targets.CustomArguments;

        Log.Information("AssemblyCompiler for {directory}: dotnet -> {Command}", command);

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
        var startInfo = new ProcessStartInfo("dotnet", command)
        {
            RedirectStandardOutput = redirectOutput
        };
        var process = Process.Start(startInfo);
        process?.WaitForExit();

        return process != null;
    }
}