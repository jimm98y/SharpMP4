using SharpISOBMFF;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// <see cref="PcmTrack"/>: QuickTime's 'lpcm' written as Apple's devices write it, the format of each coding of QuickTime
/// and ISO read, a block of frames written as a sample of each, and <see cref="Readers.VideoReader.ReadSamples"/> reading
/// them back in runs.
/// </summary>
[TestClass]
public class PcmTrackTests
{
    // An iPhone's 'lpcm' (IMG_8956.MOV): version 2, 48000 Hz, 2 channels of 16 bits, signed, packed, little-endian
    private const string IPhoneLpcm =
        "000000486c70636d000000000000000100020000000000000003" + "0010fffe00000001000000000048" + "40e7700000000000" + "00000002" +
        "7f000000000000100000000c0000000400000001";

    [TestMethod]
    public void WritesLpcmAsAnIPhone()
    {
        var track = new PcmTrack(48000, 2, 16);
        CollectionAssert.AreEqual(Hex(IPhoneLpcm), BytesOf(track.CreateSampleEntryBox()));
    }

    [TestMethod]
    public void ReadsLpcmAsAnIPhoneWritesIt()
    {
        var entry = (AudioSampleEntry)ReadEntry(Hex(IPhoneLpcm));
        Assert.IsTrue(PcmTrack.IsPcm(entry));

        var track = new PcmTrack(entry, 48000, 1);
        Assert.AreEqual(("lpcm", 48000u, (ushort)2, (ushort)16, false, true, true, 4),
            (track.Coding, track.SampleRate, track.ChannelCount, track.BitsPerSample, track.IsFloat, track.IsSigned, track.IsLittleEndian, track.BytesPerFrame));
        CollectionAssert.AreEqual(Hex(IPhoneLpcm), BytesOf(track.CreateSampleEntryBox()));
    }

    /// <summary>
    /// The codings of version 0 and ISO: of their own sizes and order, an 'enda' in the 'wave' making 'in24' little-endian,
    /// a 'pcmC' saying ISO's.
    /// </summary>
    [TestMethod]
    public void ReadsTheFormatOfEachCoding()
    {
        Assert.AreEqual((16, true, true), Format(Entry("sowt", 16)));
        Assert.AreEqual((16, true, false), Format(Entry("twos", 16)));
        Assert.AreEqual((8, true, false), Format(Entry("twos", 8)));
        Assert.AreEqual((8, false, false), Format(Entry("raw ", 8)));
        Assert.AreEqual((24, true, false), Format(Entry("in24", 16)));
        Assert.AreEqual((24, true, true), Format(Entry("in24", 16, new AppleWaveBox { Children = new List<Box> { new AppleEndiannessBox { LittleEndian = 1 } } })));
        Assert.AreEqual((24, true, true), Format(Entry("ipcm", 16, new PcmCBox { FormatFlags = 1, PcmSampleSize = 24 })));
        Assert.AreEqual((32, true, false), Format(Entry("ipcm", 16, new PcmCBox { FormatFlags = 0, PcmSampleSize = 32 })));

        var fl64 = new PcmTrack(Entry("fl64", 16));
        Assert.AreEqual((true, 64, 16), (fl64.IsFloat, (int)fl64.BitsPerSample, fl64.BytesPerFrame));
        Assert.IsFalse(PcmTrack.IsPcm(Entry("mp4a", 16)));
    }

    /// <summary>
    /// Blocks of frames written as a sample of each, of the block's duration shared out, their sizes one in the 'stsz';
    /// read back a sample at a time, or in runs as long as asked for.
    /// </summary>
    [TestMethod]
    public void WritesFramesOfABlockAndReadsThemInRuns()
    {
        var data = new byte[3 * 1000 * 4];
        new Random(1).NextBytes(data);

        var (container, reader, trackID) = WriteAndRead(new PcmTrack(44100, 2, 16), (builder, id) =>
        {
            for (int i = 0; i < 3; i++)
                builder.ProcessTrackSample(id, new ArraySegment<byte>(data, i * 4000, 4000), 1000);
        });

        var stsz = container.Children.OfType<MovieBox>().Single().Children.OfType<TrackBox>().Single()
            .Children.OfType<MediaBox>().Single().Children.OfType<MediaInformationBox>().Single()
            .Children.OfType<SampleTableBox>().Single().Children.OfType<SampleSizeBox>().Single();
        Assert.AreEqual((4u, 3000u), (stsz.SampleSize, stsz.SampleCount));

        var track = (PcmTrack)reader.Tracks[trackID].Track;
        Assert.AreEqual((44100u, (ushort)2, (ushort)16, true), (track.SampleRate, track.ChannelCount, track.BitsPerSample, track.IsLittleEndian));

        var first = reader.ReadSample(trackID);
        Assert.AreEqual((4, 1, 0L), (first.Data.Count, first.Duration, first.DTS));

        var read = new List<byte>(first.Data);
        long dts = 1;
        for (var run = reader.ReadSamples(trackID, 1200); run != null; run = reader.ReadSamples(trackID, 1200))
        {
            Assert.AreEqual(dts, run.DTS);
            Assert.AreEqual(run.Data.Count / 4, run.Duration);
            Assert.IsTrue(run.Data.Count <= 1200 * 4);
            dts += run.Duration;
            read.AddRange(run.Data);
        }
        Assert.AreEqual(3000L, dts);
        CollectionAssert.AreEqual(data, read.ToArray());
    }

    private static (int Bits, bool Signed, bool LittleEndian) Format(AudioSampleEntry entry)
    {
        var track = new PcmTrack(entry);
        Assert.AreEqual(44100u, track.SampleRate);
        return (track.BitsPerSample, track.IsSigned, track.IsLittleEndian);
    }

    private static AudioSampleEntry Entry(string coding, ushort sampleSize, params Box[] children)
    {
        var entry = new AudioSampleEntry(IsoStream.FromFourCC(coding))
        {
            Children = new List<Box>(),
            ReservedSampleEntry = new byte[6],
            DataReferenceIndex = 1,
            Channelcount = 2,
            Samplesize = sampleSize,
            Samplerate = 44100u << 16,
        };
        foreach (var child in children)
        {
            child.SetParent(entry);
            entry.Children.Add(child);
        }
        return entry;
    }

    // a sample entry of its bytes, as an 'stsd' reads it
    private static Box ReadEntry(byte[] bytes)
    {
        var stream = new IsoStream(new StreamWrapper(new MemoryStream(bytes)));
        stream.ReadBox(0, new SampleDescriptionBox(), out Box box, "");
        return box;
    }
}
