using SharpISOBMFF;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// Boxes that were read as runs of bytes, of the fields their specifications give them, made here byte by byte: QuickTime's
/// track references and controller type, ISO/IEC 23001-17's uncompressed image configuration, JPEG 2000's component
/// mapping, and the 32 bit floats of IEEE 754 - which were read as 16.16 fixed point. Each is read as it is, and written
/// back the same.
/// </summary>
[TestClass]
public class DocumentedBoxesTests
{
    private static byte[] Box(string type, string body) =>
        Hex((8 + body.Length / 2).ToString("x8") + Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes(type)) + body);

    private static string BoxHex(string type, string body) => Convert.ToHexString(Box(type, body));

    private static string Float(float value) => BitConverter.ToUInt32(BitConverter.GetBytes(value), 0).ToString("x8");

    private static (Container Container, byte[] Written) ReadAndWrite(byte[] bytes)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(bytes))));
        var output = new MemoryStream();
        container.Write(new IsoStream(new StreamWrapper(output)));
        return (container, output.ToArray());
    }

    /// <summary>
    /// The track references of QuickTime and of hint tracks, each a list of the IDs of the tracks referred to: in a 'tref',
    /// as any reference is; and where one is read on its own, of the class of its type.
    /// </summary>
    [TestMethod]
    public void ReadsTrackReferencesOfTheirTrackIDs()
    {
        byte[] bytes = Box("tref", BoxHex("chap", "00000002" + "00000003") + BoxHex("sync", "00000001") + BoxHex("hint", "00000001" + "00000002"));
        var (container, written) = ReadAndWrite(bytes);

        var references = container.Children.OfType<TrackReferenceBox>().Single().Children.OfType<TrackReferenceTypeBox>().ToArray();
        CollectionAssert.AreEqual(new[] { "chap", "sync", "hint" }, references.Select(x => IsoStream.ToFourCC(x.FourCC)).ToArray());
        CollectionAssert.AreEqual(new uint[] { 2, 3 }, references[0].TrackIDs);
        CollectionAssert.AreEqual(new uint[] { 1, 2 }, references[2].TrackIDs, "a hint track of more than one track");
        CollectionAssert.AreEqual(bytes, written);

        byte[] alone = Box("chap", "00000002" + "00000003").Concat(Box("sync", "00000001")).Concat(Box("hint", "00000001" + "00000002")).ToArray();
        (container, written) = ReadAndWrite(alone);
        CollectionAssert.AreEqual(new uint[] { 2, 3 }, container.Children.OfType<AppleChapterListBox>().Single().TrackIDs);
        CollectionAssert.AreEqual(new uint[] { 1 }, container.Children.OfType<SynchronizedTrackBox>().Single().TrackIDs);
        CollectionAssert.AreEqual(new uint[] { 1, 2 }, container.Children.OfType<HintBox>().Single().TrackIDs);
        CollectionAssert.AreEqual(alone, written);
    }

    [TestMethod]
    public void ReadsTheControllerType()
    {
        byte[] bytes = Box("ctyp", Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes("qtvr")));
        var (container, written) = ReadAndWrite(bytes);

        Assert.AreEqual("qtvr", IsoStream.ToFourCC(container.Children.OfType<ControllerTypeBox>().Single().ControllerType));
        CollectionAssert.AreEqual(bytes, written);
    }

    /// <summary>
    /// 'uncC' of version 0 - three components of 8 bits, interleaved pixel by pixel, of the flags of its byte - and of
    /// version 1, a profile alone; 'cmpd' of red, green and blue, and a component of a URI.
    /// </summary>
    [TestMethod]
    public void ReadsTheUncompressedFrameConfiguration()
    {
        string components = "0000" + "07" + "00" + "00" + "0001" + "07" + "00" + "00" + "0002" + "07" + "00" + "00";
        string version0 = "00000000" + "00000000" + "00000003" + components + "00" + "01" + "00" + "a8" + "00000003" + "00000000" + "00000000" + "00000001" + "00000000";
        byte[] bytes = Box("uncC", version0).Concat(Box("uncC", "01000000" + Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes("rgb3"))))
            .Concat(Box("cmpd", "00000004" + "0004" + "0005" + "0006" + "8000" + Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes("urn:example:depth\0")))).ToArray();
        var (container, written) = ReadAndWrite(bytes);

        var uncC = container.Children.OfType<UncompressedFrameConfigurationBox>().ToArray();
        Assert.AreEqual(3u, uncC[0].ComponentCount);
        CollectionAssert.AreEqual(new ushort[] { 0, 1, 2 }, uncC[0].ComponentIndex);
        CollectionAssert.AreEqual(new byte[] { 7, 7, 7 }, uncC[0].ComponentBitDepthMinusOne);
        Assert.AreEqual((byte)1, uncC[0].InterleaveType, "interleaved by pixel");
        Assert.IsTrue(uncC[0].ComponentsLittleEndian);
        Assert.IsFalse(uncC[0].BlockPadLsb);
        Assert.IsTrue(uncC[0].BlockLittleEndian);
        Assert.AreEqual(3u, uncC[0].PixelSize);
        Assert.AreEqual(1u, uncC[0].NumTileColsMinusOne);
        Assert.AreEqual("rgb3", IsoStream.ToFourCC(uncC[1].Profile));

        var cmpd = container.Children.OfType<ComponentDefinitionBox>().Single();
        CollectionAssert.AreEqual(new ushort[] { 4, 5, 6, 0x8000 }, cmpd.ComponentType);
        Assert.AreEqual("urn:example:depth", cmpd.ComponentTypeUri[3].ToString().TrimEnd('\0'));
        CollectionAssert.AreEqual(bytes, written);
    }

    /// <summary>'cpat' of a Bayer pattern: of each of its four cells, a component and its gain, an IEEE 754 float.</summary>
    [TestMethod]
    public void ReadsTheComponentPatternOfItsGains()
    {
        string cells = "00000000" + Float(1.0f) + "00000001" + Float(0.5f) + "00000001" + Float(0.5f) + "00000002" + Float(1.75f);
        byte[] bytes = Box("cpat", "00000000" + "0002" + "0002" + cells);
        var (container, written) = ReadAndWrite(bytes);

        var cpat = container.Children.OfType<ComponentPatternDefinitionBox>().Single();
        CollectionAssert.AreEqual(new uint[] { 0, 1, 1, 2 }, cpat.ComponentIndex);
        CollectionAssert.AreEqual(new double[] { 1.0, 0.5, 0.5, 1.75 }, cpat.ComponentGain);
        CollectionAssert.AreEqual(bytes, written);
    }

    /// <summary>JPEG 2000's 'cmap': of each channel, its component, and whether through the palette's column.</summary>
    [TestMethod]
    public void ReadsTheJpeg2000ComponentMapping()
    {
        byte[] bytes = Box("cmap", "0000" + "01" + "00" + "0000" + "01" + "01" + "0000" + "01" + "02");
        var (container, written) = ReadAndWrite(bytes);

        var mappings = container.Children.OfType<J2kComponentMappingBox>().Single().Mappings;
        Assert.AreEqual(3, mappings.Length);
        CollectionAssert.AreEqual(new byte[] { 0, 1, 2 }, mappings.Select(x => x.PaletteColumn).ToArray());
        Assert.IsTrue(mappings.All(x => x.MappingType == 1), "through the palette");
        CollectionAssert.AreEqual(bytes, written);
    }

    /// <summary>
    /// QuickTime's 'wfex', Microsoft's WAVEFORMATEX, little-endian: of PCM, 2 channels of 16 bits at 44100 Hz, the size
    /// of what follows and it; and a WAVEFORMAT of 16 bytes, of neither.
    /// </summary>
    [TestMethod]
    public void ReadsTheWaveFormatLittleEndian()
    {
        byte[] bytes = Box("wfex", "0100" + "0200" + "44ac0000" + "10b10200" + "0400" + "1000" + "0200" + "abcd");
        var (container, written) = ReadAndWrite(bytes);

        var wfex = container.Children.OfType<MicrosoftWaveFormatExBox>().Single();
        Assert.AreEqual(((ushort)1, (ushort)2, 44100u, 176400u, (ushort)4, (ushort)16), (wfex.FormatTag, wfex.Channels, wfex.SamplesPerSec, wfex.AvgBytesPerSec, wfex.BlockAlign, wfex.BitsPerSample));
        Assert.IsTrue(wfex.ExtraSizePresent);
        Assert.AreEqual((ushort)2, wfex.ExtraSize);
        CollectionAssert.AreEqual(Hex("abcd"), wfex.Extra);
        CollectionAssert.AreEqual(bytes, written);

        byte[] waveFormat = Box("wfex", "0100" + "0100" + "80bb0000" + "00770100" + "0200" + "1000");
        (container, written) = ReadAndWrite(waveFormat);
        wfex = container.Children.OfType<MicrosoftWaveFormatExBox>().Single();
        Assert.AreEqual(48000u, wfex.SamplesPerSec);
        Assert.IsFalse(wfex.ExtraSizePresent);
        CollectionAssert.AreEqual(waveFormat, written);
    }

    /// <summary>
    /// JPEG XL's 'jxli', of varints: two key frames at 1/30 s a tick, the second 300 bytes on, of two bytes; the first's
    /// offset of 0 in two bytes, as a writer may pad one, written back as it was.
    /// </summary>
    [TestMethod]
    public void ReadsTheJpegXLFrameIndexOfVarints()
    {
        byte[] bytes = Box("jxli", "02" + "00000001" + "0000001e" + "8000" + "0a" + "01" + "ac02" + "14" + "02");
        var (container, written) = ReadAndWrite(bytes);

        var jxli = container.Children.OfType<JpegXLFrameIndexBox>().Single();
        Assert.AreEqual(2ul, jxli.FrameCount.Value);
        Assert.AreEqual((1u, 30u), (jxli.TickNumerator, jxli.TickDenominator));
        CollectionAssert.AreEqual(new ulong[] { 0, 300 }, jxli.Offset.Select(x => x.Value).ToArray());
        Assert.AreEqual(2, jxli.Offset[0].Length, "padded");
        CollectionAssert.AreEqual(new ulong[] { 10, 20 }, jxli.Duration.Select(x => x.Value).ToArray());
        CollectionAssert.AreEqual(new ulong[] { 1, 2 }, jxli.Frames.Select(x => x.Value).ToArray());
        CollectionAssert.AreEqual(bytes, written);
    }

    /// <summary>
    /// 'icef' of its codes: of offsets implied - each unit after the one before - and sizes of 16 bits; of offsets and
    /// sizes of 64 bits, past 32 bits, which were written of their low 32 bits alone.
    /// </summary>
    [TestMethod]
    public void ReadsTheCompressedUnitsOfTheirCodes()
    {
        byte[] implied = Box("icef", "00000000" + "04" + "00000003" + "0010" + "0020" + "0005");
        var (container, written) = ReadAndWrite(implied);

        var units = container.Children.OfType<GenericCompressedUnitsItemInfoBox>().Single().Units;
        Assert.AreEqual(((byte)0, (byte)1), (units.UnitOffsetCode, units.UnitSizeCode));
        CollectionAssert.AreEqual(new ulong[] { 16, 32, 5 }, units.UnitSize);
        CollectionAssert.AreEqual(new ulong[] { 0, 16, 48 }, units.UnitOffset, "each after the one before");
        CollectionAssert.AreEqual(implied, written);

        byte[] large = Box("icef", "00000000" + "90" + "00000001" + "0000000100000000" + "0000000200000003");
        (container, written) = ReadAndWrite(large);

        units = container.Children.OfType<GenericCompressedUnitsItemInfoBox>().Single().Units;
        Assert.AreEqual(0x100000000ul, units.UnitOffset[0]);
        Assert.AreEqual(0x200000003ul, units.UnitSize[0]);
        CollectionAssert.AreEqual(large, written);
    }

    /// <summary>
    /// Apple's 'uqua', of three IEEE 754 floats: read as the floats they are - as 16.16 fixed point they were other numbers -
    /// and written back as they were.
    /// </summary>
    [TestMethod]
    public void ReadsFloatsAsIeee754()
    {
        byte[] bytes = Box("uqua", "00000000" + Float(0.5f) + Float(-0.25f) + Float(0.7071068f));
        var (container, written) = ReadAndWrite(bytes);

        var uqua = container.Children.OfType<CameraSystemUnitQuaternionTransformBox>().Single();
        Assert.AreEqual((0.5, -0.25, (double)0.7071068f), (uqua.Xyz0, uqua.Xyz1, uqua.Xyz2));
        CollectionAssert.AreEqual(bytes, written);
    }
}
