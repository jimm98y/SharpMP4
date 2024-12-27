
using BoxGenerator2;
using System;
using System.Collections.Generic;
using System.IO;

public class IsoStream
{
    private readonly Stream _stream;

    public IsoStream(Stream stream)
    {
        _stream = stream;
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

    internal ulong ReadBox<T>(ulong count, out T[] value) where T : Box
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

    internal ulong ReadInt16(out short value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadInt32(out int value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadInt32(out long value)
    {
        throw new NotImplementedException();
    }

    internal  ulong ReadInt64(out long value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadInt8(out sbyte value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadString(out string value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt16(out ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt16(out uint value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt16Array(int count, out ushort[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt16Array(int count, out uint[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt24(out uint value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt32(out uint value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt32(out ulong value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt32Array(uint count, out uint[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt32Array(uint count, out ulong[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt48(out ulong value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt64(out ulong value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt8(out byte value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt8(out ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadStringArray(uint count, out string[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt64Array(uint count, out ulong[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt8Array(out byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadUInt8Array(ulong count, out byte[] value)
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

    internal ulong WriteBox(ulong count, Box[] values)
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

    internal ulong WriteClass(IMp4Serializable[] values)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteClass(uint count, IMp4Serializable[] values)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteInt16(short value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteInt32(int value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteInt32(long value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteInt64(long value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteInt8(sbyte value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteString(string lang)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt16(ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt16(uint value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt16Array(uint count, ushort[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt16Array(uint count, uint[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt24(uint reserved)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt32(uint value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt32(ulong value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt48(ulong value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt64(ulong value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt8(byte value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt8(ushort value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteStringArray(uint count, string[] values)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt8Array(byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt8Array(ulong count, byte[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt32Array(uint count, uint[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt32Array(uint count, ulong[] value)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt64Array(uint count, ulong[] ulongs)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteUInt32Array(uint[] values)
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
        throw new NotImplementedException();
    }

    internal static ulong CalculateBoxSize(Box box)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateSize(string[] values)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateClassSize(IMp4Serializable value)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateClassSize(IMp4Serializable[] value)
    {
        throw new NotImplementedException();
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

    internal ulong ReadByteAlignment(out byte byte_alignment)
    {
        throw new NotImplementedException();
    }

    internal ulong WriteByteAlignment(byte byte_alignment)
    {
        throw new NotImplementedException();
    }

    internal static int BitsToDecode()
    {
        throw new NotImplementedException();
    }

    internal ulong WriteIso639(string language)
    {
        throw new NotImplementedException();
    }

    internal ulong ReadIso639(out string language)
    {
        throw new NotImplementedException();
    }
}