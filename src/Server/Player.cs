using Terraria;

namespace Mint.Server;

public partial class Player
{
    public Player(int index)
    {
        Index = index;
        Messenger = new PlayerMessenger(this);
    }

    /// <summary>
    /// Player's client index.
    /// </summary>
    public virtual int Index { get; init; }

    /// <summary>
    /// Player's remote client
    /// </summary>
    public virtual RemoteClient RemoteClient => Netplay.Clients[Index];

    /// <summary>
    /// Player's terraria player
    /// </summary>
    public virtual TPlayer TPlayer => Main.player[Index];

    /// <summary>
    /// Player's name.
    /// </summary>
    public virtual string? Name { get; internal set; }

    /// <summary>
    /// Player's UUID.
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
}