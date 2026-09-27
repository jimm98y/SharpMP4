using SharpH26X;
using SharpH265;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against the multi-layer video parameter set extension, using the one an iPhone writes
/// into its spatial video: two layers, one per eye, the second predicting from the first.
///
/// Reading a set back and writing it out again is no check on its own here. Read and write are
/// generated from the same syntax, so a field read in the wrong place is written back in the same
/// wrong place, and every misread in this file round-tripped byte for byte. The expectations below
/// come from outside the parser instead: the video is 1920x1080, coded 1920x1088, so the
/// representation format has to say so; and ffmpeg's trace_headers places the end of the payload
/// at bit 448.
/// </summary>
[TestClass]
public class H265VpsExtensionTests
{
    // The video parameter set from an Apple MV-HEVC recording, as it appears in hvcC.
    private static readonly byte[] AppleVps = Convert.FromHexString(
        "40010c11ffff016000000300b0000003000003007b15c15b7b200028245970602000000bf800000300" +
        "0007b8d07800440a01e5c52bf708500808080080");

    /// <summary>
    /// The representation format comes late in the extension, after everything the tests below
    /// cover, so reading the picture size right is the check that everything before it was read
    /// in step. Each of the misreads fixed here left it reading 16864x272.
    /// </summary>
    [TestMethod]
    public void ReadsThePictureSizeOfTheVideo()
    {
        var (vps, _) = Read(AppleVps);

        var format = vps.VpsExtension.RepFormat[0];
        Assert.AreEqual(1920u, (uint)format.PicWidthVpsInLumaSamples);
        Assert.AreEqual(1088u, (uint)format.PicHeightVpsInLumaSamples);
        Assert.AreEqual<byte>(1, format.ConformanceWindowVpsFlag);
    }

    /// <summary>
    /// output_layer_flag is only coded when default_output_layer_idc is 2 or the output layer set
    /// is an additional one; otherwise every layer of the set is an output layer, and every output
    /// layer is necessary. Two misderivations left only the first layer necessary: the inference
    /// loop stopped one layer short, and the count of necessary layers reused the marking loop's
    /// variable and ran it to the end.
    /// </summary>
    [TestMethod]
    public void MarksEveryOutputLayerNecessary()
    {
        var (_, context) = Read(AppleVps);

        CollectionAssert.AreEqual(new uint[] { 1, 1 }, context.OutputLayerFlag[1]);
        CollectionAssert.AreEqual(new uint[] { 1, 1 }, context.NecessaryLayerFlag[1]);
    }

    /// <summary>
    /// sub_layer_dpb_info_present_flag is only coded above the first sub-layer and is inferred to
    /// be 1 for the first, so each output layer set carries a buffer size for every necessary
    /// layer. Both eyes use B pictures, so both keep five.
    /// </summary>
    [TestMethod]
    public void ReadsTheBufferSizeOfEveryLayer()
    {
        var (vps, _) = Read(AppleVps);

        var dpb = vps.VpsExtension.DpbSize;
        Assert.AreEqual<byte>(1, dpb.SubLayerDpbInfoPresentFlag[1][0]);
        Assert.AreEqual(4ul, dpb.MaxVpsDecPicBufferingMinus1[1][0][0]);   // [ ols ][ layer ][ sub-layer ]
        Assert.AreEqual(4ul, dpb.MaxVpsDecPicBufferingMinus1[1][1][0]);
        Assert.AreEqual(2ul, dpb.MaxVpsNumReorderPics[1][0]);
    }

    /// <summary>
    /// The reference layer lists were derived only when a dependency type was read for each pair
    /// of layers. This set codes one type for all layers, so the lists stayed empty and writing any
    /// slice above layer 0 threw.
    /// </summary>
    [TestMethod]
    public void DerivesReferenceLayersWhenOneTypeCoversAllLayers()
    {
        var (vps, context) = Read(AppleVps);

        Assert.AreEqual<byte>(1, vps.VpsExtension.DirectDependencyAllLayersFlag);
        Assert.IsTrue(context.NumRefListLayers.Length > 1, "no reference layer list for layer 1");
        Assert.AreEqual(1u, context.NumRefListLayers[1]);
    }

    // Conformance stream LAYERID_A_NOKIA_2: two independent layers, the second with nuh_layer_id 2,
    // and one additional layer set of the second alone. 1280x720.
    private static readonly byte[] NokiaLayerIdVps = Convert.FromHexString(
        "40010c11ffff0160000003000003000003000003007b9490957f5d080028480a5c050000030000030000030000030005" +
        "e23682800168500fd14a48a4d2");

    // Conformance stream ALPHA_A_BBC_1: a base layer and an alpha layer predicted from it, with a
    // VUI. 1920x1080.
    private static readonly byte[] BbcAlphaVps = Convert.FromHexString(
        "40010c11ffff0180000003000003000003000003007b94905700000303e90000ea607f7b180000c3024e0f0000030000" +
        "1f100000030000f75907800438a0085293dfc85010101608");

