using System.Diagnostics;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Encryption;
using SharpMP4.Readers;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Protects Chromium's clear test clips with <see cref="FragmentedMp4Builder"/> and <see cref="Mp4Builder"/>, in each scheme of ISO/IEC 23001-7, and
/// reads them back: in the clear with the key, the very samples written; without it, each sample saying how it is
/// protected. And ffmpeg, whose demuxer decrypts as it reads, is to read the same samples out of them.
/// </summary>
[TestClass]
public class EncryptingTrackTests
{
    private static readonly byte[] KeyId = Convert.FromHexString("00112233445566778899aabbccddeeff");
    private static readonly byte[] Key128 = Convert.FromHexString("0f1e2d3c4b5a69788796a5b4c3d2e1f0");
    private static readonly byte[] Key256 = Convert.FromHexString("0f1e2d3c4b5a69788796a5b4c3d2e1f000112233445566778899aabbccddeeff");

    [TestMethod]
    [DataRow("bear-640x360-v_frag.mp4", "cenc")]
    [DataRow("bear-640x360-v_frag.mp4", "cbc1")]
    [DataRow("bear-640x360-v_frag.mp4", "cens")]
    [DataRow("bear-640x360-v_frag.mp4", "cbcs")]
    [DataRow("bear-1280x720-hevc.mp4", "cenc")]
    [DataRow("bear-1280x720-hevc.mp4", "cbcs")]
    [DataRow("bear-640x360-a_frag.mp4", "cenc")]
    [DataRow("bear-640x360-a_frag.mp4", "cbc1")]
    [DataRow("bear-640x360-a_frag.mp4", "cens")]
    [DataRow("bear-640x360-a_frag.mp4", "cbcs")]
    // AES-256, as the draft of 23001-7:2023 Amendment 1 (DAM 1) has it: 'tenc' version 2 saying so
    [DataRow("bear-640x360-v_frag.mp4", "cenc", 32)]
    [DataRow("bear-640x360-v_frag.mp4", "cbcs", 32)]
    [DataRow("bear-640x360-a_frag.mp4", "cenc", 32)]
    // not fragmented: the 'senc' in the 'trak', the 'saiz' and 'saio' in the 'stbl', its offset the file's
    [DataRow("bear-640x360-v_frag.mp4", "cenc", 16, false)]
    [DataRow("bear-640x360-v_frag.mp4", "cbc1", 16, false)]
    [DataRow("bear-640x360-v_frag.mp4", "cens", 16, false)]
    [DataRow("bear-640x360-v_frag.mp4", "cbcs", 16, false)]
    [DataRow("bear-1280x720-hevc.mp4", "cenc", 16, false)]
    [DataRow("bear-1280x720-hevc.mp4", "cbcs", 16, false)]
    [DataRow("bear-640x360-a_frag.mp4", "cenc", 16, false)]
    [DataRow("bear-640x360-a_frag.mp4", "cbcs", 16, false)]
    [DataRow("bear-640x360-v_frag.mp4", "cenc", 32, false)]
    public void WritesSamplesTheyDecryptBackTo(string clip, string scheme, int keySize = 16, bool fragmented = true)
    {
        byte[] Key = keySize == 32 ? Key256 : Key128;
        string? root = ConformanceCorpus.Locate();
        string path = Path.Combine(root ?? "", "chromium", clip);
        if (root == null || !File.Exists(path))
            Assert.Inconclusive("no Chromium files; run DownloadConformance.ps1 -Codec Chromium");

        var (clear, protectedFile, isVideo) = Protect(path, scheme, Key, fragmented);

        // with the key: the very samples written
        var decrypted = Read(protectedFile, _ => Key, out var protection, out var entryType, out _);
        Assert.AreEqual(scheme, protection!.Scheme);
        Assert.AreEqual(keySize == 32, protection.UseAes256, "use_AES_256");
        Assert.AreEqual(isVideo ? "encv" : "enca", entryType);
        CollectionAssert.AreEqual(KeyId, protection.DefaultKeyId);
        Assert.AreEqual(clear.Count, decrypted.Count, "sample count");
        for (int i = 0; i < clear.Count; i++)
            CollectionAssert.AreEqual(clear[i], decrypted[i], $"sample {i}");

        // without it: protected, as each sample says
        var encrypted = Read(protectedFile, null, out _, out _, out var encryptions);
        Assert.IsTrue(encryptions.All(e => e != null && e.IsProtected), "a sample not said to be protected");
        Assert.IsTrue(Enumerable.Range(0, clear.Count).Any(i => !clear[i].SequenceEqual(encrypted[i])), "nothing was encrypted");
        if (isVideo)
        {
            // one subsample a NAL unit, each covering the sample exactly (23001-7, 9.5.1)
            for (int i = 0; i < clear.Count; i++)
                Assert.AreEqual(clear[i].Length, encryptions[i]!.Subsamples.Sum(s => s.ClearBytes + (long)s.ProtectedBytes), $"sample {i}'s subsamples");
        }
        if (scheme != "cbcs")
        {
            // no IV twice under the key (10.1, 10.2, 10.3)
            Assert.AreEqual(encryptions.Count, encryptions.Select(e => Convert.ToHexString(e!.IV)).Distinct().Count(), "an IV used twice");
        }

        // the boxes: read and written back as they are, and each 'saio' pointing at its 'senc's samples, the same
        // auxiliary information - which neither reading above takes from it, both having the 'senc'
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(protectedFile))));
        using (var written = new MemoryStream())
        {
            container.Write(new IsoStream(new StreamWrapper(written)));
            CollectionAssert.AreEqual(protectedFile, written.ToArray(), "the file read and written back");
        }
        // in each 'traf', offsets from the 'moof'; or in the 'trak' and its 'stbl', offsets from the file's start
        var places = fragmented
            ? container.Children.OfType<MovieFragmentBox>().Select(moof => (Base: moof.GetBoxOffset(), Holder: (Box)moof.Children.OfType<TrackFragmentBox>().Single(), Table: (Box)moof.Children.OfType<TrackFragmentBox>().Single()))
            : container.Children.OfType<MovieBox>().SelectMany(moov => moov.Children.OfType<TrackBox>()).Select(trak => (Base: 0L, Holder: (Box)trak,
                Table: (Box)trak.Children.OfType<MediaBox>().Single().Children.OfType<MediaInformationBox>().Single().Children.OfType<SampleTableBox>().Single()));
        Assert.IsTrue(places.Any(), fragmented ? "no fragment" : "no track");
        foreach (var (baseOffset, holder, table) in places)
        {
            var senc = holder.Children.OfType<SampleEncryptionBox>().Single();
            var saio = table.Children.OfType<SampleAuxiliaryInformationOffsetsBox>().SingleOrDefault();
            var saiz = table.Children.OfType<SampleAuxiliaryInformationSizesBox>().SingleOrDefault();
            if (saio == null)
            {
                // none only where there is no auxiliary information: a constant IV and no subsamples (7.1)
                Assert.IsTrue((senc.SampleData?.Length ?? 0) == 0 && (senc.Flags & 2) == 0, "no 'saio' for auxiliary information");
                continue;
            }
            Assert.AreEqual(senc.GetBoxOffset() + 16, baseOffset + (long)saio.Offset[0], "'saio' not at the 'senc's samples");
            Assert.AreEqual(senc.SampleCount, saiz!.SampleCount);
        }

        // and as ffmpeg decrypts them - but for 'cens' without subsamples, which 23001-7 protects in whole blocks, the
        // 0 to 15 bytes after the last left clear (9.7, 10.3): ffmpeg's cens_scheme_decrypt takes every byte of such a
        // sample for AES-CTR's, as 'cenc' has them. Chromium, the other reader at hand, has no 'cens'.
        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null || (scheme == "cens" && !isVideo) || keySize == 32) // nor has ffmpeg AES-256
            return;
        string output = Path.Combine(Path.GetTempPath(), $"sharpmp4-{Guid.NewGuid():N}.mp4");
        string input = Path.Combine(Path.GetTempPath(), $"sharpmp4-{Guid.NewGuid():N}.mp4");
        try
        {
            File.WriteAllBytes(input, protectedFile);
            var start = new ProcessStartInfo(ffmpeg) { UseShellExecute = false, RedirectStandardError = true, CreateNoWindow = true };
            foreach (string argument in new[] { "-hide_banner", "-loglevel", "error", "-y", "-decryption_key", Convert.ToHexString(Key), "-i", input, "-c", "copy", "-map", "0", output })
                start.ArgumentList.Add(argument);
            using (var process = Process.Start(start)!)
            {
                string errors = process.StandardError.ReadToEnd();
                process.WaitForExit();
                Assert.AreEqual(0, process.ExitCode, errors);
            }

            var byFfmpeg = Read(File.ReadAllBytes(output), null, out _, out _, out _);
            Assert.AreEqual(clear.Count, byFfmpeg.Count, "ffmpeg's sample count");
            for (int i = 0; i < clear.Count; i++)
                CollectionAssert.AreEqual(clear[i], byFfmpeg[i], $"sample {i} as ffmpeg decrypts it");
        }
        finally
        {
            File.Delete(input);
            File.Delete(output);
        }
    }

    /// <summary>A clip's first video or audio track, its samples, and the same written protected.</summary>
    private static (List<byte[]> Clear, byte[] Protected, bool IsVideo) Protect(string path, string scheme, byte[] Key, bool fragmented)
    {
        using var stream = File.OpenRead(path);
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(stream)));
        var reader = new VideoReader();
        reader.Parse(container);

        var (trackID, context) = reader.Tracks.OrderBy(t => t.Value.Track.HandlerType == "vide" ? 0 : 1).First(t => t.Value.Track.HandlerType is "vide" or "soun");
        var track = context.Track;
        bool isVideo = track.HandlerType == "vide";

        var samples = new List<(byte[] Data, int Duration, bool IsRandomAccessPoint)>();
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
            samples.Add((sample.Data.ToArray(), sample.Duration, sample.IsRandomAccessPoint));

        using var output = new MemoryStream();
        var protection = TrackProtection.Create(scheme, KeyId, isVideo);
        protection.Systems.Add(new ProtectionSystemHeader
        {
            SystemId = Convert.FromHexString("1077efecc0b24d02ace33c1e52e2fb4b"), // W3C Common PSSH (ClearKey)
            KeyIds = [KeyId],
        });
        IMp4Builder builder;
        if (fragmented)
        {
            var fragmentedBuilder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 1000);
            fragmentedBuilder.AddTrack(track, protection, Key);
            builder = fragmentedBuilder;
        }
        else
        {
            var mp4Builder = new Mp4Builder(new SingleStreamOutput(output));
            mp4Builder.AddTrack(track, protection, Key);
            builder = mp4Builder;
        }
        foreach (var (data, duration, isRandomAccessPoint) in samples)
            builder.ProcessRawSample(track.TrackID, data, duration, isRandomAccessPoint);
        builder.FinalizeMedia();

        return (samples.Select(s => s.Data).ToList(), output.ToArray(), isVideo);
    }

    private static List<byte[]> Read(byte[] file, Func<byte[], byte[]?>? keyProvider, out TrackProtection? protection, out string? entryType, out List<SampleEncryption?> encryptions)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        var reader = new VideoReader { KeyProvider = keyProvider! };
        reader.Parse(container);

        var (trackID, context) = reader.Tracks.OrderBy(t => t.Value.Track.HandlerType == "vide" ? 0 : 1).First(t => t.Value.Track.HandlerType is "vide" or "soun");
        protection = context.Protection;
        entryType = IsoStream.ToFourCC(context.Stbl.Children.OfType<SampleDescriptionBox>().Single().Children.First().FourCC);

        var samples = new List<byte[]>();
        encryptions = [];
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
        {
            samples.Add(sample.Data.ToArray());
            encryptions.Add(sample.Encryption);
        }
        return samples;
    }
}
