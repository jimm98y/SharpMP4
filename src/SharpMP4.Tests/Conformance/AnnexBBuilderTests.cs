using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// The builders' ProcessAnnexBTrackSample, against the conformance streams of H.264, H.265 and H.266: their NAL
/// units given as the Annex B byte stream - in chunks of one unit or several, behind start codes of three bytes and
/// of four - make the file that the same units given one at a time to ProcessTrackSample make.
/// </summary>
[TestClass]
public class AnnexBBuilderTests
{
    [TestMethod]
    [DataRow("h264", false)]
    [DataRow("h265", false)]
    [DataRow("h266", false)]
    [DataRow("h264", true)]
    [DataRow("h265", true)]
    [DataRow("h266", true)]
    public void WritesTheStreamAsItsNalUnitsGivenOneAtATime(string codec, bool fragmented)
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");
        var streams = ConformanceCorpus.Streams(root, codec).Take(4).ToList();
        if (streams.Count == 0)
            Assert.Inconclusive($"no {codec} streams under {root}");

        var failures = new List<string>();
        foreach (string path in streams)
        {
            string name = Path.GetRelativePath(root, path);
            try
            {
                var units = NalUnits(codec, File.ReadAllBytes(path));

                var oneAtATime = Write(codec, fragmented, (builder, id) =>
                {
                    foreach (var unit in units)
                        builder.ProcessTrackSample(id, unit, 1001);
                });
                var annexB = Write(codec, fragmented, (builder, id) =>
                {
                    var random = new Random(units.Count);
                    for (int i = 0; i < units.Count;)
                    {
                        int count = Math.Min(random.Next(1, 6), units.Count - i);
                        var chunk = new List<byte>();
                        for (int j = 0; j < count; j++, i++)
                        {
                            chunk.AddRange(random.Next(2) == 0 ? new byte[] { 0, 0, 1 } : new byte[] { 0, 0, 0, 1 });
                            chunk.AddRange(units[i]);
                        }
                        builder.ProcessAnnexBTrackSample(id, chunk.ToArray(), 1001);
                    }
                });

                var (entryA, samplesA) = Read(oneAtATime);
                var (entryB, samplesB) = Read(annexB);
                if (!entryA.AsSpan().SequenceEqual(entryB))
                    failures.Add($"{name}: sample entry differs");
                if (samplesA.Count != samplesB.Count)
                    failures.Add($"{name}: {samplesB.Count} samples of {samplesA.Count}");
                else if (Enumerable.Range(0, samplesA.Count).FirstOrDefault(i => !samplesA[i].AsSpan().SequenceEqual(samplesB[i]), -1) is int differs && differs >= 0)
                    failures.Add($"{name}: sample {differs} differs");
            }
            catch (Exception ex)
            {
                failures.Add($"{name}: threw {ex.GetType().Name}: {ex.Message}");
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    /// <summary>The builders take the Annex B byte stream for H.26x only, and say so for any other track.</summary>
    [TestMethod]
    public void RefusesTheAnnexBByteStreamOfOtherCodecs()
    {
        var builder = new Mp4Builder(new SingleStreamOutput(new MemoryStream()));
        var track = new AACTrack(2, 48000, 16);
        builder.AddTrack(track);

        Assert.ThrowsExactly<NotSupportedException>(() => builder.ProcessAnnexBTrackSample(track.TrackID, new byte[] { 0, 0, 1, 0x40 }));
    }

    private static ITrack NewTrack(string codec) => codec switch
    {
        "h264" => new H264Track(30000, 1001),
        "h265" => new H265Track(30000, 1001),
        _ => new H266Track(30000, 1001),
    };

    /// <summary>The NAL units of a stream, each an array of its own.</summary>
    private static List<byte[]> NalUnits(string codec, byte[] stream) =>
        ((H26XTrackBase)NewTrack(codec)).ParseAnnexB(stream, 0, stream.Length).Select(unit => unit.ToArray()).ToList();

    private static byte[] Write(string codec, bool fragmented, Action<IMp4Builder, uint> feed)
    {
        var output = new MemoryStream();
        IMp4Builder builder = fragmented
            ? new FragmentedMp4Builder(new SingleStreamOutput(output), 1000)
            : new Mp4Builder(new SingleStreamOutput(output));
        var track = NewTrack(codec);
        builder.AddTrack(track);
        feed(builder, track.TrackID);
        builder.ProcessTrackSample(track.TrackID, (byte[])null!);
        builder.FinalizeMedia();
        return output.ToArray();
    }

    /// <summary>The written sample entry, as its bytes, and every sample of the track, in order.</summary>
    private static (byte[] Entry, List<byte[]> Samples) Read(byte[] file)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();

        var entryStream = new MemoryStream();
        using (var iso = new IsoStream(entryStream))
            iso.WriteBox(reader.Tracks[trackID].Track.CreateSampleEntryBox(), "entry");

        var samples = new List<byte[]>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
            samples.Add(sample.Data.ToArray());
        return (entryStream.ToArray(), samples);
    }
}
