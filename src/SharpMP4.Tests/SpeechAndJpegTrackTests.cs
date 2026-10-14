using SharpISOBMFF;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// The tracks of 3GPP's and QuickTime's older codings: <see cref="AmrTrack"/> of 'samr' and 'sawb', <see cref="MjpegTrack"/>
/// of 'jpeg' and of MP4's 'mp4v' of JPEG, <see cref="PcmTrack"/> of G.711's 'alaw' and 'ulaw', and <see cref="Mp3Track"/>
/// of QuickTime's '.mp2' and of a frame's layer.
/// </summary>
[TestClass]
public class SpeechAndJpegTrackTests
{
    // a JPEG picture's start: SOI, then a baseline frame header (SOF0) of 1280x720, 8 bits, Y Cb Cr of the sampling given
    private static byte[] JpegHeader(byte lumaSampling) => Hex(
        "FFD8" + "FFC00011" + "08" + "02D0" + "0500" + "03" + "01" + lumaSampling.ToString("X2") + "00" + "021101" + "031101" + "FFD9");

    [TestMethod]
    public void ReadsAJpegFrameHeader()
    {
        var jpeg = JpegHeader(0x22);
        Assert.IsTrue(MjpegTrack.TryReadFrameHeader(jpeg, 0, jpeg.Length, out int width, out int height, out int chroma));
        Assert.AreEqual((1280, 720, 1), (width, height, chroma));

        jpeg = JpegHeader(0x21);
        Assert.IsTrue(MjpegTrack.TryReadFrameHeader(jpeg, 0, jpeg.Length, out _, out _, out chroma));
        Assert.AreEqual(2, chroma, "4:2:2");

        // of no start of image, no picture
        Assert.IsFalse(MjpegTrack.TryReadFrameHeader(jpeg, 2, jpeg.Length - 2, out _, out _, out _));
    }

    /// <summary>A track made anew writes QuickTime's 'jpeg', which reads back as Motion JPEG of the pictures' size; each picture a sync sample.</summary>
    [TestMethod]
    public void WritesAndReadsMotionJpeg()
    {
        var picture = JpegHeader(0x22);
        var (_, reader, trackID) = WriteAndRead(new MjpegTrack(30000, 1001), (builder, id) =>
        {
            builder.ProcessTrackSample(id, picture, 1001);
            builder.ProcessTrackSample(id, picture, 1001);
        });

        var track = (MjpegTrack)reader.Tracks[trackID].Track;
        Assert.AreEqual(("jpeg", 1280, 720), (track.Coding, track.Width, track.Height));
        var first = reader.ReadSample(trackID);
        CollectionAssert.AreEqual(picture, first.Data.ToArray());
        Assert.IsTrue(first.IsRandomAccessPoint);
        Assert.IsTrue(MjpegTrack.IsMjpeg("mjpa"));
        Assert.IsFalse(MjpegTrack.IsMjpeg("avc1"));
    }

    /// <summary>
    /// A track made anew writes a 'samr' of a 'damr' of its frames a sample, which reads back as AMR of 8 kHz, a sample of
    /// 20 ms a frame; a sample's frames are told apart by their frame types.
    /// </summary>
    [TestMethod]
    public void WritesAndReadsAmr()
    {
        // two frames of mode 7, 12.2 kbit/s: a byte of the frame type (7 << 3) and quality (4), then 31 bytes of speech
        var frame = new byte[32];
        frame[0] = (7 << 3) | 4;
        var sample = frame.Concat(frame).ToArray();

        var (_, reader, trackID) = WriteAndRead(new AmrTrack(framesPerSample: 2, modeSet: 0x80), (builder, id) =>
            builder.ProcessTrackSample(id, sample, 320));

        var track = (AmrTrack)reader.Tracks[trackID].Track;
        Assert.AreEqual((false, 8000u, (byte)2, (ushort)0x80, 160), (track.IsWideband, track.SampleRate, track.FramesPerSample, track.ModeSet, track.SamplesPerFrame));
        var read = reader.ReadSample(trackID).Data.ToArray();
        var frames = track.ParseFrames(read, 0, read.Length).ToList();
        Assert.AreEqual(2, frames.Count);
        Assert.AreEqual(32, frames[1].Count);

        // AMR-WB's mode 8, 23.85 kbit/s, of 60 bytes, and the frames of no data, of a byte alone
        Assert.AreEqual(61, AmrTrack.FrameLength(8 << 3, wideband: true));
        Assert.AreEqual(1, AmrTrack.FrameLength(15 << 3, wideband: false));
    }

