using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// <see cref="ImageBuilder.Create"/> and the HEIF images it writes: HEIC and AVIF, of one coded picture or of the tiles of
/// a grid, read back by <see cref="ImageReader"/> as they were written.
/// </summary>
[TestClass]
public class ImageBuilderTests
{
    // 16x16 grey HEVC of H26XTrackTests: its parameter sets, and an IDR picture
    private const string H265Vps = "40010c01ffff01600000030090000003000003001e928090";
    private const string H265Sps = "42010101600000030090000003000003001ea08845964aabcaf0168080000003008000000c84";
    private const string H265Pps = "4401c171a112";
    private const string H265Idr = "2801af0b60f59b50";

    // AV1's sequence header of AV1TrackTests: profile 0, 16x16
    private const string Av1SequenceHeader = "0a0c200000f8cffc42142040404a";

    private static (Container Container, ImageReader Reader) Write(ImageFormat format, ImageBuilderOptions? options, Action<IImageBuilder> feed)
    {
        var output = new MemoryStream();
        using (var builder = ImageBuilder.Create(new SingleStreamOutput(output), format, options))
        {
            feed(builder);
            builder.FinalizeImage();
        }

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        Assert.IsFalse(container.IsTruncated);
        var reader = new ImageReader();
        reader.Parse(container);
        return (container, reader);
    }

    private static MetaBox Meta(Container container) => container.Children.OfType<MetaBox>().Single();

    private static string[] Brands(Container container)
    {
        var ftyp = container.Children.OfType<FileTypeBox>().Single();
        return new[] { ftyp.MajorBrand }.Concat(ftyp.CompatibleBrands).Select(IsoStream.ToFourCC).ToArray();
    }

    /// <summary>
    /// An HEVC picture given a NAL unit at a time, its parameter sets first: a HEIC file of one 'hvc1' item, the primary,
    /// of the 'hvcC' the parameter sets make - essential - its size and the bits of its channels; read back as it was.
    /// </summary>
    [TestMethod]
    public void WritesAHeicImageOfItsNalUnits()
    {
        var (container, reader) = Write(ImageFormat.Heic, null, builder =>
        {
            foreach (string unit in new[] { H265Vps, H265Sps, H265Pps, H265Idr })
                builder.ProcessImageSample(Hex(unit));
        });

        CollectionAssert.AreEqual(new[] { "heic", "mif1", "heic" }, Brands(container));
        var meta = Meta(container);
        Assert.AreEqual("pict", IsoStream.ToFourCC(meta.TheHandler.HandlerType));
        Assert.AreEqual(1u, meta.PrimaryResource.ItemID);
        var infe = meta.ItemInfos.ItemInfos.Single();
        Assert.AreEqual("hvc1", IsoStream.ToFourCC(infe.ItemType));
        Assert.AreEqual(0u, infe.Flags, "shown");

        var ipco = meta.Children.OfType<ItemPropertiesBox>().Single().PropertyContainer;
        var hvcC = ipco.Children.OfType<HEVCConfigurationBox>().Single();
        Assert.AreEqual(3, hvcC._HEVCConfig.NumOfArrays, "VPS, SPS and PPS");
        var pixi = ipco.Children.OfType<PixelInformationProperty>().Single();
        CollectionAssert.AreEqual(new byte[] { 8, 8, 8 }, pixi.BitsPerChannel);
        var ipma = meta.Children.OfType<ItemPropertiesBox>().Single().Association.Single();
        Assert.IsTrue(ipma.Essential[0][0], "the configuration a decoder needs");

        Assert.IsInstanceOfType<H265Track>(reader.Track);
        Assert.AreEqual(16u, reader.Ispe.ImageWidth);
        Assert.AreEqual(16u, reader.Ispe.ImageHeight);
        CollectionAssert.AreEqual(Hex("00000008" + H265Idr), reader.ReadSample()!.Data, "the picture, behind its length");
        Assert.IsNull(reader.ReadSample());
    }

    /// <summary>The same, of the Annex B byte stream an encoder hands out: the start codes stripped.</summary>
    [TestMethod]
    public void WritesAHeicImageOfAnnexB()
    {
        byte[] stream = new[] { H265Vps, H265Sps, H265Pps, H265Idr }.SelectMany(unit => Hex("00000001" + unit)).ToArray();
        var (_, reader) = Write(ImageFormat.Heic, null, builder => builder.ProcessAnnexBImageSample(stream));

        CollectionAssert.AreEqual(Hex("00000008" + H265Idr), reader.ReadSample()!.Data);
    }

