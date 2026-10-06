using SharpISOBMFF;
using SharpMP4.Common;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="AACTrack"/>, <see cref="OpusTrack"/>, <see cref="Mp3Track"/>, <see cref="FlacTrack"/>,
/// <see cref="AlacTrack"/>, <see cref="AC3Track"/>, <see cref="EAC3Track"/> and the audio tracks <see cref="TrackFactory"/> makes:
/// a sample entry written back as it was read, and one made to be written as a reader reads it.
/// </summary>
[TestClass]
public class AudioTrackTests
{
    // HE-AAC, explicitly signalled (ISO/IEC 14496-3 1.6.2.1): audioObjectType 5, 24000 Hz, channelConfiguration 6 (5.1),
    // extensionSamplingFrequencyIndex 3 (48000 Hz), the core's audioObjectType 2, its GASpecificConfig
    private static readonly byte[] HeAacConfig = Hex("2b318800");

    /// <summary>
    /// An 'mp4a' sample entry with an 'esds' of the objectTypeIndication and decoder specific info given, as a reader
    /// reads it: its 'esds', which a reader hands a track, returned.
    /// </summary>
    private static ESDBox Esds(byte objectTypeIndication, byte[]? decoderSpecificInfo, ushort channels = 2, uint rate = 48000)
    {
        var entry = new AudioSampleEntryV1(IsoStream.FromFourCC("mp4a"))
        {
            Children = new List<Box>(),
            Channelcount = channels,
            Samplerate = rate << 16,
            Samplesize = 16,
            DataReferenceIndex = 1,
            ReservedSampleEntry = new byte[6],
            Reserved = new ushort[3],
        };
        var esds = new ESDBox();
        esds.SetParent(entry);
        entry.Children.Add(esds);
        var es = new ES_Descriptor { Children = new List<Descriptor>(), ESID = 1 };
        esds._ES = es;
        var decoderConfig = new DecoderConfigDescriptor { Children = new List<Descriptor>(), ObjectTypeIndication = objectTypeIndication, StreamType = 5 };
        decoderConfig.SetParent(es);
        es.Children.Add(decoderConfig);
        if (decoderSpecificInfo != null)
        {
            var info = new GenericDecoderSpecificInfo { Data = decoderSpecificInfo };
            info.SetParent(decoderConfig);
            decoderConfig.Children.Add(info);
        }
        var sl = new SLConfigDescriptor { Predefined = 2, Ocr = new byte[0], UseTimeStampsFlag = true };
        sl.SetParent(es);
        es.Children.Add(sl);
        return esds;
    }

    private static ITrack Create(Box config, uint timescale, int duration, IMp4Logger? logger = null) =>
        TrackFactory.DefaultCreateTrack(1, config, timescale, duration, IsoStream.FromFourCC(HandlerTypes.Sound), "SoundHandler", logger ?? new CapturingLogger());

    /// <summary>
    /// MP3 in an 'esds' - of objectTypeIndication 0x6B or 0x69, or of MPEG-4 Audio's Layer 3 object type - is of an MP3
    /// track, its frames passed as they are: it was made an AAC track, which took an ADTS header off each MP3 frame, whose
    /// sync word it mistook for one.
    /// </summary>
    [TestMethod]
    public void ReadsMp3()
    {
        byte[] frame = Hex("fffb9064000000000000000000000000");
        foreach (var esds in new[] { Esds(0x6B, null), Esds(0x69, null), Esds(0x40, Hex("f84400")) /* audioObjectType 34 */ })
        {
            var track = Create(esds, 44100, 1152);
            Assert.IsInstanceOfType<Mp3Track>(track);
            track.ProcessSample(frame, out var output, out _);
            CollectionAssert.AreEqual(frame, output.ToArray());
        }

        Assert.IsInstanceOfType<AACTrack>(Create(Esds(0x40, HeAacConfig), 48000, 2048));
        Assert.IsInstanceOfType<AACTrack>(Create(Esds(0x67, Hex("1190")), 48000, 1024));
    }

