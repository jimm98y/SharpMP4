using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System.Diagnostics;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// DTS as ffmpeg's encoder makes it - there is no conformance set of DTS in MP4 to fetch, nor an encoder of DTS-HD to make
/// one: put in a file by <see cref="DtsTrack"/>, each stream decodes to the audio its raw stream does, of the channels and
/// rate it has. And the 'ddts' of Chromium's files reads as Chromium's muxer wrote it.
/// </summary>
[TestClass]
public class DtsTests
{
    /// <summary>
    /// 5.1 at 48 kHz and stereo at 44.1 kHz, of ffmpeg's encoder: split into their frames, written by <see cref="DtsTrack"/>
    /// and decoded by ffmpeg, to the same audio as their raw streams, with no error, as 'dtsc' of their layout.
    /// </summary>
    [TestMethod]
    public void PutsTheStreamsInMp4AsTheyDecode()
    {
        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");

        string folder = Path.Combine(Path.GetTempPath(), $"sharpmp4-dts-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        try
        {
            var failures = new List<string>();
            foreach (var (name, rate, filter, layout) in new[]
            {
                ("5.1", 48000, "pan=5.1|c0=c0|c1=c0|c2=c0|c3=c0|c4=c0|c5=c0", "5.1(side)"),
                ("stereo", 44100, "pan=stereo|c0=c0|c1=c0", "stereo"),
            })
            {
                string raw = Path.Combine(folder, name + ".dts"), mp4 = Path.Combine(folder, name + ".mp4");
                Run(ffmpeg!, "-hide_banner", "-nostdin", "-v", "error", "-y", "-f", "lavfi", "-i", $"sine=f=440:d=1:r={rate}", "-af", filter,
                    "-c:a", "dca", "-strict", "-2", raw);
                byte[] data = File.ReadAllBytes(raw);
                var units = DtsTrack.ParseFrames(data, 0, data.Length).ToList();
                if (units.Sum(u => u.Count) != data.Length)
                    failures.Add($"{name}: {units.Sum(u => u.Count)} of the stream's {data.Length} bytes split into frames");

                var track = new DtsTrack(DtsFrame.Parse(units[0].Array!, units[0].Offset, units[0].Count));
                using (var output = File.Create(mp4))
                using (var builder = new Mp4Builder(new SingleStreamOutput(output)))
                {
                    builder.AddTrack(track);
                    foreach (var unit in units)
                        builder.ProcessTrackSample(track.TrackID, unit);
                    builder.FinalizeMedia();
                }

                string probed = string.Join(",", Run(Ffprobe(ffmpeg!), "-v", "error", "-show_entries", "stream=codec_name,sample_rate,channel_layout", "-of", "csv=p=0", mp4));
                if (probed != $"dts,{rate},{layout}")
                    failures.Add($"{name}: ffprobe reads {probed}, not dts,{rate},{layout}");
                string errors = string.Join(" ", RunErrors(ffmpeg!, "-hide_banner", "-nostdin", "-v", "error", "-i", mp4, "-f", "null", "-"));
                if (errors.Length > 0)
                    failures.Add($"{name}: ffmpeg decodes it with {errors}");
                if (Md5(ffmpeg!, raw) != Md5(ffmpeg!, mp4))
                    failures.Add($"{name}: it decodes to other audio than its raw stream");
            }
            Assert.AreEqual(0, failures.Count, string.Join("\n", failures));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    /// <summary>
    /// Chromium's 'dtsc' of 5.1 and 'dtse' of DTS Express - files of no samples, their sample entry the test - are of DTS
    /// tracks, read as Chromium's muxer wrote them: rate, frame duration, construction, layouts and core.
    /// </summary>
    [TestMethod]
    public void ReadsChromiumsSampleEntries()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null || !File.Exists(Path.Combine(root, "chromium", "bear_dtsc.mp4")))
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var dtsc = Read(Path.Combine(root!, "chromium", "bear_dtsc.mp4"));
        Assert.AreEqual(("dtsc", 48000u, 512, (byte)1, (ushort)0xF, (byte)6), (dtsc.Coding, dtsc.SamplingRate, dtsc.SamplesPerFrame, dtsc.StreamConstruction, dtsc.ChannelLayout, dtsc.ChannelCount));
        Assert.AreEqual(((byte)9, true, (ushort)1024, 768000u), (dtsc.CoreLayout, dtsc.CoreLfePresent, dtsc.CoreSize, dtsc.AvgBitRate));

        var dtse = Read(Path.Combine(root!, "chromium", "bear_dtse.mp4"));
        Assert.AreEqual(("dtse", 48000u, 4096, (byte)18, (byte)31, (ushort)0), (dtse.Coding, dtse.SamplingRate, dtse.SamplesPerFrame, dtse.StreamConstruction, dtse.CoreLayout, dtse.CoreSize));
    }

    private static DtsTrack Read(string path)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(File.ReadAllBytes(path)))));
        using var reader = new VideoReader();
        reader.Parse(container);
        return (DtsTrack)reader.GetTracks().Single().Clone();
    }

    private static string Ffprobe(string ffmpeg) =>
        Path.Combine(Path.GetDirectoryName(ffmpeg)!, Path.GetFileName(ffmpeg).Replace("ffmpeg", "ffprobe"));

    private static string Md5(string ffmpeg, string path) =>
        string.Join("", Run(ffmpeg, "-hide_banner", "-nostdin", "-v", "error", "-i", path, "-f", "md5", "-"));

    private static List<string> Run(string program, params string[] arguments) => Execute(program, arguments).Output;

    private static List<string> RunErrors(string program, params string[] arguments) => Execute(program, arguments).Errors;

    private static (List<string> Output, List<string> Errors) Execute(string program, string[] arguments)
    {
        var start = new ProcessStartInfo(program) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (string argument in arguments)
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var errors = process.StandardError.ReadToEndAsync();
        var lines = new List<string>();
        for (string? line = process.StandardOutput.ReadLine(); line != null; line = process.StandardOutput.ReadLine())
            lines.Add(line);
        process.WaitForExit();
        return (lines, errors.Result.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0).ToList());
    }
}
