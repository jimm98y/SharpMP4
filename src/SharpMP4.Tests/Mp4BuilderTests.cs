using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="Mp4Builder"/> and the sample tables it writes.</summary>
[TestClass]
public class Mp4BuilderTests
{
    /// <summary>
    /// Durations used to be averaged into a single stts entry, which only holds at constant frame
    /// rate; a track whose samples differ in length came out with the wrong duration.
    /// </summary>
    [TestMethod]
    public void WritesEveryDistinctSampleDuration()
    {
        var durations = new[] { 20, 20, 20, 21, 20, 20 };
        var file = BuildFile(durations.Length, i => durations[i], i => 0);

        var stts = Boxes.Find<TimeToSampleBox>(file);
        Assert.AreEqual(3u, stts.EntryCount);
        CollectionAssert.AreEqual(new uint[] { 3, 1, 2 }, stts.SampleCount);
        CollectionAssert.AreEqual(new uint[] { 20, 21, 20 }, stts.SampleDelta);

        // The track duration is the sum, not the count times an average.
        long total = stts.SampleCount.Zip(stts.SampleDelta, (c, d) => (long)c * d).Sum();
        Assert.AreEqual(durations.Sum(), total);
    }

    [TestMethod]
    public void WritesNoCompositionOffsetsWhenNothingIsReordered()
    {
        var file = BuildFile(4, _ => 20, _ => 0);

        Assert.IsNull(Boxes.FindOrNull<CompositionOffsetBox>(file));
    }

    /// <summary>
    /// Pictures coded out of presentation order need the gap between composition and decode time
    /// recorded, or a player shows them in decode order.
    /// </summary>
    [TestMethod]
    public void WritesCompositionOffsetsWhenSamplesAreReordered()
    {
        // Decode order 0, 3, 1, 2 against a presentation order of 0, 1, 2, 3.
        var presentation = new[] { 0, 3, 1, 2 };
        var file = BuildFile(4, _ => 20, i => (presentation[i] - i) * 20);

        var ctts = Boxes.Find<CompositionOffsetBox>(file);
        Assert.AreEqual<byte>(0, ctts.Version);   // version 0 biases the offsets to be non-negative

        var offsets = Expand(ctts.SampleCount, ctts.SampleOffset);
        Assert.AreEqual(4, offsets.Length);

        // The bias is uniform, so the spacing between samples is what has to survive.
        long bias = offsets[0];
        CollectionAssert.AreEqual(new[] { 0L, 40, -20, -20 }, offsets.Select(o => o - bias).ToArray());
    }

    /// <summary>
    /// Sample positions used to be truncated to 32 bits, so past 4 GB of media data the offsets
    /// wrapped and the file pointed at the wrong bytes with nothing reported. And past 4 GB the
    /// 'mdat' header is 16 bytes, with its size in the largesize, while the offsets were counted
    /// from an 8 byte one: every sample 8 bytes off. Over 5 GB really goes through the builder
    /// here, from a storage that keeps only its length to an output that keeps only the start.
    /// </summary>
    [TestMethod]
    public void WritesSixtyFourBitOffsetsWhenTheMediaIsLargerThanFourGigabytes()
    {
        const int sampleSize = 64 << 20;
        const int sampleCount = 80; // 5.4 GB
        var output = new HeadStream(1 << 20);
        var builder = new Mp4Builder(new SingleStreamOutput(output)) { TemporaryStorageFactory = new BlankStorageFactory() };
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);

        // a second each, so that each is a chunk of its own
        var sample = new ArraySegment<byte>(new byte[sampleSize]);
        for (int i = 0; i < sampleCount; i++)
            builder.ProcessRawSample(track.TrackID, sample, 44100, isRandomAccessPoint: true);
        builder.FinalizeMedia();

        long mediaLength = (long)sampleSize * sampleCount;
        var file = new Container();
        var iso = new IsoStream(new StreamWrapper(new MemoryStream(output.Head)));
        while (Boxes.FindOrNull<MovieBox>(file) == null && file.ReadSingleBox(iso) != 0)
        {
        }

        Assert.IsNull(Boxes.FindOrNull<ChunkOffsetBox>(file));
        var co64 = Boxes.Find<ChunkLargeOffsetBox>(file);
        Assert.AreEqual((uint)sampleCount, co64.EntryCount);

