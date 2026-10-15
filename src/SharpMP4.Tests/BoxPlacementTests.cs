using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// <see cref="Box.ValidatePlacement"/>: whether a box is in a container its definitions place it in - for information, as
/// nothing enforces it - of a parent of a type, the top of the file, a parent of a class, and of a protected sample entry
/// the format its 'frma' says.
/// </summary>
[TestClass]
public class BoxPlacementTests
{
    private static T In<T>(IMp4Serializable parent, T box) where T : Box
    {
        box.SetParent(parent);
        if (parent is Box container)
            (container.Children ??= new List<Box>()).Add(box);
        return box;
    }

    [TestMethod]
    public void SaysWhetherABoxIsInItsContainer()
    {
        var file = new Container();
        var moov = In(file, new MovieBox());
        Assert.IsTrue(moov.ValidatePlacement(), "'moov' at the top of the file");
        Assert.IsTrue(In(moov, new TrackBox()).ValidatePlacement(), "'trak' in 'moov'");

        Assert.IsFalse(In(file, new TrackBox()).ValidatePlacement(), "'trak' at the top of the file");
        Assert.IsFalse(In(In(moov, new TrackBox()), new MovieBox()).ValidatePlacement(), "'moov' in 'trak'");

        // a box of no parent is at the top, as a box not yet put in a file is
        Assert.IsTrue(new MovieBox().ValidatePlacement());
    }

    [TestMethod]
    public void TakesAnywhereAndUnsaidAsPlaced()
    {
        var file = new Container();
        var trak = In(In(file, new MovieBox()), new TrackBox());
        Assert.IsTrue(In(trak, new FreeSpaceBox()).ValidatePlacement(), "'free' is anywhere");
        Assert.IsTrue(In(file, new FreeSpaceBox()).ValidatePlacement());
    }

    /// <summary>
    /// A configuration box is of the sample entries of its format, of their types: an 'avcC' in an 'avc1', not in an 'hvc1';
    /// and in an 'encv' of an 'avc1' - its 'frma' says so - but not in one of an 'hvc1'.
    /// </summary>
    [TestMethod]
    public void TakesAProtectedSampleEntryAsTheFormatItWas()
    {
        var stsd = new SampleDescriptionBox();
        var avc1 = In(stsd, new VisualSampleEntry(IsoStream.FromFourCC("avc1")));
        Assert.IsTrue(In(avc1, new AVCConfigurationBox()).ValidatePlacement());
        Assert.IsTrue(avc1.ValidatePlacement(), "a sample entry in 'stsd'");

        var hvc1 = In(stsd, new VisualSampleEntry(IsoStream.FromFourCC("hvc1")));
        Assert.IsFalse(In(hvc1, new AVCConfigurationBox()).ValidatePlacement(), "'avcC' in 'hvc1'");

        foreach (var (format, placed) in new[] { ("avc1", true), ("hvc1", false) })
        {
            var encv = In(stsd, new VisualSampleEntry(IsoStream.FromFourCC("encv")));
            var sinf = In(encv, new ProtectionSchemeInfoBox());
            In(sinf, new OriginalFormatBox { DataFormat = IsoStream.FromFourCC(format) });
            Assert.AreEqual(placed, In(encv, new AVCConfigurationBox()).ValidatePlacement(), $"'avcC' in an 'encv' of '{format}'");
        }
    }

    /// <summary>A box that is not where its definitions say is read all the same: the placement is not enforced.</summary>
    [TestMethod]
    public void ReadsAMisplacedBoxAllTheSame()
    {
        // a 'trak' at the top of a file, before its 'moov'
        byte[] file = Convert.FromHexString("00000008" + "7472616b" + "00000008" + "6d6f6f76");
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        Assert.AreEqual(2, container.Children.Count);
        Assert.IsFalse(container.Children[0].ValidatePlacement());
        Assert.IsTrue(container.Children[1].ValidatePlacement());
    }
}
