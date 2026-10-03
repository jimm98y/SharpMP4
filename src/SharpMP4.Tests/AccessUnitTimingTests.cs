using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// H.264 given a NAL unit at a time, each with the timing of its access unit: a track gives an access unit back only once
/// the next one starts, so the builders wrote it with the next one's duration and composition offset, and the last, given
/// back by the flush at the end, with the default and none - or, fragmented, not at all. And of pictures coded out of
/// order, the edit list puts the first shown at 0, as ffmpeg does, rather than as late as it is reordered by.
/// </summary>
[TestClass]
public class AccessUnitTimingTests
{
    // Ten frames of 32x32 test pattern from x264 with two B frames - I P B B P B B P B B in decode order - its SEI dropped.
    private const string Stream =
        "AAAAAWf0AAqRmyktgIgAAAMACAAAAwGQeJEssAAAAAFozg8ZIAAAAWWIhADoIQAENAIB5Yb15UwBaQZC2qE+ABTZETG/QyO4ADoMojmJNNS4V5VXAmaJP1BdNN/ZgPf4Nw9+EAACAAADAYAqWHsAAFAA5pgKNgdHzwGQAAIBYAAgDgB9z8ABECQhiAdDgv1G3wTE0AEbbfKbILkr8OlL+ZgAFhO8L6PfhAABUAoLlh//wAEAAEAkoAUDclNFFQQ1ZVcP4oAD5EY1QhEcX0I4DKXv0RgNwpr9mAACJAiEg1Ee/CJFLD//hCAHAwgAeRCaBGSJBMU3AjAfr/wcAAQAQAOAXLASArcofEOX9OMTC05pDd2gwIzMjQ26HRnwAMFM12pQ8DvijX9vDBCAAEAAQJTUxkgZNBvfd8AHEUhCsBOSF5ow0CYAehwACgBAVLALXARMpgDJbpfzNHEYl+0GBT81zkz//gA4AAgDjB5vxREokDWSjHUpRB0zlgFi4AhK9gM63S/ojIMo2muNdu0GAsZENFIVHE9KA/D46ADjBhA9PkdQ6F6cIkuGU4/AB4IgAXUjzRBJ5X1e/yAABAEAAEAYDLhUAfcAOiAJwAgBWXiweLgJg9ykE660Uey/xIAG4WPEs0LTUE5W/hIABa0AwMjsAAuYMwAVYToh0wBioT2/xAFhEBg0M8ngFJQel1OXuFGJABYxjQynvcwBbfv5mAP4c+54AAgCDEgUmomrYRnGtKO6Rv8QAWBAAWAMDMYaJ4BLC+Y7qjmUCsfhAAWFtgyCWur8wP/79iATjyx0ABAAQFSDLJA3C3ekwdJVs26v4APYQZqB6pAw+b9XgeAAIATAMAOuDhAbEBxtu8AY7A01igOJoTgA3DzobgKExygDv3aEgAG5AA4Hw2AAIswAcAwKbcWdJmwKohJ/iQFhkVAA2gQIjPAdmQR8IGJb3aFMQAFgMwEySm33aJKpTwABAwVQAcoRUIZBeinWiin/EgCwJABYA0BvHLmeBWZAHhBEUW6QkAFhwWkwAfD7iAZzdogCMVLgAAAAAUGaIwCCfJF00vu/5Irba+q8AAAAAUGeQUAkiwAAAAEBnmJAURYAAAABQZpmNECk+JihgykyBmpaxu0dfXijbPeJ8UOMeFYraPYkf/iYoMGrTIfGWvp5o9d+KacUQ8UHH/C4uMeZYj1iwAAAAAFBnoRFEShBFgAAAAEBnqVAQRYAAAABQZqpNEGT4mKBiHhjLQE2DFJkPtT4Tq8XFyZAYrFZ6Pw/Ky+/xMsBifA1+WoaYNtMjBaUlkJRWKx/1Bi4poGMeEeZlRpwAAAAAUGex0UVKKRYAAAAAQGe6EDEWA==";

    // Where each access unit, in decode order, is in presentation order.
    private static readonly int[] DisplayIndex = { 0, 3, 1, 2, 6, 4, 5, 9, 7, 8 };

    // Each access unit's own duration, all different, so that one written with another's shows.
    private static readonly int[] Durations = Enumerable.Range(0, 10).Select(i => 3000 + 300 * i).ToArray();

    /// <summary>The decode time of each access unit: the durations before it.</summary>
    private static long[] DecodeTimes() => Enumerable.Range(0, 10).Select(i => (long)Durations.Take(i).Sum()).ToArray();

    /// <summary>
    /// Composition minus decode time of each access unit: its presentation time the decode time of the access unit
    /// that many into the stream, plus two frames - a delay so that none is shown before it is decoded.
    /// </summary>
    private static int[] CompositionOffsets()
    {
        var decode = DecodeTimes();
        return Enumerable.Range(0, 10).Select(i => (int)(decode[DisplayIndex[i]] + 6000 - decode[i])).ToArray();
    }

    /// <summary>The NAL units of the stream, each with the access unit it is of: a parameter set of the picture after it.</summary>
    private static List<(byte[] Nal, int AccessUnit)> NalUnits()
    {
        byte[] stream = Convert.FromBase64String(Stream);
        var starts = new List<int>();
        for (int i = 0; i + 2 < stream.Length; i++)
        {
            if (stream[i] == 0 && stream[i + 1] == 0 && stream[i + 2] == 1)
                starts.Add(i + 3);
        }

        var units = new List<(byte[], int)>();
        int accessUnit = -1;
        for (int k = 0; k < starts.Count; k++)
        {
            int end = k + 1 < starts.Count ? starts[k + 1] - 3 : stream.Length;
            while (end > starts[k] && stream[end - 1] == 0)
                end--; // the leading zero of a four byte start code
            byte[] nal = stream[starts[k]..end];
            int type = nal[0] & 0x1f;
            bool isSlice = type == 1 || type == 5;
            if (isSlice)
                accessUnit++;
            units.Add((nal, isSlice ? accessUnit : accessUnit + 1));
        }
        return units;
    }