    // An 'stsd' of FFmpeg's 'sawb' entry: of 1 channel at 16000 Hz, its 'damr' of vendor 'ffmp', decoder version 0, mode
    // set 0x81ff, mode change period 0, a frame a sample
    private const string AmrWbStsd =
        "00000045" + "73747364" + "00000000" + "00000001" +
        "00000035" + "73617762" + "000000000000" + "0001" + "0000000000000000" + "0001" + "0010" + "0000" + "0000" + "3e800000" +
        "00000011" + "64616d72" + "66666d70" + "00" + "81ff" + "00" + "01";

    /// <summary>An AMR-WB entry as a file has it: read of its 'damr', and written back, and of a clone, as it was.</summary>
    [TestMethod]
    public void ReadsAmrWidebandAndWritesItsEntryBack()
    {
        var stsd = new SampleDescriptionBox();
        new IsoStream(new StreamWrapper(new MemoryStream(Hex(AmrWbStsd)))).ReadBox(0, null, out Box box, "");
        Box entry = box.Children[0];
        var track = (AmrTrack)TrackFactory.DefaultCreateTrack(1, entry.Children[0], 16000, 320, IsoStream.FromFourCC(HandlerTypes.Sound), "SoundHandler", new CapturingLogger());

        Assert.AreEqual((true, 16000u, IsoStream.FromFourCC("ffmp"), (ushort)0x81FF, (byte)1, 320),
            (track.IsWideband, track.SampleRate, track.Vendor, track.ModeSet, track.FramesPerSample, track.DefaultSampleDuration));
        CollectionAssert.AreEqual(BytesOf(entry), BytesOf(track.CreateSampleEntryBox()));
        CollectionAssert.AreEqual(BytesOf(entry), BytesOf(track.Clone().CreateSampleEntryBox()));
    }

    /// <summary>G.711's QuickTime entries: of a byte a sample, companded, whatever the entry's sample size says.</summary>
    [TestMethod]
    public void ReadsG711AsCompandedPcm()
    {
        foreach (var (coding, companding) in new[] { ("alaw", PcmCompanding.ALaw), ("ulaw", PcmCompanding.MuLaw) })
        {
            var entry = new AudioSampleEntry(IsoStream.FromFourCC(coding))
            {
                Children = new List<Box>(),
                ReservedSampleEntry = new byte[6],
                DataReferenceIndex = 1,
                Channelcount = 2,
                Samplesize = 16,
                Samplerate = 8000u << 16,
            };
            Assert.IsTrue(PcmTrack.IsPcm(entry));
            var track = new PcmTrack(entry);
            Assert.AreEqual((companding, (ushort)8, 2, 8000u), (track.Companding, track.BitsPerSample, track.BytesPerFrame, track.SampleRate));
            Assert.AreEqual(companding, ((PcmTrack)track.Clone()).Companding);
        }
    }

    /// <summary>QuickTime's '.mp2' and '.mp1' are MPEG audio, as '.mp3' is; a frame's header says its layer.</summary>
    [TestMethod]
    public void ReadsTheLayerOfMpegAudio()
    {
        Assert.IsTrue(Mp3Track.IsQuickTimeMp3(".mp2"));
        Assert.IsTrue(Mp3Track.IsQuickTimeMp3(".mp1"));
        // MPEG-1, of 128 kbit/s at 48 kHz: Layer II (FFFD), Layer III (FFFB), Layer I (FFFF)
        Assert.AreEqual(2, Mp3Track.LayerOf(Hex("FFFD9400"), 0));
        Assert.AreEqual(3, Mp3Track.LayerOf(Hex("FFFB9400"), 0));
        Assert.AreEqual(1, Mp3Track.LayerOf(Hex("FFFF9400"), 0));
        Assert.AreEqual(0, Mp3Track.LayerOf(Hex("00009400"), 0));
    }
}
