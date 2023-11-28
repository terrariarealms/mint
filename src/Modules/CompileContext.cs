using Mint.Modules;

namespace Mint.Modules;

public struct CompileContext
{
    public CompileContext(string input, string framework, string? customArgs, CompileCallback callback)
    {
        InputPath = input;
        Framework = framework;
        CustomArguments = customArgs;
        Callback = callback;
    }
    
    public string InputPath;
    public string Framework;
    public string? CustomArguments;
    public CompileCallback Callback;
}