using SharpISOBMFF;
using SharpMP4.Readers;

namespace SharpMP4.Tests;

/// <summary>
/// The Exif item of a HEIF image (ISO/IEC 23008-12 A.2.1) among the metadata of a file: each tag of its IFDs, of its
/// name, its value as the item has it.
/// </summary>
[TestClass]
public class ExifMetadataTests
{
    private static string Conformance(string path)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        // the repository's, of the files of other projects - not the tests' own Conformance
        while (root != null && !Directory.Exists(Path.Combine(root.FullName, "conformance", "chromium")))
            root = root.Parent;
        string file = root == null ? "" : Path.Combine(root.FullName, "conformance", path);
        if (!File.Exists(file))
            Assert.Inconclusive($"No {path}.");
        return file;
    }

    private static List<MetadataTag> Read(string path)
    {
        using var stream = File.OpenRead(Conformance(path));
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(stream)));
        return container.ReadMetadata();
    }

    [TestMethod]
    public void ReadsTheTagsOfAnExifItem()
    {
        var exif = Read("libavif/paris_icc_exif_xmp.avif").Where(t => t.Family == MetadataFamily.Exif).ToList();

        Console.WriteLine(string.Join(Environment.NewLine, exif.Select(t => $"{t.Namespace}:{t}")));
        // ASCII as it is: of its terminating zero
        Assert.AreEqual("Google\0", exif.Single(t => t.Namespace == "IFD0" && t.Key == "Make").Value);
        Assert.AreEqual("Pixel 4a\0", exif.Single(t => t.Key == "Model").Value);
        StringAssert.StartsWith((string)exif.Single(t => t.Key == "Software").Value, "GIMP 2.10.32");
        Assert.AreEqual("2022:08:24 22:07:42\0", exif.Single(t => t.Key == "DateTime").Value);
        // of the Exif IFD the 0th points to; ExifVersion of its four characters
        Assert.AreEqual("0232", exif.Single(t => t.Namespace == "Exif" && t.Key == "ExifVersion").Value);
        Assert.IsTrue(exif.All(t => t.Box is ItemInfoEntry), "of the item's 'infe'");
    }

    [TestMethod]
    public void ReadsTheExifOfAPhone()
    {
        var exif = Read("exiv2/IMG_3578.heic").Where(t => t.Family == MetadataFamily.Exif).ToList();

        Assert.AreEqual("Apple\0", exif.Single(t => t.Key == "Make").Value);
        // a rational, as numerator/denominator
        StringAssert.Contains((string)exif.Single(t => t.Key == "FNumber").Value, "/");
        Assert.IsTrue(exif.Any(t => t.Namespace == "GPS") || exif.Count > 20, "the IFDs the 0th points to");
    }
}
