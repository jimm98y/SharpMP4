using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against the 'rtp ' sample entry of an RTP hint track (ISO/IEC 14496-12 9.1.2) and against reading a box whose
/// syntax asks for more than the box holds.
/// </summary>
[TestClass]
public class HintSampleEntryTests
{
    // An 'stbl' of an 'stsd' of one 'rtp ' entry, then an 'stsz' of no samples
    private static string Stbl(string entry)
    {
        string stsd = (16 + entry.Length / 2).ToString("x8") + "73747364" + "00000000" + "00000001" + entry;
        string stsz = "00000014" + "7374737a" + "00000000" + "00000000" + "00000000";
        return (8 + (stsd.Length + stsz.Length) / 2).ToString("x8") + "7374626c" + stsd + stsz;
    }

    private static (SampleTableBox Stbl, byte[] Written) ReadAndWrite(string hex)
    {
        var stream = new IsoStream(new StreamWrapper(new MemoryStream(Convert.FromHexString(hex))));
        stream.ReadBox(0, null, out Box box, "");
        var output = new MemoryStream();
        new IsoStream(new StreamWrapper(output)).WriteBox(box, "");
        return ((SampleTableBox)box, output.ToArray());
    }

    /// <summary>
    /// An 'rtp ' entry as the RTP hint track's syntax has it: its versions, its largest packet, and the timescale of the
    /// RTP time stamps in a 'tims' box, read as such and written back as they were.
    /// </summary>
    [TestMethod]
    public void ReadsAnRtpHintSampleEntry()
    {
        string entry = "00000024" + "72747020" + "000000000000" + "0001" + "0001" + "0001" + "000005dc" +
                       "0000000c" + "74696d73" + "00015f90";
        string hex = Stbl(entry);

        var (stbl, written) = ReadAndWrite(hex);

        var rtp = (RtpHintSampleEntry)stbl.Children.OfType<SampleDescriptionBox>().Single().Children.Single();
        Assert.AreEqual(1, rtp.Hinttrackversion);
        Assert.AreEqual(1, rtp.Highestcompatibleversion);
        Assert.AreEqual(1500u, rtp.Maxpacketsize);
        Assert.AreEqual(90000u, rtp.Children.OfType<TimeScaleEntry>().Single().Timescale);
        CollectionAssert.AreEqual(Convert.FromHexString(hex), written);
    }

    /// <summary>
    /// An 'rtp ' entry without the fields of its hint track - as frag_bunny.mp4 has them, 16 bytes - is kept as its bytes,
    /// and the box after it read as it is: its fields were read out of the header of the 'stsz' after it, and the entry
    /// was written 8 bytes longer than its header said.
    /// </summary>
    [TestMethod]
    public void KeepsAnEntryThatDoesNotFitItsSyntaxAsItsBytes()
    {
        string hex = Stbl("00000010" + "72747020" + "000000000000" + "0001");

        var (stbl, written) = ReadAndWrite(hex);

        Assert.IsInstanceOfType<UnreadableBox>(stbl.Children.OfType<SampleDescriptionBox>().Single().Children.Single());
        Assert.AreEqual(0u, stbl.Children.OfType<SampleSizeBox>().Single().SampleCount);
        CollectionAssert.AreEqual(Convert.FromHexString(hex), written);
    }
}
