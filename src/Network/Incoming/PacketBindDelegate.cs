namespace Mint.Network.Incoming;

public delegate void PacketBindDelegate(Player sender, Packet packet, ref bool ignore);