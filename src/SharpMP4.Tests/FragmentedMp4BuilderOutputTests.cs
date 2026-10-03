using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// What <see cref="FragmentedMp4Builder"/> writes besides its track runs: fragments written while a track is silent, the
/// 'mfra' that finds them, a storage a fragment rather than a sample, and what it does when it is used wrong.
/// </summary>
[TestClass]
public class FragmentedMp4BuilderOutputTests
{
    // the parameter sets of Chromium's bear-640x360-v_frag.mp4
    private static H264Track Video()
    {
        var video = new H264Track();
        video.ProcessSample(Convert.FromHexString("6764001eacd940a02ff9701100000303e90000ea600f162d96"), out _, out _);
        video.ProcessSample(Convert.FromHexString("68ebe3cb22c0"), out _, out _);
        return video;
    }

    private static FragmentedMp4Builder Create(IMp4Output output, ulong fragmentLength = 1000) =>
        new FragmentedMp4Builder(output, fragmentLength) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() };

    private static Container Read(byte[] file)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        return container;
    }

    private static List<(uint Track, double Start)> Fragments(Container file, Dictionary<uint, uint> timescales) =>
        file.Children.OfType<MovieFragmentBox>()
            .Select(moof => moof.Children.OfType<TrackFragmentBox>().Single())
            .Select(traf => (traf.Children.OfType<TrackFragmentHeaderBox>().Single().TrackID,
                             traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().Single().BaseMediaDecodeTime))
            .Select(f => (f.Item1, f.Item2 / (double)timescales[f.Item1]))
            .ToList();

    /// <summary>
    /// Fragments were written only while every track had one ready, so a track that falls silent - here, audio that
    /// never comes - held back every fragment of the video until the end. Now they are written once they run more
    /// than the delay past it, in the order of their time.
    /// </summary>
    [TestMethod]
    public void WritesFragmentsWhileATrackIsSilent()
    {
        using var output = new MemoryStream();
        var builder = Create(new SingleStreamOutput(output));
        var video = Video();
        var audio = new AACTrack(2, 44100, 16);
        builder.AddTrack(video);
        builder.AddTrack(audio);

        // a second of both, then 30 s of video alone, a key frame each second
        int frame = (int)(video.Timescale / 25);
        for (int i = 0; i < 44100 / 1024; i++)
            builder.ProcessRawSample(audio.TrackID, new byte[16], 1024, true);
        for (int i = 0; i < 25 * 30; i++)
            builder.ProcessRawSample(video.TrackID, new byte[64], frame, i % 25 == 0);

        var timescales = new Dictionary<uint, uint> { [video.TrackID] = video.Timescale, [audio.TrackID] = audio.Timescale };
        var written = Fragments(Read(output.ToArray()), timescales);
        Assert.IsTrue(written.Count(f => f.Track == video.TrackID) >= 25, $"{written.Count} fragments written before the end");

        builder.FinalizeMedia();
        var all = Fragments(Read(output.ToArray()), timescales);
        for (int i = 1; i < all.Count; i++)
            Assert.IsTrue(all[i].Start >= all[i - 1].Start, $"fragment {i} at {all[i].Start:0.00} s after one at {all[i - 1].Start:0.00} s");
        Assert.AreEqual(1, all.Count(f => f.Track == audio.TrackID), "the audio's second, cut off when it was waited on no longer");
    }

    /// <summary>A track fed after another waits within the delay: here a remuxer's, the audio no more than a fragment behind.</summary>
    [TestMethod]
    public void WaitsForATrackWithinTheDelay()
    {
        using var output = new MemoryStream();
        var builder = Create(new SingleStreamOutput(output));
        builder.MaxFragmentDelayInMs = 60_000;
        var video = Video();
        var audio = new AACTrack(2, 44100, 16);
        builder.AddTrack(video);
        builder.AddTrack(audio);

        int frame = (int)(video.Timescale / 25);
        for (int i = 0; i < 25 * 30; i++)
            builder.ProcessRawSample(video.TrackID, new byte[64], frame, i % 25 == 0);
        Assert.AreEqual(0, output.Length, "nothing yet: the audio may still come first");
        for (int i = 0; i < 44100 * 30 / 1024; i++)
            builder.ProcessRawSample(audio.TrackID, new byte[16], 1024, true);
        builder.FinalizeMedia();

        var all = Fragments(Read(output.ToArray()), new Dictionary<uint, uint> { [video.TrackID] = video.Timescale, [audio.TrackID] = audio.Timescale });
        for (int i = 1; i < all.Count; i++)
            Assert.IsTrue(all[i].Start >= all[i - 1].Start, $"fragment {i} at {all[i].Start:0.00} s after one at {all[i - 1].Start:0.00} s");
    }

    /// <summary>
    /// The 'mfra' says where each track's fragments are: of each, the 'moof' it is in, where the file has it, the
    /// presentation time and number of its first sync sample. Offsets were those of a new stream over the output, the
    /// 'mfro' was put in the 'mfra' parented to itself. Of an output of a stream per fragment, the offsets are into the
    /// streams put together, which is the file they make.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void WritesWhereEachFragmentIs(bool blobs)
    {
        using var output = new MemoryStream();
        IMp4Output mp4Output;
        if (blobs)
        {
            var blobOutput = new FragmentedBlobOutput();
            blobOutput.OnFragmentReady += (_, e) => output.Write(e.Data);
            mp4Output = blobOutput;
        }
        else
        {
            mp4Output = new SingleStreamOutput(output);
        }

        var builder = Create(mp4Output);
        var video = Video();
        var audio = new AACTrack(2, 44100, 16);
        builder.AddTrack(video);
        builder.AddTrack(audio);
        int frame = (int)(video.Timescale / 25);
        for (int i = 0; i < 25 * 10; i++)
        {
            // the key frame of each second third in it, after two pictures shown before it
            builder.ProcessRawSample(video.TrackID, new byte[64], frame, i % 25 == 2, i % 25 == 2 ? 2 * frame : 0);
            if (i % 2 == 0)
                builder.ProcessRawSample(audio.TrackID, new byte[16], 1024, true);
        }
        builder.FinalizeMedia();

        byte[] bytes = output.ToArray();
        var file = Read(bytes);
        var mfra = file.Children.OfType<MovieFragmentRandomAccessBox>().Single();
        var mfro = mfra.Children.OfType<MovieFragmentRandomAccessOffsetBox>().Single();
        Assert.AreEqual((uint)(mfra.CalculateSize() >> 3), mfro.ParentSize);
        Assert.AreEqual(bytes.Length - (int)mfro.ParentSize, file.Children.Take(file.Children.IndexOf(mfra)).Sum(b => (int)(b.CalculateSize() >> 3)));

        foreach (var tfra in mfra.Children.OfType<TrackFragmentRandomAccessBox>())
        {
            Assert.AreNotEqual(0u, tfra.NumberOfEntry);
            for (int i = 0; i < tfra.NumberOfEntry; i++)
            {
                long offset = (long)tfra.MoofOffset[i];
                Assert.AreEqual("moof", System.Text.Encoding.ASCII.GetString(bytes, (int)offset + 4, 4), $"track {tfra.TrackID} entry {i}");

                var moof = (MovieFragmentBox)Read(bytes[(int)offset..]).Children[0];
                var traf = moof.Children.OfType<TrackFragmentBox>().Single();
                Assert.AreEqual(tfra.TrackID, traf.Children.OfType<TrackFragmentHeaderBox>().Single().TrackID);
                Assert.AreEqual(1, tfra.TrafNumber[i].Last());
                Assert.AreEqual(1, tfra.TrunNumber[i].Last());

                // the first sync sample, by number from 1, and its presentation time
                var trun = traf.Children.OfType<TrackRunBox>().Single();
                ulong tfdt = traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().Single().BaseMediaDecodeTime;
                // the first video fragment's third, the others' first: they start at a key frame
                int sample = tfra.TrackID == video.TrackID && i == 0 ? 3 : 1;
                Assert.AreEqual((byte)sample, tfra.SampleDelta[i].Last());
                ulong decode = tfdt + (ulong)trun._TrunEntry.Take(sample - 1).Sum(e => (long)e.SampleDuration);
                Assert.AreEqual(decode + (ulong)trun._TrunEntry[sample - 1].SampleCompositionTimeOffset0, tfra.Time[i]);
            }
        }
    }

    /// <summary>A time past 32 bits takes the 64-bit version 1 of the 'tfra', where it wrapped in version 0.</summary>
    [TestMethod]
    public void WritesASixtyFourBitRandomAccessTableWhereItNeedsOne()
    {
        using var output = new MemoryStream();
        var builder = Create(new SingleStreamOutput(output));
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        for (int i = 0; i < 4; i++)
            builder.ProcessRawSample(track.TrackID, new byte[16], 2_000_000_000, true);
        builder.FinalizeMedia();

        var tfra = Boxes.Find<TrackFragmentRandomAccessBox>(Read(output.ToArray()));
        Assert.AreEqual<byte>(1, tfra.Version);
        Assert.AreEqual(4u, tfra.NumberOfEntry);
        Assert.AreEqual(6_000_000_000UL, tfra.Time[3]);
    }

    /// <summary>
    /// A temporary storage was made for every sample and never disposed: a temporary file for each, by default. Now there
    /// is one a fragment, from <see cref="FragmentedMp4Builder.TemporaryStorageFactory"/>, disposed once it is written.
    /// </summary>
    [TestMethod]
    public void MakesAStorageAFragmentAndDisposesItOnceWritten()
    {
        using var output = new MemoryStream();
        var storages = new CountingStorageFactory();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), 1000) { TemporaryStorageFactory = storages };
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        for (int i = 0; i < 44100 * 10 / 1024; i++)
            builder.ProcessRawSample(track.TrackID, new byte[16], 1024, true);

        Assert.IsTrue(storages.Created >= 9 && storages.Created <= 11, $"{storages.Created} storages for 10 fragments");
        Assert.IsTrue(storages.Disposed >= storages.Created - 1, $"{storages.Disposed} of {storages.Created} disposed: all but the fragment being assembled");
        builder.FinalizeMedia();
        Assert.AreEqual(storages.Created, storages.Disposed);
    }

    [TestMethod]
    public void WritesTheInitializationSegmentOfNoSamples()
    {
        using var output = new MemoryStream();
        var builder = Create(new SingleStreamOutput(output));
        builder.AddTrack(new AACTrack(2, 44100, 16));
        builder.FinalizeMedia();

        var file = Read(output.ToArray());
        Assert.IsNotNull(Boxes.FindOrNull<MovieBox>(file));
        Assert.IsNull(Boxes.FindOrNull<MovieFragmentBox>(file));
        Assert.AreEqual(0u, Boxes.Find<TrackFragmentRandomAccessBox>(file).NumberOfEntry);
    }

    [TestMethod]
    public void RefusesWhatComesTooLate()
    {
        using var output = new MemoryStream();
        var builder = Create(new SingleStreamOutput(output));
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        for (int i = 0; i < 100; i++)
            builder.ProcessRawSample(track.TrackID, new byte[16], 1024, true);
        Assert.AreNotEqual(0, output.Length, "the initialization segment written, with the first fragment");

        var late = Assert.ThrowsExactly<InvalidOperationException>(() => builder.AddTrack(new AACTrack(2, 44100, 16)));
        StringAssert.Contains(late.Message, "initialization segment");
        Assert.ThrowsExactly<ArgumentException>(() => builder.ProcessRawSample(9, new byte[16], 1024, true));

        builder.FinalizeMedia();
        long length = output.Length;
        Assert.ThrowsExactly<InvalidOperationException>(builder.FinalizeMedia);
        Assert.ThrowsExactly<InvalidOperationException>(() => builder.ProcessRawSample(track.TrackID, new byte[16], 1024, true));
        Assert.AreEqual(length, output.Length, "finalized once");
    }

    /// <summary>Disposed without FinalizeMedia, nothing more is written, and the storages of fragments not written are let go.</summary>
    [TestMethod]
    public void WritesNothingMoreDisposedWithoutFinalizeMedia()
    {
        using var output = new MemoryStream();
        var storages = new CountingStorageFactory();
        long length;
        using (var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), 1000) { TemporaryStorageFactory = storages })
        {
            var track = new AACTrack(2, 44100, 16);
            builder.AddTrack(track);
            for (int i = 0; i < 100; i++)
                builder.ProcessRawSample(track.TrackID, new byte[16], 1024, true);
            length = output.Length;
        }
        Assert.AreEqual(length, output.Length);
        Assert.IsTrue(storages.Created > 0);
        Assert.AreEqual(storages.Created, storages.Disposed);
    }

    /// <summary>The track logs where the builder does, as of <see cref="Mp4Builder"/>.</summary>
    [TestMethod]
    public void GivesTheTrackItsLogger()
    {
        var logger = new DefaultMp4Logger();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(new MemoryStream()), 1000) { Logger = logger };
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);
        Assert.AreSame(logger, track.Logger);
    }
}