    /// <summary>An AV1 picture stored as it is: an AVIF file of one 'av01' item of the 'av1C' of its sequence header.</summary>
    [TestMethod]
    public void WritesAnAvifImage()
    {
        byte[] picture = Hex("32050102030405");
        var (container, reader) = Write(ImageFormat.Avif, null, builder =>
        {
            builder.ProcessImageSample(Hex(Av1SequenceHeader));
            builder.ProcessRawImage(picture);
        });

        CollectionAssert.AreEqual(new[] { "avif", "avif", "mif1", "miaf" }, Brands(container));
        var meta = Meta(container);
        Assert.AreEqual("av01", IsoStream.ToFourCC(meta.ItemInfos.ItemInfos.Single().ItemType));
        var av1C = meta.Children.OfType<ItemPropertiesBox>().Single().PropertyContainer.Children.OfType<AV1CodecConfigurationBox>().Single();
        CollectionAssert.AreEqual(Hex(Av1SequenceHeader), av1C.Av1Config.ConfigOBUs);

        Assert.IsInstanceOfType<AV1Track>(reader.Track);
        Assert.AreEqual(16u, reader.Ispe.ImageWidth);
        CollectionAssert.AreEqual(picture, reader.ReadSample()!.Data);
    }

    /// <summary>
    /// Tiles of a grid: an item each, hidden, of the shared configuration and size; the primary item a 'grid' derived from
    /// them row by row ('dimg'), of the output size, its description in the 'idat'. The reader takes the tiles'
    /// configuration, and reads each tile, then the description.
    /// </summary>
    [TestMethod]
    public void WritesAGridOfTiles()
    {
        var tiles = Enumerable.Range(0, 6).Select(i => Hex("3203" + i.ToString("x2") + "aabb")).ToArray();
        var options = new ImageBuilderOptions { GridColumns = 3, GridRows = 2, OutputWidth = 40, OutputHeight = 30 };
        var (container, reader) = Write(ImageFormat.Avif, options, builder =>
        {
            builder.ProcessImageSample(Hex(Av1SequenceHeader));
            foreach (var tile in tiles)
                builder.ProcessRawImage(tile);
        });

        var meta = Meta(container);
        Assert.AreEqual(7u, meta.PrimaryResource.ItemID, "the grid, after its tiles");
        var items = meta.ItemInfos.ItemInfos.ToArray();
        Assert.AreEqual(7, items.Length);
        Assert.IsTrue(items.Take(6).All(x => IsoStream.ToFourCC(x.ItemType) == "av01" && x.Flags == 1), "tiles, hidden");
        Assert.AreEqual("grid", IsoStream.ToFourCC(items[6].ItemType));

        var dimg = meta.ItemRefs.References.Single();
        Assert.AreEqual("dimg", IsoStream.ToFourCC(dimg.FourCC));
        Assert.AreEqual(7, dimg.FromItemID);
        CollectionAssert.AreEqual(new ushort[] { 1, 2, 3, 4, 5, 6 }, dimg.ToItemID);

        var iprp = meta.Children.OfType<ItemPropertiesBox>().Single();
        var extents = iprp.PropertyContainer.Children.OfType<ImageSpatialExtentsProperty>().ToArray();
        var ipma = iprp.Association.Single();
        int gridEntry = Array.IndexOf(ipma.ItemID, 7u);
        var gridExtent = ipma.PropertyIndex[gridEntry].Select(i => iprp.PropertyContainer.Children[i - 1]).OfType<ImageSpatialExtentsProperty>().Single();
        Assert.AreEqual((40u, 30u), (gridExtent.ImageWidth, gridExtent.ImageHeight));

        Assert.IsInstanceOfType<AV1Track>(reader.Track, "of the tiles' configuration");
        Assert.AreEqual(16u, reader.Ispe.ImageWidth, "of a tile");
        foreach (var tile in tiles)
            CollectionAssert.AreEqual(tile, reader.ReadSample()!.Data);
        // version 0, flags 0 (16 bit sizes), rows and columns less one, then the output size
        CollectionAssert.AreEqual(Hex("00000102" + "0028" + "001e"), reader.ReadSample()!.Data, "the grid's description, of the 'idat'");
        Assert.IsNull(reader.ReadSample());
    }

