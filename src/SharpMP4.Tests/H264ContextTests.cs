// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
