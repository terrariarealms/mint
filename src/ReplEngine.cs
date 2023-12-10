using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Mint.Core;

internal class ReplEngine
{
    internal ScriptOptions options = ScriptOptions.Default;
    internal string usings = "";

    internal void Initialize()
    {
        options = options.WithOptimizationLevel(OptimizationLevel.Release)
                        .WithAllowUnsafe(true)
                        .WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview)
                        .WithImports()
                        .WithReferences(AppDomain.CurrentDomain.GetAssemblies());

        AddUsing("System");
        AddUsing("Terraria");
        AddUsing("Mint.Core");
        AddUsing("Mint.Server");
        AddUsing("Mint.Server.Auth");
        AddUsing("Mint.Server.Chat");
        AddUsing("Mint.Server.Commands");
        AddUsing("Mint.Network");
        AddUsing("Mint.Network.Incoming");
        AddUsing("Mint.Network.Outcoming");
        AddUsing("Mint.Data");
        AddUsing("Mint.Assemblies");
        AddUsing("Mint.Assemblies.Modules");
    }

    internal void AddUsing(string name) => usings += "using " + name + "; ";

    internal void RunCode(string code)
    {
        try
        {
            code = code.Replace("plr[", "MintServer.Players[")
                        .Replace("@", "MintServer.ServerPlayer");

            CSharpScript.RunAsync(usings + code, options);
        }
        catch (CompilationErrorException e)
        {
            Console.Error.WriteLine($"Compilation error: {string.Join(Environment.NewLine, e.Diagnostics)}");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Running error: {e.Message}");
        }
    }
}