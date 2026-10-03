using System.Diagnostics;
using SharpISOBMFF;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// <see cref="VideoReader"/> on files of the corpus whose layout it misread: a sample entry whose codec configuration is
/// not its first box, and samples in the 'moov' of a fragmented file. What it reads is what ffprobe reads.
/// </summary>
[TestClass]
public class ReaderCorpusTests
{
    private static string Chromium(string name)
    {
        string? root = ConformanceCorpus.Locate();
        string path = Path.Combine(root ?? "", "chromium", name);
        if (root == null || !File.Exists(path))
            Assert.Inconclusive("no Chromium files; run DownloadConformance.ps1 -Codec Chromium");
        return path;
    }

    private static VideoReader Read(string path)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(File.ReadAllBytes(path)))));
        var reader = new VideoReader();
        reader.Parse(container);
        return reader;
    }

    /// <summary>
    /// The 'avc1' entry of bear-1280x720.mp4 has its 'pasp' before its 'avcC': the track is H.264, made from the 'avcC',
    /// where it was a generic track made from the 'pasp'.
    /// </summary>
    [TestMethod]
    public void MakesATrackFromItsCodecConfigurationWhereverItIs()
    {
        var reader = Read(Chromium("bear-1280x720.mp4"));
        var video = reader.Tracks.Values.Single(t => t.Track.HandlerType == "vide");
        var entry = video.Stbl.Children.OfType<SampleDescriptionBox>().Single().Children[0];
        Assert.AreEqual("pasp", IsoStream.ToFourCC(entry.Children[0].FourCC), "the entry's first box");
        Assert.IsInstanceOfType<H264Track>(video.Track);
    }

    /// <summary>
    /// bbb-320x240-2video-2audio.mp4 has the first 127 samples of each video track in its 'moov', the rest in its 593
    /// fragments: every one is read, as many as ffprobe reads, where those of the 'moov' were not.
    /// </summary>
    [TestMethod]
    public void ReadsAsManySamplesAsFfprobe()
    {
        string path = Chromium("bbb-320x240-2video-2audio.mp4");
        var reader = Read(path);
        var counts = reader.Tracks.OrderBy(t => t.Key).Select(t =>
        {
            int n = 0;
            for (var sample = reader.ReadSample(t.Key); sample != null; sample = reader.ReadSample(t.Key))
                n++;
            return n;
        }).ToArray();

        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
        {
            CollectionAssert.AreEqual(new[] { 720, 1127, 720, 1127 }, counts, "as ffprobe counted them");
            return;
        }

        var start = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(ffmpeg)!, "ffprobe" + Path.GetExtension(ffmpeg)))
        {
            UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true,
        };
        foreach (string argument in new[] { "-v", "error", "-count_packets", "-show_entries", "stream=nb_read_packets", "-of", "csv=p=0", path })
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        _ = process.StandardError.ReadToEndAsync();
        var probed = process.StandardOutput.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
        process.WaitForExit();

        CollectionAssert.AreEqual(probed, counts, $"ffprobe {string.Join(",", probed)}, read {string.Join(",", counts)}");
    }
}
