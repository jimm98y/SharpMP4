using SharpISOBMFF;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// The samples of the video tracks of the conformance corpus's files, their NAL units or OBUs given one at a time to a
/// track - as an encoder's output is - come out as the same samples, with the same of them sync samples, as the files'
/// muxers made them: each track finds where an access unit or temporal unit ends and whether it is a random access
/// point as ISO/IEC 14496-15 and the AV1 binding have it.
/// </summary>
[TestClass]
public class SyncSampleTests
{
    public TestContext TestContext { get; set; } = null!;

    private const string RecoveryPoint = "marks a picture of a recovery point SEI a sync sample: ffmpeg's and Shaka's key frames, where 14496-15 has only IDR pictures";
    private const string GpacCra = "leaves CRA pictures out of 'stss', as GPAC does for a SAP of type 3: 14496-15 has them sync samples";
    private const string MadeWrong = "made with the wrong sync samples on purpose";
    private const string NoPicture = "has a sample with no picture: an SEI NAL unit, or parameter sets, alone";

    /// <summary>Files whose samples are not as a stream's access units are, or whose sync samples are not as 14496-15 has them.</summary>
    private static readonly Dictionary<string, string> KnownDeviations = new()
    {
        ["isobmff/heif/C031.heic"] = "marks a trailing picture a sync sample",
        ["isobmff/nalu/hevc/alst_hvc1.mp4"] = "leaves an IDR picture out of 'stss'",
        ["isobmff/nalu/hevc/subs_slice_hvc1.mp4"] = GpacCra,
        ["isobmff/nalu/hevc/subs_tile_hvc1.mp4"] = GpacCra,
        ["isobmff/nalu/hevc/trgr_hvc1.mp4"] = GpacCra,
        ["chromium/bear-320x240-v-2frames-keyframe-is-non-sync-sample_frag-hevc.mp4"] = MadeWrong,
        ["chromium/bear-320x240-v-2frames-nonkeyframe-is-sync-sample_frag-hevc.mp4"] = MadeWrong,
        ["chromium/bear-640x360-v-2frames-keyframe-is-non-sync-sample_frag.mp4"] = MadeWrong,
        ["chromium/bear-640x360-v-2frames-nonkeyframe-is-sync-sample_frag.mp4"] = MadeWrong,
        ["chromium/bear-320x240-v-2fragments-open-gop_frag.mp4"] = RecoveryPoint,
        ["shaka/6339/v.mp4"] = RecoveryPoint,
        ["fate/mov/buck480p30_na.mp4"] = RecoveryPoint,
        ["fate/mov/test_iibbibb.mp4"] = RecoveryPoint,
        ["fate/mov/test_iibbibb_neg_ctts.mp4"] = RecoveryPoint,
        ["firefox/crashtests/1414444.mp4"] = NoPicture,
        ["firefox/crashtests/1833896.mp4"] = NoPicture,
        ["firefox/crashtests/1389304.mp4"] = "is a crash test: it marks pictures of non-IDR slices sync samples",
        ["firefox/crashtests/1903669.mp4"] = "is a crash test: a sample of slices whose headers differ as those of new pictures do (7.4.1.2.4)",
        ["mp4parse/mp4parse_capi/no_timescale.mp4"] = NoPicture,
        ["fate/h264/attachment631-small.mp4"] = NoPicture,
        ["firefox/pixel_aspect_ratio.mp4"] = "has an IDR slice with nal_ref_idc 0, which H.264 7.4.1 does not allow: a new picture, by 7.4.1.2.4",
        ["fate/h264/mixed-nal-coding.mp4"] = "has both fields of a frame in one sample, where each field is an access unit",
    };

    [TestMethod]
    public void TracksFindTheSamplesAndSyncSamplesOfFiles()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");

        var files = ConformanceCorpus.FileFormatFiles(root).Select(x => x.File)
            .Concat(ConformanceCorpus.ChromiumFiles(root)).Concat(ConformanceCorpus.FirefoxFiles(root))
            .Concat(ConformanceCorpus.ShakaFiles(root)).Concat(ConformanceCorpus.Mp4parseFiles(root)).Concat(ConformanceCorpus.FateFiles(root))
            .Where(f => Path.GetExtension(f).ToLowerInvariant() is ".mp4" or ".mov" or ".m4v" or ".3gp" or ".mj2" or ".heic" or ".cmfv")
            .Distinct().ToList();

