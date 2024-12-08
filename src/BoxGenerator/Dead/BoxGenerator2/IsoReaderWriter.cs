
using BoxGenerator2;

internal class IsoReaderWriter
{
    internal static byte ReadBit(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static byte ReadBits(Stream stream, int count)
    {
        throw new NotImplementedException();
    }

    internal static byte[] ReadBitsArray(Stream stream, int v1, int v2)
    {
        throw new NotImplementedException();
    }

    internal static Box ReadBox(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static Box[] ReadBoxes(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static byte[] ReadBytes(Stream stream, ulong length)
    {
        throw new NotImplementedException();
    }

    internal static object ReadClass(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static short ReadInt16(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static int ReadInt32(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static CompositionToDecodeBox.compositionStartTime ReadInt64(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static byte ReadInt8(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static string ReadString(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static ushort ReadUInt16(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static uint ReadUInt24(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static uint ReadUInt32(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static TrackHeaderBox.reserved ReadUInt32Array(Stream stream, int v)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt48(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static ulong ReadUInt64(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static byte ReadUInt8(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBit(Stream stream, byte value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBits(Stream stream, int count, long value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBitsArray(Stream stream, int v1, int v2, byte[] language)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBox(Stream stream, Box box)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBoxes(Stream stream, Box[] any_box)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteBytes(Stream stream, ulong length, byte[] stereo_indication_type)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteClass(Stream stream, object o)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt16(Stream stream, short time_offset)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt32(Stream stream, int offset)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt64(Stream stream, CompositionToDecodeBox.compositionEndTime compositionEndTime)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteInt8(Stream stream, byte value)
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

    internal static ulong WriteUInt24(Stream stream, uint reserved)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt32(Stream stream, uint value)
    {
        throw new NotImplementedException();
    }

    internal static ulong WriteUInt32Array(Stream stream, int v, uint[] matrix)
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
}