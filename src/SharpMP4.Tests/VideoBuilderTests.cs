using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// <see cref="VideoBuilder.Create"/>: the builder of the layout asked for, of the options given, an option of the other
/// layout refused; the file's format whichever the layout.
/// </summary>
[TestClass]
public class VideoBuilderTests
{
    private static (Container Container, VideoReader Reader) WriteAudio(IMp4Builder builder, MemoryStream output)
    {
        var audio = new AACTrack(2, 44100, 16);
        builder.AddTrack(audio);
        for (int i = 0; i < 10; i++)
            builder.ProcessRawSample(audio.TrackID, new byte[16], 1024, true);
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var reader = new VideoReader();
        reader.Parse(container);
        return (container, reader);
    }

    [TestMethod]
    public void MakesAProgressiveBuilderOfTheOptions()
    {
        var output = new MemoryStream();
        var builder = VideoBuilder.Create(new SingleStreamOutput(output), Mp4Layout.Progressive, new VideoBuilderOptions
        {
            MovieTimescale = 600,
            NegativeCompositionOffsets = NegativeCompositionOffsets.Version1,
            ChunkDurationInMs = 250,
            TemporaryStorageFactory = new TemporaryMemoryStorageFactory(),
        });

        var mp4 = (Mp4Builder)builder;
        Assert.AreEqual(600u, mp4.MovieTimescale);
        Assert.AreEqual(NegativeCompositionOffsets.Version1, mp4.NegativeCompositionOffsets);
        Assert.AreEqual(250u, mp4.ChunkDurationInMs);
        Assert.IsInstanceOfType<TemporaryMemoryStorageFactory>(mp4.TemporaryStorageFactory);

        var (container, reader) = WriteAudio(builder, output);
        Assert.IsFalse(reader.IsFragmented);
        Assert.AreEqual(600u, container.Children.OfType<MovieBox>().Single().Children.OfType<MovieHeaderBox>().Single().Timescale);
    }

    [TestMethod]
    public void MakesAFragmentedBuilderOfTheOptions()
    {
        var output = new MemoryStream();
        var builder = VideoBuilder.Create(new SingleStreamOutput(output), Mp4Layout.Fragmented, new VideoBuilderOptions
        {
            FragmentLengthInMs = 100,
            AppendMovieFragmentRandomAccessBox = false,
            MaxFragmentDelayInMs = 5000,
            TemporaryStorageFactory = new TemporaryMemoryStorageFactory(),
        });

        var fragmented = (FragmentedMp4Builder)builder;
        Assert.AreEqual(5000ul, fragmented.MaxFragmentDelayInMs);

        var (container, reader) = WriteAudio(builder, output);
        Assert.IsTrue(reader.IsFragmented);
        Assert.IsTrue(container.Children.OfType<MovieFragmentBox>().Count() > 1, "fragments of 100 ms");
        Assert.IsFalse(container.Children.OfType<MovieFragmentRandomAccessBox>().Any(), "no 'mfra', as asked");
    }

    /// <summary>The format is of either layout: QuickTime, fragmented, has QuickTime's 'ftyp' and boxes.</summary>
    [TestMethod]
    [DataRow(Mp4Layout.Progressive)]
    [DataRow(Mp4Layout.Fragmented)]
    public void WritesTheFormatOfEitherLayout(Mp4Layout layout)
    {
        var output = new MemoryStream();
        var builder = VideoBuilder.Create(new SingleStreamOutput(output), layout, new VideoBuilderOptions
        {
            FileFormat = Mp4FileFormat.QuickTime,
            TemporaryStorageFactory = new TemporaryMemoryStorageFactory(),
        });
        Assert.AreEqual(Mp4FileFormat.QuickTime, builder.FileFormat);

        var (container, _) = WriteAudio(builder, output);
        Assert.AreEqual("qt  ", IsoStream.ToFourCC(container.Children.OfType<FileTypeBox>().Single().MajorBrand));
        Assert.IsTrue(Boxes.FindAll<AppleDataReferenceAliasBox>(container).Any());
    }

    [TestMethod]
    public void MakesAnMp4OfTheDefaultsWithoutOptions()
    {
        var builder = VideoBuilder.Create(new SingleStreamOutput(new MemoryStream()), Mp4Layout.Progressive);

        Assert.IsInstanceOfType<Mp4Builder>(builder);
        Assert.AreEqual(Mp4FileFormat.Mp4, builder.FileFormat);
        Assert.AreEqual(1000u, builder.MovieTimescale);
        Assert.AreSame(DefaultMp4Logger.Instance, builder.Logger);
    }

    /// <summary>An option of the other layout is refused, rather than set on nothing.</summary>
    [TestMethod]
    public void RefusesAnOptionOfTheOtherLayout()
    {
        var output = new SingleStreamOutput(new MemoryStream());

        var e = Assert.ThrowsExactly<ArgumentException>(() =>
            VideoBuilder.Create(output, Mp4Layout.Progressive, new VideoBuilderOptions { FragmentLengthInMs = 1000 }));
        StringAssert.Contains(e.Message, nameof(VideoBuilderOptions.FragmentLengthInMs));

        e = Assert.ThrowsExactly<ArgumentException>(() =>
            VideoBuilder.Create(output, Mp4Layout.Fragmented, new VideoBuilderOptions { NegativeCompositionOffsets = NegativeCompositionOffsets.AsApple }));
        StringAssert.Contains(e.Message, nameof(VideoBuilderOptions.NegativeCompositionOffsets));
    }
}
