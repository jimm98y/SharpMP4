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
