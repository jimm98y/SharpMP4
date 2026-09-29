using SharpISOBMFF;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// A file reads the same from a stream that cannot seek as from one that can: what a box's layout depends on - an 'enct'
/// entry's 'frma', whether a string or a box comes next, a 'meta' box's header, zero padding, a 'senc's IV size - is
/// looked ahead at without seeking. Each file of the corpus, read both ways, is written back to the same bytes.
/// </summary>
[TestClass]
public class NonSeekableReadingTests
{
    public TestContext TestContext { get; set; } = null!;

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

    private static byte[]? ReadAndWrite(byte[] file, bool seekable)
    {
        try
        {
            Stream input = new MemoryStream(file, writable: false);
            if (!seekable)
                input = new ForwardOnlyStream(input);
            var container = new Container();
            container.Read(new IsoStream(new StreamWrapper(input)) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() });
            using var output = new MemoryStream();
            container.Write(new IsoStream(new StreamWrapper(output)));
            return output.ToArray();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// A malformed box - an 'stsz' of 2^31 samples in 20 bytes - is kept as its bytes, and the box after it read, from a
    /// stream that cannot seek as from one that can: up to <see cref="IsoStream.MaxRecoverableBoxBytes"/> of it kept to
    /// be read again. With none kept, it and what follows it are lost.
    /// </summary>
    [TestMethod]
    [DataRow(true, PeekableStorage.DefaultMaxRecorded, true)]
    [DataRow(false, PeekableStorage.DefaultMaxRecorded, true)]
    [DataRow(false, 12, true)]
    [DataRow(false, 11, false)]
    [DataRow(false, 0, false)]
    public void KeepsAMalformedBoxAsItsBytes(bool seekable, int maxRecoverableBoxBytes, bool kept)
    {
        byte[] stsz = [0, 0, 0, 20, .. "stsz"u8, 0, 0, 0, 0, 0, 0, 0, 0, 0x80, 0, 0, 0]; // version, flags, size 0, count 2^31
        byte[] free = [0, 0, 0, 8, .. "free"u8];
        Stream input = new MemoryStream([.. stsz, .. free], writable: false);
        if (!seekable)
            input = new ForwardOnlyStream(input);

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(input)) { MaxRecoverableBoxBytes = maxRecoverableBoxBytes });

        if (kept)
        {
            Assert.AreEqual(2, container.Children.Count);
            Assert.IsInstanceOfType(container.Children[0], typeof(UnreadableBox));
            Assert.AreEqual("free", IsoStream.ToFourCC(container.Children[1].FourCC));
        }
        else
        {
            Assert.AreEqual(0, container.Children.Count, "the box, past what is kept of it, lost, and what follows it");
        }
    }

    [TestMethod]
    public void ReadsAsItDoesWhereTheStreamSeeks()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.FileFormatFiles(root).Select(x => x.File)
            .Concat(ConformanceCorpus.ChromiumFiles(root)).Concat(ConformanceCorpus.ShakaFiles(root))
            .Concat(ConformanceCorpus.FirefoxFiles(root)).Concat(ConformanceCorpus.Mp4parseFiles(root))
            .Where(f => new FileInfo(f).Length < 16 << 20)
            .Distinct().ToList();

        int compared = 0, unreadable = 0;
        var failures = new List<string>();
        foreach (string file in files)
        {
            byte[] bytes = File.ReadAllBytes(file);
            byte[]? seeking = ReadAndWrite(bytes, seekable: true);
            if (seeking == null)
            {
                unreadable++;
                continue;
            }
            byte[]? forward = ReadAndWrite(bytes, seekable: false);
            compared++;
            if (forward == null)
                failures.Add($"{Path.GetRelativePath(root, file)}: read where the stream seeks, not where it does not");
            else if (!forward.AsSpan().SequenceEqual(seeking))
                failures.Add($"{Path.GetRelativePath(root, file)}: written as {forward.Length} bytes, {seeking.Length} where the stream seeks, first differing at {FirstDifference(forward, seeking)}");
        }

        TestContext.WriteLine($"{compared} files read both ways, {unreadable} not read where the stream seeks, {failures.Count} read otherwise");
        foreach (string failure in failures)
            TestContext.WriteLine(failure);
        Assert.IsTrue(compared > 0);
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures.Take(40)));
    }

    private static int FirstDifference(byte[] a, byte[] b)
    {
        int n = Math.Min(a.Length, b.Length);
        for (int i = 0; i < n; i++)
        {
            if (a[i] != b[i])
                return i;
        }
        return n;
    }
}
