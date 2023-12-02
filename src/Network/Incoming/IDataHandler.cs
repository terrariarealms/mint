namespace Mint.Network.Incoming;

public interface IDataHandler
{
    /// <summary>
    /// Register packet bind.
    /// </summary>
    /// <param name="packetId">Packet ID</param>
    /// <param name="bind">Bind delegate</param>
    public void Register(int packetId, PacketBindDelegate bind);

    /// <summary>
    /// Unregister packet bind.
    /// </summary>
    /// <param name="packetId">Packet ID</param>
    /// <param name="bind">Bind delegate</param>
    public void Unregister(int packetId, PacketBindDelegate bind);

    /// <summary>
    /// Invoke packet.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="packet">Target packet</param>
    /// <param name="ignore">Ignore packet</param>
    public void Invoke(Player sender, Packet packet, ref bool ignore);
}