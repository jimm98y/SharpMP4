using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="FragmentedMp4Builder"/> and the track runs it writes.</summary>
[TestClass]
public class FragmentedMp4BuilderTests
{
    /// <summary>
    /// A fragment carries its timing in trun rather than in stts and ctts, so reordered pictures
    /// need the composition time offset recorded there; without it a player shows the samples in
    /// decode order.
    /// </summary>
    [TestMethod]
    public void WritesCompositionOffsetsIntoTheTrackRun()
    {
        // Decode order 0, 3, 1, 2 against a presentation order of 0, 1, 2, 3.
        var presentation = new[] { 0, 3, 1, 2 };
        var file = BuildFile(4, i => (presentation[i] - i) * 20);

        var truns = Boxes.FindAll<TrackRunBox>(file);
        Assert.AreEqual(1, truns.Count);

        var trun = truns[0];
        Assert.AreEqual<byte>(1, trun.Version);
        Assert.AreEqual(0x800u, trun.Flags & 0x800);   // sample_composition_time_offset present
        CollectionAssert.AreEqual(new[] { 0, 40, -20, -20 },
            trun._TrunEntry.Select(e => e.SampleCompositionTimeOffset0).ToArray());
    }

    /// <summary>A stream that is not reordered should not pay for the extra field per sample.</summary>
    [TestMethod]
    public void WritesNoCompositionOffsetsWhenNothingIsReordered()
    {
        var file = BuildFile(4, _ => 0);

        var truns = Boxes.FindAll<TrackRunBox>(file);
        Assert.AreEqual(1, truns.Count);

        var trun = truns[0];
        Assert.AreEqual<byte>(0, trun.Version);
        Assert.AreEqual(0u, trun.Flags & 0x800);
    }

    /// <summary>
    /// A fragment of video starts at a random access point - where a player seeking to it can decode from - so it is not
    /// cut before one comes, however long it has run; and each sample says whether it is a sync sample, the first alone
    /// in the first sample's flags where it is the fragment's only one, else in each sample's.
    /// </summary>
    [TestMethod]
    public void StartsVideoFragmentsAtRandomAccessPointsAndFlagsEachSyncSample()
    {
        // the parameter sets of Chromium's bear-640x360-v_frag.mp4
        byte[] sps = Convert.FromHexString("6764001eacd940a02ff9701100000303e90000ea600f162d96");
        byte[] pps = Convert.FromHexString("68ebe3cb22c0");
        var track = new H264Track();
        track.ProcessSample(sps, out _, out _);
        track.ProcessSample(pps, out _, out _);

        // 40 samples of 100 ms, the key frames at 0, 0.7, 1.2 and 3 s, fragments of 1 s wanted
        var keyFrames = new HashSet<int> { 0, 7, 12, 30 };
        int duration = (int)(track.Timescale / 10);
        using var output = new MemoryStream();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 1000);
        builder.AddTrack(track);
        for (int i = 0; i < 40; i++)
            builder.ProcessRawSample(track.TrackID, new byte[64], duration, keyFrames.Contains(i), new TemporaryMemory());
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));

        // cut at the first key frame each second has passed: 1.2 s and 3 s, not 0.7 s
        var truns = Boxes.FindAll<TrackRunBox>(container);
        CollectionAssert.AreEqual(new uint[] { 12, 18, 10 }, truns.Select(t => t.SampleCount).ToArray());
        Assert.AreEqual(0x400u, truns[0].Flags & 0x404, "a fragment of two sync samples flags each sample");
        Assert.AreEqual(0x4u, truns[1].Flags & 0x404, "a fragment of one sync sample flags its first");

        var reader = new SharpMP4.Readers.VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        var sync = new List<bool>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
            sync.Add(sample.IsRandomAccessPoint);
        CollectionAssert.AreEqual(Enumerable.Range(0, 40).Select(keyFrames.Contains).ToArray(), sync.ToArray());
    }

    /// <summary>
    /// Builds a single fragment holding every sample and parses the file back. The fragment length
    /// is far longer than the media, so the samples cannot be split across track runs.
    /// </summary>
    private static Container BuildFile(int sampleCount, Func<int, int> compositionOffset)
    {
        using var output = new MemoryStream();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 60_000);

        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);

        for (int i = 0; i < sampleCount; i++)
        {
            builder.ProcessRawSample(track.TrackID, new byte[64], 20,
                isRandomAccessPoint: true, new TemporaryMemory(), compositionOffset(i));
        }

        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        return container;
    }
}
