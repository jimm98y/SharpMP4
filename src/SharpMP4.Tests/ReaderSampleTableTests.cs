using System.Diagnostics;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests on how <see cref="VideoReader"/> finds the samples of a sample table - where each is, its times, whether it is a
/// sync sample - and of the fragments after it. A file is written with <see cref="Mp4Builder"/>, read, and its tables then
/// changed in memory to have the runs a test is about, which the reader reads as it would have read them from the file.
/// </summary>
[TestClass]
public class ReaderSampleTableTests
{
    /// <summary>
    /// A file of runs of every kind - chunks of different numbers of samples, durations, composition offsets of both
    /// signs, sync samples here and there - reads each sample as the tables say, walking them from the start for each:
    /// read one after another, and read again after the reader is moved back and on.
    /// </summary>
    [TestMethod]
    public void ReadsEachSampleAsTheTablesSay()
    {
        const int count = 60;
        var (file, data) = BuildFile(count, i => 10 + (i * 7) % 50, compositionOffset: i => (i % 3) * 20, rap: i => i % 7 == 0);
        var container = Read(file);
        var stbl = SampleTable(container);

        // the samples, one after another in one 'mdat', as written
        long[] offsets = NaiveOffsets(stbl, count);
        for (int i = 1; i < count; i++)
            Assert.AreEqual(offsets[i - 1] + data[i - 1].Length, offsets[i], "the samples are written one after another");

        // chunks of 1, 1, 4, 4, 4, 2, 7, 7, 5, 5, 5, 5, 5, 5
        int[] chunks = [1, 1, 4, 4, 4, 2, 7, 7, 5, 5, 5, 5, 5, 5];
        Assert.AreEqual(count, chunks.Sum());
        Rechunk(stbl, chunks, offsets);

        // durations past 2^31, whose products past 2^32 the decode times need whole
        var stts = stbl.Children.OfType<TimeToSampleBox>().Single();
        stts.SampleCount = [3, 1, 10, 6, 40];
        stts.SampleDelta = [0x90000000, 21, 1000, 7, 20];
        stts.EntryCount = 5;

        var ctts = stbl.Children.OfType<CompositionOffsetBox>().Single();
        ctts.Version = 1;
        ctts.SampleCount = [2, 5, 1, 30, 22];
        ctts.SampleOffset0 = [0, -40, 1000, 7, -1];
        ctts.SampleOffset = ctts.SampleOffset0.Select(x => (uint)x).ToArray();
        ctts.EntryCount = 5;

        // an audio track has none: every sample is a sync sample
        var stss = stbl.Children.OfType<SyncSampleBox>().SingleOrDefault();
        if (stss == null)
        {
            stss = new SyncSampleBox();
            stss.SetParent(stbl);
            stbl.Children.Add(stss);
        }
        stss.SampleNumber = [1, 2, 9, 10, 11, 33, 60];
        stss.EntryCount = 7;

        var expected = Naive(stbl, count);
        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();

        for (int i = 0; i < count; i++)
            AssertSample(expected[i], data[i], reader.ReadSample(trackID), i);
        Assert.IsNull(reader.ReadSample(trackID), "past the last sample");

        // moved back, and on, the reader reads the sample it is moved to
        foreach (uint index in new uint[] { 37, 5, 6, 59, 0, 13, 14, 45 })
        {
            reader.Tracks[trackID].SampleIndex = index;
            AssertSample(expected[(int)index], data[(int)index], reader.ReadSample(trackID), (int)index);
        }
    }