    /// <summary>
    /// A track read from a file writes its AudioSpecificConfig back as it was, and its channels: an HE-AAC 5.1 track was
    /// written an AAC-LC config, its channels 2.
    /// </summary>
    [TestMethod]
    public void WritesTheAudioSpecificConfigBack()
    {
        var read = (AACTrack)Create(Esds(0x40, HeAacConfig, channels: 6), 48000, 2048);
        Assert.AreEqual(6, read.ChannelCount);

        // a clone, as a remux writes
        var (_, reader, trackID) = WriteAndRead(read.Clone(), (builder, id) => builder.ProcessRawSample(id, new byte[16], 2048, true));
        var written = (AACTrack)reader.Tracks[trackID].Track;

        CollectionAssert.AreEqual(HeAacConfig, written.DecoderSpecificInfo);
        Assert.AreEqual(6, written.ChannelCount);
        Assert.AreEqual(6, written.AudioSpecificConfig.ChannelConfiguration);
        Assert.AreEqual(3, written.AudioSpecificConfig.ExtensionSamplingFrequencyIndex, "the SBR rate");
        Assert.AreEqual(2048, written.DefaultSampleDuration);
    }

    /// <summary>A rate the table has no index of is written after the escape index, 0xF: the track threw.</summary>
    [TestMethod]
    public void WritesARateOfNoIndex()
    {
        var (_, reader, trackID) = WriteAndRead(new AACTrack(2, 12345, 16), (builder, id) => builder.ProcessRawSample(id, new byte[16], 1024, true));
        var written = (AACTrack)reader.Tracks[trackID].Track;

        Assert.AreEqual(0xF, written.AudioSpecificConfig.SamplingFrequencyIndex);
        Assert.AreEqual(12345u, written.AudioSpecificConfig.SamplingFrequency);
        Assert.AreEqual(12345u, written.Timescale);
    }

    /// <summary>
    /// An Opus track's entry has the rate 48000 in 16.16, as a reader takes it, its media 48000 a second, and its 'dOps'
    /// reads back as it was written: the rate was written as an integer, read as 0; the timescale was 0; the output gain
    /// was lost; a table of channels was made for mapping family 0.
    /// </summary>
    [TestMethod]
    public void WritesOpusAsItIsRead()
    {
        var stereo = new OpusTrack(2, 312, -256, 0) { InputSampleRate = 44100 };
        Assert.AreEqual(48000u, stereo.Timescale);
        Assert.AreEqual(960, stereo.DefaultSampleDuration);
        Assert.IsNull(((Box)stereo.CreateSampleEntryBox()).Children.OfType<OpusSpecificBox>().Single()._ChannelMappingTable, "of family 0");

        var (container, reader, trackID) = WriteAndRead(stereo, (builder, id) =>
        {
            builder.ProcessTrackSample(id, new byte[] { 0xFC, 0xFF, 0xFE });
            builder.ProcessTrackSample(id, new byte[] { 0xFC, 0xFF, 0xFE });
        });

        Assert.AreEqual(48000u, Boxes.Find<MediaHeaderBox>(container).Timescale);
        uint samplerate = Boxes.Find<SampleDescriptionBox>(container).Children[0] switch
        {
            AudioSampleEntryV1 v1 => v1.Samplerate,
            AudioSampleEntry v0 => v0.Samplerate,
            var other => throw new InvalidOperationException($"no audio sample entry: {other.GetType().Name}"),
        };
        Assert.AreEqual(48000u, samplerate >> 16);
        var read = (OpusTrack)reader.Tracks[trackID].Track;
        Assert.AreEqual(48000u, read.SamplingRate);
        Assert.AreEqual((ushort)312, read.PreSkip);
        Assert.AreEqual((short)-256, read.OutputGain);
        Assert.AreEqual(44100u, read.InputSampleRate);
        Assert.AreEqual((byte)0, read.ChannelMappingFamily);

        var surround = new OpusTrack(6, 312, 0, 1, 4, 2, new byte[] { 0, 4, 1, 2, 3, 5 });
        (_, reader, trackID) = WriteAndRead(surround, (builder, id) => builder.ProcessTrackSample(id, new byte[] { 0xFC, 0xFF, 0xFE }));
        read = (OpusTrack)reader.Tracks[trackID].Track;
        Assert.AreEqual((byte)1, read.ChannelMappingFamily);
        Assert.AreEqual((byte)4, read.StreamCount);
        Assert.AreEqual((byte)2, read.CoupledCount);
        CollectionAssert.AreEqual(new byte[] { 0, 4, 1, 2, 3, 5 }, read.ChannelMapping);

        // and read as a file's, written again: the same
        var clone = (OpusTrack)read.Clone();
        Assert.AreEqual(read.Timescale, clone.Timescale);
        CollectionAssert.AreEqual(BytesOf(read.CreateSampleEntryBox()), BytesOf(clone.CreateSampleEntryBox()));
    }

