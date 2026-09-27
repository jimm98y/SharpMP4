using System.Text;

namespace SharpMP4.Tests.Conformance;

public enum Outcome
{
    /// <summary>Every unit both read, read the same, field for field.</summary>
    Match,

    /// <summary>A field differs in name, width or value, or one side read more of a unit.</summary>
    Diverged,

    /// <summary>SharpMP4 threw.</summary>
    SharpFailed,

    /// <summary>ffmpeg traced nothing, so there is nothing to compare against.</summary>
    NoReference,
}

/// <summary>What comparing one stream found.</summary>
public sealed class StreamResult
{
    public required string Path { get; init; }
    public Outcome Outcome { get; set; }

    /// <summary>Where the two disagreed, or what was thrown - for a person to read.</summary>
    public string Detail { get; set; } = "";

    /// <summary>A short key to group streams failing the same way: the unit and field, or the exception.</summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// Every way the stream fails, the first of each: one unit type read wrong does not stop the
    /// others being compared, so it cannot hide what is wrong with them.
    /// </summary>
    public List<string> Keys { get; } = [];

    public void Fail(Outcome outcome, string key, string detail)
    {
        if (Keys.Contains(key))
            return;

        if (Keys.Count == 0)
            Key = key;
        if (Outcome != Outcome.SharpFailed)
            Outcome = outcome;

        Keys.Add(key);
        Detail += (Detail.Length > 0 ? "\n    " : "") + detail;
    }

    public int UnitsCompared { get; set; }
    public long FieldsCompared { get; set; }

    /// <summary>Units only one side read - ffmpeg does not decompose every type.</summary>
    public int UnitsUnpaired { get; set; }
}

/// <summary>
/// Lines up what SharpMP4 and ffmpeg read out of the same stream and finds where they disagree:
/// the first place in a unit, since once a field is read at the wrong width everything after it
/// in that unit is noise, and the first unit of each way of disagreeing.
/// </summary>
public static class TraceComparison
{
    /// <summary>
    /// Elements one side reports and the other does not, which say nothing about the reading:
    /// the bits that end a unit and pad it to a byte.
    /// </summary>
    private static readonly HashSet<string> Ignored =
    [
        "rbsp_stop_one_bit", "rbsp_alignment_zero_bit", "rbsp_trailing_bits",
        "alignment_bit_equal_to_one", "alignment_bit_equal_to_zero", "alignment_zero_bit",
        "cabac_zero_word", "byte_alignment",
        // Where ffmpeg reads on into an H.264 slice's data, to the byte its CABAC starts at.
        "cabac_alignment_one_bit",
        // The zeros after an AV1 OBU's trailing one bit, which ffmpeg does not trace all of.
        "trailing_zero_bit",
        // ffmpeg's Annex B demuxer rewrites each OBU - adding the size, dropping padding and
        // trailing zero bytes - so the size it reads is its own. Misread, obu_size would put
        // everything after it out of step anyway.
        "obu_size",
    ];

    /// <summary>
    /// AV1's leb128() elements, which may be coded in more bytes than their value needs - Argon
    /// Streams does, on purpose. SharpMP4 reports the bytes read; ffmpeg the value's shortest form.
    /// </summary>
    private const long AV1TemporalDelimiter = 2;
    private const long AV1FrameHeader = 3;
    private const long AV1TileGroup = 4;
    private const long AV1Frame = 6;
    private const long AV1TileList = 8;
    private const long AV1Padding = 15;

    private static readonly HashSet<string> ValueOnly = ["metadata_type"];

    /// <summary>
    /// AV1 elements ffmpeg reads as one run of one bit increments and traces once, with the value
    /// they count up to, where the spec - and SharpMP4 - read each bit as an element of its own.
    /// Matched by width: the value ffmpeg traces is the variable, not the bits.
    /// </summary>
    private static readonly Dictionary<string, string[]> Increments = new()
    {
        ["increment_tile_cols_log2"] = ["increment_tile_cols_log2"],
        ["increment_tile_rows_log2"] = ["increment_tile_rows_log2"],
        ["lr_unit_shift"] = ["lr_unit_shift", "lr_unit_extra_shift"],
        ["subexp_more_bits"] = ["subexp_more_bits"],
    };

    /// <summary>
    /// AV1 elements ffmpeg traces with the variable they set rather than the bits read - TxMode
    /// for tx_mode_select, lr_unit_shift after its implicit increment - so only the width can be
    /// held against SharpMP4's.
    /// </summary>
    private static readonly HashSet<string> WidthOnly =
    [
        "tx_mode_select", "lr_unit_shift", "increment_tile_cols_log2", "increment_tile_rows_log2", "subexp_more_bits",
    ];

