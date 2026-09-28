using SharpISOBMFF;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Reads a file with SharpMP4 and writes it back, checking the bytes written against the file's own
/// as they come, so that a file of gigabytes needs no copy of itself.
/// </summary>
/// <remarks>
/// What an 'mdat' holds is not compared: SharpMP4 reads none of it, only where it is, and writes it back
/// by copying those bytes of the file - the file compared with itself, most of a file of gigabytes. Its
/// header is compared, and so is everything after it, which a payload of any other length or place moves.
/// </remarks>
public static class RoundTrip
{
    public static StreamResult Check(string path)
    {
        var result = new StreamResult { Path = path };

        using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
        using var original = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(file)));

        // The payloads, where the file has them: copied from a stream as long as the file that reads nothing,
        // and not compared. A payload cut short by the end of the file ends there, as the file does: what is
        // written past its end is compared, and differs.
        var payloads = new List<(long Start, long End)>();
        var nothing = new IsoStream(new StreamWrapper(new NothingStream(original.Length)));
        foreach (var mdat in MediaData(container.Children))
        {
            payloads.Add((mdat.Data.Position, Math.Min(mdat.Data.Position + mdat.Data.Length, original.Length)));
            mdat.Data.Stream = nothing;
        }
        payloads.Sort();

        var comparing = new ComparingStream(original, payloads);
        try
        {
            container.Write(new IsoStream(new StreamWrapper(comparing)));
        }
        catch (Exception ex)
        {
            result.Fail(Outcome.SharpFailed, $"write: {ex.GetType().Name}", $"writing threw at {comparing.Position}: {ex.Message}");
            return result;
        }

        result.UnitsCompared = container.Children.Count;
        if (comparing.FirstDifference is long offset)
        {
            // Writing past the end of the file shows here too, as bytes the file has none of.
            string where = Locate(container.Children, (ulong)offset);
            result.Fail(Outcome.Diverged, $"{where}: differs", $"{where}: differs at byte {offset} of {original.Length}");
        }
        else if (comparing.Position < original.Length)
        {
            string where = Locate(container.Children, (ulong)comparing.Position);
            result.Fail(Outcome.Diverged, $"{where}: short", $"{where}: writes {comparing.Position} of {original.Length} bytes");
        }
        else
        {
            result.Outcome = Outcome.Match;
        }

        return result;
    }

    /// <summary>Every 'mdat' with a payload, at any depth.</summary>
    private static IEnumerable<MediaDataBox> MediaData(List<Box>? boxes)
    {
        foreach (var box in boxes ?? [])
        {
            if (box is MediaDataBox mdat && mdat.Data != null)
                yield return mdat;
            foreach (var inner in MediaData(box.Children))
                yield return inner;
        }
    }

    /// <summary>The path of the innermost box read at an offset, found through the sizes read.</summary>
    private static string Locate(List<Box> boxes, ulong offset)
    {
        ulong start = 0;
        foreach (var box in boxes)
        {
            ulong size = box.Header != null && box.Header.Size != 0 ? box.Header.GetBoxSizeInBits() >> 3 : ulong.MaxValue - start;
            if (offset < start + size)
                return Name(box.FourCC);
            start += size;
        }

        return "past the boxes read";
    }

    private static string Name(uint fourCC)
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
    /// A stream that takes writes and compares them with another stream, byte for byte, but for the
    /// ranges it is told to pass over.
    /// </summary>
    private sealed class ComparingStream : Stream
    {
        private readonly Stream expected;
        private readonly List<(long Start, long End)> skipped;
        private byte[] buffer = new byte[1 << 16];
        private long position;

        public ComparingStream(Stream expected, List<(long Start, long End)> skipped)
        {
            this.expected = expected;
            this.skipped = skipped;
        }

        /// <summary>Where the first byte written differs from the original, if one does.</summary>
        public long? FirstDifference { get; private set; }

        public override void Write(byte[] data, int offset, int count)
        {
            while (count > 0)
            {
                // Up to where a range passed over starts or ends
                int n = count;
                bool skip = false;
                foreach (var (start, end) in skipped)
                {
                    if (position < start)
                    {
                        n = (int)Math.Min(n, start - position);
                        break;
                    }
                    if (position < end)
                    {
                        n = (int)Math.Min(n, end - position);
                        skip = true;
                        break;
                    }
                }

                if (skip)
                    expected.Seek(n, SeekOrigin.Current);
                else if (FirstDifference == null)
                    Compare(data, offset, n);

                position += n;
                offset += n;
                count -= n;
            }
        }

        private void Compare(byte[] data, int offset, int count)
        {
            if (buffer.Length < count)
                buffer = new byte[count];

            int read = 0;
            while (read < count)
            {
                int n = expected.Read(buffer, read, count - read);
                if (n == 0)
                    break;
                read += n;
            }

            for (int i = 0; i < count; i++)
            {
                if (i >= read || buffer[i] != data[offset + i])
                {
                    FirstDifference = position + i;
                    break;
                }
            }
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => position;
        public override long Position { get => position; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] data, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }

    /// <summary>A stream as long as a file that reads nothing: what it gives is whatever the buffer held.</summary>
    private sealed class NothingStream(long length) : Stream
    {
        public override int Read(byte[] data, int offset, int count)
        {
            int n = (int)Math.Clamp(length - Position, 0, count);
            Position += n;
            return n;
        }

        public override long Seek(long offset, SeekOrigin origin) => Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Position + offset,
            _ => Length + offset,
        };

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => length;
        public override long Position { get; set; }
        public override void Flush() { }
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] data, int offset, int count) => throw new NotSupportedException();
    }
}
