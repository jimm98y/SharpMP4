using SharpH266;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="H266Context"/>: the variables the H.266 syntax is conditioned on and
/// does not code, which the context works out or infers. Each of these was worked out only when an
/// optional element was coded, or at the wrong point, and the conformance streams read differently
/// from ffmpeg until they were not.
/// </summary>
[TestClass]
public class H266ContextTests
{
    /// <summary>
    /// A PPS codes the first tile columns and rows, and the last size coded repeats to fill the
    /// picture (6.5.1). The lists were as long as the sizes coded, so the first repeat ran past
    /// their end.
    /// </summary>
    [TestMethod]
    public void RepeatsTheLastTileSizeToFillThePicture()
    {
        var context = new H266Context
        {
            SeqParameterSetRbsp = new SeqParameterSetRbsp(),
            PicParameterSetRbsp = new PicParameterSetRbsp
            {
                PpsNumExpTileColumnsMinus1 = 0,
                PpsTileColumnWidthMinus1 = [2],
                PpsNumExpTileRowsMinus1 = 0,
                PpsTileRowHeightMinus1 = [1],
            },
            PicWidthInCtbsY = 10,
            PicHeightInCtbsY = 6,
        };

        context.OnPpsTileRowHeightMinus1(0);

        Assert.AreEqual(4, context.NumTileColumns, "3 + 3 + 3 + the 1 left");
        Assert.AreEqual(3, context.NumTileRows);
        Assert.AreEqual(12, context.NumTilesInPic);
        CollectionAssert.AreEqual(new ulong[] { 0, 3, 6, 9, 10 }, context.TileColBdVal);
    }

    /// <summary>
    /// Without pps_rpl1_idx_present_flag, list 1 does as list 0 did (7.4.9): taken from the SPS
    /// when list 0 is, at the same index. RplsIdx was worked out only as rpl_idx was read, so the
    /// structure list 1 used was whatever the slice before left.
    /// </summary>
    [TestMethod]
    public void ListOneTakesListZerosStructureWhenNotCoded()
    {
        var context = new H266Context
        {
            SeqParameterSetRbsp = new SeqParameterSetRbsp { SpsNumRefPicLists = [3, 3] },
            PicParameterSetRbsp = new PicParameterSetRbsp { PpsRpl1IdxPresentFlag = 0 },
        };
        var lists = new RefPicLists { RplSpsFlag = [1, 0], RplIdx = [2, 0] };

        Assert.AreEqual<byte>(1, context.InferRplSpsFlag(lists, 1));
        Assert.AreEqual(2ul, lists.RplIdx[1]);
        Assert.AreEqual(2ul, context.RplsIdx[1]);
    }

    /// <summary>
    /// A list coded in the header is at index sps_num_ref_pic_lists, past the SPS's own.
    /// </summary>
    [TestMethod]
    public void AListCodedInTheHeaderIsPastTheSpsOnes()
    {
        var context = new H266Context
        {
            SeqParameterSetRbsp = new SeqParameterSetRbsp { SpsNumRefPicLists = [3, 3] },
            PicParameterSetRbsp = new PicParameterSetRbsp { PpsRpl1IdxPresentFlag = 1 },
        };
        var lists = new RefPicLists { RplSpsFlag = [0, 0], RplIdx = [0, 0] };

        Assert.AreEqual<byte>(0, context.InferRplSpsFlag(lists, 0));
        Assert.AreEqual(3ul, context.RplsIdx[0]);
    }

    /// <summary>
    /// What a slice header leaves out is inferred before it is read (7.4.8): a slice of a picture
    /// with no inter slices is an I slice, the reference counts are overridden, and a P slice's
    /// collocated picture is in list 0. sh_slice_type left at 0 read as B, so every intra slice
    /// went on to read reference counts.
    /// </summary>
    [TestMethod]
    public void InfersWhatASliceHeaderLeavesOut()
    {
        var context = new H266Context();
        var header = new SliceHeader();

        context.InferSliceHeader(header);

        Assert.AreEqual(H266FrameTypes.I, header.ShSliceType);
        Assert.AreEqual<byte>(1, header.ShNumRefIdxActiveOverrideFlag);
        Assert.AreEqual<byte>(1, header.ShCollocatedFromL0Flag);
    }

    /// <summary>
    /// With the override on and sh_num_ref_idx_active_minus1 not coded - a list of one entry - it
    /// is 0, and a P slice predicts from one picture of list 0 and none of list 1. NumRefIdxActive
    /// was worked out only as sh_num_ref_idx_active_minus1 was read.
    /// </summary>
    [TestMethod]
    public void CountsActiveReferencesWhenNoCountIsCoded()
    {
        var header = new SliceHeader { ShSliceType = H266FrameTypes.P, ShNumRefIdxActiveOverrideFlag = 1 };
        var context = new H266Context
        {
            PicParameterSetRbsp = new PicParameterSetRbsp { PpsNumRefIdxDefaultActiveMinus1 = [3, 3] },
            SliceLayerRbsp = new SliceLayerRbsp { SliceHeader = header },
        };

        CollectionAssert.AreEqual(new ulong[] { 1, 0 }, context.DeriveNumRefIdxActive());
    }

    /// <summary>
    /// A picture header sent in its own NAL unit is the one the picture's slices read against.
    /// They read it through the slice header's own picture header, which such slices lack.
    /// </summary>
    [TestMethod]
    public void SlicesReadAgainstAPictureHeaderSentOnItsOwn()
    {
        var context = new H266Context();
        var pictureHeader = new PictureHeaderStructure();

        context.OnPhGdrOrIrapPicFlag(pictureHeader);

        Assert.AreSame(pictureHeader, context.PictureHeader);
    }

    /// <summary>
    /// What a picture header leaves out is inferred as it is read (7.4.3.8): the collocated picture
    /// is in list 0, and intra slices are allowed. Only as it is read - written, the values coded
    /// would be written over - so it is a step of its own, apart from making the header current.
    /// </summary>
    [TestMethod]
    public void InfersWhatAPictureHeaderLeavesOut()
    {
        var context = new H266Context();
        var pictureHeader = new PictureHeaderStructure();

        context.InferPictureHeader(pictureHeader);

        Assert.AreEqual<byte>(1, pictureHeader.PhCollocatedFromL0Flag);
        Assert.AreEqual<byte>(1, pictureHeader.PhIntraSliceAllowedFlag);
    }

    /// <summary>
    /// Making a picture header current infers nothing into it: done as it is written too, it would
    /// write over the values coded.
    /// </summary>
    [TestMethod]
    public void MakingAPictureHeaderCurrentLeavesItsValues()
    {
        var context = new H266Context();
        var pictureHeader = new PictureHeaderStructure { PhCollocatedFromL0Flag = 0, PhIntraSliceAllowedFlag = 0 };

        context.OnPhGdrOrIrapPicFlag(pictureHeader);

        Assert.AreEqual<byte>(0, pictureHeader.PhCollocatedFromL0Flag);
        Assert.AreEqual<byte>(0, pictureHeader.PhIntraSliceAllowedFlag);
    }
}
