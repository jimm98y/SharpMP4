using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// The tracks against the corpus: AV1's test vectors made samples of however their OBUs are given, and the files of
/// codecs no track reads written again with the sample entry they had.
/// </summary>
[TestClass]
public class TrackConformanceTests
{
    public TestContext TestContext { get; set; } = null!;

    /// <summary>
    /// The temporal units of libaom's test vectors, given to a track an IVF frame at a time and an OBU at a time, come out
    /// as samples of the temporal units, without their temporal delimiters and padding: frame headers and the tile groups
    /// after them - av1-1-b8-22-svc's - and the frames of a second spatial layer in one sample.
    /// </summary>
    [TestMethod]
    public void MakesTheTemporalUnitsOfAV1VectorsSamples()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.Streams(root, "av1").Where(f => f.Contains("libaom") && f.EndsWith(".ivf", StringComparison.OrdinalIgnoreCase)).ToList();
        if (files.Count == 0)
            Assert.Inconclusive("no libaom AV1 vectors");

        var failures = new List<string>();
        int units = 0;
        foreach (string file in files)
        {
            var temporalUnits = SharpTrace.IvfFrames(file).ToList();
            var expected = temporalUnits.Select(WithoutDelimitersAndPadding).Where(u => u.Length > 0).ToList();
            units += expected.Count;

            foreach (var (how, inputs) in new (string, IEnumerable<byte[]>)[] { ("a unit at a time", temporalUnits), ("an OBU at a time", temporalUnits.SelectMany(Obus)) })
            {
                var actual = Feed(new AV1Track { Logger = new CapturingLogger() }, inputs);
                string name = Path.GetFileName(file);
                if (actual.Count != expected.Count)
                {
                    failures.Add($"{name}, {how}: {actual.Count} samples of {expected.Count} temporal units");
                    continue;
                }
                int differs = Enumerable.Range(0, expected.Count).FirstOrDefault(i => !expected[i].SequenceEqual(actual[i]), -1);
                if (differs >= 0)
                    failures.Add($"{name}, {how}: sample {differs} is not its temporal unit");
            }
        }

