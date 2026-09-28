using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// What a third-party tool - ffmpeg, ExifTool - made of a file, kept so that the tool runs once per file and
/// not on every test run. An entry is found by the tool's programs, its arguments and the file, each by path,
/// length and time of last write: a new build of the tool, other arguments or a file downloaded anew miss.
/// What is kept is the tool's output before the harness reads anything into it, so that changing the
/// harness - ffmpeg's aliases, say - needs no entry made again.
/// </summary>
/// <remarks>
/// The entries are in cache under the conformance folder, or under SHARPMP4_TOOL_CACHE if set; set to
/// "off" it runs the tools every time. An entry nothing finds any more is left behind: delete the folder
/// to be rid of them.
/// </remarks>
public static class ToolCache
{
    private static readonly Lazy<string?> Folder = new(() =>
    {
        string? configured = Environment.GetEnvironmentVariable("SHARPMP4_TOOL_CACHE");
        if (string.Equals(configured, "off", StringComparison.OrdinalIgnoreCase))
            return null;
        if (!string.IsNullOrEmpty(configured))
            return configured;

        string? root = ConformanceCorpus.Locate();
        return root == null ? null : Path.Combine(root, "cache");
    });

    /// <summary>
    /// Where the entry of a tool run on a file is, or null when nothing is cached.
    /// </summary>
    /// <param name="tool">The tool's name, and the folder its entries are in.</param>
    /// <param name="programs">What runs: the executable, and for a script the interpreter and the script.</param>
    /// <param name="arguments">The arguments other than the file, and the version of what the entry holds.</param>
    /// <param name="file">The file the tool is run on.</param>
    public static string? EntryOf(string tool, IEnumerable<string> programs, string arguments, string file)
    {
        if (Folder.Value == null)
            return null;

        var key = new StringBuilder();
        foreach (string program in programs.Append(file))
        {
            var info = new FileInfo(program);
            key.Append(info.FullName).Append('|').Append(info.Exists ? info.Length : -1).Append('|')
                .Append(info.Exists ? info.LastWriteTimeUtc.Ticks : 0).Append('\n');
        }
        key.Append(arguments);

        string hash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(key.ToString())));
        return Path.Combine(Folder.Value, tool, hash.Substring(0, 2), hash + ".gz");
    }

    /// <summary>Reads an entry, if there is one and it can be read.</summary>
    public static bool TryLoad<T>(string? entry, Func<BinaryReader, T> read, out T value)
    {
        value = default!;
        if (entry == null || !File.Exists(entry))
            return false;

        try
        {
            using var file = new FileStream(entry, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
            using var reader = new BinaryReader(new GZipStream(file, CompressionMode.Decompress), Encoding.UTF8);
            value = read(reader);
            return true;
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            // one cut short, by a run stopped as it wrote it: the tool runs again
            return false;
        }
    }

    /// <summary>Writes an entry, whole or not at all: a test run stopped partway leaves no half of one.</summary>
    public static void Save<T>(string? entry, T value, Action<BinaryWriter, T> write)
    {
        if (entry == null)
            return;

        Directory.CreateDirectory(Path.GetDirectoryName(entry)!);
        string partial = entry + "." + Guid.NewGuid().ToString("N") + ".part";
        try
        {
            using (var file = new FileStream(partial, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1 << 16))
            using (var writer = new BinaryWriter(new GZipStream(file, CompressionLevel.Fastest), Encoding.UTF8))
                write(writer, value);
            File.Move(partial, entry, overwrite: true);
        }
        finally
        {
            File.Delete(partial);
        }
    }

    /// <summary>The tool's output for a file, from its entry if there is one, from the tool if not.</summary>
    public static T GetOrRun<T>(string? entry, Func<T> run, Func<BinaryReader, T> read, Action<BinaryWriter, T> write)
    {
        if (TryLoad(entry, read, out T value))
            return value;

        value = run();
        Save(entry, value, write);
        return value;
    }
}
