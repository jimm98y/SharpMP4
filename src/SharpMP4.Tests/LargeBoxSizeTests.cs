using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// A box of more than 4 GB is written with a largesize (ISO/IEC 14496-12 4.2), a header of 16 bytes: its size counts them,
/// as the writer writes them, where it counted 8 and the largesize came out 8 bytes too small.
/// </summary>
[TestClass]
public class LargeBoxSizeTests
{
    private const long Payload = 5L << 30; // 5 GB of 'mdat' data, not held anywhere: only counted

    private static MediaDataBox LargeMdat() =>
        new MediaDataBox { Data = new StreamMarker(0, Payload, new IsoStream(new StreamWrapper(new MemoryStream()))) };

    [TestMethod]
    public void CountsTheLargesizeOfABoxOver4GB()
    {
        var mdat = LargeMdat();
        Assert.AreEqual((ulong)(Payload + 16) * 8, IsoStream.CalculateBoxSize(mdat), "the box, as written");

        var container = new Container();
        container.Children.Add(new MediaDataBox { Data = new StreamMarker(0, 0, new IsoStream(new StreamWrapper(new MemoryStream()))) });
        container.Children.Add(mdat);
        Assert.AreEqual((ulong)(8 + Payload + 16) * 8, container.CalculateSize(), "the file, as written");

        // the header written: size 1, then the largesize of the whole box, its 16 byte header with it
        using var output = new MemoryStream();
        new IsoStream(new StreamWrapper(output)).WriteBoxHeader(mdat);
        byte[] header = output.ToArray();
        Assert.AreEqual(16, header.Length);
        Assert.AreEqual(1u, System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(header));
        Assert.AreEqual((ulong)(Payload + 16), System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(header.AsSpan(8)));
    }

    /// <summary>One under 4 GB keeps its header of 8 bytes.</summary>
    [TestMethod]
    public void KeepsTheHeaderOfABoxUnder4GB()
    {
        var mdat = new MediaDataBox { Data = new StreamMarker(0, uint.MaxValue - 8, new IsoStream(new StreamWrapper(new MemoryStream()))) };
        Assert.AreEqual((ulong)uint.MaxValue * 8, IsoStream.CalculateBoxSize(mdat));
    }
}
