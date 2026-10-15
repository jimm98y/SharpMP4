using SharpISOBMFF;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// Boxes whose definitions had the layout of another: QTIF's image description ('idsc'), of QuickTime's ImageDescription
/// record; 3GPP's hypertext modifier ('href'); MPEG-4 Systems' 'mpod' reference. Each read of its fields, and written
/// back as it was.
/// </summary>
[TestClass]
public class DefinitionFixesTests
{
    private static T Read<T>(byte[] bytes) where T : Box
    {
        new IsoStream(new StreamWrapper(new MemoryStream(bytes))).ReadBox(0, null, out Box box, "");
        Assert.IsInstanceOfType<T>(box);
        CollectionAssert.AreEqual(bytes, BytesOf(box), "written back as it was read");
        return (T)box;
    }

    [TestMethod]
    public void ReadsAQuickTimeImageDescription()
    {
        // 'idsc' of a 640x480 JPEG: its record of 86 bytes, then a 'colr' extension
        var idsc = Read<AppleImageDescriptionBox>(Hex(
            "00000070" + "69647363" +
            "00000056" + "6A706567" + "000000000000" + "0000" + "0000" + "0000" + "6170706C" + "00000000" + "00000200" +
            "0280" + "01E0" + "00480000" + "00480000" + "00000000" + "0001" +
            "0550686F746F" + "0000000000000000000000000000000000000000000000000000" + "0018" + "FFFF" +
            "00000012" + "636F6C72" + "6E636C63" + "0001" + "0001" + "0001"));
        Assert.AreEqual(("jpeg", (ushort)640, (ushort)480, (ushort)24, (short)-1, (ushort)1),
            (IsoStream.ToFourCC(idsc.CompressorType), idsc.Width, idsc.Height, idsc.Depth, idsc.ColorTableId, idsc.FrameCount));
        Assert.AreEqual("colr", IsoStream.ToFourCC(idsc.Children.Single().FourCC));
    }

    [TestMethod]
    public void ReadsATimedTextHyperlink()
    {
        // characters 1 to 5 a link to "abc", of no alternative text
        var href = Read<HrefBox>(Hex("00000012" + "68726566" + "0001" + "0005" + "03" + "616263" + "00"));
        Assert.AreEqual(((ushort)1, (ushort)5, "abc", (byte)0), (href.Startcharoffset, href.Endcharoffset, System.Text.Encoding.ASCII.GetString(href.Url), href.AltLength));
    }

    [TestMethod]
    public void ReadsTheTracksOfAnMpodReference()
    {
        var mpod = Read<MpodBox>(Hex("00000010" + "6D706F64" + "00000001" + "00000002"));
        CollectionAssert.AreEqual(new uint[] { 1, 2 }, mpod.TrackIDs);
    }
}
