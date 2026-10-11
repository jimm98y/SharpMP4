using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// The boxes of a QuickTime movie, where QuickTime's differ from those of the ISO base media file format: the media and
/// data handlers' components, a data reference of an alias, the base media header of text, and of AAC and ALAC a version
/// 1 sound description with its codec's box in a 'wave'. Written by either builder of <see cref="Mp4FileFormat.QuickTime"/>;
/// an MP4's are as they were.
/// </summary>
[TestClass]
public class QuickTimeBoxesTests
{
    private static readonly byte[] KeyId = Hex("00112233445566778899aabbccddeeff");
    private static readonly byte[] Key = Hex("0f1e2d3c4b5a69788796a5b4c3d2e1f0");

    // the parameter sets of Chromium's bear-640x360-v_frag.mp4
    private static H264Track Video()
    {
        var video = new H264Track();
        video.ProcessSample(Hex("6764001eacd940a02ff9701100000303e90000ea600f162d96"), out _, out _);
        video.ProcessSample(Hex("68ebe3cb22c0"), out _, out _);
        return video;
    }

    private const string AlacConfig = "00001000" + "00" + "18" + "28" + "0a" + "0e" + "02" + "0000" + "00006004" + "00204cc0" + "0000ac44";

    /// <summary>A file of the tracks given, a sample each, written by the builder made, and read back.</summary>
    private static (Container Container, VideoReader Reader) Write(Func<Stream, IMp4Builder> create, params ITrack[] tracks)
    {
        using var output = new MemoryStream();
        var builder = create(output);
        foreach (var track in tracks)
        {
            builder.AddTrack(track);
            byte[] sample = track is TimedTextTrack text ? text.CreateSample(Array.Empty<SubtitleCue>()) : new byte[16];
            builder.ProcessRawSample(track.TrackID, sample, track.DefaultSampleDuration > 0 ? track.DefaultSampleDuration : 1000, true);
        }
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var reader = new VideoReader();
        reader.Parse(container);
        return (container, reader);
    }

    private static (Container Container, VideoReader Reader) WriteMov(params ITrack[] tracks) =>
        Write(output => new Mp4Builder(new SingleStreamOutput(output)) { FileFormat = Mp4FileFormat.QuickTime }, tracks);

    private static (Container Container, VideoReader Reader) WriteMp4(params ITrack[] tracks) =>
        Write(output => new Mp4Builder(new SingleStreamOutput(output)), tracks);

    private static TrackBox Trak(Container container, string handlerType) =>
        Boxes.FindAll<TrackBox>(container).Single(trak =>
            IsoStream.ToFourCC(trak.Children.OfType<MediaBox>().Single().Children.OfType<HandlerBox>().Single().HandlerType) == handlerType);

    private static MediaInformationBox Minf(TrackBox trak) =>
        trak.Children.OfType<MediaBox>().Single().Children.OfType<MediaInformationBox>().Single();

    private static Box Entry(TrackBox trak) =>
        Minf(trak).Children.OfType<SampleTableBox>().Single().Children.OfType<SampleDescriptionBox>().Single().Children.Single();

    private static string Text(BinaryUTF8String name) => System.Text.Encoding.UTF8.GetString(name.Bytes);

    [TestMethod]
    public void WritesAQuickTimeFtyp()
    {
        var (container, _) = WriteMov(Video());

        var ftyp = container.Children.OfType<FileTypeBox>().Single();
        Assert.AreEqual("qt  ", IsoStream.ToFourCC(ftyp.MajorBrand));
    }