    /// <summary>A grid's output is all of its tiles unless set; larger than they are, it is refused.</summary>
    [TestMethod]
    public void MakesAGridOfAllOfItsTilesUnlessSizedAndRefusesOneLargerThanThem()
    {
        var (_, reader) = Write(ImageFormat.Avif, new ImageBuilderOptions { GridColumns = 2, GridRows = 1 }, builder =>
        {
            builder.ProcessImageSample(Hex(Av1SequenceHeader));
            builder.ProcessRawImage(Hex("3200"));
            builder.ProcessRawImage(Hex("3200"));
        });
        reader.ReadSample();
        reader.ReadSample();
        CollectionAssert.AreEqual(Hex("00000001" + "0020" + "0010"), reader.ReadSample()!.Data, "32x16, of two tiles of 16x16");

        using var builder = ImageBuilder.Create(new SingleStreamOutput(new MemoryStream()), ImageFormat.Avif,
            new ImageBuilderOptions { GridColumns = 2, GridRows = 1, OutputWidth = 33, OutputHeight = 16 });
        builder.ProcessImageSample(Hex(Av1SequenceHeader));
        builder.ProcessRawImage(Hex("3200"));
        builder.ProcessRawImage(Hex("3200"));
        Assert.ThrowsExactly<InvalidOperationException>(() => builder.FinalizeImage());
    }

    [TestMethod]
    public void RefusesAPictureMoreThanTheImageIsOf()
    {
        using var single = ImageBuilder.Create(new SingleStreamOutput(new MemoryStream()), ImageFormat.Avif);
        single.ProcessRawImage(Hex("3200"));
        Assert.ThrowsExactly<InvalidOperationException>(() => single.ProcessRawImage(Hex("3200")));

        using var grid = ImageBuilder.Create(new SingleStreamOutput(new MemoryStream()), ImageFormat.Avif, new ImageBuilderOptions { GridColumns = 2, GridRows = 2 });
        grid.ProcessImageSample(Hex(Av1SequenceHeader));
        grid.ProcessRawImage(Hex("3200"));
        var e = Assert.ThrowsExactly<InvalidOperationException>(() => grid.FinalizeImage());
        StringAssert.Contains(e.Message, "4 tiles");
    }

    [TestMethod]
    public void RefusesOptionsThatMakeNoGrid()
    {
        var output = new SingleStreamOutput(new MemoryStream());
        Assert.ThrowsExactly<ArgumentException>(() => ImageBuilder.Create(output, ImageFormat.Heic, new ImageBuilderOptions { GridColumns = 2 }));
        Assert.ThrowsExactly<ArgumentException>(() => ImageBuilder.Create(output, ImageFormat.Heic, new ImageBuilderOptions { OutputWidth = 100 }));
        Assert.ThrowsExactly<ArgumentException>(() => ImageBuilder.Create(output, ImageFormat.Heic, new ImageBuilderOptions { GridColumns = 300, GridRows = 1 }));
    }

    [TestMethod]
    public void TakesAnnexBOfHevcAlone()
    {
        using var builder = ImageBuilder.Create(new SingleStreamOutput(new MemoryStream()), ImageFormat.Avif);
        Assert.ThrowsExactly<NotSupportedException>(() => builder.ProcessAnnexBImageSample(Hex("000001" + Av1SequenceHeader)));
    }

    /// <summary>
    /// The colour of the options is the image's 'colr', of the picture and of the grid alike; of none, and a stream that
    /// says nothing of it - this sequence header's color description is all unspecified, of limited range - there is none.
    /// </summary>
    [TestMethod]
    public void WritesTheColourOfTheOptionsOrOfTheStream()
    {
        var colour = new ColourInformationBox
        {
            ColourType = IsoStream.FromFourCC("nclx"),
            ColourPrimaries = 12,
            TransferCharacteristics = 13,
            MatrixCoefficients = 6,
            FullRangeFlag = true,
        };
        var (container, _) = Write(ImageFormat.Avif, new ImageBuilderOptions { GridColumns = 2, GridRows = 1, Colour = colour }, builder =>
        {
            builder.ProcessImageSample(Hex(Av1SequenceHeader));
            builder.ProcessRawImage(Hex("3200"));
            builder.ProcessRawImage(Hex("3200"));
        });

        var iprp = Meta(container).Children.OfType<ItemPropertiesBox>().Single();
        var colr = iprp.PropertyContainer.Children.OfType<ColourInformationBox>().Single();
        Assert.AreEqual("nclx", IsoStream.ToFourCC(colr.ColourType));
        Assert.AreEqual(((ushort)12, (ushort)13, (ushort)6, true), (colr.ColourPrimaries, colr.TransferCharacteristics, colr.MatrixCoefficients, colr.FullRangeFlag));
        ushort index = (ushort)(iprp.PropertyContainer.Children.IndexOf(colr) + 1);
        var ipma = iprp.Association.Single();
        Assert.IsTrue(ipma.PropertyIndex.All(properties => properties.Contains(index)), "of each tile and of the grid");

        (container, _) = Write(ImageFormat.Avif, null, builder =>
        {
            builder.ProcessImageSample(Hex(Av1SequenceHeader));
            builder.ProcessRawImage(Hex("3200"));
        });
        Assert.IsFalse(Boxes.FindAll<ColourInformationBox>(container).Any());
    }