    internal static byte[] Build(IMp4Builder builder, MemoryStream output)
    {
        var track = new H264Track { TimescaleOverride = 90000 };
        builder.AddTrack(track);
        var offsets = CompositionOffsets();
        foreach (var (nal, accessUnit) in NalUnits())
            builder.ProcessTrackSample(track.TrackID, nal, Durations[accessUnit], offsets[accessUnit]);
        builder.FinalizeMedia();
        return output.ToArray();
    }

    private static Container Read(byte[] file)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        return container;
    }

    private static long[] Expand(uint[] counts, uint[] values)
    {
        var result = new List<long>();
        for (int i = 0; i < counts.Length; i++)
            result.AddRange(Enumerable.Repeat((long)values[i], (int)counts[i]));
        return result.ToArray();
    }

    [TestMethod]
    public void WritesEachAccessUnitWithItsOwnTiming()
    {
        using var output = new MemoryStream();
        var file = Read(Build(new Mp4Builder(new SingleStreamOutput(output)) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() }, output));

        var stts = Boxes.Find<TimeToSampleBox>(file);
        CollectionAssert.AreEqual(Durations.Select(d => (long)d).ToArray(), Expand(stts.SampleCount, stts.SampleDelta));

        // ctts version 0 holds the offsets as they were given, none being negative
        var ctts = Boxes.Find<CompositionOffsetBox>(file);
        CollectionAssert.AreEqual(CompositionOffsets().Select(o => (long)o).ToArray(), Expand(ctts.SampleCount, ctts.SampleOffset));
    }

    [TestMethod]
    public void WritesEachAccessUnitWithItsOwnTimingFragmented()
    {
        using var output = new MemoryStream();
        var file = Read(Build(new FragmentedMp4Builder(new SingleStreamOutput(output), 60_000) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() }, output));

        var entries = Boxes.FindAll<TrackRunBox>(file).SelectMany(trun => trun._TrunEntry).ToList();
        Assert.AreEqual(10, entries.Count, "every access unit, the last too");
        CollectionAssert.AreEqual(Durations.Select(d => (uint)d).ToArray(), entries.Select(e => e.SampleDuration).ToArray());
        CollectionAssert.AreEqual(CompositionOffsets(), entries.Select(e => e.SampleCompositionTimeOffset0).ToArray());
    }

    /// <summary>The edit list starts the presentation at the earliest composition time: the first picture shown at 0.</summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void StartsThePresentationAtTheFirstPictureShown(bool fragmented)
    {
        using var output = new MemoryStream();
        IMp4Builder builder = fragmented
            ? new FragmentedMp4Builder(new SingleStreamOutput(output), 60_000) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() }
            : new Mp4Builder(new SingleStreamOutput(output)) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() };
        var file = Read(Build(builder, output));

        var decode = DecodeTimes();
        var offsets = CompositionOffsets();
        long first = Enumerable.Range(0, 10).Min(i => decode[i] + offsets[i]);

        var elst = Boxes.Find<EditListBox>(file);
        Assert.AreEqual(1u, elst.EntryCount);
        Assert.AreEqual(first, elst.MediaTime[0]);
        Assert.AreEqual((short)1, elst.MediaRateInteger[0]);
        if (!fragmented)
        {
            // the track's duration in the movie's timescale, as its 'tkhd' says
            var tkhd = Boxes.Find<TrackHeaderBox>(file);
            Assert.AreEqual((ulong)Math.Ceiling(Durations.Sum() * 1000.0 / 90000), elst.EditDuration[0]);
            Assert.AreEqual(tkhd.Duration, elst.EditDuration[0]);
        }

        // shown from 0, in the order the pictures were given
        var presented = Enumerable.Range(0, 10).Select(i => decode[i] + offsets[i] - elst.MediaTime[0]).ToArray();
        Assert.AreEqual(0, presented.Min());
        CollectionAssert.AreEqual(DisplayIndex, presented.Select(p => presented.Count(q => q < p)).ToArray());
    }

    /// <summary>
    /// A caller giving the timing with the slices alone, the parameter sets with none: the first access unit takes the
    /// slice's, not the default the parameter sets came with.
    /// </summary>
    [TestMethod]
    public void TakesTheTimingOfTheSliceOverTheDefaultOfTheParameterSets()
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output)) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() };
        var track = new H264Track { TimescaleOverride = 90000 };
        builder.AddTrack(track);
        var offsets = CompositionOffsets();
        foreach (var (nal, accessUnit) in NalUnits())
        {
            int type = nal[0] & 0x1f;
            if (type == 7 || type == 8)
                builder.ProcessTrackSample(track.TrackID, nal);
            else
                builder.ProcessTrackSample(track.TrackID, nal, Durations[accessUnit], offsets[accessUnit]);
        }
        builder.FinalizeMedia();

        var stts = Boxes.Find<TimeToSampleBox>(Read(output.ToArray()));
        CollectionAssert.AreEqual(Durations.Select(d => (long)d).ToArray(), Expand(stts.SampleCount, stts.SampleDelta));
    }
}
