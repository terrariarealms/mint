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
    /// Get reader for this packet.
    /// </summary>
    /// <returns>BinaryReader</returns>
    public BinaryReader GetReader()
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        _reader.BaseStream.Position = Start;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        return _reader;
    }

    /// <summary>
    /// Create reader for this packet.
    /// </summary>
    /// <returns>BinaryReader</returns>
    internal BinaryReader CreateReader()
    {
        _stream = new MemoryStream(Net.buffer[Sender].readBuffer);
        _reader = new BinaryReader(_stream);
        _reader.BaseStream.Position = Start;

        return _reader;
    }

    /// <summary>
    /// Dispose reader that you got from CreateReader(). (above)
    /// </summary>
    /// <param name="reader">BinaryReader</param>
    internal void DisposeReader()
    {
        _reader?.Dispose();
        _reader?.BaseStream.Dispose();
    }

    /// <summary>
    /// Get sender's Mint.Server.Player instance.
    /// </summary>
    /// <returns></returns>
    public Player GetSender() => MintServer.Players.players[Sender];

    private MemoryStream? _stream;
    private BinaryReader? _reader;

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