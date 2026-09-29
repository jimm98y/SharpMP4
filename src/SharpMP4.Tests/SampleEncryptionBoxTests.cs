using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="SampleEncryptionBox"/>, 'senc' of ISO/IEC 23001-7, whose per sample IV size is in the
/// track's 'tenc' or a 'seig' sample group, not in the box: read alone, it is inferred from the box's size.
/// </summary>
[TestClass]
public class SampleEncryptionBoxTests
{
    /// <summary>A version 0 'senc' of three samples, each with an IV of the size given, and subsamples if asked.</summary>
    private static byte[] Build(int ivSize, bool subsamples)
    {
        var body = new List<byte> { 0, 0, 0, (byte)(subsamples ? 2 : 0), 0, 0, 0, 3 }; // version, flags, sample_count
        for (int sample = 0; sample < 3; sample++)
        {
            body.AddRange(Enumerable.Range(0xA0 + sample, ivSize).Select(b => (byte)b));
            if (subsamples)
                body.AddRange(new byte[] { 0, 1, 0, 0x20, 0, 0, 0x01, 0x00 }); // one subsample: 32 clear, 256 protected
        }
        int size = 8 + body.Count;
        return [0, 0, (byte)(size >> 8), (byte)size, (byte)'s', (byte)'e', (byte)'n', (byte)'c', .. body];
    }

    /// <summary>
    /// 'cbcs' audio has a constant IV and no subsamples, so its 'senc' holds a sample count and nothing more. That
    /// was read as IVs of 16 bytes, the size assumed before any is known, as nothing after the count was left to
    /// tell the sizes apart.
    /// </summary>
    [TestMethod]
    public void ReadsNothingAfterTheSampleCountAsAConstantIv()
    {
        Assert.AreEqual(0, Read(Build(0, subsamples: false)).PerSampleIVSize);
    }

    [TestMethod]
    [DataRow(0, true)]
    [DataRow(8, false)]
    [DataRow(8, true)]
    [DataRow(16, false)]
    [DataRow(16, true)]
    public void InfersTheIvSizeFromTheBoxSize(int ivSize, bool subsamples)
    {
        Assert.AreEqual((byte)ivSize, Read(Build(ivSize, subsamples)).PerSampleIVSize);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(8, true)]
    [DataRow(16, false)]
    public void WritesBackAsItWasRead(int ivSize, bool subsamples)
    {
        byte[] bytes = Build(ivSize, subsamples);
        var container = new Container();
        using (var input = new MemoryStream(bytes))
            container.Read(new IsoStream(new StreamWrapper(input)));

        using var written = new MemoryStream();
        container.Write(new IsoStream(new StreamWrapper(written)));

        CollectionAssert.AreEqual(bytes, written.ToArray());
        Assert.AreEqual((ulong)bytes.Length * 8, ((SampleEncryptionBox)container.Children.Single()).CalculateSize());
    }

    private static SampleEncryptionBox Read(byte[] bytes)
    {
        var container = new Container();
        using var stream = new MemoryStream(bytes);
        container.Read(new IsoStream(new StreamWrapper(stream)));
        return (SampleEncryptionBox)container.Children.Single();
    }
}
