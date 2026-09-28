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

    /// <summary>
    /// The entries GPAC gives of a box - its sample table's SampleSizeEntry, its ftyp's BrandEntry - by the name of
    /// their element, each with its attributes, in order: the elements in the box's that are not boxes.
    /// </summary>
    public Dictionary<string, List<Dictionary<string, string>>> Entries { get; } = new(StringComparer.Ordinal);

    /// <summary>The same entries as they are nested: a senc's samples, each with its subsamples.</summary>
    public List<GpacEntry> EntryTree { get; } = [];

    /// <summary>The box SharpMP4 read, of a tree made of what it read; null in GPAC's.</summary>
    public SharpISOBMFF.Box? Source { get; init; }
}

/// <summary>An entry of a box as GPAC dumps it: its element's name, its attributes, and the entries in it.</summary>
public sealed record GpacEntry(string Name, Dictionary<string, string> Attributes)
{
    public List<GpacEntry> Children { get; } = [];

    /// <summary>The entries in it of a name, in order.</summary>
    public List<GpacEntry> Named(string name) => Children.Where(c => c.Name == name).ToList();
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
        Collect(file, root, null);
        return root;
    }

    /// <summary>
    /// The boxes in an element, found through whatever elements that are not boxes lie between, and those elements
    /// as the box's entries: in the entry the element is, if it is one.
    /// </summary>
    private static void Collect(JsonElement element, DumpedBox parent, GpacEntry? entry)
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
                    Add(property.Name, item, parent, entry);
            }
            else
            {
                Add(property.Name, property.Value, parent, entry);
            }
        }
    }

    private static void Add(string name, JsonElement element, DumpedBox parent, GpacEntry? entry)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return;

        if (!element.TryGetProperty("@Type", out var type))
        {
            // an entry of the box, and the boxes it may hold
            var attributes = Attributes(element);
            if (!parent.Entries.TryGetValue(name, out var entries))
                parent.Entries[name] = entries = [];
            entries.Add(attributes);
            var nested = new GpacEntry(name, attributes);
            (entry?.Children ?? parent.EntryTree).Add(nested);
            Collect(element, parent, nested);
            return;
        }

        var fields = Attributes(element);
        ulong? size = fields.TryGetValue("Size", out string? text) && ulong.TryParse(text, out ulong value) ? value : null;
        var box = new DumpedBox(type.GetString() ?? "", size, fields);
        parent.Children.Add(box);
        Collect(element, box, null);
    }

    /// <summary>An element's attributes, by their names without the @.</summary>
    private static Dictionary<string, string> Attributes(JsonElement element)
    {
        var attributes = new Dictionary<string, string>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.StartsWith('@') && property.Value.ValueKind == JsonValueKind.String)
                attributes[property.Name.Substring(1)] = property.Value.GetString()!;
        }
        return attributes;
    }
}