        int tracks = 0, samples = 0;
        var failures = new List<string>();
        var known = new List<string>();
        foreach (string file in files)
        {
            foreach (var (codec, result) in Check(file))
            {
                tracks++;
                samples += result.Samples;
                string name = Path.GetRelativePath(root, file).Replace('\\', '/');
                if (result.Failure == null)
                    continue;
                if (KnownDeviations.TryGetValue(name, out string? why))
                    known.Add($"{codec} {name}: {result.Failure} - the file {why}");
                else
                    failures.Add($"{codec} {name}: {result.Failure}");
            }
        }

        TestContext.WriteLine($"{tracks} video tracks, {samples} samples, {failures.Count} not as the file has them, {known.Count} where the file is not as a stream is");
        foreach (string failure in failures.Concat(known))
            TestContext.WriteLine(failure);
        Assert.IsTrue(tracks > 0, "no video track of H.264, H.265, H.266 or AV1");
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures.Take(50)));
    }

    private sealed record Result(int Samples, string? Failure);

    private static IEnumerable<(string Codec, Result Result)> Check(string file)
    {
        VideoReader reader;
        FileStream stream;
        try
        {
            stream = File.OpenRead(file);
            var container = new Container();
            container.Read(new IsoStream(new StreamWrapper(stream)));
            reader = new VideoReader();
            reader.Parse(container);
        }
        catch
        {
            // what cannot be read is the business of the tests that read the files
            yield break;
        }

        using (stream)
        {
            foreach (uint trackID in reader.Tracks.Keys.ToList())
            {
                var info = reader.Tracks[trackID];
                if (info.Protection != null || info.Track is not (H264Track or H265Track or H266Track or AV1Track))
                    continue;

                Result result;
                try
                {
                    result = Check(reader, trackID, info.Track, info.Stbl);
                }
                catch (Exception ex)
                {
                    result = new Result(0, $"threw {ex.GetType().Name}: {ex.Message} {ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim()}");
                }
                yield return (info.Track.GetType().Name.Replace("Track", ""), result);
            }
        }
    }

    private static Result Check(VideoReader reader, uint trackID, ITrack track, Box stbl)
    {
        // the size of the lengths before the file's NAL units
        var entry = stbl.Children.OfType<SampleDescriptionBox>().Single().Children.First();
        int lengthSize = entry.Children.Select(b => b switch
        {
            AVCConfigurationBox avcC => avcC._AVCConfig.LengthSizeMinusOne + 1,
            HEVCConfigurationBox hvcC => hvcC._HEVCConfig.LengthSizeMinusOne + 1,
            VvcConfigurationBox vvcC => vvcC._VvcConfig._LengthSizeMinusOne + 1,
            _ => 0,
        }).FirstOrDefault(x => x > 0);
        bool av1 = track is AV1Track;
        if (!av1 && lengthSize == 0)
            return new Result(0, null);

        // the file's samples, read first: a track the reader cannot read yet is the reader's tests' business
        var fileSamples = new List<(byte[] Data, bool Sync)>();
        try
        {
            for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
                fileSamples.Add((sample.Data.ToArray(), sample.IsRandomAccessPoint));
        }
        catch (Exception)
        {
            return new Result(0, null);
        }

        // a track of a layer whose samples are empty or made of other tracks' - implicit reconstruction, extractors - or that
        // aggregates NAL units is the file's to put together, not a stream's
        if (fileSamples.Any(x => x.Data.Length == 0) ||
            (track is H265Track && fileSamples.Any(x => NalUnits(x.Data, lengthSize).Any(u => ((u[0] >> 1) & 0x3f) is 48 or 49))))
            return new Result(0, null);

        // the track writes the lengths the file has
        int outputLengthSize = track is H26XTrackBase h26x ? h26x.NalLengthSize : 0;
        var expected = new List<(List<byte[]> Units, bool Sync)>();
        var actual = new List<(List<byte[]> Units, bool Sync)>();
        foreach (var (data, sync) in fileSamples)
        {
            var units = av1 ? Obus(data) : NalUnits(data, lengthSize);
            expected.Add((Comparable(track, units), sync));
            foreach (byte[] unit in units)
            {
                track.ProcessSample(unit, out var output, out bool isSync);
                if (output.Array != null)
                    actual.Add((Comparable(track, av1 ? Obus(output.ToArray()) : NalUnits(output.ToArray(), outputLengthSize)), isSync));
            }
        }
        if (!av1)
        {
            track.ProcessSample(null, out var last, out bool lastSync);
            if (last.Array != null)
                actual.Add((Comparable(track, NalUnits(last.ToArray(), outputLengthSize)), lastSync));
        }

        if (expected.Count == 0)
            return new Result(0, null);

        for (int i = 0; i < Math.Min(expected.Count, actual.Count); i++)
        {
            if (!SameUnits(expected[i].Units, actual[i].Units))
                return new Result(expected.Count, $"sample {i} of {expected.Count}: {Describe(actual[i].Units, av1)} where the file has {Describe(expected[i].Units, av1)}");
            if (expected[i].Sync != actual[i].Sync)
                return new Result(expected.Count, $"sample {i} of {expected.Count} ({Describe(expected[i].Units, av1)}): {(actual[i].Sync ? "a" : "not a")} sync sample where the file has it {(expected[i].Sync ? "one" : "not")}");
        }
        if (expected.Count != actual.Count)
            return new Result(expected.Count, $"{actual.Count} samples where the file has {expected.Count}");
        return new Result(expected.Count, null);
    }

    /// <summary>
    /// The units of a sample but for its parameter sets and access unit delimiters, which a track keeps in its sample
    /// entry and drops: an 'avc1', 'hvc1' or 'vvc1' sample need not have them. An AV1 sample's all.
    /// </summary>
    private static List<byte[]> Comparable(ITrack track, List<byte[]> units) => units.Where(u => track switch
    {
        H264Track => (u[0] & 0x1f) is not (7 or 8 or 9 or 13 or 15),
        H265Track => ((u[0] >> 1) & 0x3f) is not (32 or 33 or 34 or 35),
        H266Track => u.Length < 2 || ((u[1] >> 3) & 0x1f) is not (12 or 13 or 14 or 15 or 16 or 20),
        _ => true,
    }).ToList();

    private static List<byte[]> NalUnits(byte[] data, int lengthSize)
    {
        var units = new List<byte[]>();
        for (int p = 0; p + lengthSize <= data.Length;)
        {
            int length = 0;
            for (int k = 0; k < lengthSize; k++)
                length = length << 8 | data[p + k];
            p += lengthSize;
            if (length <= 0 || p + length > data.Length)
                break;
            units.Add(data.AsSpan(p, length).ToArray());
            p += length;
        }
        return units;
    }

    /// <summary>The OBUs of a sample, each with its header and size; one without a size field is the rest.</summary>
    private static List<byte[]> Obus(byte[] data)
    {
        var obus = new List<byte[]>();
        for (int p = 0; p < data.Length;)
        {
            int header = 1 + ((data[p] & 0x04) != 0 ? 1 : 0);
            int size = data.Length - p;
            if ((data[p] & 0x02) != 0)
            {
                long value = 0;
                int i = 0;
                for (; i < 8 && p + header + i < data.Length; i++)
                {
                    value |= (long)(data[p + header + i] & 0x7f) << (7 * i);
                    if ((data[p + header + i] & 0x80) == 0)
                        break;
                }
                size = header + i + 1 + (int)value;
            }
            if (size <= 0 || p + size > data.Length)
                break;
            obus.Add(data.AsSpan(p, size).ToArray());
            p += size;
        }
        return obus;
    }

    private static bool SameUnits(List<byte[]> a, List<byte[]> b) =>
        a.Count == b.Count && a.Zip(b).All(x => x.First.AsSpan().SequenceEqual(x.Second));

    private static string Describe(List<byte[]> units, bool av1) => "[" + string.Join(" ", units.Select(u => av1
        ? $"obu{(u[0] >> 3) & 0xF}"
        : u.Length >= 2 ? $"{u[0]:x2}{u[1]:x2}" : "?")) + "]";
}
