namespace Mint.Modules;

public delegate void CompileCallback(byte result, CompileContext ctx, string? binPath, string? binFile, string? name);