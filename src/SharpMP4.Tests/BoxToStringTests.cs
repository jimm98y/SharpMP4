using SharpISOBMFF;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// The ToString of boxes, of their generated ToString(BoxStringBuilder): a line of each field as FFmpeg's trace_headers
/// traces one - its position in bits, its name, its bits, its value - each box's fields a level deeper than the box.
/// </summary>
[TestClass]
public class BoxToStringTests
{
    private static Box Read(byte[] bytes)
    {
        new IsoStream(new StreamWrapper(new MemoryStream(bytes))).ReadBox(0, null, out Box box, "");
        return box;
    }

    private static string NL(params string[] lines) => string.Join(Environment.NewLine, lines);

    private static string[] Lines(object value) => value.ToString()!.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

    // ftyp of the major brand 'isom', version 512, compatible with 'isom' and 'mp41'
    private static readonly byte[] Ftyp = Hex("00000018" + "66747970" + "69736F6D" + "00000200" + "69736F6D" + "6D703431");

    // stbl of an stts of two entries: 3 samples of 1024, 1 of 512
    private static readonly byte[] Stbl = Hex("00000028" + "7374626C" +
        "00000020" + "73747473" + "00000000" + "00000002" + "00000003" + "00000400" + "00000001" + "00000200");

    [TestMethod]
    public void DescribesEachFieldAsFFmpegTracesIt()
    {
        // the position in bits, the name - its box's fields a level in - the bits, ending where FFmpeg's do, the value
        CollectionAssert.AreEqual(new[]
        {
            "            'ftyp' FileTypeBox",
            "0             size                       00000000000000000000000000011000 = 24",
            "32            type                       01100110011101000111100101110000 = 1718909296 ('ftyp')",
            "64            major_brand                01101001011100110110111101101101 = 1769172845 ('isom')",
            "96            minor_version              00000000000000000000001000000000 = 512",
            "128           compatible_brands                                           = [1769172845 ('isom'), 1836069937 ('mp41')]",
        }, Lines(Read(Ftyp)));
    }

    [TestMethod]
    public void IndentsTheBoxesInABoxAndIndexesTheFieldsOfALoop()
    {
        CollectionAssert.AreEqual(new[]
        {
            "            'stbl' SampleTableBox",
            "0             size                       00000000000000000000000000101000 = 40",
            "32            type                       01110011011101000110001001101100 = 1937007212 ('stbl')",
            "              'stts' TimeToSampleBox",
            "64              size                     00000000000000000000000000100000 = 32",
            "96              type                     01110011011101000111010001110011 = 1937011827 ('stts')",
            "128             version                                          00000000 = 0",
            "136             flags                            000000000000000000000000 = 0",
            "160             entry_count              00000000000000000000000000000010 = 2",
            "192             sample_count[0]          00000000000000000000000000000011 = 3",
            "224             sample_delta[0]          00000000000000000000010000000000 = 1024",
            "256             sample_count[1]          00000000000000000000000000000001 = 1",
            "288             sample_delta[1]          00000000000000000000001000000000 = 512",
        }, Lines(Read(Stbl)));
    }

    [TestMethod]
    public void ShowsTheFieldsAsTheyAreNow()
    {
        var ftyp = (FileTypeBox)Read(Ftyp);
        ftyp.MajorBrand = IsoStream.FromFourCC("mp42");
        ftyp.CompatibleBrands = [IsoStream.FromFourCC("mp42")];

        string[] lines = Lines(ftyp);

        StringAssert.EndsWith(lines[1], " = 20");
        StringAssert.EndsWith(lines[3], "('mp42')");
        StringAssert.EndsWith(lines[5], "= [1836069938 ('mp42')]");
    }

    [TestMethod]
    public void ShowsTheBytesOfAStreamMarkerAsAnEllipsis()
    {
        string[] lines = Lines(Read(Hex("00000010" + "6D646174" + "0102030405060708")));

        Assert.AreEqual("64            data                                                        = ...", lines.Last());
    }

    [TestMethod]
    public void DescribesTheDescriptorsOfABox()
    {
        // esds of an ES_Descriptor of ES_ID 1, of a DecoderConfigDescriptor of AAC, its AudioSpecificConfig, and an SLConfigDescriptor
        string text = Read(Hex("00000027" + "65736473" + "00000000" +
            "03" + "19" + "0001" + "00" +
            "04" + "11" + "40" + "15" + "000000" + "00000000" + "00000000" + "05" + "02" + "1210" +
            "06" + "01" + "02")).ToString();

        // each descriptor's tag and size, of 7 bits a byte, before its fields; of none written, no line
        StringAssert.Contains(text, NL("              ES: ES_Descriptor", "96              tag                                              00000011 = 3", "104             sizeOfInstance                                            = 25"));
        StringAssert.Contains(text, NL("                AudioSpecificConfig", "256                 tag                                          00000101 = 5"));
        StringAssert.Contains(text, NL("                  GASpecificConfig", "285                   frameLengthFlag                                   0 = 0"));
        StringAssert.Contains(text, NL("304               predefined                                     00000010 = 2", ""));
    }

    [TestMethod]
    [DataRow("itunes-items.mp4")]
    [DataRow("user-data-strings.mp4")]
    [DataRow("xmp.mp4")]
    public void DescribesAFile(string name)
    {
        using var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "TestData", "Metadata", name));
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(stream)));

        string text = container.ToString();

        Console.WriteLine(text);
        Assert.IsFalse(Lines(container).Any(x => x.TrimStart().StartsWith("error") || x.Contains(" error ")), text);
        StringAssert.Contains(text, "'moov' MovieBox");
    }
}