    /// <summary>An audio sample entry of the coding given, with the box given in it: as a reader reads it.</summary>
    private static T InEntry<T>(string coding, T box, ushort channels = 2, uint rate = 44100, ushort sampleSize = 16) where T : Box
    {
        var entry = new AudioSampleEntry(IsoStream.FromFourCC(coding))
        {
            Children = new List<Box>(),
            Channelcount = channels,
            Samplerate = rate << 16,
            Samplesize = sampleSize,
            DataReferenceIndex = 1,
        };
        box.SetParent(entry);
        entry.Children.Add(box);
        return box;
    }

    /// <summary>
    /// QuickTime's '.mp3' entry has no configuration, only boxes such as 'chan': known by its coding, its channels and rate
    /// of the entry, and of MPEG-2 at a rate MPEG-1 has not, its frames then of 576 samples.
    /// </summary>
    [TestMethod]
    public void ReadsQuickTimeMp3()
    {
        var mpeg1 = Create(InEntry(".mp3", new AudioChannelLayoutBox()), 44100, 0);
        Assert.IsInstanceOfType<Mp3Track>(mpeg1);
        Assert.AreEqual((byte)2, ((Mp3Track)mpeg1).ChannelCount);
        Assert.AreEqual(Mp3Track.MPEG1_AUDIO_OBJECT_TYPE_INDICATION, ((Mp3Track)mpeg1).ObjectTypeIndication);
        Assert.AreEqual(1152, mpeg1.DefaultSampleDuration);

        var mpeg2 = (Mp3Track)Create(InEntry(".mp3", new AudioChannelLayoutBox(), channels: 1, rate: 24000), 24000, 0);
        Assert.AreEqual(Mp3Track.MPEG2_AUDIO_OBJECT_TYPE_INDICATION, mpeg2.ObjectTypeIndication);
        Assert.AreEqual(576, mpeg2.DefaultSampleDuration);
        Assert.AreEqual((byte)1, mpeg2.ChannelCount);
    }

    /// <summary>
    /// An MP3 track's 'mp4a' reads back as it was written: its object type, of MPEG-1 or MPEG-2 by its rate, its channels,
    /// rate and frame duration, and no decoder specific info.
    /// </summary>
    [TestMethod]
    public void WritesMp3AsItIsRead()
    {
        foreach (var (rate, oti, duration) in new[] { (44100u, (byte)0x6B, 1152), (22050u, (byte)0x69, 576) })
        {
            var (container, reader, trackID) = WriteAndRead(new Mp3Track(2, rate), (builder, id) => builder.ProcessRawSample(id, Hex("fffb9064000000000000000000000000"), duration, true));
            var read = (Mp3Track)reader.Tracks[trackID].Track;
            Assert.AreEqual(oti, read.ObjectTypeIndication);
            Assert.AreEqual(rate, read.SamplingRate);
            Assert.AreEqual(rate, read.Timescale);
            Assert.AreEqual((byte)2, read.ChannelCount);
            Assert.AreEqual(duration, read.DefaultSampleDuration);
            var decoderConfig = Boxes.Find<ESDBox>(container)._ES.Children.OfType<DecoderConfigDescriptor>().Single();
            Assert.IsFalse(decoderConfig.Children.Any(x => x.Tag == DescriptorTags.DecSpecificInfoTag), "no decoder specific info");

            var clone = (Mp3Track)read.Clone();
            CollectionAssert.AreEqual(BytesOf(read.CreateSampleEntryBox()), BytesOf(clone.CreateSampleEntryBox()));
        }
    }

