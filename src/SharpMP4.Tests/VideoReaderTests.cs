using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="VideoReader"/> and the times it reports for each sample.
/// </summary>
[TestClass]
public class VideoReaderTests
{
    /// <summary>
    /// The decode time of a sample is the sum of the durations before it, and its duration is the
    /// one its run in the time-to-sample table gives. The first sample used to be reported with a
    /// duration of zero: the walk over the runs started past it, so no run was ever consulted.
    /// </summary>
    [TestMethod]
    public void ReadsDecodeTimesAndDurationsFromTheTimeToSampleTable()
    {
        // A run of 20s, one 21 in the middle, then 20s again - as a camera writes when it holds a
        // nominal rate that does not divide the timescale.
        int[] durations = [20, 20, 20, 21, 20, 20];
        var samples = ReadBack(durations.Length, i => durations[i], _ => 0);

        CollectionAssert.AreEqual(durations, samples.Select(s => s.Duration).ToArray());
        CollectionAssert.AreEqual(new long[] { 0, 20, 40, 60, 81, 101 },
            samples.Select(s => s.DTS).ToArray());
    }

    /// <summary>
    /// A composition offset moves when a picture is shown, not when it is decoded. Adding it to
    /// the decode time as well used to leave the decode times jumping back and forth - on an
    /// iPhone recording they came back 0, 20, 100, 120, 100, 100 - and a caller that pairs samples
    /// by decode time, or measures a duration from the gap between two, then reads nonsense.
    /// </summary>
    [TestMethod]
    public void KeepsCompositionOffsetsOutOfDecodeTimes()
    {
        // Decode order 0, 3, 1, 2 against a presentation order of 0, 1, 2, 3: a B picture pyramid.
        int[] presentation = [0, 3, 1, 2];
        var samples = ReadBack(4, _ => 20, i => (presentation[i] - i) * 20);

        CollectionAssert.AreEqual(new long[] { 0, 20, 40, 60 }, samples.Select(s => s.DTS).ToArray());

        // The writer is free to bias the offsets to keep them non-negative, so what has to hold is
        // the spacing: each picture is shown where its presentation order puts it.
        long bias = samples[0].PTS - samples[0].DTS;
        CollectionAssert.AreEqual(presentation.Select(p => p * 20L).ToArray(),
            samples.Select(s => s.PTS - bias).ToArray());
    }

    /// <summary>Builds a single track MP4 with the given timing and reads every sample back.</summary>
    private static List<MediaSample> ReadBack(int sampleCount, Func<int, int> duration,
        Func<int, int> compositionOffset)
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);

        for (int i = 0; i < sampleCount; i++)
        {
            builder.ProcessRawSample(track.TrackID, new byte[64], duration(i),
                isRandomAccessPoint: true, compositionOffset(i));
        }

        builder.FinalizeMedia();

        var container = new Container();
        // The stream stays open: the reader goes back to the media data for each sample.
        var iso = new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray())));
        container.Read(iso);

        var reader = new VideoReader();
        reader.Parse(container);

        uint trackID = reader.Tracks.Keys.Single();
        var samples = new List<MediaSample>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
            samples.Add(sample);

        Assert.AreEqual(sampleCount, samples.Count);
        return samples;
    }
}