    /// <summary>
    /// Names ffmpeg and SharpMP4 use for the same element in one codec and not another, so they
    /// cannot be renamed on ffmpeg's side alone - ffmpeg name first.
    /// </summary>
    private static readonly (string, string)[] Equivalent =
    [
        // ffmpeg reads every SEI message header the H.265 way; H.266 names the bytes differently.
        ("last_payload_type_byte", "payload_type_byte"),
        ("last_payload_size_byte", "payload_size_byte"),
        ("bit_equal_to_one", "sei_payload_bit_equal_to_one"),
        ("bit_equal_to_one", "payload_bit_equal_to_one"),
        ("bit_equal_to_zero", "payload_bit_equal_to_zero"),
        // ffmpeg's plural for the H.265 SCC palette predictor initializers.
        ("sps_palette_predictor_initializers", "sps_palette_predictor_initializer"),
        ("pps_palette_predictor_initializers", "pps_palette_predictor_initializer"),
        ("bit_equal_to_zero", "sei_payload_bit_equal_to_zero"),
        // The later edition's name for the same element.
        ("gci_num_additional_bits", "gci_num_reserved_bits"),
        // ffmpeg names H.265's delta_chroma_offset_l0 as H.264 names its own.
        ("chroma_offset_l0", "delta_chroma_offset_l0"),
        ("chroma_offset_l1", "delta_chroma_offset_l1"),
        // SharpMP4 numbers the second of two elements of one name in a structure - the VCL
        // HRD's copy of the NAL HRD's, in an H.264 buffering period.
        ("initial_cpb_removal_delay", "initial_cpb_removal_delay0"),
        ("initial_cpb_removal_delay_offset", "initial_cpb_removal_delay_offset0"),
    ];

    private static bool SameName(string sharp, string ffmpeg) =>
        sharp == ffmpeg || Equivalent.Contains((ffmpeg, sharp));

