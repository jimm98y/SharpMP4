using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using SharpISOBMFF;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Reads the subtitle tracks of the corpus into their cues: Shaka Player's WebVTT and TTML segments as its own tests
/// expect them (test/text/mp4_vtt_parser_unit.js, mp4_ttml_parser_unit.js), and the 3GPP timed text, TTML and simple
/// text tracks ffmpeg reads as ffmpeg reads them - each sample's time and bytes, and of timed text the text.
/// </summary>
[TestClass]
public class SubtitleTrackTests
{
    private sealed record Cue(double Start, double End, string Text, string? Settings);

    private static string? Shaka(string name)
    {
        string? root = ConformanceCorpus.Locate();
        string path = Path.Combine(root ?? "", "shaka", name);
        return root != null && File.Exists(path) ? path : null;
    }

    /// <summary>An init segment and a media segment, one after the other as a player appends them.</summary>
    private static (ISubtitleTrack Track, List<(MediaSample Sample, byte[] Data)> Samples) Read(params string[] names)
    {
        var paths = names.Select(Shaka).ToList();
        if (paths.Any(p => p == null))
            Assert.Inconclusive("no Shaka files; run DownloadConformance.ps1 -Codec Shaka");
        byte[] bytes = paths.SelectMany(p => File.ReadAllBytes(p!)).ToArray();
        return Read(bytes);
    }

