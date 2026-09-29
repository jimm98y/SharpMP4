using System.Diagnostics;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Encryption;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Subtitle tracks written - WebVTT, TTML, 3GPP timed text and simple text, fragmented and not - read back as the cues
/// they were written from, each sample over its time, a gap between cues a sample of none; and 3GPP timed text written
/// decodes in ffmpeg to the same text.
/// </summary>
[TestClass]
public class SubtitleWriterTests
{
    // over 1000 units a second: a cue, two cues together, a gap, and a cue with an ID and settings
    private static readonly (long Start, long End, SubtitleCue[] Cues)[] Samples =
    [
        (0, 1500, [new SubtitleCue { Text = "Hello, world." }]),
        (1500, 3000, [new SubtitleCue { Text = "Two lines\nof text" }, new SubtitleCue { Text = "and another" }]),
        (3000, 4000, []),
        (4000, 6250, [new SubtitleCue { Text = "Žluťoučký kůň", Id = "cue-4", Settings = "align:right line:1" }]),
    ];

    public static IEnumerable<object[]> Tracks() =>
        from kind in new[] { "wvtt", "stpp", "tx3g", "stxt" }
        from fragmented in new[] { false, true }
        select new object[] { kind, fragmented };

    private static SubtitleTrackBase Create(string kind) => kind switch
    {
        "wvtt" => new WebVttTrack(),
        "stpp" => new TtmlTrack(),
        "tx3g" => new TimedTextTrack(),
        _ => new SimpleTextTrack(),
    };

    /// <summary>What a kind of track keeps of the cues of a sample: TTML and 3GPP timed text keep one text a sample.</summary>
    private static SubtitleCue[] Kept(string kind, SubtitleCue[] cues) => kind switch
    {
        // a TTML sample is always a document: of a gap, an empty one
        "stpp" => [new SubtitleCue { Text = cues.Length == 0 ? EmptyTtml : Ttml(cues) }],
        "tx3g" or "stxt" => cues.Length == 0 ? [] : [new SubtitleCue { Text = string.Join("\n", cues.Select(c => c.Text)) }],
        _ => cues,
    };

    private const string EmptyTtml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><tt xmlns=\"http://www.w3.org/ns/ttml\"><body/></tt>";

    private static string Ttml(SubtitleCue[] cues) =>
        "<?xml version=\"1.0\" encoding=\"UTF-8\"?><tt xmlns=\"http://www.w3.org/ns/ttml\"><body><div>" +
        string.Concat(cues.Select(c => $"<p>{c.Text}</p>")) + "</div></body></tt>";

    private static readonly byte[] KeyId = Enumerable.Range(1, 16).Select(x => (byte)x).ToArray();
    private static readonly byte[] Key = Enumerable.Range(17, 16).Select(x => (byte)x).ToArray();

