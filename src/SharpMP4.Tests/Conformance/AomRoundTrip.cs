using SharpAV1;
using SharpAVX;
using SharpMP4.Common;
using SharpVP9;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Reads an AV1 stream OBU by OBU, recording each, and writes each again with a context of its own -
/// one that has only written, as an encoder has - checking the bytes are the stream's.
/// </summary>
public static class AomRoundTrip
{
    /// <summary>
    /// For each element, the occurrences written as they were recorded that the state they were read into
    /// gives otherwise, and in how many streams: what only the record says, which an edit cannot reach.
    /// </summary>
    public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (int Occurrences, int Streams)> RecordOnly = new();

    /// <summary>The same, of VP9's streams.</summary>
    public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (int Occurrences, int Streams)> Vp9RecordOnly = new();

    public static StreamResult CheckAv1(string path)
    {
        var result = new StreamResult { Path = path };
        // Every layer, and read as far as a stream can be: its defects are written back as they are
        var reader = new AV1Context { AllLayers = 1, RecordSyntax = true };
        var writer = new AV1Context { AllLayers = 1 };

        bool ivf = Path.GetExtension(path).Equals(".ivf", StringComparison.OrdinalIgnoreCase);
        var temporalUnits = ivf ? SharpTrace.IvfFrames(path) : SharpTrace.IsAnnexB(path) ? SharpTrace.AnnexBObus(path) : [File.ReadAllBytes(path)];

        var recordOnly = new Dictionary<string, int>();
        int obus = 0;
        string? unreadable = null;
        foreach (byte[] temporalUnit in temporalUnits)
        {
            int offset = 0;
            while (offset < temporalUnit.Length)
            {
                int left = temporalUnit.Length - offset;
                try
                {
                    using var input = new AomStream(new MemoryStream(temporalUnit, offset, left), NullMp4Logger.Instance);
                    reader.Read(input, left);
                }
                catch (Exception ex)
                {
                    // What cannot be read cannot be written from what was read: as a decoder drops it, and
                    // ffmpeg its packet, the rest of its unit is kept as it was, and the stream read on.
                    unreadable ??= $"OBU {obus} at {offset}: reading threw: {ex.Message}; the rest of its unit kept as it was";

                    // Written as it was, the writer following on from where the reader was left
                    var kept = new MemoryStream();
                    using (var output = new AomStream(kept, NullMp4Logger.Instance))
                        writer.Write(output, reader.LastObu);
                    if (reader.LastObu?.Unreadable == null || FirstDifference(kept.ToArray(), temporalUnit, offset, left) >= 0)
                    {
                        result.Fail(Outcome.Diverged, "unreadable OBU: not kept", $"OBU {obus} at {offset}: not written as it was");
                        result.UnitsCompared = obus;
                        return result;
                    }
                    break;
                }

                int header = 1 + (reader._ObuExtensionFlag != 0 ? 1 : 0) + (reader.ObuSizeLen >> 3);
                int size = reader._ObuHasSizeField != 0 ? header + reader._ObuSize : left;
                if (size <= 0 || size > left)
                {
                    result.Fail(Outcome.SharpFailed, "read: size", $"OBU {obus} at {offset}: {size} bytes of {left}");
                    result.UnitsCompared = obus;
                    return result;
                }

                var written = new MemoryStream();
                var positions = new Positions();
                try
                {
                    using var output = new AomStream(written, NullMp4Logger.Instance) { RecordOnly = recordOnly, ElementEnded = positions.Add };
                    writer.Write(output, reader.LastObu);
                }
                catch (Exception ex)
                {
                    result.Fail(Outcome.SharpFailed, $"write obu_type {reader._ObuType}: {ex.GetType().Name}", $"OBU {obus} at {offset}: writing threw: {ex.Message} {ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim()}" + (unreadable != null ? $"; before it, {unreadable}" : ""));
                    result.UnitsCompared = obus;
                    return result;
                }

                byte[] bytes = written.ToArray();
                int differs = FirstDifference(bytes, temporalUnit, offset, size);
                if (differs >= 0)
                {
                    long bit = differs * 8L + FirstBit(differs < bytes.Length ? bytes[differs] : 0, offset + differs < temporalUnit.Length && differs < size ? temporalUnit[offset + differs] : 0);
                    string element = positions.At(bit);
                    result.Fail(Outcome.Diverged, $"obu_type {reader._ObuType}: {element}",
                        $"OBU {obus} at {offset}: written as {bytes.Length} bytes of {size}, first differing at bit {bit}, in {element}");
                    result.UnitsCompared = obus;
                    return result;
                }

                // The frame header a redundant copy repeats, for each as the caller gives it
                if (reader._ObuType is AV1ObuTypes.OBU_FRAME_HEADER or AV1ObuTypes.OBU_FRAME)
                {
                    reader.LastObuFrameHeader = temporalUnit.AsSpan(offset + header, reader._ObuSize).ToArray();
                    writer.LastObuFrameHeader = bytes.AsSpan(header, reader._ObuSize).ToArray();
                }

                offset += size;
                obus++;
            }
        }

        foreach (var (name, count) in recordOnly)
            RecordOnly.AddOrUpdate(name, (count, 1), (_, total) => (total.Occurrences + count, total.Streams + 1));
        result.UnitsCompared = obus;
        if (result.Keys.Count == 0 && unreadable != null)
        {
            // Every OBU read written as it was; one that could not be, kept as it was
            result.Outcome = Outcome.Malformed;
            result.Detail = "malformed, and kept as it was: " + unreadable;
        }
        else if (result.Keys.Count == 0)
            result.Outcome = Outcome.Match;
        return result;
    }

