using SharpH261;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System.Diagnostics;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// H.261 streams as ffmpeg makes them - there is no conformance set to fetch, nor a trace of ffmpeg's to compare the
/// headers with: each picture is written back as it was, its size and whether it releases a frozen picture - which ffmpeg
/// takes for a key frame - are those ffprobe gives it, and put in a file by <see cref="H261Track"/> the stream decodes to
/// the same pictures.
/// </summary>
[TestClass]
public class H261Tests
{
    private static (string Root, string Ffmpeg, IReadOnlyList<string> Streams) Corpus()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");
        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");
        var streams = ConformanceCorpus.Streams(root, "h261");
        if (streams.Count == 0)
            Assert.Inconclusive($"no h261 streams under {root}: ffmpeg's h261 encoder makes them, raw, into h261/ffmpeg");
        return (root, ffmpeg, streams);
    }

    /// <summary>Each picture written again by a context that has only written is the stream's, bit for bit.</summary>
    [TestMethod]
    public void WritesThePicturesBackAsTheyWere()
    {
        var (root, _, streams) = Corpus();
        var failures = new List<string>();
        foreach (string path in streams)
        {
            byte[] data = File.ReadAllBytes(path);
            var reader = new H261Context();
            var writer = new H261Context();
            int index = 0;
            foreach (var (offset, length) in H261Context.Pictures(data, 0, data.Length))
            {
                var picture = reader.ReadPicture(H261Context.StreamOf(data, offset, length));
                var written = new MemoryStream();
                var output = H261Context.StreamOf(written, NullMp4Logger.Instance);
                writer.WritePicture(output, picture);
                output.Dispose();
                if (!written.ToArray().AsSpan().SequenceEqual(data.AsSpan(offset, length)))
                {
                    failures.Add($"{Path.GetRelativePath(root, path)}: picture {index} not written as it was");
                    break;
                }
                index++;
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// Each picture's size, read from its header, is the one ffprobe gives it, and it releases a frozen picture where ffprobe
    /// gives it as a key frame.
    /// </summary>
    [TestMethod]
    public void ReadsThePictureHeadersAsFfmpegReadsThem()
    {
        var (root, ffmpeg, streams) = Corpus();
        var failures = new List<string>();
        foreach (string path in streams)
        {
            string name = Path.GetRelativePath(root, path);
            var probed = Frames(ffmpeg, path);
            byte[] data = File.ReadAllBytes(path);
            var context = new H261Context();
            var pictures = H261Context.Pictures(data, 0, data.Length);
            if (pictures.Count != probed.Count)
                failures.Add($"{name}: {pictures.Count} pictures where ffprobe has {probed.Count}");

            for (int i = 0; i < Math.Min(pictures.Count, probed.Count); i++)
            {
                var picture = context.ReadPicture(H261Context.StreamOf(data, pictures[i].Offset, pictures[i].Length));
                bool released = picture.FreezePictureRelease == 1;
                var (key, width, height) = probed[i];
                if (released != key || context.Width != width || context.Height != height)
                {
                    failures.Add($"{name}: picture {i} read as {(released ? "a release" : "no release")} {context.Width}x{context.Height}, ffprobe's {(key ? "key" : "not key")} {width}x{height}");
                    break;
                }
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// Put in a file by the track, a stream decodes to the pictures it decodes to as it is; its sync samples are ffmpeg's
    /// key frames, it reads back as a track of H.261, and its samples are of 12:11.
    /// </summary>
    [TestMethod]
    public void PutsTheStreamsInAFileAsTheyDecode()
    {
        var (root, ffmpeg, streams) = Corpus();
        var failures = new List<string>();
        foreach (string path in streams)
        {
            string name = Path.GetRelativePath(root, path);
            string file = Path.Combine(Path.GetTempPath(), $"sharpmp4-h261-{Guid.NewGuid():N}.mov");
            try
            {
                byte[] data = File.ReadAllBytes(path);
                var sync = new List<bool>();
                var track = new H261Track(30000, 1001);
                using (var output = File.Create(file))
                {
                    var builder = new Mp4Builder(new SingleStreamOutput(output));
                    builder.AddTrack(track);
                    foreach (var (offset, length) in H261Context.Pictures(data, 0, data.Length))
                    {
                        track.ProcessSample(data, offset, length, out var sample, out bool isSync);
                        sync.Add(isSync);
                        builder.ProcessRawSample(track.TrackID, sample, 1001, isSync);
                    }
                    builder.FinalizeMedia();
                }

                var probed = Frames(ffmpeg, path);
                for (int i = 0; i < Math.Min(sync.Count, probed.Count); i++)
                {
                    if (sync[i] != probed[i].Key)
                        failures.Add($"{name}: sample {i} {(sync[i] ? "a" : "not a")} sync sample, ffprobe's picture {(probed[i].Key ? "key" : "not key")}");
                }

                var fromStream = FrameHashes(ffmpeg, "h261", path);
                var fromFile = FrameHashes(ffmpeg, "mov", file);
                if (fromStream.Count == 0 || !fromStream.SequenceEqual(fromFile))
                    failures.Add($"{name}: decoded to {fromFile.Count} pictures of the stream's {fromStream.Count}, or other ones");

                string aspect = Probe(ffmpeg, "mov", file, "sample_aspect_ratio");
                if (aspect != "12:11")
                    failures.Add($"{name}: pixel aspect ratio {aspect}, not CIF's 12:11");

                using (var stream = File.OpenRead(file))
                {
                    var container = new Container();
                    container.Read(new IsoStream(new StreamWrapper(stream)));
                    var reader = new VideoReader();
                    reader.Parse(container);
                    var read = reader.Tracks.Values.Single().Track;
                    if (read is not H261Track)
                        failures.Add($"{name}: read back as {read.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"{name}: threw {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                File.Delete(file);
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    private static string Ffprobe(string ffmpeg) => Path.Combine(Path.GetDirectoryName(ffmpeg)!, "ffprobe" + Path.GetExtension(ffmpeg));

    /// <summary>Whether each picture is a key frame, and its size, as ffprobe gives them, in the order of the stream.</summary>
    private static List<(bool Key, int Width, int Height)> Frames(string ffmpeg, string path)
    {
        var frames = new List<(bool, int, int)>();
        foreach (string line in Run(Ffprobe(ffmpeg), "-v", "error", "-f", "h261", "-show_entries", "frame=key_frame,width,height", "-of", "csv=p=0", path))
        {
            // ffprobe gives them in its own order: key_frame, width, height
            var parts = line.Split(',');
            if (parts.Length >= 3)
                frames.Add((parts[0].Trim() == "1", int.Parse(parts[1]), int.Parse(parts[2])));
        }
        return frames;
    }

    private static string Probe(string ffmpeg, string format, string path, string entry) =>
        Run(Ffprobe(ffmpeg), "-v", "error", "-f", format, "-show_entries", "stream=" + entry, "-of", "csv=p=0", path).FirstOrDefault()?.Trim() ?? "";

    /// <summary>The MD5 of each picture ffmpeg decodes a file to, on one thread, frame for frame.</summary>
    private static List<string> FrameHashes(string ffmpeg, string format, string path) =>
        Run(ffmpeg, "-hide_banner", "-nostdin", "-v", "error", "-threads", "1", "-f", format, "-i", path, "-map", "0:v:0", "-fps_mode", "passthrough", "-f", "framemd5", "-")
            .Where(line => !line.StartsWith('#')).Select(line => line.Split(',').Last().Trim()).ToList();

    private static List<string> Run(string program, params string[] arguments)
    {
        var start = new ProcessStartInfo(program) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (string argument in arguments)
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        process.StandardError.ReadToEndAsync();
        var lines = new List<string>();
        for (string? line = process.StandardOutput.ReadLine(); line != null; line = process.StandardOutput.ReadLine())
            lines.Add(line);
        process.WaitForExit();
        return lines;
    }
}
