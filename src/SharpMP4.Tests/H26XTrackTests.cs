using SharpISOBMFF;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="H264Track"/>, <see cref="H265Track"/> and <see cref="H266Track"/>: what they keep for the sample
/// entry, what they say of the samples they hand out, and what a clone of one has.
/// </summary>
[TestClass]
public class H26XTrackTests
{
    // 16x16 grey, three pictures - an IDR picture then two P pictures - by x264 (-qp 40 -bf 0), its SEI left out
    private const string H264Sps = "6764000aacb23d80880000030008000003019078913240";
    private const string H264Pps = "68ebc0e4b22c";
    private static readonly string[] H264Pictures = { "6588843ff0f839", "419a3b1fdf80", "419a4f0864ca61ffdf81" };

    // the same, 32x16: an SPS of the same ID, of another size
    private const string H264Sps32 = "6764000aacb2176022000003000200000300641e244c90";

    // 16x16 grey, three pictures - an IDR picture then two P pictures - by x265 (-qp 40, bframes=0), its SEI left out
    private const string H265Vps = "40010c01ffff01600000030090000003000003001e928090";
    private const string H265Sps = "42010101600000030090000003000003001ea08845964aabcaf0168080000003008000000c84";
    private const string H265Pps = "4401c171a112";
    private static readonly string[] H265Pictures = { "2801af0b60f59b50", "0201d0097e10c61cc0fce4", "0201d011ffd443061c40fce4" };

    // 16x16 grey, three pictures, by vvenc (-qp 40): its SPS, PPS and the slices of each picture
    private const string H266Sps = "007900ad0210800000822111a801cdd36e991a4526f384d958c10209e00ca0c5842183110819220d487a3d5a92f249a92c9116a22f1126a22452444992224d4659210b5085e10bd5a92f25ea19418b100c5880810810102a102020690810102c408081104040b2080812101064081108102c8408122041a081241070832045a104908710d0972395ffa5d97f9c900000030010000003019188";
    private const string H266Pps = "0081000110888907ee08";
    private static readonly string[] H266Pictures = { "0039c404602a92f0f961ddb4c314", "00169400d08a509fc08cb0", "000e9408d08b589fc08cb0" };

    private const string Uuid = "0123456789abcdef0123456789abcdef";

    /// <summary>A prefix SEI NAL unit of one user_data_unregistered message (payloadType 5), its one byte of data given.</summary>
    private static byte[] UnregisteredSei(string header, byte data) => Hex(header + "0511" + Uuid + data.ToString("x2") + "80");

    /// <summary>A prefix SEI NAL unit of one user_data_registered_itu_t_t35 message (payloadType 4): of a picture, as captions are.</summary>
    private static byte[] RegisteredSei(string header, byte data) => Hex(header + "0404b50031" + data.ToString("x2") + "80");

    /// <summary>
    /// A track says a sample is a sync sample only of the sample it hands out: a call that hands out none - a slice of the
    /// picture assembled, an SEI after it - said the access unit being assembled was one, an IDR picture's.
    /// </summary>
    [TestMethod]
    public void SaysASampleIsASyncSampleOnlyOfOneHandedOut()
    {
        var calls = new List<(bool Output, bool Sync)>();
        void Feed(ITrack track, IEnumerable<byte[]> units)
        {
            foreach (byte[] unit in units)
            {
                track.ProcessSample(unit, out var output, out bool sync);
                calls.Add((output.Array != null, sync));
            }
            track.ProcessSample(null, out var last, out bool lastSync);
            calls.Add((last.Array != null, lastSync));
        }

        // an SEI after each picture: of the next access unit, held, which hands nothing out
        Feed(new H264Track(), new[] { H264Sps, H264Pps }.Select(Hex)
            .Concat(H264Pictures.SelectMany(p => new[] { Hex(p), UnregisteredSei("06", 1) })));
        Feed(new H265Track(), new[] { H265Vps, H265Sps, H265Pps }.Select(Hex)
            .Concat(H265Pictures.SelectMany(p => new[] { Hex(p), UnregisteredSei("4e01", 1) })));
        Feed(new H266Track(), new[] { H266Sps, H266Pps }.Select(Hex)
            .Concat(H266Pictures.SelectMany(p => new[] { Hex(p), UnregisteredSei("00b9", 1) })));

        Assert.IsTrue(calls.Any(c => c.Output && c.Sync), "the IDR pictures are sync samples");
        Assert.AreEqual(0, calls.Count(c => !c.Output && c.Sync), "a call that hands out no sample says it is no sync sample");
    }

