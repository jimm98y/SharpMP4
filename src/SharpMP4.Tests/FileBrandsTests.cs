using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Encryption;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// The brands of the 'ftyp' the builders write: of the features the file has, as ISO/IEC 14496-12 Annex E defines them,
/// of the codecs of its tracks where they have a brand (MP4RA), and of QuickTime where a track's entry is QuickTime's.
/// </summary>
[TestClass]
public class FileBrandsTests
{
    // the parameter sets of Chromium's bear-640x360-v_frag.mp4
    private static H264Track Video()
    {
        var video = new H264Track();
        video.ProcessSample(Convert.FromHexString("6764001eacd940a02ff9701100000303e90000ea600f162d96"), out _, out _);
        video.ProcessSample(Convert.FromHexString("68ebe3cb22c0"), out _, out _);
        return video;
    }

    private static (string Major, string[] Compatible) Brands(bool fragmented, Action<IMp4Builder> add)
    {
        var output = new MemoryStream();
        IMp4Builder builder = fragmented
            ? new FragmentedMp4Builder(new SingleStreamOutput(output), 1000) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() }
            : new Mp4Builder(new SingleStreamOutput(output));
        add(builder);
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var ftyp = container.Children.OfType<FileTypeBox>().Single();
        return (IsoStream.ToFourCC(ftyp.MajorBrand), ftyp.CompatibleBrands.Select(IsoStream.ToFourCC).ToArray());
    }

    private static void Add(IMp4Builder builder, ITrack track)
    {
        builder.AddTrack(track);
        builder.ProcessRawSample(track.TrackID, new byte[16], 1000, true);
    }

    [TestMethod]
    public void BrandsAFileOfAvcAsIsomWithTheAvcBrand()
    {
        var (major, compatible) = Brands(false, builder => Add(builder, Video()));

        Assert.AreEqual("isom", major);
        CollectionAssert.AreEqual(new[] { "isom", "mp41", "avc1" }, compatible);
    }

    /// <summary>
    /// Every 'tfhd' of the fragmented builder sets default-base-is-moof, which a file marked 'isom', 'avc1' or 'iso2' to
    /// 'iso4' cannot (Annex E), and it writes 'tfdt' and 'trun' of version 1, of 'iso6': it was marked 'mp42' and 'isom',
    /// and 'avc1' of H.264.
    /// </summary>
    [TestMethod]
    public void BrandsAFragmentedFileIso6AndNotIsomOrAvc1()
    {
        var (major, compatible) = Brands(true, builder => Add(builder, Video()));

        Assert.AreEqual("iso6", major);
        CollectionAssert.AreEqual(new[] { "iso6", "mp41" }, compatible);
    }

    /// <summary>'hvc1', 'vvc1' and 'av02' are the types of sample entries, not brands, of which MP4RA registers none.</summary>
    [TestMethod]
    public void GivesNoBrandOfACodecThatHasNone()
    {
        foreach (var track in new ITrack[] { new H265Track(), new H266Track(), new AV2Track() })
            Assert.IsNull(track.CompatibleBrand, track.GetType().Name);
        Assert.AreEqual("av01", new AV1Track().CompatibleBrand);
    }

    /// <summary>A protected track's 'saiz' and 'saio' are of 'iso6', its 'sinf' of 'iso2', which 'iso6' requires too.</summary>
    [TestMethod]
    public void BrandsAProtectedFileIso6()
    {
        var protection = TrackProtection.Create("cenc", Convert.FromHexString("00112233445566778899aabbccddeeff"), isVideo: false);
        var (_, compatible) = Brands(false, builder =>
        {
            var audio = new AACTrack(2, 44100, 16);
            builder.AddTrack(audio, protection, new byte[16]);
            builder.ProcessRawSample(audio.TrackID, new byte[16], 1024, true);
        });

        CollectionAssert.Contains(compatible, "iso6");
    }

    /// <summary>A track of subtitles of the handler 'subt' - TTML's - has an 'sthd', of 'iso8'.</summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void BrandsAFileOfSubtitlesIso8(bool fragmented)
    {
        var (_, compatible) = Brands(fragmented, builder =>
        {
            var subtitles = new TtmlTrack();
            builder.AddTrack(subtitles);
            builder.ProcessRawSample(subtitles.TrackID, subtitles.CreateSample(Array.Empty<SubtitleCue>()), 1000, true);
        });

        CollectionAssert.Contains(compatible, "iso8");
    }

    /// <summary>A track of an entry only QuickTime has - 'H261' - makes a QuickTime file, marked 'qt  ' alone, as ffmpeg marks it.</summary>
    [TestMethod]
    public void BrandsAFileOfAQuickTimeEntryQuickTime()
    {
        var (major, compatible) = Brands(false, builder => Add(builder, new H261Track(30000, 1001)));

        Assert.AreEqual("qt  ", major);
        CollectionAssert.AreEqual(new[] { "qt  " }, compatible);
    }
}
