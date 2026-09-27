using System.Collections.Concurrent;
using SharpISOBMFF;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Checks what SharpMP4 reads out of a file against the file itself, where there is nothing else
/// to check it against - a box of a vendor's, or metadata no specification describes. A box read
/// with the wrong syntax rarely ends where it should, so the signs of it are:
/// <list type="bullet">
/// <item>bytes left over at the end of a box, where its syntax ended first;</item>
/// <item>a box in it larger than the room left, which is read as an <see cref="InvalidBox"/>;</item>
/// <item>a type no box has - what was not a box, read as one;</item>
/// <item>a size worked out from what was read that differs from the size read, which would write
/// the box wrong once built anew, though a round trip keeps the header it read.</item>
/// </list>
/// Reading past a box's end throws, and is reported as SharpMP4 failing.
/// </summary>
public static class BoxHealth
{
    /// <summary>Checks a file, and counts the boxes in it SharpMP4 does not know, by parent and type.</summary>
    public static StreamResult Check(string path, ConcurrentDictionary<string, ConcurrentBag<string>> unknown)
    {
        var result = new StreamResult { Path = path };
        var container = new Container();
        try
        {
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
            container.Read(new IsoStream(new StreamWrapper(file)));
            Walk(container.Children, "file", path, result, unknown);
        }
        catch (Exception ex)
        {
            result.Fail(Outcome.SharpFailed, $"read: {ex.GetType().Name}: {ex.Message}", $"threw: {ex.GetType().Name}: {ex.Message}");
        }

        if (container.Padding != null && container.Padding.Length > 0)
            result.Fail(Outcome.Diverged, "file: left over", $"{container.Padding.Length} bytes after the last box");

        return result;
    }

    private static void Walk(IEnumerable<Box>? boxes, string parent, string path, StreamResult result, ConcurrentDictionary<string, ConcurrentBag<string>> unknown)
    {
        foreach (var box in boxes ?? [])
        {
            string type = BoxTreeComparison.TypeName(box.FourCC);
            string at = $"{parent}/{type}";
            result.UnitsCompared++;

            if (box is UnreadableBox unreadable)
            {
                result.Fail(Outcome.Diverged, $"{at}: could not be read", $"{at}: could not be read, kept as its bytes: {unreadable.Error}");
                continue;
            }

            if (box is InvalidBox)
            {
                result.Fail(Outcome.Diverged, $"{at}: larger than its parent", $"{at}: declares {box.Header?.GetBoxSizeInBits() >> 3} bytes, more than its parent has left");
                continue;
            }

            // A type a definition knows is a box, whatever its bytes: QuickTime's terminator is all zeros,
            // some QuickTime audio codecs are 'ms' and a number, and a key is typed by its index.
            if (!IsBoxType(box.FourCC) && box is UnknownBox)
                result.Fail(Outcome.Diverged, $"{parent}/?: not a box type", $"{at}: a type no box has, at {box.GetBoxOffset()}");

            if (box is UnknownBox)
                unknown.GetOrAdd(at, _ => []).Add(path);

            if (box.Padding != null && box.Padding.Length > 0 && !IsQuickTimeTerminator(parent, type, box.Padding, path))
                result.Fail(Outcome.Diverged, $"{at}: left over", $"{at}: {box.Padding.Length} bytes left over at {box.GetBoxOffset()}");

            if (box.Header != null && box.Header.Size != 0 && box is not UnknownBox)
            {
                ulong read = box.Header.GetBoxSizeInBits() >> 3;
                ulong calculated = box.CalculateSize() >> 3;
                if (calculated != read)
                    result.Fail(Outcome.Diverged, $"{at}: calculated size", $"{at}: read {read} bytes, calculates {calculated}");
            }

            Walk(BoxTreeComparison.BoxFields(box), type, path, result, unknown);
            Walk(box.Children, type, path, result, unknown);
        }
    }

    /// <summary>
    /// Zeros at the end of a user data list or of a sample description, which the box keeps as its
    /// padding: the 32 bit zero QuickTime lets such a list end with (QuickTime File Format
    /// Specification), and the zeros some writers pad a sound description with.
    /// </summary>
    private static bool IsQuickTimeTerminator(string parent, string type, StreamMarker padding, string path)
    {
        if ((type != "udta" && parent != "stsd") || padding.Length > 4096)
            return false;

        using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        file.Position = padding.Position;
        var bytes = new byte[padding.Length];
        return file.Read(bytes, 0, bytes.Length) == bytes.Length && bytes.All(b => b == 0);
    }

    /// <summary>Printable ASCII, and '©', which starts QuickTime's user data types.</summary>
    private static bool IsBoxType(uint fourCC)
    {
        for (int shift = 24; shift >= 0; shift -= 8)
        {
            byte b = (byte)(fourCC >> shift);
            if ((b < 0x20 || b > 0x7E) && b != 0xA9)
                return false;
        }
        return true;
    }
}
