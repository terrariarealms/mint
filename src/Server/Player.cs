using Terraria;

namespace Mint.Server;

public partial class Player
{
    public Player(int index)
    {
        Index = index;
    }

    /// <summary>
    /// Player's client index.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// Player's remote client
    /// </summary>
    public RemoteClient RemoteClient => Netplay.Clients[Index];

    /// <summary>
    /// Player's terraria player
    /// </summary>
    public TPlayer TPlayer => Main.player[Index];

    /// <summary>
    /// Player's name.
    /// </summary>
    public string Name => Main.player[Index].name;
}