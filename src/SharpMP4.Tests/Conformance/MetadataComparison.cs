using System.Globalization;
using System.Text.RegularExpressions;
using SharpISOBMFF;
using SharpMP4.Readers;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Compares the metadata tags SharpMP4 reads (<see cref="MetadataReader"/>) with those ExifTool reads: the
/// iTunes items, the freeform items, keyed metadata and the strings of 'udta', of the movie and its tracks.
/// A tag is its scope (movie or track), family, id and language; the values of a tag are compared as a
/// set, each as ExifTool gives it.
/// </summary>
public static partial class MetadataComparison
{
    /// <summary>The families of ExifTool's groups that are compared, and which scope each is.</summary>
    private static (string Scope, string Family)? Family(string group)
    {
        if (group == "ItemList") return ("movie", "ItemList");
        if (group.EndsWith("ItemList", StringComparison.Ordinal)) return ("track", "ItemList");
        if (group == "iTunes") return ("movie", "Freeform");
        if (group == "Keys") return ("movie", "Keys");
        if (group.EndsWith("Keys", StringComparison.Ordinal)) return ("track", "Keys");
        if (group == "UserData") return ("movie", "UserData");
        if (group.EndsWith("UserData", StringComparison.Ordinal)) return ("track", "UserData");
        // the packet; the properties in it are the XMP-* groups
        if (group == "XMP") return ("file", "Xmp");
        return null;
    }

    /// <summary>
    /// Atoms of 'udta' that ExifTool reads into groups of their own, not as a UserData tag: SharpMP4 gives
    /// them as their bytes, which ExifTool does not.
    /// </summary>
    private static readonly Dictionary<string, string> ReadIntoOtherGroups = new(StringComparer.Ordinal)
    {
        ["TAGS"] = "Pentax, FujiFilm and the like",
        ["Xtra"] = "Microsoft",
        ["ptv "] = "Video: PrintToVideo",
    };

    /// <summary>Freeform items ExifTool reads into groups of their own, not as an iTunes tag.</summary>
    private static readonly Dictionary<string, string> FreeformReadIntoOtherGroups = new(StringComparer.Ordinal)
    {
        ["Encoding Params"] = "QuickTime: AudioEncodingParamsVersion, AudioBitRateControlMode and the like",
    };

    public static StreamResult Compare(string path, List<MetadataTag> sharp, List<ExifToolTag> exifTool, Dictionary<int, string> macintosh, Dictionary<string, string> xmpPrefixes)
    {
        var result = new StreamResult { Path = path };
        var theirs = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var tag in exifTool)
        {
            // An XMP property: XMP-dc:creator, a language alternative's other languages title-fr (its id has the language)
            if (tag.Group.StartsWith("XMP-", StringComparison.Ordinal))
            {
                Add(theirs, Key("file", tag.Group, tag.Id, null), tag.Value);
                continue;
            }

            var family = Family(tag.Group);
            // 'free' in 'udta' is room left, not a tag
            if (family == null || tag.Id is "free" or "skip")
                continue;

            string id = tag.Id, language = null;
            if (family.Value.Family == "UserData")
            {
                // A track's are Track1tsel in VideoUserData
                id = TrackPrefix().Replace(id, "");
                (id, language) = SplitLanguage(id);
            }
            else if (family.Value.Family == "Keys")
            {
                // A key it has a name for keeps its domain (com.android.version), the rest lose it
                id = MdtaDomain().Replace(id, "");
                language = SplitLanguage(tag.Name).Language;
            }
            Add(theirs, Key(family.Value.Scope, family.Value.Family, id, language), tag.Value);
        }

