using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Encryption;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests on how <see cref="VideoReader"/> decrypts the samples of a protected track: in the scheme of each sample's own
/// sample entry, and, where it has no key, as asked.
/// </summary>
[TestClass]
public class ReaderEncryptionTests
{
    private static readonly byte[] KeyId = Convert.FromHexString("00112233445566778899aabbccddeeff");
    private static readonly byte[] Key = Convert.FromHexString("0f1e2d3c4b5a69788796a5b4c3d2e1f0");

    private static (byte[] File, byte[][] Clear) BuildProtected(string scheme)
    {
        var clear = Enumerable.Range(0, 6).Select(i =>
        {
            var bytes = new byte[64 + 16 * i];
            new Random(i).NextBytes(bytes);
            return bytes;
        }).ToArray();

        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track, TrackProtection.Create(scheme, KeyId, isVideo: false), Key);
        foreach (var sample in clear)
            builder.ProcessRawSample(track.TrackID, sample, 20, true);
        builder.FinalizeMedia();
        return (output.ToArray(), clear);
    }

    private static SampleDescriptionBox SampleDescriptions(Container container) =>
        container.Children.OfType<MovieBox>().Single().Children.OfType<TrackBox>().Single()
            .Children.OfType<MediaBox>().Single().Children.OfType<MediaInformationBox>().Single()
            .Children.OfType<SampleTableBox>().Single().Children.OfType<SampleDescriptionBox>().Single();

    /// <summary>
    /// A track of two protected sample entries, of two schemes, has each sample decrypted in the scheme of its own: its
    /// chunk's. They were decrypted in the scheme of the first, whichever theirs was.
    /// </summary>
    [TestMethod]
    public void DecryptsInTheSchemeOfTheSamplesOwnSampleEntry()
    {
        var (file, clear) = BuildProtected(ProtectionSchemes.Cbc1);
        var container = ReaderSampleTableTests.Read(file);

        // before the track's 'cbc1' entry, another, of 'cenc' and the same key, that no sample is of
        var stsd = SampleDescriptions(container);
        var other = SampleDescriptions(ReaderSampleTableTests.Read(file)).Children[0];
        other.Children.OfType<ProtectionSchemeInfoBox>().Single().Children.OfType<SchemeTypeBox>().Single().SchemeType = IsoStream.FromFourCC(ProtectionSchemes.Cenc);
        other.SetParent(stsd);
        stsd.Children.Insert(0, other);
        stsd.EntryCount++;
        var stsc = ((Box)stsd.GetParent()).Children.OfType<SampleToChunkBox>().Single();
        stsc.SampleDescriptionIndex = stsc.SampleDescriptionIndex.Select(_ => 2u).ToArray();

        var reader = new VideoReader { KeyProvider = _ => Key };
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();
        Assert.AreEqual(ProtectionSchemes.Cenc, reader.Tracks[trackID].Protection.Scheme, "the track's first protection");

        for (int i = 0; i < clear.Length; i++)
        {
            var sample = reader.ReadSample(trackID)!;
            Assert.IsNull(sample.Encryption, $"sample {i} left protected");
            CollectionAssert.AreEqual(clear[i], sample.Data.ToArray(), $"sample {i}");
        }
    }

    /// <summary>
    /// A sample whose key the provider does not have is left protected, saying how it is; or, asked for, is an error.
    /// </summary>
    [TestMethod]
    public void LeavesASampleWithoutItsKeyProtectedOrThrowsAsAsked()
    {
        var (file, clear) = BuildProtected(ProtectionSchemes.Cenc);

        var reader = new VideoReader { KeyProvider = _ => null! };
        reader.Parse(ReaderSampleTableTests.Read(file));
        uint trackID = reader.Tracks.Keys.Single();
        var sample = reader.ReadSample(trackID)!;
        Assert.IsNotNull(sample.Encryption, "said to be protected");
        Assert.IsTrue(sample.Encryption.IsProtected);
        CollectionAssert.AreNotEqual(clear[0], sample.Data.ToArray(), "decrypted, without a key");

        reader = new VideoReader { KeyProvider = _ => null!, ThrowOnMissingKey = true };
        reader.Parse(ReaderSampleTableTests.Read(file));
        var missing = Assert.ThrowsExactly<KeyNotFoundException>(() => reader.ReadSample(trackID));
        StringAssert.Contains(missing.Message, Convert.ToHexString(KeyId));

        // without a provider, nothing is to be decrypted, and nothing is missing
        reader = new VideoReader { ThrowOnMissingKey = true };
        reader.Parse(ReaderSampleTableTests.Read(file));
        Assert.IsNotNull(reader.ReadSample(trackID)!.Encryption);
    }
}
