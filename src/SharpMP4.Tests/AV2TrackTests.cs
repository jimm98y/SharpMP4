using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Encryption;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="AV2Track"/>: AV2 in MP4 as the AV2 Codec ISO Media File Format Binding
/// (working group draft) has it.
/// </summary>
[TestClass]
public class AV2TrackTests
{
    // The top left 16x16 of akiyo's first four frames, encoded by AVM v1.0.0 (avmenc --limit=4
    // --cpu-used=6): four temporal units, each of OBUs led by their leb128() lengths - a temporal
    // delimiter, the sequence header and a closed loop key frame, then a frame and two TIP frames.
    private static readonly string[] TemporalUnits =
    {
        "01081704800a000cffc1e6000000e6ee6809abf3bbf526e65690980110f0000000d81c0e649d4ba5abefb0b6eac1cc77a3395d2273a3ec104ea65fbf8a03698f48cd0041d16fb1956c2e2a5e988938fa4310c7817fb1ae90181f3a24c87ee793642e512d141adda0e418007ce3e7e8fe6b3dfdb191d67d19a8f15914c0295dc4c3c7fdfb0af9c5529f9d07cdbc3fd342d01a11c85f4cfa7ee68bb1b42f40756972b83d75563ddace0bf3a8e34cbe1be2dbffcc80",
        "0108091cf80931011000c0a3",
        "01080538e04ac540",
        "01080538e06bc540",
    };

    private static readonly byte[] SequenceHeader = Convert.FromHexString("1704800a000cffc1e6000000e6ee6809abf3bbf526e65690");

    /// <summary>
    /// Each temporal unit is a sample, without its temporal delimiter; the sequence header is in the
    /// 'av2C' box, not in a sample; and the first sample, a closed loop key frame, is the one sync sample.
    /// </summary>
    [TestMethod]
    public void MakesATemporalUnitASample()
    {
        var (container, samples) = BuildAndRead(TemporalUnits.Select(Convert.FromHexString));

        var entry = Entry(container);
        Assert.AreEqual(16, entry.Width);
        Assert.AreEqual(16, entry.Height);
        var av2C = entry.Children.OfType<AV2CodecConfigurationBox>().Single();
        CollectionAssert.AreEqual(SequenceHeader.Skip(1).ToArray(), av2C.ConfigObus.Single(), "the sequence header, without its length");
        Assert.AreEqual(1, entry.Children.OfType<ColourInformationBox>().Count());

        Assert.AreEqual(4, samples.Count);
        CollectionAssert.AreEqual(new[] { true, false, false, false }, samples.Select(s => s.IsRandomAccessPoint).ToArray());

        // The temporal units, but for their delimiters and the sequence header
        var expected = TemporalUnits.Select(Convert.FromHexString).Select(WithoutDelimiterAndSequenceHeader).ToList();
        for (int i = 0; i < samples.Count; i++)
            CollectionAssert.AreEqual(expected[i], samples[i].Data, $"sample {i}");
    }

    /// <summary>The same, fed an OBU at a time rather than a temporal unit at a time.</summary>
    [TestMethod]
    public void MakesTheSameSamplesOfOneObuAtATime()
    {
        var obus = TemporalUnits.Select(Convert.FromHexString).SelectMany(SplitObus);
        var (_, samples) = BuildAndRead(obus);

        var expected = TemporalUnits.Select(Convert.FromHexString).Select(WithoutDelimiterAndSequenceHeader).ToList();
        Assert.AreEqual(expected.Count, samples.Count);
        for (int i = 0; i < samples.Count; i++)
            CollectionAssert.AreEqual(expected[i], samples[i].Data, $"sample {i}");
    }

    /// <summary>A sample read back splits into its OBUs, each with its length, as it was stored.</summary>
    [TestMethod]
    public void ParsesASampleIntoItsObus()
    {
        var (container, samples) = BuildAndRead(TemporalUnits.Select(Convert.FromHexString));
        var track = new AV2Track(Entry(container).Children.OfType<AV2CodecConfigurationBox>().Single(), 30000, 1001);

        var sample = samples[0].Data;
        var obus = track.ParseSample(sample).Select(o => o.ToArray()).ToList();
        CollectionAssert.AreEqual(SplitObus(sample).ToList(), obus, new BytesComparer());
        Assert.AreEqual(1, obus.Count, "the key frame's tile group");
    }