    /// <summary>
    /// An MPEG audio frame's header tells its length, rate, channels and samples: MPEG-1 Layer III at 128 kbit/s and 44100
    /// Hz is of 417 bytes, 418 padded; MPEG-2 Layer III at 64 kbit/s and 24000 Hz of 192, mono; a run of frames, after
    /// bytes of none, is split into them.
    /// </summary>
    [TestMethod]
    public void ParsesMp3Frames()
    {
        Assert.IsTrue(Mp3Track.TryParseFrameHeader(Hex("fffb9064"), 0, out int length, out uint rate, out int channels, out int samples));
        Assert.AreEqual((417, 44100u, 2, 1152), (length, rate, channels, samples));
        Assert.IsTrue(Mp3Track.TryParseFrameHeader(Hex("fffb9264"), 0, out length, out _, out _, out _));
        Assert.AreEqual(418, length, "padded");
        Assert.IsTrue(Mp3Track.TryParseFrameHeader(Hex("fff384c4"), 0, out length, out rate, out channels, out samples));
        Assert.AreEqual((192, 24000u, 1, 576), (length, rate, channels, samples));
        Assert.IsFalse(Mp3Track.TryParseFrameHeader(Hex("fffb0064"), 0, out _, out _, out _, out _), "free bit rate");
        Assert.IsFalse(Mp3Track.TryParseFrameHeader(Hex("fffb9c64"), 0, out _, out _, out _, out _), "reserved rate");

        var run = new byte[3 + 417 + 418];
        Hex("fffb9064").CopyTo(run, 3);
        Hex("fffb9264").CopyTo(run, 3 + 417);
        var frames = new Mp3Track(2, 44100).ParseFrames(run, 0, run.Length).ToList();
        Assert.AreEqual(2, frames.Count);
        Assert.AreEqual((3, 417), (frames[0].Offset, frames[0].Count));
        Assert.AreEqual((420, 418), (frames[1].Offset, frames[1].Count));
    }

    // STREAMINFO of ffmpeg's FLAC encoder: blocks of 4608, 44100 Hz, 2 channels, 24 bits
    private const string StreamInfo = "1200120000073c0009fd0ac44370000204cc8eb71d5878a9e31c073962183688a32f";

    /// <summary>
    /// A FLAC track's 'dfLa' reads back as it was written: its blocks - STREAMINFO first, a VORBIS_COMMENT after, the last
    /// marked so - and its rate, channels and bits of STREAMINFO; above 65535 Hz the entry's rate is 0, and the stream's of
    /// STREAMINFO.
    /// </summary>
    [TestMethod]
    public void WritesFlacAsItIsRead()
    {
        var comment = (Type: (byte)4, Data: Hex("0400000074657374"));
        var track = new FlacTrack(new[] { (FlacTrack.STREAMINFO, Hex(StreamInfo)), comment });
        Assert.AreEqual(((byte)2, 44100u, (byte)24, (ushort)4608), (track.ChannelCount, track.SamplingRate, track.BitsPerSample, track.MaxBlockSize));

        var (container, reader, trackID) = WriteAndRead(track, (builder, id) => builder.ProcessRawSample(id, Hex("fff8c90c00"), 4608, true));
        var dfLa = Boxes.Find<FLACSpecificBox>(container);
        Assert.AreEqual(2, dfLa.Blocks.Length);
        Assert.IsFalse(dfLa.Blocks[0]._LastMetadataBlockFlag);
        Assert.IsTrue(dfLa.Blocks[1]._LastMetadataBlockFlag);
        var read = (FlacTrack)reader.Tracks[trackID].Track;
        CollectionAssert.AreEqual(Hex(StreamInfo), read.StreamInfo);
        CollectionAssert.AreEqual(comment.Data, read.MetadataBlocks[1].Data);
        Assert.AreEqual(((byte)2, 44100u, (byte)24, 4608, 44100u), (read.ChannelCount, read.SamplingRate, read.BitsPerSample, read.DefaultSampleDuration, read.Timescale));
        CollectionAssert.AreEqual(BytesOf(read.CreateSampleEntryBox()), BytesOf(read.Clone().CreateSampleEntryBox()));

        // 96000 Hz: 0x17700 in STREAMINFO's 20 bits
        byte[] hiRes = Hex(StreamInfo);
        hiRes[10] = 0x17;
        hiRes[11] = 0x70;
        hiRes[12] = (byte)(hiRes[12] & 0x0F);
        (container, reader, trackID) = WriteAndRead(new FlacTrack(hiRes), (builder, id) => builder.ProcessRawSample(id, Hex("fff8c90c00"), 4608, true));
        uint entryRate = Boxes.Find<SampleDescriptionBox>(container).Children[0] switch
        {
            AudioSampleEntryV1 v1 => v1.Samplerate,
            AudioSampleEntry v0 => v0.Samplerate,
            var other => throw new InvalidOperationException($"no audio sample entry: {other.GetType().Name}"),
        };
        Assert.AreEqual(0u, entryRate, "above 65535");
        read = (FlacTrack)reader.Tracks[trackID].Track;
        Assert.AreEqual(96000u, read.SamplingRate);
        Assert.AreEqual(96000u, read.Timescale);
    }

