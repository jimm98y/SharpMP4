using System.Collections.Concurrent;
using System.Text;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Reads every conformance bitstream with SharpMP4 and with ffmpeg, and checks the two read the
/// same headers: every syntax element, its width and its value. Run DownloadConformance.ps1 first,
/// and have an ffmpeg recent enough to trace H.266 on the PATH or in SHARPMP4_FFMPEG; without
/// either the tests are inconclusive rather than failed.
/// </summary>
/// <remarks>
/// Each test writes conformance\report-&lt;codec&gt;.txt with every stream's result, and fails
/// with a summary of the streams that disagree, grouped by where they first do.
/// SHARPMP4_CONFORMANCE_FILTER narrows the run to paths containing it.
/// </remarks>
[TestClass]
public class ConformanceTests
{
    [TestMethod]
    public void H264HeadersReadAsFfmpegReadsThem() => Run("h264", _ => "h264", path => SharpTrace.ReadH26X(path, "h264"));

    [TestMethod]
    public void H265HeadersReadAsFfmpegReadsThem() => Run("h265", _ => "hevc", path => SharpTrace.ReadH26X(path, "h265"));

    [TestMethod]
    public void H266HeadersReadAsFfmpegReadsThem() => Run("h266", _ => "vvc", path => SharpTrace.ReadH26X(path, "h266"));

    [TestMethod]
    public void AV1HeadersReadAsFfmpegReadsThem() => Run("av1",
        _ => "ivf",
        SharpTrace.ReadAv1,
        // ffmpeg's demuxers for raw OBUs - Annex B or not - put each temporal unit together by
        // writing its headers anew, and in doing so even out what a stream coded the long way: a
        // delta_q of 0 coded, a leb128 padded. Argon Streams codes such things on purpose. Its
        // IVF demuxer passes frames on as they are, so ffmpeg is given the stream as IVF.
        path => Path.GetExtension(path).Equals(".ivf", StringComparison.OrdinalIgnoreCase) ? null : SharpTrace.ObusToIvf(path));

    /// <summary>
    /// Reads every file format conformance file and checks SharpMP4 finds the boxes GPAC's dump
    /// has, where it has them and as large. Needs no ffmpeg: the dumps come with the files.
    /// </summary>
    [TestMethod]
    public void IsoBmffBoxesReadAsGpacDumpsThem()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.FileFormatFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no file format conformance files under {root}");

