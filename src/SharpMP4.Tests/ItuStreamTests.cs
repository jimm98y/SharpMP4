using SharpH26X;
using SharpMP4.Common;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="ItuStream"/>.</summary>
[TestClass]
public class ItuStreamTests
{
    /// <summary>
    /// Array lengths in the H.26x syntax are read from the stream, and several are Exp-Golomb
    /// coded, so a corrupt stream can put an enormous value there. Allocating from one unchecked
    /// let a NAL unit of a few kilobytes ask for gigabytes.
    /// </summary>
    [TestMethod]
    public void RejectsACountThatCannotFitInWhatIsLeft()
    {
        using var stream = new ItuStream(new MemoryStream(new byte[64]));

        // Every entry occupies at least one bit, so 64 bytes can never hold 125 million of them.
        Assert.ThrowsExactly<ItuEndOfStreamException>(
            () => stream.CheckArrayAllocation(125_898_939, "num_negative_pics"));
    }

    [TestMethod]
    public void AllowsACountThatFits()
    {
        using var stream = new ItuStream(new MemoryStream(new byte[64]));

        stream.CheckArrayAllocation(64 * 8, "used_by_curr_pic_s0_flag");
    }

    /// <summary>
    /// The bit reader buffers a byte ahead, so the stream position can already sit at the end
    /// while bits are still to be handed out. A small array at the very end of a valid parameter
    /// set must not be rejected.
    /// </summary>
    [TestMethod]
    public void AllowsASmallCountAtTheEndOfTheStream()
    {
        var memory = new MemoryStream(new byte[4]);
        memory.Seek(4, SeekOrigin.Begin);   // everything consumed, bits may still be buffered
        using var stream = new ItuStream(memory);

        stream.CheckArrayAllocation(1, "layer_id_included_flag");
    }

    /// <summary>
    /// Both log helpers used to build their message before asking whether it would be logged,
    /// which dominated the cost of reading a bitstream. Reading with logging off should not
    /// allocate per element.
    /// </summary>
    [TestMethod]
    public void ReadingDoesNotAllocateWhenLoggingIsOff()
    {
        var logger = new DefaultMp4Logger();   // logging disabled by default
        using var stream = new ItuStream(new MemoryStream(new byte[8192]), logger);

        stream.ReadUnsignedInt(0, 8, out byte _, "warmup");

        // Per thread, so allocations elsewhere in the process cannot creep into the measurement.
        long before = GC.GetAllocatedBytesForCurrentThread();
        ulong size = 0;
        for (int i = 0; i < 4096; i++)
            size += stream.ReadUnsignedInt(size, 8, out byte _, "sample_element");
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.AreEqual(4096ul * 8, size);
        Assert.IsTrue(allocated < 4096, $"reading 4096 elements allocated {allocated} bytes");
    }

    [TestMethod]
    public void RoundTripsBitsThroughWriteAndRead()
    {
        using var memory = new MemoryStream();
        using (var writer = new ItuStream(memory))
        {
            writer.WriteUnsignedInt(8, (byte)120, "first");
            writer.WriteUnsignedInt(16, 4242u, "second");
        }

        using var reader = new ItuStream(new MemoryStream(memory.ToArray()));
        ulong size = reader.ReadUnsignedInt(0, 8, out byte first, "first");
        reader.ReadUnsignedInt(size, 16, out uint second, "second");

        Assert.AreEqual<byte>(120, first);
        Assert.AreEqual(4242u, second);
    }

    /// <summary>
    /// Bytes read in bulk come out exactly as they do eight bits at a time: the same bytes, the
    /// same position, and the same state after, so bit reads carry on from where the bulk read
    /// stopped. The data is mostly zeros, so emulation prevention bytes turn up everywhere -
    /// including across the join with the bits in front.
    /// </summary>
    [TestMethod]
    public void ReadingBytesMatchesReadingBits()
    {
        for (int seed = 0; seed < 500; seed++)
        {
            var random = new Random(seed);
            var (headerBits, data, tail) = Pieces(random);
            byte[] stored = Write(headerBits, data, tail, bulk: false);

            using var bitwise = new ItuStream(new MemoryStream(stored));
            using var bulk = new ItuStream(new MemoryStream(stored));

            ReadBits(bitwise, headerBits.Length);
            ReadBits(bulk, headerBits.Length);

            byte[] expected = ReadBytesBitwise(bitwise, data.Length);
            byte[] actual = new byte[data.Length];
            Assert.AreEqual(data.Length, bulk.ReadBytes(actual, 0, actual.Length), $"seed {seed}");

            CollectionAssert.AreEqual(expected, actual, $"seed {seed}");
            CollectionAssert.AreEqual(data, actual, $"seed {seed}: the bytes written are not the bytes read");
            Assert.AreEqual(bitwise.Bitstream.BitsPosition, bulk.Bitstream.BitsPosition, $"seed {seed}");

            CollectionAssert.AreEqual(ReadBytesBitwise(bitwise, tail.Length), ReadBytesBitwise(bulk, tail.Length),
                $"seed {seed}: reading on after the bulk read differs");
        }
    }