    /// <summary>A FLAC stream's header, 'fLaC' and its blocks, reads into the blocks it was made of.</summary>
    [TestMethod]
    public void ReadsAFlacStreamHeader()
    {
        var track = new FlacTrack(new[] { (FlacTrack.STREAMINFO, Hex(StreamInfo)), ((byte)1, new byte[8]) });
        byte[] header = track.CreateStreamHeader();
        Assert.AreEqual("fLaC", System.Text.Encoding.ASCII.GetString(header, 0, 4));
        Assert.AreEqual(0x00, header[4], "STREAMINFO, not the last");
        Assert.AreEqual(0x81, header[4 + 4 + 34], "PADDING, the last");
        var blocks = FlacTrack.ParseStreamHeader(header);
        Assert.AreEqual(2, blocks.Count);
        CollectionAssert.AreEqual(Hex(StreamInfo), blocks[0].Data);
        Assert.AreEqual((byte)1, blocks[1].Type);
    }

    // ALACSpecificConfig of ffmpeg's ALAC encoder: frames of 4096, 24 bits, 2 channels, 44100 Hz
    private const string AlacConfig = "00001000" + "00" + "18" + "28" + "0a" + "0e" + "02" + "0000" + "00006004" + "00204cc0" + "0000ac44";

    /// <summary>
    /// An ALAC track's 'alac' box reads back as it was written - its version and flags, then the config - and of
    /// QuickTime's entry the box in a 'wave' is read: its frame length, bits, channels and rate.
    /// </summary>
    [TestMethod]
    public void WritesAlacAsItIsRead()
    {
        var track = new AlacTrack(Hex(AlacConfig));
        Assert.AreEqual((4096u, (byte)24, (byte)2, 44100u), (track.FrameLength, track.BitDepth, track.ChannelCount, track.SamplingRate));

        var (container, reader, trackID) = WriteAndRead(track, (builder, id) => builder.ProcessRawSample(id, new byte[16], 4096, true));
        var box = Boxes.Find<SampleDescriptionBox>(container).Children[0].Children.OfType<CodecConfigurationBox>().Single();
        CollectionAssert.AreEqual(Hex("00000000" + AlacConfig), box.Data);
        var read = (AlacTrack)reader.Tracks[trackID].Track;
        CollectionAssert.AreEqual(Hex(AlacConfig), read.Config);
        Assert.AreEqual((4096, 44100u), (read.DefaultSampleDuration, read.Timescale));
        CollectionAssert.AreEqual(BytesOf(read.CreateSampleEntryBox()), BytesOf(read.Clone().CreateSampleEntryBox()));

        // QuickTime's: the box in a 'wave'
        var wave = new AppleWaveBox { Children = new List<Box>() };
        var alac = new CodecConfigurationBox(IsoStream.FromFourCC("alac")) { Data = Hex("00000000" + AlacConfig) };
        alac.SetParent(wave);
        wave.Children.Add(alac);
        var quickTime = Create(InEntry("alac", wave, sampleSize: 16), 44100, 4096);
        Assert.IsInstanceOfType<AlacTrack>(quickTime);
        CollectionAssert.AreEqual(Hex(AlacConfig), ((AlacTrack)quickTime).Config);
    }

