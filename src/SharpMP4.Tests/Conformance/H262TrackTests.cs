using SharpH262;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System.Diagnostics;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// H.262 streams put in MP4 by <see cref="H262Track"/>: ffmpeg decodes the file to the pictures it decodes the stream to,
/// the sync samples are the I pictures, and the file reads back as the track it was written from.
/// </summary>
[TestClass]
public class H262TrackTests
{
    [TestMethod]
    public void PutsTheStreamsInMp4AsTheyDecode()
    {
        string? root = ConformanceCorpus.Locate();
        if (root == null)
            Assert.Inconclusive("no conformance files; run DownloadConformance.ps1, or set SHARPMP4_CONFORMANCE");
        string? ffmpeg = ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");

        var streams = ConformanceCorpus.Streams(root, "h262");
        if (streams.Count == 0)
            Assert.Inconclusive($"no h262 streams under {root}; run DownloadConformance.ps1 -Codec H262");

        var failures = new List<string>();
        int checkedStreams = 0;
        foreach (string path in streams)
        {
            byte[] es;
            try
            {
                es = SharpTrace.H262ElementaryStream(path);
            }
            catch (InvalidOperationException)
            {
                continue; // no video ffmpeg can identify: t.mpg
            }

            string name = Path.GetRelativePath(root, path);
            string mp4 = Path.Combine(Path.GetTempPath(), $"sharpmp4-h262-{Guid.NewGuid():N}.mp4");
            string m2v = Path.Combine(Path.GetTempPath(), $"sharpmp4-h262-{Guid.NewGuid():N}.m2v");
            try
            {
                File.WriteAllBytes(m2v, es);
                var (sync, iPictures) = Write(es, mp4);

                // the pictures of the file, as ffmpeg decodes them, are those of the stream - but for those ffmpeg decodes
                // otherwise each time: a damaged picture, which it conceals with what it has at hand (the last of
                // mpeg2_field_encoding.ts, a transport stream cut short)
                var fromStream = FrameHashes(ffmpeg, "mpegvideo", m2v);
                var again = FrameHashes(ffmpeg, "mpegvideo", m2v);
                var fromFile = FrameHashes(ffmpeg, "mp4", mp4);
                if (fromStream.Count == 0)
                    continue;
                if (fromFile.Count != fromStream.Count)
                    failures.Add($"{name}: decoded to {fromFile.Count} pictures of the stream's {fromStream.Count}");
                int differs = Enumerable.Range(0, Math.Min(fromStream.Count, fromFile.Count))
                    .FirstOrDefault(i => i < again.Count && fromStream[i] == again[i] && fromStream[i] != fromFile[i], -1);
                if (differs >= 0)
                    failures.Add($"{name}: picture {differs} of {fromStream.Count} decoded otherwise");
                if (!sync.SequenceEqual(iPictures))
                    failures.Add($"{name}: sync samples not the I pictures");

                // read back: a track of H.262, the entry of the stream's sequence header
                using (var stream = File.OpenRead(mp4))
                {
                    var container = new Container();
                    container.Read(new IsoStream(new StreamWrapper(stream)));
                    var reader = new VideoReader();
                    reader.Parse(container);
                    uint trackID = reader.Tracks.Keys.Single();
                    var track = reader.Tracks[trackID];
                    if (track.Track is not H262Track)
                        failures.Add($"{name}: read back as {track.Track.GetType().Name}");
                    int read = 0;
                    for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
                    {
                        if (sample.IsRandomAccessPoint != sync[read])
                            failures.Add($"{name}: sample {read} read back {(sample.IsRandomAccessPoint ? "a" : "not a")} sync sample");
                        read++;
                    }
                    if (read != sync.Count)
                        failures.Add($"{name}: {read} samples read back of {sync.Count}");
                }
                checkedStreams++;
            }
            catch (Exception ex)
            {
                failures.Add($"{name}: threw {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                File.Delete(mp4);
                File.Delete(m2v);
            }
        }

        Assert.IsTrue(checkedStreams > 0, "no stream checked");
        Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// Writes a stream in MP4, unit by unit through a track: the sync flag of each sample, and whether its first picture is
    /// an I picture, as the stream says.
    /// </summary>
    private static (List<bool> Sync, List<bool> IPictures) Write(byte[] es, string path)
    {
        var sync = new List<bool>();
        var iPictures = new List<bool>();
        var track = new H262Track(90000, 3600);
        using var output = File.Create(path);
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        builder.AddTrack(track);

        void Add(ArraySegment<byte> sample, bool isSync)
        {
            if (sample.Array == null)
                return;
            sync.Add(isSync);
            iPictures.Add(FirstPictureType(sample) == 1);
            builder.ProcessRawSample(track.TrackID, sample, 3600, isSync);
        }

        foreach (var (offset, length) in H262Context.Units(es, 0, es.Length))
        {
            track.ProcessSample(es, offset, length, out var sample, out bool isSync);
            Add(sample, isSync);
        }
        track.ProcessSample(null, out var last, out bool lastSync);
        Add(last, lastSync);
        builder.FinalizeMedia();
        return (sync, iPictures);
    }

    // picture_coding_type of a sample's first picture header: the 3 bits after temporal_reference's 10
    private static int FirstPictureType(ArraySegment<byte> sample)
    {
        var bytes = sample.ToArray();
        foreach (var (offset, length) in H262Context.Units(bytes, 0, bytes.Length))
        {
            if (bytes[offset + 3] == H262StartCodes.PICTURE && length >= 6)
                return (bytes[offset + 5] >> 3) & 0x7;
        }
        return -1;
    }

    /// <summary>
    /// The MD5 of each picture ffmpeg decodes a file to, in the order it gives them: on one thread, as how a picture cut short
    /// at the end of a stream is concealed depends, on more, on which thread gets there first.
    /// </summary>
    private static List<string> FrameHashes(string ffmpeg, string format, string path)
    {
        var start = new ProcessStartInfo(ffmpeg) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (string argument in new[] { "-hide_banner", "-nostdin", "-v", "error", "-threads", "1", "-f", format, "-i", path, "-map", "0:v:0", "-fps_mode", "passthrough", "-f", "framemd5", "-" })
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        process.StandardError.ReadToEndAsync();
        var hashes = new List<string>();
        for (string? line = process.StandardOutput.ReadLine(); line != null; line = process.StandardOutput.ReadLine())
        {
            if (line.StartsWith('#'))
                continue;
            hashes.Add(line.Split(',').Last().Trim());
        }
        process.WaitForExit();
        return hashes;
    }

}
