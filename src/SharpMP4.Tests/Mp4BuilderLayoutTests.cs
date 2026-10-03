using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// What <see cref="Mp4Builder"/> writes besides the sample tables: interleaved chunks, each track's own duration, 64-bit
/// headers where a value needs them, the creation time, and what it does when it is used wrong.
/// </summary>
[TestClass]
public class Mp4BuilderLayoutTests
{
    // the parameter sets of Chromium's bear-640x360-v_frag.mp4
    private static H264Track Video()
    {
        var video = new H264Track();
        video.ProcessSample(Convert.FromHexString("6764001eacd940a02ff9701100000303e90000ea600f162d96"), out _, out _);
        video.ProcessSample(Convert.FromHexString("68ebe3cb22c0"), out _, out _);
        return video;
    }

    private static Mp4Builder Create(Stream output) =>
        new Mp4Builder(new SingleStreamOutput(output)) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() };

    private static Container Read(byte[] file)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        return container;
    }

    private static T Of<T>(TrackBox trak) where T : Box =>
        Boxes.FindOrNull<T>(new Container { Children = new List<Box> { trak } }) ?? throw new InvalidOperationException(typeof(T).Name);

    /// <summary>A sample's bytes: its track and number in them, so that one read back in the wrong place shows.</summary>
    private static byte[] Sample(uint track, int number, int size) =>
        Enumerable.Range(0, size).Select(i => (byte)(i < 4 ? number >> (8 * i) : i == 4 ? (int)track : i)).ToArray();

    /// <summary>
    /// A remuxer feeds one track after another; written as they came, the file was all of one track, then all of the
    /// other, one sample a chunk, and a player had to seek between its ends. Now the chunks - of half a second, several
    /// samples each - alternate, and every sample reads back as it was given.
    /// </summary>
    [TestMethod]
    public void InterleavesTracksFedOneAfterAnother()
    {
        using var output = new MemoryStream();
        var builder = Create(output);
        var video = Video();
        var audio = new AACTrack(2, 44100, 16);
        builder.AddTrack(video);
        builder.AddTrack(audio);

        // ten seconds of each, the video first
        int frame = (int)(video.Timescale / 24);
        for (int i = 0; i < 240; i++)
            builder.ProcessRawSample(video.TrackID, Sample(video.TrackID, i, 64), frame, i % 48 == 0);
        for (int i = 0; i < 44100 * 10 / 1024; i++)
            builder.ProcessRawSample(audio.TrackID, Sample(audio.TrackID, i, 16), 1024, true);
        builder.FinalizeMedia();

        var file = Read(output.ToArray());
        var traks = Boxes.FindAll<TrackBox>(file);
        var chunks = new List<(uint Track, uint Offset)>();
        foreach (var trak in traks)
        {
            uint id = Of<TrackHeaderBox>(trak).TrackID;
            var stco = Of<ChunkOffsetBox>(trak);
            var stsc = Of<SampleToChunkBox>(trak);
            Assert.IsTrue(stsc.SamplesPerChunk.Max() > 1, "several samples a chunk");
            Assert.IsTrue(stco.EntryCount >= 19 && stco.EntryCount <= 21, $"{stco.EntryCount} chunks of half a second in 10 s");
            chunks.AddRange(stco.ChunkOffset.Select(offset => (id, offset)));
        }

        var order = chunks.OrderBy(c => c.Offset).Select(c => c.Track).ToList();
        int switches = order.Zip(order.Skip(1), (a, b) => a != b).Count(x => x);
        Assert.IsTrue(switches >= 35, $"the tracks' chunks alternate: {switches} switches");

        // every sample where the tables say it is
        var reader = new VideoReader();
        reader.Parse(file);
        foreach (var (id, count, size) in new[] { (video.TrackID, 240, 64), (audio.TrackID, 44100 * 10 / 1024, 16) })
        {
            int n = 0;
            for (var sample = reader.ReadSample(id); sample != null; sample = reader.ReadSample(id), n++)
                CollectionAssert.AreEqual(Sample(id, n, size), sample.Data.ToArray(), $"track {id} sample {n}");
            Assert.AreEqual(count, n);
        }
    }

    /// <summary>
    /// Each track's 'tkhd' and 'mdhd' say its own duration - the sum of its samples', in the movie's timescale and its
    /// own - where every track was said to last as long as the longest, rounded through milliseconds.
    /// </summary>
    [TestMethod]
    public void WritesEachTracksOwnDuration()
    {
        using var output = new MemoryStream();
        var builder = Create(output);
        builder.MovieTimescale = 600;
        var video = Video();
        var audio = new AACTrack(2, 44100, 16);
        builder.AddTrack(video);
        builder.AddTrack(audio);
        int frame = (int)(video.Timescale / 25);
        for (int i = 0; i < 50; i++)
            builder.ProcessRawSample(video.TrackID, new byte[8], frame, i == 0); // 2 s
        for (int i = 0; i < 100; i++)
            builder.ProcessRawSample(audio.TrackID, new byte[8], 1023, true); // 2.3197 s
        builder.FinalizeMedia();

        var file = Read(output.ToArray());
        var traks = Boxes.FindAll<TrackBox>(file);
        var videoTrak = traks.Single(t => Of<TrackHeaderBox>(t).TrackID == video.TrackID);
        var audioTrak = traks.Single(t => Of<TrackHeaderBox>(t).TrackID == audio.TrackID);

        Assert.AreEqual(50UL * (ulong)frame, Of<MediaHeaderBox>(videoTrak).Duration);
        Assert.AreEqual(102300UL, Of<MediaHeaderBox>(audioTrak).Duration);
        Assert.AreEqual(1200UL, Of<TrackHeaderBox>(videoTrak).Duration);
        Assert.AreEqual((ulong)Math.Ceiling(102300 * 600 / 44100.0), Of<TrackHeaderBox>(audioTrak).Duration);
        Assert.AreEqual(Of<TrackHeaderBox>(audioTrak).Duration, Boxes.Find<MovieHeaderBox>(file).Duration, "the movie as long as its longest track");
    }

    /// <summary>A duration past 32 bits takes the 64-bit version 1 of the headers, where it wrapped in version 0.</summary>
    [TestMethod]
    public void WritesSixtyFourBitHeadersForLongDurations()
    {
        using var output = new MemoryStream();
        var builder = Create(output);
        builder.MovieTimescale = 90000;
        var track = new AACTrack(2, 48000, 16);
        builder.AddTrack(track);
        for (int i = 0; i < 3; i++)
            builder.ProcessRawSample(track.TrackID, new byte[8], 2_000_000_000, true); // 6e9 at 48 kHz: 34.7 hours
        builder.FinalizeMedia();

        var file = Read(output.ToArray());
        var mdhd = Boxes.Find<MediaHeaderBox>(file);
        Assert.AreEqual<byte>(1, mdhd.Version);
        Assert.AreEqual(6_000_000_000UL, mdhd.Duration);
        var tkhd = Boxes.Find<TrackHeaderBox>(file);
        Assert.AreEqual<byte>(1, tkhd.Version);
        Assert.AreEqual(6_000_000_000UL * 90000 / 48000, tkhd.Duration);
        var mvhd = Boxes.Find<MovieHeaderBox>(file);
        Assert.AreEqual<byte>(1, mvhd.Version);
        Assert.AreEqual(tkhd.Duration, mvhd.Duration);
    }

    /// <summary>
    /// The headers say when the file was made, in seconds since 1904 (14496-12 8.2.2.3), where they said 0: now, or the
    /// time set, so that a file can be written the same each time.
    /// </summary>
    [TestMethod]
    public void WritesTheCreationTime()
    {
        byte[] Build(DateTime? time)
        {
            using var output = new MemoryStream();
            var builder = Create(output);
            builder.CreationTime = time;
            var track = new AACTrack(2, 44100, 16);
            builder.AddTrack(track);
            builder.ProcessRawSample(track.TrackID, new byte[8], 1024, true);
            builder.FinalizeMedia();
            return output.ToArray();
        }

        var set = new DateTime(2024, 2, 29, 12, 0, 0, DateTimeKind.Utc);
        ulong expected = (ulong)(set - new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        var file = Read(Build(set));
        foreach (var (creation, modification) in new[]
        {
            (Boxes.Find<MovieHeaderBox>(file).CreationTime, Boxes.Find<MovieHeaderBox>(file).ModificationTime),
            (Boxes.Find<TrackHeaderBox>(file).CreationTime, Boxes.Find<TrackHeaderBox>(file).ModificationTime),
            (Boxes.Find<MediaHeaderBox>(file).CreationTime, Boxes.Find<MediaHeaderBox>(file).ModificationTime),
        })
        {
            Assert.AreEqual(expected, creation);
            Assert.AreEqual(expected, modification);
        }
        CollectionAssert.AreEqual(Build(set), Build(set), "the same file, of the same time");

        ulong now = (ulong)(DateTime.UtcNow - new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        ulong written = Boxes.Find<MovieHeaderBox>(Read(Build(null))).CreationTime;
        Assert.IsTrue(written <= now + 1 && written + 60 >= now, $"{written} against {now}");
    }

    /// <summary>A duration of 0 is a sample of none, as in the fragmented builder; a negative one the track's default.</summary>
    [TestMethod]
    public void TakesZeroForADurationOfNone()
    {
        using var output = new MemoryStream();
        var builder = Create(output);
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        builder.ProcessRawSample(track.TrackID, new byte[8], 1024, true);
        builder.ProcessRawSample(track.TrackID, new byte[8], -1, true);
        builder.ProcessRawSample(track.TrackID, new byte[8], 0, true);
        builder.FinalizeMedia();

        var stts = Boxes.Find<TimeToSampleBox>(Read(output.ToArray()));
        CollectionAssert.AreEqual(new uint[] { 2, 1 }, stts.SampleCount);
        CollectionAssert.AreEqual(new uint[] { (uint)track.DefaultSampleDuration, 0 }, stts.SampleDelta);
    }

    /// <summary>Given no samples, a 'moov' of tracks with none, where it threw NullReferenceException.</summary>
    [TestMethod]
    public void WritesAnEmptyMovieOfNoSamples()
    {
        using var output = new MemoryStream();
        var builder = Create(output);
        builder.AddTrack(new AACTrack(2, 44100, 16));
        builder.FinalizeMedia();

        var file = Read(output.ToArray());
        Assert.AreEqual(0u, Boxes.Find<SampleSizeBox>(file).SampleCount);
        Assert.AreEqual(0UL, Boxes.Find<MovieHeaderBox>(file).Duration);
        Assert.IsNull(Boxes.FindOrNull<MediaDataBox>(file));
    }

    [TestMethod]
    public void RefusesWhatComesAfterFinalizeMedia()
    {
        using var output = new MemoryStream();
        var builder = Create(output);
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        builder.ProcessRawSample(track.TrackID, new byte[8], 1024, true);
        builder.FinalizeMedia();
        long length = output.Length;

        Assert.ThrowsExactly<InvalidOperationException>(builder.FinalizeMedia);
        Assert.ThrowsExactly<InvalidOperationException>(() => builder.ProcessRawSample(track.TrackID, new byte[8], 1024, true));
        Assert.ThrowsExactly<InvalidOperationException>(() => builder.AddTrack(new AACTrack(2, 44100, 16)));
        Assert.AreEqual(length, output.Length, "written once");
    }

    [TestMethod]
    public void RefusesATrackItDoesNotHave()
    {
        var builder = Create(new MemoryStream());
        builder.AddTrack(new AACTrack(2, 44100, 16));
        var e = Assert.ThrowsExactly<ArgumentException>(() => builder.ProcessRawSample(7, new byte[8], 1024, true));
        StringAssert.Contains(e.Message, "7");
        Assert.ThrowsExactly<ArgumentException>(() => builder.ProcessTrackSample(7, new byte[8]));
    }

    /// <summary>Disposed without FinalizeMedia, nothing is written - an MP4 without its 'moov' is of no use - and its storage is let go.</summary>
    [TestMethod]
    public void WritesNothingDisposedWithoutFinalizeMedia()
    {
        using var output = new MemoryStream();
        var storages = new CountingStorageFactory();
        using (var builder = new Mp4Builder(new SingleStreamOutput(output)) { TemporaryStorageFactory = storages })
        {
            var track = new AACTrack(2, 44100, 16);
            builder.AddTrack(track);
            builder.ProcessRawSample(track.TrackID, new byte[8], 1024, true);
        }
        Assert.AreEqual(0, output.Length);
        Assert.AreEqual(1, storages.Created);
        Assert.AreEqual(1, storages.Disposed);
    }

    /// <summary>The track logs where the builder does.</summary>
    [TestMethod]
    public void GivesTheTrackItsLogger()
    {
        var logger = new DefaultMp4Logger();
        var builder = new Mp4Builder(new SingleStreamOutput(new MemoryStream())) { Logger = logger };
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        Assert.AreSame(logger, track.Logger);
    }
}

/// <summary>Makes in-memory storages, counting how many it made and how many of them were disposed.</summary>
internal sealed class CountingStorageFactory : ITemporaryStorageFactory
{
    public int Created;
    public int Disposed;

    public IStorage Create(IMp4Logger? logger = null)
    {
        Created++;
        return new Counted(this);
    }

    private sealed class Counted : TemporaryMemory
    {
        private readonly CountingStorageFactory _factory;
        private bool _disposed;

        public Counted(CountingStorageFactory factory) => _factory = factory;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                _factory.Disposed++;
            }
            base.Dispose(disposing);
        }
    }
}
