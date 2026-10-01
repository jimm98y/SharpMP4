using SharpH26X;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using SharpMPEG4;
using System.Diagnostics;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// MPEG-4 Visual streams as ffmpeg's encoders - its own and Xvid's - make them, and FFmpeg's samples of it: there is no
/// conformance set to fetch, nor a trace of ffmpeg's to compare the headers with, so each unit is written back as it was,
/// each plane's type and size are those ffprobe gives it, and put in MP4 by <see cref="MPEG4VisualTrack"/> the stream
/// decodes to the same pictures.
/// </summary>
[TestClass]
public class MPEG4Tests
{
    private static (string Root, string Ffmpeg, IReadOnlyList<string> Streams) Corpus()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");
        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");
        var streams = ConformanceCorpus.Streams(root, "mpeg4");
        if (streams.Count == 0)
            Assert.Inconclusive($"no mpeg4 streams under {root}: ffmpeg's mpeg4 and libxvid encoders make them, raw, into mpeg4/ffmpeg");
        return (root, ffmpeg, streams);
    }

    /// <summary>Each unit written again by a context that has only written is the stream's, bit for bit.</summary>
    [TestMethod]
    public void WritesTheUnitsBackAsTheyWere()
    {
        var (root, _, streams) = Corpus();
        var failures = new List<string>();
        foreach (string path in streams)
        {
            byte[] data = File.ReadAllBytes(path);
            var reader = new MPEG4Context();
            var writer = new MPEG4Context();
            int index = 0;
            foreach (var (offset, length) in MPEG4Context.Units(data, 0, data.Length))
            {
                try
                {
                    var unit = reader.ReadUnit(data, offset, length);
                    var written = new MemoryStream();
                    var output = MPEG4Context.StreamOf(written, NullMp4Logger.Instance);
                    writer.WriteUnit(output, unit);
                    output.Dispose();
                    if (!written.ToArray().AsSpan().SequenceEqual(data.AsSpan(offset, length)))
                    {
                        failures.Add($"{Path.GetRelativePath(root, path)}: unit {index} ({unit.GetType().Name}) not written as it was");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    failures.Add($"{Path.GetRelativePath(root, path)}: unit {index} threw {ex.GetType().Name}: {ex.Message}");
                    break;
                }
                index++;
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// Each coded plane's type, read from its header, and the size of its layer are those ffprobe gives the picture - in the
    /// order ffprobe gives them, of display, B-VOPs before the plane they are predicted backwards from.
    /// </summary>
    [TestMethod]
    public void ReadsThePlaneHeadersAsFfmpegReadsThem()
    {
        var (root, ffmpeg, streams) = Corpus();
        var failures = new List<string>();
        foreach (string path in streams)
        {
            string name = Path.GetRelativePath(root, path);
            byte[] data = File.ReadAllBytes(path);
            var context = new MPEG4Context();
            var decoded = new List<(string Type, uint Width, uint Height)>();
            var unread = 0;
            foreach (var (offset, length) in MPEG4Context.Units(data, 0, data.Length))
            {
                var unit = context.ReadUnit(data, offset, length);
                if (unit is VideoObjectPlane plane && plane.VopCoded == 1)
                    decoded.Add(("IPBS"[(int)plane.VopCodingType].ToString(), context.VideoObjectLayer.VideoObjectLayerWidth, context.VideoObjectLayer.VideoObjectLayerHeight));
                else if (unit is UnknownUnit && data[offset + 3] == MPEG4StartCodes.VOP)
                    unread++;
            }
            if (context.IsStudio)
            {
                // the studio profiles' planes are not read
                continue;
            }
            if (NotOfTheirLayer.Contains(Path.GetFileName(path)))
                continue;

            var shown = DisplayOrder(decoded);
            var probed = Frames(ffmpeg, path);
            if (unread > 0)
                failures.Add($"{name}: {unread} planes not read");
            if (shown.Count != probed.Count)
                failures.Add($"{name}: {shown.Count} coded planes where ffprobe has {probed.Count} pictures");
            for (int i = 0; i < Math.Min(shown.Count, probed.Count); i++)
            {
                var (type, width, height) = shown[i];
                var (pictType, probedWidth, probedHeight) = probed[i];
                if (type != pictType || width != probedWidth || height != probedHeight)
                {
                    failures.Add($"{name}: picture {i} read as {type} {width}x{height}, ffprobe's {pictType} {probedWidth}x{probedHeight}");
                    break;
                }
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    // Streams whose planes are not of the syntax of their layer: of demo.m4v (libavcodec 54), vop_time_increment is of more
    // bits than vop_time_increment_resolution takes (6.3.5) - the marker bit after them 0 - which ffmpeg finds by guessing
    // the width until a marker bit is where it would be, and the syntax has no way to
    private static readonly HashSet<string> NotOfTheirLayer = new(StringComparer.OrdinalIgnoreCase) { "demo.m4v" };

    // The pictures of decoding order in display order: each I-, P- or S-VOP shown when the next of them is decoded, the
    // B-VOPs between as they are decoded
    private static List<(string Type, uint Width, uint Height)> DisplayOrder(List<(string Type, uint Width, uint Height)> decoded)
    {
        var shown = new List<(string, uint, uint)>();
        (string, uint, uint)? anchor = null;
        foreach (var picture in decoded)
        {
            if (picture.Type == "B")
            {
                shown.Add(picture);
                continue;
            }
            if (anchor != null)
                shown.Add(anchor.Value);
            anchor = picture;
        }
        if (anchor != null)
            shown.Add(anchor.Value);
        return shown;
    }

    /// <summary>
    /// Put in MP4 by the track, a stream decodes to the pictures it decodes to as it is; its sync samples are the I-VOPs, it
    /// reads back as a track of MPEG-4 Visual, and its pixel aspect ratio is the stream's.
    /// </summary>
    [TestMethod]
    public void PutsTheStreamsInMp4AsTheyDecode()
    {
        var (root, ffmpeg, streams) = Corpus();
        var failures = new List<string>();
        foreach (string path in streams)
        {
            string name = Path.GetRelativePath(root, path);
            string mp4 = Path.Combine(Path.GetTempPath(), $"sharpmp4-mpeg4-{Guid.NewGuid():N}.mp4");
            try
            {
                byte[] data = File.ReadAllBytes(path);
                var track = new MPEG4VisualTrack(30000, 1001);
                using (var output = File.Create(mp4))
                {
                    var builder = new Mp4Builder(new SingleStreamOutput(output));
                    builder.AddTrack(track);
                    foreach (var (offset, length) in MPEG4Context.Units(data, 0, data.Length))
                    {
                        track.ProcessSample(data, offset, length, out var sample, out bool isSync);
                        if (sample.Count > 0)
                            builder.ProcessRawSample(track.TrackID, sample, 1001, isSync);
                    }
                    track.ProcessSample(null!, 0, 0, out var last, out bool lastIsSync);
                    if (last.Count > 0)
                        builder.ProcessRawSample(track.TrackID, last, 1001, lastIsSync);
                    builder.FinalizeMedia();
                }

                var fromStream = FrameHashes(ffmpeg, "m4v", path);
                var fromFile = FrameHashes(ffmpeg, "mp4", mp4);
                if (fromStream.Count == 0 || !fromStream.SequenceEqual(fromFile))
                    failures.Add($"{name}: decoded to {fromFile.Count} pictures of the stream's {fromStream.Count}, or other ones");

                // ffmpeg's parser keys a plane by its type: an I-VOP not coded too, which is the plane before it again (6.3.5),
                // and no sync sample
                var notCoded = NotCodedPlanes(data);
                var streamKeys = Keys(ffmpeg, "m4v", path).Where(index => !notCoded.Contains(index)).ToList();
                var fileKeys = Keys(ffmpeg, "mp4", mp4);
                if (!NotOfTheirLayer.Contains(Path.GetFileName(path)) && !streamKeys.SequenceEqual(fileKeys))
                    failures.Add($"{name}: sync samples {string.Join(",", fileKeys)}, ffmpeg's key frames {string.Join(",", streamKeys)}");

                string streamAspect = Probe(ffmpeg, "m4v", path, "sample_aspect_ratio");
                string fileAspect = Probe(ffmpeg, "mp4", mp4, "sample_aspect_ratio");
                if (streamAspect != fileAspect)
                    failures.Add($"{name}: pixel aspect ratio {fileAspect} of the stream's {streamAspect}");

                using (var stream = File.OpenRead(mp4))
                {
                    var container = new Container();
                    container.Read(new IsoStream(new StreamWrapper(stream)));
                    var reader = new VideoReader();
                    reader.Parse(container);
                    var read = reader.Tracks.Values.Single().Track;
                    if (read is not MPEG4VisualTrack)
                        failures.Add($"{name}: read back as {read.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"{name}: threw {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                File.Delete(mp4);
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    private static string Ffprobe(string ffmpeg) => Path.Combine(Path.GetDirectoryName(ffmpeg)!, "ffprobe" + Path.GetExtension(ffmpeg));

    /// <summary>The type and size of each picture, as ffprobe gives them, in the order it gives them.</summary>
    private static List<(string PictType, uint Width, uint Height)> Frames(string ffmpeg, string path)
    {
        var frames = new List<(string, uint, uint)>();
        foreach (string line in Run(Ffprobe(ffmpeg), "-v", "error", "-threads", "1", "-f", "m4v", "-show_entries", "frame=pict_type,width,height", "-of", "csv=p=0", path))
        {
            var parts = line.Split(',');
            if (parts.Length >= 3)
                frames.Add((parts[2].Trim(), uint.Parse(parts[0]), uint.Parse(parts[1])));
        }
        return frames;
    }

    /// <summary>The indices of the planes of a stream that are not coded, vop_coded 0, among all its planes.</summary>
    private static HashSet<int> NotCodedPlanes(byte[] data)
    {
        var context = new MPEG4Context();
        var notCoded = new HashSet<int>();
        int index = 0;
        foreach (var (offset, length) in MPEG4Context.Units(data, 0, data.Length))
        {
            if (data[offset + 3] != MPEG4StartCodes.VOP)
            {
                context.ReadUnit(data, offset, length);
                continue;
            }
            if (context.ReadUnit(data, offset, length) is VideoObjectPlane { VopCoded: 0 })
                notCoded.Add(index);
            index++;
        }
        return notCoded;
    }

    /// <summary>The indices of the packets ffprobe gives as key frames: of the stream, its parser's I-VOPs; of MP4, the sync samples.</summary>
    private static List<int> Keys(string ffmpeg, string format, string path) =>
        Run(Ffprobe(ffmpeg), "-v", "error", "-f", format, "-select_streams", "v:0", "-show_entries", "packet=flags", "-of", "csv=p=0", path)
            .Select((flags, index) => (flags, index)).Where(p => p.flags.Contains('K')).Select(p => p.index).ToList();

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