    private static byte[] Build(string kind, bool fragmented, string? scheme = null)
    {
        using var output = new MemoryStream();
        var track = Create(kind);
        IMp4Builder builder;
        if (fragmented)
        {
            var fragmentedBuilder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 2000);
            if (scheme != null)
                fragmentedBuilder.AddTrack(track, TrackProtection.Create(scheme, KeyId, isVideo: false), Key);
            else
                fragmentedBuilder.AddTrack(track);
            builder = fragmentedBuilder;
        }
        else
        {
            var mp4Builder = new Mp4Builder(new SingleStreamOutput(output));
            if (scheme != null)
                mp4Builder.AddTrack(track, TrackProtection.Create(scheme, KeyId, isVideo: false), Key);
            else
                mp4Builder.AddTrack(track);
            builder = mp4Builder;
        }
        foreach (var (start, end, cues) in Samples)
        {
            var kept = Kept(kind, cues);
            byte[] sample = kind == "stxt" && kept.Length == 0 ? [] : track.CreateSample(kept);
            builder.ProcessTrackSample(track.TrackID, sample, (int)(end - start));
        }
        builder.FinalizeMedia();
        return output.ToArray();
    }

    [TestMethod]
    [DynamicData(nameof(Tracks))]
    public void ReadsBackTheCuesItWasWrittenFrom(string kind, bool fragmented) => ReadsBack(Build(kind, fragmented), kind, null);

    public static IEnumerable<object[]> ProtectedTracks() =>
        from kind in new[] { "wvtt", "stpp", "tx3g", "stxt" }
        from fragmented in new[] { false, true }
        from scheme in new[] { "cenc", "cbcs" }
        select new object[] { kind, fragmented, scheme };

    /// <summary>
    /// Protected, a text track's sample entry is 'enct', laid out as the one it was, which its 'frma' names; its samples
    /// are protected whole. Read without the key, it is known for what it is and says how each sample is protected; with
    /// it, the cues are the ones written.
    /// </summary>
    [TestMethod]
    [DynamicData(nameof(ProtectedTracks))]
    public void ReadsBackProtectedCues(string kind, bool fragmented, string scheme)
    {
        byte[] file = Build(kind, fragmented, scheme);

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        var entry = Boxes.Find<SampleDescriptionBox>(container).Children.Single();
        Assert.AreEqual("enct", IsoStream.ToFourCC(entry.FourCC));
        Assert.AreEqual(Create(kind).CreateSampleEntryBox().GetType(), entry.GetType(), "laid out as the entry it was");

        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        Assert.AreEqual(kind, reader.Tracks[trackID].Protection.OriginalFormat);
        Assert.AreEqual(Create(kind).GetType(), reader.Tracks[trackID].Track.GetType());
        int protectedSamples = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            if (sample.Data.Count > 0)
            {
                Assert.IsNotNull(sample.Encryption, "a sample not protected");
                Assert.IsNull(sample.Encryption.Subsamples, "protected whole");
                protectedSamples++;
            }
        }
        Assert.IsTrue(protectedSamples > 0);

        ReadsBack(file, kind, id => id.SequenceEqual(KeyId) ? Key : null);
    }

    private static void ReadsBack(byte[] file, string kind, Func<byte[], byte[]?>? keyProvider)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        var reader = new VideoReader { KeyProvider = keyProvider! };
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        var track = reader.Tracks[trackID].Track as ISubtitleTrack;
        Assert.IsNotNull(track, $"read as {reader.Tracks[trackID].Track?.GetType().Name}");
        Assert.AreEqual(Create(kind).GetType(), track.GetType());

        int i = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID), i++)
        {
            var (start, end, cues) = Samples[i];
            Assert.AreEqual(start, sample.PTS, $"sample {i} start");
            Assert.AreEqual(end - start, sample.Duration, $"sample {i} duration");
            byte[] data = sample.Data.ToArray();
            var read = track.ParseCues(data, 0, data.Length, sample.PTS, sample.Duration);
            var expected = Kept(kind, cues);
            Assert.AreEqual(expected.Length, read.Count, $"sample {i} cues");
            for (int c = 0; c < expected.Length; c++)
            {
                Assert.AreEqual(expected[c].Text, read[c].Text, $"sample {i} cue {c}");
                Assert.AreEqual(start, read[c].Start);
                Assert.AreEqual(end, read[c].End);
                if (kind == "wvtt")
                {
                    Assert.AreEqual(expected[c].Id, read[c].Id);
                    Assert.AreEqual(expected[c].Settings, read[c].Settings);
                }
            }
        }
        Assert.AreEqual(Samples.Length, i);
    }

    /// <summary>
    /// ffmpeg decodes the 3GPP timed text written to the text it was written from, at the times it was. Not protected:
    /// ffmpeg reads an 'enct' track as data, its 'frma' taken for audio and video alone.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void WritesTimedTextFfmpegDecodes(bool fragmented)
    {
        string? ffmpeg = SharpMP4.Tests.Conformance.ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");

        string path = Path.Combine(Path.GetTempPath(), $"sharpmp4-tx3g-{Guid.NewGuid():N}.mp4");
        try
        {
            File.WriteAllBytes(path, Build("tx3g", fragmented));
            var start = new ProcessStartInfo(ffmpeg) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true, StandardOutputEncoding = System.Text.Encoding.UTF8 };
            foreach (string arg in new[] { "-v", "error", "-i", path, "-map", "0:0", "-c:s", "text", "-f", "srt", "-" })
                start.ArgumentList.Add(arg);
            using var process = Process.Start(start)!;
            string srt = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();
            Assert.AreEqual("", errors.Trim(), "ffmpeg");

            string expected = string.Join("\n\n", Samples.Where(s => s.Cues.Length > 0).Select((s, n) =>
                $"{n + 1}\n{Time(s.Start)} --> {Time(s.End)}\n{Kept("tx3g", s.Cues)[0].Text}")) + "\n\n";
            Assert.AreEqual(expected, srt.Replace("\r\n", "\n"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    public static IEnumerable<object[]> ForcedTracks() =>
        from kind in new[] { "wvtt", "stpp", "tx3g", "stxt" }
        from fragmented in new[] { false, true }
        from forced in new[] { true, false }
        select new object[] { kind, fragmented, forced };

    private static byte[] BuildOne(string kind, bool fragmented, bool forced)
    {
        using var output = new MemoryStream();
        var track = Create(kind);
        track.Forced = forced;
        IMp4Builder builder = fragmented
            ? new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 2000)
            : new Mp4Builder(new SingleStreamOutput(output));
        builder.AddTrack(track);
        builder.ProcessTrackSample(track.TrackID, track.CreateSample(Kept(kind, [new SubtitleCue { Text = "Forced?" }])), 1000);
        builder.FinalizeMedia();
        return output.ToArray();
    }

    /// <summary>
    /// Forced, a subtitle track of any kind says so in a 'kind' box of the DASH role 'forced-subtitle' in its 'trak' - which
    /// ffmpeg reads as its forced disposition - and 3GPP timed text in its entry's display flags too, every sample forced,
    /// 0x80000000, which VLC selects the track by default for. Read back, the track is forced.
    /// </summary>
    [TestMethod]
    [DynamicData(nameof(ForcedTracks))]
    public void WritesWhetherTheTrackIsForced(string kind, bool fragmented, bool forced)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(BuildOne(kind, fragmented, forced)))));

        var trak = Boxes.Find<TrackBox>(container);
        Assert.AreEqual(forced, SubtitleTrackBase.IsForced(trak), "the 'kind' box");
        if (forced)
        {
            var kindBox = Boxes.Find<KindBox>(container);
            Assert.AreEqual("urn:mpeg:dash:role:2011", kindBox.SchemeURI.Text);
            Assert.AreEqual("forced-subtitle", kindBox.Value.Text);
        }
        if (kind == "tx3g")
            Assert.AreEqual(forced ? 0x80000000u : 0u, Boxes.Find<TextSampleEntrytx3gDup>(container).DisplayFlags);

        var reader = new VideoReader();
        reader.Parse(container);
        var track = (ISubtitleTrack)reader.Tracks.Values.Single().Track;
        Assert.AreEqual(forced, track.Forced, "read back");
        Assert.AreEqual(forced, ((ISubtitleTrack)track.Clone()).Forced, "cloned");
    }

    /// <summary>ffmpeg reads the 'kind' box as the track's forced disposition, of the kinds of track it reads.</summary>
    [TestMethod]
    [DataRow("tx3g")]
    [DataRow("stpp")]
    [DataRow("stxt")]
    public void WritesAForcedTrackFfmpegReadsAsForced(string kind)
    {
        string? ffmpeg = SharpMP4.Tests.Conformance.ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");
        string ffprobe = Path.Combine(Path.GetDirectoryName(ffmpeg)!, "ffprobe" + Path.GetExtension(ffmpeg));

        foreach (bool forced in new[] { true, false })
        {
            string path = Path.Combine(Path.GetTempPath(), $"sharpmp4-forced-{Guid.NewGuid():N}.mp4");
            try
            {
                File.WriteAllBytes(path, BuildOne(kind, fragmented: false, forced));
                var start = new ProcessStartInfo(ffprobe) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
                foreach (string arg in new[] { "-v", "error", "-show_entries", "stream_disposition=forced", "-of", "csv=p=0", path })
                    start.ArgumentList.Add(arg);
                using var process = Process.Start(start)!;
                string disposition = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                Assert.AreEqual(forced ? "1" : "0", disposition, $"forced {forced}");
            }
            finally
            {
                File.Delete(path);
            }
        }
    }

    private static string Time(long ms) => TimeSpan.FromMilliseconds(ms).ToString(@"hh\:mm\:ss\,fff");
}
