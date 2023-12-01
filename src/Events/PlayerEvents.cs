namespace Mint.Events;

public static class PlayerEvents
{
    public delegate void OnPlayerConnected(Player player);
    public delegate void OnPlayerLeft(Player player);

    public static event OnPlayerConnected? PlayerConnected;
    internal static void InvokePlayerConnected(Player player) => PlayerConnected?.Invoke(player);


    public static event OnPlayerLeft? PlayerLeft;
    internal static void InvokePlayerLeft(Player player) => PlayerLeft?.Invoke(player);
}