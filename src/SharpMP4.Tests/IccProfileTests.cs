using System.Text;
using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// The ICC profile of a 'colr' box of the colour type 'prof' (ISO 15076-1, ICC.1:2010): its header, its tag table and
/// the elements of its tags, each of its type, traced of the bytes the profile is - and written as they were read.
/// </summary>
[TestClass]
public class IccProfileTests
{
    private static void U16(List<byte> bytes, int value) => bytes.AddRange(new[] { (byte)(value >> 8), (byte)value });
    private static void U32(List<byte> bytes, uint value) => bytes.AddRange(new[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value });
    private static void FourCC(List<byte> bytes, string fourCC) => bytes.AddRange(Encoding.ASCII.GetBytes(fourCC));

    /// <summary>
    /// A display profile of a white point, red and green curves of one element - a gamma of 2.2 - and a copyright in
    /// English, after its 128 bytes of header and its table of 3 tags.
    /// </summary>
    private static byte[] Profile()
    {
        var elements = new List<byte>();
        int start = 128 + 4 + 3 * 12;

        int wtpt = start + elements.Count;
        FourCC(elements, "XYZ "); U32(elements, 0);
        U32(elements, 0x0000F6D6); U32(elements, 0x00010000); U32(elements, 0x0000D32D);

        int curv = start + elements.Count;
        FourCC(elements, "curv"); U32(elements, 0); U32(elements, 1); U16(elements, 0x0233);
        elements.AddRange(new byte[2]); // to 4 bytes

        int cprt = start + elements.Count;
        FourCC(elements, "mluc"); U32(elements, 0); U32(elements, 1); U32(elements, 12);
        FourCC(elements, "en"); FourCC(elements, "US"); U32(elements, 6); U32(elements, 28);
        elements.AddRange(Encoding.BigEndianUnicode.GetBytes("(c)"));

        var profile = new List<byte>();
        U32(profile, (uint)(start + elements.Count));
        FourCC(profile, "appl"); profile.AddRange(new byte[] { 4, 0x20, 0, 0 });
        FourCC(profile, "mntr"); FourCC(profile, "RGB "); FourCC(profile, "XYZ ");
        foreach (int part in new[] { 2024, 1, 2, 3, 4, 5 })
            U16(profile, part);
        FourCC(profile, "acsp"); FourCC(profile, "APPL"); U32(profile, 0); FourCC(profile, "APPL"); U32(profile, 0);
        profile.AddRange(new byte[8]); U32(profile, 0);
        U32(profile, 0x0000F6D6); U32(profile, 0x00010000); U32(profile, 0x0000D32D);
        FourCC(profile, "appl");
        profile.AddRange(new byte[16 + 28]);
        Assert.AreEqual(128, profile.Count);

        U32(profile, 3);
        FourCC(profile, "wtpt"); U32(profile, (uint)wtpt); U32(profile, 20);
        FourCC(profile, "rTRC"); U32(profile, (uint)curv); U32(profile, 14);
        FourCC(profile, "gTRC"); U32(profile, (uint)curv); U32(profile, 14);
        // a table of 4 entries would not fit: of 3, the copyright is not in it, and is unused
        profile.AddRange(elements);
        _ = cprt;
        return profile.ToArray();
    }

    private static byte[] Colr(byte[] profile)
    {
        var box = new List<byte>();
        U32(box, (uint)(12 + profile.Length));
        FourCC(box, "colr");
        FourCC(box, "prof");
        box.AddRange(profile);
        return box.ToArray();
    }

    private static Box Read(byte[] bytes)
    {
        new IsoStream(new StreamWrapper(new MemoryStream(bytes))).ReadBox(0, null, out Box box, "");
        return box;
    }

    private static byte[] Write(Box box)
    {
        using var stream = new MemoryStream();
        new IsoStream(stream).WriteBox(box, "");
        return stream.ToArray();
    }

    [TestMethod]
    public void ReadsTheHeaderAndTheTagTable()
    {
        var colr = (ColourInformationBox)Read(Colr(Profile()));
        var profile = colr.ICCProfile;

        Assert.IsNotNull(profile);
        Assert.AreEqual("mntr", IsoStream.ToFourCC(profile.Header.ProfileDeviceClass));
        Assert.AreEqual("RGB ", IsoStream.ToFourCC(profile.Header.ColourSpace));
        Assert.AreEqual("XYZ ", IsoStream.ToFourCC(profile.Header.ProfileConnectionSpace));
        Assert.AreEqual(2024, profile.Header.Year);
        CollectionAssert.AreEqual(new[] { "wtpt", "rTRC", "gTRC" }, profile.Tags.Select(t => IsoStream.ToFourCC(t.TagSignature)).ToArray());
        Assert.AreEqual(profile.Tags[1].Offset, profile.Tags[2].Offset);
        // the two curves of one element, of the generated class of its type
        Assert.AreEqual(2, profile.Elements.Count);
        Assert.IsInstanceOfType<ICCXYZType>(profile.Elements[0].Value);
        var curve = (ICCCurveType)profile.Elements[1].Value;
        Assert.AreEqual(0x0233, curve.Gamma);
        Assert.AreEqual("rTRC, gTRC", profile.Elements[1].Name);
    }

