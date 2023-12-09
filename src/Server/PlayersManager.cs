
using Terraria;
using Terraria.Localization;
using Terraria.Net.Sockets;

namespace Mint.Server;

public sealed class PlayersManager
{
    /// <summary>
    /// All active players that enabled PvP Mode.
    /// </summary>
    public IEnumerable<Player> InPvP => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined && p.TPlayer.hostile);

    /// <summary>
    /// All active players that not enabled PvP Mode.
    /// </summary>
    public IEnumerable<Player> NotInPvP => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined && !p.TPlayer.hostile);

    /// <summary>
    /// All active players that alive.
    /// </summary>
    public IEnumerable<Player> Alive => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined && !p.TPlayer.dead);

    /// <summary>
    /// All active players that alive.
    /// </summary>
    public IEnumerable<Player> Dead => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined && p.TPlayer.dead);

    /// <summary>
    /// All active players.
    /// </summary>
    public IEnumerable<Player> Active => 
        QuickWhere((p) => p.PlayerState == PlayerState.Joined);

    /// <summary>
    /// Get count of active players.
    /// </summary>
    /// <returns>Active players count.</returns>
    public int GetActivePlayersCount()
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
    public IEnumerable<Player> QuickWhere(Predicate<Player> predicate)
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
    public void QuickForEach(Action<Player> action)
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

    internal Player[] players = new Player[255];

    internal void Initialize()
    {
        On.Terraria.Netplay.OnConnectionAccepted += OnConnectionAccepted;
        On.Terraria.NetMessage.SyncDisconnectedPlayer += SyncDisconnectedPlayer;
    }

    private void OnConnectionAccepted(On.Terraria.Netplay.orig_OnConnectionAccepted orig, ISocket client)
    {
		int num = Netplay.FindNextOpenClientSlot();
		if (num != -1)
		{
			Netplay.Clients[num].Reset();
			Netplay.Clients[num].Socket = client;

            HandleConnection(num);
		}
		else
		{
			lock (Netplay.fullBuffer)
			{
				Netplay.KickClient(client, NetworkText.FromKey("CLI.ServerIsFull"));
			}
		}
		if (Netplay.FindNextOpenClientSlot() == -1)
		{
			Netplay.StopListening();
			Netplay.IsListening = false;
		}
    }

    private void SyncDisconnectedPlayer(On.Terraria.NetMessage.orig_SyncDisconnectedPlayer orig, int plr)
    {
        HandleDisconnect(plr);
		Net.SyncOnePlayer(plr, -1, plr);
		Net.EnsureLocalPlayerIsPresent();
    }


    public Player this[int index]
    {
        get => players[index];
        set => players[index] = value;
    }

    private void HandleConnection(int index)
    {
        players[index] = new Player(index)
        {
            PlayerState = PlayerState.Joined
        };
        players[index].StartPacketHandler();

        Console.WriteLine($"Players: Created player instance for {index}.");

        Events.InvokePlayerConnected(players[index]);
    }

    private void HandleDisconnect(int index)
    {
        if (players[index] == null) 
            return;

        players[index].StopCommandHandler();
        players[index].StopPacketHandler();
        players[index].PlayerState = PlayerState.Left;

        Events.InvokePlayerLeft(players[index]);

        players[index].Socket.Close();
    }
    public static class Events
    {
        public delegate void OnPlayerConnected(Player player);
        public delegate void OnPlayerLeft(Player player);

        /// <summary>
        /// Invokes when player was connected (socket)
        /// </summary>
        public static event OnPlayerConnected? PlayerConnected;
        internal static void InvokePlayerConnected(Player player) 
            => PlayerConnected?.Invoke(player);

        /// <summary>
        /// Invokes when player was left (socket)
        /// </summary>
        public static event OnPlayerLeft? PlayerLeft;
        internal static void InvokePlayerLeft(Player player) 
            => PlayerLeft?.Invoke(player);
    }
}