        var results = new ConcurrentBag<StreamResult>();
        Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2) }, pair =>
        {
            StreamResult result;
            try
            {
                var (sharp, error) = BoxTreeComparison.ReadWithSharpMp4(pair.File);
                result = BoxTreeComparison.Compare(pair.File, sharp, error, GpacDump.Read(pair.Dump));
            }
            catch (Exception ex)
            {
                result = new StreamResult
                {
                    Path = pair.File,
                    Outcome = Outcome.SharpFailed,
                    Detail = ex.ToString(),
                    Key = $"harness: {ex.GetType().Name}",
                };
            }

            results.Add(result);
        });

        var ordered = results.OrderBy(r => r.Path, StringComparer.Ordinal).ToList();
        string summary = Summarise("isobmff", root, ordered);
        File.WriteAllText(Path.Combine(root, "report-isobmff.txt"), summary + Details(root, ordered));

        int failing = ordered.Count(r => r.Outcome is Outcome.Diverged or Outcome.SharpFailed);
        Assert.AreEqual(0, failing, summary);
    }

    /// <summary>
    /// Reads FFmpeg's ISOBMFF and QuickTime samples, which have the metadata and vendor boxes no
    /// specification describes, so nothing to compare them with: each file is checked against
    /// itself instead (see <see cref="BoxHealth"/>) and must write back byte for byte. The boxes
    /// SharpMP4 does not know are listed in the report, those in the most files first.
    /// </summary>
    [TestMethod]
    public void FateFilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.FateFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no FATE samples under {root}; run DownloadConformance.ps1 -Codec Fate");

        var unknown = new ConcurrentDictionary<string, ConcurrentBag<string>>();
        var results = new ConcurrentBag<StreamResult>();
        Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2) }, file =>
        {
            var result = BoxHealth.Check(file, unknown);
            if (result.Outcome != Outcome.SharpFailed)
            {
                try
                {
                    var roundTrip = RoundTrip.Check(file);
                    for (int i = 0; i < roundTrip.Keys.Count; i++)
                        result.Fail(roundTrip.Outcome, $"round trip: {roundTrip.Keys[i]}", roundTrip.Detail.Split('\n')[i].Trim());
                }
                catch (Exception ex)
                {
                    result.Fail(Outcome.SharpFailed, $"round trip: {ex.GetType().Name}", ex.Message);
                }
            }

            if (result.Keys.Count == 0)
                result.Outcome = Outcome.Match;
            else
                JudgeMalformed(root, result);
            results.Add(result);
        });

        // A known defect that is no longer found means the list, or the reading, has changed: either way it is looked at.
        foreach (var (file, defect) in MalformedFateFiles.SelectMany(m => m.Value.Defects.Select(d => (m.Key, d))))
        {
            var result = results.FirstOrDefault(r => Path.GetRelativePath(root, r.Path) == file);
            if (result != null && result.Outcome == Outcome.Match)
            {
                result.Fail(Outcome.Diverged, $"known defect not found: {defect}", $"{defect}: known in this file, but not found");
            }
        }

        var ordered = results.OrderBy(r => r.Path, StringComparer.Ordinal).ToList();
        string summary = Summarise("fate", root, ordered);

        var coverage = new StringBuilder();
        coverage.AppendLine();
        coverage.AppendLine($"Boxes SharpMP4 does not know, by the number of files they are in ({unknown.Count} kinds):");
        foreach (var kind in unknown.OrderByDescending(k => k.Value.Distinct().Count()).ThenBy(k => k.Key, StringComparer.Ordinal))
        {
            var where = kind.Value.Distinct().OrderBy(f => f, StringComparer.Ordinal).ToList();
            coverage.AppendLine($"  {where.Count,4} x {kind.Key}    e.g. {Path.GetRelativePath(root, where[0])}");
        }

        File.WriteAllText(Path.Combine(root, "report-fate.txt"), summary + coverage + Details(root, ordered));

        int failing = ordered.Count(r => r.Outcome is Outcome.Diverged or Outcome.SharpFailed);
        Assert.AreEqual(0, failing, summary);
    }

    /// <summary>
    /// FATE samples that are malformed on purpose, or by the tool that wrote them, with what is wrong in
    /// them: SharpMP4 is to find exactly that, keep it as it was, and write the file back byte for byte.
    /// </summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedFateFiles = new()
    {
        [Path.Combine("fate", "h264", "thezerotheorem-cut.mp4")] =
            ("an 8 byte 'box' of type 0x00000099 after the 'colr' of its 'avc1'", ["avc1/?: not a box type"]),
        [Path.Combine("fate", "mov", "invalid_elst_entry_count.mov")] =
            ("an 'elst' counting more entries than it holds", ["edts/elst: could not be read"]),
        [Path.Combine("fate", "qt-surge-suite", "surge-2-16-B-QDM2.mov")] =
            ("8 bytes after the terminator of its 'wave', that no box can be", ["wave/00000004: larger than its parent"]),
    };

    /// <summary>
    /// A file known to be malformed passes as <see cref="Outcome.Malformed"/> when it failed in no other
    /// way: SharpMP4 did not throw, found its known defects and nothing else, and wrote it back as it was.
    /// </summary>
    private static void JudgeMalformed(string root, StreamResult result)
    {
        if (!MalformedFateFiles.TryGetValue(Path.GetRelativePath(root, result.Path), out var malformed))
            return;

        if (result.Outcome == Outcome.SharpFailed || result.Keys.Any(k => k.StartsWith("round trip")))
            return;

        if (!result.Keys.ToHashSet().SetEquals(malformed.Defects))
            return;

        result.Outcome = Outcome.Malformed;
        result.Detail = $"malformed, and kept as it was: {malformed.Why}\n    " + result.Detail;
    }

    /// <summary>
    /// Reads every file format conformance file and writes it back, which must give the file again,
    /// byte for byte.
    /// </summary>
    [TestMethod]
    public void IsoBmffFilesWriteBackAsTheyWere()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.FileFormatFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no file format conformance files under {root}");

        var results = new ConcurrentBag<StreamResult>();
        Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2) }, pair =>
        {
            StreamResult result;
            try
            {
                result = RoundTrip.Check(pair.File);
            }
            catch (Exception ex)
            {
                result = new StreamResult
                {
                    Path = pair.File,
                    Outcome = Outcome.SharpFailed,
                    Detail = ex.ToString(),
                    Key = $"read: {ex.GetType().Name}",
                };
            }

            results.Add(result);
        });

        var ordered = results.OrderBy(r => r.Path, StringComparer.Ordinal).ToList();
        string summary = Summarise("isobmff round trip", root, ordered);
        File.WriteAllText(Path.Combine(root, "report-isobmff-roundtrip.txt"), summary + Details(root, ordered));

        int failing = ordered.Count(r => r.Outcome is Outcome.Diverged or Outcome.SharpFailed);
        Assert.AreEqual(0, failing, summary);
    }

    /// <param name="ffmpegFormat">ffmpeg's demuxer for a stream, by its path.</param>
    /// <param name="read">How SharpMP4 reads a stream.</param>
    /// <param name="ffmpegInput">What to give ffmpeg instead of the stream, if anything: a file it
    /// deletes afterwards.</param>
    private static void Run(string codec, Func<string, string> ffmpegFormat,
        Func<string, (List<TracedUnit> Units, Exception? Error)> read,
        Func<string, string?>? ffmpegInput = null)
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance bitstreams; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");

        var streams = ConformanceCorpus.Streams(root, codec);
        if (streams.Count == 0)
            Assert.Inconclusive($"no {codec} bitstreams under {root}");

        var results = new ConcurrentBag<StreamResult>();
        Parallel.ForEach(streams, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2) }, path =>
        {
            StreamResult result;
            try
            {
                var (sharp, error) = read(path);
                string? input = ffmpegInput?.Invoke(path);
                List<TracedUnit> reference;
                try
                {
                    reference = FfmpegTrace.Read(ffmpeg, input ?? path, ffmpegFormat(path));
                }
                finally
                {
                    if (input != null)
                        File.Delete(input);
                }
                result = TraceComparison.Compare(path, sharp, error, reference);
            }
            catch (Exception ex)
            {
                result = new StreamResult
                {
                    Path = path,
                    Outcome = Outcome.SharpFailed,
                    Detail = ex.ToString(),
                    Key = $"harness: {ex.GetType().Name}",
                };
            }

            results.Add(result);
        });

        var ordered = results.OrderBy(r => r.Path, StringComparer.Ordinal).ToList();
        string summary = Summarise(codec, root, ordered);
        File.WriteAllText(Path.Combine(root, $"report-{codec}.txt"), summary + Details(root, ordered));

        int failing = ordered.Count(r => r.Outcome is Outcome.Diverged or Outcome.SharpFailed);
        Assert.AreEqual(0, failing, summary);
    }

    private static string Summarise(string codec, string root, List<StreamResult> results)
    {
        var text = new StringBuilder();
        text.AppendLine($"{codec}: {results.Count} streams, " +
            $"{results.Count(r => r.Outcome == Outcome.Match)} read the same, " +
            $"{results.Count(r => r.Outcome == Outcome.Diverged)} diverge, " +
            $"{results.Count(r => r.Outcome == Outcome.SharpFailed)} throw, " +
            $"{results.Count(r => r.Outcome == Outcome.NoReference)} without a reference" +
            (results.Any(r => r.Outcome == Outcome.Malformed) ? $", {results.Count(r => r.Outcome == Outcome.Malformed)} malformed and kept as they were; " : "; ") +
            $"{results.Sum(r => r.FieldsCompared):N0} fields compared.");

        // A stream failing several ways counts under each.
        foreach (var group in results
            .Where(r => r.Outcome is Outcome.Diverged or Outcome.SharpFailed)
            .SelectMany(r => r.Keys.DefaultIfEmpty(r.Key).Select(key => (Key: key, Result: r)))
            .GroupBy(pair => pair.Key, pair => pair.Result)
            .OrderByDescending(g => g.Count()))
        {
            text.AppendLine($"  {group.Count(),4} x {group.Key}");
            foreach (var r in group.Take(3))
                text.AppendLine($"         {Path.GetRelativePath(root, r.Path)}");
        }

        return text.ToString();
    }

    private static string Details(string root, List<StreamResult> results)
    {
        var text = new StringBuilder();
        text.AppendLine();
        foreach (var r in results)
        {
            text.AppendLine($"{r.Outcome,-11} {Path.GetRelativePath(root, r.Path)}  " +
                $"({r.UnitsCompared} units, {r.FieldsCompared} fields, {r.UnitsUnpaired} unpaired)");
            if (r.Detail.Length > 0)
                text.AppendLine("    " + r.Detail);
        }

        return text.ToString();
    }
}
