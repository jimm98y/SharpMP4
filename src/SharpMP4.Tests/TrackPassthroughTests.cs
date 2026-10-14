using SharpISOBMFF;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against what goes to a file as it was read - <see cref="GenericTrack"/>'s sample entry, a subtitle track's -
/// and against the tracks <see cref="TrackFactory"/> falls back to.
/// </summary>
[TestClass]
public class TrackPassthroughTests
{
    // An 'stsd' of MPEG-H 3D Audio's 'mhm1' entry (ISO/IEC 23008-3 20.5), of a codec none reads: its 'mhaC' of version 1,
    // profile and level 0x0D, the reference channel layout 2 and a configuration of no bytes
    private const string MpegHStsd =
        "00000041" + "73747364" + "00000000" + "00000001" +
        "00000031" + "6d686d31" + "000000000000" + "0001" + "0000000000000000" + "0002" + "0010" + "0000" + "0000" + "bb800000" +
        "0000000d" + "6d686143" + "01" + "0d" + "02" + "0000";

    private static SampleDescriptionBox ReadStsd(string hex)
    {
        new IsoStream(new StreamWrapper(new MemoryStream(Hex(hex)))).ReadBox(0, null, out Box box, "");
        return (SampleDescriptionBox)box;
    }

    /// <summary>
    /// A track of a codec none reads writes its whole sample entry as it was, and a copy of it: it wrote the entry's first
    /// box - 'mhaC' - where the entry belongs, and handed the reader's own box to a writer to put in its tree.
    /// </summary>
    [TestMethod]
    public void WritesTheWholeSampleEntryOfACodecNoneReads()
    {
        var stsd = ReadStsd(MpegHStsd);
        Box entry = stsd.Children[0];
        Box mhaC = entry.Children[0];

        var logger = new CapturingLogger();
        ITrack track = TrackFactory.DefaultCreateTrack(1, mhaC, 48000, 1024, IsoStream.FromFourCC(HandlerTypes.Sound), "SoundHandler", logger);
        Assert.IsInstanceOfType<GenericTrack>(track);
        Assert.AreEqual(1, logger.Warnings.Count, "a codec none reads, said");

        foreach (var written in new[] { track.CreateSampleEntryBox(), track.Clone().CreateSampleEntryBox() })
        {
            Assert.AreEqual("mhm1", IsoStream.ToFourCC(written.FourCC));
            Assert.AreNotSame(entry, written);
            CollectionAssert.AreEqual(BytesOf(entry), BytesOf(written));

            // a writer puts it in its own tree: the reader's is as it was
            written.SetParent(new SampleDescriptionBox());
            Assert.AreSame(stsd, entry.GetParent());
            Assert.AreSame(entry, mhaC.GetParent());
        }
    }

    // An 'stsd' of the 'rtp ' entry of a hint track, as frag_bunny.mp4 has it: a box the library does not know, kept as its
    // bytes - hinttrackversion, highestcompatibleversion and maxpacketsize after the data reference index
    private const string RtpStsd =
        "00000020" + "73747364" + "00000000" + "00000001" +
        "00000010" + "72747020" + "000000000000" + "0001";

    /// <summary>
    /// The copy of an entry the library does not know is written as it was read, after the copy was made: it kept its bytes
    /// in the stream it was read back from, which was closed once the copy was made, so writing the init segment of a remux
    /// of frag_bunny.mp4 - with its hint tracks - threw ObjectDisposedException.
    /// </summary>
    [TestMethod]
    public void WritesTheCopyOfAnEntryItDoesNotKnow()
    {
        var stsd = ReadStsd(RtpStsd);
        Box entry = stsd.Children[0];

        ITrack track = TrackFactory.DefaultCreateTrack(1, entry, 90000, 0, IsoStream.FromFourCC("hint"), "HintHandler", new CapturingLogger());
        Assert.IsInstanceOfType<GenericTrack>(track);

        GC.Collect();
        using var clone = track.Clone();
        foreach (var written in new[] { track.CreateSampleEntryBox(), clone.CreateSampleEntryBox() })
            CollectionAssert.AreEqual(BytesOf(entry), BytesOf(written));
        track.Dispose();
    }