    /// <summary>
    /// The media's handler is the component 'mhlr', its name counted, and 'minf' has the data handler 'dhlr' of an alias,
    /// before the 'dinf' whose data reference is that alias, of the media in this file - as Apple and ffmpeg write them.
    /// </summary>
    [TestMethod]
    public void WritesQuickTimesHandlersAndAnAlias()
    {
        var (container, _) = WriteMov(Video());
        var trak = Trak(container, HandlerTypes.Video);

        var hdlr = trak.Children.OfType<MediaBox>().Single().Children.OfType<HandlerBox>().Single();
        Assert.AreEqual("mhlr", IsoStream.ToFourCC(hdlr.PreDefined));
        string name = Text(hdlr.Name);
        Assert.AreEqual(name.Length - 1, (int)name[0], "counted: its length, then its text");

        var minf = Minf(trak);
        var dhlr = minf.Children.OfType<HandlerBox>().Single();
        Assert.AreEqual("dhlr", IsoStream.ToFourCC(dhlr.PreDefined));
        Assert.AreEqual("alis", IsoStream.ToFourCC(dhlr.HandlerType));
        Assert.IsTrue(minf.Children.IndexOf(dhlr) < minf.Children.IndexOf(minf.Children.OfType<DataInformationBox>().Single()));

        var dref = Boxes.FindAll<DataReferenceBox>(container).First();
        var alias = dref.Children.OfType<AppleDataReferenceAliasBox>().Single();
        Assert.AreEqual(1u, alias.Flags, "the media is in this file");
        Assert.IsFalse(dref.Children.OfType<DataEntryUrlBox>().Any());
    }

    /// <summary>
    /// QuickTime's text - of 3GPP's 'text' handler and Apple's 'sbtl' - has the base media header, a 'gmin' and a 'text'
    /// of the identity matrix, without which Apple's players do not show it: it had a null media header.
    /// </summary>
    [TestMethod]
    [DataRow(HandlerTypes.Text)]
    [DataRow(HandlerTypes.AppleSubtitle)]
    public void WritesTheBaseMediaHeaderOfText(string handlerType)
    {
        var (container, reader) = WriteMov(new TimedTextTrack(1000, handlerType));
        var minf = Minf(Trak(container, handlerType));

        Assert.IsFalse(minf.Children.OfType<NullMediaHeaderBox>().Any());
        var gmhd = minf.Children.OfType<GenericMediaHeaderAtom>().Single();
        Assert.AreEqual((ushort)0x40, gmhd.Children.OfType<BaseMediaInfoAtom>().Single().GraphicsMode);
        CollectionAssert.AreEqual(
            Hex("00010000" + "00000000" + "00000000" + "00000000" + "00010000" + "00000000" + "00000000" + "00000000" + "40000000"),
            gmhd.Children.OfType<TextGmhdMediaBox>().Single().TextData);

        Assert.IsInstanceOfType<TimedTextTrack>(reader.Tracks.Values.Single().Track);
    }

    /// <summary>
    /// AAC as QuickTime has it: a version 1 sound description, of variable bit rate, its 'esds' in a 'wave' after the
    /// 'frma' and 'mp4a' that say the coding, then a terminator; read back as the track it was.
    /// </summary>
    [TestMethod]
    public void WritesAacInAWave()
    {
        var (container, reader) = WriteMov(new AACTrack(2, 44100, 16));

        var entry = (AudioSampleEntry)Entry(Trak(container, HandlerTypes.Sound));
        Assert.AreEqual("mp4a", IsoStream.ToFourCC(entry.FourCC));
        Assert.AreEqual((ushort)1, entry.Soundversion);
        Assert.AreEqual((ushort)0xFFFE, entry.PreDefined, "the compression ID of variable bit rate, -2");
        Assert.AreEqual(1024u, entry.SamplesPerPacket);
        Assert.AreEqual((ushort)2, entry.Channelcount);
        Assert.AreEqual(44100u << 16, entry.Samplerate);

        var wave = entry.Children.OfType<AppleWaveBox>().Single();
        CollectionAssert.AreEqual(new[] { "frma", "mp4a", "esds", "\0\0\0\0" }, wave.Children.Select(x => IsoStream.ToFourCC(x.FourCC)).ToArray());
        Assert.AreEqual("mp4a", IsoStream.ToFourCC(wave.Children.OfType<OriginalFormatBox>().Single().DataFormat));
        Assert.IsFalse(entry.Children.OfType<ESDBox>().Any(), "the 'esds' is in the 'wave' alone");

        var read = (AACTrack)reader.Tracks.Values.Single().Track;
        Assert.AreEqual(2, read.ChannelCount);
        Assert.AreEqual(44100u, read.SamplingRate);
    }

