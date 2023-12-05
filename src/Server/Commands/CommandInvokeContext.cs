namespace Mint.Server.Commands;

public class CommandInvokeContext
{
    public CommandInvokeContext(Player sender, ICommand command, List<string> parameters)
    {
        Sender = sender;
        BaseCommand = command;
        Parameters = parameters.AsReadOnly();
    }

    public CommandInvokeContext(Player sender, ICommand command, IReadOnlyList<string> parameters)
    {
        Sender = sender;
        BaseCommand = command;
        Parameters = parameters;
    }

    /// <summary>
    /// Player that sent that command.
    /// </summary>
    public Player Sender { get; }

    /// <summary>
    /// Player messenger that allows to send output messages in Mint style.
    /// </summary>
    public PlayerMessenger Messenger => Sender.Messenger;

    /// <summary>
    /// Sender's reference to Terraria.Player.
    /// </summary>
    public TPlayer TPlayer => Sender.TPlayer;

    /// <summary>
    /// Command invoke parameters. Starts from first parameter (command name ignored). 
    /// Example of parsed parameters: "/command param0 param1 param2"
    /// </summary>
    public IReadOnlyList<string> Parameters;

    /// <summary>
    /// Base command that was invoked.
    /// </summary>
    public ICommand BaseCommand { get; }
}