using SharpISOBMFF;
using SharpMP4.Encryption;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="SampleEncryptionBox"/>, 'senc' of ISO/IEC 23001-7, whose per sample IV size is in the
/// track's 'tenc' or a 'seig' sample group, not in the box: the box keeps its samples as bytes, and
/// <see cref="SampleEncryptionReader"/> reads them with the IV size of each sample's track and group.
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
        return Box("senc", body);
    }

    private static byte[] Box(string type, IEnumerable<byte> body)
    {
        var bytes = body.ToList();
        int size = 8 + bytes.Count;
        return [(byte)(size >> 24), (byte)(size >> 16), (byte)(size >> 8), (byte)size, .. System.Text.Encoding.ASCII.GetBytes(type), .. bytes];
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

    /// <summary>
    /// The samples split by the IV size the track gives: 'cbcs' audio, of a constant IV and no subsamples, has nothing
    /// after the sample count.
    /// </summary>
    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(8, false)]
    [DataRow(8, true)]
    [DataRow(16, false)]
    [DataRow(16, true)]
    public void ReadsTheSamplesByTheTracksIvSize(int ivSize, bool subsamples)
    {
        var senc = Read(Build(ivSize, subsamples));
        var entries = SampleEncryptionReader.ReadSampleData(senc.SampleData, (int)senc.SampleCount, senc.Flags, _ => ivSize);

        Assert.AreEqual(3, entries.Count);
        for (int sample = 0; sample < 3; sample++)
        {
            CollectionAssert.AreEqual(ivSize == 0 ? null : Enumerable.Range(0xA0 + sample, ivSize).Select(b => (byte)b).ToArray(), entries[sample].IV);
            if (subsamples)
                Assert.AreEqual("32+256", string.Join(" ", entries[sample].Subsamples!.Select(s => $"{s.ClearBytes}+{s.ProtectedBytes}")));
            else
                Assert.IsNull(entries[sample].Subsamples);
        }
    }

    /// <summary>
    /// A fragment whose middle sample is of a 'seig' group of 16 byte IVs, the others of the track's 8 byte ones: each
    /// sample's IV is as long as its own group or track says - which no one size for the box could give.
    /// </summary>
    [TestMethod]
    public void ReadsEachSampleByTheIvSizeOfItsGroup()
    {
        byte[] kid = Enumerable.Range(1, 16).Select(b => (byte)b).ToArray();
        byte[] sbgp = Box("sbgp", [
            0, 0, 0, 0, .. "seig"u8, 0, 0, 0, 3, // version, flags, grouping type, entries
            0, 0, 0, 1, 0, 0, 0, 0,               // a sample of the track's defaults
            0, 0, 0, 1, 0, 1, 0, 1,               // a sample of the fragment's first group (0x10001)
            0, 0, 0, 1, 0, 0, 0, 0]);
        byte[] sgpd = Box("sgpd", [
            1, 0, 0, 0, .. "seig"u8, 0, 0, 0, 20, 0, 0, 0, 1, // version 1, grouping type, default length, entries
            0, 0, 1, 16, .. kid]);                             // reserved, pattern, protected, 16 byte IVs, KID
        byte[] senc = Box("senc", [
            0, 0, 0, 0, 0, 0, 0, 3,
            .. Enumerable.Repeat((byte)0xA0, 8), .. Enumerable.Repeat((byte)0xB0, 16), .. Enumerable.Repeat((byte)0xC0, 8)]);
        byte[] traf = Box("traf", [.. sbgp, .. sgpd, .. senc]);

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(traf))));
        var protection = new TrackProtection { Scheme = ProtectionSchemes.Cenc, DefaultIsProtected = true, DefaultPerSampleIVSize = 8, DefaultKeyId = kid };

        var samples = SampleEncryptionReader.ForFragment(protection, container.Children.OfType<TrackFragmentBox>().Single(), null, 3, 0, null);
        CollectionAssert.AreEqual(Enumerable.Repeat((byte)0xA0, 8).ToArray(), samples[0].IV);
        CollectionAssert.AreEqual(Enumerable.Repeat((byte)0xB0, 16).ToArray(), samples[1].IV);
        CollectionAssert.AreEqual(Enumerable.Repeat((byte)0xC0, 8).ToArray(), samples[2].IV);
    }

    private static SampleEncryptionBox Read(byte[] bytes)
    {
        var container = new Container();
        using var stream = new MemoryStream(bytes);
        container.Read(new IsoStream(new StreamWrapper(stream)));
        return (SampleEncryptionBox)container.Children.Single();
    }
}