    // Conformance stream MVHEVCS_D_NTT_3: two views coded independently of each other. 1024x768.
    private static readonly byte[] NttViewsVps = Convert.FromHexString(
        "40010c11ffff01600000030000030000030000030099949056ff99200021192e0c04000003001f100000030001331a08" +
        "000601403f45293480");

    /// <summary>
    /// The layer variables are indexed by nuh_layer_id, which need not count up from 0 - here the
    /// second layer is 2. Sized by the number of layers, they ran out at it.
    /// </summary>
    [TestMethod]
    public void IndexesLayersByTheirNuhLayerId()
    {
        var (vps, context) = Read(NokiaLayerIdVps);

        Assert.AreEqual(2u, vps.VpsExtension.LayerIdInNuh[1]);
        Assert.AreEqual(1u, context.LayerIdxInVps[2]);
        Assert.AreEqual(0u, context.NumDirectRefLayers[2]);
    }

    /// <summary>
    /// A tree of layers lists the layer heading it and then the layers predicted from it, so its
    /// count starts at 1 (F-6). Counted from 0, a layer alone made an empty tree,
    /// highest_layer_idx_plus1 was read 0 bits wide instead of 1, and with the additional layer set
    /// it describes lost, everything after it was misread. The picture size coming out right is
    /// the check that it was not.
    /// </summary>
    [TestMethod]
    public void CountsTheLayerHeadingATree()
    {
        var (vps, context) = Read(NokiaLayerIdVps);

        Assert.AreEqual(2u, context.NumIndependentLayers);
        CollectionAssert.AreEqual(new uint[] { 1, 1 }, context.NumLayersInTreePartition);
        Assert.AreEqual(1ul, vps.VpsExtension.HighestLayerIdxPlus1[0][1]);
        CollectionAssert.AreEqual(new[] { 2 }, context.LayerSetLayerIdList[2].Take(context.NumLayersInIdList[2]).ToArray());

        var format = vps.VpsExtension.RepFormat[0];
        Assert.AreEqual(1280u, (uint)format.PicWidthVpsInLumaSamples);
        Assert.AreEqual(720u, (uint)format.PicHeightVpsInLumaSamples);
    }

    /// <summary>
    /// Two layers coded independently: no direct_dependency_flag set, so two trees, and
    /// num_add_layer_sets is coded. The dependencies used to be worked out at each flag, reading the
    /// rows of layers not read yet.
    /// </summary>
    [TestMethod]
    public void ReadsLayersCodedIndependently()
    {
        var (vps, context) = Read(NttViewsVps);

        Assert.AreEqual(2u, context.NumIndependentLayers);
        Assert.AreEqual(2u, context.NumViews);
        var format = vps.VpsExtension.RepFormat[0];
        Assert.AreEqual(1024u, (uint)format.PicWidthVpsInLumaSamples);
        Assert.AreEqual(768u, (uint)format.PicHeightVpsInLumaSamples);
    }

    /// <summary>
    /// cross_layer_irap_aligned_flag is coded only when cross_layer_pic_type_aligned_flag is 0, and
    /// is otherwise inferred to be 1 - so all_layers_idr_aligned_flag after it is coded. Left 0,
    /// that flag went unread and the rest of the VUI was read a bit early, past the end of the set.
    /// </summary>
    [TestMethod]
    public void InfersIrapAlignmentFromPictureTypeAlignment()
    {
        var (vps, _) = Read(BbcAlphaVps);

        var vui = vps.VpsExtension.VpsVui;
        Assert.AreEqual<byte>(1, vui.CrossLayerPicTypeAlignedFlag);
        Assert.AreEqual<byte>(1, vui.CrossLayerIrapAlignedFlag);
        Assert.AreEqual<byte>(1, vui.AllLayersIdrAlignedFlag);
        Assert.AreEqual<byte>(1, vui.TilesNotInUseFlag);
        Assert.AreEqual<byte>(1, vui.WppNotInUseFlag);
        Assert.AreEqual<byte>(0, vps.VpsExtension2Flag);
    }

    /// <summary>Read in step, the set has to come back unchanged.</summary>
    [TestMethod]
    public void RoundTripsThroughWrite()
    {
        var (vps, context) = Read(AppleVps);

        using var memory = new MemoryStream();
        using (var stream = new ItuStream(memory))
        {
            context.NalHeader.Write(context, stream);
            vps.Write(context, stream);
        }

        CollectionAssert.AreEqual(AppleVps, memory.ToArray());
    }

    private static (VideoParameterSetRbsp Vps, H265Context Context) Read(byte[] nalu)
    {
        var context = new H265Context();
        using var stream = new ItuStream(new MemoryStream(nalu));

        var nalUnit = new NalUnit((uint)nalu.Length);
        context.NalHeader = nalUnit;
        nalUnit.Read(context, stream);

        // The syntax reads back values it has already parsed, so the set has to be in the
        // context before reading starts.
        var vps = new VideoParameterSetRbsp();
        context.VideoParameterSetRbsp = vps;
        vps.Read(context, stream);

        return (vps, context);
    }
}
