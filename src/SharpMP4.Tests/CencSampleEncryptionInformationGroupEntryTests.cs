using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="CencSampleEncryptionInformationGroupEntry"/>, 'seig' of ISO/IEC 23001-7, in the multiple key
/// form of its Amendment 1 (2019): multi_key_flag, then key_count keys, each with its IV size and KID, and a constant IV
/// where its IV size is 0.
/// </summary>
[TestClass]
public class CencSampleEncryptionInformationGroupEntryTests
{
    /// <summary>An 'sgpd' of version 1 with one 'seig' entry of two keys: the first with IVs of 8 bytes, the second a constant IV of 16.</summary>
    private static byte[] Build()
    {
        var entry = new List<byte> { 0x80, 0x19, 1, 0, 2 }; // multi_key_flag, reserved; crypt 1, skip 9; isProtected; key_count 2
        entry.Add(8);
        entry.AddRange(Enumerable.Range(0x10, 16).Select(b => (byte)b));
        entry.Add(0);
        entry.AddRange(Enumerable.Range(0x20, 16).Select(b => (byte)b));
        entry.Add(16);
        entry.AddRange(Enumerable.Range(0xA0, 16).Select(b => (byte)b));

        var body = new List<byte> { 1, 0, 0, 0 };                               // version 1, flags
        body.AddRange("seig"u8.ToArray());                                     // grouping_type
        body.AddRange([0, 0, 0, 0]);                                           // default_length: each entry's own
        body.AddRange([0, 0, 0, 1]);                                           // entry_count
        body.AddRange([0, 0, 0, (byte)entry.Count]);                           // description_length
        body.AddRange(entry);

        int size = 8 + body.Count;
        return [0, 0, 0, (byte)size, (byte)'s', (byte)'g', (byte)'p', (byte)'d', .. body];
    }

    [TestMethod]
    public void ReadsEachKey()
    {
        var container = new Container();
        using (var stream = new MemoryStream(Build()))
            container.Read(new IsoStream(new StreamWrapper(stream)));

        var seig = (CencSampleEncryptionInformationGroupEntry)((SampleGroupDescriptionBox)container.Children.Single())._SampleGroupDescriptionEntry.Single();
        Assert.IsTrue(seig.MultiKeyFlag);
        Assert.AreEqual(1, seig.CryptByteBlock);
        Assert.AreEqual(9, seig.SkipByteBlock);
        Assert.AreEqual(2, seig.KeyCount);
        CollectionAssert.AreEqual(new byte[] { 8, 0 }, seig.MultiPerSampleIVSize);
        CollectionAssert.AreEqual(Enumerable.Range(0x20, 16).Select(b => (byte)b).ToArray(), seig.MultiKID[1]);
        CollectionAssert.AreEqual(Enumerable.Range(0xA0, 16).Select(b => (byte)b).ToArray(), seig.MultiConstantIV[1]);
    }

    [TestMethod]
    public void WritesBackAsItWasRead()
    {
        byte[] bytes = Build();
        var container = new Container();
        using (var input = new MemoryStream(bytes))
            container.Read(new IsoStream(new StreamWrapper(input)));

        using var written = new MemoryStream();
        container.Write(new IsoStream(new StreamWrapper(written)));

        CollectionAssert.AreEqual(bytes, written.ToArray());
    }
}