    public static StreamResult Compare(string path, List<TracedUnit> sharp, Exception? sharpError, List<TracedUnit> ffmpeg)
    {
        var result = new StreamResult { Path = path };

        // ffmpeg traced nothing: its parser rejected what the pictures depend on and passed none
        // on. Where SharpMP4 cannot read a single picture of the stream either, the two agree.
        if (ffmpeg.Count == 0 && sharp.Any(IsSlice) && sharp.Where(IsSlice).All(u => u.Error != null))
        {
            result.Outcome = Outcome.Match;
            return result;
        }

        if (ffmpeg.Count == 0)
        {
            result.Outcome = Outcome.NoReference;
            result.Key = "ffmpeg traced nothing";
            return result;
        }

        int i = 0, j = 0;

        // ffmpeg gives up on the rest of a packet with a unit it could not read, and at times on
        // the rest of the stream. Where SharpMP4 could not read that unit either, it may not read
        // the units of that layer that depend on it: ffmpeg has nothing to hold those against,
        // until SharpMP4 reads a slice of the layer again.
        long? unreadableLayer = null;

        // AV1: ffmpeg drops the rest of the temporal unit after an OBU it rejects, whether SharpMP4
        // rejects that OBU too or not; the temporal unit ends at the next temporal delimiter.
        bool temporalUnitDropped = false;
        bool Excused(TracedUnit unit)
        {
            if (temporalUnitDropped)
            {
                if (unit.Fields.Any(f => f.Name == "obu_type" && f.Value == AV1TemporalDelimiter))
                    temporalUnitDropped = false;
                else
                    return true;
            }

            if (unreadableLayer == null || LayerOf(unit) != unreadableLayer)
                return false;
            if (unit.Error == null && IsSlice(unit))
            {
                unreadableLayer = null;
                return false;
            }
            return true;
        }

        while (i < sharp.Count && j < ffmpeg.Count)
        {
            // The rest of a temporal unit ffmpeg dropped: none of it is in ffmpeg's trace.
            if (temporalUnitDropped && Excused(sharp[i]))
            {
                i++;
                result.UnitsUnpaired++;
                continue;
            }

            long? sharpType = TypeOf(sharp[i]);
            long? ffmpegType = TypeOf(ffmpeg[j]);

            if (sharpType != ffmpegType)
            {
                // One side has units the other lacks: skip those on whichever side makes the
                // two agree longest after, and the fewer if both do. ffmpeg traces the parameter
                // sets of its extradata before the first packet repeats them; a fixed look-ahead
                // missed a partner eleven units on, skipped SharpMP4's unit instead, and paired
                // the rest a GOP apart.
                int toFfmpeg = Distance(ffmpeg, j, sharpType);
                int toSharp = Distance(sharp, i, ffmpegType);
                int ffmpegRun = toFfmpeg == int.MaxValue ? -1 : Agreeing(sharp, i, ffmpeg, j + toFfmpeg);
                int sharpRun = toSharp == int.MaxValue ? -1 : Agreeing(sharp, i + toSharp, ffmpeg, j);
                if (ffmpegRun > sharpRun || (ffmpegRun == sharpRun && ffmpegRun >= 0 && toFfmpeg <= toSharp))
                    j++;
                else if (!Excused(sharp[i]) && sharp[i].Error != null)
                    return FailThrown(result, sharp, i, sharp[i].Error!);
                else
                    i++;

                result.UnitsUnpaired++;
                continue;
            }

            string? divergence = CompareUnit(sharp[i], ffmpeg[j], result, out string key, out int agreed);
            result.UnitsCompared++;

            var unitError = sharp[i].Error ?? (i == sharp.Count - 1 ? sharpError : null);
            if (unitError != null)
            {
                // A unit ffmpeg gave up on as well, read the same as far as ffmpeg read it: the
                // two agree the unit cannot be read (PSEXT_A sets bits later editions took for
                // the SCC extension).
                if ((ffmpeg[j].Truncated || StopsEarly(ffmpeg[j])) && divergence == null)
                {
                    // The AV1 reader stops at the unit it cannot read: nothing after it to hold.
                    if (sharp[i].Error == null)
                        sharpError = null;
                    unreadableLayer = LayerOf(sharp[i]);
                    i++;
                    j++;
                    continue;
                }

                // The unit SharpMP4 threw in: if what it read so far agrees, the element ffmpeg
                // read next is the one it could not.
                if (agreed == sharp[i].Fields.Count(f => !Ignored.Contains(f.Name) && f.Bits > 0))
                {
                    var next = ffmpeg[j].Fields.Where(f => !Ignored.Contains(f.Name)).Skip(agreed).FirstOrDefault();
                    result.Fail(Outcome.SharpFailed, $"{ffmpeg[j].Title}: {unitError.GetType().Name} reading {next.Name}",
                        $"unit {i} ({ffmpeg[j].Title}): threw reading what ffmpeg reads as {next}, " +
                        $"after {agreed} fields that agree: {unitError.GetType().Name}: {unitError.Message}" +
                        Context(sharp[i].Fields, sharp[i].Fields.Count) + Where(unitError));
                    return result;
                }

                if (divergence != null)
                    result.Fail(Outcome.Diverged, key, $"unit {i} ({sharp[i].Title}, ffmpeg: {ffmpeg[j].Title}): {divergence}");
                return FailThrown(result, sharp, i, unitError);
            }

            if (divergence != null)
                result.Fail(Outcome.Diverged, key, $"unit {i} ({sharp[i].Title}, ffmpeg: {ffmpeg[j].Title}): {divergence}");

            Excused(sharp[i]);
            if (ffmpeg[j].Truncated && ffmpeg[j].Fields.Any(f => f.Name == "obu_type"))
                temporalUnitDropped = true;
            i++;
            j++;
        }

        // What one side has left once the other ran out. A few are the units the other does not
        // decompose; more from ffmpeg than that mean SharpMP4 never saw them.
        result.UnitsUnpaired += (sharp.Count - i) + (ffmpeg.Count - j);
        // Where ffmpeg's trace ends with a unit it could not read, it gave up on the stream there:
        // what SharpMP4 reads after it has nothing to be held against.
        bool ffmpegGaveUp = j == ffmpeg.Count && ffmpeg.Count > 0 && ffmpeg[^1].Truncated;
        for (int k = i; k < sharp.Count && !ffmpegGaveUp; k++)
        {
            if (!Excused(sharp[k]) && sharp[k].Error != null)
                return FailThrown(result, sharp, k, sharp[k].Error!);
        }

        if (sharpError == null && ffmpeg.Count - j > 2)
        {
            result.Fail(Outcome.Diverged, "SharpMP4 stops before ffmpeg does",
                $"SharpMP4 read {sharp.Count} units and stopped; ffmpeg has {ffmpeg.Count - j} more, from {ffmpeg[j].Title}");
            return result;
        }

        if (sharpError != null)
            return FailThrown(result, sharp, sharp.Count - 1, sharpError);

        if (result.Keys.Count == 0)
            result.Outcome = Outcome.Match;
        return result;
    }

