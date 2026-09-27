using SharpAV1;
using SharpAVX;
using SharpMP4.Common;

namespace SharpMP4.Tests;

/// <summary>Tests against reading AV1 OBUs.</summary>
[TestClass]
public class AV1ObuTests
{
    // An OBU from the Argon Streams conformance stream profile0_core_special/test1, in Annex B, so
    // without obu_size: HDR content light level metadata, its metadata_type coded as a leb128 of 6
    // bytes - a valid if roundabout encoding of 1 - then 16 trailing bits.
    private static readonly byte[] MetadataObu = Convert.FromHexString("28818080808000000000008000");

    /// <summary>
    /// The bytes of a leb128 count in get_position(): an OBU without obu_size is as long as its
    /// payload, which ends in trailing bits worked out from the position. The leb128 bytes were
    /// read around the bitstream, so the payload came out 48 bits short and 48 more trailing bits
    /// were read - past the end of the OBU.
    /// </summary>
    [TestMethod]
    public void CountsTheBytesOfALeb128InThePayload()
    {
        var context = new AV1Context();
        using var stream = new AomStream(new MemoryStream(MetadataObu), new DefaultMp4Logger());

        context.Read(stream, MetadataObu.Length);

        Assert.AreEqual(AV1ObuTypes.OBU_METADATA, context._ObuType);
        Assert.AreEqual(MetadataObu.Length * 8, stream.GetPosition(), "read to the end of the OBU, and no further");
    }

    // An OBU from Argon's profile0_core_special/test30: ITU-T T.35 metadata, obu_size in a leb128
    // of 5 bytes and metadata_type in one of 8, country code 0x7F, 21 bytes of payload, then its
    // trailing bits and 9 bytes of zeros.
    private static readonly byte[] ItutT35Obu = Convert.FromHexString(
        "2aa78080800084808080808080007f4b7a158d5142a17ee6b3d2c5674ad48dfdcde469de800000000000000000");

    /// <summary>
    /// The T.35 payload runs to the OBU's trailing bits; the syntax gives it no length. It was
    /// read as nothing, and the payload taken for trailing bits.
    /// </summary>
    [TestMethod]
    public void ReadsAT35PayloadUpToTheTrailingBits()
    {
        var context = new AV1Context();
        using var stream = new AomStream(new MemoryStream(ItutT35Obu), new DefaultMp4Logger());

        context.Read(stream, ItutT35Obu.Length);

        CollectionAssert.AreEqual(Convert.FromHexString("4b7a158d5142a17ee6b3d2c5674ad48dfdcde469de"), context.ItutT35Payload);
        Assert.AreEqual(ItutT35Obu.Length * 8, stream.GetPosition());
    }

    /// <summary>
    /// ObuSizeLen is obu_size's length, which says where the payload starts. Every leb128 read
    /// set it, so after this OBU it was metadata_type's 8 bytes, not obu_size's 5.
    /// </summary>
    [TestMethod]
    public void KeepsTheLengthOfObuSizeOnly()
    {
        var context = new AV1Context();
        using var stream = new AomStream(new MemoryStream(ItutT35Obu), new DefaultMp4Logger());

        context.Read(stream, ItutT35Obu.Length);

        Assert.AreEqual(5 * 8, context.ObuSizeLen);
    }

    private static byte[] WriteAgain(byte[] obu, Action<AV1Obu>? change = null)
    {
        var reader = new AV1Context { RecordSyntax = true };
        using (var stream = new AomStream(new MemoryStream(obu), new DefaultMp4Logger()))
            reader.Read(stream, obu.Length);
        change?.Invoke(reader.LastObu);

        var written = new MemoryStream();
        using (var stream = new AomStream(written, new DefaultMp4Logger()))
            new AV1Context().Write(stream, reader.LastObu);
        return written.ToArray();
    }

    /// <summary>
    /// An OBU is written again as it was read: its leb128s as long as they were coded - obu_size in
    /// 5 bytes and metadata_type in 8 where 1 each would do - and its payload.
    /// </summary>
    [TestMethod]
    public void WritesAnObuAsItWasRead()
    {
        CollectionAssert.AreEqual(ItutT35Obu, WriteAgain(ItutT35Obu));
        CollectionAssert.AreEqual(MetadataObu, WriteAgain(MetadataObu));
    }

    /// <summary>A payload changed in the state it was read into is written in its place.</summary>
    [TestMethod]
    public void WritesAChangedT35Payload()
    {
        byte[] payload = Convert.FromHexString("0102030405060708090a0b0c0d0e0f101112131415");
        byte[] written = WriteAgain(ItutT35Obu, obu => obu.Edit().ItutT35Payload = payload);

        var context = new AV1Context();
        using var stream = new AomStream(new MemoryStream(written), new DefaultMp4Logger());
        context.Read(stream, written.Length);
        CollectionAssert.AreEqual(payload, context.ItutT35Payload);
        Assert.AreEqual(ItutT35Obu.Length, written.Length);
    }
}