        // the 'mdat' right after the 'moov', its size in the largesize: its samples start 16 bytes into it
        long mdat = file.Children.Sum(box => (long)(box.CalculateSize() >> 3));
        var header = output.Head.AsSpan((int)mdat, 16).ToArray();
        Assert.AreEqual(1u, (uint)(header[0] << 24 | header[1] << 16 | header[2] << 8 | header[3]), "a size of 1: the size is in the largesize");
        Assert.AreEqual("mdat", System.Text.Encoding.ASCII.GetString(header, 4, 4));
        ulong largesize = 0;
        for (int i = 8; i < 16; i++)
            largesize = largesize << 8 | header[i];
        Assert.AreEqual((ulong)(mediaLength + 16), largesize);

        Assert.AreEqual((ulong)(mdat + 16), co64.ChunkOffset[0], "the first sample right after the 16 byte header");
        Assert.AreEqual((ulong)(mdat + 16 + (long)sampleSize * (sampleCount - 1)), co64.ChunkOffset[sampleCount - 1]);
        Assert.IsTrue(co64.ChunkOffset[sampleCount - 1] > uint.MaxValue);
        Assert.AreEqual(mdat + 16 + mediaLength, output.Length, "the file ends where the last sample does");
    }

    [TestMethod]
    public void WritesThirtyTwoBitOffsetsForOrdinaryFiles()
    {
        var file = BuildFile(4, _ => 20, _ => 0);

        Assert.IsNull(Boxes.FindOrNull<ChunkLargeOffsetBox>(file));
        Assert.IsNotNull(Boxes.FindOrNull<ChunkOffsetBox>(file));
    }

    private static int[] Expand(uint[] counts, uint[] offsets)
    {
        var result = new List<int>();
        for (int i = 0; i < counts.Length; i++)
            result.AddRange(Enumerable.Repeat((int)offsets[i], (int)counts[i]));
        return result.ToArray();
    }

    /// <summary>
    /// Builds a single track MP4 and parses it back. The samples are opaque bytes written through
    /// ProcessRawSample, so the test exercises the sample tables rather than a codec parser.
    /// </summary>
    private static Container BuildFile(
        int sampleCount,
        Func<int, int> duration,
        Func<int, int> compositionOffset)
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));

        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);

        for (int i = 0; i < sampleCount; i++)
        {
            builder.ProcessRawSample(track.TrackID, new byte[64], duration(i),
                isRandomAccessPoint: true, compositionOffset(i));
        }

        builder.FinalizeMedia();

        // Read only as far as the moov: the mdat that follows it is the media itself.
        var container = new Container();
        var iso = new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray())));
        while (Boxes.FindOrNull<MovieBox>(container) == null && container.ReadSingleBox(iso) != 0)
        {
        }

        return container;
    }

    /// <summary>Makes storages that keep only how much was written to them, and read back as zeros.</summary>
    private sealed class BlankStorageFactory : ITemporaryStorageFactory
    {
        public IStorage Create(IMp4Logger? logger = null) => new BlankStorage();
    }

    /// <summary>
    /// Keeps only its length, so gigabytes can go through the builder without being kept anywhere: what is read back is
    /// zeros, as the samples written to it were.
    /// </summary>
    private sealed class BlankStorage : IStorage
    {
        private long _length;
        private long _position;

        public IMp4Logger Logger { get; set; } = new DefaultMp4Logger();

        public long GetPosition() => _position;
        public long GetLength() => _length;
        public bool CanStreamSeek() => true;
        public void Flush() { }
        public void Write(byte[] buffer, int offset, int length) { _position += length; _length = Math.Max(_length, _position); }
        public void WriteByte(byte value) => Write(new[] { value }, 0, 1);
        public int ReadByte() => _position < _length ? Zero(ref _position) : -1;
        public int Read(byte[] buffer, int offset, int length)
        {
            int count = (int)Math.Min(length, _length - _position);
            Array.Clear(buffer, offset, count);
            _position += count;
            return count;
        }
        public void ReadExactly(byte[] data, int offset, int length)
        {
            if (Read(data, offset, length) != length)
                throw new EndOfStreamException();
        }
        public long SeekFromCurrent(long offset) => _position += offset;
        public long SeekFromEnd(long offset) => _position = _length + offset;
        public long SeekFromBeginning(long offset) => _position = offset;
        public void Dispose() { }

        private static int Zero(ref long position) { position++; return 0; }
    }

    /// <summary>An output that keeps the first bytes written to it - the 'ftyp' and 'moov' - and counts the rest.</summary>
    private sealed class HeadStream : Stream
    {
        private readonly MemoryStream _head = new();
        private readonly int _keep;
        private long _length;

        public HeadStream(int keep) => _keep = keep;

        public byte[] Head => _head.ToArray();

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _length;
        public override long Position { get => _length; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count)
        {
            int kept = (int)Math.Max(0, Math.Min(count, _keep - _head.Length));
            _head.Write(buffer, offset, kept);
            _length += count;
        }
    }
}
