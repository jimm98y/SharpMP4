
using BoxGenerator2;
using System;
using System.IO;

internal class IsoReaderWriter
{
    internal static ulong ReadBit(Stream stream, out bool value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadBits(Stream stream, int count, out byte value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadBits(Stream stream, int count, out ushort value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadBits(Stream stream, int count, out uint value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadBitsArray(Stream stream, int bitCount, int count, out byte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadBox<T>(Stream stream, out T value) where T : Box
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadBox<T>(Stream stream, out T[] value) where T : Box
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadBytes(Stream stream, ulong length, out byte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadClass<T>(Stream stream, out T value) where T : class
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadClass<T>(Stream stream, out T[] value) where T : class
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadClass<T>(Stream stream, int count, out T[] value) where T : class
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadInt16(Stream stream, out short value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadInt32(Stream stream, out int value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadInt64(Stream stream, out long value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadInt8(Stream stream, out sbyte value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadString(Stream stream, out string value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt16(Stream stream, out ushort value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt16Array(Stream stream, int count, out ushort[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt24(Stream stream, out uint value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt32(Stream stream, out uint value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt32Array(Stream stream, uint count, out uint[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt48(Stream stream, out ulong value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt64(Stream stream, out ulong value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt8(Stream stream, out byte value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadInt8Array(Stream stream, out sbyte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadStringArray(Stream stream, uint count, out string[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt64Array(Stream stream, uint count, out ulong[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt8Array(Stream stream, out byte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadInt32Array(Stream stream, out int[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt32Array(Stream stream, out uint[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadInt8Array(Stream stream, uint count, out sbyte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBit(Stream stream, bool value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBits(Stream stream, int count, long value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBitsArray(Stream stream, int bitCount, int count, byte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBox(Stream stream, Box value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBox(Stream stream, Box[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBytes(Stream stream, ulong count, byte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteClass(Stream stream, object value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteClass(Stream stream, object[] values)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt16(Stream stream, short value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt32(Stream stream, int value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt64(Stream stream, long value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt8(Stream stream, sbyte value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteString(Stream stream, string lang)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt16(Stream stream, ushort value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt16Array(Stream stream, uint count, ushort[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt24(Stream stream, uint reserved)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt32(Stream stream, uint value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt32Array(Stream stream, uint count, uint[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt48(Stream stream, ulong value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt64(Stream stream, ulong value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt8(Stream stream, byte value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt8Array(Stream stream, sbyte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt8Array(Stream stream, uint count, sbyte[] value)
    {
        throw new NotImplementedException();
    }


    internal static ulong WriteStringArray(Stream stream, uint count, string[] values)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt8Array(Stream stream, byte[] value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt64Array(Stream stream, uint count, ulong[] ulongs)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteClass(Stream stream, uint count, object[] values)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt32Array(Stream stream, uint[] values)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt32Array(Stream stream, int[] values)
    {
        throw new NotImplementedException();
    }

    internal static uint FromFourCC(string input)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateSize(Box[] boxes)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateSize(Box box)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateSize(string[] values)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateClassSize(object value)
    {
        throw new NotImplementedException();
    }

    internal static ulong CalculateClassSize(object[] value)
    {
        throw new NotImplementedException();
    }
}