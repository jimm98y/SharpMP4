using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="ItemEncryptionBox"/>, 'ienc' of ISO/IEC 23001-7.</summary>
[TestClass]
public class ItemEncryptionBoxTests
{
    // Version 0, two keys, each with a constant IV: the first of 8 bytes, the second of 16
    private static readonly byte[] TwoKeys = Build();

    private static byte[] Build()
    {
        var body = new List<byte> { 0, 0, 0, 0, 0, 0, 2 }; // version, flags, reserved, reserved, num_keys
        foreach (int ivSize in new[] { 8, 16 })
        {
            body.Add(0); // Per_Sample_IV_Size: a constant IV
            body.AddRange(Enumerable.Range(0x10, 16).Select(b => (byte)b)); // KID
            body.Add((byte)ivSize);
            body.AddRange(Enumerable.Range(0xA0, ivSize).Select(b => (byte)b));
        }
        int size = 8 + body.Count;
        return [0, 0, 0, (byte)size, (byte)'i', (byte)'e', (byte)'n', (byte)'c', .. body];
    }

    /// <summary>
    /// Each key's constant IV is as long as its own constant_IV_size. Read with the sizes of all the keys
    /// taken for one number, two keys' IVs were read as 2064 bytes (0x0810), and three keys' threw.
    /// </summary>
    [TestMethod]
    public void ReadsEachKeysIvAsLongAsItsOwnSize()
    {
        var ienc = Read(TwoKeys);

        Assert.AreEqual(8, ienc.ConstantIV[0].Length);
        Assert.AreEqual(16, ienc.ConstantIV[1].Length);
    }

    [TestMethod]
    public void WritesBackAsItWasRead()
    {
        var container = new Container();
        using (var input = new MemoryStream(TwoKeys))
            container.Read(new IsoStream(new StreamWrapper(input)));

        using var written = new MemoryStream();
        container.Write(new IsoStream(new StreamWrapper(written)));

        CollectionAssert.AreEqual(TwoKeys, written.ToArray());
        Assert.AreEqual((ulong)TwoKeys.Length * 8, ((ItemEncryptionBox)container.Children.Single()).CalculateSize());
    }

    private static ItemEncryptionBox Read(byte[] bytes)
    {
        var container = new Container();
        using var stream = new MemoryStream(bytes);
        container.Read(new IsoStream(new StreamWrapper(stream)));
        return (ItemEncryptionBox)container.Children.Single();
    }
}