    /// <summary>
    /// A track that writes back the entry it was read with keeps a stream the entries it hands out are read from: disposed,
    /// it lets go of it and hands out no more, and a clone, which has a stream of its own, goes on.
    /// </summary>
    [TestMethod]
    public void LetsGoOfItsEntryAsItIsDisposedButNotOfItsClones()
    {
        Box entry = ReadStsd(RtpStsd).Children[0];
        ITrack track = TrackFactory.DefaultCreateTrack(1, entry, 90000, 0, IsoStream.FromFourCC("hint"), "HintHandler", new CapturingLogger());
        using var clone = track.Clone();

        track.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => track.CreateSampleEntryBox());
        CollectionAssert.AreEqual(BytesOf(entry), BytesOf(clone.CreateSampleEntryBox()));
    }

    /// <summary>
    /// A configuration its track cannot read - an SPS cut short - leaves the samples passed through, under the entry as
    /// it was, and says so: the track's exception was the reader's.
    /// </summary>
    [TestMethod]
    public void PassesThroughATrackOfAConfigurationItCannotRead()
    {
        var entry = new VisualSampleEntry(IsoStream.FromFourCC("avc1"))
        {
            Children = new List<Box>(),
            ReservedSampleEntry = new byte[6],
            PreDefined0 = new uint[3],
            DataReferenceIndex = 1,
            Width = 16,
            Height = 16,
            Horizresolution = 72 << 16,
            Vertresolution = 72 << 16,
            FrameCount = 1,
            Depth = 24,
            Compressorname = BinaryUTF8String.GetBytes(new string('\0', 32)),
        };
        var avcC = new AVCConfigurationBox
        {
            _AVCConfig = new AVCDecoderConfigurationRecord
            {
                ConfigurationVersion = 1,
                LengthSizeMinusOne = 3,
                NumOfSequenceParameterSets = 1,
                SequenceParameterSetNALUnit = new[] { Hex("6764") },
                SequenceParameterSetLength = new ushort[] { 2 },
                PictureParameterSetNALUnit = Array.Empty<byte[]>(),
                PictureParameterSetLength = Array.Empty<ushort>(),
            },
        };
        avcC.SetParent(entry);
        entry.Children.Add(avcC);

        var logger = new CapturingLogger();
        ITrack track = TrackFactory.DefaultCreateTrack(1, avcC, 30000, 1001, IsoStream.FromFourCC(HandlerTypes.Video), "VideoHandler", logger);

        Assert.IsInstanceOfType<GenericTrack>(track);
        Assert.AreEqual(1, logger.Warnings.Count(w => w.Contains("avcC")), string.Join("; ", logger.Warnings));
        Assert.AreEqual("avc1", IsoStream.ToFourCC(track.CreateSampleEntryBox().FourCC));
    }

    /// <summary>
    /// A subtitle track read from a file writes a copy of its sample entry: it handed the reader's own to a writer, which
    /// took it out of the reader's tree.
    /// </summary>
    [TestMethod]
    public void WritesACopyOfASubtitleSampleEntry()
    {
        var stsd = new SampleDescriptionBox { Children = new List<Box>() };
        Box entry = new TimedTextTrack().CreateSampleEntryBox();
        entry.SetParent(stsd);
        stsd.Children.Add(entry);

        var track = new TimedTextTrack(entry, 1000, HandlerTypes.Text);
        foreach (var written in new[] { track.CreateSampleEntryBox(), track.Clone().CreateSampleEntryBox() })
        {
            Assert.AreNotSame(entry, written);
            CollectionAssert.AreEqual(BytesOf(entry), BytesOf(written));
            written.SetParent(new SampleDescriptionBox());
            Assert.AreSame(stsd, entry.GetParent());
        }
    }

    /// <summary>
    /// A VP9 sample parsed and given back to a track is the sample again: a superframe was split in its frames and its
    /// index, which the track took each for a sample.
    /// </summary>
    [TestMethod]
    public void ParsesAVP9SampleIntoWhatMakesItAgain()
    {
        // two frames, of 3 and 4 bytes, and the superframe index after them (Annex B): a marker, the sizes, the marker
        byte[] superframe = Hex("820083" + "82008300" + "c10304c1");
        var reader = new VP9Track(30000, 1001);
        var writer = new VP9Track(30000, 1001) { Logger = new CapturingLogger() };

        var samples = new List<byte[]>();
        foreach (var unit in reader.ParseSample(superframe))
        {
            writer.ProcessSample(unit.ToArray(), out var output, out _);
            if (output.Array != null)
                samples.Add(output.ToArray());
        }

        Assert.AreEqual(1, samples.Count);
        CollectionAssert.AreEqual(superframe, samples[0]);
        Assert.AreEqual(3, VP9Track.ParseFrames(superframe, 0, superframe.Length).Count(), "its frames and its index");
    }

    /// <summary>A unit that does not start with a start code is said not to: the warning said it did.</summary>
    [TestMethod]
    public void SaysAUnitDoesNotStartWithAStartCode()
    {
        var h262 = new CapturingLogger();
        new H262Track(30000, 1001) { Logger = h262 }.ProcessSample(Hex("01020304"), out _, out _);
        var mpeg4 = new CapturingLogger();
        new MPEG4Track(30000, 1001) { Logger = mpeg4 }.ProcessSample(Hex("01020304"), out _, out _);

        Assert.IsTrue(h262.Warnings.Single().Contains("does not start with a start code"), h262.Warnings.Single());
        Assert.IsTrue(mpeg4.Warnings.Single().Contains("does not start with a start code"), mpeg4.Warnings.Single());
    }
}
