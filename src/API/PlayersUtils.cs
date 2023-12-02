namespace Mint.API;

public static class PlayersUtils
{
    /// <summary>
    /// PlayersManager reference.
    /// </summary>
    public static PlayersManager Players => MintServer.Players;

    /// <summary>
    /// All active players that enabled PvP Mode.
    /// </summary>
    public static IEnumerable<Player> InPvP => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined && p.TPlayer.hostile);

    /// <summary>
    /// All active players that not enabled PvP Mode.
    /// </summary>
    public static IEnumerable<Player> NotInPvP => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined && !p.TPlayer.hostile);

    /// <summary>
    /// All active players that alive.
    /// </summary>
    public static IEnumerable<Player> Alive => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined && !p.TPlayer.dead);

    /// <summary>
    /// All active players that alive.
    /// </summary>
    public static IEnumerable<Player> Dead => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined && p.TPlayer.dead);

    /// <summary>
    /// All active players.
    /// </summary>
    public static IEnumerable<Player> Active => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined);

    /// <summary>
    /// Get count of active players.
    /// </summary>
    /// <returns>Active players count.</returns>
    public static int GetActivePlayersCount()
    {
        if (MintServer.Players?.players == null) 
            return -1;

        byte count = 0;

        for (int i = 0; i < 255; i++)
        {
            Player? player = MintServer.Players?.players[i];
            if (player?.PlayerState == PlayerState.Joined) 
                count++;
        }

        return count;
    }

    /// <summary>
    /// Faster implementation of Where from LINQ.
    /// </summary>
    /// <param name="predicate">Target predicate</param>
    /// <returns>Predicate result</returns>
    public static IEnumerable<Player> QuickWhere(Predicate<Player> predicate)
    {
        if (MintServer.Players?.players == null) 
            yield break;

        for (int i = 0; i < 255; i++)
        {
            Player? player = MintServer.Players.players[i];
            if (player != null && predicate(player))
                yield return player;
        }
    }

    /// <summary>
    /// Implementation of foreach.
    /// </summary>
    /// <param name="action">Target action</param>
    public static void QuickForEach(Action<Player> action)
    {
        if (MintServer.Players?.players == null) 
            return;

        for (int i = 0; i < 255; i++)
        {
            Player? player = MintServer.Players?.players[i];
            if (player != null)
                action(player);
        }
    }
}