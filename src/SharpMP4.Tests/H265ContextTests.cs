using SharpH265;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="H265Context"/>.</summary>
[TestClass]
public class H265ContextTests
{
    /// <summary>
    /// A slice is read against the sets it names, not the last ones parsed. The width of
    /// slice_segment_address comes from the picture size, so when another set - another layer's,
    /// say - was parsed after the slice's own, the second slice of every picture was misread, and
    /// the rest of its header with it. A first slice has no address, which is how it went unseen.
    /// </summary>
    [TestMethod]
    public void ASliceIsReadAgainstTheSetsItNames()
    {
        var context = new H265Context();
        var small = Sps(id: 0, width: 640, height: 480);
        var large = Sps(id: 1, width: 3840, height: 2160);
        var pps = new PicParameterSetRbsp { PpsPicParameterSetId = 0, PpsSeqParameterSetId = 0 };

        context.SeqParameterSets[0] = small;
        context.SeqParameterSets[1] = large;
        context.PicParameterSets[0] = pps;

        // The slice's picture parameter set is already the one in force, and the large sequence
        // parameter set was the last parsed, working out its picture size as it went.
        context.PicParameterSetRbsp = pps;
        context.SeqParameterSetRbsp = large;
        context.OnLog2DiffMaxMinLumaCodingBlockSize();
        Assert.AreEqual(60 * 34, context.PicSizeInCtbsY);

        context.SetSlicePicParameterSetId(0);

        Assert.AreSame(small, context.SeqParameterSetRbsp);
        Assert.AreEqual(10 * 8, context.PicSizeInCtbsY, "640x480 in 64x64 coding tree blocks");
    }

    /// <summary>
    /// A set parsed under an id that is already taken replaces the one there, as the spec has it:
    /// a stream changes resolution, or two streams are joined, by sending new sets under the same
    /// ids. Only the first was kept, and once a slice looked its sets up by id it was read against
    /// the old picture size.
    /// </summary>
    [TestMethod]
    public void ASetSentAgainUnderTheSameIdReplacesTheOldOne()
    {
        var context = new H265Context();
        var before = Sps(id: 0, width: 640, height: 480);
        var after = Sps(id: 0, width: 3840, height: 2160);
        var ppsBefore = new PicParameterSetRbsp { PpsPicParameterSetId = 0, PpsSeqParameterSetId = 0 };
        var ppsAfter = new PicParameterSetRbsp { PpsPicParameterSetId = 0, PpsSeqParameterSetId = 0 };

        // Each set registers itself as its id is read.
        context.SeqParameterSetRbsp = before;
        context.SetSpsSeqParameterSetId(0);
        context.PicParameterSetRbsp = ppsBefore;
        context.SetPpsPicParameterSetId(0);

        context.SeqParameterSetRbsp = after;
        context.SetSpsSeqParameterSetId(0);
        context.PicParameterSetRbsp = ppsAfter;
        context.SetPpsPicParameterSetId(0);

        context.SetSlicePicParameterSetId(0);

        Assert.AreSame(ppsAfter, context.PicParameterSetRbsp);
        Assert.AreSame(after, context.SeqParameterSetRbsp);
        Assert.AreEqual(60 * 34, context.PicSizeInCtbsY, "3840x2160 in 64x64 coding tree blocks");
    }

    [TestMethod]
    public void ASliceNamingAMissingSetIsRejected()
    {
        var context = new H265Context();
        context.PicParameterSets[0] = new PicParameterSetRbsp { PpsPicParameterSetId = 0, PpsSeqParameterSetId = 0 };
        context.PicParameterSetRbsp = context.PicParameterSets[0];

        Assert.ThrowsExactly<Exception>(() => context.SetSlicePicParameterSetId(5));
    }

    /// <summary>A 4:2:0 set with 64x64 coding tree blocks.</summary>
    private static SeqParameterSetRbsp Sps(ulong id, ulong width, ulong height) => new SeqParameterSetRbsp
    {
        SpsSeqParameterSetId = id,
        ChromaFormatIdc = 1,
        PicWidthInLumaSamples = width,
        PicHeightInLumaSamples = height,
        Log2MinLumaCodingBlockSizeMinus3 = 0,
        Log2DiffMaxMinLumaCodingBlockSize = 3,
    };
}
