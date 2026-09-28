using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SharpMP4.Tests.Conformance;

/// <summary>One syntax element as a parser read it: its name, how many bits it took, its value.</summary>
public readonly record struct TracedField(string Name, int Bits, long Value)
{
    /// <summary>The value of a field read as a run of bytes, which the parser does not log.</summary>
    public const long Opaque = long.MinValue;

    public override string ToString() => $"{Name} ({Bits} bits) = {Value}";
}

/// <summary>The syntax elements read out of one NAL unit or OBU, in order.</summary>
public sealed class TracedUnit
{
    public TracedUnit(string title) => Title = title;

    /// <summary>What the unit is, as the parser that read it calls it.</summary>
    public string Title { get; }

    public List<TracedField> Fields { get; } = [];

    /// <summary>
    /// The parser gave up partway through - ffmpeg does on the parts of the spec it does not
    /// implement, 3D-HEVC's extensions among them - so the unit has more than it read.
    /// </summary>
    public bool Truncated { get; set; }

    /// <summary>What SharpMP4 threw reading the unit, if it did; it reads on with the next.</summary>
    public Exception? Error { get; set; }
}

/// <summary>
/// The headers of a bitstream as ffmpeg reads them, through its trace_headers filter: an
/// implementation independent of SharpMP4, which is what makes it an oracle. Reading and writing
/// back through SharpMP4 cannot catch a field read at the wrong width - the writer uses the same
/// width - but ffmpeg disagrees at the very field.
/// </summary>
public static partial class FfmpegTrace
{
    // "16          first_slice_segment_in_pic_flag                             1 = 1"
    [GeneratedRegex(@"^(\d+)\s+(\S+)\s+([01]+)\s+=\s+(-?\d+)$")]
    private static partial Regex FieldLine();

    /// <summary>
    /// Reads every header ffmpeg can decompose, as ffmpeg traced it the last time it was given the
    /// stream (<see cref="ToolCache"/>), or as it traces it now.
    /// </summary>
    /// <param name="format">ffmpeg's name for the raw format: h264, hevc, vvc, ivf or obu.</param>
    /// <param name="input">What to give ffmpeg instead of the stream, if anything: a file deleted
    /// once traced. Made only when ffmpeg is run.</param>
    public static List<TracedUnit> Read(string ffmpeg, string path, string format, Func<string, string?>? input = null)
    {
        // The arguments count in finding an entry, and so does the version of what it holds
        string? entry = ToolCache.EntryOf("ffmpeg", [ffmpeg], $"trace 1 -f {format} -bsf:v trace_headers", path);
        var units = ToolCache.GetOrRun(entry, () =>
        {
            string? made = input?.Invoke(path);
            try
            {
                return Trace(ffmpeg, made ?? path, format);
            }
            finally
            {
                if (made != null)
                    File.Delete(made);
            }
        }, ReadUnits, WriteUnits);

        // ffmpeg's names, kept as it wrote them, as the specs spell them
        var names = new Dictionary<string, string>();
        foreach (var unit in units)
        {
            for (int i = 0; i < unit.Fields.Count; i++)
            {
                string name = unit.Fields[i].Name;
                if (!names.TryGetValue(name, out string? plain))
                    names[name] = plain = Canonical(Indexed.TryGetValue(name, out string? spec) ? spec : StripIndices(name));
                unit.Fields[i] = unit.Fields[i] with { Name = plain };
            }
        }
        return units;
    }

    private static void WriteUnits(BinaryWriter writer, List<TracedUnit> units)
    {
        // A name is written once, then by its number: a stream's trace names a few hundred elements millions of times
        var numbers = new Dictionary<string, int>(StringComparer.Ordinal);
        writer.Write(units.Count);
        foreach (var unit in units)
        {
            writer.Write(unit.Title);
            writer.Write(unit.Truncated);
            writer.Write(unit.Fields.Count);
            foreach (var field in unit.Fields)
            {
                if (numbers.TryGetValue(field.Name, out int number))
                {
                    writer.Write7BitEncodedInt(number + 1);
                }
                else
                {
                    writer.Write7BitEncodedInt(0);
                    writer.Write(field.Name);
                    numbers[field.Name] = numbers.Count;
                }
                writer.Write7BitEncodedInt(field.Bits);
                writer.Write7BitEncodedInt64(field.Value);
            }
        }
    }

    private static List<TracedUnit> ReadUnits(BinaryReader reader)
    {
        var names = new List<string>();
        var units = new List<TracedUnit>(reader.ReadInt32());
        for (int count = units.Capacity; units.Count < count;)
        {
            var unit = new TracedUnit(reader.ReadString()) { Truncated = reader.ReadBoolean() };
            int fields = reader.ReadInt32();
            unit.Fields.Capacity = fields;
            for (int i = 0; i < fields; i++)
            {
                int number = reader.Read7BitEncodedInt();
                string name;
                if (number == 0)
                    names.Add(name = reader.ReadString());
                else
                    name = names[number - 1];
                unit.Fields.Add(new TracedField(name, reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt64()));
            }
            units.Add(unit);
        }
        return units;
    }

    /// <summary>
    /// Runs ffmpeg's trace_headers over a stream. The trace is read as it is written - a long
    /// stream's runs to hundreds of megabytes - and kept only as the fields, named as ffmpeg names them.
    /// </summary>
    private static List<TracedUnit> Trace(string ffmpeg, string path, string format)
    {
        var start = new ProcessStartInfo(ffmpeg)
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };

