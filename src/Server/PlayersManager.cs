namespace Mint.Server;

internal class PlayersManager
{
    internal Player?[] players = new Player?[256];

    internal void NewPlayer(int index)
    {
        players[index] = new Player(index);
    }
}