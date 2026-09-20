using SharpH26X;
using SharpMP4.Common;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="ItuStream"/>.</summary>
public class ItuStreamTests
{
    /// <summary>
    /// Array lengths in the H.26x syntax are read from the stream, and several are Exp-Golomb
    /// coded, so a corrupt stream can put an enormous value there. Allocating from one unchecked
    /// let a NAL unit of a few kilobytes ask for gigabytes.
    /// </summary>
    [Fact]
    public void RejectsACountThatCannotFitInWhatIsLeft()
    {
        using var stream = new ItuStream(new MemoryStream(new byte[64]));

        // Every entry occupies at least one bit, so 64 bytes can never hold 125 million of them.
        Assert.Throws<ItuEndOfStreamException>(
            () => stream.CheckArrayAllocation(125_898_939, "num_negative_pics"));
    }

    [Fact]
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
    [Fact]
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
    [Fact]
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

        Assert.Equal(4096ul * 8, size);
        Assert.True(allocated < 4096, $"reading 4096 elements allocated {allocated} bytes");
    }

    [Fact]
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

        Assert.Equal(120, first);
        Assert.Equal(4242u, second);
    }
}