    /// <summary>
    /// The sample entry says the size of the lengths the samples are written with: a track of a file of 2 byte lengths
    /// wrote 'avcC', 'hvcC' and 'vvcC' of 4, its samples of 2.
    /// </summary>
    [TestMethod]
    public void WritesTheLengthSizeTheSamplesHave()
    {
        var tracks = new (H26XTrackBase Track, string[] ParameterSets, string[] Pictures)[]
        {
            (new H264Track { NalLengthSize = 2 }, new[] { H264Sps, H264Pps }, H264Pictures),
            (new H265Track { NalLengthSize = 2 }, new[] { H265Vps, H265Sps, H265Pps }, H265Pictures),
            (new H266Track { NalLengthSize = 2 }, new[] { H266Sps, H266Pps }, H266Pictures),
        };

        foreach (var (track, parameterSets, pictures) in tracks)
        {
            track.ProcessSample(Hex(parameterSets[0]), out _, out _);
            foreach (string unit in parameterSets.Skip(1).Concat(pictures))
                track.ProcessSample(Hex(unit), out _, out _);
            track.ProcessSample(null, out var last, out _);

            // the last picture, behind a length of 2 bytes
            CollectionAssert.AreEqual(new byte[] { 0, (byte)Hex(pictures[^1]).Length }.Concat(Hex(pictures[^1])).ToArray(), last.ToArray(), track.GetType().Name);

            var entry = track.CreateSampleEntryBox();
            int lengthSize = entry.Children.Select(b => b switch
            {
                AVCConfigurationBox avcC => avcC._AVCConfig.LengthSizeMinusOne + 1,
                HEVCConfigurationBox hvcC => hvcC._HEVCConfig.LengthSizeMinusOne + 1,
                VvcConfigurationBox vvcC => vvcC._VvcConfig._LengthSizeMinusOne + 1,
                _ => 0,
            }).Single(x => x > 0);
            Assert.AreEqual(2, lengthSize, track.GetType().Name);
        }
    }

    /// <summary>
    /// SEI NAL units go to the 'hvcC' and 'vvcC' only of declarative messages, each once and so many at the most: every
    /// prefix SEI was kept - a reference compared, no two alike - and the configuration record grew with the stream.
    /// </summary>
    [TestMethod]
    public void KeepsDeclarativeSeiOnceInTheConfigurationRecord()
    {
        var h265 = new H265Track();
        foreach (string unit in new[] { H265Vps, H265Sps, H265Pps })
            h265.ProcessSample(Hex(unit), out _, out _);
        var h266 = new H266Track();
        foreach (string unit in new[] { H266Sps, H266Pps })
            h266.ProcessSample(Hex(unit), out _, out _);

        foreach (var (track, header) in new (H26XTrackBase, string)[] { (h265, "4e01"), (h266, "00b9") })
        {
            // the same settings at each key frame: once
            for (int i = 0; i < 20; i++)
                track.ProcessSample(UnregisteredSei(header, 1), out _, out _);
            // captions of each picture: none
            for (int i = 0; i < 20; i++)
                track.ProcessSample(RegisteredSei(header, (byte)i), out _, out _);
        }

        Assert.AreEqual(1, h265.PrefixSeiRaw.Count);
        Assert.AreEqual(1, h265.PrefixSei.Count);
        CollectionAssert.AreEqual(UnregisteredSei("4e01", 1), h265.PrefixSeiRaw[0]);
        Assert.AreEqual(1, h266.PrefixSeiRaw.Count);
        Assert.AreEqual(1, h266.PrefixSei.Count);

        // a declarative message new at each: so many at the most
        for (int i = 2; i < 40; i++)
            h265.ProcessSample(UnregisteredSei("4e01", (byte)i), out _, out _);
        Assert.AreEqual(H26XTrackBaseLimit, h265.PrefixSeiRaw.Count);

        var hvcC = h265.CreateSampleEntryBox().Children.OfType<HEVCConfigurationBox>().Single();
        Assert.AreEqual(H26XTrackBaseLimit, hvcC._HEVCConfig.NalUnit[3].Length, "the SEI array");
    }