        foreach (var argument in new[]
        {
            "-hide_banner", "-nostdin", "-v", "info", "-f", format, "-i", path,
            // -copyinkf: stream copy drops what comes before the first key frame otherwise.
            "-map", "0:v:0", "-c", "copy", "-copyinkf", "-bsf:v", "trace_headers", "-f", "null", "-",
        })
        {
            start.ArgumentList.Add(argument);
        }

        using var process = Process.Start(start)!;
        process.StandardOutput.ReadToEndAsync();

        var units = new List<TracedUnit>();
        TracedUnit? current = null;
        string? heading = null;
        // one string per name, not one per line
        var names = new Dictionary<string, string>(StringComparer.Ordinal);

        for (string? line = process.StandardError.ReadLine(); line != null; line = process.StandardError.ReadLine())
        {
            string? text = TraceText(line);
            if (text == null)
                continue;

            if (text.Length == 0 || text.StartsWith("Packet:", StringComparison.Ordinal) || text == "Extradata")
                continue;

            if (text.StartsWith("Failed to read unit", StringComparison.Ordinal))
            {
                if (current != null)
                    current.Truncated = true;
                continue;
            }

            var match = FieldLine().Match(text);
            if (!match.Success)
            {
                // A heading: of the next unit if a unit header follows it, otherwise of a part of
                // this one - each message in an SEI unit gets one.
                heading = text;
                continue;
            }

            string name = match.Groups[2].Value;
            if (heading != null && name is "forbidden_zero_bit" or "obu_forbidden_bit")
            {
                current = new TracedUnit(heading);
                units.Add(current);
            }

            heading = null;
            if (current == null)
                continue;

            if (!names.TryGetValue(name, out string? kept))
                names[name] = kept = name;

            current.Fields.Add(new TracedField(kept, match.Groups[3].Length, long.Parse(match.Groups[4].Value)));
        }

        process.WaitForExit();
        return units;
    }

    /// <summary>
    /// The trace in a line of ffmpeg's log, if there is one. ffmpeg demuxes on a thread of its own:
    /// its H.264 parser reports a missing feature ("FMO is not implemented") in two pieces, and a
    /// trace line written in between follows the first piece without a prefix of its own, which
    /// left a field out of the trace now and then.
    /// </summary>
    private static string? TraceText(string line)
    {
        int marker = line.IndexOf("[trace_headers @ ", StringComparison.Ordinal);
        if (marker >= 0)
            return line.Substring(line.IndexOf(']', marker) + 1).Trim();

        foreach (string feature in MissingFeatures)
        {
            int at = line.IndexOf("] " + feature, StringComparison.Ordinal);
            if (at >= 0 && line.StartsWith("[", StringComparison.Ordinal))
            {
                string rest = line.Substring(at + 2 + feature.Length).Trim();
                if (rest.Length > 0 && !rest.StartsWith("is not implemented", StringComparison.Ordinal))
                    return rest;
            }
        }

        return null;
    }

    /// <summary>The features ffmpeg's parsers report missing on the streams traced.</summary>
    private static readonly string[] MissingFeatures = ["FMO"];

    /// <summary>
    /// Where ffmpeg spells an element differently from the spec text SharpMP4 is generated from.
    /// Only the name is forgiven: the width and value are still compared.
    /// </summary>
    private static readonly Dictionary<string, string> Aliases = new()
    {
        ["gaps_in_frame_num_allowed_flag"] = "gaps_in_frame_num_value_allowed_flag",
        ["scaling_list_delta_coeff"] = "scaling_list_delta_coef",
        ["sps_palette_predictor_initializer_present_flag"] = "sps_palette_predictor_initializers_present_flag",
        ["intra_boundary_filtering_disable_flag"] = "intra_boundary_filtering_disabled_flag",
        ["pps_palette_predictor_initializer_present_flag"] = "pps_palette_predictor_initializers_present_flag",
        ["sps_num_palette_predictor_initializer_minus1"] = "sps_num_palette_predictor_initializers_minus1",
        ["pps_num_palette_predictor_initializer"] = "pps_num_palette_predictor_initializers",
        // AV1: the one bit increments, which ffmpeg names after the variable they count up.
        ["tile_cols_log2"] = "increment_tile_cols_log2",
        ["tile_rows_log2"] = "increment_tile_rows_log2",
        ["tx_mode"] = "tx_mode_select",
        ["tile_size_bytes_minus1"] = "tile_size_bytes_minus_1",
        ["golden_frame_idx"] = "gold_frame_idx",
        ["delta_frame_id_minus1"] = "delta_frame_id_minus_1",
    };

    /// <summary>
    /// Elements ffmpeg names as one array where the spec names some of its entries: the base
    /// spec's reserved slice header bits, the first two of which the multi-layer annex gives names.
    /// </summary>
    private static readonly Dictionary<string, string> Indexed = new()
    {
        ["slice_reserved_flag[0]"] = "discardable_flag",
        ["slice_reserved_flag[1]"] = "cross_layer_bla_flag",
    };

    /// <summary>
    /// The spec's name for an element: ffmpeg's alias if it has one, and without the structure
    /// ffmpeg prefixes AV1's with - delta_q_y_dc.delta_coded is the spec's delta_coded.
    /// </summary>
    private static string Canonical(string name)
    {
        int dot = name.LastIndexOf('.');
        if (dot >= 0)
            name = name.Substring(dot + 1);

        return Aliases.TryGetValue(name, out string? spec) ? spec : name;
    }

    /// <summary>"delta_poc_s0_minus1[0]" to "delta_poc_s0_minus1": SharpMP4 names an element without its indices.</summary>
    public static string StripIndices(string name)
    {
        int bracket = name.IndexOf('[');
        return bracket < 0 ? name : name.Substring(0, bracket);
    }
}