    /// <summary>
    /// The times a player seeks by are those reading gives each sample, and a track moved to a sample reads it next: what
    /// a seek to a sync sample, then decoding on to the time wanted, needs.
    /// </summary>
    [TestMethod]
    public void GivesEachSamplesTimesAndSeeksToAny()
    {
        const int count = 60;
        var (file, data) = BuildFile(count, i => 10 + (i * 7) % 50, compositionOffset: i => (i % 3) * 20, rap: i => i % 7 == 0);
        var reader = new VideoReader();
        reader.Parse(Read(file));
        uint trackID = reader.Tracks.Keys.Single();

        var timings = reader.GetSampleTimings(trackID);
        var read = ReadAll(reader, trackID);
        Assert.AreEqual(read.Count, timings.Length);
        for (int i = 0; i < count; i++)
        {
            Assert.AreEqual(read[i].PTS, timings[i].PTS, $"sample {i}: presentation time");
            Assert.AreEqual(read[i].DTS, timings[i].DTS, $"sample {i}: decode time");
            Assert.AreEqual(read[i].Duration, timings[i].Duration, $"sample {i}: duration");
            Assert.AreEqual(read[i].IsRandomAccessPoint, timings[i].IsSyncSample, $"sample {i}: sync sample");
        }

        // after the last sample, and back, and on
        foreach (uint index in new uint[] { 42, 0, 7, 59, 13 })
        {
            reader.SeekSample(trackID, index);
            var sample = reader.ReadSample(trackID);
            CollectionAssert.AreEqual(data[index], sample.Data.ToArray(), $"sample {index}");
            Assert.AreEqual(timings[index].PTS, sample.PTS);
        }
        reader.SeekSample(trackID, count);
        Assert.IsNull(reader.ReadSample(trackID), "moved past the last sample");
    }

    /// <summary>
    /// A fragmented file's samples are timed and sought as those of one that is not: of every fragment, in the order they
    /// are read, with the times reading gives them - a seek into a fragment reads it, from the sample sought, and on into
    /// the fragments after it.
    /// </summary>
    [TestMethod]
    public void GivesEachSamplesTimesOfAFragmentedFileAndSeeksToAny()
    {
        const int count = 60;
        var data = Samples(count, i => 10 + (i * 7) % 50);
        using var output = new MemoryStream();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 2);
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        for (int i = 0; i < count; i++)
            builder.ProcessRawSample(track.TrackID, data[i], 20, i % 7 == 0, (i % 3) * 20);
        builder.FinalizeMedia();

