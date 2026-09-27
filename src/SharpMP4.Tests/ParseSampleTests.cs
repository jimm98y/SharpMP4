using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="H26XTrackBase.ParseSample(byte[])"/>, which splits a sample into its
/// NAL units whichever way it carries them: behind their lengths, as a file holds them, or behind
/// start codes, as an encoder hands them out.
/// </summary>
[TestClass]
public class ParseSampleTests
{
    /// <summary>A sample out of a file: each NAL unit behind its four byte length.</summary>
    [TestMethod]
    public void SplitsALengthPrefixedSample()
    {
        byte[] sample = [0, 0, 0, 3, 0x40, 0x01, 0xAA,
                         0, 0, 0, 4, 0x42, 0x01, 0xBB, 0xCC];

        var units = Parse(sample);

        Assert.AreEqual(2, units.Count);
        CollectionAssert.AreEqual(new byte[] { 0x40, 0x01, 0xAA }, units[0]);
        CollectionAssert.AreEqual(new byte[] { 0x42, 0x01, 0xBB, 0xCC }, units[1]);
    }

    /// <summary>Both start code lengths appear in the same stream, and either may separate two units.</summary>
    [TestMethod]
    public void SplitsOnThreeAndFourByteStartCodes()
    {
        byte[] sample = [0, 0, 0, 1, 0x40, 0x01, 0xAA,
                         0x00, 0x00, 0x01, 0x42, 0x01, 0xBB, 0xCC,
                         0x00, 0x00, 0x00, 0x01, 0x44, 0x01, 0xDD];

        var units = Parse(sample);

        Assert.AreEqual(3, units.Count);
        CollectionAssert.AreEqual(new byte[] { 0x40, 0x01, 0xAA }, units[0]);
        CollectionAssert.AreEqual(new byte[] { 0x42, 0x01, 0xBB, 0xCC }, units[1]);
        CollectionAssert.AreEqual(new byte[] { 0x44, 0x01, 0xDD }, units[2]);
    }

    /// <summary>
    /// A unit ends where the next one's start code begins. Ending it where the next one's payload
    /// begins instead leaves 00 00 01 - or 00 00 00 01 - on the end of it, which is a sequence no
    /// NAL unit may contain, and which then travels with a parameter set into the sample entry
    /// written from it.
    /// </summary>
    [TestMethod]
    public void DoesNotCarryTheNextStartCode()
    {
        var units = Parse([0, 0, 0, 1, 0x42, 0x01, 0x99, 0, 0, 0, 1, 0x44, 0x01, 0x77]);

        CollectionAssert.AreEqual(new byte[] { 0x42, 0x01, 0x99 }, units[0]);
        CollectionAssert.AreEqual(new byte[] { 0x44, 0x01, 0x77 }, units[1]);
    }

    /// <summary>
    /// trailing_zero_8bits may follow a NAL unit, and is not part of it. A zero the encoder meant
    /// to keep is protected by an emulation prevention byte, so it is never the last byte.
    /// </summary>
    [TestMethod]
    public void DropsTrailingZeroBytes()
    {
        var units = Parse([0, 0, 1, 0x26, 0x01, 0x55, 0, 0, 0, 0, 0, 1, 0x02, 0x01, 0x66, 0, 0]);

        Assert.AreEqual(2, units.Count);
        CollectionAssert.AreEqual(new byte[] { 0x26, 0x01, 0x55 }, units[0]);
        CollectionAssert.AreEqual(new byte[] { 0x02, 0x01, 0x66 }, units[1]);
    }

    /// <summary>Zeros inside a unit stay, and so do the emulation prevention bytes protecting them.</summary>
    [TestMethod]
    public void KeepsZerosAndEmulationPreventionBytesInsideAUnit()
    {
        var units = Parse([0, 0, 0, 1, 0x40, 0x01, 0x00, 0x00, 0x03, 0x01, 0x00, 0x11]);

        CollectionAssert.AreEqual(
            new byte[] { 0x40, 0x01, 0x00, 0x00, 0x03, 0x01, 0x00, 0x11 }, units.Single());
    }

