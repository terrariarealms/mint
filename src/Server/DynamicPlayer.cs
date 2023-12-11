using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Net.Sockets;

namespace Mint.Server;

public class DynamicPlayer : Player
{
    public DynamicPlayer(string name, Account account, PlayerMessenger messenger) : base(-1)
    {
        Name = name;

        Authorized = true;
        Account = account;

        Messenger = messenger;
    }
    public DynamicPlayer() : base(-1)
    {
    }

    public override ISocket Socket => new EmptySocket();

    public override string IP { get => "0.0.0.0"; set {} }

    public override TPlayer TPlayer => throw new InvalidOperationException("Cannot get TPlayer of DynamicPlayer.");

    public override Terraria.RemoteClient RemoteClient => throw new InvalidOperationException("Cannot get RemoteClient of DynamicPlayer.");

    public override void SetName(string name, bool netBroadcast)
    {}

    public override void Kick(NetworkText reason)
    {}

    public override void Kick(string reason)
    {}

    public override void Hurt(int damage, PlayerDeathReason? reason = null)
    {}

    public override void Kill(PlayerDeathReason? reason = null)
    {}

    public override void SendMessage(string text, Color color)
    {}

    public override void SendBytes(byte[] bytes)
    {}

    public override void SendPacket(int packetId, NetworkText? text, int num1 = 0, float num2 = 0f, float num3 = 0f, float num4 = 0f, int num5 = 0, int num6 = 0, int num7 = 0)
    {}
}