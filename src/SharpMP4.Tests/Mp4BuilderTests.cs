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
    /// wrapped and the file pointed at the wrong bytes with nothing reported.
    /// </summary>
    [TestMethod]
    public void WritesSixtyFourBitOffsetsWhenTheMediaIsLargerThanFourGigabytes()
    {
        var file = BuildFile(4, _ => 20, _ => 0, new HugeStorage(5_000_000_000));

        Assert.IsNull(Boxes.FindOrNull<ChunkOffsetBox>(file));
        var co64 = Boxes.Find<ChunkLargeOffsetBox>(file);
        Assert.IsTrue(co64.ChunkOffset.All(offset => offset > uint.MaxValue),
            "every chunk offset should sit past the 32-bit range");
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
        Func<int, int> compositionOffset,
        IStorage? storage = null)
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        if (storage != null)
            builder.Storage = storage;

        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);

        for (int i = 0; i < sampleCount; i++)
        {
            builder.ProcessRawSample(track.TrackID, new byte[64], duration(i),
                isRandomAccessPoint: true, compositionOffset(i));
        }

        builder.FinalizeMedia();

        // Read only as far as the moov. The mdat that follows it is the media itself, and with a
        // storage that reports positions past 4 GB there are not that many bytes behind it.
        var container = new Container();
        var iso = new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray())));
        while (Boxes.FindOrNull<MovieBox>(container) == null && container.ReadSingleBox(iso) != 0)
        {
        }

        return container;
    }

    /// <summary>
    /// Reports positions past 4 GB without writing that much, so the switch to 64-bit chunk
    /// offsets can be exercised without a five gigabyte temporary file.
    /// </summary>
    private sealed class HugeStorage : IStorage
    {
        private readonly MemoryStream _inner = new();
        private readonly long _base;

        public HugeStorage(long baseOffset) => _base = baseOffset;

        public IMp4Logger Logger { get; set; } = new DefaultMp4Logger();

        public long GetPosition() => _base + _inner.Position;
        public long GetLength() => _base + _inner.Length;
        public bool CanStreamSeek() => true;
        public void Flush() => _inner.Flush();
        public void Write(byte[] buffer, int offset, int length) => _inner.Write(buffer, offset, length);
        public void WriteByte(byte value) => _inner.WriteByte(value);
        public int ReadByte() => _inner.ReadByte();
        public int Read(byte[] buffer, int offset, int length) => _inner.Read(buffer, offset, length);
        public void ReadExactly(byte[] data, int offset, int length) => _inner.ReadExactly(data, offset, length);
        public long SeekFromCurrent(long offset) => _base + _inner.Seek(offset, SeekOrigin.Current);
        public long SeekFromEnd(long offset) => _base + _inner.Seek(offset, SeekOrigin.End);
        public long SeekFromBeginning(long offset) => _base + _inner.Seek(Math.Max(0, offset - _base), SeekOrigin.Begin);
        public void Dispose() => _inner.Dispose();
    }
}
