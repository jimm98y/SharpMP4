
using BoxGenerator2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class IsoStream
{
    private readonly Stream _stream;

    public IsoStream(Stream stream)
    {
        _stream = stream;
    }

    private byte ReadByte()
    {
        int read = _stream.ReadByte();
        if (read == -1) throw new EndOfStreamException();
        return (byte)(read & 0xff);
    }

    private byte WriteByte(byte value)
    {
        _stream.WriteByte(value);
        return 8;
    }

    internal ulong ReadBit(out bool value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBits(uint count, out byte value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBits(uint count, out ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBits(uint count, out short value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBits(uint count, out uint value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBox<T>(out T value) where T : Box
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBox<T>(out T[] value) where T : Box
    {
        throw new NotImplementedException();
    }

    internal ulong ReadClass<T>(out T value) where T : IMp4Serializable
    {
        throw new NotImplementedException();
    }

    internal ulong ReadClass<T>(out T[] value) where T : IMp4Serializable
    {
        throw new NotImplementedException();
    }

    internal ulong ReadClass<T>(ulong count, out T[] value) where T : IMp4Serializable
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBytes(ulong length, out byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadString(out string value)
    {
        throw new NotImplementedException();
    }
    
    internal ulong ReadStringZeroTerminated(out string value)
    {
        List<byte> buffer = new List<byte>();
        byte c;
        while ((c = ReadByte()) != 0)
        {
            buffer.Add(c);
        }
        value = Encoding.UTF8.GetString(buffer.ToArray());
        return (ulong)(buffer.Count + 1);
    }

    internal ulong ReadStringArray(uint count, out string[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadInt16(out short value)
    {
        ulong count = unchecked(ReadUInt16(out ushort v));
        value = unchecked((short)v);
        return count;
    }

    internal ulong ReadInt32(out int value)
    {
        ulong count = unchecked(ReadUInt32(out uint v));
        value = unchecked((int)v);
        return count;
    }

    internal ulong ReadInt32(out long value)
    {
        ulong count = unchecked(ReadUInt32(out uint v));
        value = unchecked((int)v);
        return count;
    }

    internal ulong ReadInt64(out long value)
    {
        ulong count = unchecked(ReadUInt64(out ulong v));
        value = unchecked((long)v);
        return count;
    }

    internal ulong ReadInt8(out sbyte value)
    {
        ulong count = unchecked(ReadUInt8(out byte v));
        value = unchecked((sbyte)v);
        return count;
    }

    internal ulong ReadUInt8(out byte value)
    {
        value = ReadByte();
        return 8;
    }

    internal ulong ReadUInt8(out ushort value)
    {
        value = ReadByte();
        return 8;
    }

    internal ulong ReadUInt16(out ushort value)
    {
        value = (ushort)(
            (ReadByte() << 8) +
            (ReadByte() << 0)
        );
        return 16;
    }

    internal ulong ReadUInt16(out uint value)
    {
        value = (uint)(
            (ReadByte() << 8) +
            (ReadByte() << 0)
        );
        return 16;
    }

    internal ulong ReadUInt24(out uint value)
    {
        value = (uint)(
            (ReadByte() << 16) + 
            (ReadByte() << 8) +
            (ReadByte() << 0)
        );
        return 24;
    }

    internal ulong ReadUInt32(out uint value)
    {
        value = (uint)(
            (ReadByte() << 24) +
            (ReadByte() << 16) +
            (ReadByte() << 8) +
            (ReadByte() << 0)
        );
        return 32;
    }

    internal ulong ReadUInt32(out ulong value)
    {
        value = (uint)(
            (ReadByte() << 24) +
            (ReadByte() << 16) +
            (ReadByte() << 8) +
            (ReadByte() << 0)
        );
        return 32;
    }

    internal ulong ReadUInt48(out ulong value)
    {
        value = (ulong)(
            (ReadByte() << 40) + 
            (ReadByte() << 32) + 
            (ReadByte() << 24) + 
            (ReadByte() << 16) + 
            (ReadByte() << 8) +
            (ReadByte() << 0)
        );
        return 48;
    }

    internal ulong ReadUInt64(out ulong value)
    {
        value = (ulong)(
            (ReadByte() << 56) + 
            (ReadByte() << 48) +
            (ReadByte() << 40) +
            (ReadByte() << 32) + 
            (ReadByte() << 24) + 
            (ReadByte() << 16) + 
            (ReadByte() << 8) + 
            (ReadByte() << 0)
        );
        return 64;
    }

    internal ulong ReadUInt16Array(int count, out ushort[] value)
    {
        ulong size = 0;
        value = new ushort[count];
        for (int i = 0; i < count; i++)
        {
            size += ReadUInt16(out value[i]);
        }
        return size;
    }

    internal ulong ReadUInt16Array(int count, out uint[] value)
    {
        ulong size = 0;
        value = new uint[count];
        for (int i = 0; i < count; i++)
        {
            size += ReadUInt16(out value[i]);
        }
        return size;
    }

    internal ulong ReadUInt32Array(uint count, out uint[] value)
    {
        ulong size = 0;
        value = new uint[count];
        for (int i = 0; i < count; i++)
        {
            size += ReadUInt32(out value[i]);
        }
        return size;
    }

    internal ulong ReadUInt32Array(uint count, out ulong[] value)
    {
        ulong size = 0;
        value = new ulong[count];
        for (int i = 0; i < count; i++)
        {
            size += ReadUInt32(out value[i]);
        }
        return size;
    }

    internal ulong ReadUInt64Array(uint count, out ulong[] value)
    {
        ulong size = 0;
        value = new ulong[count];
        for (int i = 0; i < count; i++)
        {
            size += ReadUInt64(out value[i]);
        }
        return size;
    }

    internal ulong ReadUInt8Array(uint count, out byte[] value)
    {
        ulong size = 0;
        value = new byte[count];
        for (int i = 0; i < count; i++)
        {
            size += ReadUInt8(out value[i]);
        }
        return size;
    }

    internal ulong ReadUInt8Array(out byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt32Array(out uint[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadDouble32(out double value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteDouble32(double value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBit(bool value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBits(uint count, byte value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBits(uint count, short value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBits(uint count, ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBits(uint count, uint value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBox(Box value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBox(Box[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBytes(ulong count, byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteClass(IMp4Serializable value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteClass(IMp4Serializable[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteClass(uint count, IMp4Serializable[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteString(string value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteStringZeroTerminated(string value)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(value);
        for (int i = 0; i < buffer.Length; i++)
        {
            WriteByte(buffer[i]);
        }
        WriteByte(0);
        return (ulong)buffer.Length + 1;
    }

    internal ulong WriteStringArray(uint count, string[] values)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteInt16(short value)
    {
        return WriteUInt16(unchecked((ushort)value));
    }

    internal ulong WriteInt32(int value)
    {
        return WriteUInt32(unchecked((uint)value));
    }

    internal ulong WriteInt32(long value)
    {
        return WriteUInt32(unchecked((uint)value));
    }

    internal ulong WriteInt64(long value)
    {
        return WriteUInt64(unchecked((ulong)value));
    }

    internal ulong WriteInt8(sbyte value)
    {
        return WriteUInt8(unchecked((byte)value));
    }

    internal ulong WriteUInt8(byte value)
    {
        _stream.WriteByte(value);
        return 1;
    }

    internal ulong WriteUInt8(ushort value)
    {
        _stream.WriteByte((byte)value);
        return 1;
    }

    internal ulong WriteUInt16(ushort value)
    {
        WriteByte((byte)(value >> 8 & 0xFF));
        WriteByte((byte)(value & 0xFF));
        return 16;
    }

    internal ulong WriteUInt16(uint value)
    {
        WriteByte((byte)(value >> 8 & 0xFF));
        WriteByte((byte)(value & 0xFF));
        return 16; throw new NotImplementedException();
    }

    internal ulong WriteUInt24(uint value)
    {
        value = value & 0xFFFFFF;
        WriteByte((byte)(value >> 16 & 0xFF));
        WriteByte((byte)(value >> 8 & 0xFF));
        WriteByte((byte)(value & 0xFF));
        return 24;
    }

    internal ulong WriteUInt32(uint value)
    {
        WriteByte((byte)(value >> 24 & 0xFF));
        WriteByte((byte)(value >> 16 & 0xFF));
        WriteByte((byte)(value >> 8 & 0xFF));
        WriteByte((byte)(value & 0xFF));
        return 32;
    }

    internal ulong WriteUInt32(ulong value)
    {
        WriteByte((byte)(value >> 24 & 0xFF));
        WriteByte((byte)(value >> 16 & 0xFF));
        WriteByte((byte)(value >> 8 & 0xFF));
        WriteByte((byte)(value & 0xFF));
        return 32;
    }

    internal ulong WriteUInt48(ulong value)
    {
        WriteByte((byte)(value >> 40 & 0xFF));
        WriteByte((byte)(value >> 32 & 0xFF));
        WriteByte((byte)(value >> 24 & 0xFF));
        WriteByte((byte)(value >> 16 & 0xFF));
        WriteByte((byte)(value >> 8 & 0xFF));
        WriteByte((byte)(value & 0xFF));
        return 48;
    }

    internal ulong WriteUInt64(ulong value)
    {
        WriteByte((byte)(value >> 56 & 0xFF));
        WriteByte((byte)(value >> 48 & 0xFF));
        WriteByte((byte)(value >> 40 & 0xFF));
        WriteByte((byte)(value >> 32 & 0xFF));
        WriteByte((byte)(value >> 24 & 0xFF));
        WriteByte((byte)(value >> 16 & 0xFF));
        WriteByte((byte)(value >> 8 & 0xFF));
        WriteByte((byte)(value & 0xFF));
        return 64;
    }

    internal ulong WriteUInt16Array(uint count, ushort[] value)
    {
        ulong size = 0;
        for (int i = 0; i < count; i++)
        {
            size += WriteUInt16(value[i]);
        }
        return size;
    }

    internal ulong WriteUInt16Array(uint count, uint[] value)
    {
        ulong size = 0;
        for (int i = 0; i < count; i++)
        {
            size += WriteUInt16(value[i]);
        }
        return size;
    }

    internal ulong WriteUInt8Array(uint count, byte[] value)
    {
        ulong size = 0;
        for (int i = 0; i < count; i++)
        {
            size += WriteUInt8(value[i]);
        }
        return size;
    }

    internal ulong WriteUInt32Array(uint count, uint[] value)
    {
        ulong size = 0;
        for (int i = 0; i < count; i++)
        {
            size += WriteUInt32(value[i]);
        }
        return size;
    }

    internal ulong WriteUInt32Array(uint count, ulong[] value)
    {
        ulong size = 0;
        for (int i = 0; i < count; i++)
        {
            size += WriteUInt32(value[i]);
        }
        return size;
    }

    internal ulong WriteUInt64Array(uint count, ulong[] value)
    {
        ulong size = 0;
        for (int i = 0; i < count; i++)
        {
            size += WriteUInt64(value[i]);
        }
        return size;
    }

    internal ulong WriteUInt8Array(byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt32Array(uint[] value)
    {
        throw new NotImplementedException();
    }

    internal static uint FromFourCC(string input)
    {
        throw new NotImplementedException();
    }

    internal static string ToFourCC(uint input)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateBoxSize(IEnumerable<Box> boxes)
    {
        ulong size = 0;
        foreach (Box box in boxes)
        {
            size += box.CalculateSize();
        }
        return size;
    }

    internal static ulong CalculateSize(string[] values)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateClassSize(IMp4Serializable value)
    {
        return value.CalculateSize();
    }

    internal static ulong CalculateClassSize(IMp4Serializable[] value)
    {
        ulong size = 0;
        foreach (IMp4Serializable c in value)
        {
            size += c.CalculateSize();
        }
        return size;
    }

    internal ulong ReadBoxChildren(ulong readSize, Box box)
    {
        ulong boxSize = 0;
        box.Children = new List<Box>();
        while ((boxSize + readSize) < box.Size)
        {
            boxSize += ReadBox(out Box b);
            box.Children.Add(b);
        }
        return boxSize;
    }

    internal static ulong CalculateBoxChildren(Box value)
    {
        return CalculateBoxSize(value.Children);
    }

    internal ulong WriteBoxChildren(Box box)
    {
        ulong boxSize = 0;
        if (box.Children != null)
        {
            foreach (var b in box.Children)
            {
                boxSize += WriteBox(b);
            }
        }
        return boxSize;
    }

    internal ulong ReadBslbf(ulong count, out byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBslbf(ulong count, byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBslbf(ulong count, out ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBslbf(ulong count, ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBslbf(out bool value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBslbf(bool value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadBslbf(ulong count, out byte value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteBslbf(ulong count, byte value)
    {
        throw new NotImplementedException();
    }
    
    internal ulong ReadUimsbf(ulong count, out byte value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUimsbf(ulong count, byte value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUimsbf(out bool value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUimsbf(bool value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUimsbf(ulong count, out ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUimsbf(ulong count, ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUimsbf(ulong count, out uint value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUimsbf(ulong count, uint value)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateByteAlignmentSize(byte byte_alignment)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadByteAlignment(out byte value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteByteAlignment(byte value)
    {
        throw new NotImplementedException();
    }

    internal static int BitsToDecode()
    {
        throw new NotImplementedException();
    }

    internal ulong WriteIso639(string value)
    {
        if (Encoding.UTF8.GetBytes(value).Length != 3)
        {
            throw new ArgumentException($"\"{value}\" value string must be 3 characters long!");
        }
        int bits = 0;
        for (int i = 0; i < 3; i++)
        {
            bits += Encoding.UTF8.GetBytes(value)[i] - 0x60 << (2 - i) * 5;
        }
        return WriteUInt16((ushort)bits);
    }

    internal ulong ReadIso639(out string value)
    {
        ushort bits;
        ReadUInt16(out bits);
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < 3; i++)
        {
            int c = bits >> (2 - i) * 5 & 0x1f;
            result.Append((char)(c + 0x60));
        }
        value = result.ToString();
        return 16;
    }
}