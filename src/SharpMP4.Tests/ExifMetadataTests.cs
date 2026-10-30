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
        // ASCII as its bytes, as the strings of boxes are: of its terminating zero
        var make = (BinaryUTF8String)exif.Single(t => t.Namespace == "IFD0" && t.Key == "Make").Value!;
        CollectionAssert.AreEqual("Google\0"u8.ToArray(), make.Bytes);
        Assert.AreEqual("Google", make.Text);
        Assert.AreEqual("Pixel 4a\0", Text(exif, "Model"));
        StringAssert.StartsWith(Text(exif, "Software"), "GIMP 2.10.32");
        Assert.AreEqual("2022:08:24 22:07:42\0", Text(exif, "DateTime"));
        // of the Exif IFD the 0th points to; ExifVersion of its four characters
        Assert.AreEqual("0232", exif.Single(t => t.Namespace == "Exif" && t.Key == "ExifVersion").Value!.ToString());
        Assert.IsTrue(exif.All(t => t.Box is ItemInfoEntry), "of the item's 'infe'");
    }

    private static string Text(List<MetadataTag> tags, string key) => tags.Single(t => t.Key == key).Value!.ToString()!;

    /// <summary>The bytes a value is of: of text, as it was read; of bytes, them.</summary>
    private static byte[]? BytesOf(object? value) => value switch
    {
        BinaryUTF8String s => s.Bytes,
        BinaryUTF16String s => s.Bytes,
        CharacterCodedString s => s.Bytes,
        byte[] bytes => bytes,
        _ => null,
    };

    [TestMethod]
    public void ReadsTheExifOfAPhone()
    {
        var exif = Read("exiv2/IMG_3578.heic").Where(t => t.Family == MetadataFamily.Exif).ToList();

        Assert.AreEqual("Apple\0", Text(exif, "Make"));
        // a rational, as numerator/denominator
        StringAssert.Contains((string)exif.Single(t => t.Key == "FNumber").Value, "/");
        Assert.IsTrue(exif.Any(t => t.Namespace == "GPS") || exif.Count > 20, "the IFDs the 0th points to");
    }

    /// <summary>Where each value is in the file: its bytes there are its value's.</summary>
    [TestMethod]
    [DataRow("libavif/paris_icc_exif_xmp.avif")]
    [DataRow("exiv2/IMG_3578.heic")]
    [DataRow("exiv2/Reagan.jxl")]
    public void SaysWhereEachValueIs(string path)
    {
        byte[] file = File.ReadAllBytes(Conformance(path));
        var exif = Read(path).Where(t => t.Family == MetadataFamily.Exif).ToList();

        Console.WriteLine(string.Join(Environment.NewLine, exif.Select(t => $"{t.Namespace}:{t} @{t.Offset} [{t.Length}] {t.Description}")));
        Assert.IsTrue(exif.Count > 0, path);
        // of text, whatever its encoding, and of bytes: the bytes of the value are the file's where it says
        var texts = exif.Where(t => t.Offset >= 0 && BytesOf(t.Value) != null).ToList();
        Assert.IsTrue(texts.Count > 0, path);
        foreach (var tag in texts)
            CollectionAssert.AreEqual(BytesOf(tag.Value), file.Skip((int)tag.Offset).Take((int)tag.Length).ToArray(), $"{tag.Namespace}:{tag.Key} at {tag.Offset}");
    }

    /// <summary>Apple's maker notes: of the IFD after "Apple iOS\0", its offsets of the notes, big endian.</summary>
    [TestMethod]
    public void ReadsTheTagsOfApplesMakerNotes()
    {
        var apple = Read("exiv2/IMG_3578.heic").Where(t => t.Family == MetadataFamily.Exif && t.Namespace == "Apple").ToList();

        Console.WriteLine(string.Join(Environment.NewLine, apple.Select(t => $"{t.Namespace}:{t}")));
        Assert.AreEqual(11L, apple.Single(t => t.Key == "MakerNoteVersion").Value);
        Assert.AreEqual("D1BB528D-3E89-4DA9-A068-ECBBA5CFC837", Text(apple, "ImageUniqueID").TrimEnd('\0'));
        Assert.AreEqual("q900n", Text(apple, "QualityHint").TrimEnd('\0'));
        // the notes as their bytes too, as they were
        Assert.IsInstanceOfType<byte[]>(Read("exiv2/IMG_3578.heic").Single(t => t.Key == "MakerNote").Value);
    }

    [TestMethod]
    public void SaysWhatValuesMean()
    {
        var exif = Read("exiv2/IMG_3578.heic").Where(t => t.Family == MetadataFamily.Exif).ToList();

        var orientation = exif.First(t => t.Key == "Orientation");
        Assert.IsNotNull(orientation.Description, $"Orientation {orientation.Value}");
        StringAssert.Contains(exif.Single(t => t.Key == "ExposureTime").Description, " s");
        Assert.IsNotNull(exif.Single(t => t.Key == "Flash").Description);
        // a tag of a name of its own, not of its number, of all the tags of the Exif IFD
        Assert.IsFalse(exif.Any(t => t.Namespace == "Exif" && t.Key.StartsWith("0x")), string.Join(", ", exif.Where(t => t.Key.StartsWith("0x")).Select(t => t.Key)));
    }
}
