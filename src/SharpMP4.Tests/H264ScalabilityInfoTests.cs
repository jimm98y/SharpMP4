using SharpH26X;
using SharpH264;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against the scalability information SEI message, which Rec. ITU-T H.264 (11/2007) codes
/// with its parameter set counts minus one and later editions without. The SVC conformance
/// bitstreams follow the 2007 syntax: read the later way, a message with parameter sets in it ran
/// off the end of its payload, and every SVC stream with one was unreadable.
/// </summary>
[TestClass]
public class H264ScalabilityInfoTests
{
    // The first NAL unit of the conformance stream SVCBST-10-L1: an SEI with one scalability
    // information message of 94 bytes, eight layers of a 360x240 picture.
    private static readonly byte[] SvcbstSei = Convert.FromHexString(
        "06185e022000007820f00171ffd000041e083c005c7afec00040f043c002e3d7f20000c1e08f0005c7afe020401000" +
        "f041e0017079042aa8041002041e083c002e0f3842aa8041802081e0878002e0f3842aa80420020c1e08f0002e0f38" +
        "42aaa080");

    /// <summary>
    /// Read by the 2007 syntax, layer 0 names SPS 0 and PPS 0 and the next layer starts where it
    /// should: layer 1, one temporal level up. Read by the later syntax, layer 0 named no sets and
    /// "layer 1" was read from two bits early.
    /// </summary>
    [TestMethod]
    public void ReadsTheMessageAsThe2007EditionCodesIt()
    {
        var sei = Read(SvcbstSei, out _);

        var payload = sei.SeiMessage[0].SeiPayload;
        Assert.IsNull(payload.ScalabilityInfo);
        var info = payload.ScalabilityInfo2007;
        Assert.IsNotNull(info);

        Assert.AreEqual(7ul, info.NumLayersMinus1);
        Assert.AreEqual(22ul, info.FrmWidthInMbsMinus1[0], "360 / 16, rounded up, minus 1");
        Assert.AreEqual(14ul, info.FrmHeightInMbsMinus1[0]);
        Assert.AreEqual(0ul, info.NumSeqParameterSetMinus1[0], "one SPS");
        Assert.AreEqual(1ul, info.LayerId[1]);
        Assert.AreEqual(1u, (uint)info.TemporalId[1]);
    }

    /// <summary>Written back, the message keeps the syntax it was read in.</summary>
    [TestMethod]
    public void WritesTheMessageBackAsItWasRead()
    {
        var sei = Read(SvcbstSei, out var context);

        using var memory = new MemoryStream();
        using (var stream = new ItuStream(memory))
        {
            context.NalHeader.Write(context, stream);
            sei.Write(context, stream);
        }

        CollectionAssert.AreEqual(SvcbstSei, memory.ToArray());
    }

    private static SeiRbsp Read(byte[] nalu, out H264Context context)
    {
        context = new H264Context();
        using var stream = new ItuStream(new MemoryStream(nalu));

        var nalUnit = new NalUnit((uint)nalu.Length);
        context.NalHeader = nalUnit;
        nalUnit.Read(context, stream);

        var sei = new SeiRbsp();
        context.SeiRbsp = sei;
        sei.Read(context, stream);
        return sei;
    }
}
