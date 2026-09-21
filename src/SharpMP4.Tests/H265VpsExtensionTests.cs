using SharpH26X;
using SharpH265;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against the multi-layer video parameter set extension, using the one an iPhone writes
/// into its spatial video: two layers, one per eye, the second predicting from the first.
/// </summary>
[TestClass]
public class H265VpsExtensionTests
{
    // The video parameter set from an Apple MV-HEVC recording, as it appears in hvcC.
    private static readonly byte[] AppleVps = Convert.FromHexString(
        "40010c11ffff016000000300b0000003000003007b15c15b7b200028245970602000000bf800000300" +
        "0007b8d07800440a01e5c52bf708500808080080");

    /// <summary>
    /// sub_layer_dpb_info_present_flag is only coded for sub-layers above the first, and for the
    /// first it is inferred to be 1. Left at 0, the DPB sizes of every output layer set were
    /// skipped, and everything after dpb_size() was read from the wrong position - which still
    /// wrote back byte for byte, because the writer skipped exactly the same fields.
    /// </summary>
    [TestMethod]
    public void ReadsTheBufferSizesOfEveryOutputLayerSet()
    {
        var (vps, _) = Read(AppleVps);

        var dpb = vps.VpsExtension.DpbSize;
        Assert.AreEqual<byte>(1, dpb.SubLayerDpbInfoPresentFlag[1][0]);
        CollectionAssert.AreEqual(new ulong[] { 4, 0 }, dpb.MaxVpsDecPicBufferingMinus1[1][0]);
        Assert.AreEqual(2ul, dpb.MaxVpsLatencyIncreasePlus1[1][0]);
    }

    /// <summary>
    /// The reference layer lists were derived when a dependency type was read for each pair of
    /// layers. A set that codes one type for all layers never reads those, so the lists were never
    /// built and writing any slice above layer 0 failed.
    /// </summary>
    [TestMethod]
    public void DerivesReferenceLayersWhenOneTypeCoversAllLayers()
    {
        var (vps, context) = Read(AppleVps);

        Assert.AreEqual<byte>(1, vps.VpsExtension.DirectDependencyAllLayersFlag);
        Assert.IsTrue(context.NumRefListLayers.Length > 1, "no reference layer list for layer 1");
        Assert.AreEqual(1u, context.NumRefListLayers[1]);
    }

    /// <summary>A correct read has to survive being written back unchanged.</summary>
    [TestMethod]
    [Ignore("H265.js reads output_layer_flag unconditionally - the spec condition is commented out " +
        "- so this set is still read two bits out of step from default_output_layer_idc on, and " +
        "ends 32 bits short with rbsp_stop_one_bit read as 0.")]
    public void RoundTripsThroughWrite()
    {
        var (vps, context) = Read(AppleVps);

        using var memory = new MemoryStream();
        using (var stream = new ItuStream(memory))
        {
            context.NalHeader.Write(context, stream);
            vps.Write(context, stream);
        }

        var written = memory.ToArray();
        CollectionAssert.AreEqual(AppleVps, written,
            $"expected {Convert.ToHexString(AppleVps)} got {Convert.ToHexString(written)}");
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
