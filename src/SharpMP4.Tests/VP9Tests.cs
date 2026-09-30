using SharpAVX;
using SharpISOBMFF;
using SharpMP4.Readers;
using SharpMP4.Tests.Conformance;
using SharpMP4.Tracks;
using SharpMP4.Common;
using SharpVP9;

namespace SharpMP4.Tests;

/// <summary>Tests against VP9's descriptors in AomStream - s(n), and the Boolean coder of B(p) and L(n) - and SharpVP9.</summary>
[TestClass]
public class VP9Tests
{
    /// <summary>
    /// What the Boolean encoder writes (libvpx's vpx_writer) the decoder of 9.2 reads back: bools of every probability,
    /// long runs of the likely and the unlikely value - which carry into the bytes written - and literals.
    /// </summary>
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void DecodesWhatTheBooleanEncoderWrites(int seed)
    {
        var random = new Random(seed);
        var coded = new List<(bool Literal, int Probability, int Value)>();
        for (int i = 0; i < 5000; i++)
        {
            if (random.Next(10) == 0)
            {
                int bits = random.Next(1, 9);
                coded.Add((true, bits, random.Next(1 << bits)));
            }
            else
            {
                int probability = random.Next(1, 256);
                // mostly as likely as the probability says, now and then a run against it
                bool against = random.Next(50) == 0;
                int value = random.Next(256) >= probability ? 1 : 0;
                coded.Add((false, probability, against ? 1 - value : value));
            }
        }

        const int size = 16384;
        using var written = new MemoryStream();
        using (var writer = new AomStream(written, NullMp4Logger.Instance))
        {
            writer.StartBool(size);
            foreach (var (literal, probability, value) in coded)
            {
                if (literal)
                    writer.WriteLiteral(probability, value, "literal");
                else
                    writer.WriteBool(probability, value, "bool");
            }
            writer.StopBool();
        }

        byte[] bytes = written.ToArray();
        Assert.AreEqual(size, bytes.Length, "padded to the size it was started for");

        using var reader = new AomStream(new MemoryStream(bytes), NullMp4Logger.Instance) { Validate = (name, value) => { if (name is "marker" or "padding") Assert.AreEqual(0, value, name); } };
        reader.InitBool(size);
        for (int i = 0; i < coded.Count; i++)
        {
            var (literal, probability, value) = coded[i];
            int read;
            if (literal)
                reader.ReadLiteral(probability, out read, "literal");
            else
                reader.ReadBool(probability, out read, "bool");
            Assert.AreEqual(value, read, $"the {i}th");
        }
        reader.ExitBool();
        Assert.AreEqual(size * 8, reader.GetPosition(), "exit_bool reads to the end of the data");
    }

    /// <summary>
    /// The encoder ends its data as libvpx does: 32 bools of 0, then a 0 byte where the last would be taken for a
    /// superframe marker (0b110xxxxx), which B.4 requires no frame end with.
    /// </summary>
    [TestMethod]
    public void EndsTheBooleanCodedDataAsLibvpxDoes()
    {
        for (int value = 0; value < 256; value++)
        {
            using var written = new MemoryStream();
            using (var writer = new AomStream(written, NullMp4Logger.Instance))
            {
                writer.StartBool(64);
                writer.WriteLiteral(8, value, "value");
                writer.StopBool();
            }

            byte[] bytes = written.ToArray();
            int end = Array.FindLastIndex(bytes, b => b != 0);
            Assert.IsTrue(end < 0 || (bytes[end] & 0xe0) != 0xc0 || end + 1 < bytes.Length, $"{value}: the data ends in a superframe marker");

            using var reader = new AomStream(new MemoryStream(bytes), NullMp4Logger.Instance);
            reader.InitBool(64);
            reader.ReadLiteral(8, out int read, "value");
            reader.ExitBool();
            Assert.AreEqual(value, read);
        }
    }

