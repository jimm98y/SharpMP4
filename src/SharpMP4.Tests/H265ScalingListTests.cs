using SharpH26X;
using SharpH265;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against scaling_list_data(), using the sequence parameter set of the conformance stream
/// SLIST_A_Sony_5, which codes most of its scaling lists coefficient by coefficient. The values
/// expected are ffmpeg's reading of the same set.
/// </summary>
[TestClass]
public class H265ScalingListTests
{
    // The sequence parameter set of SLIST_A_Sony_5, NAL unit header included.
    private static readonly byte[] SlistSps = Convert.FromHexString(
        "42010101600000030000030000030000030078a0068201e1fe59499246f8f2c997932c8501e003fe03fa80203c07f280" +
        "406010163c040018407001840c203040c206103080c10308184070018404042607f03000e1030018006001800c006001" +
        "8006003000e1030008194d10730a3cc3849cc2028709c2464e1309528240a42a09620114201182304828928b1c936239" +
        "8c71d14c21ff5ebe4fc47f5f9be2bc21c35822828358f840787878f0583c78f0743c78f1e0481e3c78f1e0561e3c78f1" +
        "e3c0c83c78f1e3c78f0320f1e3c78f1e0561e3c78f1e0481e3c78f0743c78f0583c78787842340fc101e1e1e3c160f1e" +
        "3c1d0f1e3c7812078f1e3c7815878f1e3c78f0320f1e3c78f1e3c0c83c78f1e3c7815878f1e3c7812078f1e3c1d0f1e3" +
        "c160f1e1e1e1063c041840787878f0583c78f0743c78f1e0481e3c78f1e0561e3c78f1e3c0c83c78f1e3c78f0320f1e3" +
        "c78f1e0561e3c78f1e0481e3c78f0743c78f0583c7878784203b80202111c1f1c38168e1c381d8e1c387012470e1c387" +
        "015c70e1c3870e0328e1c3870e1c380ca3870e1c387015c70e1c387012470e1c381d8e1c38168e1c1f1c11807b804084" +
        "4707c70e05a3870e0763870e1c0491c3870e1c0571c3870e1c380ca3870e1c3870e0328e1c3870e1c0571c3870e1c049" +
        "1c3870e0763870e05a38707c70463c07e044707c70e05a3870e0763870e1c0491c3870e1c0571c3870e1c380ca3870e1" +
        "c3870e0328e1c3870e1c0571c3870e1c0491c3870e0763870e05a38707c7045830b11552626197d9f5e6fcb3ebc288c4" +
        "e5d9");

    /// <summary>
    /// scaling_list_data() works out ScalingList as it reads each coefficient, and the array it
    /// works it out into was never allocated, so a set that coded a list rather than predicting it
    /// threw at the first coefficient.
    /// </summary>
    [TestMethod]
    public void ReadsListsCodedCoefficientByCoefficient()
    {
        var sps = Read(SlistSps);

        Assert.AreEqual<byte>(1, sps.SpsScalingListDataPresentFlag);
        var lists = sps.ScalingListData;
        Assert.AreEqual<byte>(1, lists.ScalingListPredModeFlag[0][0]);
        Assert.AreEqual(1ul, lists.ScalingListPredMatrixIdDelta[0][1]);
        Assert.AreEqual(2ul, lists.ScalingListPredMatrixIdDelta[1][3]);
        CollectionAssert.AreEqual(new long[] { -7, -6, 0, -7, 119, 247 }, lists.ScalingListDcCoefMinus8[0]);
        Assert.AreEqual(-7L, lists.ScalingListDcCoefMinus8[1][0]);

        // What follows the lists, read in step.
        Assert.AreEqual<byte>(1, sps.AmpEnabledFlag);
        Assert.AreEqual<byte>(0, sps.PcmEnabledFlag);
        Assert.AreEqual<byte>(0, sps.VuiParametersPresentFlag);
    }

    private static SeqParameterSetRbsp Read(byte[] nalu)
    {
        var context = new H265Context();
        using var stream = new ItuStream(new MemoryStream(nalu));

        var nalUnit = new NalUnit((uint)nalu.Length);
        context.NalHeader = nalUnit;
        nalUnit.Read(context, stream);

        var sps = new SeqParameterSetRbsp();
        context.SeqParameterSetRbsp = sps;
        sps.Read(context, stream);
        return sps;
    }
}
