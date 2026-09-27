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
