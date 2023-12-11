namespace Mint.Network;

public delegate void NetworkBindDelegate<TPacket>(Player player, TPacket packet, ref bool ignore) where TPacket : struct;