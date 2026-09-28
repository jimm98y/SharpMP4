using System.Text.Json;

namespace SharpMP4.Tests.Conformance;

/// <summary>A box as GPAC's MP4Box dumps it: its type, its size and the boxes in it.</summary>
public sealed class DumpedBox
{
    public DumpedBox(string type, ulong? size, Dictionary<string, string> fields)
    {
        Type = type;
        Size = size;
        Fields = fields;
    }

    public string Type { get; }

    /// <summary>The box's size in bytes, header included; null where GPAC gives none.</summary>
    public ulong? Size { get; }

    /// <summary>Everything else GPAC gives about the box, by its name for it.</summary>
    public Dictionary<string, string> Fields { get; }

    public List<DumpedBox> Children { get; } = [];

    /// <summary>The box SharpMP4 read, of a tree made of what it read; null in GPAC's.</summary>
    public SharpISOBMFF.Box? Source { get; init; }
}

/// <summary>
/// Reads the *_gpac.json that MPEG's file format conformance files come with: MP4Box's XML dump
/// of every box, turned into JSON. An element is a box when it has a @Type. Elements of one name
/// were gathered into an array in the conversion, so the order of boxes of one type within their
/// parent is kept, but not how boxes of different types were interleaved.
/// </summary>
public static class GpacDump
{
    public static DumpedBox Read(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllBytes(path));
        var file = document.RootElement.GetProperty("IsoMediaFile");
        var root = new DumpedBox("file", null, []);
        Collect(file, root);
        return root;
    }

    /// <summary>The boxes in an element, found through whatever elements that are not boxes lie between.</summary>
    private static void Collect(JsonElement element, DumpedBox parent)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.StartsWith('@'))
                continue;

            // GPAC writes a text sample entry's font table into its DefaultBox as well as after it,
            // where the one 'ftab' there is.
            if (property.Name == "DefaultBox")
                continue;

            if (property.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in property.Value.EnumerateArray())
                    Add(item, parent);
            }
            else
            {
                Add(property.Value, parent);
            }
        }
    }

    private static void Add(JsonElement element, DumpedBox parent)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return;

        if (!element.TryGetProperty("@Type", out var type))
        {
            Collect(element, parent);
            return;
        }

        var fields = new Dictionary<string, string>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.StartsWith('@') && property.Value.ValueKind == JsonValueKind.String)
                fields[property.Name.Substring(1)] = property.Value.GetString()!;
        }

        ulong? size = fields.TryGetValue("Size", out string? text) && ulong.TryParse(text, out ulong value) ? value : null;
        var box = new DumpedBox(type.GetString() ?? "", size, fields);
        parent.Children.Add(box);
        Collect(element, box);
    }
}
