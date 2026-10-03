using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using SharpProRes;
using System.Diagnostics;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Apple ProRes in QuickTime files: FFmpeg's samples (fate/prores), files of ffmpeg's encoders of every profile, of fields
/// and of alpha (prores/ffmpeg), and a camera's (prores/apple). There is no trace of ffmpeg's to compare the headers with:
/// each frame is written back as it was and its slices add up to its pictures, its header says what ffprobe says of the
/// picture, and put in a file by <see cref="ProResTrack"/> the frames decode to the same pictures.
/// </summary>
[TestClass]
public class ProResTests
{
    // frames of a file read at most: a camera's run to hundreds of megabytes
    private const int MaxFrames = 30;

    private static (string Root, string Ffmpeg, IReadOnlyList<string> Files) Corpus()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");
        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");

        var files = new[] { Path.Combine(root, "prores"), Path.Combine(root, "fate", "prores") }
            .Where(Directory.Exists)
            .SelectMany(folder => Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            .Where(path => Path.GetExtension(path).Equals(".mov", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
        if (files.Count == 0)
            Assert.Inconclusive($"no ProRes files under {root}: fate/prores, and ffmpeg's prores_ks made into prores/ffmpeg");
        return (root, ffmpeg, files);
    }

    /// <summary>The ProRes track of a file, and its first frames, each an array of its own, with their durations.</summary>
    private static (ProResTrack Track, List<(byte[] Frame, int Duration)> Frames) Read(string path)
    {
        using var stream = File.OpenRead(path);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(stream)));
        using var reader = new VideoReader();
        reader.Parse(container);
        var (trackID, context) = reader.Tracks.First(t => t.Value.Track is ProResTrack);
        var frames = new List<(byte[], int)>();
        for (var sample = reader.ReadSample(trackID); sample != null && frames.Count < MaxFrames; sample = reader.ReadSample(trackID))
            frames.Add((sample.Data.ToArray(), sample.Duration));
        return ((ProResTrack)context.Track.Clone(), frames);
    }

