using System.Reflection;
using SharpISOBMFF;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Compares the boxes SharpMP4 reads out of a file with GPAC's dump of it: which boxes there are,
/// where, and how large. Within a parent, boxes are paired by type in the order they come, as the
/// dump keeps only that order.
/// </summary>
public static class BoxTreeComparison
{
    /// <summary>Reads a file with SharpMP4 into the same shape as a dump.</summary>
    public static (DumpedBox Root, Exception? Error) ReadWithSharpMp4(string path)
    {
        var root = new DumpedBox("file", null, []);
        var container = new Container();
        Exception? error = null;
        try
        {
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
            container.Read(new IsoStream(new StreamWrapper(file)));
        }
        catch (Exception ex)
        {
            // What was read before the failure is compared all the same.
            error = ex;
        }

        Add(WithinFile(container.Children, (ulong)new FileInfo(path).Length), root);
        return (root, error);
    }

    /// <summary>
    /// The top-level boxes that end within the file. GPAC leaves out one that runs past the end of
    /// a file cut short, as the last 'mdat' of green\video_2500000bps_0.mp4 does.
    /// </summary>
    private static List<Box> WithinFile(List<Box>? boxes, ulong length)
    {
        var within = new List<Box>();
        ulong end = 0;
        foreach (var box in boxes ?? [])
        {
            if (box.Header != null && box.Header.Size != 0)
                end += box.Header.GetBoxSizeInBits() >> 3;
            if (end > length)
                break;
            within.Add(box);
        }

        return within;
    }

    /// <summary>Boxes GPAC reads but leaves out of its dump, by the type of their parent.</summary>
    private static readonly HashSet<(string Parent, string Type)> NotDumped =
    [
        ("hinf", "maxr"),
    ];

    /// <summary>
    /// Boxes GPAC keeps only one of in a parent, dumping that one alone: a sample entry's 'colr'
    /// (nalu\hevc\hev1_clg1_header.mp4 has two) and a user data box's 'strk' (sg-tl-st.mp4 has two).
    /// </summary>
    private static readonly HashSet<string> OneKept = ["colr", "strk"];

    private static void Add(List<Box>? boxes, DumpedBox parent)
    {
        if (boxes == null)
            return;

        foreach (var box in boxes)
        {
            ulong? size = null;
            if (box.Header != null && box.Header.Size != 0)
                size = box.Header.GetBoxSizeInBits() >> 3;

            var node = new DumpedBox(TypeName(box.FourCC), size, []);
            parent.Children.Add(node);
            Add(BoxFields(box), node);
            Add(box.Children, node);
        }
    }

    /// <summary>A box type as GPAC writes it: a byte outside ASCII in hex, '©swr' as A9swr.</summary>
    internal static string TypeName(uint fourCC)
    {
        var name = new System.Text.StringBuilder();
        for (int shift = 24; shift >= 0; shift -= 8)
        {
            byte b = (byte)(fourCC >> shift);
            name.Append(b >= 0x80 || b < 0x20 ? b.ToString("X2") : ((char)b).ToString());
        }
        return name.ToString();
    }

    /// <summary>
    /// The boxes a box keeps in fields of its own - an iref's references, a meta's handler - which
    /// the syntax reads one by one rather than into its children.
    /// </summary>
    internal static List<Box> BoxFields(Box box)
    {
        var found = new List<Box>();
        for (var type = box.GetType(); type != null && type != typeof(Box); type = type.BaseType)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                object? value = field.GetValue(box);
                if (value is Box single && !ReferenceEquals(single, box))
                    found.Add(single);
                else if (value is IEnumerable<Box> many && field.Name != "children")
                    found.AddRange(many.Where(b => b != null));
            }
        }

        return found;
    }

    public static StreamResult Compare(string path, DumpedBox sharp, Exception? error, DumpedBox gpac)
    {
        var result = new StreamResult { Path = path };
        CompareChildren(sharp, gpac, "", result);

        if (error != null)
        {
            result.Fail(Outcome.SharpFailed, $"{error.GetType().Name}",
                $"threw: {error.GetType().Name}: {error.Message}");
        }

        if (result.Keys.Count == 0)
            result.Outcome = Outcome.Match;
        return result;
    }

    private static void CompareChildren(DumpedBox sharp, DumpedBox gpac, string where, StreamResult result)
    {
        var types = gpac.Children.Select(b => b.Type).Concat(sharp.Children.Select(b => b.Type)).Distinct();
        foreach (string type in types)
        {
            var ours = sharp.Children.Where(b => b.Type == type).ToList();
            var theirs = gpac.Children.Where(b => b.Type == type).ToList();
            string at = where.Length == 0 ? type : $"{where}/{type}";

            if (theirs.Count == 0 && NotDumped.Contains((sharp.Type, type)))
                continue;

            // Of several, pair GPAC's one with the one of ours as large, or else the last.
            if (theirs.Count == 1 && ours.Count > 1 && OneKept.Contains(type))
                ours = [ours.LastOrDefault(b => b.Size == theirs[0].Size) ?? ours[^1]];

            for (int i = 0; i < Math.Min(ours.Count, theirs.Count); i++)
            {
                result.UnitsCompared++;
                // GPAC gives 0 for a box it does not parse - a sample entry of a type it does not know.
                if (ours[i].Size != null && theirs[i].Size is ulong gpacSize && gpacSize != 0)
                {
                    result.FieldsCompared++;
                    if (ours[i].Size != theirs[i].Size)
                    {
                        result.Fail(Outcome.Diverged, $"{at}: size",
                            $"{at}[{i}]: GPAC reads {theirs[i].Size} bytes, SharpMP4 {ours[i].Size}");
                    }
                }

                CompareChildren(ours[i], theirs[i], at, result);
            }

            if (ours.Count != theirs.Count)
            {
                result.UnitsUnpaired += Math.Abs(ours.Count - theirs.Count);
                string key = theirs.Count > ours.Count ? $"{at}: missing" : $"{at}: extra";
                result.Fail(Outcome.Diverged, key,
                    $"{at}: GPAC reads {theirs.Count}, SharpMP4 {ours.Count}");
            }
        }
    }
}
