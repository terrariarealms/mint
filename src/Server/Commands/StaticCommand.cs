using System.Reflection;

namespace Mint.Server.Commands;

public class StaticCommand : ICommand
{
    internal StaticCommand(MethodInfo method, string name, string description, string? syntax, string? permission, CommandFlags flags)
    {
        this.method = method;
        parameters = method.GetParameters();

        Name = name;
        Description = description;
        Syntax = syntax;
        Permission = permission;
        Flags = flags;
    }

    public string Name { get; }

    public string Description { get; }

    public string? Syntax { get; }

    public string? Permission { get; }

    public CommandFlags Flags { get; }

    internal MethodInfo method;
    internal ParameterInfo[] parameters;

    public void Invoke(CommandInvokeContext ctx)
    {
        List<object>? invokeParameters = BuildParameters(ctx);
        if (invokeParameters != null)
        {
            method.Invoke(null, invokeParameters.ToArray());
        }
    }

    private List<object>? BuildParameters(CommandInvokeContext ctx)
    {
        List<object> invokeParameters = new List<object>(parameters.Length)
        {
            ctx
        };

        string commandSource = "/" + Name;

        for (int i = 1; i < parameters.Length; i++)
        {
            ParameterInfo parameter = parameters[i];

            if (ctx.Parameters.Count < i)
            {
                if (parameter.HasDefaultValue && parameter.DefaultValue != null)
                {
                    invokeParameters.Add(parameter.DefaultValue);
                    continue;
                }

                ctx.Messenger.Send(MessageMark.Error, commandSource, $"Command was failed [argument {i}]: ");
                ctx.Messenger.Send(MessageMark.Error, commandSource, "Not enough arguments.");
                return null;
            }

            object value;
            ParseResult result = MintServer.Commands.TryParse(parameter.ParameterType, ctx.Parameters[i - 1], out value);
            
            switch (result)
            {
                case ParseResult.ParserNotFound:
                    ctx.Messenger.Send(MessageMark.Error, commandSource, $"Command was failed [argument {i}]: ");
                    ctx.Messenger.Send(MessageMark.Error, commandSource, $"Cannot find parser for type {parameter.ParameterType.Name}.");
                    break;

                case ParseResult.InvalidArgument:
                    ctx.Messenger.Send(MessageMark.Error, commandSource, $"Command was failed [argument {i}]: ");
                    ctx.Messenger.Send(MessageMark.Error, commandSource, $"Cannot parse '{ctx.Parameters[i - 1]}' to {parameter.Name} ({parameter.ParameterType.Name}).");
                    break;

                case ParseResult.Success:
                    invokeParameters.Add(value);
                    break;
            }
        }
        return invokeParameters;
    }
}
