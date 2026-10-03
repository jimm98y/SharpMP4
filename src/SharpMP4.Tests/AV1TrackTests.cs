using SharpISOBMFF;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="AV1Track"/>: a temporal unit is a sample, without its temporal delimiter and padding (AV1
/// binding 2.4), however its OBUs are given.
/// </summary>
[TestClass]
public class AV1TrackTests
{
    // 16x16 testsrc, three frames, by rav1e (-speed 10): its IVF frames - temporal units - each a temporal delimiter and
    // OBUs: the sequence header and a key frame; a frame not shown and one shown; a frame header showing a frame again
    private static readonly string[] TemporalUnits =
    {
        "12000a0c200000f8cffc42142040404a32ee021002cb1ba2948afe60001000080400000000000061861810088ed1c746d79270ad383331eab736f60c01b0917de1918394509833852641bb252b653b1bc7c67edd41fe9fa1c6e138e65d4b2d1d5692285e4e5a129b143e2a7179e3bdba59b4fcabd416647a437f7c4149e1eca24f3b8ed83af0d728e1803a1e680a3f587eb1bd108869ccf73ff0370a12c0d8e91842fe1cacad0b2681a864f923aeb52e95afc2fa311b1964d5ede3dcea3204214afd73c92d455d3521f36b7eba9ed8499e27db2614bc234821f26be24f62be96bae0bb9846465918863cf593a3a4df8ed7c29ab6021ee435ca2374aadd00a9199dc0d085095c1d036431a0e1e4bc3030a79de9efa588791596f1b615c8157f19f00fbdd0e09b6bd2ca7b7d1aafaabd2cca7fc614b88c7ab0d92a6c91f6ea61ca17e9b80dd7ae4f927b5f29f09bb3e8739695c4cf94c1d370a5ed808133259ab8c2fdbac4fe61f3581793ca4fc4a3ebe97cf5b223a3ca974ace7113d6712dad0012",
        "12003215280908008002030921a18e8afb6071c71c38007ae83215300a2001401006524f471b17f4c104104068007ae8",
        "12001a01c8",
    };

    private const string SequenceHeader = "0a0c200000f8cffc42142040404a";

    // A padding OBU (obu_type 15) of three bytes
    private const string Padding = "7a03aaaaaa";

    /// <summary>The OBUs of a temporal unit, each with its header and size.</summary>
    private static List<byte[]> Obus(byte[] unit)
    {
        var obus = new List<byte[]>();
        for (int p = 0; p < unit.Length;)
        {
            int header = 1 + ((unit[p] & 0x04) != 0 ? 1 : 0);
            int size = 0, shift = 0, i = 0;
            byte b;
            do
            {
                b = unit[p + header + i++];
                size |= (b & 0x7f) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            int total = header + i + size;
            obus.Add(unit.AsSpan(p, total).ToArray());
            p += total;
        }
        return obus;
    }

    /// <summary>A temporal unit as a sample has it: without its temporal delimiter (obu_type 2) and padding (15).</summary>
    private static byte[] Sample(string unit) =>
        Obus(Hex(unit)).Where(o => ((o[0] >> 3) & 0xF) is not (2 or 15)).SelectMany(o => o).ToArray();

    private static List<(byte[] Data, bool Sync)> Feed(AV1Track track, IEnumerable<byte[]> inputs)
    {
        var samples = new List<(byte[], bool)>();
        foreach (byte[] input in inputs)
        {
            track.ProcessSample(input, out var output, out bool sync);
            if (output.Array != null)
                samples.Add((output.ToArray(), sync));
        }
        track.ProcessSample(null, out var last, out bool lastSync);
        if (last.Array != null)
            samples.Add((last.ToArray(), lastSync));
        return samples;
    }

    private static void AssertSamples(List<(byte[] Data, bool Sync)> samples)
    {
        Assert.AreEqual(TemporalUnits.Length, samples.Count);
        for (int i = 0; i < samples.Count; i++)
            CollectionAssert.AreEqual(Sample(TemporalUnits[i]), samples[i].Data, $"sample {i}");
        CollectionAssert.AreEqual(new[] { true, false, false }, samples.Select(s => s.Sync).ToArray());
    }

    /// <summary>
    /// A temporal unit at a time, as an IVF file has them, each led by its temporal delimiter and with a padding OBU: a
    /// sample each, without the delimiter and the padding. Every OBU had the whole of what was given appended for it.
    /// </summary>
    [TestMethod]
    public void MakesATemporalUnitASample()
    {
        var samples = Feed(new AV1Track(), TemporalUnits.Select(u => Hex(u + Padding)));
        AssertSamples(samples);
    }

    /// <summary>The same, an OBU at a time: a frame header shown ended the sample before the tile groups after it.</summary>
    [TestMethod]
    public void MakesTheSameSamplesOfOneObuAtATime()
    {
        var samples = Feed(new AV1Track(), TemporalUnits.SelectMany(u => Obus(Hex(u + Padding))));
        AssertSamples(samples);
    }

    /// <summary>
    /// The same, of samples as a file has them - no temporal delimiters - fed whole and fed an OBU at a time: the end of
    /// the stream ends the last, which was never handed out.
    /// </summary>
    [TestMethod]
    public void MakesTheSameSamplesOfAFilesSamples()
    {
        AssertSamples(Feed(new AV1Track(), TemporalUnits.Select(Sample)));
        AssertSamples(Feed(new AV1Track(), TemporalUnits.SelectMany(u => Obus(Sample(u)))));
    }

    /// <summary>
    /// Written by a builder an OBU at a time, each temporal unit with a duration of its own: as the track hands a unit out
    /// when the next begins, the builder writes it with the duration given with its OBUs, not with those of the next.
    /// </summary>
    [TestMethod]
    public void WritesEachTemporalUnitWithItsOwnDuration()
    {
        int[] durations = { 1000, 2000, 3000 };
        var output = new MemoryStream();
        var builder = new SharpMP4.Builders.Mp4Builder(new SharpMP4.Builders.SingleStreamOutput(output));
        var track = new AV1Track(30000, 1000);
        builder.AddTrack(track);
        for (int i = 0; i < TemporalUnits.Length; i++)
        {
            foreach (byte[] obu in Obus(Hex(TemporalUnits[i])))
                builder.ProcessTrackSample(track.TrackID, obu, durations[i]);
        }
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var stts = Boxes.Find<TimeToSampleBox>(container);
        var written = stts.SampleCount.Zip(stts.SampleDelta, (count, delta) => Enumerable.Repeat((int)delta, (int)count)).SelectMany(d => d).ToArray();
        CollectionAssert.AreEqual(durations, written);
    }

    /// <summary>A frame header shown and the tile group after it are of one temporal unit: it ended at the header.</summary>
    [TestMethod]
    public void KeepsATileGroupWithItsFrameHeader()
    {
        // a frame OBU split in a frame header OBU and a tile group OBU would be more than a test should take apart: the
        // third temporal unit's frame header, which shows a frame again, and a tile group after it stand for them
        var track = new AV1Track();
        Feed(track, new[] { Hex(TemporalUnits[0]), Hex(TemporalUnits[1]) });

        byte[] frameHeaderShown = Hex("1a01c8");         // obu_type 3: show_existing_frame, as the third unit's
        byte[] tileGroup = Hex("2203010203");            // obu_type 4, of three bytes
        track.ProcessSample(Hex("1200"), out _, out _);
        track.ProcessSample(frameHeaderShown, out var none, out _);
        Assert.IsNull(none.Array);
        track.ProcessSample(tileGroup, out none, out _);
        Assert.IsNull(none.Array, "a tile group is of the frame before it");
        track.ProcessSample(null, out var last, out _);
        CollectionAssert.AreEqual(frameHeaderShown.Concat(tileGroup).ToArray(), last.ToArray());
    }

    /// <summary>
    /// A sample in a larger buffer - as a reader hands it, in the buffer it reads every sample into - is parsed as the
    /// slice it is: the whole buffer was.
    /// </summary>
    [TestMethod]
    public void ParsesASampleWhereItLies()
    {
        byte[] sample = Sample(TemporalUnits[1]);
        byte[] buffer = new byte[7].Concat(sample).Concat(Hex(TemporalUnits[0])).ToArray();

        var obus = new AV1Track().ParseSample(buffer, 7, sample.Length).Select(o => o.ToArray()).ToList();

        Assert.AreEqual(2, obus.Count);
        CollectionAssert.AreEqual(sample, obus.SelectMany(o => o).ToArray());

        // given to a track, the sample again
        var track = new AV1Track();
        Feed(track, new[] { Hex(TemporalUnits[0]) });
        var samples = Feed(track, obus);
        Assert.AreEqual(1, samples.Count);
        CollectionAssert.AreEqual(sample, samples[0].Data);
    }

    /// <summary>
    /// The timing of a sequence header's timing_info: a tick of num_units_in_display_tick 1001 of time_scale 30000, a
    /// picture one of them. It was always the fallback, 24000 and 1001.
    /// </summary>
    [TestMethod]
    public void TakesTheTimingOfTheSequenceHeader()
    {
        var track = new AV1Track();
        track.ProcessSample(SequenceHeaderWithTiming(1001, 30000, ticksPerPicture: 1), out _, out _);
        Assert.AreEqual(30000u, track.Timescale);
        Assert.AreEqual(1001, track.DefaultSampleDuration);

        track = new AV1Track();
        track.ProcessSample(SequenceHeaderWithTiming(1, 50, ticksPerPicture: 2), out _, out _);
        Assert.AreEqual(50u, track.Timescale);
        Assert.AreEqual(2, track.DefaultSampleDuration);

        // a file's timing stands
        track = new AV1Track(90000, 3000);
        track.ProcessSample(SequenceHeaderWithTiming(1001, 30000, ticksPerPicture: 1), out _, out _);
        Assert.AreEqual(90000u, track.Timescale);
        Assert.AreEqual(3000, track.DefaultSampleDuration);
    }

    /// <summary>
    /// A clone writes the sample entry of its original at once - its sequence header in it - and an empty track has no
    /// container samples rather than null.
    /// </summary>
    [TestMethod]
    public void ClonesWithTheSequenceHeader()
    {
        Assert.IsNotNull(new AV1Track().GetContainerSamples());
        Assert.AreEqual(0, new AV1Track().GetContainerSamples().Count());

        var original = new AV1Track(25, 1) { Language = "fra", FrameTickOverride = 2 };
        Feed(original, TemporalUnits.Select(Hex));

        var clone = (AV1Track)original.Clone();
        Assert.AreEqual("fra", clone.Language);
        Assert.AreEqual(2, clone.FrameTickOverride);
        CollectionAssert.AreEqual(Hex(SequenceHeader), clone.SequenceHeaderObuRaw);
        var entry = (VisualSampleEntry)clone.CreateSampleEntryBox();
        Assert.AreEqual(16, entry.Width);
        CollectionAssert.AreEqual(Hex(SequenceHeader), entry.Children.OfType<AV1CodecConfigurationBox>().Single().Av1Config.ConfigOBUs);
    }

    /// <summary>A sequence header of profile 0, 16x16, with a timing_info of the values given.</summary>
    private static byte[] SequenceHeaderWithTiming(uint numUnitsInDisplayTick, uint timeScale, uint ticksPerPicture)
    {
        var bits = new List<bool>();
        void Put(ulong value, int count)
        {
            for (int i = count - 1; i >= 0; i--)
                bits.Add(((value >> i) & 1) != 0);
        }
        void Uvlc(uint value)
        {
            // leading zeros, a one, and the value less 2^leadingZeros - 1 in that many bits
            int leadingZeros = 0;
            while ((value + 1) >> (leadingZeros + 1) != 0)
                leadingZeros++;
            Put(0, leadingZeros);
            Put(1, 1);
            Put(value + 1 - (1u << leadingZeros), leadingZeros);
        }

        Put(0, 3);                     // seq_profile
        Put(0, 1);                     // still_picture
        Put(0, 1);                     // reduced_still_picture_header
        Put(1, 1);                     // timing_info_present_flag
        Put(numUnitsInDisplayTick, 32);
        Put(timeScale, 32);
        Put(1, 1);                     // equal_picture_interval
        Uvlc(ticksPerPicture - 1);     // num_ticks_per_picture_minus_1
        Put(0, 1);                     // decoder_model_info_present_flag
        Put(0, 1);                     // initial_display_delay_present_flag
        Put(0, 5);                     // operating_points_cnt_minus_1
        Put(0, 12);                    // operating_point_idc[0]
        Put(0, 5);                     // seq_level_idx[0]
        Put(3, 4);                     // frame_width_bits_minus_1
        Put(3, 4);                     // frame_height_bits_minus_1
        Put(15, 4);                    // max_frame_width_minus_1
        Put(15, 4);                    // max_frame_height_minus_1
        Put(0, 1);                     // frame_id_numbers_present_flag
        Put(0, 3);                     // use_128x128_superblock, enable_filter_intra, enable_intra_edge_filter
        Put(0, 5);                     // enable_interintra_compound, enable_masked_compound, enable_warped_motion, enable_dual_filter, enable_order_hint
        Put(0, 1);                     // seq_choose_screen_content_tools
        Put(0, 1);                     // seq_force_screen_content_tools
        Put(0, 3);                     // enable_superres, enable_cdef, enable_restoration
        Put(0, 1);                     // high_bitdepth
        Put(0, 1);                     // mono_chrome
        Put(0, 1);                     // color_description_present_flag
        Put(0, 1);                     // color_range
        Put(0, 2);                     // chroma_sample_position
        Put(0, 1);                     // separate_uv_delta_q
        Put(0, 1);                     // film_grain_params_present
        Put(1, 1);                     // trailing_one_bit
        while (bits.Count % 8 != 0)
            bits.Add(false);

        var payload = new byte[bits.Count / 8];
        for (int i = 0; i < bits.Count; i++)
            if (bits[i])
                payload[i / 8] |= (byte)(0x80 >> (i % 8));
        return new byte[] { 0x0A, (byte)payload.Length }.Concat(payload).ToArray();
    }
}
