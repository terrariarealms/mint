namespace Mint.Network;

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

    public byte PacketID;
    public byte Sender;

    public int Start;
    public int Length;
}