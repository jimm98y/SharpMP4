using System.Diagnostics;
using SharpISOBMFF;
using SharpMP4.Encryption;
using SharpMP4.Readers;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Reads Chromium's protected test clips in the clear, and compares each of their samples with the same sample of the
/// clip they were made from. Chromium protects them with one test key (media/test/data/README.md): key ID
/// 30313233343536373839303132333435 and key ebdd62f16814d27b68ef122afce4ae3c; each crypto period of the clips with
/// key rotation takes both rotated left by one more byte.
/// </summary>
/// <remarks>
/// The 'senc' clips have no clip they were made from - the README's "same as above" is another encode - so they are
/// decrypted to an H.264 stream that ffmpeg is to decode without an error - which any ffmpeg does, where only some
/// decrypt them right. From 2025-04-05 (fe73b84879, "simplify and optimize av_aes_ctr_crypt()") to 2025-09-11
/// (335ba4a649, "reintroduce the block offset state"; e4e57bef31 on release/8.0) ffmpeg's AES-CTR started each call
/// on a new counter block, so each subsample's protected bytes, where 'cenc' runs the key stream on from the range
/// before: a build of those months (N-120424 of 2025-07-31) decrypts bear-640x360-v_frag-cenc-mdat.mp4 wrongly from
/// byte 694, the first range of the clips to end within a block. A build after (N-121271 of 2025-09-30) decrypts all
/// the clips here, the 'senc' ones too, to the very samples SharpMP4 does.
/// </remarks>
[TestClass]
public class EncryptedTrackTests
{
    private static readonly byte[] KeyId = Convert.FromHexString("30313233343536373839303132333435");
    private static readonly byte[] Key = Convert.FromHexString("ebdd62f16814d27b68ef122afce4ae3c");

    /// <summary>The test key of a key ID: the key rotated as the ID is.</summary>
    private static byte[]? KeyOf(byte[] keyId)
    {
        for (int n = 0; n < 16; n++)
        {
            if (Rotate(KeyId, n).SequenceEqual(keyId))
                return Rotate(Key, n);
        }
        return null;
    }

    private static byte[] Rotate(byte[] bytes, int n) => bytes.Skip(n).Concat(bytes.Take(n)).ToArray();

    [TestMethod]
    [DataRow("bear-640x360-v_frag.mp4", "bear-640x360-v_frag-cenc.mp4", "cenc")]
    [DataRow("bear-640x360-v_frag.mp4", "bear-640x360-v_frag-cenc-mdat.mp4", "cenc")]
    [DataRow("bear-640x360-v_frag.mp4", "bear-640x360-v_frag-cenc-key_rotation.mp4", "cenc")]
    [DataRow("bear-640x360-v_frag.mp4", "bear-640x360-v_frag-cens.mp4", "cens")]
    [DataRow("bear-640x360-v_frag.mp4", "bear-640x360-v_frag-cbc1.mp4", "cbc1")]
    [DataRow("bear-640x360-v_frag.mp4", "bear-640x360-v_frag-cbcs.mp4", "cbcs")]
    [DataRow("bear-640x360-a_frag.mp4", "bear-640x360-a_frag-cenc.mp4", "cenc")]
    [DataRow("bear-640x360-a_frag.mp4", "bear-640x360-a_frag-cenc-key_rotation.mp4", "cenc")]
    [DataRow("bear-640x360-a_frag.mp4", "bear-640x360-a_frag-cbcs.mp4", "cbcs")]
    [DataRow("bear-320x240-v_frag-vp9.mp4", "bear-320x240-v_frag-vp9-cenc.mp4", "cenc")]
    // encrypted with Bento4's mp4encrypt, and a key of its own
    [DataRow("bear-320x240-v-2fragments-open-gop_frag.mp4", "bear-320x240-v-2fragments-open-gop_frag-cenc.mp4", "cenc", "00000000000000000000000000000001")]
    public void DecryptsToTheSamplesItWasMadeFrom(string clearName, string protectedName, string scheme, string? key = null)
    {
        string? root = ConformanceCorpus.Locate();
        string clearPath = Path.Combine(root ?? "", "chromium", clearName), protectedPath = Path.Combine(root ?? "", "chromium", protectedName);
        if (root == null || !File.Exists(clearPath) || !File.Exists(protectedPath))
            Assert.Inconclusive("no Chromium files; run DownloadConformance.ps1 -Codec Chromium");

        Func<byte[], byte[]?> keyOf = key == null ? KeyOf : _ => Convert.FromHexString(key);
        var clear = Samples(clearPath, null, out _);
        var decrypted = Samples(protectedPath, keyOf, out TrackProtection? protection);

        Assert.IsNotNull(protection, "the track is not seen as protected");
        Assert.AreEqual(scheme, protection.Scheme);
        Assert.AreEqual(clear.Count, decrypted.Count, "sample count");
        for (int i = 0; i < clear.Count; i++)
            CollectionAssert.AreEqual(clear[i], decrypted[i], $"sample {i}");
    }

