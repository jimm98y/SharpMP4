using SharpH262;
using SharpMP4.Common;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Reads an H.262 stream unit by unit and writes each again with a context of its own - one that has only written - checking
/// the bytes are the stream's: the headers, the stuffing before each start code, and the slices' data.
/// </summary>
public static class H262RoundTrip
{
    public static StreamResult Check(string path)
    {
        var result = new StreamResult { Path = path };
        var reader = new H262Context();
        var writer = new H262Context();

        byte[] data;
        try
        {
            data = SharpTrace.H262ElementaryStream(path);
        }
        catch (InvalidOperationException ex)
        {
            // a stream whose video ffmpeg cannot identify - FATE's sample of probing, t.mpg - has none to copy out
            result.Outcome = Outcome.NoReference;
            result.Detail = ex.Message;
            return result;
        }
        int units = 0;
        foreach (var (offset, length) in H262Context.Units(data, 0, data.Length))
        {
            SharpH26X.IItuSerializable unit;
            try
            {
                unit = reader.ReadUnit(H262Context.StreamOf(data, offset, length));
            }
            catch (Exception ex)
            {
                result.Fail(Outcome.SharpFailed, $"read unit {data[offset + 3]:x2}: {ex.GetType().Name}", $"unit {units} at {offset}: reading threw: {ex.Message}");
                result.UnitsCompared = units;
                return result;
            }

            var written = new MemoryStream();
            try
            {
                var output = H262Context.StreamOf(written, NullMp4Logger.Instance);
                writer.WriteUnit(output, unit);
                output.Dispose();
            }
            catch (Exception ex)
            {
                result.Fail(Outcome.SharpFailed, $"write unit {data[offset + 3]:x2}: {ex.GetType().Name}", $"unit {units} at {offset}: writing threw: {ex.Message} {ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim()}");
                result.UnitsCompared = units;
                return result;
            }

            byte[] bytes = written.ToArray();
            int differs = FirstDifference(bytes, data, offset, length);
            if (differs >= 0)
            {
                result.Fail(Outcome.Diverged, $"unit {data[offset + 3]:x2}: {unit.GetType().Name}",
                    $"unit {units} at {offset}: written as {bytes.Length} bytes of {length}, first differing at byte {differs}");
                result.UnitsCompared = units;
                return result;
            }
            units++;
        }

        result.UnitsCompared = units;
        if (result.Keys.Count == 0)
            result.Outcome = Outcome.Match;
        return result;
    }

    /// <summary>The first byte written that is not the stream's, or past what either has; -1 if none.</summary>
    private static int FirstDifference(byte[] written, byte[] data, int offset, int length)
    {
        int common = Math.Min(written.Length, length);
        for (int i = 0; i < common; i++)
        {
            if (written[i] != data[offset + i])
                return i;
        }
        return written.Length == length ? -1 : common;
    }
}
