using Microsoft.Xna.Framework;

namespace Mint.Outcoming;

public unsafe class PacketWriter : BinaryWriter
{
    /// <summary>
    /// Create new PacketWriter instance. This way is faster.
    /// </summary>
    /// <param name="size">Packet size</param>
    /// <returns>PacketWriter instance</returns>
    public static PacketWriter New(byte packetId, int size)
    {
        byte[] data = new byte[size];
        MemoryStream stream = new MemoryStream(data);

        return new PacketWriter(data, stream).WriteByte(packetId);
    }
    /// <summary>
    /// Create new PacketWriter instance.
    /// </summary>
    /// <returns>PacketWriter instance</returns>
    public static PacketWriter New(byte packetId)
    {
        MemoryStream stream = new MemoryStream();
        return new PacketWriter(stream).WriteByte(packetId);
    }

    internal byte[]? data;
    internal MemoryStream stream;

    public PacketWriter(byte[] data, MemoryStream stream) : base(stream)
    {
        this.data = data;
        this.stream = stream;
    } 

    public PacketWriter(MemoryStream stream) : base(stream)
    {
        this.stream = stream;
    } 

    /// <summary>
    /// Write Byte.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteByte(byte value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write SByte.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteSByte(sbyte value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write Int16. (short)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteInt16(short value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write UInt16. (ushort)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteUInt16(ushort value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write Int32. (int)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteInt32(int value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write UInt32. (uint)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteUInt32(uint value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write Int64. (long)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteInt64(long value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write UInt64. (ulong)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteUInt64(ulong value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write Single. (float)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteSingle(float value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write Decimal.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteDecimal(decimal value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write Double.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteDouble(double value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write String.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteString(string value)
    {
        base.Write(value);
        return this;
    }

    /// <summary>
    /// Write Vector2.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteVector2(Vector2 value)
    {
        base.Write(value.X);
        base.Write(value.Y);
        return this;
    }

    /// <summary>
    /// Write Point.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WritePoint(Point value)
    {
        base.Write(value.X);
        base.Write(value.Y);
        return this;
    }

    /// <summary>
    /// Write Rectangle.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>PacketWriter instance</returns>
    public PacketWriter WriteRectangle(Rectangle value)
    {
        base.Write(value.X);
        base.Write(value.Y);
        base.Write(value.Width);
        base.Write(value.Height);
        return this;
    }

    /// <summary>
    /// Build packet.
    /// </summary>
    /// <returns>Packet bytes array</returns>
    public byte[] Build()
    {
        ushort length = (ushort)stream.Position;
        stream.Position = 0;
        WriteUInt16(length);

        stream.Position = length;

        return data ?? stream.ToArray();
    }
}