    /// <summary>
    /// Read without a key, a protected sample says how it is protected: which of its bytes, with which key and IV.
    /// </summary>
    [TestMethod]
    public void TellsHowEachSampleIsProtected()
    {
        string? root = ConformanceCorpus.Locate();
        string path = Path.Combine(root ?? "", "chromium", "bear-640x360-v_frag-cenc.mp4");
        if (root == null || !File.Exists(path))
            Assert.Inconclusive("no Chromium files; run DownloadConformance.ps1 -Codec Chromium");

        using var stream = File.OpenRead(path);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(stream)));
        var reader = new VideoReader();
        reader.Parse(container);

        uint trackID = reader.Tracks.Keys.Single();
        var protection = reader.Tracks[trackID].Protection;
        Assert.AreEqual("avc1", protection.OriginalFormat);
        CollectionAssert.AreEqual(KeyId, protection.DefaultKeyId);
        Assert.IsTrue(protection.Systems.Count > 0, "the 'pssh' boxes");
        Assert.IsInstanceOfType(reader.Tracks[trackID].Track, typeof(SharpMP4.Tracks.H264Track), "the codec behind 'encv'");

        // the clip starts in the clear, from a sample entry of its own: its first protected sample
        var sample = reader.ReadSample(trackID);
        while (sample != null && sample.Encryption == null)
            sample = reader.ReadSample(trackID);
        Assert.IsNotNull(sample, "no protected sample");
        CollectionAssert.AreEqual(KeyId, sample.Encryption.KeyId);
        Assert.AreEqual(protection.DefaultPerSampleIVSize, sample.Encryption.IV.Length);
        Assert.IsNotNull(sample.Encryption.Subsamples, "video's NAL units keep their headers clear");
        Assert.AreEqual(sample.Data.Count, sample.Encryption.Subsamples.Sum(s => s.ClearBytes + (long)s.ProtectedBytes));
    }

    /// <summary>
    /// A clip with no clip it was made from, decrypted to an H.264 stream - its avcC's parameter sets, then each sample's
    /// NAL units with start codes - which ffmpeg decodes without an error. Each has a sample whose first protected range
    /// ends within a block, which a key stream not run on from one range to the next leaves undecodable.
    /// </summary>
    [TestMethod]
    [DataRow("bear-640x360-v_frag-cenc-senc.mp4")]
    [DataRow("bear-640x360-v_frag-cenc-senc-no-saiz-saio.mp4")]
    public void DecryptsToAStreamThatDecodes(string protectedName)
    {
        string? root = ConformanceCorpus.Locate();
        string path = Path.Combine(root ?? "", "chromium", protectedName);
        if (root == null || !File.Exists(path))
            Assert.Inconclusive("no Chromium files; run DownloadConformance.ps1 -Codec Chromium");
        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");

        string output = Path.Combine(Path.GetTempPath(), $"sharpmp4-{Path.GetFileNameWithoutExtension(path)}-{Guid.NewGuid():N}.h264");
        try
        {
            using (var stream = File.OpenRead(path))
            using (var h264 = File.Create(output))
            {
                var container = new Container();
                container.Read(new IsoStream(new StreamWrapper(stream)));
                var reader = new VideoReader { KeyProvider = KeyOf! };
                reader.Parse(container);
                uint trackID = reader.Tracks.Keys.Single();

                var avcC = reader.Tracks[trackID].Stbl.Children.OfType<SampleDescriptionBox>().Single()
                    .Children.First().Children.OfType<AVCConfigurationBox>().Single()._AVCConfig;
                byte[] startCode = [0, 0, 0, 1];
                foreach (byte[] parameterSet in avcC.SequenceParameterSetNALUnit.Concat(avcC.PictureParameterSetNALUnit))
                {
                    h264.Write(startCode);
                    h264.Write(parameterSet);
                }

                int lengthSize = avcC.LengthSizeMinusOne + 1;
                for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
                {
                    Assert.IsNull(sample.Encryption, "a sample was not decrypted");
                    var data = sample.Data;
                    for (int p = 0; p < data.Count;)
                    {
                        int length = 0;
                        for (int k = 0; k < lengthSize; k++)
                            length = length << 8 | data[p + k];
                        p += lengthSize;
                        h264.Write(startCode);
                        h264.Write(data.Array!, data.Offset + p, length);
                        p += length;
                    }
                }
            }

            var start = new ProcessStartInfo(ffmpeg) { UseShellExecute = false, RedirectStandardError = true, CreateNoWindow = true };
            foreach (string argument in new[] { "-hide_banner", "-v", "error", "-i", output, "-f", "null", "-" })
                start.ArgumentList.Add(argument);
            using var process = Process.Start(start)!;
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();
            Assert.AreEqual(0, process.ExitCode, errors);
            Assert.AreEqual("", errors.Trim(), "ffmpeg's decoder");
        }
        finally
        {
            File.Delete(output);
        }
    }

    private static List<byte[]> Samples(string path, Func<byte[], byte[]?>? keyProvider, out TrackProtection? protection)
    {
        using var stream = File.OpenRead(path);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(stream)));
        var reader = new VideoReader { KeyProvider = keyProvider! };
        reader.Parse(container);

        uint trackID = reader.Tracks.Keys.Single();
        protection = reader.Tracks[trackID].Protection;

        var samples = new List<byte[]>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            if (keyProvider != null)
                Assert.IsNull(sample.Encryption, $"sample {samples.Count} was not decrypted");
            samples.Add(sample.Data.ToArray());
        }
        return samples;
    }
}
