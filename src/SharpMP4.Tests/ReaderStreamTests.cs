using SharpISOBMFF;
using SharpMP4.Common;
using SharpMP4.Readers;

namespace SharpMP4.Tests;

/// <summary>
/// Tests on reading from a stream that cannot seek - whose 'mdat' boxes are copied into temporary storage as they are
/// read - and from one that ends before its last box does.
/// </summary>
[TestClass]
public class ReaderStreamTests
{
    /// <summary>A stream read through once, as from a network or a pipe.</summary>
    private sealed class ForwardOnlyStream(Stream inner) : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    /// <summary>Temporary storage in memory that says whether it was disposed of.</summary>
    private sealed class TrackedStorageFactory : ITemporaryStorageFactory
    {
        public List<TrackedMemory> Created { get; } = [];

        public IStorage Create(IMp4Logger? logger = null)
        {
            var storage = new TrackedMemory();
            Created.Add(storage);
            return storage;
        }
    }

    private sealed class TrackedMemory : IStorage
    {
        private readonly TemporaryMemory _memory = new();
        public bool IsDisposed { get; private set; }
        public IMp4Logger Logger { get => _memory.Logger; set => _memory.Logger = value; }
        public bool CanStreamSeek() => _memory.CanStreamSeek();
        public void Flush() => _memory.Flush();
        public long GetLength() => _memory.GetLength();
        public long GetPosition() => _memory.GetPosition();
        public long SeekFromCurrent(long offset) => _memory.SeekFromCurrent(offset);
        public long SeekFromEnd(long offset) => _memory.SeekFromEnd(offset);
        public long SeekFromBeginning(long offset) => _memory.SeekFromBeginning(offset);
        public void ReadExactly(byte[] data, int offset, int length) => _memory.ReadExactly(data, offset, length);
        public void Write(byte[] buffer, int offset, int length) => _memory.Write(buffer, offset, length);
        public int ReadByte() => _memory.ReadByte();
        public void WriteByte(byte value) => _memory.WriteByte(value);
        public int Read(byte[] buffer, int offset, int length) => _memory.Read(buffer, offset, length);
        public void Dispose()
        {
            IsDisposed = true;
            _memory.Dispose();
        }
    }

    private static IsoStream Open(byte[] file, bool seekable, ITemporaryStorageFactory? temporary = null)
    {
        Stream input = new MemoryStream(file, writable: false);
        if (!seekable)
            input = new ForwardOnlyStream(input);
        return new IsoStream(new StreamWrapper(input)) { TemporaryStorageFactory = temporary ?? new TemporaryMemoryStorageFactory() };
    }

    private static List<(byte[] Data, long DTS, long PTS, long Duration, bool IsRandomAccessPoint)> ReadSamples(byte[] file, bool seekable, ITemporaryStorageFactory? temporary = null)
    {
        var container = new Container();
        container.Read(Open(file, seekable, temporary));
        var reader = new VideoReader();
        reader.Parse(container);
        return ReaderSampleTableTests.ReadAll(reader, reader.Tracks.Keys.Single());
    }

    /// <summary>
    /// The samples of a file read from a stream that cannot seek are those read from one that can: the chunk offsets and
    /// the runs' data offsets are of the file, and are found in the copies of its 'mdat' boxes, and a 'moof' knows where
    /// in the file it was. A file not fragmented, a fragmented one of several fragments, and one of both.
    /// </summary>
    [TestMethod]
    [DataRow("plain", false)]
    [DataRow("fragmented", false)]
    [DataRow("both", false)]
    [DataRow("plain", true)]
    [DataRow("fragmented", true)]
    public void ReadsTheSamplesOfAStreamThatCannotSeek(string kind, bool temporaryFile)
    {
        var data = Enumerable.Range(0, 12).Select(i => Enumerable.Range(0, 50 + i * 13).Select(b => (byte)(b * 7 + i)).ToArray()).ToArray();
        byte[] file = kind switch
        {
            "plain" => BuildPlain(data),
            "fragmented" => ReaderSampleTableTests.BuildFragmentedFile(data, 100), // a fragment of every few samples
            _ => ReaderSampleTableTests.BuildFileWithSamplesInMoovAndFragments(4, 8).File,
        };

        var seeking = ReadSamples(file, seekable: true);
        var forward = ReadSamples(file, seekable: false, temporaryFile ? new TemporaryFileStorageFactory() : null);

        Assert.IsTrue(seeking.Count >= 8, "samples where the stream seeks");
        Assert.AreEqual(seeking.Count, forward.Count, "samples");
        for (int i = 0; i < seeking.Count; i++)
        {
            CollectionAssert.AreEqual(seeking[i].Data, forward[i].Data, $"sample {i}");
            Assert.AreEqual((seeking[i].DTS, seeking[i].PTS, seeking[i].Duration, seeking[i].IsRandomAccessPoint),
                (forward[i].DTS, forward[i].PTS, forward[i].Duration, forward[i].IsRandomAccessPoint), $"sample {i}");
        }
        if (kind != "both")
        {
            for (int i = 0; i < data.Length; i++)
                CollectionAssert.AreEqual(data[i], forward[i].Data, $"sample {i} as written");
        }
    }

