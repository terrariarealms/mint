
namespace Mint.API;

public static class PlayersAPI
{
    /// <summary>
    /// Get player by index.
    /// </summary>
    /// <param name="index">Player index</param>
    /// <returns>Player</returns>
    public static Player? GetPlayer(byte index)
    {        
        return MintServer.Players?.players[index] ?? null;
    }

    /// <summary>
    /// Get player by index.
    /// </summary>
    /// <param name="index">Player index</param>
    /// <returns>Player</returns>
    public static Player? GetPlayer(int index)
    {        
        if (index < 0 || index > 255)
            return null;

        return GetPlayer((byte)index);
    }
}