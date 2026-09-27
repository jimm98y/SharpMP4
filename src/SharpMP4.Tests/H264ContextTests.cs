using SharpH264;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="H264Context"/>: how it keeps and activates parameter sets.</summary>
[TestClass]
public class H264ContextTests
{
    /// <summary>
    /// A subset SPS - MVC's, SVC's - is kept under its own id. It registered itself as the SPS
    /// being parsed instead, so a PPS for a non-base view, naming it, threw: "SeqParameterSet with
    /// id 1 not found".
    /// </summary>
    [TestMethod]
    public void APpsFindsTheSubsetSpsItNames()
    {
        var context = new H264Context { NalHeader = new NalUnit(0) { NalUnitType = H264NALTypes.SUBSET_SPS } };
        var data = new SeqParameterSetData { SeqParameterSetId = 1, PicWidthInMbsMinus1 = 79 };
        var subset = new SubsetSeqParameterSetRbsp { SeqParameterSetData = data };
        context.SubsetSeqParameterSetRbsp = subset;

        context.SetSeqParameterSetId(1, data);
        Assert.AreSame(subset, context.SubsetSeqParameterSets[1]);

        context.NalHeader.NalUnitType = H264NALTypes.PPS;
        context.SetSeqParameterSetId(1, new PicParameterSetRbsp());

        Assert.AreSame(data, context.SeqParameterSetRbsp.SeqParameterSetData);
        Assert.AreEqual(80ul, context.PicWidthInMbs);
    }

    /// <summary>
    /// A set sent again under the same id replaces the one there. The first was kept, and slices
    /// were read against it.
    /// </summary>
    [TestMethod]
    public void ASetSentAgainUnderTheSameIdReplacesTheOldOne()
    {
        var context = new H264Context { NalHeader = new NalUnit(0) { NalUnitType = H264NALTypes.SPS } };
        var before = new SeqParameterSetRbsp { SeqParameterSetData = new SeqParameterSetData { PicWidthInMbsMinus1 = 39 } };
        var after = new SeqParameterSetRbsp { SeqParameterSetData = new SeqParameterSetData { PicWidthInMbsMinus1 = 119 } };

        context.SeqParameterSetRbsp = before;
        context.SetSeqParameterSetId(0, before.SeqParameterSetData);
        context.SeqParameterSetRbsp = after;
        context.SetSeqParameterSetId(0, after.SeqParameterSetData);

        var pps = new PicParameterSetRbsp { PicParameterSetId = 0, SeqParameterSetId = 0 };
        context.SetPicParameterSetId(0, pps);
        context.NalHeader.NalUnitType = H264NALTypes.IDR_SLICE;
        context.SetPicParameterSetId(0, new SliceHeader());

        Assert.AreSame(after, context.SeqParameterSetRbsp);
        Assert.AreEqual(120ul, context.PicWidthInMbs);
    }

    /// <summary>
    /// chroma_format_idc is coded only for the high profiles and is 1 - 4:2:0 - otherwise
    /// (7.4.2.1.1). It was left 0, so a Baseline or Main stream read as monochrome whenever
    /// ChromaArrayType was worked out from it, and read chroma weights and offsets it does not have
    /// the other way round.
    /// </summary>
    [TestMethod]
    public void ChromaFormatIsFourTwoZeroWhenNotCoded()
    {
        var context = new H264Context { NalHeader = new NalUnit(0) { NalUnitType = H264NALTypes.SPS } };
        var data = new SeqParameterSetData();
        context.OnProfileIdc(data);
        Assert.AreEqual(1ul, data.ChromaFormatIdc);

        context.SeqParameterSetRbsp = new SeqParameterSetRbsp { SeqParameterSetData = data };
        context.SetSeqParameterSetId(0, data);
        context.SetSeqParameterSetId(0, new PicParameterSetRbsp());

        Assert.AreEqual(1ul, context.ChromaArrayType);
    }
}