    private static (ISubtitleTrack Track, List<(MediaSample Sample, byte[] Data)> Samples) Read(byte[] bytes)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(bytes))));
        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        var track = reader.Tracks[trackID].Track as ISubtitleTrack;
        Assert.IsNotNull(track, $"read as {reader.Tracks[trackID].Track?.GetType().Name}");

        var samples = new List<(MediaSample, byte[])>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
            samples.Add((sample, sample.Data.ToArray()));
        return (track, samples);
    }

    private static List<Cue> Cues(ISubtitleTrack track, List<(MediaSample Sample, byte[] Data)> samples) =>
        samples.SelectMany(s => track.ParseCues(s.Data, 0, s.Data.Length, s.Sample.PTS, s.Sample.Duration))
            .Select(c => new Cue((double)c.Start / track.Timescale, (double)c.End / track.Timescale, c.Text, c.Settings))
            .ToList();

    private static void AreEqual(IEnumerable<(double Start, double End, string Text)> expected, List<Cue> actual)
    {
        var want = expected.ToList();
        Assert.AreEqual(want.Count, actual.Count, "cues: " + string.Join(" | ", actual));
        for (int i = 0; i < want.Count; i++)
        {
            Assert.AreEqual(want[i].Start, actual[i].Start, 1e-6, $"cue {i} start");
            Assert.AreEqual(want[i].End, actual[i].End, 1e-6, $"cue {i} end");
            Assert.AreEqual(want[i].Text, actual[i].Text, $"cue {i} text");
        }
    }

    [TestMethod]
    public void ReadsWebVttSegmentsAsShakaDoes()
    {
        var (track, samples) = Read("vtt-init.mp4", "vtt-segment.mp4");
        Assert.IsInstanceOfType(track, typeof(WebVttTrack));
        StringAssert.StartsWith(((WebVttTrack)track).Config, "WEBVTT");
        AreEqual([
            (111.8, 115.8, "It has shed much innocent blood.\n"),
            (118, 120, "You're a fool for traveling alone,\nso completely unprepared.\n"),
        ], Cues(track, samples));
    }

    /// <summary>
    /// Two cues of one sample share its time. The segment's 'trun' says its samples are 128 bytes from the 'moof', 16 past
    /// where the 'mdat' has them - Shaka reads an 'mdat' through, whatever the offsets say - so that is put right first.
    /// </summary>
    [TestMethod]
    public void ReadsTheCuesOfASampleOverItsTime()
    {
        if (Shaka("vtt-init.mp4") is not string init || Shaka("vtt-segment-multi-payload.mp4") is not string path)
        {
            Assert.Inconclusive("no Shaka files; run DownloadConformance.ps1 -Codec Shaka");
            return;
        }
        byte[] segment = File.ReadAllBytes(path);
        int trun = segment.AsSpan().IndexOf("trun"u8);
        int dataOffset = trun + 4 + 4 + 4; // after the type, version and flags, and sample_count
        Assert.AreEqual(0x80, segment[dataOffset + 3], "the data offset the file has");
        segment[dataOffset + 3] = 0x70; // the moof's 104 bytes and the mdat's header
        var (track, samples) = Read(File.ReadAllBytes(init).Concat(segment).ToArray());
        AreEqual([(110, 113, "Hello"), (110, 113, "and"), (113, 116.276, "goodbye")], Cues(track, samples));
    }

    [TestMethod]
    public void ReadsTheSettingsOfACue()
    {
        var (track, samples) = Read("vtt-init.mp4", "vtt-segment-settings.mp4");
        var cues = Cues(track, samples);
        Assert.AreEqual(2, cues.Count);
        // Shaka's textAlign right, size 50, position 10; and vertical left to right, line 1
        foreach (string setting in new[] { "align:right", "size:50%", "position:10%" })
            StringAssert.Contains(cues[0].Settings, setting);
        foreach (string setting in new[] { "vertical:lr", "line:1" })
            StringAssert.Contains(cues[1].Settings, setting);
    }

    /// <summary>Without a duration in the 'trun' or 'tfhd', the 'trex's (shaka-player#919).</summary>
    [TestMethod]
    public void ReadsSamplesWithoutADurationOfTheirOwn()
    {
        var (track, samples) = Read("vtt-init.mp4", "vtt-segment-no-duration.mp4");
        AreEqual(Enumerable.Range(10, 10).Select(i => ((double)i, (double)i + 1, $"cue {i}")), Cues(track, samples));
    }

    /// <summary>A segment of four fragments, each timed by its 'tfdt': a cue, a 'vtte', and a cue the last repeats.</summary>
    [TestMethod]
    public void ReadsEveryFragmentOfASegment()
    {
        var (track, samples) = Read("vtt-chunked-init.mp4", "vtt-chunked-segment.mp4");
        var cues = Cues(track, samples);
        CollectionAssert.AreEqual(
            new (double, double)[] { (1790260814, 1790260814.5), (1790260815, 1790260815.5), (1790260815.5, 1790260816) },
            cues.Select(c => (c.Start, c.End)).ToArray());
        Assert.AreEqual(cues[1].Text, cues[2].Text);
        Assert.AreNotEqual(cues[0].Text, cues[1].Text);
    }

    [TestMethod]
    [DataRow("ttml-segment-multiple-mdat.mp4")]
    [DataRow("ttml-segment-multiple-sample.mp4")]
    public void ReadsEachTtmlDocument(string segment)
    {
        var (track, samples) = Read("ttml-init.mp4", segment);
        Assert.IsInstanceOfType(track, typeof(TtmlTrack));
        var cues = Cues(track, samples);
        // two documents, each a body of a div of five paragraphs
        Assert.AreEqual(2, cues.Count);
        foreach (var cue in cues)
            Assert.AreEqual(5, Regex.Matches(cue.Text, @"<(\w+:)?p[\s>]").Count, cue.Text);
    }

    [TestMethod]
    public void ReadsEachTtmlSampleOverItsOwnTime()
    {
        var (track, samples) = Read("ttml-init.mp4", "ttml-segment-multiple-sample-timed.mp4");
        CollectionAssert.AreEqual(new (double, double)[] { (0, 0.5), (0.5, 1), (1, 1.5), (1.5, 2) }, Cues(track, samples).Select(c => (c.Start, c.End)).ToArray());
    }

    [TestMethod]
    public void ReadsATtmlDocument()
    {
        var (track, samples) = Read("ttml-init.mp4", "ttml-segment.mp4");
        var text = string.Concat(Cues(track, samples).Select(c => c.Text));
        foreach (string line in new[] { "Thom.", "Look Celia, we have to follow our passions;", "just want to be awesome in space." })
            StringAssert.Contains(text, line);
    }

    /// <summary>An IMSC document with its images after it: the cue is the document alone.</summary>
    [TestMethod]
    public void ReadsTheDocumentOfATtmlSampleWithImages()
    {
        var (track, samples) = Read("imsc-image-init.cmft", "imsc-image-segment.cmft");
        var cues = Cues(track, samples);
        Assert.IsTrue(cues.Count > 0);
        var sample = samples.First(s => s.Data.Length > 0);
        string document = track.ParseCues(sample.Data, 0, sample.Data.Length, 0, 0).Single().Text;
        StringAssert.Matches(document, new Regex(@"</(\w+:)?tt\s*>$"));
        Assert.IsTrue(System.Text.Encoding.UTF8.GetByteCount(document) < sample.Data.Length, "the images after it are left out");
    }

    // The subtitle and text tracks ffmpeg reads: 3GPP timed text, TTML and simple text
    private static readonly string[] FfmpegFiles =
    [
        "chromium/bear-1280x720-avt_subt_frag.mp4",
        "fate/sub/MovText_capability_tester.mp4",
        "isobmff/isobmff/22_tx3g.mp4",
        "isobmff/isobmff/19_ttml.mp4",
        "isobmff/isobmff/20_stxt.mp4",
        "isobmff/uvvu/Solekai007_1920_29_1x1_v7clear.uvu",
    ];

    /// <summary>
    /// Each sample of a subtitle track at the time, for the time, and of the bytes ffmpeg's packets are; and of 3GPP timed
    /// text, the cues the text ffmpeg decodes it to.
    /// </summary>
    [TestMethod]
    public void ReadsSubtitleTracksAsFfmpegDoes()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");
        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");
        string ffprobe = Path.Combine(Path.GetDirectoryName(ffmpeg)!, "ffprobe" + Path.GetExtension(ffmpeg));

        int tracks = 0;
        var failures = new List<string>();
        foreach (string name in FfmpegFiles)
        {
            string path = Path.Combine(root, name);
            if (!File.Exists(path))
                continue;

            var container = new Container();
            using var stream = File.OpenRead(path);
            container.Read(new IsoStream(new StreamWrapper(stream)));
            var reader = new VideoReader();
            reader.Parse(container);

            // ffmpeg's streams by the track ID they are of
            var streams = Run(ffprobe, "-v", "error", "-show_entries", "stream=index,id,codec_type", "-of", "csv=p=0", path)
                .Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim().Split(','))
                .Where(f => f.Length >= 3 && f[2].StartsWith("0x"))
                // index, codec_type and id, in ffprobe's order of them
                .ToDictionary(f => Convert.ToUInt32(f[2], 16), f => (Index: int.Parse(f[0]), Type: f[1]));

            foreach (var (trackID, info) in reader.Tracks)
            {
                if (info.Track is not ISubtitleTrack track || !streams.TryGetValue(trackID, out var s))
                    continue;
                tracks++;
                string at = $"{name} track {trackID}";

                var samples = new List<(MediaSample Sample, byte[] Data)>();
                for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
                    samples.Add((sample, sample.Data.ToArray()));

                // the packets: their times, in the track's timescale as ffmpeg's is for MP4, and their bytes
                var packets = Run(ffprobe, "-v", "error", "-select_streams", s.Index.ToString(), "-show_entries", "packet=pts,duration,size", "-of", "csv=p=0", path)
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim().Split(',')).ToList();
                if (packets.Count != samples.Count)
                {
                    failures.Add($"{at}: {samples.Count} samples, ffmpeg {packets.Count} packets");
                    continue;
                }
                byte[] data = RunBytes(ffmpeg, "-v", "error", "-i", path, "-map", $"0:{s.Index}", "-c", "copy", "-f", "data", "-");
                int offset = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    var (sample, bytes) = samples[i];
                    // ffprobe shows a duration of 0 as N/A
                    long pts = long.Parse(packets[i][0]), duration = long.TryParse(packets[i][1], out long d) ? d : 0;
                    // a last sample of no duration in the file ffmpeg gives one, to where the other tracks end
                    bool lastWithout = i == samples.Count - 1 && sample.Duration == 0;
                    if (sample.PTS != pts || (sample.Duration != duration && !lastWithout) || bytes.Length != int.Parse(packets[i][2]) ||
                        !data.AsSpan(offset, Math.Min(bytes.Length, data.Length - offset)).SequenceEqual(bytes))
                    {
                        failures.Add($"{at} sample {i}: {sample.PTS}+{sample.Duration}, {bytes.Length} bytes; ffmpeg {pts}+{duration}, {packets[i][2]} bytes");
                        break;
                    }
                    offset += bytes.Length;
                }

                if (track is TimedTextTrack)
                {
                    var expected = Srt(Run(ffmpeg, "-v", "error", "-i", path, "-map", $"0:{s.Index}", "-c:s", "text", "-f", "srt", "-"));
                    var cues = Cues(track, samples).Where(c => c.Text.Length > 0).ToList();
                    string Show(IEnumerable<(double Start, double End, string Text)> list) => string.Join(" | ", list.Select(c => $"{c.Start:0.000}-{c.End:0.000} {c.Text}"));
                    string ours = Show(cues.Select(c => (c.Start, c.End, c.Text.Replace("\r\n", "\n").TrimEnd('\n'))));
                    string theirs = Show(expected);
                    if (ours != theirs)
                        failures.Add($"{at}: cues {ours}; ffmpeg {theirs}");
                }
            }
        }

        Assert.IsTrue(tracks > 0, "no subtitle track read");
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    /// <summary>The cues of an SRT document: their times in seconds, to the millisecond, and their text.</summary>
    private static List<(double Start, double End, string Text)> Srt(string srt)
    {
        var cues = new List<(double, double, string)>();
        foreach (Match m in Regex.Matches(srt.Replace("\r\n", "\n"), @"\d+\n(\d+):(\d+):(\d+),(\d+) --> (\d+):(\d+):(\d+),(\d+)\n(.*?)(?:\n\n|\n?$)", RegexOptions.Singleline))
        {
            double Time(int g) => int.Parse(m.Groups[g].Value) * 3600 + int.Parse(m.Groups[g + 1].Value) * 60 + int.Parse(m.Groups[g + 2].Value) + int.Parse(m.Groups[g + 3].Value) / 1000.0;
            cues.Add((Time(1), Time(5), m.Groups[9].Value.TrimEnd('\n')));
        }
        return cues;
    }

    private static string Run(string exe, params string[] args) => System.Text.Encoding.UTF8.GetString(RunBytes(exe, args));

    private static byte[] RunBytes(string exe, params string[] args)
    {
        var start = new ProcessStartInfo(exe) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (string arg in args)
            start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        var output = new MemoryStream();
        var errors = process.StandardError.ReadToEndAsync();
        process.StandardOutput.BaseStream.CopyTo(output);
        process.WaitForExit();
        return output.ToArray();
    }
}
