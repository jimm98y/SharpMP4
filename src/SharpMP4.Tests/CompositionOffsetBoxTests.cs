using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="CompositionOffsetBox"/>, the box that records how far a sample's
/// composition time sits from its decode time.
/// </summary>
public class CompositionOffsetBoxTests
{
    /// <summary>
    /// Version 1 stores the offsets signed. Reading one used to throw, because only the offsets
    /// were allocated and not the counts beside them, so any file carrying a version 1 ctts - what
    /// ffmpeg writes with -movflags +negative_cts_offsets - could not be opened at all.
    /// </summary>
    [Fact]
    public void ReadsVersion1WithNegativeOffsets()
    {
        var box = ReadBox(BuildCtts(version: 1, entries: [(3u, 0), (1u, -40), (2u, 120)]));

        Assert.Equal(1, box.Version);
        Assert.Equal(3u, box.EntryCount);
        Assert.Equal([3u, 1u, 2u], box.SampleCount);
        Assert.Equal([0, -40, 120], box.SampleOffset0);
    }

    /// <summary>Version 0 stores the offsets unsigned, and was always readable.</summary>
    [Fact]
    public void ReadsVersion0()
    {
        var box = ReadBox(BuildCtts(version: 0, entries: [(5u, 0), (2u, 60)]));

        Assert.Equal(0, box.Version);
        Assert.Equal([5u, 2u], box.SampleCount);
        Assert.Equal([0u, 60u], box.SampleOffset);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void RoundTripsThroughWrite(byte version)
    {
        var original = BuildCtts(version, entries: [(4u, 0), (1u, version == 1 ? -20 : 20)]);

        var box = ReadBox(original);
        using var written = new MemoryStream();
        box.Write(new IsoStream(new StreamWrapper(written)));

        // The box is read from just past its header, so writing it back produces the body alone.
        Assert.Equal(original.Skip(8), written.ToArray());
    }

    private static CompositionOffsetBox ReadBox(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        var iso = new IsoStream(new StreamWrapper(stream));

        // Skip the size and type the box header carries, and read what follows.
        var box = new CompositionOffsetBox();
        iso.SeekFromBeginning(8);
        box.Read(iso, (ulong)(bytes.Length - 8) * 8);
        return box;
    }

    /// <summary>Builds a ctts by hand, so the test does not depend on the writer it is checking.</summary>
    private static byte[] BuildCtts(byte version, (uint Count, int Offset)[] entries)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        WriteBigEndian(writer, (uint)(16 + entries.Length * 8));
        writer.Write(new[] { (byte)'c', (byte)'t', (byte)'t', (byte)'s' });
        writer.Write(version);
        writer.Write(new byte[3]); // flags
        WriteBigEndian(writer, (uint)entries.Length);

        foreach (var (count, offset) in entries)
        {
            WriteBigEndian(writer, count);
            WriteBigEndian(writer, unchecked((uint)offset));
        }

        writer.Flush();
        return memory.ToArray();
    }

    private static void WriteBigEndian(BinaryWriter writer, uint value)
    {
        writer.Write((byte)(value >> 24));
        writer.Write((byte)(value >> 16));
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }
}
