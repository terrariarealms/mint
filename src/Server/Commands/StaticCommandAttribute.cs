namespace Mint.Server.Commands;

public class StaticCommandAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;
    public readonly string? Syntax;

    public StaticCommandAttribute(string name, string description, string? syntax)
    {
        Name = name;
        Description = description;
        Syntax = syntax;
    }
}