        TestContext.WriteLine($"{files.Count} vectors, {units} temporal units");
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures.Take(30)));
    }

    /// <summary>
    /// A track of a codec none reads - AMR-WB - written again as a remux writes it has its sample entry as the file had it,
    /// byte for byte: the entry's first box was written where the entry belongs.
    /// </summary>
    [TestMethod]
    [DataRow("mp4parse/mp4parse/amr_wb_1f.3gp")]
    public void WritesTheSampleEntryOfACodecNoneReadsAsItWas(string name)
    {
        string? root = ConformanceCorpus.Locate();
        string path = Path.Combine(root ?? "", name);
        if (root == null || !File.Exists(path))
            Assert.Inconclusive($"no {name}; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        using var input = File.OpenRead(path);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(input)));
        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        var track = reader.Tracks[trackID].Track;
        Assert.IsInstanceOfType<GenericTrack>(track);
        byte[] original = EntryBytes(container);

        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var clone = track.Clone();
        builder.AddTrack(clone);
        int samples = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID), samples++)
            builder.ProcessTrackSample(clone.TrackID, sample.Data, sample.Duration);
        builder.FinalizeMedia();
        Assert.IsTrue(samples > 0);

        var written = new Container();
        written.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        CollectionAssert.AreEqual(original, EntryBytes(written));

        // the original's tree is as it was: its entry still its 'stsd''s
        var stsd = Boxes.Find<SampleDescriptionBox>(container);
        Assert.AreSame(stsd, stsd.Children[0].GetParent());
    }

    /// <summary>
    /// AC-3, E-AC-3, FLAC, MP3 and ALAC of others' muxers - Chromium's, FFmpeg's, iTunes' - read by their own tracks and
    /// written again as a remux writes them: every sample as it was, and the configuration - the 'dac3', 'dec3', 'dfLa' or
    /// 'alac' - byte for byte; of MP3, whose 'esds' is written as the track writes one, its object type and bit rates.
    /// </summary>
    [TestMethod]
    [DataRow("chromium/bear-ac3-only-frag.mp4", typeof(AC3Track))]
    [DataRow("chromium/bear-eac3-only-frag.mp4", typeof(EAC3Track))]
    [DataRow("chromium/bear-flac.mp4", typeof(FlacTrack))]
    [DataRow("fate/mpegaudio/packed_maindata.mp3.mp4", typeof(Mp3Track))]
    [DataRow("fate/lossless-audio/inside.m4a", typeof(AlacTrack))]
    [DataRow("metadata/mutagen/alac.m4a", typeof(AlacTrack))]
    public void RemuxesTheAudioOfOthers(string name, Type type)
    {
        string? root = ConformanceCorpus.Locate();
        string path = Path.Combine(root ?? "", name);
        if (root == null || !File.Exists(path))
            Assert.Inconclusive($"no {name}; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        using var input = File.OpenRead(path);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(input)));
        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single(k => reader.Tracks[k].Track.HandlerType == HandlerTypes.Sound);
        var track = reader.Tracks[trackID].Track;
        Assert.IsInstanceOfType(track, type);

        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var clone = track.Clone();
        builder.AddTrack(clone);
        var samples = new List<byte[]>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            samples.Add(sample.Data.ToArray());
            builder.ProcessTrackSample(clone.TrackID, sample.Data, sample.Duration);
        }
        builder.FinalizeMedia();
        Assert.IsTrue(samples.Count > 0);

        var written = new Container();
        written.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var rereader = new VideoReader();
        rereader.Parse(written);
        uint writtenID = rereader.Tracks.Keys.Single();
        var reread = rereader.Tracks[writtenID].Track;
        Assert.IsInstanceOfType(reread, type);
        int index = 0;
        for (var sample = rereader.ReadSample(writtenID); sample != null; sample = rereader.ReadSample(writtenID), index++)
            CollectionAssert.AreEqual(samples[index], sample.Data.ToArray(), $"sample {index}");
        Assert.AreEqual(samples.Count, index);

        if (reread is Mp3Track mp3)
        {
            var read = (Mp3Track)track;
            Assert.AreEqual((read.ObjectTypeIndication, read.ChannelCount, read.SamplingRate, read.MaxBitrate, read.AvgBitrate, read.BufferSizeDB),
                (mp3.ObjectTypeIndication, mp3.ChannelCount, mp3.SamplingRate, mp3.MaxBitrate, mp3.AvgBitrate, mp3.BufferSizeDB));
        }
        else
        {
            CollectionAssert.AreEqual(ConfigBytes(container), ConfigBytes(written));
        }
    }

    /// <summary>The bytes of the codec configuration of a file's first sample entry: its first box but a 'btrt' or 'chan'.</summary>
    private static byte[] ConfigBytes(Container container)
    {
        var entry = Boxes.Find<SampleDescriptionBox>(container).Children[0];
        var config = entry.Children.First(x => x.FourCC != IsoStream.FromFourCC("btrt") && x.FourCC != IsoStream.FromFourCC("chan"));
        using var memory = new MemoryStream();
        new IsoStream(new StreamWrapper(memory)).WriteBox(config, "");
        return memory.ToArray();
    }

    private static byte[] EntryBytes(Container container)
    {
        using var memory = new MemoryStream();
        new IsoStream(new StreamWrapper(memory)).WriteBox(Boxes.Find<SampleDescriptionBox>(container).Children[0], "");
        return memory.ToArray();
    }

    private static List<byte[]> Feed(AV1Track track, IEnumerable<byte[]> inputs)
    {
        var samples = new List<byte[]>();
        foreach (byte[] input in inputs)
        {
            track.ProcessSample(input, out var output, out _);
            if (output.Array != null)
                samples.Add(output.ToArray());
        }
        track.ProcessSample(null, out var last, out _);
        if (last.Array != null)
            samples.Add(last.ToArray());
        return samples;
    }

    private static byte[] WithoutDelimitersAndPadding(byte[] unit) =>
        Obus(unit).Where(o => ((o[0] >> 3) & 0xF) is not (2 or 15)).SelectMany(o => o).ToArray();

    /// <summary>The OBUs of a temporal unit, each with its header and size; one without a size field is the rest.</summary>
    private static List<byte[]> Obus(byte[] data)
    {
        var obus = new List<byte[]>();
        for (int p = 0; p < data.Length;)
        {
            int header = 1 + ((data[p] & 0x04) != 0 ? 1 : 0);
            int size = data.Length - p;
            if ((data[p] & 0x02) != 0)
            {
                long value = 0;
                int i = 0;
                for (; i < 8; i++)
                {
                    value |= (long)(data[p + header + i] & 0x7f) << (7 * i);
                    if ((data[p + header + i] & 0x80) == 0)
                        break;
                }
                size = header + i + 1 + (int)value;
            }
            obus.Add(data.AsSpan(p, size).ToArray());
            p += size;
        }
        return obus;
    }
}
