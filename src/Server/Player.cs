using System.Collections.Concurrent;
using Terraria;

namespace Mint.Server;

public partial class Player
{
    public Player(int index)
    {
        Index = index;
        Messenger = new PlayerMessenger(this);
        Character = new ClientsideCharacter(this);
        Storage = new MemoryStorage();
    }

    /// <summary>
    /// Player client index.
    /// </summary>
    public virtual int Index { get; init; }

    /// <summary>
    /// Player remote client
    /// </summary>
    public virtual RemoteClient RemoteClient => Netplay.Clients[Index];

    /// <summary>
    /// Player terraria player
    /// </summary>
    public virtual TPlayer TPlayer => Main.player[Index];

    /// <summary>
    /// Player name.
    /// </summary>
    public virtual string? Name { get; internal set; }

    /// <summary>
    /// Player UUID.
    /// </summary>
    public virtual string? UUID { get; internal set; }
    
    /// <summary>
    /// Player state (None, AlmostJoined, Joined, Left)
    /// </summary>
    public virtual PlayerState PlayerState { get; internal set; }

    /// <summary>
    /// Player messenger. Use it in commands context.
    /// (if you will use SendMessage you are gay)
    /// </summary>
    public virtual PlayerMessenger Messenger { get; internal set; }

    /// <summary>
    /// Storage of custom data.
    /// </summary>
    public virtual MemoryStorage Storage { get; internal set; }

    /// <summary>
    /// Commands queue.
    /// </summary>
    public virtual BlockingCollection<string> CommandsQueue { get; } = new BlockingCollection<string>();

    internal CancellationTokenSource? cmdTokenSource;
    internal CancellationToken? cmdToken;
    internal Task? commandHandlerTask;
    
    internal void StartCommandHandler()
    {
        cmdTokenSource = new CancellationTokenSource();
        cmdToken = cmdTokenSource.Token;

        commandHandlerTask = new Task(CommandHandler);
        commandHandlerTask.Start();
    }

    internal void StopCommandHandler()
    {
        cmdTokenSource?.Cancel();
    }

    private void CommandHandler()
    {
        if (cmdToken == null)
            return;

        while (!cmdToken.Value.IsCancellationRequested)
        {
            try
            {
                string commandText = CommandsQueue.Take(cmdToken.Value);
                CommandResult result = MintServer.Commands.InvokeCommand(this, commandText);
                MintServer.Chat.DisplayResult(this, result);
            }
            catch (OperationCanceledException)
            {}
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Player: CommandHandler:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}