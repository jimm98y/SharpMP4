using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="AudioSpecificConfig"/> with a program_config_element (ISO/IEC 14496-3:2009,
/// Table 4.2): configurations of FFmpeg's AAC conformance samples (fate-suite, aac/), read as AACTrack reads
/// them, against what the specification reads out of them.
/// </summary>
[TestClass]
public class AudioSpecificConfigTests
{
    /// <summary>
    /// The PCE's byte_alignment() takes the 0 to 7 bits to the next byte, counted from the start of the
    /// AudioSpecificConfig (Table 4.2, Note 1; 1.3.36). Where the elements before it end on a byte it takes
    /// none: read as 8 bits, it took the comment's length, and the config read past its end.
    /// </summary>
    [TestMethod]
    [DataRow("120005080020010f00", 2, 4, 2, 0, 0, 0, 0, 0, DisplayName = "al06_44: aligned already, 0 bits")]
    [DataRow("0880004400000000", 1, 1, 1, 0, 0, 0, 1, 0, DisplayName = "am00_88: AAC Main, 1 bit")]
    [DataRow("21800cc400002000", 4, 3, 1, 0, 0, 0, 1, 0, DisplayName = "ap05_48: AAC LTP, 1 bit")]
    [DataRow("1200050c05200109440003ac042f", 2, 4, 3, 0, 1, 1, 2, 3, DisplayName = "al22_chCfg0PCE_44: 2 bits, a comment of 3 bytes")]
    [DataRow("12000508000000f000", 2, 4, 2, 0, 0, 0, 4, 0, DisplayName = "al17_44: 4 bits")]
    [DataRow("12000504002001e000", 2, 4, 1, 0, 0, 0, 5, 0, DisplayName = "al04_44: 5 bits")]
    [DataRow("1000040805020108800000", 2, 0, 2, 0, 1, 1, 6, 0, DisplayName = "al07_96: 6 bits")]
    public void ReadsTheProgramConfigElementToItsEnd(string hex, int objectType, int samplingFrequencyIndex,
        int front, int side, int back, int lfe, int alignmentBits, int commentBytes)
    {
        byte[] bytes = Convert.FromHexString(hex);
        var (config, read) = Read(bytes);
        var pce = config._GASpecificConfig.ProgramConfigElement;

        Assert.AreEqual(objectType, config.AudioObjectType.AudioObjectType);
        Assert.AreEqual(0, config.ChannelConfiguration);
        Assert.AreEqual(samplingFrequencyIndex, config.SamplingFrequencyIndex);
        Assert.AreEqual(front, pce.NumFrontChannelElements);
        Assert.AreEqual(side, pce.NumSideChannelElements);
        Assert.AreEqual(back, pce.NumBackChannelElements);
        Assert.AreEqual(lfe, pce.NumLfeChannelElements);
        Assert.AreEqual(alignmentBits, pce.ByteAlignment.Bits);
        Assert.AreEqual(commentBytes, pce.CommentFieldBytes);

        // the PCE ends the config: read, sized and written back to its last bit
        Assert.AreEqual((ulong)bytes.Length * 8, read);
        Assert.AreEqual((ulong)bytes.Length * 8, config.CalculateSize());
        CollectionAssert.AreEqual(bytes, Write(config));
    }

    /// <summary>
    /// SBR and PS as each is signalled (1.6.2.1, Table 1.15; 1.6.6), an escaped object type (Table 1.14), and what
    /// follows a config's last field: read, sized and written back as they were. The values are those ffprobe
    /// gives the streams, but for PS where the config signals it as the backward compatible extension.
    /// </summary>
    [TestMethod]
    [DataRow("2b8a0800", 2, 5, 22050, 44100, 1, true, false, 7ul, DisplayName = "CT_DecoderCheck/File7.3gp: explicit SBR, audioObjectType 5 then 2")]
    [DataRow("eb8a0800", 2, 29, 22050, 44100, 1, true, true, 7ul, DisplayName = "CT_DecoderCheck/File5.mp4: explicit PS, audioObjectType 29 then 2")]
    [DataRow("138856e5a54880", 2, 0, 22050, 44100, 1, true, true, 7ul, DisplayName = "CT_DecoderCheck/File4.mp4: SBR and PS after 0x2b7 and 0x548")]
    [DataRow("140856e5ad4880", 2, 0, 16000, 32000, 1, true, true, 7ul, DisplayName = "tones_afconvert_16000_stereo_aac_he2: SBR and PS after 0x2b7 and 0x548")]
    [DataRow("f8e82000", 39, 0, 44100, 0, 1, false, false, 2ul, DisplayName = "er_eld1001np_44_ep0: ELD, escaped as 31 and 7")]
    [DataRow("f94643221cc05852002000a04046d0b800", 42, 0, 48000, 0, 2, false, false, 5ul, DisplayName = "usac/xhe_target_level: USAC, its UsacConfig 14 bytes after 19 bits, 5 after them")]
    [DataRow("1190000000", 2, 0, 48000, 0, 2, false, false, 13ul, DisplayName = "twofields_packet: zeros after the config")]
    public void ReadsAndWritesBackTheSignallingAndWhatFollows(string hex, int objectType, int signalledType, int samplingFrequency,
        int extensionSamplingFrequency, int channelConfiguration, bool sbr, bool ps, ulong remainingBits)
    {
        int[] rates = { 96000, 88200, 64000, 48000, 44100, 32000, 24000, 22050, 16000, 12000, 11025, 8000, 7350 };
        byte[] bytes = Convert.FromHexString(hex);
        var (config, read) = Read(bytes);

        Assert.AreEqual(objectType, config.AudioObjectType.AudioObjectType);
        Assert.AreEqual(signalledType, config.SignalledAudioObjectType?.AudioObjectType ?? 0);
        Assert.AreEqual(samplingFrequency, rates[config.SamplingFrequencyIndex]);
        if (sbr)
            Assert.AreEqual(extensionSamplingFrequency, rates[config.ExtensionSamplingFrequencyIndex]);
        Assert.AreEqual(channelConfiguration, config.ChannelConfiguration);
        Assert.AreEqual(sbr, config.SbrPresentFlag);
        Assert.AreEqual(ps, config.PsPresentFlag);
        Assert.AreEqual(remainingBits, config.Remainder.Count);

        Assert.AreEqual((ulong)bytes.Length * 8, read);
        Assert.AreEqual((ulong)bytes.Length * 8, config.CalculateSize());
        CollectionAssert.AreEqual(bytes, Write(config));
    }

    /// <summary>
    /// ALS's config (11.4.1, Table 11.1) after an escaped object type (36) and a sampling frequency of 24 bits that
    /// starts within a byte, ending in the 44 bytes of the original WAV header (als_07_2ch192k32bF).
    /// </summary>
    [TestMethod]
    public void ReadsAndWritesBackTheConfigOfAls()
    {
        byte[] bytes = Convert.FromHexString("f89e05dc0000414c53000002ee00002bdfca00012e1fff00700a10800000002c0000000a524946467efe5e0157415645666d7420100000000300020000ee020000701700080020006461746150fe5e0146616b65020000000000d5b3d840");
        var (config, read) = Read(bytes);
        var als = config._ALSSpecificConfig;

        Assert.AreEqual(36, config.AudioObjectType.AudioObjectType);
        Assert.AreEqual(192000u, config.SamplingFrequency);
        Assert.AreEqual(2, als.Channels + 1);
        Assert.IsTrue(als.Floating);
        Assert.AreEqual(44u, als.HeaderSize);
        Assert.AreEqual("RIFF", System.Text.Encoding.ASCII.GetString(als.OrigHeader, 0, 4));

        Assert.AreEqual((ulong)bytes.Length * 8, read);
        Assert.AreEqual((ulong)bytes.Length * 8, config.CalculateSize());
        CollectionAssert.AreEqual(bytes, Write(config));
    }

    /// <summary>
    /// ALS's channel rearrangement (11.4.1, Table 11.1), which none of the samples has: a config built by hand
    /// of 3 channels (channels = 2), chan_pos of ChBits = ceil[log2(3)] = 2 bits each for each of them, then the
    /// byte_align of 2 bits, no header (header_size 0xFFFFFFFF) and a trailer of 4 bytes.
    /// </summary>
    [TestMethod]
    public void ReadsAndWritesBackAlsChannelPositions()
    {
        byte[] bytes = Convert.FromHexString("f88600414c53000000bb80000003e800020407ff000014010084ffffffff0000000444415441");
        var (config, read) = Read(bytes);
        var als = config._ALSSpecificConfig;

        Assert.AreEqual(2, als.Channels);
        Assert.IsTrue(als.ChanSort);
        CollectionAssert.AreEqual(new byte[] { 2, 0, 1 }, als.ChanPos.Select(p => p[0]).ToArray());
        Assert.AreEqual(2, als.ByteAlignment.Bits);
        Assert.AreEqual(0xFFFFFFFFu, als.HeaderSize);
        Assert.AreEqual("DATA", System.Text.Encoding.ASCII.GetString(als.OrigTrailer));

        Assert.AreEqual((ulong)bytes.Length * 8, read);
        Assert.AreEqual((ulong)bytes.Length * 8, config.CalculateSize());
        CollectionAssert.AreEqual(bytes, Write(config));
    }

    /// <summary>A comment of 44 bytes after 6 bits of alignment (al15_44).</summary>
    [TestMethod]
    public void ReadsTheCommentAfterTheAlignment()
    {
        byte[] bytes = Convert.FromHexString("1200050c0102010884002c456e636f64656420627920446f6c6279204c61626f7261746f726965732c20446563656d6265722032303031");
        var (config, _) = Read(bytes);
        var pce = config._GASpecificConfig.ProgramConfigElement;

        Assert.AreEqual(6, pce.ByteAlignment.Bits);
        Assert.AreEqual("Encoded by Dolby Laboratories, December 2001", System.Text.Encoding.ASCII.GetString(pce.CommentFieldData));
        CollectionAssert.AreEqual(bytes, Write(config));
    }

    private static (AudioSpecificConfig Config, ulong Read) Read(byte[] bytes)
    {
        var config = new AudioSpecificConfig();
        using var stream = new IsoStream(new MemoryStream(bytes));
        ulong read = config.Read(stream, (ulong)bytes.Length * 8);
        return (config, read);
    }

    private static byte[] Write(AudioSpecificConfig config)
    {
        using var memory = new MemoryStream();
        using (var stream = new IsoStream(memory))
            config.Write(stream);
        return memory.ToArray();
    }
}
