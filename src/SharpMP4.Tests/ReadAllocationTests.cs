using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests on what reading a track costs in allocations. Reading used to hand out an array per
/// sample and another per NAL unit, so going through a 25 second recording allocated more than the
/// recording itself; each sample is now read into a buffer the track reuses and handed out as a
/// slice of it.
/// </summary>
[TestClass]
public class ReadAllocationTests
{
    /// <summary>
    /// Reading every sample of a track, past the first, allocates neither the sample nor anything
    /// proportional to it: what is left is the handful of bytes that describe it.
    /// </summary>
    [TestMethod]
    public void ReadingSamplesDoesNotAllocatePerSample()
    {
        const int sampleCount = 200;
        const int sampleSize = 64 * 1024;

        var (reader, trackID) = Read(BuildFile(sampleCount, sampleSize));

        // The first sample makes the buffer the rest are read into.
        Assert.IsNotNull(reader.ReadSample(trackID));

        long before = GC.GetAllocatedBytesForCurrentThread();

        int read = 1;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            Assert.AreEqual(sampleSize, sample.Data.Count);
            read++;
        }

        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.AreEqual(sampleCount, read);
        Assert.IsTrue(allocated < sampleSize,
            $"reading {read} samples of {sampleSize} bytes allocated {allocated} bytes, " +
            $"{allocated / (double)read:F0} per sample");
    }

    /// <summary>
    /// And splitting each sample into its NAL units allocates nothing proportional either: a unit
    /// is a slice of the sample it is already in.
    /// </summary>
    [TestMethod]
    public void SplittingSamplesDoesNotAllocatePerNalUnit()
    {
        const int sampleCount = 200;
        const int sampleSize = 64 * 1024;

        var (reader, trackID) = Read(BuildFile(sampleCount, sampleSize));
        Assert.IsNotNull(reader.ReadSample(trackID));

        long before = GC.GetAllocatedBytesForCurrentThread();

        int units = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            foreach (var unit in reader.ParseSample(trackID, sample.Data))
                units += unit.Count > 0 ? 1 : 0;
        }

        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.AreEqual(sampleCount - 1, units);
        Assert.IsTrue(allocated < sampleSize,
            $"splitting {units} samples allocated {allocated} bytes, {allocated / (double)units:F0} per sample");
    }

    /// <summary>
    /// A track with one large sample among small ones reads them all through one buffer, rather
    /// than growing into it a sample at a time.
    /// </summary>
    [TestMethod]
    public void MakesTheBufferLargeEnoughAtOnce()
    {
        var sizes = new int[64];
        for (int i = 0; i < sizes.Length; i++)
            sizes[i] = 1024 + i * 4096;                 // steadily larger, the worst case for growing

        var (reader, trackID) = Read(BuildFile(sizes));

        long before = GC.GetAllocatedBytesForCurrentThread();

        int read = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            Assert.AreEqual(sizes[read], sample.Data.Count);
            read++;
        }

        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.AreEqual(sizes.Length, read);

        // One buffer of the largest sample, and nothing else worth counting.
        int largest = sizes[sizes.Length - 1];
        Assert.IsTrue(allocated < largest * 2,
            $"reading {read} samples up to {largest} bytes allocated {allocated} bytes");
    }

    private static (VideoReader Reader, uint TrackID) Read(byte[] file)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));

        var reader = new VideoReader();
        reader.Parse(container);
        return (reader, reader.Tracks.Keys.Single());
    }

    private static byte[] BuildFile(int sampleCount, int sampleSize) =>
        BuildFile(Enumerable.Repeat(sampleSize, sampleCount).ToArray());

    /// <summary>A single track file whose samples are opaque bytes of the given sizes.</summary>
    private static byte[] BuildFile(int[] sizes)
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);

        foreach (int size in sizes)
            builder.ProcessRawSample(track.TrackID, new byte[size], 20, isRandomAccessPoint: true);

        builder.FinalizeMedia();
        return output.ToArray();
    }
}