    /// <summary>
    /// Each frame written again by a context that has only written is the file's, byte for byte; and of each picture the
    /// slices its slice table gives add up to what its picture_size leaves of it.
    /// </summary>
    [TestMethod]
    public void WritesTheFramesBackAsTheyWere()
    {
        var (root, _, files) = Corpus();
        var failures = new List<string>();
        foreach (string path in files)
        {
            string name = Path.GetRelativePath(root, path);
            var (track, frames) = Read(path);
            using (track)
            {
                var reader = new ProResContext();
                var writer = new ProResContext();
                for (int i = 0; i < frames.Count; i++)
                {
                    byte[] data = frames[i].Frame;
                    try
                    {
                        var frame = reader.ReadFrame(ProResContext.StreamOf(data, 0, data.Length));
                        foreach (var picture in new[] { frame.Picture, frame.Picture0 }.Where(p => p != null))
                        {
                            long slices = picture!.SliceTable.CodedSizeOfSlice.SelectMany(row => row).Sum(size => (long)size);
                            if (slices != picture.SliceData.Data.Length)
                                failures.Add($"{name}: frame {i}: slices of {slices} bytes, a picture of {picture.SliceData.Data.Length} after its slice table");
                        }

                        var written = new MemoryStream();
                        var output = ProResContext.StreamOf(written, NullMp4Logger.Instance);
                        writer.WriteFrame(output, frame);
                        output.Dispose();
                        if (!written.ToArray().AsSpan().SequenceEqual(data))
                        {
                            failures.Add($"{name}: frame {i} not written as it was");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        failures.Add($"{name}: frame {i} threw {ex.GetType().Name}: {ex.Message}");
                        break;
                    }
                }
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    // ffmpeg's names of the colour codes of H.273, which RDD 36's are (Tables 5 and 6): of those met
    private static readonly Dictionary<uint, string> Primaries = new() { [1] = "bt709", [5] = "bt470bg", [6] = "smpte170m", [9] = "bt2020", [11] = "smpte431", [12] = "smpte432" };
    private static readonly Dictionary<uint, string> Transfers = new() { [1] = "bt709", [6] = "smpte170m", [16] = "smpte2084", [18] = "arib-std-b67" };
    private static readonly Dictionary<uint, string> Matrices = new() { [1] = "bt709", [6] = "smpte170m", [9] = "bt2020nc" };

    /// <summary>
    /// Each file's first frame header says what ffprobe says of its picture: its size, its sampling - 4:2:2 or 4:4:4, of
    /// alpha or not - its fields and their order, and its colour where it says one.
    /// </summary>
    [TestMethod]
    public void ReadsTheFrameHeadersAsFfprobeReadsThem()
    {
        var (root, ffmpeg, files) = Corpus();
        var failures = new List<string>();
        foreach (string path in files)
        {
            string name = Path.GetRelativePath(root, path);
            var (track, frames) = Read(path);
            using (track)
            {
                var header = new ProResContext().ReadFrame(ProResContext.StreamOf(frames[0].Frame, 0, frames[0].Frame.Length)).FrameHeader;
                var probed = Probe(ffmpeg, path, "width,height,pix_fmt,color_primaries,color_transfer,color_space");

                // the field order of the decoded frame, which the decoder takes from the frame: a container's 'fiel' may say
                // otherwise - gray.mov's says two fields of frames of one
                var decoded = ProbeFrame(ffmpeg, path, "interlaced_frame,top_field_first");
                string chroma = header.ChromaFormat == 3 ? "444" : "422";
                string fieldOrder = header.InterlaceMode switch { 1 => "top first", 2 => "bottom first", _ => "progressive" };
                string probedFieldOrder = decoded["interlaced_frame"] == "0" ? "progressive" : decoded["top_field_first"] == "1" ? "top first" : "bottom first";
                var expected = new List<(string Name, string Ours, string Theirs)>
                {
                    ("width", header.HorizontalSize.ToString(), probed["width"]),
                    ("height", header.VerticalSize.ToString(), probed["height"]),
                    ("sampling", chroma + (header.AlphaChannelType != 0 ? " alpha" : ""), (probed["pix_fmt"].Contains("444") ? "444" : "422") + (probed["pix_fmt"].StartsWith("yuva") ? " alpha" : "")),
                    ("field order", fieldOrder, probedFieldOrder),
                };
                if (Primaries.TryGetValue(header.ColorPrimaries, out string? primaries) && probed["color_primaries"] != "unknown")
                    expected.Add(("primaries", primaries, probed["color_primaries"]));
                if (Transfers.TryGetValue(header.TransferCharacteristic, out string? transfer) && probed["color_transfer"] != "unknown")
                    expected.Add(("transfer", transfer, probed["color_transfer"]));
                if (Matrices.TryGetValue(header.MatrixCoefficients, out string? matrix) && probed["color_space"] != "unknown")
                    expected.Add(("matrix", matrix, probed["color_space"]));

                foreach (var (what, ours, theirs) in expected.Where(e => e.Ours != e.Theirs))
                    failures.Add($"{name}: {what} {ours}, ffprobe's {theirs}");
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// Put in a QuickTime file by a track made for writing - of the file's profile, its entry made of the frames - the
    /// frames decode to the pictures the file decodes to, as they are coded: not rotated by the display matrix of a
    /// camera's 'tkhd', nor cropped by the 'clap' of an entry, which are the file's and not the frames'. The file is
    /// QuickTime's, reads back as a track of ProRes of the profile, and its entry says the fields and the colour the
    /// frames do - where the file it came from may say none, or other.
    /// </summary>
    [TestMethod]
    public void PutsTheFramesInAFileAsTheyDecode()
    {
        var (root, ffmpeg, files) = Corpus();
        var failures = new List<string>();
        foreach (string path in files)
        {
            string name = Path.GetRelativePath(root, path);
            string written = Path.Combine(Path.GetTempPath(), $"sharpmp4-prores-{Guid.NewGuid():N}.mov");
            try
            {
                var (source, frames) = Read(path);
                using (source)
                using (var track = new ProResTrack(source.Profile, source.Timescale, frames[0].Duration))
                {
                    using (var output = File.Create(written))
                    using (var builder = new Mp4Builder(new SingleStreamOutput(output)) { FileFormat = Mp4FileFormat.QuickTime })
                    {
                        builder.AddTrack(track);
                        foreach (var (frame, duration) in frames)
                            builder.ProcessTrackSample(track.TrackID, frame, duration);
                        builder.FinalizeMedia();
                    }
                }

                var fromFile = FrameHashes(ffmpeg, path, frames.Count);
                var fromWritten = FrameHashes(ffmpeg, written, frames.Count);
                if (fromFile.Count == 0 || !fromFile.SequenceEqual(fromWritten))
                    failures.Add($"{name}: decoded to {fromWritten.Count} pictures of the file's {fromFile.Count}, or other ones");

                var header = new ProResContext().ReadFrame(ProResContext.StreamOf(frames[0].Frame, 0, frames[0].Frame.Length)).FrameHeader;
                var probedWritten = Probe(ffmpeg, written, "field_order,color_primaries,color_transfer,color_space,codec_tag_string");
                var expected = new Dictionary<string, string>
                {
                    ["codec_tag_string"] = Probe(ffmpeg, path, "codec_tag_string")["codec_tag_string"],
                    ["field_order"] = header.InterlaceMode switch { 1 => "tb", 2 => "bt", _ => "progressive" },
                    ["color_primaries"] = Primaries.TryGetValue(header.ColorPrimaries, out string? primaries) ? primaries : "unknown",
                    ["color_transfer"] = Transfers.TryGetValue(header.TransferCharacteristic, out string? transfer) ? transfer : "unknown",
                    ["color_space"] = Matrices.TryGetValue(header.MatrixCoefficients, out string? matrix) ? matrix : "unknown",
                };
                foreach (var (entry, value) in expected.Where(e => probedWritten[e.Key] != e.Value))
                    failures.Add($"{name}: {entry} {probedWritten[entry]}, of the frames {value}");

                using var stream = File.OpenRead(written);
                var container = new Container();
                container.Read(new IsoStream(new StreamWrapper(stream)));
                var ftyp = container.Children.OfType<FileTypeBox>().Single();
                if (IsoStream.ToFourCC(ftyp.MajorBrand) != "qt  ")
                    failures.Add($"{name}: written as '{IsoStream.ToFourCC(ftyp.MajorBrand)}'");
                using var reader = new VideoReader();
                reader.Parse(container);
                if (reader.Tracks.Values.Single().Track is not ProResTrack { } readBack || readBack.Profile != source.Profile)
                    failures.Add($"{name}: read back as {reader.Tracks.Values.Single().Track.GetType().Name}");
            }
            catch (Exception ex)
            {
                failures.Add($"{name}: threw {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                File.Delete(written);
            }
        }
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    private static string Ffprobe(string ffmpeg) => Path.Combine(Path.GetDirectoryName(ffmpeg)!, "ffprobe" + Path.GetExtension(ffmpeg));

    /// <summary>Entries of the first video stream, as ffprobe gives them.</summary>
    private static Dictionary<string, string> Probe(string ffmpeg, string path, string entries) =>
        Run(Ffprobe(ffmpeg), "-v", "error", "-select_streams", "v:0", "-show_entries", "stream=" + entries, "-of", "default=nw=1", path)
            .Select(line => line.Split('=', 2)).Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1].Trim());

    /// <summary>Entries of the first decoded frame of the first video stream, as ffprobe gives them.</summary>
    private static Dictionary<string, string> ProbeFrame(string ffmpeg, string path, string entries) =>
        Run(Ffprobe(ffmpeg), "-v", "error", "-select_streams", "v:0", "-read_intervals", "%+#1", "-show_entries", "frame=" + entries, "-of", "default=nw=1", path)
            .Select(line => line.Split('=', 2)).Where(parts => parts.Length == 2)
            .GroupBy(parts => parts[0]).ToDictionary(group => group.Key, group => group.First()[1].Trim());

    /// <summary>
    /// The MD5 of each of the first pictures ffmpeg decodes a file to, on one thread, as they are coded: not rotated by a
    /// display matrix nor cropped by a clean aperture.
    /// </summary>
    private static List<string> FrameHashes(string ffmpeg, string path, int frames) =>
        Run(ffmpeg, "-hide_banner", "-nostdin", "-v", "error", "-threads", "1", "-noautorotate", "-apply_cropping", "none", "-i", path, "-map", "0:v:0", "-frames:v", frames.ToString(), "-fps_mode", "passthrough", "-f", "framemd5", "-")
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