    private const int H26XTrackBaseLimit = 8;

    /// <summary>
    /// A parameter set that changes is the one the entry is made of - the first of an ID was kept, the samples after a
    /// new one with the old - and one that changes after the entry was made is said to.
    /// </summary>
    [TestMethod]
    public void KeepsTheLatestParameterSetOfAnId()
    {
        var logger = new CapturingLogger();
        var track = new H264Track { Logger = logger };
        foreach (string unit in new[] { H264Sps, H264Pps, H264Pictures[0] })
            track.ProcessSample(Hex(unit), out _, out _);

        track.ProcessSample(Hex(H264Sps32), out _, out _);
        CollectionAssert.AreEqual(Hex(H264Sps32), track.SpsRaw[0]);
        Assert.AreEqual(1, track.SpsRaw.Count);
        Assert.AreEqual(32, ((VisualSampleEntry)track.CreateSampleEntryBox()).Width);
        Assert.AreEqual(0, logger.Warnings.Count, "changed before the entry was made");

        // the same again: nothing changes
        track.ProcessSample(Hex(H264Sps32), out _, out _);
        Assert.AreEqual(0, logger.Warnings.Count);

        track.ProcessSample(Hex(H264Sps), out _, out _);
        CollectionAssert.AreEqual(Hex(H264Sps), track.SpsRaw[0]);
        Assert.AreEqual(1, logger.Warnings.Count(w => w.Contains("SPS") && w.Contains("after the sample entry")), string.Join("; ", logger.Warnings));
    }

    /// <summary>
    /// The 'hvcC' is of the base layer: a parameter set of another layer stays in the samples - a PPS of layer 1 was put in
    /// the 'hvcC' as though of the base layer, or took the place of the base layer's of its ID.
    /// </summary>
    [TestMethod]
    public void KeepsOnlyTheBaseLayersParameterSetsInTheHvcC()
    {
        var track = new H265Track();
        foreach (string unit in new[] { H265Vps, H265Sps, H265Pps, H265Pictures[0] })
            track.ProcessSample(Hex(unit), out _, out _);

        // the PPS again, of nuh_layer_id 1: of its own ID 1, and of the base layer's ID 0
        byte[] layer1Pps1 = Hex("4409505c684480");
        byte[] layer1Pps0 = Hex("4409" + H265Pps.Substring(4));
        track.ProcessSample(layer1Pps1, out _, out _);
        track.ProcessSample(layer1Pps0, out _, out _);

        Assert.AreEqual(1, track.PpsRaw.Count);
        CollectionAssert.AreEqual(Hex(H265Pps), track.PpsRaw[0]);

        // in the next sample, before its picture
        track.ProcessSample(Hex(H265Pictures[1]), out var first, out _);
        track.ProcessSample(null, out var second, out _);
        Assert.IsNotNull(first.Array);
        var units = track.ParseSample(second.ToArray()).Select(u => u.ToArray()).ToList();
        Assert.AreEqual(3, units.Count);
        CollectionAssert.AreEqual(layer1Pps1, units[0]);
        CollectionAssert.AreEqual(layer1Pps0, units[1]);
    }

