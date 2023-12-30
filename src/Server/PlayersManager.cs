using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
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

    public Player this[int index]
    {
        get => players[index];
        set => players[index] = value;
    }

    /// <summary>
    /// Get count of active players.
    /// </summary>
    /// <returns>Active players count.</returns>
    public int GetActivePlayersCount()
    {
        if (MintServer.Players?.players == null)
            return -1;

        byte count = 0;

        for (var i = 0; i < 255; i++)
        {
            var player = MintServer.Players?.players[i];
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

        for (var i = 0; i < 255; i++)
        {
            var player = MintServer.Players.players[i];
            if (player != null && predicate(player))
                yield return player;
        }
    }

    /// <summary>
    /// Get first player that matches predicate
    /// </summary>
    /// <param name="predicate">Target predicate</param>
    /// <returns>Predicate result</returns>
    public Player? First(Predicate<Player> predicate)
    {
        if (MintServer.Players?.players == null)
            return null;

        for (var i = 0; i < 255; i++)
        {
            var player = MintServer.Players.players[i];
            if (player != null && predicate(player))
                return player;
        }

        return null;
    }

    /// <summary>
    /// Implementation of foreach.
    /// </summary>
    /// <param name="action">Target action</param>
    public void QuickForEach(Action<Player> action)
    {
        if (MintServer.Players?.players == null)
            return;

        for (var i = 0; i < 255; i++)
        {
            var player = MintServer.Players?.players[i];
            if (player != null)
                action(player);
        }
    }

    internal Player[] players = new Player[255];

    internal void Initialize()
    {
        On.Terraria.Netplay.OnConnectionAccepted += OnConnectionAccepted;
        ModSend.SyncDisconnectedPlayer += SyncDisconnectedPlayer;
        ModSend.greetPlayer += OnGreetPlayer;
        ModSend.SyncOnePlayer += SyncOnePlayer;
    }

    private void SyncOnePlayer(ModSend.orig_SyncOnePlayer orig, int plr, int toWho, int fromWho)
    {
        var num = 0;
        if (Main.player[plr].active) num = 1;
        if (Netplay.Clients[plr].State == 10)
        {
            Net.SendData(14, toWho, fromWho, NetworkText.Empty, plr, num);
            Net.SendData(4, toWho, fromWho, NetworkText.Empty, plr);
            Net.SendData(13, toWho, fromWho, NetworkText.Empty, plr);
            if (Main.player[plr].statLife <= 0) Net.SendData(135, toWho, fromWho, NetworkText.Empty, plr);
            Net.SendData(16, toWho, fromWho, NetworkText.Empty, plr);
            Net.SendData(30, toWho, fromWho, NetworkText.Empty, plr);
            Net.SendData(45, toWho, fromWho, NetworkText.Empty, plr);
            Net.SendData(42, toWho, fromWho, NetworkText.Empty, plr);
            Net.SendData(50, toWho, fromWho, NetworkText.Empty, plr);
            Net.SendData(80, toWho, fromWho, NetworkText.Empty, plr, Main.player[plr].chest);
            Net.SendData(142, toWho, fromWho, NetworkText.Empty, plr);
            Net.SendData(147, toWho, fromWho, NetworkText.Empty, plr, Main.player[plr].CurrentLoadoutIndex);
            for (var i = 0; i < 59; i++)
                Net.SendData(5, toWho, fromWho, NetworkText.Empty, plr, PlayerItemSlotID.Inventory0 + i,
                    (int)Main.player[plr].inventory[i].prefix);
            for (var j = 0; j < Main.player[plr].armor.Length; j++)
                Net.SendData(5, toWho, fromWho, NetworkText.Empty, plr, PlayerItemSlotID.Armor0 + j,
                    (int)Main.player[plr].armor[j].prefix);
            for (var k = 0; k < Main.player[plr].dye.Length; k++)
                Net.SendData(5, toWho, fromWho, NetworkText.Empty, plr, PlayerItemSlotID.Dye0 + k,
                    (int)Main.player[plr].dye[k].prefix);
            Net.SyncOnePlayer_ItemArray(plr, toWho, fromWho, Main.player[plr].miscEquips, PlayerItemSlotID.Misc0);
            Net.SyncOnePlayer_ItemArray(plr, toWho, fromWho, Main.player[plr].miscDyes, PlayerItemSlotID.MiscDye0);
            Net.SyncOnePlayer_ItemArray(plr, toWho, fromWho, Main.player[plr].Loadouts[0].Armor,
                PlayerItemSlotID.Loadout1_Armor_0);
            Net.SyncOnePlayer_ItemArray(plr, toWho, fromWho, Main.player[plr].Loadouts[0].Dye,
                PlayerItemSlotID.Loadout1_Dye_0);
            Net.SyncOnePlayer_ItemArray(plr, toWho, fromWho, Main.player[plr].Loadouts[1].Armor,
                PlayerItemSlotID.Loadout2_Armor_0);
            Net.SyncOnePlayer_ItemArray(plr, toWho, fromWho, Main.player[plr].Loadouts[1].Dye,
                PlayerItemSlotID.Loadout2_Dye_0);
            Net.SyncOnePlayer_ItemArray(plr, toWho, fromWho, Main.player[plr].Loadouts[2].Armor,
                PlayerItemSlotID.Loadout3_Armor_0);
            Net.SyncOnePlayer_ItemArray(plr, toWho, fromWho, Main.player[plr].Loadouts[2].Dye,
                PlayerItemSlotID.Loadout3_Dye_0);
            if (!Netplay.Clients[plr].IsAnnouncementCompleted) Netplay.Clients[plr].IsAnnouncementCompleted = true;
            return;
        }

        num = 0;
        Net.SendData(14, -1, plr, NetworkText.Empty, plr, num);
        if (Netplay.Clients[plr].IsAnnouncementCompleted)
        {
            Netplay.Clients[plr].IsAnnouncementCompleted = false;
            Netplay.Clients[plr].Name = "Anonymous";
        }

        TPlayer.Hooks.PlayerDisconnect(plr);
    }

    public static event PlayerGreetEvent? OnPlayerGreet;


    private void OnGreetPlayer(ModSend.orig_greetPlayer orig, int plr)
    {
        players[plr].PlayerState = PlayerState.Joined;
        MintServer.Chat.SystemBroadcast(
            $"{players[plr].Name} {MintServer.Localization.Translate("has joined")}. [{GetActivePlayersCount()}/{Main.maxNetPlayers}]");

        foreach (var line in MintServer.Config.Game.MOTD)
            players[plr].SendMessage(line, Color.White);

        OnPlayerGreet?.Invoke(players[plr]);
        Log.Information("Players: greeting player on {Index} ({Name})", plr, players[plr].Name ?? "unknown");
    }

    private void OnConnectionAccepted(On.Terraria.Netplay.orig_OnConnectionAccepted orig, ISocket client)
    {
        var num = Netplay.FindNextOpenClientSlot();
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

    /// <summary>
    /// Invokes when player was connected (socket)
    /// </summary>
    public static event PlayerConnectedEvent? OnPlayerConnected;

    private void HandleConnection(int index)
    {
        players[index] = new Player(index)
        {
            PlayerState = PlayerState.AlmostJoined
        };
        players[index].StartPacketHandler();
        players[index].StartCommandHandler();
        players[index].Character = new ClientsideCharacter(players[index]);

        Log.Information("Players: created instance for player on {Index}", index);

        OnPlayerConnected?.Invoke(players[index]);
    }

    /// <summary>
    /// Invokes when player was left (socket)
    /// </summary>
    public static event PlayerLeftEvent? OnPlayerLeft;

    private void HandleDisconnect(int index)
    {
        if (players[index] == null)
            return;

        if (players[index].PlayerState == PlayerState.Joined && players[index].Name != null)
            MintServer.Chat.SystemBroadcast(
                $"{players[index].Name} {MintServer.Localization.Translate("has left")}. [{GetActivePlayersCount() - 1}/{Main.maxNetPlayers}]");

        players[index].StopCommandHandler();
        players[index].StopPacketHandler();
        players[index].Socket.Close();
        players[index].PlayerState = PlayerState.Left;

        Log.Information("Players: disconnected player on {Index} ({Name})", index, players[index].Name ?? "unknown");

        OnPlayerLeft?.Invoke(players[index]);
    }
}