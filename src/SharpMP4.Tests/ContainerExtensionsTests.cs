using SharpISOBMFF;
using SharpMP4.Readers;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="ContainerExtensions"/> and <see cref="XmpReader"/>, with files ExifTool wrote the
/// tags of (TestData/Metadata/README.md): each tag reads as it was written, and the file writes back
/// byte for byte.
/// </summary>
[TestClass]
public class ContainerExtensionsTests
{
    private static string File(string name) => Path.Combine(AppContext.BaseDirectory, "TestData", "Metadata", name);

    private static List<MetadataTag> Read(string name)
    {
        using var stream = System.IO.File.OpenRead(File(name));
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(stream)));
        return ContainerExtensions.ReadMetadata(container);
    }

    private static object? Value(List<MetadataTag> tags, MetadataFamily family, string key) =>
        tags.Single(t => t.Family == family && t.Key == key).Value;

    /// <summary>iTunes items that were defined as bytes, or not at all, hold their 'data' boxes.</summary>
    [TestMethod]
    public void ReadsITunesItems()
    {
        var tags = Read("itunes-items.mp4");

        var expected = new Dictionary<string, string>
        {
            ["@pti"] = "Parent", ["@sti"] = "Short", ["GUID"] = "guid-1", ["VERS"] = "v1", ["ownr"] = "Me",
            ["prID"] = "pid-1", ["rldt"] = "2020:01:02", ["©arg"] = "Arr", ["©dir"] = "Dir",
            ["©grp"] = "Group1", ["©prd"] = "Prod", ["©st3"] = "sub",
        };
        foreach (var (key, value) in expected)
            Assert.AreEqual(value, Value(tags, MetadataFamily.ItemList, key), key);
        Assert.AreEqual("80", Value(tags, MetadataFamily.ItemList, "rate"));
    }

    /// <summary>Cameras' strings of 'udta', and the 3GPP collection name with its language.</summary>
    [TestMethod]
    public void ReadsUserDataStrings()
    {
        var tags = Read("user-data-strings.mp4");

        var expected = new Dictionary<string, string>
        {
            ["angl"] = "front", ["reel"] = "reel1", ["scen"] = "sc1", ["shot"] = "shot1", ["clid"] = "clip1",
            ["slno"] = "SN1", ["clfn"] = "cf1", ["info"] = "fw1", ["CNFV"] = "fw1",
        };
        foreach (var (key, value) in expected)
            Assert.AreEqual(value, Value(tags, MetadataFamily.UserData, key), key);

        var collection = tags.Single(t => t.Key == "coll");
        Assert.AreEqual("coll1", collection.Value);
        Assert.IsNull(collection.Language); // written as 0, no language
    }

    /// <summary>
    /// The XMP packet of the XMP Specification's 'uuid' box, and its values: language alternatives,
    /// arrays, structures in structures, and an attribute of x:xmpmeta.
    /// </summary>
    [TestMethod]
    public void ReadsXmp()
    {
        var packet = (string)Read("xmp.mp4").Single(t => t.Family == MetadataFamily.Xmp).Value!;
        var properties = XmpReader.Read(packet);
        string Path(XmpProperty p) => string.Concat(p.Path.Select((s, i) => s.Index > 0 ? s.ToString() : (i > 0 ? "/" : "") + s));
        var values = properties.ToDictionary(Path, p => (p.Value, p.Language));

        Assert.AreEqual(("Image::ExifTool 13.59", (string?)null), values["x:xmptk"]);
        Assert.AreEqual(("Default title", "x-default"), values["dc:title[1]"]);
        Assert.AreEqual(("Titre", "fr"), values["dc:title[2]"]);
        Assert.AreEqual(("Titel", "de-DE"), values["dc:title[3]"]);
        CollectionAssert.AreEqual(new[] { "one", "two", "three" }, properties.Where(p => p.Name == "subject").Select(p => p.Value).ToArray());
        CollectionAssert.AreEqual(new[] { "Alice", "Bob" }, properties.Where(p => p.Name == "creator").Select(p => p.Value).ToArray());
        Assert.AreEqual("Prague", values["Iptc4xmpCore:CreatorContactInfo/Iptc4xmpCore:CiAdrCity"].Value);
        Assert.AreEqual("0.5", values["mwg-rs:Regions/mwg-rs:RegionList[1]/mwg-rs:Area/stArea:x"].Value);
        Assert.AreEqual("100", values["mwg-rs:Regions/mwg-rs:AppliedToDimensions/stDim:w"].Value);
        Assert.AreEqual("1/125", values["exif:ExposureTime"].Value);
        Assert.AreEqual("2021-05-06T07:08:09+02:00", values["xmp:CreateDate"].Value);
        Assert.AreEqual("True", values["xmpRights:Marked"].Value);
        Assert.AreEqual("http://purl.org/dc/elements/1.1/", properties.First(p => p.Name == "title").Namespace);
    }

    [TestMethod]
    [DataRow("itunes-items.mp4")]
    [DataRow("user-data-strings.mp4")]
    [DataRow("xmp.mp4")]
    public void WritesBackAsItWasRead(string name)
    {
        byte[] original = System.IO.File.ReadAllBytes(File(name));
        var container = new Container();
        using (var input = new MemoryStream(original))
        {
            container.Read(new IsoStream(new StreamWrapper(input)));
            using var written = new MemoryStream();
            container.Write(new IsoStream(new StreamWrapper(written)));
            CollectionAssert.AreEqual(original, written.ToArray());
        }
    }
}
