using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Encryption;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="SampleAuxiliaryInformationSizesBox"/>, 'saiz', whose versions 1 and 2 (14496-12 9th edition)
/// give each sample's size in 16 and 32 bits: a sample's Common Encryption information - its IV, then 2 bytes and 6 a
/// subsample - outgrows version 0's 8 bits at 41 subsamples with an 8 byte IV, 43 with none, as an AV1 sample of one
/// subsample a tile does (AOMediaCodec/av2-isobmff#60).
/// </summary>
[TestClass]
public class SampleAuxiliaryInformationSizesBoxTests
{
    [TestMethod]
    [DataRow((byte)0, 200u)]
    [DataRow((byte)1, 300u)]
    [DataRow((byte)2, 70000u)]
    public void WritesBackAsItWasRead(byte version, uint largest)
    {
        var box = new SampleAuxiliaryInformationSizesBox(version, 1)
        {
            AuxInfoType = IsoStream.FromFourCC("cenc"),
            SampleCount = 3,
            SampleInfoSize = [16, largest, 22],
        };
        byte[] bytes = Write(box);

        // header, version and flags, aux_info_type and its parameter, default size, count, and the sizes
        int width = version == 0 ? 1 : version == 1 ? 2 : 4;
        Assert.AreEqual(8 + 4 + 8 + width + 4 + 3 * width, bytes.Length);

        var read = Read(bytes);
        Assert.AreEqual(version, read.Version);
        Assert.AreEqual(0u, read.DefaultSampleInfoSize);
        CollectionAssert.AreEqual(new uint[] { 16, largest, 22 }, read.SampleInfoSize);
        CollectionAssert.AreEqual(bytes, Write(read));
    }

    /// <summary>
    /// Samples whose information fits 8 bits keep to version 0, as readers before the 9th edition read it; one of more takes
    /// version 1, where before 'saiz' and 'saio' were left out and only 'senc' said how the samples are protected. Read
    /// back, the samples decrypt by them.
    /// </summary>
    [TestMethod]
    [DataRow(30, (byte)0)]
    [DataRow(60, (byte)1)]
    public void TakesTheVersionTheSizesNeed(int slices, byte version)
    {
        // the parameter sets of Chromium's bear-640x360-v_frag.mp4
        var track = new H264Track();
        track.ProcessSample(Convert.FromHexString("6764001eacd940a02ff9701100000303e90000ea600f162d96"), out _, out _);
        track.ProcessSample(Convert.FromHexString("68ebe3cb22c0"), out _, out _);

        // a sample of so many IDR slices of 100 bytes, each a subsample of its own
        var random = new Random(1);
        var sample = new List<byte>();
        for (int i = 0; i < slices; i++)
        {
            var slice = new byte[100];
            random.NextBytes(slice);
            slice[0] = 0x65;
            sample.AddRange(new byte[] { 0, 0, 0, 100 });
            sample.AddRange(slice);
        }

        byte[] keyId = Enumerable.Range(1, 16).Select(x => (byte)x).ToArray(), key = Enumerable.Range(17, 16).Select(x => (byte)x).ToArray();
        using var output = new MemoryStream();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 60_000);
        builder.AddTrack(track, TrackProtection.Create(ProtectionSchemes.Cenc, keyId, isVideo: true), key);
        builder.ProcessRawSample(track.TrackID, sample.ToArray(), 1000, true, new TemporaryMemory());
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var saiz = Boxes.FindAll<SampleAuxiliaryInformationSizesBox>(container).Single();
        Assert.AreEqual(version, saiz.Version);
        // the IV, the subsample count and 6 bytes a subsample: a slice whose header the random bytes make long is left
        // clear, so of the slices, most
        uint size = saiz.DefaultSampleInfoSize;
        Assert.AreEqual(0u, (size - 8 - 2) % 6);
        Assert.IsTrue((size - 10) / 6 > slices * 3 / 4, $"{(size - 10) / 6} subsamples of {slices} slices");
        Assert.AreEqual(version == 0, size <= byte.MaxValue);

        // read by 'saiz' and 'saio' alone, without the 'senc'
        foreach (var traf in Boxes.FindAll<TrackFragmentBox>(container))
            traf.Children.RemoveAll(b => b is SampleEncryptionBox);
        var reader = new VideoReader { KeyProvider = id => id.SequenceEqual(keyId) ? key : null };
        reader.Parse(container);
        var read = reader.ReadSample(reader.Tracks.Keys.Single());
        Assert.IsNotNull(read);
        Assert.IsNull(read.Encryption, "not decrypted");
        CollectionAssert.AreEqual(sample.ToArray(), read.Data.ToArray());
    }

    private static byte[] Write(Box box)
    {
        // a box's header is its container's to write
        var container = new Container();
        container.Children.Add(box);
        box.SetParent(container);
        using var output = new MemoryStream();
        container.Write(new IsoStream(output));
        return output.ToArray();
    }

    private static SampleAuxiliaryInformationSizesBox Read(byte[] bytes)
    {
        var container = new Container();
        using (var input = new MemoryStream(bytes))
            container.Read(new IsoStream(new StreamWrapper(input)));
        return container.Children.OfType<SampleAuxiliaryInformationSizesBox>().Single();
    }
}