    /// <summary>SharpMP4 threw reading a unit ffmpeg read, or one ffmpeg has no partner for.</summary>
    private static StreamResult FailThrown(StreamResult result, List<TracedUnit> sharp, int index, Exception error)
    {
        var unit = index >= 0 && index < sharp.Count ? sharp[index] : null;
        string lastField = unit != null && unit.Fields.Count > 0 ? unit.Fields[^1].ToString() : "nothing";
        result.Fail(Outcome.SharpFailed,
            $"{error.GetType().Name} in {unit?.Title} after {(unit != null && unit.Fields.Count > 0 ? unit.Fields[^1].Name : "-")}",
            $"threw at unit {index} ({unit?.Title}), after reading {lastField}: " +
            $"{error.GetType().Name}: {error.Message}" + Where(error));
        return result;
    }

    /// <summary>Where in SharpMP4 an exception came from: the top of its stack, which is enough to find it.</summary>
    private static string Where(Exception error)
    {
        var frames = (error.StackTrace ?? "").Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.StartsWith("at Sharp", StringComparison.Ordinal))
            .Take(3)
            .Select(line => line.Split(" in ")[0].Substring(3));

        return "\n      at: " + string.Join(" <- ", frames);
    }

    /// <summary>
    /// What pairs a unit with its counterpart: its type, and for H.265 its layer - ffmpeg skips
    /// most units of the layers above the base one, so a type alone paired a layer 1 slice with the
    /// next base layer one.
    /// </summary>
    private static long? TypeOf(TracedUnit unit)
    {
        long? type = null;
        foreach (var field in unit.Fields)
        {
            if (field.Name is "nal_unit_type" or "obu_type")
                type = field.Value;
            else if (field.Name == "nuh_layer_id" && type != null)
                return type * 64 + field.Value;
            else if (type != null)
                break;
        }

        return type;
    }

    /// <summary>
    /// Whether ffmpeg reads only the start of a unit of this kind: the Annex B demuxer drops a
    /// padding OBU's content, and the entries of a tile list are not broken down.
    /// </summary>
    private static bool StopsEarly(TracedUnit ffmpeg)
    {
        var obuType = ffmpeg.Fields.FirstOrDefault(f => f.Name == "obu_type");
        return obuType.Name != null && obuType.Value is AV1Padding or AV1TileList;
    }

    /// <summary>The layer of a unit: nuh_layer_id where the codec has one, 0 otherwise.</summary>
    private static long LayerOf(TracedUnit unit) =>
        unit.Fields.FirstOrDefault(f => f.Name == "nuh_layer_id").Value;

    /// <summary>Whether a unit is a slice: whether it reads a slice header's first element.</summary>
    private static bool IsSlice(TracedUnit unit) =>
        unit.Fields.Any(f => f.Name is "first_slice_segment_in_pic_flag" or "first_mb_in_slice" or "sh_picture_header_in_slice_header_flag" ||
            (f.Name == "obu_type" && f.Value is AV1FrameHeader or AV1TileGroup or AV1Frame));

    /// <summary>For how many units, up to 16, the types of the two sides agree from here.</summary>
    private static int Agreeing(List<TracedUnit> sharp, int i, List<TracedUnit> ffmpeg, int j)
    {
        int run = 0;
        while (run < 16 && i + run < sharp.Count && j + run < ffmpeg.Count && TypeOf(sharp[i + run]) == TypeOf(ffmpeg[j + run]))
            run++;
        return run;
    }

    /// <summary>How many units on a unit of a type turns up, if it does soon.</summary>
    private static int Distance(List<TracedUnit> units, int from, long? type)
    {
        for (int k = from; k < units.Count && k < from + 256; k++)
        {
            if (TypeOf(units[k]) == type)
                return k - from;
        }

        return int.MaxValue;
    }

    /// <param name="agreed">How many of SharpMP4's fields agreed before the first that did not.</param>
    private static string? CompareUnit(TracedUnit sharp, TracedUnit ffmpeg, StreamResult result, out string key, out int agreed)
    {
        agreed = 0;
        // A field of no bits - order_hint when OrderHintBits is 0, say - is read without reading
        // anything; SharpMP4 logs it, ffmpeg does not.
        var a = sharp.Fields.Where(f => !Ignored.Contains(f.Name) && f.Bits > 0).ToList();
        var b = ffmpeg.Fields.Where(f => !Ignored.Contains(f.Name)).ToList();
        key = "";

        int ka = 0, kb = 0;
        bool addedObuSize = false;

        // Past the part of such a unit ffmpeg reads, there is nothing to compare.
        bool ffmpegStopsEarly = StopsEarly(ffmpeg);
        while (ka < a.Count && kb < b.Count)
        {
            result.FieldsCompared++;
            var theirs = b[kb];
            int taken = 1;

            // ffmpeg reads at most 32 bits at a time, and traces a wider element as consecutive
            // pieces under one name. Only joined when SharpMP4 read it wider: several elements of
            // an array share a name too, once their indices are stripped.
            while (a[ka].Name == theirs.Name && a[ka].Bits > theirs.Bits &&
                kb + taken < b.Count && b[kb + taken].Name == theirs.Name)
            {
                var piece = b[kb + taken];
                theirs = new TracedField(theirs.Name, theirs.Bits + piece.Bits, (theirs.Value << piece.Bits) | piece.Value);
                taken++;
            }

            // A run of increments ffmpeg traced as one: take as many of SharpMP4's as its width.
            if (Increments.TryGetValue(theirs.Name, out var parts) && a[ka].Name == theirs.Name && a[ka].Bits < theirs.Bits)
            {
                int bits = 0, next = ka;
                while (next < a.Count && bits < theirs.Bits && parts.Contains(a[next].Name))
                    bits += a[next++].Bits;

                if (bits == theirs.Bits)
                {
                    ka = next;
                    kb += taken;
                    agreed = ka;
                    continue;
                }
            }

            // ffmpeg does not break everything down: what it reads as extension_data, as the
            // bytes of an SEI message it does not know, or as the bytes of a slice header's
            // extension - which the multi-layer annex fills with poc_reset_idc and the like - it
            // cannot see into, so there is nothing more to compare against in this unit.
            if (theirs.Name is "extension_data" or "payload_byte" or "slice_segment_header_extension_data_byte" or "obu_padding_byte")
                return null;

            // Where ffmpeg runs out of an SEI message's syntax before its payload ends, it reads
            // the rest as extension data. ffmpeg does not parse the multi-layer VPS extension, so
            // the SPS of a layer that takes its format from there reads as 4:0:0, and a decoded
            // picture hash has one plane for it where the payload carries three (POC_A_Ericsson).
            if (theirs.Name == "reserved_payload_extension_data" && a[ka].Name != theirs.Name)
                return null;

            // ffmpeg's Annex B demuxer hands on each OBU with an obu_size the stream did not code:
            // where SharpMP4 reads the flag clear, ffmpeg reads it set and the size it added.
            if (theirs.Name == "obu_has_size_field" && a[ka].Name == theirs.Name && a[ka].Value == 0 && theirs.Value == 1)
            {
                ka++;
                kb += taken;
                agreed = ka;
                addedObuSize = true;
                continue;
            }

            if (addedObuSize && theirs.Name == "obu_size" && a[ka].Name != "obu_size")
            {
                addedObuSize = false;
                kb += taken;
                continue;
            }

            if ((SameName(a[ka].Name, theirs.Name) && (a[ka].Bits == theirs.Bits || ValueOnly.Contains(theirs.Name)) && a[ka].Value == theirs.Value) ||
                (WidthOnly.Contains(theirs.Name) && a[ka].Name == theirs.Name && a[ka].Bits == theirs.Bits) ||
                (a[ka].Value == TracedField.Opaque && a[ka].Name == theirs.Name && a[ka].Bits == theirs.Bits))
            {
                ka++;
                kb += taken;
                agreed = ka;
                continue;
            }

            key = a[ka].Name == theirs.Name
                ? $"{ffmpeg.Title}: {a[ka].Name}"
                : $"{ffmpeg.Title}: {theirs.Name} vs {a[ka].Name}";

            return $"field {ka}: ffmpeg read {theirs}, SharpMP4 read {a[ka]}" + Context(a, ka);
        }

        // Where ffmpeg gave up on the unit, or stops short of it, SharpMP4 reading on is no
        // disagreement.
        if (ka < a.Count && kb == b.Count && (ffmpeg.Truncated || ffmpegStopsEarly))
            return null;

        if (ka < a.Count || kb < b.Count)
        {
            bool sharpLonger = ka < a.Count;
            var extra = sharpLonger ? a[ka] : b[kb];
            string who = sharpLonger ? "SharpMP4" : "ffmpeg";
            int more = sharpLonger ? a.Count - ka : b.Count - kb;
            key = $"{ffmpeg.Title}: {who} reads on with {extra.Name}";
            return $"{who} reads {more} more fields, from {extra}" + Context(sharpLonger ? a : b, sharpLonger ? ka : kb);
        }

        return null;
    }

    /// <summary>The fields just before a divergence, which usually say why.</summary>
    private static string Context(List<TracedField> fields, int at)
    {
        var text = new StringBuilder("\n      after: ");
        for (int k = Math.Max(0, at - 4); k < at; k++)
            text.Append(fields[k]).Append("; ");
        return text.ToString();
    }
}
