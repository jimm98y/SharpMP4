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
    /// The other tracks' fragments are cut where the video's start, as a player reading the file a fragment at a time
    /// wants them - VLC reached each video fragment late, where the audio was cut on a clock of its own - even with the
    /// audio fed before the video, as a remuxer reading one track after another does: split as the video's cuts come.
    /// </summary>
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void CutsTheOtherTracksWhereTheVideoIsCut(bool audioFirst)
    {
        // the parameter sets of Chromium's bear-640x360-v_frag.mp4
        var video = new H264Track();
        video.ProcessSample(Convert.FromHexString("6764001eacd940a02ff9701100000303e90000ea600f162d96"), out _, out _);
        video.ProcessSample(Convert.FromHexString("68ebe3cb22c0"), out _, out _);
        var audio = new AACTrack(2, 22050, 16);

        using var output = new MemoryStream();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 2000);
        builder.AddTrack(audio);
        builder.AddTrack(video);

        // a minute of each: the video's key frames every 64 frames, so its fragments are of 2.67 s
        int frame = (int)(video.Timescale / 24);
        void Audio() { for (int i = 0; i < 22050 * 60 / 1024; i++) builder.ProcessRawSample(audio.TrackID, new byte[16], 1024, true, new TemporaryMemory()); }
        void Video() { for (int i = 0; i < 24 * 60; i++) builder.ProcessRawSample(video.TrackID, new byte[64], frame, i % 64 == 0, new TemporaryMemory()); }
        if (audioFirst) { Audio(); Video(); } else { Video(); Audio(); }
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var fragments = container.Children.OfType<MovieFragmentBox>()
            .Select(moof => moof.Children.OfType<TrackFragmentBox>().Single())
            .Select(traf => (Track: traf.Children.OfType<TrackFragmentHeaderBox>().Single().TrackID,
                             Start: traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().Single().BaseMediaDecodeTime,
                             Samples: traf.Children.OfType<TrackRunBox>().Single().SampleCount))
            .ToList();
        var videoStarts = fragments.Where(f => f.Track == video.TrackID).Select(f => f.Start / (double)video.Timescale).ToList();
        var audioStarts = fragments.Where(f => f.Track == audio.TrackID).Select(f => f.Start / (double)audio.Timescale).ToList();

        Assert.AreEqual(videoStarts.Count, audioStarts.Count, "a fragment of audio for each of video");
        for (int i = 1; i < audioStarts.Count; i++)
        {
            // the first audio sample starting at or past the video's cut
            double late = audioStarts[i] - videoStarts[i];
            Assert.IsTrue(late >= 0 && late < 1024.0 / 22050, $"audio fragment {i} at {audioStarts[i]:0.000} s, video's at {videoStarts[i]:0.000} s");
        }
        Assert.AreEqual(22050 * 60 / 1024, fragments.Where(f => f.Track == audio.TrackID).Sum(f => (long)f.Samples), "every audio sample");
    }

    /// <summary>
    /// Fragments are written in the order of their time, whatever their lengths: video cut at its key frames every 64
    /// frames, 2.67 s, audio every 2 s. Written a fragment of each track in turn, the video ran ahead of the audio in the
    /// file, 15 s by the end of a minute, and playback stuttered. Of two starting together, the video's is first.
    /// </summary>
    [TestMethod]
    public void WritesFragmentsInTheOrderOfTheirTime()
    {
        // the parameter sets of Chromium's bear-640x360-v_frag.mp4
        var video = new H264Track();
        video.ProcessSample(Convert.FromHexString("6764001eacd940a02ff9701100000303e90000ea600f162d96"), out _, out _);
        video.ProcessSample(Convert.FromHexString("68ebe3cb22c0"), out _, out _);
        var audio = new AACTrack(2, 44100, 16);

        using var output = new MemoryStream();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 2000);
        builder.AddTrack(video);
        builder.AddTrack(audio);

        // a minute of each, the video fed first, as a remuxer reading one track after another does
        int frame = (int)(video.Timescale / 24);
        for (int i = 0; i < 24 * 60; i++)
            builder.ProcessRawSample(video.TrackID, new byte[64], frame, i % 64 == 0, new TemporaryMemory());
        for (int i = 0; i < 44100 * 60 / 1024; i++)
            builder.ProcessRawSample(audio.TrackID, new byte[16], 1024, true, new TemporaryMemory());
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var timescales = new Dictionary<uint, double> { [video.TrackID] = video.Timescale, [audio.TrackID] = audio.Timescale };
        var starts = container.Children.OfType<MovieFragmentBox>()
            .Select(moof => moof.Children.OfType<TrackFragmentBox>().Single())
            .Select(traf => (Track: traf.Children.OfType<TrackFragmentHeaderBox>().Single().TrackID,
                             Start: traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().Single().BaseMediaDecodeTime))
            .Select(f => (f.Track, Seconds: f.Start / timescales[f.Track]))
            .ToList();

        Assert.IsTrue(starts.Count > 40, $"{starts.Count} fragments");
        Assert.AreEqual(video.TrackID, starts[0].Track, "a video fragment first");
        for (int i = 1; i < starts.Count; i++)
            Assert.IsTrue(starts[i].Seconds >= starts[i - 1].Seconds, $"fragment {i} at {starts[i].Seconds:0.00} s after one at {starts[i - 1].Seconds:0.00} s");
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
