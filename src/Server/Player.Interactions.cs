using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Mint.Server;

public partial class Player
{    
    /// <summary>
    /// Update player's name
    /// </summary>
    /// <param name="name">New name</param>
    /// <param name="netBroadcast">Broadcast PlayerInfo (4) packet with updated name</param>
    public void SetName(string name, bool netBroadcast)
    {
        Main.player[Index].name = name;
        Netplay.Clients[Index].Name = name;

        if (netBroadcast) Net.SendData(4, -1, -1, NetworkText.Empty, Index);
    }

    /// <summary>
    /// Kick player.
    /// </summary>
    /// <param name="reason">Kick reason</param>
    public void Kick(NetworkText reason) => Net.BootPlayer(Index, reason);

    /// <summary>
    /// Kick player.
    /// </summary>
    /// <param name="reason">Kick reason</param>
    public void Kick(string reason) => Kick(NetworkText.FromLiteral(reason));

    /// <summary>
    /// Hurt player.
    /// </summary>
    /// <param name="damage">Damage</param>
    /// <param name="reason">Death reason</param>
    public void Hurt(int damage, PlayerDeathReason? reason = null)
    {
        NetMessage.SendPlayerHurt(Index, reason ?? PlayerDeathReason.LegacyDefault(), damage, -1, false, false, 0, -1, -1);
    }

    /// <summary>
    /// Kill player.
    /// </summary>
    /// <param name="reason">Death reason</param>
    public void Kill(PlayerDeathReason? reason = null)
    {
        NetMessage.SendPlayerDeath(Index, reason ?? PlayerDeathReason.LegacyDefault(), 32767, -1, false, -1, -1);
    }

    /// <summary>
    /// Send message to player.
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="color">Message color</param>
    public void SendMessage(string text, Color color)
    {
        ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(text), color, Index);
    }
}