    /// <summary>A subset SPS, which no 'mvcC' or 'svcC' is written to hold, stays in the samples: it was dropped.</summary>
    [TestMethod]
    public void KeepsASubsetSpsInTheSamples()
    {
        var track = new H264Track();
        foreach (string unit in new[] { H264Sps, H264Pps, H264Pictures[0] })
            track.ProcessSample(Hex(unit), out _, out _);

        byte[] subsetSps = Hex("6f76000aacb23d80"); // nal_unit_type 15
        track.ProcessSample(subsetSps, out _, out _);
        track.ProcessSample(Hex(H264Pictures[1]), out _, out _);
        track.ProcessSample(null, out var second, out _);

        var units = track.ParseSample(second.ToArray()).Select(u => u.ToArray()).ToList();
        Assert.AreEqual(2, units.Count);
        CollectionAssert.AreEqual(subsetSps, units[0]);
    }

    /// <summary>A track header of a track that has had no SPS is left without a size: it threw.</summary>
    [TestMethod]
    public void FillsATrackHeaderWithoutAnSps()
    {
        foreach (ITrack track in new ITrack[] { new H264Track(), new H265Track(), new H266Track() })
        {
            var tkhd = new TrackHeaderBox();
            track.FillTkhdBox(tkhd);
            Assert.AreEqual(0u, tkhd.Width, track.GetType().Name);
        }
    }

    /// <summary>
    /// A clone writes the sample entry its original would, at once, and reads the slices its original's parameter sets
    /// are of: it had only the timing, and threw making a sample entry until fed the parameter sets again.
    /// </summary>
    [TestMethod]
    public void ClonesWithTheConfiguration()
    {
        var originals = new (H26XTrackBase Track, string[] Units, string Picture)[]
        {
            (new H264Track(30000, 1001), new[] { H264Sps, H264Pps }, H264Pictures[0]),
            (new H265Track(30000, 1001), new[] { H265Vps, H265Sps, H265Pps, "4e010511" + Uuid + "0180" }, H265Pictures[0]),
            (new H266Track(30000, 1001), new[] { H266Sps, H266Pps }, H266Pictures[0]),
        };

        foreach (var (original, units, picture) in originals)
        {
            original.NalLengthSize = 2;
            original.Language = "deu";
            original.TimescaleOverride = 25;
            original.FrameTickOverride = 1;
            foreach (string unit in units)
                original.ProcessSample(Hex(unit), out _, out _);

            var clone = (H26XTrackBase)original.Clone();
            string name = original.GetType().Name;
            Assert.AreEqual(2, clone.NalLengthSize, name);
            Assert.AreEqual("deu", clone.Language, name);
            Assert.AreEqual(25u, clone.TimescaleOverride, name);
            Assert.AreEqual(1, clone.FrameTickOverride, name);
            Assert.AreEqual(original.Timescale, clone.Timescale, name);
            Assert.AreEqual(original.DefaultSampleDuration, clone.DefaultSampleDuration, name);
            CollectionAssert.AreEqual(BytesOf(original.CreateSampleEntryBox()), BytesOf(clone.CreateSampleEntryBox()), name);
            CollectionAssert.AreEqual(original.GetContainerSamples().ToList(), clone.GetContainerSamples().ToList(), new BytesComparer(), name);

            // its slices read, with nothing more given: one picture, one sample
            clone.ProcessSample(Hex(picture), out _, out _);
            clone.ProcessSample(null, out var sample, out bool sync);
            Assert.IsNotNull(sample.Array, name);
            Assert.IsTrue(sync, name);
        }
    }

    /// <summary>The language of a track is ISO 639-2's undetermined, as 14496-12 has an 'mdhd' without one: it was "eng".</summary>
    [TestMethod]
    public void HasNoLanguageOfItsOwn()
    {
        var tracks = new ITrack[]
        {
            new H264Track(), new H265Track(), new H266Track(), new AV1Track(), new AV2Track(), new VP9Track(),
            new AACTrack(2, 48000, 16), new OpusTrack(2, 312, 0, 0),
        };
        foreach (var track in tracks)
            Assert.AreEqual("und", track.Language, track.GetType().Name);
    }

    private sealed class BytesComparer : System.Collections.IComparer
    {
        public int Compare(object? x, object? y) => ((byte[])x!).SequenceEqual((byte[])y!) ? 0 : 1;
    }
}
