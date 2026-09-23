namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="AnnexB"/>, which splits an elementary stream into NAL units.</summary>
[TestClass]
public class AnnexBTests
{
    /// <summary>Both start code lengths appear in the same stream, and either may separate two units.</summary>
    [TestMethod]
    public void SplitsOnThreeAndFourByteStartCodes()
    {
        byte[] stream = [0, 0, 0, 1, 0x40, 0x01, 0xAA,
                         0x00, 0x00, 0x01, 0x42, 0x01, 0xBB, 0xCC,
                         0x00, 0x00, 0x00, 0x01, 0x44, 0x01, 0xDD];

        var units = AnnexB.ParseNalUnits(stream).ToList();

        Assert.AreEqual(3, units.Count);
        CollectionAssert.AreEqual(new byte[] { 0x40, 0x01, 0xAA }, units[0]);
        CollectionAssert.AreEqual(new byte[] { 0x42, 0x01, 0xBB, 0xCC }, units[1]);
        CollectionAssert.AreEqual(new byte[] { 0x44, 0x01, 0xDD }, units[2]);
    }

    /// <summary>
    /// A unit ends where the next one's start code begins. Ending it where the next one's payload
    /// begins instead leaves 00 00 01 - or 00 00 00 01 - on the end of it, which is a sequence no
    /// NAL unit may contain, and which then travels with a parameter set into the sample entry
    /// written from it.
    /// </summary>
    [TestMethod]
    public void DoesNotCarryTheNextStartCode()
    {
        byte[] stream = [0, 0, 0, 1, 0x42, 0x01, 0x99, 0, 0, 0, 1, 0x44, 0x01, 0x77];

        var units = AnnexB.ParseNalUnits(stream).ToList();

        CollectionAssert.AreEqual(new byte[] { 0x42, 0x01, 0x99 }, units[0]);
        CollectionAssert.AreEqual(new byte[] { 0x44, 0x01, 0x77 }, units[1]);
    }

    /// <summary>
    /// trailing_zero_8bits may follow a NAL unit, and is not part of it. A zero the encoder meant
    /// to keep is protected by an emulation prevention byte, so it is never the last byte.
    /// </summary>
    [TestMethod]
    public void DropsTrailingZeroBytes()
    {
        byte[] stream = [0, 0, 1, 0x26, 0x01, 0x55, 0, 0, 0, 0, 0, 1, 0x02, 0x01, 0x66, 0, 0];

        var units = AnnexB.ParseNalUnits(stream).ToList();

        Assert.AreEqual(2, units.Count);
        CollectionAssert.AreEqual(new byte[] { 0x26, 0x01, 0x55 }, units[0]);
        CollectionAssert.AreEqual(new byte[] { 0x02, 0x01, 0x66 }, units[1]);
    }

    /// <summary>Emulation prevention bytes belong to the NAL unit and are left where they are.</summary>
    [TestMethod]
    public void KeepsEmulationPreventionBytes()
    {
        byte[] stream = [0, 0, 0, 1, 0x40, 0x01, 0x00, 0x00, 0x03, 0x01, 0x11];

        var units = AnnexB.ParseNalUnits(stream).Single();

        CollectionAssert.AreEqual(new byte[] { 0x40, 0x01, 0x00, 0x00, 0x03, 0x01, 0x11 }, units);
    }

    [TestMethod]
    public void ReturnsNothingForEmptyInput()
    {
        Assert.AreEqual(0, AnnexB.ParseNalUnits(null).Count());
        Assert.AreEqual(0, AnnexB.ParseNalUnits([]).Count());
        Assert.AreEqual(0, AnnexB.ParseNalUnits([0, 0, 0, 1]).Count());
    }
}
