namespace Mint.Network.Incoming;

public struct IncomingPacket
{
    public IncomingPacket(byte packetId, byte sender, int start, int length)
    {
        PacketID = packetId;
        Sender = sender;
        Start = start;
        Length = length;
    }

    /// <summary>
    /// Create reader for this packet.
    /// </summary>
    /// <returns>BinaryReader</returns>
    public BinaryReader CreateReader()
    {
        MemoryStream stream = new MemoryStream(Net.buffer[Sender].readBuffer);
        BinaryReader reader = new BinaryReader(stream);
        reader.BaseStream.Position = Start;

        return reader;
    }

    /// <summary>
    /// Dispose reader that you got from CreateReader(). (above)
    /// </summary>
    /// <param name="reader">BinaryReader</param>
    public void DisposeReader(BinaryReader reader)
    {
        reader.Dispose();
        reader.BaseStream.Dispose();
    }

    /// <summary>
    /// Get sender's Mint.Server.Player instance.
    /// </summary>
    /// <returns></returns>
    public Player GetSender() => MintServer.Players.players[Sender];

    /// <summary>
    /// Packet ID.
    /// </summary>
    public byte PacketID;

    /// <summary>
    /// Packet sender index.
    /// </summary>
    public byte Sender;

    /// <summary>
    /// Packet start.
    /// </summary>
    public int Start;

    /// <summary>
    /// Packet length.
    /// </summary>
    public int Length;
}