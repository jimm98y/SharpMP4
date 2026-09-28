using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SharpMP4.Tests.Conformance;

/// <summary>A tag as ExifTool reads it: its group (family 1), name, id and value.</summary>
public sealed record ExifToolTag(string Group, string Name, string Id, string Value);

/// <summary>
/// Runs ExifTool - a program of its own, run by Perl - over files, and reads its JSON: every tag with its
/// group, every copy of it (-a, -G1:4), its id (-H) and its value unconverted where ExifTool does not convert
/// it anyway (-n), binary values as base64 (-b); and the XMP packet as it is (-XMP).
/// </summary>
public static partial class ExifToolTrace
{
    // --MediaData: not the 'mdat's too, as base64 - tens of megabytes of a file, and in a group not compared
    private static readonly string[] Arguments = ["-j", "-a", "-u", "-G1:4", "-H", "-n", "-b", "-q", "-q", "-all", "--MediaData", "-XMP", "-charset", "filename=utf8"];

    /// <summary>
    /// The tags of each file, as ExifTool read them the last time it was given the file (<see cref="ToolCache"/>),
    /// or as it reads them now.
    /// </summary>
    public static Dictionary<string, List<ExifToolTag>> Read(string perl, string exifTool, IReadOnlyList<string> files)
    {
        // What ExifTool wrote of each file, its object of the JSON; empty where it wrote nothing
        var objects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var entries = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var missing = new List<string>();
        foreach (string file in files.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            // The arguments count in finding an entry, and so does the version of what it holds
            string? entry = entries[file] = ToolCache.EntryOf("exiftool", [perl, exifTool], "json 1 " + string.Join(' ', Arguments), file);
            if (ToolCache.TryLoad(entry, reader => reader.ReadString(), out string json))
                objects[file] = json;
            else
                missing.Add(file);
        }

        // In batches, each a Perl of its own: ExifTool's start takes longer than a file
        foreach (var batch in missing.Chunk(200))
        {
            var written = Run(perl, exifTool, batch);
            if (written == null)
                continue;

            foreach (string file in batch)
            {
                string json = written.TryGetValue(file, out string? text) ? text : "";
                objects[file] = json;
                ToolCache.Save(entries[file], json, (writer, value) => writer.Write(value));
            }
        }

        var result = new Dictionary<string, List<ExifToolTag>>(StringComparer.OrdinalIgnoreCase);
        foreach (var (path, json) in objects)
        {
            if (json.Length == 0)
                continue;

            using var document = JsonDocument.Parse(json);
            var tags = new List<ExifToolTag>();
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Object || !property.Value.TryGetProperty("id", out var id))
                    continue;
                // Group:Copy1:Name - a tag found again is a copy (family 4), else JSON keeps only the last
                string[] parts = property.Name.Split(':');
                string group = parts.Length > 1 ? parts[0] : "";
                string name = parts[^1];
                tags.Add(new ExifToolTag(group, name, IdText(id), ValueText(property.Value.GetProperty("val"))));
            }
            result[path] = tags;
        }
        return result;
    }

    /// <summary>
    /// Runs ExifTool over files, and gives each file's object of its JSON by the file's full path; null when
    /// ExifTool wrote nothing at all.
    /// </summary>
    private static Dictionary<string, string>? Run(string perl, string exifTool, string[] files)
    {
        string list = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(list, files, new UTF8Encoding(false));
            var start = new ProcessStartInfo(perl)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8,
            };
            foreach (var argument in new[] { exifTool }.Concat(Arguments).Concat(["-@", list]))
                start.ArgumentList.Add(argument);
            using var process = Process.Start(start)!;
            var errors = process.StandardError.ReadToEndAsync();
            string json = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            _ = errors.Result;
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var objects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using var document = JsonDocument.Parse(EscapeControlCharacters(json));
            foreach (var file in document.RootElement.EnumerateArray())
                objects[Path.GetFullPath(file.GetProperty("SourceFile").GetString()!)] = file.GetRawText();
            return objects;
        }
        finally
        {
            File.Delete(list);
        }
    }

    /// <summary>An id as the atom type it is: "base64:qW5hbQ==" as ©nam, its bytes taken as ISO 8859-1.</summary>
    private static string IdText(JsonElement id)
    {
        string text = id.ValueKind == JsonValueKind.String ? id.GetString()! : id.GetRawText();
        return text.StartsWith("base64:", StringComparison.Ordinal) ? Encoding.Latin1.GetString(Convert.FromBase64String(text.Substring(7))) : text;
    }

    private static string ValueText(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString()!,
        // an XMP boolean, True or False, JSON gives as true or false
        JsonValueKind.True => "True",
        JsonValueKind.False => "False",
        JsonValueKind.Array => string.Join(" ", value.EnumerateArray().Select(ValueText)),
        _ => value.GetRawText(),
    };

    /// <summary>
    /// ExifTool's JSON with the control characters it leaves raw in strings escaped: a string of a file
    /// may hold any byte, and JSON takes none below 0x20 as it is.
    /// </summary>
    private static string EscapeControlCharacters(string json)
    {
        var text = new StringBuilder(json.Length);
        bool inString = false, escaped = false;
        foreach (char c in json)
        {
            if (inString)
            {
                if (escaped)
                    escaped = false;
                else if (c == '\\')
                    escaped = true;
                else if (c == '"')
                    inString = false;
                else if (c < 0x20)
                {
                    text.Append("\\u").Append(((int)c).ToString("x4"));
                    continue;
                }
            }
            else if (c == '"')
            {
                inString = true;
            }
            text.Append(c);
        }
        return text.ToString();
    }

    /// <summary>
    /// ExifTool's Macintosh language codes (Font.pm, %ttLang Macintosh): what it calls a QuickTime string in
    /// such a language. Read from the ExifTool it runs, so that the comparison names languages as it does.
    /// </summary>
    public static Dictionary<int, string> MacintoshLanguages(string exifTool)
    {
        string font = Path.Combine(Path.GetDirectoryName(exifTool)!, "lib", "Image", "ExifTool", "Font.pm");
        var languages = new Dictionary<int, string>();
        if (!File.Exists(font))
            return languages;
        string text = File.ReadAllText(font);
        int start = text.IndexOf("Macintosh => {", text.IndexOf("%ttLang", StringComparison.Ordinal), StringComparison.Ordinal);
        int end = text.IndexOf('}', start);
        foreach (Match m in MacintoshEntry().Matches(text.Substring(start, end - start)))
            languages[int.Parse(m.Groups[1].Value)] = m.Groups[2].Value;
        return languages;
    }

    /// <summary>
    /// ExifTool's prefixes of XMP namespaces (XMP.pm, %nsURI, shortened as %stdXlatNS shortens them), by
    /// namespace URI: its XMP-* group of a property is the prefix it gives the namespace, not the packet's.
    /// Read from the ExifTool it runs.
    /// </summary>
    public static Dictionary<string, string> XmpPrefixes(string exifTool)
    {
        string xmp = Path.Combine(Path.GetDirectoryName(exifTool)!, "lib", "Image", "ExifTool", "XMP.pm");
        var prefixes = new Dictionary<string, string>(StringComparer.Ordinal);
        if (!File.Exists(xmp))
            return prefixes;
        string text = File.ReadAllText(xmp);
        int start = text.IndexOf("%nsURI = (", StringComparison.Ordinal);
        int end = text.IndexOf("\n);", start, StringComparison.Ordinal);
        foreach (Match m in NamespaceEntry().Matches(text.Substring(start, end - start)))
            prefixes.TryAdd(m.Groups[2].Value, m.Groups[1].Value);

        var shorter = new Dictionary<string, string>(StringComparer.Ordinal);
        start = text.IndexOf("%stdXlatNS = (", StringComparison.Ordinal);
        end = text.IndexOf("\n);", start, StringComparison.Ordinal);
        foreach (Match m in NamespaceEntry().Matches(text.Substring(start, end - start)))
            shorter[m.Groups[1].Value] = m.Groups[2].Value;
        foreach (var uri in prefixes.Keys.ToList())
            if (shorter.TryGetValue(prefixes[uri], out string? prefix))
                prefixes[uri] = prefix;
        return prefixes;
    }

    [GeneratedRegex(@"(\d+)\s*=>\s*'([^']+)'")]
    private static partial Regex MacintoshEntry();

    [GeneratedRegex(@"^\s*'?([\w-]+)'?\s*=>\s*'([^']+)'", RegexOptions.Multiline)]
    private static partial Regex NamespaceEntry();
}
