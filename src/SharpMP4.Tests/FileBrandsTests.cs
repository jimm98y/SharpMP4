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

    private static (string Major, string[] Compatible) Brands(bool fragmented, Action<IMp4Builder> add) =>
        Brands(Mp4FileFormat.Mp4, fragmented, add).Brands;

    private static ((string Major, string[] Compatible) Brands, uint Minor) Brands(Mp4FileFormat format, bool fragmented, Action<IMp4Builder> add)
    {
        var output = new MemoryStream();
        IMp4Builder builder = Builder(format, fragmented, output);
        add(builder);
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var ftyp = container.Children.OfType<FileTypeBox>().Single();
        return ((IsoStream.ToFourCC(ftyp.MajorBrand), ftyp.CompatibleBrands.Select(IsoStream.ToFourCC).ToArray()), ftyp.MinorVersion);
    }

    private static IMp4Builder Builder(Mp4FileFormat format, bool fragmented, Stream output)
    {
        IMp4Builder builder = fragmented
            ? new FragmentedMp4Builder(new SingleStreamOutput(output), 1000) { TemporaryStorageFactory = new TemporaryMemoryStorageFactory() }
            : new Mp4Builder(new SingleStreamOutput(output));
        builder.FileFormat = format;
        return builder;
    }

    private static AACTrack Audio() => new AACTrack(2, 44100, 16);

    private static H263Track H263() => new H263Track(30000, 1001);

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

    [TestMethod]
    [DataRow(Mp4FileFormat.M4V, false, "M4V ", new[] { "M4V ", "isom", "mp41", "avc1" })]
    [DataRow(Mp4FileFormat.M4V, true, "M4V ", new[] { "M4V ", "iso6", "mp41" })]
    [DataRow(Mp4FileFormat.QuickTime, false, "qt  ", new[] { "qt  " })]
    [DataRow(Mp4FileFormat.ThreeGpp2, false, "3g2a", new[] { "3g2a", "isom", "mp41", "avc1" })]
    public void BrandsAFileOfVideoOfItsFormat(Mp4FileFormat format, bool fragmented, string major, string[] compatible)
    {
        var (brands, _) = Brands(format, fragmented, builder => { Add(builder, Video()); Add(builder, Audio()); });

        Assert.AreEqual(major, brands.Major);
        CollectionAssert.AreEqual(compatible, brands.Compatible);
    }

    [TestMethod]
    [DataRow(Mp4FileFormat.M4A, new[] { "M4A ", "isom", "mp41" })]
    [DataRow(Mp4FileFormat.M4B, new[] { "M4B ", "M4A ", "isom", "mp41" })]
    public void BrandsAFileOfAudioOfItsFormat(Mp4FileFormat format, string[] compatible)
    {
        var (brands, _) = Brands(format, false, builder => Add(builder, Audio()));

        Assert.AreEqual(compatible[0], brands.Major);
        CollectionAssert.AreEqual(compatible, brands.Compatible);
    }

    /// <summary>A file of Apple's audio holds no video: the track is refused as it is added, not written into a file that lies.</summary>
    [TestMethod]
    [DataRow(Mp4FileFormat.M4A)]
    [DataRow(Mp4FileFormat.M4B)]
    public void RefusesVideoInAFileOfAudio(Mp4FileFormat format)
    {
        var builder = Builder(format, false, new MemoryStream());
        builder.AddTrack(Audio());

        Assert.ThrowsExactly<InvalidOperationException>(() => builder.AddTrack(Video()));
    }

    /// <summary>
    /// A 3GP file is marked '3gp6', and the releases before as TS 26.244's guidelines list them - but, fragmented, not
    /// '3gp4' or '3gp5', whose releases do not allow movie fragments (5.5, NOTE 1).
    /// </summary>
    [TestMethod]
    [DataRow(false, new[] { "3gp6", "3gp5", "3gp4", "isom", "mp41" })]
    [DataRow(true, new[] { "3gp6", "iso6", "mp41" })]
    public void BrandsA3gpFileOfItsRelease(bool fragmented, string[] compatible)
    {
        var (brands, _) = Brands(Mp4FileFormat.ThreeGpp, fragmented, builder => { Add(builder, H263()); Add(builder, Audio()); });

        Assert.AreEqual("3gp6", brands.Major);
        CollectionAssert.AreEqual(compatible, brands.Compatible);
    }

    [TestMethod]
    public void GivesA3g2FileTheMinorVersionFfmpegDoes()
    {
        var (_, minor) = Brands(Mp4FileFormat.ThreeGpp2, false, builder => Add(builder, Audio()));

        Assert.AreEqual(0x10000u, minor);
    }

    /// <summary>The Basic profile of 3GP (TS 26.244 5.4.3): one track of video, of audio and of text at most.</summary>
    [TestMethod]
    public void RefusesASecond3gpTrackOfAMediaType()
    {
        var builder = Builder(Mp4FileFormat.ThreeGpp, false, new MemoryStream());
        builder.AddTrack(H263());
        builder.AddTrack(Audio());

        Assert.ThrowsExactly<InvalidOperationException>(() => builder.AddTrack(Audio()));
    }

    /// <summary>A codec 3GP files do not list is written, and warned of: a 3GP player may not play it.</summary>
    [TestMethod]
    public void WarnsOfACodecA3gpFileDoesNotList()
    {
        var logger = new CapturingLogger();
        var builder = Builder(Mp4FileFormat.ThreeGpp, false, new MemoryStream());
        builder.Logger = logger;
        builder.AddTrack(new OpusTrack(2, 312, 0, 0));

        Assert.IsTrue(logger.Warnings.Any(w => w.Contains("OpusTrack")), string.Join("; ", logger.Warnings));
    }

    /// <summary>A track of an entry only QuickTime has cannot be in a file of another format than QuickTime or MP4, which it makes QuickTime.</summary>
    [TestMethod]
    public void RefusesAQuickTimeEntryInAFileOfAnotherFormat()
    {
        var builder = Builder(Mp4FileFormat.M4V, false, new MemoryStream());

        Assert.ThrowsExactly<InvalidOperationException>(() => builder.AddTrack(new H261Track(30000, 1001)));
    }
}