    /// <summary>ALAC as QuickTime has it: its 'alac' box in a 'wave', after the 'frma'; read back as the track it was.</summary>
    [TestMethod]
    public void WritesAlacInAWave()
    {
        var (container, reader) = WriteMov(new AlacTrack(Hex(AlacConfig)));

        var entry = (AudioSampleEntry)Entry(Trak(container, HandlerTypes.Sound));
        Assert.AreEqual("alac", IsoStream.ToFourCC(entry.FourCC));
        Assert.AreEqual((ushort)1, entry.Soundversion);
        Assert.AreEqual(4096u, entry.SamplesPerPacket);

        var wave = entry.Children.OfType<AppleWaveBox>().Single();
        CollectionAssert.AreEqual(new[] { "frma", "alac", "\0\0\0\0" }, wave.Children.Select(x => IsoStream.ToFourCC(x.FourCC)).ToArray());

        var read = (AlacTrack)reader.Tracks.Values.Single().Track;
        CollectionAssert.AreEqual(Hex(AlacConfig), read.Config);
    }

    /// <summary>A fragmented file of the QuickTime format has QuickTime's boxes in its initialization segment too.</summary>
    [TestMethod]
    public void WritesQuickTimesBoxesOfAFragmentedFile()
    {
        var (container, _) = Write(output => new FragmentedMp4Builder(new SingleStreamOutput(output), 1000)
        {
            FileFormat = Mp4FileFormat.QuickTime,
            TemporaryStorageFactory = new TemporaryMemoryStorageFactory(),
        }, new AACTrack(2, 44100, 16));

        Assert.AreEqual((ushort)1, ((AudioSampleEntry)Entry(Trak(container, HandlerTypes.Sound))).Soundversion);
        Assert.IsTrue(Boxes.FindAll<AppleDataReferenceAliasBox>(container).Any());
    }

