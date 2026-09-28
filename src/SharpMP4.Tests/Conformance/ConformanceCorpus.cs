namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Where the conformance bitstreams and an ffmpeg to compare against are found. The bitstreams
/// come from DownloadConformance.ps1 at the root of the repository; ffmpeg has to be one recent
/// enough to trace H.266 and MV-HEVC headers.
/// </summary>
public static class ConformanceCorpus
{
    /// <summary>The extensions bitstreams are stored under, per codec folder.</summary>
    private static readonly Dictionary<string, string[]> Extensions = new()
    {
        ["h264"] = [".264", ".jsv", ".26l", ".h264", ".avc", ".jvt", ".bits"],
        ["h265"] = [".bit", ".bin"],
        ["h266"] = [".bit", ".bin", ".266", ".vvc"],
        ["av1"] = [".ivf", ".obu"],
    };

    /// <summary>
    /// The folder DownloadConformance.ps1 filled: SHARPMP4_CONFORMANCE if set, otherwise the
    /// conformance folder beside the script, found by walking up from the test assembly. Null when
    /// there is none.
    /// </summary>
    public static string? Locate()
    {
        string? configured = Environment.GetEnvironmentVariable("SHARPMP4_CONFORMANCE");
        if (!string.IsNullOrEmpty(configured))
            return Directory.Exists(configured) ? configured : null;

        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "DownloadConformance.ps1")))
            {
                string folder = Path.Combine(dir.FullName, "conformance");
                return Directory.Exists(folder) ? folder : null;
            }
        }

        return null;
    }

    /// <summary>
    /// An ffmpeg to trace headers with: SHARPMP4_FFMPEG (the executable or its folder) if set,
    /// otherwise the first one on the PATH. Null when there is none.
    /// </summary>
    public static string? LocateFfmpeg()
    {
        string executable = OperatingSystem.IsWindows() ? "ffmpeg.exe" : "ffmpeg";

        string? configured = Environment.GetEnvironmentVariable("SHARPMP4_FFMPEG");
        if (!string.IsNullOrEmpty(configured))
        {
            string candidate = Directory.Exists(configured) ? Path.Combine(configured, executable) : configured;
            return File.Exists(candidate) ? candidate : null;
        }

        foreach (var dir in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
        {
            if (string.IsNullOrWhiteSpace(dir))
                continue;

            string candidate = Path.Combine(dir.Trim(), executable);
            if (File.Exists(candidate))
                return candidate;
        }

        return null;
    }

    /// <summary>
    /// Every bitstream of a codec, in a stable order. SHARPMP4_CONFORMANCE_FILTER, if set, keeps
    /// only the paths that contain it - to look at one set or one stream.
    /// </summary>
    public static IReadOnlyList<string> Streams(string root, string codec)
    {
        string folder = Path.Combine(root, codec);
        if (!Directory.Exists(folder))
            return [];

        string? filter = Environment.GetEnvironmentVariable("SHARPMP4_CONFORMANCE_FILTER");
        var extensions = Extensions[codec];

        return Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
            .Where(path => extensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .Where(path => string.IsNullOrEmpty(filter) || path.Contains(filter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Every file format conformance file that has GPAC's dump of its boxes beside it, in a stable
    /// order: name_gpac.json goes with name.ext, or with name.ext\name.ext where the file came
    /// zipped. SHARPMP4_CONFORMANCE_FILTER narrows them as it does the bitstreams.
    /// </summary>
    public static IReadOnlyList<(string File, string Dump)> FileFormatFiles(string root)
    {
        string folder = Path.Combine(root, "isobmff");
        if (!Directory.Exists(folder))
            return [];

        string? filter = Environment.GetEnvironmentVariable("SHARPMP4_CONFORMANCE_FILTER");
        var files = new List<(string File, string Dump)>();
        foreach (string dump in Directory.EnumerateFiles(folder, "*_gpac.json", SearchOption.AllDirectories))
        {
            string directory = Path.GetDirectoryName(dump)!;
            string name = Path.GetFileName(dump)[..^"_gpac.json".Length];

            string? file = Directory.EnumerateFiles(directory, name + ".*")
                .FirstOrDefault(f => !f.EndsWith(".json", StringComparison.OrdinalIgnoreCase) && !f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
            file ??= Directory.EnumerateDirectories(directory, name + ".*")
                .Select(unzipped => Path.Combine(unzipped, Path.GetFileName(unzipped)))
                .FirstOrDefault(File.Exists);

            if (file != null && (string.IsNullOrEmpty(filter) || file.Contains(filter, StringComparison.OrdinalIgnoreCase)))
                files.Add((file, dump));
        }

        return files.OrderBy(f => f.File, StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// ExifTool as DownloadConformance.ps1 fetched it (tools\exiftool-*\exiftool), and a Perl to run it: SHARPMP4_PERL,
    /// the perl on the PATH or Git for Windows's. Null when either is missing.
    /// </summary>
    public static (string Perl, string ExifTool)? LocateExifTool(string root)
    {
        string tools = Path.Combine(root, "tools");
        string? exifTool = Directory.Exists(tools)
            ? Directory.EnumerateDirectories(tools, "exiftool-*").Select(d => Path.Combine(d, "exiftool")).FirstOrDefault(File.Exists)
            : null;
        if (exifTool == null)
            return null;

        string executable = OperatingSystem.IsWindows() ? "perl.exe" : "perl";
        var candidates = new List<string?> { Environment.GetEnvironmentVariable("SHARPMP4_PERL") };
        candidates.AddRange((Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator)
            .Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => Path.Combine(d.Trim(), executable)));
        if (OperatingSystem.IsWindows())
            candidates.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Git", "usr", "bin", "perl.exe"));
        string? perl = candidates.FirstOrDefault(c => !string.IsNullOrEmpty(c) && File.Exists(c));
        return perl == null ? null : (perl, exifTool);
    }

    /// <summary>
    /// The files whose metadata is compared: the metadata set, the FATE samples, the browsers' test files, the
    /// file format conformance files and the tests' metadata files. SHARPMP4_CONFORMANCE_FILTER narrows them as it does the others.
    /// </summary>
    public static IReadOnlyList<string> MetadataFiles(string root)
    {
        var files = MetadataSetFiles(root).ToList();
        files.AddRange(FateFiles(root));
        files.AddRange(ChromiumFiles(root));
        files.AddRange(FirefoxFiles(root));
        files.AddRange(FileFormatFiles(root).Select(f => f.File));
        // and the tests' own, which ExifTool wrote the tags of
        string testData = Path.Combine(AppContext.BaseDirectory, "TestData", "Metadata");
        if (Directory.Exists(testData))
            files.AddRange(Directory.EnumerateFiles(testData, "*.mp4"));
        return files.OrderBy(f => f, StringComparer.Ordinal).ToList();
    }

    /// <summary>The MP4 files of Chromium's media tests, in a stable order. SHARPMP4_CONFORMANCE_FILTER narrows them.</summary>
    public static IReadOnlyList<string> ChromiumFiles(string root) => FolderFiles(root, "chromium");

    /// <summary>The MP4 files of Firefox's media tests, in a stable order. SHARPMP4_CONFORMANCE_FILTER narrows them.</summary>
    public static IReadOnlyList<string> FirefoxFiles(string root) => FolderFiles(root, "firefox");

    /// <summary>Every file of a set's folder and its subfolders, in a stable order.</summary>
    private static IReadOnlyList<string> FolderFiles(string root, string name)
    {
        string folder = Path.Combine(root, name);
        if (!Directory.Exists(folder))
            return [];

        string? filter = Environment.GetEnvironmentVariable("SHARPMP4_CONFORMANCE_FILTER");
        return Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith(".part", StringComparison.OrdinalIgnoreCase))
            .Where(f => string.IsNullOrEmpty(filter) || f.Contains(filter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>The files of the metadata set, in a stable order. SHARPMP4_CONFORMANCE_FILTER narrows them.</summary>
    public static IReadOnlyList<string> MetadataSetFiles(string root)
    {
        string folder = Path.Combine(root, "metadata");
        if (!Directory.Exists(folder))
            return [];

        string? filter = Environment.GetEnvironmentVariable("SHARPMP4_CONFORMANCE_FILTER");
        return Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith(".part", StringComparison.OrdinalIgnoreCase))
            .Where(f => string.IsNullOrEmpty(filter) || f.Contains(filter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Every FATE sample that is an ISOBMFF or QuickTime file, in a stable order. Those that were not
    /// are listed in not-isobmff.txt, not kept. SHARPMP4_CONFORMANCE_FILTER narrows them.
    /// </summary>
    public static IReadOnlyList<string> FateFiles(string root)
    {
        string folder = Path.Combine(root, "fate");
        if (!Directory.Exists(folder))
            return [];

        string? filter = Environment.GetEnvironmentVariable("SHARPMP4_CONFORMANCE_FILTER");
        return Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) && !f.EndsWith(".part", StringComparison.OrdinalIgnoreCase))
            .Where(f => string.IsNullOrEmpty(filter) || f.Contains(filter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToList();
    }
}