    /// <summary>
    /// Bytes written in bulk come out exactly as they do eight bits at a time, emulation
    /// prevention bytes and all, and bits written after them land where they would have.
    /// </summary>
    [TestMethod]
    public void WritingBytesMatchesWritingBits()
    {
        for (int seed = 0; seed < 500; seed++)
        {
            var random = new Random(seed);
            var (headerBits, data, tail) = Pieces(random);

            CollectionAssert.AreEqual(Write(headerBits, data, tail, bulk: false), Write(headerBits, data, tail, bulk: true),
                $"seed {seed}");
        }
    }

    /// <summary>
    /// An emulation prevention byte follows two zero bytes, and a stream that opens 00 03 has only
    /// one. The reader used to take the zero it starts out holding for a byte of the stream and drop
    /// the 03 - an H.265 NAL unit header reads 00 03 for a TRAIL_N picture at temporal layer 2.
    /// </summary>
    [TestMethod]
    public void AStreamOpeningWithZeroThreeKeepsTheThree()
    {
        byte[] stored = [0x00, 0x03, 0x14, 0x40];

        using var bitwise = new ItuStream(new MemoryStream(stored));
        CollectionAssert.AreEqual(stored, ReadBytesBitwise(bitwise, stored.Length));

        using var bulk = new ItuStream(new MemoryStream(stored));
        var read = new byte[stored.Length];
        Assert.AreEqual(stored.Length, bulk.ReadBytes(read, 0, read.Length));
        CollectionAssert.AreEqual(stored, read);
    }

    [TestMethod]
    public void ReadingBytesStopsAtTheEndOfTheStream()
    {
        using var stream = new ItuStream(new MemoryStream(new byte[] { 0x11, 0x22, 0x33 }));

        var buffer = new byte[8];
        Assert.AreEqual(3, stream.ReadBytes(buffer, 0, buffer.Length));
        CollectionAssert.AreEqual(new byte[] { 0x11, 0x22, 0x33 }, buffer.Take(3).ToArray());
    }

    [TestMethod]
    public void BytesNeedAByteAlignedPosition()
    {
        using var stream = new ItuStream(new MemoryStream(new byte[4]));
        stream.Bitstream.ReadBit();

        Assert.ThrowsExactly<InvalidOperationException>(() => stream.ReadBytes(new byte[1], 0, 1));
    }

    /// <summary>
    /// Some bits in front - padded with zeros to a byte boundary, as a slice header ends - then the
    /// bytes, then some more. Mostly zeros, so that emulation prevention has plenty to do.
    /// </summary>
    private static (int[] HeaderBits, byte[] Data, byte[] Tail) Pieces(Random random)
    {
        int bits = random.Next(0, 33);
        var header = new int[bits + (8 - bits % 8) % 8];
        for (int i = 0; i < bits; i++)
            header[i] = random.Next(4) == 0 ? 1 : 0;

        return (header, ZeroHeavy(random, random.Next(0, 300)), ZeroHeavy(random, random.Next(0, 6)));
    }

    private static byte[] ZeroHeavy(Random random, int length)
    {
        var bytes = new byte[length];
        for (int i = 0; i < length; i++)
        {
            bytes[i] = random.Next(8) switch
            {
                < 4 => 0x00,
                4 => 0x01,
                5 => 0x02,
                6 => 0x03,
                _ => (byte)random.Next(256),
            };
        }

        return bytes;
    }

    private static byte[] Write(int[] headerBits, byte[] data, byte[] tail, bool bulk)
    {
        var memory = new MemoryStream();
        using (var stream = new ItuStream(memory))
        {
            foreach (int bit in headerBits)
                stream.Bitstream.WriteBit(bit);

            if (bulk)
                stream.WriteBytes(data, 0, data.Length);
            else
                WriteBytesBitwise(stream, data);

            WriteBytesBitwise(stream, tail);
        }

        return memory.ToArray();
    }

    private static void WriteBytesBitwise(ItuStream stream, byte[] bytes)
    {
        foreach (byte value in bytes)
            for (int bit = 7; bit >= 0; bit--)
                stream.Bitstream.WriteBit(value >> bit & 1);
    }

    private static void ReadBits(ItuStream stream, int count)
    {
        for (int i = 0; i < count; i++)
            stream.Bitstream.ReadBit();
    }

    private static byte[] ReadBytesBitwise(ItuStream stream, int count)
    {
        var bytes = new byte[count];
        for (int i = 0; i < count; i++)
            for (int bit = 0; bit < 8; bit++)
                bytes[i] = (byte)(bytes[i] << 1 | stream.Bitstream.ReadBit());

        return bytes;
    }
}