    /// <summary>
    /// A protected track of AAC as ffmpeg writes CENC in a movie: QuickTime's version 1 entry renamed 'enca', its 'wave'
    /// as of the clear track - its 'frma' still 'mp4a' - and the 'sinf' after it; read back, and decrypted, as it was.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void WritesAProtectedTrackOfQuickTimesEntry(bool fragmented)
    {
        var clear = Enumerable.Range(0, 4).Select(i =>
        {
            var bytes = new byte[64 + 16 * i];
            new Random(i).NextBytes(bytes);
            return bytes;
        }).ToArray();

        using var output = new MemoryStream();
        IMp4Builder builder = fragmented
            ? new FragmentedMp4Builder(new SingleStreamOutput(output), 1000) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() }
            : new Mp4Builder(new SingleStreamOutput(output));
        builder.FileFormat = Mp4FileFormat.QuickTime;
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track, Encryption.TrackProtection.Create(Encryption.ProtectionSchemes.Cenc, KeyId, isVideo: false), Key);
        foreach (var sample in clear)
            builder.ProcessRawSample(track.TrackID, sample, 1024, true);
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));

        var entry = (AudioSampleEntry)Entry(Trak(container, HandlerTypes.Sound));
        Assert.AreEqual("enca", IsoStream.ToFourCC(entry.FourCC));
        Assert.AreEqual((ushort)1, entry.Soundversion);
        CollectionAssert.AreEqual(new[] { "wave", "sinf" }, entry.Children.Select(x => IsoStream.ToFourCC(x.FourCC)).ToArray());
        var wave = entry.Children.OfType<AppleWaveBox>().Single();
        CollectionAssert.AreEqual(new[] { "frma", "mp4a", "esds", "\0\0\0\0" }, wave.Children.Select(x => IsoStream.ToFourCC(x.FourCC)).ToArray());
        Assert.AreEqual("mp4a", IsoStream.ToFourCC(wave.Children.OfType<OriginalFormatBox>().Single().DataFormat));
        var sinf = entry.Children.OfType<ProtectionSchemeInfoBox>().Single();
        Assert.AreEqual("mp4a", IsoStream.ToFourCC(sinf.Children.OfType<OriginalFormatBox>().Single().DataFormat));

        var reader = new VideoReader { KeyProvider = _ => Key };
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        Assert.IsInstanceOfType<AACTrack>(reader.Tracks[trackID].Track);
        for (int i = 0; i < clear.Length; i++)
        {
            var sample = reader.ReadSample(trackID)!;
            Assert.IsNull(sample.Encryption, $"sample {i} left protected");
            CollectionAssert.AreEqual(clear[i], sample.Data.ToArray(), $"sample {i}");
        }
    }

    /// <summary>An MP4's boxes are the ISO base media file format's, as they were.</summary>
    [TestMethod]
    public void WritesAnMp4AsItWas()
    {
        var (container, _) = WriteMp4(Video(), new AACTrack(2, 44100, 16), new TimedTextTrack());

        foreach (var hdlr in Boxes.FindAll<HandlerBox>(container))
        {
            Assert.AreEqual(0u, hdlr.PreDefined);
            Assert.AreEqual((byte)0, hdlr.Name.Bytes[^1], "zero terminated");
        }
        Assert.IsFalse(Boxes.FindAll<AppleDataReferenceAliasBox>(container).Any());
        Assert.AreEqual(3, Boxes.FindAll<DataEntryUrlBox>(container).Count);
        Assert.IsTrue(Minf(Trak(container, HandlerTypes.Text)).Children.OfType<NullMediaHeaderBox>().Any());
        Assert.IsFalse(Boxes.FindAll<GenericMediaHeaderAtom>(container).Any());

        var entry = (AudioSampleEntry)Entry(Trak(container, HandlerTypes.Sound));
        Assert.AreEqual((ushort)0, entry.Soundversion);
        Assert.IsTrue(entry.Children.OfType<ESDBox>().Any());
        Assert.IsFalse(entry.Children.OfType<AppleWaveBox>().Any());
    }

    /// <summary>
    /// Of offsets written negative - as Apple writes them, or in version 1 - the 'cslg' says how far they run and the
    /// composition times they make, as Apple writes it with them; of offsets shifted positive there is none.
    /// </summary>
    [TestMethod]
    [DataRow(NegativeCompositionOffsets.AsApple, true)]
    [DataRow(NegativeCompositionOffsets.Version1, true)]
    [DataRow(NegativeCompositionOffsets.Shifted, false)]
    public void WritesTheCompositionShiftOfNegativeOffsets(NegativeCompositionOffsets negative, bool written)
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output)) { FileFormat = Mp4FileFormat.QuickTime, NegativeCompositionOffsets = negative };
        var video = Video();
        builder.AddTrack(video);

        // I P B B in decode order, shown I B B P: decoded at 0, 1000, 2000, 3000, shown at 0, 3000, 1000, 2000
        int[] offsets = { 0, 2000, -1000, -1000 };
        for (int i = 0; i < offsets.Length; i++)
            builder.ProcessRawSample(video.TrackID, new byte[16], 1000, i == 0, offsets[i]);
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var cslg = Boxes.FindAll<CompositionToDecodeBox>(container).SingleOrDefault();

        if (!written)
        {
            Assert.IsNull(cslg);
            return;
        }

        Assert.IsNotNull(cslg);
        Assert.AreEqual(1000L, cslg.CompositionToDTSShift);
        Assert.AreEqual(-1000L, cslg.LeastDecodeToDisplayDelta);
        Assert.AreEqual(2000L, cslg.GreatestDecodeToDisplayDelta);
        Assert.AreEqual(0L, cslg.CompositionStartTime);
        Assert.AreEqual(4000L, cslg.CompositionEndTime);
    }
}