    /// <summary>A box read from a stream that cannot seek is where it is in the file, as from one that can.</summary>
    [TestMethod]
    public void KnowsWhereABoxIsWhereTheStreamCannotSeek()
    {
        byte[] file = ReaderSampleTableTests.BuildFragmentedFile(Enumerable.Range(0, 6).Select(i => new byte[100 + i]).ToArray(), 40);
        var seeking = new Container();
        seeking.Read(Open(file, seekable: true));
        var forward = new Container();
        forward.Read(Open(file, seekable: false));

        CollectionAssert.AreEqual(seeking.Children.Select(x => x.GetBoxOffset()).ToArray(), forward.Children.Select(x => x.GetBoxOffset()).ToArray());
        var seekingMdat = seeking.Children.OfType<MediaDataBox>().Select(x => x.Data.SourcePosition).ToArray();
        CollectionAssert.AreEqual(seekingMdat, forward.Children.OfType<MediaDataBox>().Select(x => x.Data.SourcePosition).ToArray());
        Assert.IsTrue(forward.Children.OfType<MediaDataBox>().All(x => x.Data.IsCopy));
        Assert.IsFalse(seeking.Children.OfType<MediaDataBox>().Any(x => x.Data.IsCopy));
    }

    /// <summary>A temporary file is made in the temporary folder, not in the current directory.</summary>
    [TestMethod]
    public void MakesATemporaryFileInTheTemporaryFolder()
    {
        using var temporary = new TemporaryFile();
        Assert.AreEqual(Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar),
            Path.GetDirectoryName(Path.GetFullPath(temporary.FilePath)));
    }

    /// <summary>The temporary storage of what was copied out of a stream that cannot seek goes with the stream.</summary>
    [TestMethod]
    public void DisposesOfItsTemporaryStorage()
    {
        var factory = new TrackedStorageFactory();
        var stream = Open(BuildPlain([new byte[100], new byte[100]]), seekable: false, factory);
        new Container().Read(stream);
        Assert.AreEqual(1, factory.Created.Count, "the 'mdat' copied");
        Assert.IsFalse(factory.Created[0].IsDisposed);

        stream.Dispose();
        Assert.IsTrue(factory.Created[0].IsDisposed, "temporary storage left behind");
    }

    /// <summary>
    /// A file that ends before its last box does is read as far as it goes, and says it was truncated; one that ends where
    /// a box does, or in a few bytes too few to be one, does not. Alike from a stream that seeks and one that does not.
    /// </summary>
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SaysWhetherTheFileIsTruncated(bool seekable)
    {
        byte[] file = BuildPlain(Enumerable.Range(0, 10).Select(i => new byte[500]).ToArray());
        var whole = new Container();
        whole.Read(Open(file, seekable));
        Assert.IsFalse(whole.IsTruncated, "the whole file");

        // at the end of each box, it is not; within any, it is
        long[] ends = whole.Children.Select(x => x.GetBoxOffset() + (long)x.Size).ToArray();
        foreach (long end in ends)
        {
            var cut = new Container();
            cut.Read(Open(file[..(int)end], seekable));
            Assert.IsFalse(cut.IsTruncated, $"cut at {end}, where a box ends");
        }
        foreach (var box in whole.Children)
        {
            // past its header, before its end
            long start = box.GetBoxOffset(), end = start + (long)box.Size;
            foreach (long at in new[] { start + 9, end - 1, start + (long)box.Size / 2 }.Where(x => x > start + 8 && x < end))
            {
                var cut = new Container();
                cut.Read(Open(file[..(int)at], seekable));
                Assert.IsTrue(cut.IsTruncated, $"cut at {at}, within '{IsoStream.ToFourCC(box.FourCC)}' of {start}..{end}");
            }
        }

        var padded = new Container();
        padded.Read(Open([.. file, 0, 0, 0], seekable));
        Assert.IsFalse(padded.IsTruncated, "a few bytes after the last box");
    }

    private static byte[] BuildPlain(byte[][] samples)
    {
        using var output = new MemoryStream();
        var builder = new SharpMP4.Builders.Mp4Builder(new SharpMP4.Builders.SingleStreamOutput(output));
        var track = new SharpMP4.Tracks.AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        foreach (var sample in samples)
            builder.ProcessRawSample(track.TrackID, sample, 20, true);
        builder.FinalizeMedia();
        return output.ToArray();
    }
}
