
using Terraria;
using Terraria.Localization;
using Terraria.Net.Sockets;

namespace Mint.Server;

public class PlayersManager
{
    internal Player[] players = new Player[254];

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

        AsyncMessage.StartSession(index);

        Console.WriteLine($"Players: Created player instance for {index}.");

        PlayerEvents.InvokePlayerConnected(players[index]);
    }

    private void HandleDisconnect(int index)
    {
        AsyncMessage.StopSession(index);

        if (players[index] == null) 
            return;

        players[index].PlayerState = PlayerState.Left;

        PlayerEvents.InvokePlayerConnected(players[index]);

        players[index].Socket.Close();
    }
}