    /// <summary>s(n) (4.9.2): a magnitude, then its sign - and a -0, which is 0, written again as it was coded.</summary>
    [TestMethod]
    [DataRow(0b0101_0, 5)]
    [DataRow(0b0101_1, -5)]
    [DataRow(0b0000_0, 0)]
    [DataRow(0b0000_1, 0)]
    public void ReadsAndWritesSignAndMagnitude(int coded, int expected)
    {
        byte[] bytes = [(byte)(coded << 3)];
        var record = new AomSyntaxRecord();
        using (var reader = new AomStream(new MemoryStream(bytes), NullMp4Logger.Instance) { Record = record })
        {
            reader.ReadSignMagnitude(4, out int value, "delta_q");
            Assert.AreEqual(expected, value);
        }

        using var written = new MemoryStream();
        using (var writer = new AomStream(written, NullMp4Logger.Instance) { Source = record })
        {
            writer.WriteSignMagnitude(4, writer.Pick("delta_q", expected, expected), "delta_q");
            writer.WriteFixed(3, 0, "");
        }
        CollectionAssert.AreEqual(bytes, written.ToArray());
    }

    /// <summary>A superframe (B.4): its index at its end, the marker at both ends of it, the sizes little-endian.</summary>
    [TestMethod]
    public void FindsTheFramesOfASuperframe()
    {
        // two frames, of 3 and 0x0102 bytes, sizes of two bytes each: marker 0b110 01 001
        byte marker = 0b110_01_001;
        var chunk = new byte[3 + 0x0102 + 6];
        chunk[^6] = marker;
        chunk[^5] = 3;
        chunk[^4] = 0;
        chunk[^3] = 0x02;
        chunk[^2] = 0x01;
        chunk[^1] = marker;
        CollectionAssert.AreEqual(new[] { 3, 0x0102 }, VP9Context.SuperframeFrameSizes(chunk, 0, chunk.Length));

        // the index's first byte not the marker: a frame, not a superframe
        chunk[^6] = 0;
        Assert.IsNull(VP9Context.SuperframeFrameSizes(chunk, 0, chunk.Length));
    }

    /// <summary>
    /// The sample entry a track writes is made of its stream's first key frame: 'vp09' of the frame's size, its 'vpcC' of
    /// version 1 with the profile, bit depth, chroma subsampling and range the file's has - and the level the frame size
    /// and rate make, 2 for 320x240 at 30 frames a second, where Chromium's file says 1, whose pictures are at most 36864.
    /// </summary>
    [TestMethod]
    public void WritesTheSampleEntryOfItsStream()
    {
        string? root = ConformanceCorpus.Locate();
        string path = Path.Combine(root ?? "", "chromium", "bear-320x240-v_frag-vp9.mp4");
        if (root == null || !File.Exists(path))
            Assert.Inconclusive("no Chromium files; run DownloadConformance.ps1 -Codec Chromium");

        using var stream = File.OpenRead(path);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(stream)));
        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        var fileEntry = (VisualSampleEntry)reader.Tracks[trackID].Stbl.Children.OfType<SampleDescriptionBox>().Single().Children.First();
        var fileConfig = fileEntry.Children.OfType<VPCodecConfigurationBox>().Single();
        Assert.IsInstanceOfType(reader.Tracks[trackID].Track, typeof(VP9Track));

        var track = new VP9Track(reader.Tracks[trackID].Track.Timescale, 0);
        var sync = new List<bool>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            track.DefaultSampleDuration = sample.Duration;
            track.ProcessSample(sample.Data.ToArray(), out _, out bool isSync);
            sync.Add(isSync);
        }
        Assert.IsTrue(sync[0], "the first sample a key frame");
        Assert.IsTrue(sync.Count(x => x) >= 1);

        var entry = (VisualSampleEntry)track.CreateSampleEntryBox();
        var config = entry.Children.OfType<VPCodecConfigurationBox>().Single();
        Assert.AreEqual("vp09", IsoStream.ToFourCC(entry.FourCC));
        Assert.AreEqual(fileEntry.Width, entry.Width);
        Assert.AreEqual(fileEntry.Height, entry.Height);
        Assert.AreEqual(1, config.Version);
        Assert.AreEqual(fileConfig.Profile, config.Profile);
        Assert.AreEqual(fileConfig.BitDepth, config.BitDepth);
        Assert.AreEqual(fileConfig.ChromaSubsampling, config.ChromaSubsampling);
        Assert.AreEqual(fileConfig.VideoFullRangeFlag, config.VideoFullRangeFlag);
        Assert.AreEqual(20, config.Level);
    }
}