    [TestMethod]
    public void TracesEachElementOfItsType()
    {
        var colr = (ColourInformationBox)Read(Colr(Profile()));
        string text = colr.ToString();

        Console.WriteLine(text);
        StringAssert.Contains(text, "ICCProfileHeader");
        StringAssert.Contains(text, "profile_device_class");
        StringAssert.Contains(text, "('mntr')");
        StringAssert.Contains(text, "profile_file_signature");
        StringAssert.Contains(text, "('acsp')");
        StringAssert.Contains(text, "tags[0]: ICCTagEntry");
        StringAssert.Contains(text, "('wtpt')");
        StringAssert.Contains(text, "wtpt: ICCXYZType");
        // the two curves of one element: it once
        StringAssert.Contains(text, "rTRC, gTRC: ICCCurveType");
        StringAssert.Contains(text, "gamma");
        // the curve's 2 bytes to 4, and the copyright the table has not: what no tag is of
        StringAssert.Contains(text, "unused");
        Assert.IsFalse(text.Contains("error"), text);
    }

    [TestMethod]
    public void TracesWhatItWrites()
    {
        byte[] bytes = Colr(Profile());
        var colr = (ColourInformationBox)Read(bytes);

        CollectionAssert.AreEqual(bytes, Write(colr));
        Assert.AreEqual(colr.ICCProfile.CalculateSize(), new BoxTracer().Append(colr.ICCProfile));
    }

    [TestMethod]
    public void ReadsTheStringsOfAMultiLocalizedElement()
    {
        // the copyright of the table: a fourth tag, the header and table 12 bytes longer, every offset with them
        var profile = Profile().ToList();
        int start = 128 + 4 + 3 * 12;
        var elements = profile.Skip(start).ToList();
        var table = new List<byte>();
        U32(table, 4);
        for (int i = 0; i < 3; i++)
        {
            var entry = profile.Skip(128 + 4 + 12 * i).Take(12).ToList();
            uint offset = (uint)(entry[4] << 24 | entry[5] << 16 | entry[6] << 8 | entry[7]) + 12;
            table.AddRange(entry.Take(4)); U32(table, offset); table.AddRange(entry.Skip(8));
        }
        FourCC(table, "cprt"); U32(table, (uint)(start + 12 + 20 + 16)); U32(table, (uint)(elements.Count - 36));
        var bytes = profile.Take(128).Concat(table).Concat(elements).ToArray();

        var colr = (ColourInformationBox)Read(Colr(bytes));
        string text = colr.ToString();

        Console.WriteLine(text);
        StringAssert.Contains(text, "cprt: ICCMultiLocalizedUnicodeType");
        var mluc = (ICCMultiLocalizedUnicodeType)colr.ICCProfile.Elements.Single(e => e.Name == "cprt").Value;
        Assert.AreEqual(1u, mluc.RecordCount);
        Assert.AreEqual("(c)", mluc.Strings.ToString());
        StringAssert.Contains(text, "alignment");
        Assert.AreEqual(colr.ICCProfile.CalculateSize(), new BoxTracer().Append(colr.ICCProfile));
    }

    [TestMethod]
    public void TracesTheProfilesOfTheConformanceFilesToTheirSize()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        // the repository's, of the files of other projects - not the tests' own Conformance
        while (root != null && !Directory.Exists(Path.Combine(root.FullName, "conformance", "chromium")))
            root = root.Parent;
        if (root == null)
            Assert.Inconclusive("No conformance files.");

        int profiles = 0;
        var texts = new List<string>();
        string conformance = Path.Combine(root.FullName, "conformance");
        foreach (var file in new[]
        {
            "chromium/quick-brown-fox-1280x720-hevc-rext-8bit-420-no-audio.mp4",
            "exiv2/IMG_3578.heic",
            "libavif/paris_icc_exif_xmp.avif",
            "libavif/seine_sdr_gainmap_srgb_icc.avif",
            "mp4parse/mp4parse/valid-avif-colr-nclx-and-prof-and-rICC.avif",
        }.Select(f => Path.Combine(conformance, f)).Where(File.Exists))
        {
            byte[] original = File.ReadAllBytes(file);
            var container = new Container();
            container.Read(new IsoStream(new StreamWrapper(new MemoryStream(original))));
            foreach (var colr in Boxes<ColourInformationBox>(container.Children).Where(c => c.ICCProfile != null))
            {
                profiles++;
                var profile = colr.ICCProfile;
                Assert.AreEqual(profile.CalculateSize(), new BoxTracer().Append(profile), file);
                Assert.IsTrue(profile.Tags.Count > 0, file);
                string text = colr.ToString();
                Assert.IsFalse(text.Contains(" error"), $"{file}\n{text}");
                Assert.AreEqual("acsp", IsoStream.ToFourCC(profile.Header.ProfileFileSignature), file);
                // the elements of the types of a class, each read of it
                Assert.IsTrue(profile.Elements.Any(e => e.Value != null), file);
                texts.AddRange(profile.Elements.Select(e => e.Value).OfType<ICCTextType>().Select(t => t.Text.Text));
                texts.AddRange(profile.Elements.Select(e => e.Value).OfType<ICCTextDescriptionType>().SelectMany(t => new[] { t.Ascii.Text, t.Unicode.Text }));
            }

            using var written = new MemoryStream();
            container.Write(new IsoStream(written));
            CollectionAssert.AreEqual(original, written.ToArray(), file);
        }
        if (profiles == 0)
            Assert.Inconclusive("No ICC profiles in the conformance files.");
        // a copyright, as text: of its bytes, its null too
        Assert.IsTrue(texts.Any(t => t.Contains("Copyright", StringComparison.OrdinalIgnoreCase)), string.Join(Environment.NewLine, texts));
    }

    private static IEnumerable<T> Boxes<T>(IEnumerable<Box>? boxes) where T : Box
    {
        foreach (var box in boxes ?? Enumerable.Empty<Box>())
        {
            if (box is T found)
                yield return found;
            foreach (var child in Boxes<T>(box.Children))
                yield return child;
        }
    }
}