        var container = Read(output.ToArray());
        Assert.IsTrue(container.Children.OfType<MovieFragmentBox>().Count() > 3, "fragments");
        AssertTimesAndSeeks(container, data);
    }

    /// <summary>The samples of a 'moov' and those of the fragments after it are timed and sought as one track.</summary>
    [TestMethod]
    public void GivesEachSamplesTimesOfTheMoovAndTheFragmentsAndSeeksToAny()
    {
        var (file, data) = BuildFileWithSamplesInMoovAndFragments(5, 12);
        AssertTimesAndSeeks(Read(file), data);
    }

    private static void AssertTimesAndSeeks(Container container, byte[][] data)
    {
        var reader = new VideoReader();
        reader.Parse(container);
        Assert.IsTrue(reader.IsFragmented);
        uint trackID = reader.Tracks.Keys.Single();

        var timings = reader.GetSampleTimings(trackID);
        var read = ReadAll(reader, trackID);
        Assert.AreEqual(data.Length, read.Count, "samples read");
        Assert.AreEqual(read.Count, timings.Length, "samples timed");
        for (int i = 0; i < read.Count; i++)
        {
            Assert.AreEqual(read[i].PTS, timings[i].PTS, $"sample {i}: presentation time");
            Assert.AreEqual(read[i].DTS, timings[i].DTS, $"sample {i}: decode time");
            Assert.AreEqual(read[i].Duration, timings[i].Duration, $"sample {i}: duration");
            Assert.AreEqual(read[i].IsRandomAccessPoint, timings[i].IsSyncSample, $"sample {i}: sync sample");
        }

        // after the last sample, and back, and on - each seek then read on for a few samples, across fragments
        int last = data.Length - 1;
        foreach (int index in new[] { last / 2, 0, 7 % data.Length, last, 3, last - 2, 1 })
        {
            reader.SeekSample(trackID, (uint)index);
            for (int i = index; i < Math.Min(data.Length, index + 5); i++)
            {
                var sample = reader.ReadSample(trackID);
                Assert.IsNotNull(sample, $"sample {i}, after a seek to {index}");
                CollectionAssert.AreEqual(data[i], sample.Data.ToArray(), $"sample {i}, after a seek to {index}");
                Assert.AreEqual(timings[i].PTS, sample.PTS, $"sample {i}, after a seek to {index}: presentation time");
                Assert.AreEqual(timings[i].DTS, sample.DTS, $"sample {i}, after a seek to {index}: decode time");
            }
        }

        reader.SeekSample(trackID, (uint)data.Length);
        Assert.IsNull(reader.ReadSample(trackID), "moved past the last sample");

        // and all of it again, from the start
        reader.SeekSample(trackID, 0);
        Assert.AreEqual(data.Length, ReadAll(reader, trackID).Count, "samples read again");
    }

    /// <summary>
    /// Reading a track is linear in its samples: each is found from where the one before it is, not by walking its
    /// chunks, its durations and its composition offsets from the start. A track of 200000 samples, one a chunk, took
    /// 2·10^10 steps to read so.
    /// </summary>
    [TestMethod]
    public void ReadsATrackInTimeLinearInItsSamples()
    {
        const int count = 200_000;
        var (file, _) = BuildFile(4, _ => 16, compositionOffset: _ => 0, rap: _ => true);
        var container = Read(file);
        var stbl = SampleTable(container);
        long first = NaiveOffsets(stbl, 4)[0];

        // one sample of one byte a chunk, every chunk the same byte of the 'mdat'
        var stsz = stbl.Children.OfType<SampleSizeBox>().Single();
        stsz.SampleSize = 1;
        stsz.SampleCount = count;
        stsz.EntrySize = null;
        SetChunkOffsets(stbl, Enumerable.Repeat(first, count).ToArray());
        var stsc = stbl.Children.OfType<SampleToChunkBox>().Single();
        stsc.FirstChunk = [1];
        stsc.SamplesPerChunk = [1];
        stsc.SampleDescriptionIndex = [1];
        stsc.EntryCount = 1;
        var stts = stbl.Children.OfType<TimeToSampleBox>().Single();
        stts.SampleCount = [count / 2, count / 2];
        stts.SampleDelta = [20, 21];
        stts.EntryCount = 2;

        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();

        var watch = Stopwatch.StartNew();
        int read = 0;
        long lastDts = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            lastDts = sample.DTS;
            read++;
        }
        watch.Stop();

        Assert.AreEqual(count, read);
        Assert.AreEqual((count / 2) * 20L + (count / 2 - 1) * 21L, lastDts);
        Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(5), $"reading {count} samples took {watch.Elapsed}");
    }

    /// <summary>
    /// Samples all of one size are looked up as that size, not made into an array of as many of it as the 'stsz' says
    /// there are: one that says 400 million is read in a few bytes, where it took 1.6 GB.
    /// </summary>
    [TestMethod]
    public void LooksUpAConstantSampleSize()
    {
        var (file, data) = BuildFile(4, _ => 16, compositionOffset: _ => 0, rap: _ => true);
        var container = Read(file);
        var stsz = SampleTable(container).Children.OfType<SampleSizeBox>().Single();
        stsz.SampleSize = 16;
        stsz.SampleCount = 400_000_000;
        stsz.EntrySize = null;

        var reader = new VideoReader();
        long before = GC.GetAllocatedBytesForCurrentThread();
        reader.Parse(container);
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.IsTrue(allocated < 1 << 20, $"reading the sample table allocated {allocated} bytes");

        // the samples there are, as many as the chunks have
        uint trackID = reader.Tracks.Keys.Single();
        for (int i = 0; i < data.Length; i++)
            CollectionAssert.AreEqual(data[i], reader.ReadSample(trackID)!.Data.ToArray(), $"sample {i}");
        Assert.IsNull(reader.ReadSample(trackID), "past the samples of the chunks");
    }

    /// <summary>
    /// A duration of the file is 32 bits unsigned: one past 2^31 is not negative, and decode times past 2^32 are whole.
    /// </summary>
    [TestMethod]
    public void ReadsDurationsAndDecodeTimesPast32Bits()
    {
        var (file, _) = BuildFile(3, _ => 16, compositionOffset: _ => 0, rap: _ => true);
        var container = Read(file);
        var stts = SampleTable(container).Children.OfType<TimeToSampleBox>().Single();
        stts.SampleCount = [3];
        stts.SampleDelta = [0x90000000];
        stts.EntryCount = 1;

        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        var samples = Enumerable.Range(0, 3).Select(_ => reader.ReadSample(trackID)!).ToArray();

        CollectionAssert.AreEqual(new long[] { 0, 0x90000000L, 2 * 0x90000000L }, samples.Select(s => s.DTS).ToArray());
        Assert.IsTrue(samples.All(s => s.LongDuration == 0x90000000L), "the duration, whole");
        Assert.IsTrue(samples.All(s => s.Duration == int.MaxValue), "the duration, as much of it as an int holds");
    }

    /// <summary>
    /// A file with an 'mvex' may have samples in its 'moov' too, before its fragments: they are read first, then those of
    /// the fragments, where only the fragments' were read.
    /// </summary>
    [TestMethod]
    public void ReadsTheSamplesOfTheMoovBeforeThoseOfTheFragments()
    {
        var (file, data) = BuildFileWithSamplesInMoovAndFragments(4, 4);
        var reader = new VideoReader();
        reader.Parse(Read(file));
        Assert.IsTrue(reader.IsFragmented);

        uint trackID = reader.Tracks.Keys.Single();
        var samples = ReadAll(reader, trackID);

        Assert.AreEqual(data.Length, samples.Count, "samples");
        for (int i = 0; i < data.Length; i++)
            CollectionAssert.AreEqual(data[i], samples[i].Data, $"sample {i}");
        CollectionAssert.AreEqual(new long[] { 0, 20, 40, 60, 80, 100, 120, 140 }, samples.Select(s => s.DTS).ToArray());
    }

    /// <summary>
    /// What is wrong is said for what it is: no track of an ID, nothing parsed, a fragment's sample of no duration.
    /// </summary>
    [TestMethod]
    public void SaysWhatIsWrong()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => new VideoReader().ReadSample(1));

        var (file, _) = BuildFile(2, _ => 16, compositionOffset: _ => 0, rap: _ => true);
        var reader = new VideoReader();
        reader.Parse(Read(file));
        var unknown = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => reader.ReadSample(99));
        StringAssert.Contains(unknown.Message, "99");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => reader.ParseSample(99, new ArraySegment<byte>(new byte[1])).ToList());

        // a fragment's sample whose duration neither its run, nor the 'tfhd', nor a 'trex' gives
        var fragmented = Read(BuildFragmentedFile(Samples(4, _ => 16), 1000));
        foreach (var traf in fragmented.Children.OfType<MovieFragmentBox>().SelectMany(m => m.Children.OfType<TrackFragmentBox>()))
        {
            traf.Children.OfType<TrackFragmentHeaderBox>().Single().Flags &= ~0x8u;
            foreach (var entry in traf.Children.OfType<TrackRunBox>().SelectMany(t => t._TrunEntry))
                entry.Flags &= ~0x100u;
        }
        reader = new VideoReader();
        reader.Parse(fragmented);
        uint trackID = reader.Tracks.Keys.Single();
        reader.Tracks[trackID].Trex = null;
        var noDuration = Assert.ThrowsExactly<InvalidDataException>(() => reader.ReadSample(trackID));
        StringAssert.Contains(noDuration.Message, "duration");
    }

    /// <summary>
    /// A sample's data is in its track's buffer, which the next sample of the track is read into: both where the file is
    /// fragmented and where it is not, the buffer is made as large as the largest sample at once - of the track, or of
    /// the fragment - rather than replaced as samples outgrow it.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ReadsSamplesIntoOneBufferATrack(bool fragmented)
    {
        var samples = Samples(4, i => i == 2 ? 200_000 : 1000);
        byte[] file = fragmented ? BuildFragmentedFile(samples, 60_000) : BuildFile(samples);
        var reader = new VideoReader();
        reader.Parse(Read(file));
        uint trackID = reader.Tracks.Keys.Single();

        var first = reader.ReadSample(trackID)!;
        byte[] buffer = first.Data.Array!;
        for (int i = 1; i < samples.Length; i++)
        {
            var sample = reader.ReadSample(trackID)!;
            Assert.AreSame(buffer, sample.Data.Array, $"sample {i} in a buffer of its own");
            CollectionAssert.AreEqual(samples[i], sample.Data.ToArray(), $"sample {i}");
        }
    }

    /// <summary>
    /// A track is made from its sample entry's codec configuration wherever it is among the entry's boxes: an 'mp4a' whose
    /// 'btrt' is before its 'esds' is AAC, where it was a generic track made from the 'btrt'.
    /// </summary>
    [TestMethod]
    public void MakesATrackFromTheCodecConfigurationWhereverItIs()
    {
        var (file, _) = BuildFile(2, _ => 16, compositionOffset: _ => 0, rap: _ => true);
        var container = Read(file);
        var entry = SampleTable(container).Children.OfType<SampleDescriptionBox>().Single().Children[0];
        var btrt = new BitRateBox { AvgBitrate = 128000, MaxBitrate = 128000 };
        btrt.SetParent(entry);
        entry.Children.Insert(0, btrt);

        var reader = new VideoReader();
        reader.Parse(container);
        Assert.IsInstanceOfType<AACTrack>(reader.Tracks.Values.Single().Track);
    }

    // --- files ---

    private static byte[][] Samples(int count, Func<int, int> size) =>
        Enumerable.Range(0, count).Select(i =>
        {
            var bytes = new byte[size(i)];
            new Random(i).NextBytes(bytes);
            return bytes;
        }).ToArray();

    private static (byte[] File, byte[][] Data) BuildFile(int count, Func<int, int> size, Func<int, int> compositionOffset, Func<int, bool> rap)
    {
        var data = Samples(count, size);
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        for (int i = 0; i < count; i++)
            builder.ProcessRawSample(track.TrackID, data[i], 20, rap(i), compositionOffset(i));
        builder.FinalizeMedia();
        return (output.ToArray(), data);
    }

    private static byte[] BuildFile(byte[][] samples)
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        foreach (var sample in samples)
            builder.ProcessRawSample(track.TrackID, sample, 20, true);
        builder.FinalizeMedia();
        return output.ToArray();
    }

    internal static byte[] BuildFragmentedFile(byte[][] samples, ulong fragmentLengthInMs)
    {
        using var output = new MemoryStream();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: fragmentLengthInMs);
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        foreach (var sample in samples)
            builder.ProcessRawSample(track.TrackID, sample, 20, true);
        builder.FinalizeMedia();
        return output.ToArray();
    }

    /// <summary>
    /// A file whose 'moov' has samples of its own and an 'mvex', and fragments after it: the 'moov' and 'mdat' of a file
    /// that is not fragmented, the 'mvex' and the fragments of one that is, of the same track, its fragments' decode
    /// times after the 'moov's samples.
    /// </summary>
    internal static (byte[] File, byte[][] Data) BuildFileWithSamplesInMoovAndFragments(int inMoov, int inFragments)
    {
        var data = Samples(inMoov + inFragments, i => 100 + i);
        var plain = Read(BuildFile(data.Take(inMoov).ToArray()));
        var fragmented = Read(BuildFragmentedFile(data.Skip(inMoov).ToArray(), 60_000));

        var moov = plain.Children.OfType<MovieBox>().Single();
        moov.Children.Add(fragmented.Children.OfType<MovieBox>().Single().Children.OfType<MovieExtendsBox>().Single());
        moov.Header = null; // of its new size
        foreach (var tfdt in fragmented.Children.OfType<MovieFragmentBox>().SelectMany(m => m.Children.OfType<TrackFragmentBox>())
            .SelectMany(t => t.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>()))
            tfdt.BaseMediaDecodeTime += (ulong)(20 * inMoov);

        var composite = new Container();
        composite.Children.AddRange(plain.Children);
        composite.Children.AddRange(fragmented.Children.Where(x => x is MovieFragmentBox || x is MediaDataBox));

        // the chunks moved with the 'mdat', which is at another place of the new file
        byte[] file = Write(composite);
        long moved = Read(file).Children.OfType<MediaDataBox>().First().Data.Position - plain.Children.OfType<MediaDataBox>().First().Data.Position;
        var stbl = SampleTable(plain);
        SetChunkOffsets(stbl, ChunkOffsets(stbl).Select(x => x + moved).ToArray());
        return (Write(composite), data);
    }

    internal static Container Read(byte[] file)
    {
        var container = new Container();
        // The stream stays open: the reader goes back to the media data for each sample.
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        return container;
    }

    private static byte[] Write(Container container)
    {
        using var output = new MemoryStream();
        container.Write(new IsoStream(new StreamWrapper(output)));
        return output.ToArray();
    }

    internal static List<(byte[] Data, long DTS, long PTS, long Duration, bool IsRandomAccessPoint)> ReadAll(VideoReader reader, uint trackID)
    {
        var samples = new List<(byte[], long, long, long, bool)>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
            samples.Add((sample.Data.ToArray(), sample.DTS, sample.PTS, sample.LongDuration, sample.IsRandomAccessPoint));
        return samples;
    }

    // --- sample tables ---

    private static SampleTableBox SampleTable(Container container) =>
        container.Children.OfType<MovieBox>().Single().Children.OfType<TrackBox>().Single()
            .Children.OfType<MediaBox>().Single().Children.OfType<MediaInformationBox>().Single()
            .Children.OfType<SampleTableBox>().Single();

    private static long[] ChunkOffsets(SampleTableBox stbl)
    {
        var stco = stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
        return stco != null
            ? stco.ChunkOffset.Select(x => (long)x).ToArray()
            : stbl.Children.OfType<ChunkLargeOffsetBox>().Single().ChunkOffset.Select(x => (long)x).ToArray();
    }

    private static void SetChunkOffsets(SampleTableBox stbl, long[] offsets)
    {
        var stco = stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
        if (stco != null)
        {
            stco.ChunkOffset = offsets.Select(x => (uint)x).ToArray();
            stco.EntryCount = (uint)offsets.Length;
        }
        else
        {
            var co64 = stbl.Children.OfType<ChunkLargeOffsetBox>().Single();
            co64.ChunkOffset = offsets.Select(x => (ulong)x).ToArray();
            co64.EntryCount = (uint)offsets.Length;
        }
    }

    /// <summary>The samples in chunks of so many each, each chunk where its first sample is.</summary>
    private static void Rechunk(SampleTableBox stbl, int[] chunks, long[] offsets)
    {
        var first = new List<uint>();
        var perChunk = new List<uint>();
        int sample = 0;
        var chunkOffsets = new long[chunks.Length];
        for (int c = 0; c < chunks.Length; c++)
        {
            chunkOffsets[c] = offsets[sample];
            sample += chunks[c];
            if (perChunk.Count == 0 || perChunk[^1] != chunks[c])
            {
                first.Add((uint)c + 1);
                perChunk.Add((uint)chunks[c]);
            }
        }
        var stsc = stbl.Children.OfType<SampleToChunkBox>().Single();
        stsc.FirstChunk = first.ToArray();
        stsc.SamplesPerChunk = perChunk.ToArray();
        stsc.SampleDescriptionIndex = first.Select(_ => 1u).ToArray();
        stsc.EntryCount = (uint)first.Count;
        SetChunkOffsets(stbl, chunkOffsets);
    }

    private static uint SizeOf(SampleTableBox stbl, int sample)
    {
        var stsz = stbl.Children.OfType<SampleSizeBox>().Single();
        return stsz.SampleSize > 0 ? stsz.SampleSize : stsz.EntrySize[sample];
    }

    /// <summary>Where each sample is, as 14496-12 8.7.4 has it: every chunk's samples counted out of 'stsc' from the start.</summary>
    private static long[] NaiveOffsets(SampleTableBox stbl, int count)
    {
        var stsc = stbl.Children.OfType<SampleToChunkBox>().Single();
        long[] chunkOffsets = ChunkOffsets(stbl);
        var offsets = new long[count];
        int sample = 0;
        for (int chunk = 0; chunk < chunkOffsets.Length && sample < count; chunk++)
        {
            int run = Array.FindLastIndex(stsc.FirstChunk, f => f <= chunk + 1);
            long offset = chunkOffsets[chunk];
            for (int k = 0; k < stsc.SamplesPerChunk[run] && sample < count; k++, sample++)
            {
                offsets[sample] = offset;
                offset += SizeOf(stbl, sample);
            }
        }
        return offsets;
    }

    /// <summary>Each sample's times and sync, by summing and searching the tables from the start for it.</summary>
    private static (long DTS, long PTS, long Duration, bool IsRandomAccessPoint)[] Naive(SampleTableBox stbl, int count)
    {
        var stts = stbl.Children.OfType<TimeToSampleBox>().Single();
        var ctts = stbl.Children.OfType<CompositionOffsetBox>().SingleOrDefault();
        var stss = stbl.Children.OfType<SyncSampleBox>().SingleOrDefault();
        var result = new (long, long, long, bool)[count];
        for (int sample = 0; sample < count; sample++)
        {
            long dts = 0, duration = 0;
            for (int i = 0; i < sample; i++)
                dts += DeltaOf(stts, i);
            duration = DeltaOf(stts, sample);

            long offset = 0;
            for (int run = 0, start = 0; ctts != null && run < ctts.SampleCount.Length; start += (int)ctts.SampleCount[run], run++)
            {
                if (sample < start + ctts.SampleCount[run])
                {
                    offset = ctts.Version == 0 ? (int)ctts.SampleOffset[run] : ctts.SampleOffset0[run];
                    break;
                }
            }
            result[sample] = (dts, dts + offset, duration, stss == null || stss.SampleNumber.Contains((uint)sample + 1));
        }
        return result;
    }

    private static long DeltaOf(TimeToSampleBox stts, int sample)
    {
        for (int run = 0, start = 0; run < stts.SampleCount.Length; start += (int)stts.SampleCount[run], run++)
        {
            if (sample < start + stts.SampleCount[run])
                return stts.SampleDelta[run];
        }
        return 0;
    }

    private static void AssertSample((long DTS, long PTS, long Duration, bool IsRandomAccessPoint) expected, byte[] data, MediaSample? sample, int index)
    {
        Assert.IsNotNull(sample, $"sample {index}");
        Assert.AreEqual(expected.DTS, sample.DTS, $"sample {index}'s decode time");
        Assert.AreEqual(expected.PTS, sample.PTS, $"sample {index}'s presentation time");
        Assert.AreEqual(expected.Duration, sample.LongDuration, $"sample {index}'s duration");
        Assert.AreEqual(expected.IsRandomAccessPoint, sample.IsRandomAccessPoint, $"sample {index} a sync sample");
        CollectionAssert.AreEqual(data, sample.Data.ToArray(), $"sample {index}'s data");
    }
}