    /// <summary>
    /// A stream is read a chunk at a time, so a NAL unit, a start code, or a run of zeros can
    /// straddle two reads - of the read buffer, or of whatever the stream hands over at a time.
    /// Both are squeezed here: one byte per read, out of a two byte buffer.
    /// </summary>
    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(1, 2)]
    [DataRow(2, 3)]
    [DataRow(3, 5)]
    [DataRow(4096, 64 * 1024)]
    public void ReadsTheSameWhateverTheReadsFallOn(int perRead, int bufferSize)
    {
        byte[] stream = [0, 0, 0, 1, 0x40, 0x01, 0x00, 0x00, 0x03, 0xAA,
                         0, 0, 1, 0x42, 0x01, 0xBB,
                         0, 0, 0, 1, 0x44, 0x01, 0xCC, 0x00];

        var units = Copy(Track().ParseSample(new DripStream(stream, perRead), bufferSize));

        Assert.AreEqual(3, units.Count);
        CollectionAssert.AreEqual(new byte[] { 0x40, 0x01, 0x00, 0x00, 0x03, 0xAA }, units[0]);
        CollectionAssert.AreEqual(new byte[] { 0x42, 0x01, 0xBB }, units[1]);
        CollectionAssert.AreEqual(new byte[] { 0x44, 0x01, 0xCC }, units[2]);
    }

    /// <summary>A NAL unit far larger than the buffer it is read into, which has to grow to hold it.</summary>
    [TestMethod]
    public void ReadsAUnitLargerThanTheBufferItStartsWith()
    {
        var unit = new List<byte> { 0x40, 0x01 };
        unit.AddRange(Enumerable.Range(0, 200_000).Select(i => (byte)(i % 251 + 1)));

        var stream = new List<byte> { 0, 0, 0, 1 };
        stream.AddRange(unit);

        CollectionAssert.AreEqual(unit.ToArray(), Parse(stream.ToArray()).Single());
    }

    [TestMethod]
    public void ReadsNothingFromAnEmptyOrStartCodeOnlyStream()
    {
        Assert.AreEqual(0, Track().ParseSample(new MemoryStream([])).Count());
        Assert.AreEqual(0, Parse([0, 0, 0, 1]).Count);
    }

    /// <summary>Bytes before the first start code are not a NAL unit and are passed over.</summary>
    [TestMethod]
    public void SkipsWhatComesBeforeTheFirstStartCode()
    {
        var units = Copy(Track().ParseSample(new MemoryStream([0xDE, 0xAD, 0, 0, 1, 0x40, 0x01, 0x11])));

        CollectionAssert.AreEqual(new byte[] { 0x40, 0x01, 0x11 }, units.Single());
    }

    /// <summary>
    /// Random streams, against a plain scan of the whole buffer. A stream is read a chunk at a
    /// time and the state is carried across the joins, which is where a hand written scanner goes
    /// wrong; this says the two agree whatever the bytes and wherever the joins fall.
    /// </summary>
    [TestMethod]
    public void AgreesWithAScanOfTheWholeBuffer()
    {
        var random = new Random(20260924);

        for (int trial = 0; trial < 200; trial++)
        {
            var stream = new List<byte>();
            int units = random.Next(1, 6);
            for (int i = 0; i < units; i++)
            {
                // Start codes of either length, sometimes with further zeros in front of them.
                stream.AddRange(Enumerable.Repeat((byte)0, random.Next(2, 5)));
                stream.Add(1);

                // Zeros turn up often, but never two in a row followed by a one: an encoder puts
                // an emulation prevention byte in the way, so no NAL unit holds a start code.
                int length = random.Next(1, 40);
                int zeros = 0;
                for (int b = 0; b < length; b++)
                {
                    byte value = zeros >= 2 ? (byte)random.Next(2, 256)
                        : random.Next(3) == 0 ? (byte)0
                        : (byte)random.Next(1, 256);
                    zeros = value == 0 ? zeros + 1 : 0;
                    stream.Add(value);
                }
            }

            var data = stream.ToArray();
            var expected = ScanWholeBuffer(data);
            var actual = Copy(Track().ParseSample(new DripStream(data, random.Next(1, 8)), random.Next(1, 16)));

            Assert.AreEqual(expected.Count, actual.Count, $"trial {trial}: {Convert.ToHexString(data)}");
            for (int i = 0; i < expected.Count; i++)
                CollectionAssert.AreEqual(expected[i], actual[i], $"trial {trial} unit {i}: {Convert.ToHexString(data)}");
        }
    }

    /// <summary>
    /// The reading state belongs to the track, so a second stream started while the first is part
    /// way through says so rather than writing over the NAL unit the first is building.
    /// </summary>
    [TestMethod]
    public void RefusesToReadTwoStreamsAtOnce()
    {
        byte[] stream = [0, 0, 0, 1, 0x40, 0x01, 0xAA, 0, 0, 0, 1, 0x42, 0x01, 0xBB];
        var track = Track();

        using var first = track.ParseSample(new MemoryStream(stream)).GetEnumerator();
        Assert.IsTrue(first.MoveNext());

        Assert.ThrowsExactly<InvalidOperationException>(
            () => track.ParseSample(new MemoryStream(stream)).ToList());

        // The first reader carries on undisturbed, and once it is done another may start.
        Assert.IsTrue(first.MoveNext());
        CollectionAssert.AreEqual(new byte[] { 0x42, 0x01, 0xBB }, first.Current.ToArray());
        Assert.IsFalse(first.MoveNext());

        Assert.AreEqual(2, track.ParseSample(new MemoryStream(stream)).Count());
    }

    private static H265Track Track() => new H265Track(30000, 1001);

    private static List<byte[]> Parse(byte[] sample) => Copy(Track().ParseSample(sample));

    /// <summary>
    /// The units as arrays of their own. What the track hands out points into a buffer it reuses,
    /// so a test that holds on to several of them copies first.
    /// </summary>
    private static List<byte[]> Copy(IEnumerable<ArraySegment<byte>> units) =>
        units.Select(unit => unit.ToArray()).ToList();

    /// <summary>The same split written the obvious way, over a buffer that holds the whole stream.</summary>
    private static List<byte[]> ScanWholeBuffer(byte[] data)
    {
        var startCodes = new List<int>();
        var payloads = new List<int>();
        for (int i = 0; i + 3 < data.Length; i++)
        {
            if (data[i] == 0 && data[i + 1] == 0 && data[i + 2] == 1)
            {
                startCodes.Add(i);
                payloads.Add(i + 3);
                i += 2;
            }
        }

        var units = new List<byte[]>();
        for (int i = 0; i < payloads.Count; i++)
        {
            int start = payloads[i];
            int end = i + 1 < payloads.Count ? startCodes[i + 1] : data.Length;
            while (end > start && data[end - 1] == 0)
                end--;
            if (end > start)
                units.Add(data.Skip(start).Take(end - start).ToArray());
        }
        return units;
    }

    /// <summary>
    /// A stream that hands over only so much per read, as a network or a pipe does. A reader that
    /// takes a short read for the end of the stream stops early on one of these.
    /// </summary>
    private sealed class DripStream : Stream
    {
        private readonly byte[] _data;
        private readonly int _perRead;
        private int _position;

        public DripStream(byte[] data, int perRead)
        {
            _data = data;
            _perRead = perRead;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = Math.Min(Math.Min(count, _perRead), _data.Length - _position);
            Buffer.BlockCopy(_data, _position, buffer, offset, read);
            _position += read;
            return read;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _data.Length;
        public override long Position { get => _position; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