    /// <summary>
    /// The shape of the pixels and the transformations are of the image - of the picture, or of the grid, not its tiles -
    /// after its descriptive properties, the transformations essential and in the order they are applied: crop, rotation,
    /// mirroring (23008-12 6.5.1, 6.5.10).
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void WritesTheTransformationsOfTheImageInTheirOrder(bool grid)
    {
        var options = new ImageBuilderOptions
        {
            // set in the reverse of their order: written in theirs, whatever the order they were set in
            Mirror = new ImageMirror { Axis = true },
            Rotation = new ImageRotation { Angle = 1 },
            CleanAperture = new CleanApertureBox
            {
                CleanApertureWidthN = 12, CleanApertureWidthD = 1, CleanApertureHeightN = 10, CleanApertureHeightD = 1,
                HorizOffN = 0, HorizOffD = 1, VertOffN = 0, VertOffD = 1,
            },
            PixelAspectRatio = new PixelAspectRatioBox { HSpacing = 4, VSpacing = 3 },
        };
        if (grid)
        {
            options.GridColumns = 2;
            options.GridRows = 1;
        }

        var (container, _) = Write(ImageFormat.Avif, options, builder =>
        {
            builder.ProcessImageSample(Hex(Av1SequenceHeader));
            for (int i = 0; i < (grid ? 2 : 1); i++)
                builder.ProcessRawImage(Hex("3200"));
        });

        var meta = Meta(container);
        var iprp = meta.Children.OfType<ItemPropertiesBox>().Single();
        var ipma = iprp.Association.Single();
        uint image = meta.PrimaryResource.ItemID;
        int entry = Array.IndexOf(ipma.ItemID, image);
        var ofImage = ipma.PropertyIndex[entry].Select(i => IsoStream.ToFourCC(iprp.PropertyContainer.Children[i - 1].FourCC)).ToArray();
        CollectionAssert.AreEqual(new[] { "pasp", "clap", "irot", "imir" }, ofImage[^4..], "the descriptive before the transformative, those as they are applied");
        CollectionAssert.AreEqual(new[] { false, true, true, true }, ipma.Essential[entry][^4..]);

        if (grid)
        {
            for (int tile = 0; tile < ipma.ItemID.Length; tile++)
            {
                if (ipma.ItemID[tile] == image)
                    continue;
                var ofTile = ipma.PropertyIndex[tile].Select(i => IsoStream.ToFourCC(iprp.PropertyContainer.Children[i - 1].FourCC));
                CollectionAssert.DoesNotContain(ofTile.ToArray(), "irot", "of the grid, not its tiles");
            }
        }

        Assert.AreEqual((byte)1, iprp.PropertyContainer.Children.OfType<ImageRotation>().Single().Angle);
        Assert.IsTrue(iprp.PropertyContainer.Children.OfType<ImageMirror>().Single().Axis);
        Assert.AreEqual(12u, iprp.PropertyContainer.Children.OfType<CleanApertureBox>().Single().CleanApertureWidthN);
    }

    [TestMethod]
    public void RefusesAnImageOfNoConfiguration()
    {
        using var builder = ImageBuilder.Create(new SingleStreamOutput(new MemoryStream()), ImageFormat.Heic);
        builder.ProcessRawImage(Hex("0000000128"));
        Assert.ThrowsExactly<InvalidOperationException>(() => builder.FinalizeImage());
    }
}