    /// <summary>
    /// Reads a VP9 stream frame by frame - and each superframe's index - recording each, and writes each again with a
    /// context of its own, checking the bytes are the stream's: the headers, the compressed header's Boolean coding,
    /// and the tile data.
    /// </summary>
    public static StreamResult CheckVp9(string path)
    {
        var result = new StreamResult { Path = path };
        var reader = new VP9Context { RecordSyntax = true };
        var writer = new VP9Context();
        // what was written, read again: where it differs from the stream, it has to read as the stream did
        var reread = new VP9Context { RecordSyntax = true };
        string? equivalent = null;

        var recordOnly = new Dictionary<string, int>();
        int units = 0;
        string? unreadable = null;
        foreach (byte[] chunk in SharpTrace.Vp9Chunks(path))
        {
            int[]? sizes = VP9Context.SuperframeFrameSizes(chunk, 0, chunk.Length);
            var parts = (sizes ?? [chunk.Length]).Select(size => (Size: size, Index: false)).ToList();
            if (sizes != null)
                parts.Add((chunk.Length - sizes.Sum(), true));

            int offset = 0;
            foreach (var (size, index) in parts)
            {
                if (size <= 0 || size > chunk.Length - offset)
                {
                    result.Fail(Outcome.SharpFailed, "read: size", $"unit {units} at {offset}: {size} bytes of {chunk.Length - offset}");
                    result.UnitsCompared = units;
                    return result;
                }

                try
                {
                    using var input = new AomStream(new MemoryStream(chunk, offset, size), NullMp4Logger.Instance);
                    if (index)
                        reader.ReadSuperframeIndex(input, size);
                    else
                        reader.Read(input, size);
                }
                catch (Exception ex)
                {
                    // As ffmpeg drops its packet: the rest of the chunk is kept as it was, and the stream read on
                    unreadable ??= $"unit {units} at {offset}: reading threw: {ex.Message}; the rest of its chunk kept as it was";

                    var kept = new MemoryStream();
                    using (var output = new AomStream(kept, NullMp4Logger.Instance))
                        writer.Write(output, reader.LastUnit);
                    if (reader.LastUnit?.Unreadable == null || FirstDifference(kept.ToArray(), chunk, offset, size) >= 0)
                    {
                        result.Fail(Outcome.Diverged, "unreadable unit: not kept", $"unit {units} at {offset}: not written as it was");
                        result.UnitsCompared = units;
                        return result;
                    }
                    break;
                }

                var written = new MemoryStream();
                var positions = new Positions();
                try
                {
                    using var output = new AomStream(written, NullMp4Logger.Instance) { RecordOnly = recordOnly, ElementEnded = positions.Add };
                    writer.Write(output, reader.LastUnit);
                }
                catch (Exception ex)
                {
                    result.Fail(Outcome.SharpFailed, $"write {(index ? "superframe index" : "frame")}: {ex.GetType().Name}", $"unit {units} at {offset}: writing threw: {ex.Message} {ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim()}" + (unreadable != null ? $"; before it, {unreadable}" : ""));
                    result.UnitsCompared = units;
                    return result;
                }

                byte[] bytes = written.ToArray();
                try
                {
                    using var again = new AomStream(new MemoryStream(bytes), NullMp4Logger.Instance);
                    if (index)
                        reread.ReadSuperframeIndex(again, bytes.Length);
                    else
                        reread.Read(again, bytes.Length);
                }
                catch (Exception)
                {
                    // compared below: what cannot be read again is not what was read
                }

                int differs = FirstDifference(bytes, chunk, offset, size);
                if (differs >= 0)
                {
                    long bit = differs * 8L + FirstBit(differs < bytes.Length ? bytes[differs] : 0, offset + differs < chunk.Length && differs < size ? chunk[offset + differs] : 0);
                    string element = positions.At(bit);
                    string where = $"unit {units} at {offset}: written as {bytes.Length} bytes of {size}, first differing at bit {bit}, in {element}";
                    if (bytes.Length != size || !SameSyntax(reader.LastUnit?.Record, reread.LastUnit?.Record))
                    {
                        result.Fail(Outcome.Diverged, $"{(index ? "superframe index" : "frame")}: {element}", where);
                        result.UnitsCompared = units;
                        return result;
                    }
                    equivalent ??= where + ", but reads as it did";
                }

                offset += size;
                units++;
            }
        }

        foreach (var (name, count) in recordOnly)
            Vp9RecordOnly.AddOrUpdate(name, (count, 1), (_, total) => (total.Occurrences + count, total.Streams + 1));
        result.UnitsCompared = units;
        if (result.Keys.Count == 0 && unreadable != null)
        {
            result.Outcome = Outcome.Malformed;
            result.Detail = "malformed, and kept as it was: " + unreadable;
        }
        else if (result.Keys.Count == 0 && equivalent != null)
        {
            result.Outcome = Outcome.Equivalent;
            result.Detail = equivalent;
        }
        else if (result.Keys.Count == 0)
            result.Outcome = Outcome.Match;
        return result;
    }

