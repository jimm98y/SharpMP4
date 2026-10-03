using SharpISOBMFF;
using SharpMP4.Common;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="AACTrack"/>, <see cref="OpusTrack"/> and the audio tracks <see cref="TrackFactory"/> makes:
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
    /// MP3 in an 'esds' - of objectTypeIndication 0x6B or 0x69, or of MPEG-4 Audio's Layer 3 object type - is passed
    /// through: it was made an AAC track, which took an ADTS header off each MP3 frame, whose sync word it mistook for one.
    /// </summary>
    [TestMethod]
    public void PassesMp3Through()
    {
        byte[] frame = Hex("fffb9064000000000000000000000000");
        foreach (var esds in new[] { Esds(0x6B, null), Esds(0x69, null), Esds(0x40, Hex("f84400")) /* audioObjectType 34 */ })
        {
            var track = Create(esds, 44100, 1152);
            Assert.IsInstanceOfType<GenericTrack>(track);
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
}