    /// <summary>
    /// An AC-3 sync frame's syncinfo and bsi make a track of its parameters: 48000 Hz, 448 kbit/s, 3/2 with LFE - six
    /// channels - past the mix levels its acmod has; one of E-AC-3, of bsid 16, is none.
    /// </summary>
    [TestMethod]
    public void ReadsAnAC3SyncFrame()
    {
        // 0x0B77, crc1, fscod 0 and frmsizecod 30; bsid 8, bsmod 0; acmod 7, cmixlev, surmixlev, lfeon 1
        Assert.IsTrue(AC3Track.TryParseSyncFrame(Hex("0b7700001e40e100"), 0, out var track));
        Assert.AreEqual((48000u, (byte)6, (ushort)448, (byte)8), (track.SamplingRate, track.ChannelCount, track.BitRate, track.Bsid));
        Assert.IsFalse(AC3Track.TryParseSyncFrame(Hex("0b7700001e80e100"), 0, out _), "bsid 16, of E-AC-3");
        Assert.IsFalse(AC3Track.TryParseSyncFrame(Hex("0b7800001c40e100"), 0, out _), "no sync word");
    }

    /// <summary>An AC-3 track's 'dac3' reads back as it was written, and its rate and frames of 1536 samples.</summary>
    [TestMethod]
    public void WritesAC3AsItIsRead()
    {
        var (_, reader, trackID) = WriteAndRead(new AC3Track(1, 8, 0, 2, false, 10), (builder, id) => builder.ProcessRawSample(id, Hex("0b770000"), 1536, true));
        var read = (AC3Track)reader.Tracks[trackID].Track;
        Assert.AreEqual(((byte)1, (byte)8, (byte)0, (byte)2, false, (byte)10), (read.Fscod, read.Bsid, read.Bsmod, read.Acmod, read.Lfeon, read.BitRateCode));
        Assert.AreEqual((44100u, (byte)2, (ushort)192, 1536, 44100u), (read.SamplingRate, read.ChannelCount, read.BitRate, read.DefaultSampleDuration, read.Timescale));
        CollectionAssert.AreEqual(BytesOf(read.CreateSampleEntryBox()), BytesOf(read.Clone().CreateSampleEntryBox()));
    }

    /// <summary>
    /// An E-AC-3 track's 'dec3' reads back as it was written: its data rate, its substream - 3/2 with LFE and a dependent
    /// one adding Lc/Rc, eight channels - and the bytes after it, of Dolby Atmos' extension, as they were.
    /// </summary>
    [TestMethod]
    public void WritesEAC3AsItIsRead()
    {
        var substream = new EAC3Track.Substream { Fscod = 0, Bsid = 16, Bsmod = 0, Acmod = 7, Lfeon = true, NumDepSub = 1, ChanLoc = 0x100 };
        var track = new EAC3Track(768, new[] { substream }, extension: Hex("0110"));
        Assert.AreEqual((48000u, (byte)8), (track.SamplingRate, track.ChannelCount));

        var (_, reader, trackID) = WriteAndRead(track, (builder, id) => builder.ProcessRawSample(id, Hex("0b770000"), 1536, true));
        var read = (EAC3Track)reader.Tracks[trackID].Track;
        Assert.AreEqual((ushort)768, read.DataRate);
        Assert.AreEqual(1, read.Substreams.Count);
        var s = read.Substreams[0];
        Assert.AreEqual(((byte)0, (byte)16, (byte)7, true, (byte)1, (ushort)0x100), (s.Fscod, s.Bsid, s.Acmod, s.Lfeon, s.NumDepSub, s.ChanLoc));
        CollectionAssert.AreEqual(Hex("0110"), read.Extension);
        Assert.AreEqual((byte)8, read.ChannelCount);
        CollectionAssert.AreEqual(BytesOf(read.CreateSampleEntryBox()), BytesOf(read.Clone().CreateSampleEntryBox()));
    }
}