    /// <summary>
    /// Protected as the AV1 binding protects AV1 - the AV2 binding has no Common Encryption yet - each tile is a subsample
    /// and all else is clear: split again, the protected sample has the same subsamples, its headers read as they were;
    /// and it decrypts to the sample it was.
    /// </summary>
    [TestMethod]
    [DataRow("cenc")]
    [DataRow("cbcs")]
    public void ProtectsTheTilesAndDecryptsBack(string scheme)
    {
        byte[] keyId = Enumerable.Range(1, 16).Select(x => (byte)x).ToArray(), key = Enumerable.Range(17, 16).Select(x => (byte)x).ToArray();
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var track = new AV2Track(30000, 1001);
        builder.AddTrack(track, TrackProtection.Create(scheme, keyId, isVideo: true), key);
        foreach (string unit in TemporalUnits)
            builder.ProcessTrackSample(track.TrackID, Convert.FromHexString(unit));
        builder.FinalizeMedia();
        byte[] file = output.ToArray();

        // read without the key: how each sample is protected
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        Assert.AreEqual(scheme, reader.Tracks[trackID].Protection.Scheme);
        var splitter = new Av2SubsampleSplitter(Boxes.Find<AV2CodecConfigurationBox>(container), null);
        int protectedBytes = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            var subsamples = sample.Encryption.Subsamples ?? [];
            Assert.AreEqual(sample.Data.Count, subsamples.Sum(s => s.ClearBytes + (long)s.ProtectedBytes), "the subsamples cover the sample");
            string Describe(EncryptionSubsample[] runs) => string.Join(" | ", runs.Select(s => $"{s.ClearBytes}+{s.ProtectedBytes}"));
            var again = splitter.Split(sample.Data.Array!, sample.Data.Offset, sample.Data.Count, wholeBlocks: scheme != "cbcs");
            Assert.AreEqual(Describe(subsamples), Describe(again), "split again, protected: its headers are clear");
            protectedBytes += subsamples.Sum(s => (int)s.ProtectedBytes);
        }
        Assert.IsTrue(protectedBytes > 0, "the key frame's tile is protected");

        // read with it: the samples they were
        container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        reader = new VideoReader { KeyProvider = id => id.SequenceEqual(keyId) ? key : null };
        reader.Parse(container);
        var expected = TemporalUnits.Select(Convert.FromHexString).Select(WithoutDelimiterAndSequenceHeader).ToList();
        int i = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID), i++)
            CollectionAssert.AreEqual(expected[i], sample.Data.ToArray(), $"sample {i}");
        Assert.AreEqual(expected.Count, i);
    }

    /// <summary>A sample as it was read: the reader reuses the buffer it hands a sample in.</summary>
    private sealed record Sample(byte[] Data, bool IsRandomAccessPoint);

    // The sample entry: 'av02', read as the VisualSampleEntry it is, as 'av01' is
    private static VisualSampleEntry Entry(Container container) =>
        Boxes.Find<VisualSampleEntry>(container) is var entry && IsoStream.ToFourCC(entry.FourCC) == "av02" ? entry : throw new InvalidOperationException("no av02");

    private static (Container Container, List<Sample> Samples) BuildAndRead(IEnumerable<byte[]> units)
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var track = new AV2Track(30000, 1001);
        builder.AddTrack(track);
        foreach (byte[] unit in units)
            builder.ProcessTrackSample(track.TrackID, unit);
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));

        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        var samples = new List<Sample>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
            samples.Add(new Sample(sample.Data.ToArray(), sample.IsRandomAccessPoint));
        return (container, samples);
    }

    /// <summary>Each OBU of a temporal unit with the leb128() length it is led by.</summary>
    private static IEnumerable<byte[]> SplitObus(byte[] unit)
    {
        int pos = 0;
        while (pos < unit.Length)
        {
            int start = pos, size = 0, shift = 0;
            byte b;
            do
            {
                b = unit[pos++];
                size |= (b & 0x7f) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            pos += size;
            yield return unit.AsSpan(start, pos - start).ToArray();
        }
    }

    private static byte[] WithoutDelimiterAndSequenceHeader(byte[] unit) =>
        SplitObus(unit).Where(o => ObuType(o) is not (1 or 2)).SelectMany(o => o).ToArray();

    private static int ObuType(byte[] framed)
    {
        int pos = 0;
        while ((framed[pos] & 0x80) != 0)
            pos++;
        return (framed[pos + 1] >> 2) & 0x1f;
    }

    private sealed class BytesComparer : System.Collections.IComparer
    {
        public int Compare(object? x, object? y) => ((byte[])x!).SequenceEqual((byte[])y!) ? 0 : 1;
    }
}
