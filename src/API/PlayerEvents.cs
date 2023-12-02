namespace Mint.API;

public static class PlayerEvents
{
    public delegate void OnPlayerConnected(Player player);
    public delegate void OnPlayerLeft(Player player);

    /// <summary>
    /// Invokes when player was connected (socket)
    /// </summary>
    public static event OnPlayerConnected? PlayerConnected;
    internal static void InvokePlayerConnected(Player player) => PlayerConnected?.Invoke(player);

    /// <summary>
    /// Invokes when player was left (socket)
    /// </summary>
    public static event OnPlayerLeft? PlayerLeft;
    internal static void InvokePlayerLeft(Player player) => PlayerLeft?.Invoke(player);
}