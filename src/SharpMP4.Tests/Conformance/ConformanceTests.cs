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
    /// has, where it has them and as large, and the fields of each as GPAC gives them. Needs no
    /// ffmpeg: the dumps come with the files. GPAC's fields SharpMP4 has none of the name of are
    /// listed in the report, the most common first.
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
        var unpaired = new StringBuilder();
        unpaired.AppendLine();
        unpaired.AppendLine($"GPAC's fields SharpMP4 has none of the name of, by how often GPAC gives them ({BoxTreeComparison.Unpaired.Count} kinds):");
        foreach (var field in BoxTreeComparison.Unpaired.OrderByDescending(f => f.Value).ThenBy(f => f.Key, StringComparer.Ordinal))
            unpaired.AppendLine($"  {field.Value,6} x {field.Key}");
        File.WriteAllText(Path.Combine(root, "report-isobmff.txt"), summary + unpaired + Details(root, ordered));

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

        CheckFilesAgainstThemselves("fate", root, files, MalformedFateFiles);
    }

    /// <summary>
    /// Every AudioSpecificConfig (ISO/IEC 14496-3 1.6.2.1) of the MPEG-4 and MPEG-2 AAC audio in the FATE
    /// samples, the metadata set, Chromium's files and the file format conformance files, read as AACTrack
    /// reads it: it has to be read to its last bit, size to what it was read from, and write back byte for
    /// byte - its PCE, its SBR and PS signalling, an escaped object type, ALS's config, and what follows.
    /// </summary>
    [TestMethod]
    public void AudioSpecificConfigsReadAndWriteBackAsTheyWere()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.FateFiles(root)
            .Concat(ConformanceCorpus.MetadataSetFiles(root))
            .Concat(ConformanceCorpus.ChromiumFiles(root))
            .Concat(ConformanceCorpus.FirefoxFiles(root))
            .Concat(ConformanceCorpus.FileFormatFiles(root).Select(f => f.File))
            .ToList();
        if (files.Count == 0)
            Assert.Inconclusive($"no files under {root}; run DownloadConformance.ps1");

        var results = new ConcurrentBag<StreamResult>();
        Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2) }, path =>
        {
            List<byte[]> configs;
            try
            {
                configs = AudioConfigs(path);
            }
            catch (Exception)
            {
                return; // not a file this reads; the other tests say so
            }
            if (configs.Count == 0)
                return;

            var result = new StreamResult { Path = path };
            foreach (byte[] bytes in configs)
            {
                result.UnitsCompared++;
                string hex = Convert.ToHexString(bytes).ToLowerInvariant();
                var config = new SharpISOBMFF.AudioSpecificConfig();
                ulong read;
                try
                {
                    using var stream = new SharpISOBMFF.IsoStream(new MemoryStream(bytes));
                    read = config.Read(stream, (ulong)bytes.Length * 8);
                }
                catch (Exception ex)
                {
                    result.Fail(Outcome.Diverged, $"read: {ex.GetType().Name}", $"{hex}: {ex.GetType().Name}: {ex.Message}");
                    continue;
                }

                ulong bits = (ulong)bytes.Length * 8;
                if (read != bits)
                    result.Fail(Outcome.Diverged, "read short", $"{hex}: read {read} of {bits} bits");
                if (config.CalculateSize() != bits)
                    result.Fail(Outcome.Diverged, "calculated size", $"{hex}: calculates {config.CalculateSize()} of {bits} bits");

                using var written = new MemoryStream();
                using (var stream = new SharpISOBMFF.IsoStream(written))
                    config.Write(stream);
                if (!written.ToArray().AsSpan().SequenceEqual(bytes))
                    result.Fail(Outcome.Diverged, "writes back otherwise", $"{hex}: writes back {Convert.ToHexString(written.ToArray()).ToLowerInvariant()}");
            }

            if (result.Keys.Count == 0)
                result.Outcome = Outcome.Match;
            else
                JudgeMalformed(root, result, MalformedAudioConfigFiles);
            results.Add(result);
        });

        var ordered = results.OrderBy(r => r.Path, StringComparer.Ordinal).ToList();
        string summary = Summarise("aac", root, ordered);
        File.WriteAllText(Path.Combine(root, "report-aac.txt"), summary + Details(root, ordered));

        int failing = ordered.Count(r => r.Outcome is Outcome.Diverged or Outcome.SharpFailed);
        Assert.AreEqual(0, failing, summary);
    }

    /// <summary>Files whose AudioSpecificConfig is malformed, with what is wrong in it.</summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedAudioConfigFiles = new()
    {
        [Path.Combine("metadata", "mutagen", "ep7.m4b")] =
            ("a config of 2 bytes whose dependsOnCoreCoder says a coreCoderDelay of 14 bits follows, which it has not", ["read: EndOfStreamException"]),
    };

    /// <summary>
    /// The AudioSpecificConfigs of a file: the DecoderSpecificInfo of each DecoderConfigDescriptor of MPEG-4 audio
    /// (objectTypeIndication 0x40) or MPEG-2 AAC (0x66 to 0x68), as the file has it: each kept as its bytes,
    /// though SharpMP4 reads MPEG-4 audio's as an AudioSpecificConfig, so that it is checked against the file.
    /// </summary>
    private static List<byte[]> AudioConfigs(string path)
    {
        var container = new SharpISOBMFF.Container();
        using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
        var factory = new SharpISOBMFF.BoxFactory();
        factory.CreateDescriptor = (tag, objectTypeIndication, logger) => tag == SharpISOBMFF.DescriptorTags.DecSpecificInfoTag
            ? new SharpISOBMFF.GenericDecoderSpecificInfo()
            : SharpISOBMFF.BoxFactory.DefaultCreateDescriptor(tag, objectTypeIndication, logger);
        container.Read(new SharpISOBMFF.IsoStream(new SharpISOBMFF.StreamWrapper(file)) { BoxFactory = factory });

        var configs = new List<byte[]>();
        void Walk(IEnumerable<SharpISOBMFF.Box>? boxes)
        {
            foreach (var box in boxes ?? [])
            {
                if (box is SharpISOBMFF.ESDBox esds && esds._ES?.Children != null)
                {
                    foreach (var decoderConfig in esds._ES.Children.OfType<SharpISOBMFF.DecoderConfigDescriptor>())
                    {
                        if (decoderConfig.ObjectTypeIndication is not (0x40 or 0x66 or 0x67 or 0x68))
                            continue;
                        foreach (var info in decoderConfig.Children?.OfType<SharpISOBMFF.GenericDecoderSpecificInfo>() ?? [])
                        {
                            if (info.Data is { Length: > 0 })
                                configs.Add(info.Data);
                        }
                    }
                }
                Walk(box.Children);
            }
        }
        Walk(container.Children);
        return configs;
    }

    /// <summary>
    /// Reads the files of the metadata set as <see cref="FateFilesReadWithoutSignsOfMisreading"/> reads
    /// FFmpeg's: each checked against itself, and written back byte for byte.
    /// </summary>
    [TestMethod]
    public void MetadataFilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.MetadataSetFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no metadata files under {root}; run DownloadConformance.ps1 -Codec Metadata");

        CheckFilesAgainstThemselves("metadata-boxes", root, files, MalformedMetadataSetFiles);
    }

    /// <summary>
    /// Reads the MP4 files of Chromium's media tests - fragmented, segmented, encrypted, HDR, Dolby Vision,
    /// AC-4, and broken on purpose - as <see cref="FateFilesReadWithoutSignsOfMisreading"/> reads FFmpeg's.
    /// </summary>
    [TestMethod]
    public void ChromiumFilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.ChromiumFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no Chromium files under {root}; run DownloadConformance.ps1 -Codec Chromium");

        CheckFilesAgainstThemselves("chromium", root, files, MalformedChromiumFiles);
    }

    /// <summary>
    /// Reads the MP4 files of Firefox's media tests - DASH segments, encrypted, AV1, HEVC, and the files of
    /// bug reports and crash tests - as <see cref="FateFilesReadWithoutSignsOfMisreading"/> reads FFmpeg's.
    /// </summary>
    [TestMethod]
    public void FirefoxFilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.FirefoxFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no Firefox files under {root}; run DownloadConformance.ps1 -Codec Firefox");

        CheckFilesAgainstThemselves("firefox", root, files, MalformedFirefoxFiles);
    }

    /// <summary>
    /// Reads the AVIF files of libavif's tests - still images and their items, grids, alpha, gain maps,
    /// image sequences - as <see cref="FateFilesReadWithoutSignsOfMisreading"/> reads FFmpeg's.
    /// </summary>
    [TestMethod]
    public void LibavifFilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.LibavifFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no libavif files under {root}; run DownloadConformance.ps1 -Codec Libavif");

        CheckFilesAgainstThemselves("libavif", root, files, MalformedLibavifFiles);
    }

    /// <summary>libavif's test files that are malformed, with what is wrong in them, as <see cref="MalformedFateFiles"/>.</summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedLibavifFiles = new()
    {
    };

    /// <summary>
    /// Reads the AVIF specification's test files - Apple's, Link-U's, Microsoft's, Netflix's and Xiph's: still
    /// images, grids, alpha, HDR, image sequences - as <see cref="FateFilesReadWithoutSignsOfMisreading"/> reads FFmpeg's.
    /// </summary>
    [TestMethod]
    public void AvifFilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.AvifFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no AVIF test files under {root}; run DownloadConformance.ps1 -Codec Avif");

        CheckFilesAgainstThemselves("avif", root, files, MalformedAvifFiles);
    }

    /// <summary>The AVIF specification's test files that are malformed, with what is wrong in them, as <see cref="MalformedFateFiles"/>.</summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedAvifFiles = new()
    {
    };

    /// <summary>
    /// Reads the ISOBMFF files of Exiv2's tests - HEIF from cameras, AVIF, CR3, JPEG XL, video, and the files of
    /// security reports - as <see cref="FateFilesReadWithoutSignsOfMisreading"/> reads FFmpeg's.
    /// </summary>
    [TestMethod]
    public void Exiv2FilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.Exiv2Files(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no Exiv2 files under {root}; run DownloadConformance.ps1 -Codec Exiv2");

        CheckFilesAgainstThemselves("exiv2", root, files, MalformedExiv2Files);
    }

    /// <summary>
    /// Reads the MP4 files of Shaka Player's tests - DASH and HLS segments, CMAF text, encrypted files, AC-3 and
    /// E-AC-3, LCEVC - as <see cref="FateFilesReadWithoutSignsOfMisreading"/> reads FFmpeg's.
    /// </summary>
    [TestMethod]
    public void ShakaFilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.ShakaFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no Shaka Player files under {root}; run DownloadConformance.ps1 -Codec Shaka");

        CheckFilesAgainstThemselves("shaka", root, files, MalformedShakaFiles);
    }

    /// <summary>Shaka Player's test files that are malformed, with what is wrong in them, as <see cref="MalformedFateFiles"/>.</summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedShakaFiles = new()
    {
        [Path.Combine("shaka", "empty_caption_video_segment.mp4")] =
            ("its 'avcn' counts its one PPS as 0xE1, in the form of the SPS count, so 225 of them", ["traf/avcn: could not be read"]),
    };

    /// <summary>
    /// Reads the test files of mp4parse, Firefox's MP4 and AVIF parser - each made for a case, corrupt ones among
    /// them - as <see cref="FateFilesReadWithoutSignsOfMisreading"/> reads FFmpeg's.
    /// </summary>
    [TestMethod]
    public void Mp4parseFilesReadWithoutSignsOfMisreading()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.Mp4parseFiles(root);
        if (files.Count == 0)
            Assert.Inconclusive($"no mp4parse files under {root}; run DownloadConformance.ps1 -Codec Mp4parse");

        CheckFilesAgainstThemselves("mp4parse", root, files, MalformedMp4parseFiles);
    }

    /// <summary>mp4parse's test files that are malformed, with what is wrong in them, as <see cref="MalformedFateFiles"/>.</summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedMp4parseFiles = new()
    {
        [Path.Combine("mp4parse", "mp4parse", "clusterfuzz-testcase-minimized-mp4-6093954524250112")] =
            ("a ClusterFuzz test case of 56 bytes: a 'moov' whose boxes run past its end", ["file/moov: could not be read"]),
        [Path.Combine("mp4parse", "mp4parse", "corrupt", "bug-1655846.avif")] =
            ("fuzzed: a box of the 'meta' of type p9Dtm, and an 'iloc' declaring 4294639646 bytes",
             ["meta/?: not a box type", "meta/iloc: larger than its parent"]),
        [Path.Combine("mp4parse", "mp4parse", "corrupt", "clusterfuzz-testcase-minimized-avif-4914209301856256.avif")] =
            ("a ClusterFuzz test case: a property of type irFFFF", ["ipco/?: not a box type"]),
        [Path.Combine("mp4parse", "mp4parse", "invalid_userdata.mp4")] =
            ("its 'cprt' item holds 9 zero bytes, not a 'data' box", ["ilst/cprt: left over"]),
        [Path.Combine("mp4parse", "mp4parse", "unknown_mdat_in_oversized_meta.avif")] =
            ("a 'meta' larger than the file, an 'mdat' of size 0 in it, and fuzzed boxes in its track",
             ["trak/?: not a box type", "stbl/stsz: left over", "meta/mdat: larger than its parent"]),
        [Path.Combine("mp4parse", "mp4parse", "valid_with_garbage_byte.avif")] =
            ("a valid file with bytes of garbage after its last box", ["file: left over"]),
        [Path.Combine("mp4parse", "mp4parse", "valid_with_garbage_overread.avif")] =
            ("a valid file with garbage after it that declares 757935405 bytes", ["----/----: larger than its parent"]),
        [Path.Combine("mp4parse", "mp4parse", "wide_box_size_0.avif")] =
            ("fuzzed: a 'trak' declaring 1529508868 bytes in a 'moov' of 2120", ["moov/trak: larger than its parent"]),
        [Path.Combine("mp4parse", "mp4parse_capi", "no_timescale.mp4")] =
            ("its 'mvhd' renamed m\\x01hd, so that there is no timescale, and an 'hdlr' declaring 32545 bytes",
             ["moov/?: not a box type", "meta/hdlr: larger than its parent"]),
        [Path.Combine("mp4parse", "mp4parse_capi", "zero_empty_stsc.mp4")] =
            ("a DecoderConfigDescriptor declaring 23 bytes for its 20, the 3 after them the SLConfigDescriptor's",
             ["mp4a/esds/ES_Descriptor/4: could not be read"]),
    };

    /// <summary>Exiv2's test files that are malformed, with what is wrong in them, as <see cref="MalformedFateFiles"/>.</summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedExiv2Files = new()
    {
        [Path.Combine("exiv2", "issue_1793_poc.heic")] =
            ("a security report's file: a 'meta' of 0xFFFFFFFF bytes in a file of 100", ["file/meta: could not be read"]),
        [Path.Combine("exiv2", "issue_2340_poc.mp4")] =
            ("a security report's file: a 'pnot' of 32 bytes, 12 of them spaces its fields do not take", ["file/pnot: left over"]),
        [Path.Combine("exiv2", "issue_2345_poc.mp4")] =
            ("a security report's file: an 'ftyp' of 13 bytes, too few for its brand and version", ["file/ftyp: could not be read"]),
        [Path.Combine("exiv2", "issue_2393_poc.mp4")] =
            ("a security report's file: 2 bytes after its last box", ["file: left over"]),
        [Path.Combine("exiv2", "issue_2423_poc.mp4")] =
            ("a security report's file: an 'stsd' of 20 bytes counting 0xE9E9E9E9 entries, 4 bytes of it no box", ["file/stsd: left over"]),
        [Path.Combine("exiv2", "issue_ghsa_crmj_qh74_2r36_poc.mov")] =
            ("a security report's file: a box of type 0x80000000 among fuzzed ones", ["file/?: not a box type"]),
        [Path.Combine("exiv2", "issue_ghsa_fgw8_p7pr_37cp_poc.mov")] =
            ("a security report's file: an 'ftyp' of 12 bytes, too few for its brand and version, and fuzzed boxes after it",
             ["file/ftyp: could not be read", "file/?: not a box type"]),
    };

    /// <summary>Firefox's test files that are malformed, with what is wrong in them, as <see cref="MalformedFateFiles"/>.</summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedFirefoxFiles = new()
    {
        [Path.Combine("firefox", "crashtests", "1389304.mp4")] =
            ("a fuzzed file: the last 'sidx' declares 0x0400002C bytes, one bit flipped from 0x2C", ["file/sidx: left over"]),
        [Path.Combine("firefox", "crashtests", "1833894.mp4")] =
            ("a fuzzed file: 'tfhd' and a box header in the file overwritten", ["traf/?: not a box type", "file/?: not a box type"]),
        [Path.Combine("firefox", "crashtests", "1845350.mp4")] =
            ("a fuzzed file: 'udta' misspelled 'udC6a'", ["moov/?: not a box type"]),
        [Path.Combine("firefox", "crashtests", "1859600.mp4")] =
            ("a fuzzed file: an 'hvcC' whose SPS declares 7983 bytes in a box of 2453", ["hev1/hvcC: could not be read"]),
        [Path.Combine("firefox", "crashtests", "encrypted-track-with-bad-sample-description-index.mp4")] =
            ("a fuzzed file: an 'mehd' declaring 33296 bytes, and a 'moof' misspelled 'moFFf'",
             ["mvex/mehd: larger than its parent", "file/?: not a box type"]),
        [Path.Combine("firefox", "crashtests", "encrypted-track-with-sample-missing-cenc-aux.mp4")] =
            ("a fuzzed file: a 'saio' of version 0xC0, which no version is, read as 64 bit offsets it has no room for", ["traf/saio: could not be read"]),
        [Path.Combine("firefox", "crashtests", "encrypted-track-without-tenc.mp4")] =
            ("'tenc' renamed '02enc', to test a track without it", ["schi/?: not a box type"]),
        [Path.Combine("firefox", "crashtests", "mp4_box_emptyrange.mp4")] =
            ("a fuzzed file: an 'elst' and a 'meta' child declaring more than their parents have, and boxes overwritten",
             ["edts/elst: larger than its parent", "meta/?: not a box type", "meta/000000FF: larger than its parent", "file/?: not a box type"]),
        [Path.Combine("firefox", "crashtests", "small-timebase.mp4")] =
            ("a fuzzed file: a descriptor in the 'esds', and an item list 'data', declaring more than their parents have, and 'stsd' misspelled 'sd00s'",
             ["mp4a/esds/ES_Descriptor/UnknownDescriptor: larger than its parent", "stbl/?: not a box type", "00o00t/data: larger than its parent"]),
    };

    /// <summary>
    /// Chromium's test files that are malformed on purpose, with what is wrong in them, as
    /// <see cref="MalformedFateFiles"/>.
    /// </summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedChromiumFiles = new()
    {
        [Path.Combine("chromium", "bear-1280x720-av_frag-initsegment-mvhd_version_0-mvhd_duration_bits_all_set.mp4")] =
            ("an init segment cut 5 bytes into the box after its 'moov'", ["file: left over"]),
        [Path.Combine("chromium", "blackwhite_yuv420p_rec709.mp4")] =
            ("an 'avcC' of 8 bytes, its header and nothing else", ["avc1/avcC: could not be read"]),
        [Path.Combine("chromium", "duplicate_track_id.mp4")] =
            ("a fuzzed file: 'trun's that overlap the boxes after them, and bytes of text where boxes are",
             ["traf/1455: larger than its parent", "traf/trun: left over", "file/?: not a box type"]),
    };

    private static void CheckFilesAgainstThemselves(string name, string root, IReadOnlyList<string> files, Dictionary<string, (string Why, string[] Defects)> malformedFiles)
    {
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
                JudgeMalformed(root, result, malformedFiles);
            results.Add(result);
        });

        // A known defect that is no longer found means the list, or the reading, has changed: either way it is looked at.
        foreach (var (file, defect) in malformedFiles.SelectMany(m => m.Value.Defects.Select(d => (m.Key, d))))
        {
            var result = results.FirstOrDefault(r => Path.GetRelativePath(root, r.Path) == file);
            if (result != null && result.Outcome == Outcome.Match)
            {
                result.Fail(Outcome.Diverged, $"known defect not found: {defect}", $"{defect}: known in this file, but not found");
            }
        }

        var ordered = results.OrderBy(r => r.Path, StringComparer.Ordinal).ToList();
        string summary = Summarise(name, root, ordered);

        var coverage = new StringBuilder();
        coverage.AppendLine();
        coverage.AppendLine($"Boxes SharpMP4 does not know, by the number of files they are in ({unknown.Count} kinds):");
        foreach (var kind in unknown.OrderByDescending(k => k.Value.Distinct().Count()).ThenBy(k => k.Key, StringComparer.Ordinal))
        {
            var where = kind.Value.Distinct().OrderBy(f => f, StringComparer.Ordinal).ToList();
            coverage.AppendLine($"  {where.Count,4} x {kind.Key}    e.g. {Path.GetRelativePath(root, where[0])}");
        }

        File.WriteAllText(Path.Combine(root, $"report-{name}.txt"), summary + coverage + Details(root, ordered));

        int failing = ordered.Count(r => r.Outcome is Outcome.Diverged or Outcome.SharpFailed);
        Assert.AreEqual(0, failing, summary);
    }

    /// <summary>
    /// FATE samples that are malformed on purpose, or by the tool that wrote them, with what is wrong in
    /// them: SharpMP4 is to find exactly that, keep it as it was, and write the file back byte for byte.
    /// </summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedFateFiles = new()
    {
        [Path.Combine("fate", "cineform", "cineform_yuv10b_hd.mov")] =
            ("a stereo 'chan' counting no channel descriptions, then 40 zeros: room for the two it does not count", ["sowt/chan: left over"]),
        [Path.Combine("fate", "h264", "thezerotheorem-cut.mp4")] =
            ("an 8 byte 'box' of type 0x00000099 after the 'colr' of its 'avc1'", ["avc1/?: not a box type"]),
        [Path.Combine("fate", "mov", "invalid_elst_entry_count.mov")] =
            ("an 'elst' counting more entries than it holds", ["edts/elst: could not be read"]),
        [Path.Combine("fate", "mov", "mp4-with-mov-in24-ver.mp4")] =
            ("'tcmi' misspelled 'tmci', with 2 zero bytes after its font name", ["tmcd/tmci: left over"]),
        [Path.Combine("fate", "qt-surge-suite", "surge-2-16-B-QDM2.mov")] =
            ("8 bytes after the terminator of its 'wave', that no box can be", ["wave/00000004: larger than its parent"]),
    };

    /// <summary>
    /// Files of the metadata set that are malformed on purpose, or by the tool that wrote them, with what is
    /// wrong in them, as <see cref="MalformedFateFiles"/>.
    /// </summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedMetadataSetFiles = new()
    {
        // ExifTool's test images keep the metadata, and little of the rest
        [Path.Combine("metadata", "exiftool", "CanonRaw.cr3")] =
            ("a 'PRVW' declaring the 372221 bytes of the preview it no longer has", ["uuid/PRVW: larger than its parent"]),
        [Path.Combine("metadata", "exiftool", "QuickTime.m4a")] =
            ("an 'stco' counting no chunks, then the zeros of the 44 offsets it had", ["stbl/stco: left over"]),
        [Path.Combine("metadata", "exiftool", "QuickTime.mov")] =
            ("an 'stco' counting no chunks, then the zeros of the 5 offsets it had", ["stbl/stco: left over"]),
        // mutagen's and TagLib's test files, each made for the defect it has
        [Path.Combine("metadata", "mutagen", "64bit.mp4")] =
            ("boxes of 64-bit sizes whose 'ilst' declares more than its 'meta' holds, and 8 bytes after the 'moov'", ["meta/ilst: larger than its parent", "file/?: not a box type"]),
        [Path.Combine("metadata", "taglib", "64bit.mp4")] =
            ("boxes of 64-bit sizes whose 'ilst' declares more than its 'meta' holds, and 8 bytes after the 'moov'", ["meta/ilst: larger than its parent", "file/?: not a box type"]),
        [Path.Combine("metadata", "mutagen", "nero-chapters.m4b")] =
            ("an 'stsz' of 8 bytes, its header and nothing else", ["stbl/stsz: could not be read"]),
        [Path.Combine("metadata", "taglib", "covr-junk.m4a")] =
            ("junk in its cover art item: a 'name' of 4 zeros after its version and flags", ["covr/name: left over"]),
        [Path.Combine("metadata", "taglib", "infloop.m4a")] =
            ("a 'data' of size 0 in its 'gnre', which loops a reader that steps by the size", ["gnre/data: larger than its parent"]),
        [Path.Combine("metadata", "mutagen", "ep7.m4b")] =
            ("an AudioSpecificConfig of 2 bytes whose dependsOnCoreCoder says a coreCoderDelay of 14 bits follows, which it has not",
             ["mp4a/esds/ES_Descriptor/DecoderConfigDescriptor/5: could not be read"]),
    };

    /// <summary>
    /// A file known to be malformed passes as <see cref="Outcome.Malformed"/> when it failed in no other
    /// way: SharpMP4 did not throw, found its known defects and nothing else, and wrote it back as it was.
    /// </summary>
    private static void JudgeMalformed(string root, StreamResult result, Dictionary<string, (string Why, string[] Defects)> malformedFiles)
    {
        if (!malformedFiles.TryGetValue(Path.GetRelativePath(root, result.Path), out var malformed))
            return;

        if (result.Outcome == Outcome.SharpFailed || result.Keys.Any(k => k.StartsWith("round trip")))
            return;

        if (!result.Keys.ToHashSet().SetEquals(malformed.Defects))
            return;

        result.Outcome = Outcome.Malformed;
        result.Detail = $"malformed, and kept as it was: {malformed.Why}\n    " + result.Detail;
    }

    /// <summary>
    /// Reads the metadata tags of the metadata set, the FATE samples and the file format conformance files -
    /// iTunes items, freeform items, keyed metadata and the strings of 'udta' - and checks SharpMP4 reads the
    /// ones ExifTool reads, with their values. Needs the Metadata set of DownloadConformance.ps1, which
    /// brings ExifTool, and a Perl to run it.
    /// </summary>
    [TestMethod]
    public void MetadataReadsAsExifToolReadsIt()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var exifTool = ConformanceCorpus.LocateExifTool(root);
        if (exifTool == null)
            Assert.Inconclusive("no ExifTool or no Perl; run DownloadConformance.ps1 -Codec Metadata, or set SHARPMP4_PERL");

        var files = ConformanceCorpus.MetadataFiles(root);
        var theirs = ExifToolTrace.Read(exifTool.Value.Perl, exifTool.Value.ExifTool, files);
        var macintosh = ExifToolTrace.MacintoshLanguages(exifTool.Value.ExifTool);
        var xmpPrefixes = ExifToolTrace.XmpPrefixes(exifTool.Value.ExifTool);

        var results = new ConcurrentBag<StreamResult>();
        Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2) }, path =>
        {
            StreamResult result;
            try
            {
                // Open while compared: what a box keeps as it was, it reads from the file when written
                var container = new SharpISOBMFF.Container();
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
                container.Read(new SharpISOBMFF.IsoStream(new SharpISOBMFF.StreamWrapper(stream)));
                var tags = SharpMP4.Readers.MetadataReader.Read(container);
                result = MetadataComparison.Compare(path, tags, theirs.TryGetValue(Path.GetFullPath(path), out var t) ? t : [], macintosh, xmpPrefixes);
            }
            catch (Exception ex)
            {
                result = new StreamResult { Path = path, Outcome = Outcome.SharpFailed, Detail = ex.ToString(), Key = $"read: {ex.GetType().Name}" };
            }
            JudgeMalformedMetadata(root, result);
            results.Add(result);
        });

        var ordered = results.OrderBy(r => r.Path, StringComparer.Ordinal).ToList();
        string summary = Summarise("metadata", root, ordered);

        // Every tag that is not read as ExifTool reads it, by how many files it is in
        var tally = new StringBuilder("\nEvery tag not read as ExifTool reads it, by the files it is in:\n");
        foreach (var group in ordered.SelectMany(r => r.Keys).GroupBy(k => k).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal))
            tally.Append($"  {group.Count(),5}  {group.Key}\n");
        File.WriteAllText(Path.Combine(root, "report-metadata.txt"), summary + tally + Details(root, ordered));

        int failing = ordered.Count(r => r.Outcome is Outcome.Diverged or Outcome.SharpFailed);
        Assert.AreEqual(0, failing, summary);
    }

    /// <summary>
    /// Metadata test files that are malformed on purpose, where ExifTool reads less than SharpMP4, with what
    /// SharpMP4 reads that ExifTool does not.
    /// </summary>
    private static readonly Dictionary<string, (string Why, string[] Defects)> MalformedMetadataFiles = new()
    {
        [Path.Combine("metadata", "taglib", "non-full-meta.m4a")] =
            ("its 'meta' is not a full box, as QuickTime's is not: ExifTool takes the first box for version and flags, finds it truncated and reads none of the items",
             ["extra movie Freeform:iTunNORM", "extra movie ItemList:covr", "extra movie ItemList:©ART", "extra movie ItemList:©too"]),
        [Path.Combine("mp4parse", "mp4parse", "invalid_userdata.mp4")] =
            ("a 'cprt' item of 9 zero bytes, no 'data' box: ExifTool gives it as a tag of no value, SharpMP4 as no tag",
             ["missing movie ItemList:cprt"]),
    };

    private static void JudgeMalformedMetadata(string root, StreamResult result)
    {
        if (!MalformedMetadataFiles.TryGetValue(Path.GetRelativePath(root, result.Path), out var malformed))
            return;
        if (result.Outcome == Outcome.SharpFailed || !result.Keys.ToHashSet().SetEquals(malformed.Defects))
            return;
        result.Outcome = Outcome.Malformed;
        result.Detail = $"malformed, and read: {malformed.Why}\n    " + result.Detail;
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

    /// <summary>
    /// Reads every AV1 conformance stream and writes each OBU again with a context that has only
    /// written, which must give the stream's bytes: its syntax elements, their lengths - a leb128
    /// padded, a uvlc long - and the tile data. Needs no ffmpeg.
    /// </summary>
    [TestMethod]
    public void AV1ObusWriteBackAsTheyWere()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance bitstreams; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var streams = ConformanceCorpus.Streams(root, "av1");
        if (streams.Count == 0)
            Assert.Inconclusive($"no av1 bitstreams under {root}");

        var results = new ConcurrentBag<StreamResult>();
        Parallel.ForEach(streams, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2) }, path =>
        {
            StreamResult result;
            try
            {
                result = AomRoundTrip.CheckAv1(path);
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
        string summary = Summarise("av1 round trip", root, ordered);
        var recordOnly = new StringBuilder("\nOnly in the record - elements the state they were read into does not give back:\n");
        foreach (var (name, total) in AomRoundTrip.RecordOnly.OrderByDescending(e => e.Value.Streams))
            recordOnly.Append($"  {name}: {total.Occurrences} occurrences in {total.Streams} streams\n");
        File.WriteAllText(Path.Combine(root, "report-av1-roundtrip.txt"), summary + recordOnly + Details(root, ordered));

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
                var reference = FfmpegTrace.Read(ffmpeg, path, ffmpegFormat(path), ffmpegInput);
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