    /// <summary>Whether two records have the same elements, in the same order, of the same values and widths.</summary>
    private static bool SameSyntax(AomSyntaxRecord? a, AomSyntaxRecord? b)
    {
        if (a == null || b == null || a.Count != b.Count)
            return false;
        for (int i = 0; i < a.Count; i++)
        {
            var (x, y) = (a.ValueAt(i), b.ValueAt(i));
            if (a.NameAt(i) != b.NameAt(i) || x.Value != y.Value || x.Bits != y.Bits
                || (x.Bytes != null) != (y.Bytes != null) || x.Bytes != null && !x.Bytes.AsSpan().SequenceEqual(y.Bytes))
                return false;
        }
        return true;
    }

    /// <summary>The first byte written that is not the stream's, or past what either has; -1 if none.</summary>
    private static int FirstDifference(byte[] written, byte[] unit, int offset, int size)
    {
        int common = Math.Min(written.Length, size);
        for (int i = 0; i < common; i++)
        {
            if (written[i] != unit[offset + i])
                return i;
        }
        return written.Length == size ? -1 : common;
    }

    private static int FirstBit(int a, int b)
    {
        int x = (a ^ b) & 0xFF;
        for (int bit = 0; bit < 8; bit++)
        {
            if ((x & (0x80 >> bit)) != 0)
                return bit;
        }
        return 0;
    }

    /// <summary>Where each element written ends: the element a bit belongs to.</summary>
    private sealed class Positions
    {
        private readonly List<(long End, string Name)> _ends = [];

        public void Add(string name, long end) => _ends.Add((end, name));

        public string At(long bit)
        {
            foreach (var (end, name) in _ends)
            {
                if (end > bit)
                    return name;
            }
            return _ends.Count > 0 ? $"after {_ends[^1].Name}" : "the OBU header";
        }
    }
}
