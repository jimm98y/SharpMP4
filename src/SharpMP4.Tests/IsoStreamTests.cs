using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="IsoStream"/> and the sizes it is willing to allocate.</summary>
[TestClass]
public class IsoStreamTests
{
    /// <summary>
    /// A box says how long it is, and the array that runs to the end of it is sized from that. The
    /// box header is part of the file, so on its own it bounds nothing: a box declaring 256 MB in a
    /// 1 MB file had 256 MB allocated for it before a single entry was read.
    /// </summary>
    [TestMethod]
    public void RejectsAnArrayLongerThanWhatIsLeftOfTheStream()
    {
        using var memory = new MemoryStream(new byte[64]);
        var stream = new IsoStream(new StreamWrapper(memory));

        // 256 MB of entries declared, 64 bytes to read them from.
        ulong declared = 256UL * 1024 * 1024 * 8;

        // Reading past the end throws either way, so what matters is that it throws before the
        // array is allocated rather than after.
        long before = GC.GetAllocatedBytesForCurrentThread();
        Assert.ThrowsExactly<IsoEndOfStreamException>(
            () => stream.ReadUInt32ArrayTillEnd(0, declared, out uint[] _, "compatible_brands"));
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.IsTrue(allocated < 1024 * 1024, $"rejecting the count allocated {allocated} bytes");
    }

    [TestMethod]
    public void ReadsAnArrayThatFitsInTheStream()
    {
        using var memory = new MemoryStream(new byte[64]);
        var stream = new IsoStream(new StreamWrapper(memory));

        stream.ReadUInt32ArrayTillEnd(0, 64 * 8, out uint[] value, "compatible_brands");

        Assert.AreEqual(16, value.Length);
    }

    /// <summary>
    /// The same thing through the door a file comes in by: four bytes of the ftyp header are enough
    /// to ask for a very large allocation out of a very small file.
    /// </summary>
    [TestMethod]
    public void DoesNotAllocateFromABoxLargerThanTheFile()
    {
        var file = BuildFtyp(declaredSize: 0x10000020);   // 256 MB, from a 20 byte file

        long before = GC.GetAllocatedBytesForCurrentThread();
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.IsTrue(allocated < 1024 * 1024, $"parsing a {file.Length} byte file allocated {allocated} bytes");
    }

    private static byte[] BuildFtyp(uint declaredSize)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        WriteBigEndian(writer, declaredSize);
        writer.Write(new[] { (byte)'f', (byte)'t', (byte)'y', (byte)'p' });
        writer.Write(new[] { (byte)'i', (byte)'s', (byte)'o', (byte)'m' });   // major_brand
        WriteBigEndian(writer, 512);                                          // minor_version
        writer.Write(new[] { (byte)'m', (byte)'p', (byte)'4', (byte)'1' });   // one compatible brand

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
