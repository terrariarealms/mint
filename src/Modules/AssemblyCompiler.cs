using System.Diagnostics;

namespace Mint.Modules;

// TODO stage 1: use .sln
// TODO stage 2: do not use sln
// TODO stage 3: rewrite all (ok i wrote that but idk how it works lol (black magic?))
// TODO stage 4: gief more monies slavaukraini 
public class AssemblyCompiler
{
    public AssemblyCompiler(int? count)
    {
        currentCompilations = count == null ? new List<Task>() : new List<Task>(count.Value);
    }

    internal List<Task> currentCompilations;

    /// <summary>
    /// Wait for all projects compilation end.
    /// </summary>
    public void Wait()
    {
        if (currentCompilations.Count > 0)
            Task.WhenAll(currentCompilations);
    }

    /// <summary>
    /// Compiles project async. Use Wait() for waiting when all projects will be compiled.
    /// </summary>
    /// <param name="ctx">Compile context</param>
    public void CompileAsync(CompileContext ctx)
    {
        Task task = new Task(() => Compile(ctx));
        task.Start();
        currentCompilations.Add(task);
    }

    /// <summary>
    /// Compiles project.
    /// </summary>
    /// <param name="ctx">Compile context</param>
    public void Compile(CompileContext ctx)
    {
        string srcDir = Path.Combine(ctx.InputPath, "src");
        string binDir = Path.Combine(ctx.InputPath, "bin");

        // modules/shitmodule -> shitmodule
        // modules/shitmodule/ -> shitmodule
        string[] fixedPath = ctx.InputPath.Replace("\\", "/").Split('/');
        string projectName = fixedPath[fixedPath.Length - 1] == "" ? fixedPath[fixedPath.Length - 2] : fixedPath[fixedPath.Length - 1]; 

        string binFile = Path.Combine(binDir, $"{projectName}.dll");

        if (!Directory.Exists(srcDir))
        {
            ctx.Callback(CompileCallbackID.DirectoryNotFound, ctx, null, null, null);
            return;
        }

        InvokeCompile(ctx, srcDir, binDir, projectName);
        byte result = File.Exists(binFile) ? CompileCallbackID.Compiled : CompileCallbackID.NotCompiled;

        ctx.Callback(result, ctx, binDir, binFile, projectName);
    }

    internal void InvokeCompile(CompileContext ctx, string srcDir, string binDir, string projectName)
    {
        if (Directory.Exists(binDir))
            Directory.Delete(binDir, true);

        if (!Directory.Exists(srcDir))
        {
            Console.WriteLine($"{projectName}: \x1b[31mCannot run compilation task: directory {projectName}/src does not exists.\x1b[0m");
            return;
        }

        string command = CreateCompileCommand(ctx, srcDir, binDir);
        ExecuteDotnet(command);
    }

    private string CreateCompileCommand(CompileContext ctx, string srcDir, string binDir)
    {
        string command = $"build {srcDir}/ -f {ctx.Framework} -o {binDir}";
        if (ctx.CustomArguments != null)
            command += " " + ctx.CustomArguments;

        return command;
    }

    private bool ExecuteDotnet(string command)
    {        
        Console.WriteLine($"dotnet command: \x1b[34mdotnet {command}\x1b[0m");

        ProcessStartInfo startInfo = new ProcessStartInfo("dotnet", command)
        {
            RedirectStandardOutput = true
        };
        Process? process = Process.Start(startInfo);
        process?.WaitForExit();

        return process != null;
    }
}