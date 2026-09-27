using SharpISOBMFF;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Reads a file with SharpMP4 and writes it back, checking the bytes written against the file's own
/// as they come, so that a file of gigabytes needs no copy of itself.
/// </summary>
public static class RoundTrip
{
    public static StreamResult Check(string path)
    {
        var result = new StreamResult { Path = path };

        using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
        using var original = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(file)));

        var comparing = new ComparingStream(original);
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

    /// <summary>A stream that takes writes and compares them with another stream, byte for byte.</summary>
    private sealed class ComparingStream : Stream
    {
        private readonly Stream expected;
        private byte[] buffer = new byte[1 << 16];
        private long position;

        public ComparingStream(Stream expected)
        {
            this.expected = expected;
        }

        /// <summary>Where the first byte written differs from the original, if one does.</summary>
        public long? FirstDifference { get; private set; }

        public override void Write(byte[] data, int offset, int count)
        {
            if (FirstDifference == null)
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

            position += count;
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
}
