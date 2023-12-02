namespace Mint.Network.Incoming;

public struct Packet
{
    public Packet(byte packetId, byte sender, int start, int length)
    {
        PacketID = packetId;
        Sender = sender;
        Start = start;
        Length = length;
    }

    public BinaryReader CreateReader()
    {
        MemoryStream stream = new MemoryStream(Net.buffer[Sender].readBuffer);
        BinaryReader reader = new BinaryReader(stream);
        reader.BaseStream.Position = Start;

        return reader;
    }

    public void DisposeReader(BinaryReader reader)
    {
        reader.BaseStream.Dispose();
        reader.Dispose();
    }

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