        var ours = new Dictionary<string, List<Value>>(StringComparer.Ordinal);
        var firstOnce = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var tag in sharp)
        {
            string scope = tag.Family == MetadataFamily.Xmp ? "file" : tag.TrackID == 0 ? "movie" : "track";
            string id = tag.Key ?? "";
            if (tag.Family == MetadataFamily.UserData && ReadIntoOtherGroups.ContainsKey(id))
                continue;
            if (tag.Family == MetadataFamily.Freeform && FreeformReadIntoOtherGroups.ContainsKey(id))
                continue;
            // one packet, whichever box it is in
            if (tag.Family == MetadataFamily.Xmp)
                id = "XMP";
            string language = tag.Family switch
            {
                MetadataFamily.Keys => KeysLanguage(tag.Language),
                MetadataFamily.UserData => UserDataLanguage(tag.Language, macintosh),
                _ => null,
            };
            // ExifTool's id of a key in the 'mdta' namespace is without com.apple.quicktime. or com.
            if (tag.Family == MetadataFamily.Keys && tag.Namespace == "mdta")
                id = MdtaDomain().Replace(id, "");
            string key = Key(scope, tag.Family.ToString(), id, language);
            if (!ours.TryGetValue(key, out var values))
                ours[key] = values = [];
            values.Add(OurValue(tag, key));

            if (tag.Family == MetadataFamily.Xmp)
            {
                foreach (var (xmpKey, value) in XmpValues((string)tag.Value, xmpPrefixes))
                {
                    // ExifTool takes the toolkit and what a packet is about once per value: of a packet after
                    // the first, only one other than the first packet's (XMP.pm, %recognizedAttrs)
                    if (OncePerValue.Contains(xmpKey))
                    {
                        // and not at all when it is empty, or 0: Perl's false
                        if (value is "" or "0" || (!firstOnce.TryAdd(xmpKey, value) && firstOnce[xmpKey] == value))
                            continue;
                    }
                    if (!ours.TryGetValue(xmpKey, out var xmpValues))
                        ours[xmpKey] = xmpValues = [];
                    // PLUS's vocabulary without its namespace, as ExifTool gives it even unconverted (PLUS.pm, ValueConv)
                    xmpValues.Add(xmpKey.Contains(" XMP-plus:", StringComparison.Ordinal) && value.StartsWith(PlusVocabulary, StringComparison.Ordinal)
                        ? new Value([value, value.Substring(PlusVocabulary.Length)])
                        : new Value([value]));
                }
            }
        }
        DropDefaultLanguageCopies(theirs, ours);

        foreach (var key in theirs.Keys.Union(ours.Keys).OrderBy(k => k, StringComparer.Ordinal))
        {
            var expected = theirs.TryGetValue(key, out var e) ? new List<string>(e) : [];
            var actual = ours.TryGetValue(key, out var a) ? new List<Value>(a) : [];
            foreach (string value in expected.ToList())
            {
                int match = actual.FindIndex(v => v.Forms.Any(form => SameValue(value, form)));
                if (match >= 0)
                {
                    actual.RemoveAt(match);
                    expected.Remove(value);
                }
                // an empty array - <rdf:Bag/> - ExifTool gives as a tag of no value, SharpMP4 as no property
                else if (value.Length == 0 && key.StartsWith("file XMP-", StringComparison.Ordinal))
                {
                    expected.Remove(value);
                }
            }
            result.UnitsCompared++;

            if (expected.Count > 0 && actual.Count > 0)
                result.Fail(Outcome.Diverged, $"differs {key}", $"{key}: ExifTool {Show(expected)}, SharpMP4 {Show(actual.Select(v => v.Forms[0]).ToList())}");
            else if (expected.Count > 0)
                result.Fail(Outcome.Diverged, $"missing {key}", $"{key}: ExifTool {Show(expected)}, SharpMP4 none");
            else if (actual.Count > 0)
                result.Fail(Outcome.Diverged, $"extra {key}", $"{key}: SharpMP4 {Show(actual.Select(v => v.Forms[0]).ToList())}, ExifTool none");
        }

        if (result.Keys.Count == 0)
            result.Outcome = Outcome.Match;
        return result;
    }

    private static string Key(string scope, string family, string id, string? language) =>
        $"{scope} {family}:{id}{(language != null ? "-" + language : "")}";

    private static void Add(Dictionary<string, List<string>> tags, string key, string value)
    {
        if (!tags.TryGetValue(key, out var values))
            tags[key] = values = [];
        values.Add(value);
    }

    private static string Show(List<string> values) => string.Join(" | ", values.Select(v => v.Length > 60 ? v.Substring(0, 60) + "..." : v));

    /// <summary>"©mak-deu" to "©mak" and "deu"; "Make-deu-DE" to its language "deu-DE".</summary>
    private static (string Id, string? Language) SplitLanguage(string id)
    {
        var m = LanguageSuffix().Match(id);
        return m.Success ? (id.Substring(0, m.Index), m.Groups[1].Value) : (id, null);
    }

    /// <summary>
    /// A keyed value's locale as ExifTool names it (GetLangCode): a Macintosh language code, and 'eng' and
    /// 'und', are the default; a country only with a language, 'und' where it is the default.
    /// </summary>
    private static string? KeysLanguage(string? language)
    {
        if (language == null)
            return null;
        string[] parts = language.Split('-');
        string? lang = parts[0].StartsWith("mac:", StringComparison.Ordinal) || parts[0] is "eng" or "und" ? null : parts[0];
        string? country = parts.Length > 1 && parts[1] != "ZZ" ? parts[1] : null;
        if (country != null)
            return $"{lang ?? parts[0] switch { "eng" => "eng", _ => "und" }}-{country}";
        return lang;
    }

    /// <summary>
    /// A QuickTime string's language as ExifTool names it: a Macintosh code by its table, 0 (English) the
    /// default, 0x7fff 'un'; an ISO code as it is, but 'eng' and 'und', the default.
    /// </summary>
    private static string? UserDataLanguage(string? language, Dictionary<int, string> macintosh)
    {
        if (language == null)
            return null;
        if (language.StartsWith("mac:", StringComparison.Ordinal))
        {
            int code = int.Parse(language.Substring(4), CultureInfo.InvariantCulture);
            if (code == 0)
                return null;
            if (code == 0x7fff)
                return "un";
            return macintosh.TryGetValue(code, out string? name) ? name : language;
        }
        return language is "eng" or "und" ? null : language;
    }

    /// <summary>
    /// The values of an XMP packet as ExifTool names them: the group of the namespace's prefix (its own
    /// prefix where it has one); the property's name, and the name of each field of a structure it is in
    /// with a capital (xmpMM:History/stEvt:action is HistoryAction); the items of an array as one list; an
    /// item of a language alternative in a language not the default under its language (XMP.pm,
    /// GetXMPTagID and FoundXMP).
    /// </summary>
    private static List<(string Key, string Value)> XmpValues(string packet, Dictionary<string, string> prefixes)
    {
        var values = new List<(string Key, string Value)>();
        var index = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var property in XmpReader.Read(packet))
        {
            var top = property.Path[0];
            string Prefix(XmpStep step) => prefixes.TryGetValue(step.Namespace, out string? prefix) ? prefix : step.Prefix;
            string group = "XMP-" + Prefix(top);
            var fields = property.Path.Where(s => s.Index == 0).ToList();
            string id = XmpName(fields[0].Name) + string.Concat(fields.Skip(1).Select(s => Capital(XmpName(s.Name))));
            // A field of another namespace in a structure of any namespace's fields is under its prefix,
            // and each value of it a tag of its own, as the top-level tag it takes after is
            bool variable = VariableNamespaceStructures.Contains((top.Namespace, top.Name)) && fields.Count > 1;
            if (variable && fields[1].Namespace != top.Namespace)
                id = Prefix(fields[1]) + ":" + id;
            // A namespace ExifTool has no table of goes to its table of others, whose ids have the prefix:
            // GIMP:api (XMP.pm, FoundXMP)
            else if (!prefixes.ContainsKey(top.Namespace) && !WrapperNamespaces.Contains(top.Namespace))
                id = top.Prefix + ":" + id;
            bool item = property.Path.Any(s => s.Index > 0);
            string? language = item && property.Language != null && property.Language != "x-default" ? property.Language : null;
            string key = Key("file", group, id, language);
            if (!variable && index.TryGetValue(key, out int at))
                values[at] = (key, values[at].Value + " " + property.Value);
            else
            {
                index[key] = values.Count;
                values.Add((key, property.Value));
            }
        }
        return values;
    }

    /// <summary>The namespace of PLUS's controlled vocabulary, which ExifTool takes off its values.</summary>
    private const string PlusVocabulary = "http://ns.useplus.org/ldf/vocab/";

    /// <summary>The namespaces of the packet's wrapper, whose attributes ExifTool reads into tables of their own.</summary>
    private static readonly HashSet<string> WrapperNamespaces = ["adobe:ns:meta/", "http://www.w3.org/1999/02/22-rdf-syntax-ns#"];

    /// <summary>The wrapper's attributes ExifTool takes once per value: the toolkit, and what the packet is about.</summary>
    private static readonly HashSet<string> OncePerValue = [Key("file", "XMP-x", "xmptk", null), Key("file", "XMP-rdf", "about", null)];

    /// <summary>ExifTool's structures whose fields may be of any namespace (XMP.pm: NAMESPACE => undef).</summary>
    private static readonly HashSet<(string Namespace, string Name)> VariableNamespaceStructures =
    [
        ("http://ns.adobe.com/xap/1.0/mm/", "Pantry"),
    ];

    /// <summary>A name as ExifTool writes it: one of capitals only in lower case, an underscore and a letter as the capital.</summary>
    private static string XmpName(string name) =>
        name.Any(char.IsLower) ? name : Regex.Replace(name.ToLowerInvariant(), "_([a-z])", m => m.Groups[1].Value.ToUpperInvariant());

    private static string Capital(string name) => name.Length == 0 ? name : char.ToUpperInvariant(name[0]) + name.Substring(1);

    /// <summary>A value of ours, in the forms ExifTool may give it: as it is, and as the bytes of its box.</summary>
    private sealed record Value(List<string> Forms);

    /// <summary>
    /// Values ExifTool converts even when asked not to (-n converts for printing only): its ValueConv, a
    /// scale here (QuickTime.pm).
    /// </summary>
    private static readonly Dictionary<string, double> ExifToolScales = new(StringComparer.Ordinal)
    {
        ["movie ItemList:gstd"] = 1e-3,                              // GoogleTrackDuration, ms as s
        ["track Keys:camera.framereadouttimeinmicroseconds"] = 1e-6, // FrameReadoutTime, us as s
    };

    /// <summary>A number as Perl prints it: 15 significant digits.</summary>
    private static string PerlNumber(double number) => number.ToString("G15", CultureInfo.InvariantCulture);

    /// <summary>A four character code as its bytes are, zeros left out.</summary>
    private static string FourBytes(uint code) =>
        new string(new[] { (char)(code >> 24), (char)((code >> 16) & 0xFF), (char)((code >> 8) & 0xFF), (char)(code & 0xFF) }.Where(c => c != 0).ToArray());

    private static Value OurValue(MetadataTag tag, string key)
    {
        var forms = new List<string> { ValueText(tag.Value) };
        if (ExifToolScales.TryGetValue(key, out double scale) && double.TryParse(forms[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            forms.Insert(0, PerlNumber(number * scale));
        // A 3GPP rating as ExifTool writes it: Entity=MPAA Criteria=XXXX and the text
        if (tag.Box is ThreeGPPRatingBox rating)
            forms.Add($"Entity={FourBytes(rating.RatingEntity)} Criteria={FourBytes(rating.RatingCriteria)} {forms[0]}");
        // What ExifTool does not read it gives as bytes: the box's, as it has them
        if (tag.Family == MetadataFamily.UserData && tag.Box != null)
            forms.Add("base64:" + Convert.ToBase64String(MetadataReader.Payload(tag.Box)));
        return new Value(forms);
    }

    /// <summary>
    /// ExifTool takes the first language of a string as the default too, where no tag of its name is
    /// (QuickTime.pm, "fill in missing defaults for alternate language tags"): a tag without a language
    /// that a tag in a language has the values of, and SharpMP4 does not read, is that copy.
    /// </summary>
    private static void DropDefaultLanguageCopies(Dictionary<string, List<string>> tags, Dictionary<string, List<Value>> ours)
    {
        foreach (var key in tags.Keys.ToList())
        {
            if (LanguageSuffix().IsMatch(key) || ours.ContainsKey(key))
                continue;
            if (tags.Any(t => t.Key.StartsWith(key + "-", StringComparison.Ordinal) && LanguageSuffix().IsMatch(t.Key) && t.Value.OrderBy(v => v).SequenceEqual(tags[key].OrderBy(v => v))))
                tags.Remove(key);
        }
    }

    private static string ValueText(object value) => value switch
    {
        null => "",
        byte[] bytes => "base64:" + Convert.ToBase64String(bytes),
        int[] numbers => string.Join(" ", numbers),
        string[] strings => string.Join(" ", strings),
        // a chapter as ExifTool gives it: its start in seconds, and its title
        MetadataChapter[] chapters => string.Join(" ", chapters.Select(c => $"{c.Start.TotalSeconds.ToString("R", CultureInfo.InvariantCulture)} {c.Title}")),
        double d => d.ToString("R", CultureInfo.InvariantCulture),
        IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "",
    };

    /// <summary>
    /// A value as ExifTool gives it: the same, or the same but for the padding after it, or the same numbers
    /// (1 of 2 is 1 2; +51.3889+006.8611/ is 51.3889 6.8611), or the same digits (a date ExifTool writes as it writes dates).
    /// </summary>
    private static bool SameValue(string exifTool, string sharp)
    {
        if (exifTool == sharp)
            return true;
        // Bytes ExifTool shows as the text or the number they are
        if (sharp.StartsWith("base64:", StringComparison.Ordinal) && !exifTool.StartsWith("base64:", StringComparison.Ordinal))
        {
            byte[] bytes = Convert.FromBase64String(sharp.Substring(7));
            if (SameValue(exifTool, System.Text.Encoding.UTF8.GetString(bytes)) || SameValue(exifTool, System.Text.Encoding.Latin1.GetString(bytes)))
                return true;
            if (bytes.Length is > 0 and <= 8 && double.TryParse(exifTool, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                long value = (sbyte)bytes[0];
                for (int i = 1; i < bytes.Length; i++)
                    value = (value << 8) | bytes[i];
                ulong unsigned = 0;
                foreach (byte b in bytes)
                    unsigned = (unsigned << 8) | b;
                if (value == number || unsigned == number)
                    return true;
            }
            return false;
        }
        // Text ExifTool takes for bytes - an XMP packet with NULs after it - it gives as base64
        if (exifTool.StartsWith("base64:", StringComparison.Ordinal) && !sharp.StartsWith("base64:", StringComparison.Ordinal))
            return SameValue(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(exifTool.Substring(7))), sharp);
        if (exifTool.TrimEnd('\0', ' ') == sharp.TrimEnd('\0', ' ') || exifTool.Replace("\0", "") == sharp.Replace("\0", ""))
            return true;
        // A list of rationals ExifTool gives as the numbers they are (XMP-aux:LensInfo 18/1 250/1 is 18 250)
        string[] theirItems = exifTool.Split(' '), ourItems = sharp.Split(' ');
        if (ourItems.Length > 1 && ourItems.Length == theirItems.Length && ourItems.Any(item => Rational().IsMatch(item))
            && theirItems.Zip(ourItems).All(pair => SameValue(pair.First, pair.Second)))
            return true;
        // An XMP boolean, True, however it is written
        if (exifTool is "True" or "False" && string.Equals(exifTool, sharp, StringComparison.OrdinalIgnoreCase))
            return true;
        // A rational ExifTool gives as the number it is (XMP: 1/25 is 0.04)
        var rational = Rational().Match(sharp);
        if (rational.Success && double.TryParse(exifTool, NumberStyles.Float, CultureInfo.InvariantCulture, out double quotient))
        {
            double denominator = double.Parse(rational.Groups[2].Value, CultureInfo.InvariantCulture);
            if (denominator != 0 && PerlNumber(double.Parse(rational.Groups[1].Value, CultureInfo.InvariantCulture) / denominator) == PerlNumber(quotient))
                return true;
        }
        var theirNumbers = Numbers(exifTool);
        var ourNumbers = Numbers(sharp);
        if (theirNumbers.Count > 0 && theirNumbers.SequenceEqual(ourNumbers))
            return true;
        // A number of a total of 0 - trkn 1 0 - ExifTool gives as the number alone
        if (theirNumbers.Count > 0 && ourNumbers.Count == theirNumbers.Count + 1 && ourNumbers[^1] == 0 && theirNumbers.SequenceEqual(ourNumbers.Take(theirNumbers.Count)))
            return true;
        string theirDigits = Digits().Replace(exifTool, ""), ourDigits = Digits().Replace(sharp, "");
        return theirDigits.Length >= 8 && theirDigits == ourDigits;
    }

    private static List<double> Numbers(string text) =>
        Number().Matches(text).Select(m => double.Parse(m.Value, CultureInfo.InvariantCulture)).ToList();

    [GeneratedRegex(@"^com\.(apple\.quicktime\.)?")]
    private static partial Regex MdtaDomain();

    [GeneratedRegex(@"^Track\d+")]
    private static partial Regex TrackPrefix();

    [GeneratedRegex(@"-([a-z]{2,3}(?:-[A-Z]{2})?|un|smi|nl-NL)$")]
    private static partial Regex LanguageSuffix();

    [GeneratedRegex(@"[-+]?\d+(?:\.\d+)?")]
    private static partial Regex Number();

    [GeneratedRegex(@"\D")]
    private static partial Regex Digits();

    [GeneratedRegex(@"^\s*(-?\d+)/(\d+)\s*$")]
    private static partial Regex Rational();
}
