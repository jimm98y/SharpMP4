// SharpMP4
// Copyright (C) 2026 Lukas Volf
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Diagnostics;
using System.Text;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Timed metadata tracks (ISO/IEC 14496-12 12.3) - 'urim', 'mett' and 'metx', fragmented and not, MP4 and QuickTime -
/// written beside a track they describe, and read back: the entry as it was made, the 'cdsc' reference to the track, and
/// each sample's bytes over its time, an empty one among them. Copied to another file, the track writes its entry and
/// its reference as it read them; ffmpeg reads the samples written as the bytes they were.
/// </summary>
[TestClass]
public class MetadataTrackTests
{
    private const string DetectionsUri = "urn:example:detections:1";
    private static readonly byte[] InitializationData = [1, 2, 3, 4];

    // over 1000 units a second: two samples of what was seen, one of nothing seen, and one more
    private static readonly (int Duration, byte[] Data)[] Samples =
    [
        (200, Encoding.UTF8.GetBytes("{\"track\":1,\"class\":\"person\",\"box\":[10,20,30,40]}")),
        (200, Encoding.UTF8.GetBytes("{\"track\":1,\"class\":\"person\",\"box\":[12,21,30,40]}")),
        (400, []),
        (200, Encoding.UTF8.GetBytes("{\"track\":2,\"class\":\"dog\",\"box\":[50,60,20,10]}")),
    ];

    public static IEnumerable<object[]> Kinds() =>
        from kind in new[] { URIMetaSampleEntry.TYPE, TextMetaDataSampleEntry.TYPE, XMLMetaDataSampleEntry.TYPE }
        from fragmented in new[] { false, true }
        from quickTime in new[] { false, true }
        select new object[] { kind, fragmented, quickTime };

    private static MetadataTrack Create(string kind) => kind switch
    {
        URIMetaSampleEntry.TYPE => new UriMetadataTrack(DetectionsUri, InitializationData),
        TextMetaDataSampleEntry.TYPE => new TextMetadataTrack("application/json"),
        _ => new XmlMetadataTrack("urn:example:detections", "urn:example:detections detections.xsd"),
    };

    /// <summary>A file of timed text - the track described - and the metadata of it, as its 'cdsc' says.</summary>
    private static byte[] Build(string kind, bool fragmented, bool quickTime, (int Duration, byte[] Data)[]? samples = null)
    {
        samples ??= Samples;
        using var output = new MemoryStream();
        var format = quickTime ? Mp4FileFormat.QuickTime : Mp4FileFormat.Mp4;
        IMp4Builder builder = fragmented
            ? new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 500) { FileFormat = format }
            : new Mp4Builder(new SingleStreamOutput(output)) { FileFormat = format };

        var text = new TimedTextTrack();
        builder.AddTrack(text);
        var metadata = Create(kind);
        builder.AddTrack(metadata);
        metadata.DescribedTrackIDs = [text.TrackID];

        builder.ProcessTrackSample(text.TrackID, text.CreateSample([new SubtitleCue { Text = "Described" }]), 1000);
        foreach (var (duration, data) in samples)
            builder.ProcessTrackSample(metadata.TrackID, data, duration);
        builder.FinalizeMedia();
        return output.ToArray();
    }

    private static (Container Container, VideoReader Reader) Read(byte[] file)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        var reader = new VideoReader();
        reader.Parse(container);
        return (container, reader);
    }

    private static uint MetadataTrackID(VideoReader reader) =>
        reader.Tracks.Single(x => x.Value.Track is MetadataTrack).Key;

    private static uint DescribedTrackID(VideoReader reader) =>
        reader.Tracks.Single(x => x.Value.Track is TimedTextTrack).Key;

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void ReadsBackTheTrackItWrote(string kind, bool fragmented, bool quickTime)
    {
        var (container, reader) = Read(Build(kind, fragmented, quickTime));
        uint trackID = MetadataTrackID(reader);
        var track = (MetadataTrack)reader.Tracks[trackID].Track;

        Assert.AreEqual(kind, track.SampleEntryType);
        Assert.AreEqual(HandlerTypes.Metadata, track.HandlerType);
        CollectionAssert.AreEqual(new[] { DescribedTrackID(reader) }, track.DescribedTrackIDs.ToArray(), "its 'cdsc'");
        switch (Create(kind))
        {
            case UriMetadataTrack uri:
                var readUri = (UriMetadataTrack)track;
                Assert.AreEqual(uri.Uri, readUri.Uri);
                CollectionAssert.AreEqual(uri.InitializationData, readUri.InitializationData);
                break;
            case TextMetadataTrack text:
                var readText = (TextMetadataTrack)track;
                Assert.AreEqual(text.MimeFormat, readText.MimeFormat);
                Assert.IsNull(readText.ContentEncoding);
                break;
            case XmlMetadataTrack xml:
                var readXml = (XmlMetadataTrack)track;
                Assert.AreEqual(xml.Namespace, readXml.Namespace);
                Assert.AreEqual(xml.SchemaLocation, readXml.SchemaLocation);
                Assert.IsNull(readXml.ContentEncoding);
                break;
        }

        // no media header of its own in an MP4; QuickTime's base one
        var trak = Boxes.FindAll<TrackBox>(container).Single(t => t.Children.OfType<TrackHeaderBox>().Single().TrackID == trackID);
        var minf = trak.Children.OfType<MediaBox>().Single().Children.OfType<MediaInformationBox>().Single();
        if (quickTime)
            Assert.AreEqual(1, minf.Children.OfType<GenericMediaHeaderAtom>().Count(), "a 'gmhd'");
        else
            Assert.AreEqual(1, minf.Children.OfType<NullMediaHeaderBox>().Count(), "an 'nmhd'");

        // the 'tref' after the 'tkhd', before the media
        var order = trak.Children.Select(b => IsoStream.ToFourCC(b.FourCC)).ToList();
        Assert.IsTrue(order.IndexOf("tkhd") < order.IndexOf("tref") && order.IndexOf("tref") < order.IndexOf("mdia"), string.Join(",", order));

        int i = 0;
        long time = 0;
        for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID), i++)
        {
            Assert.AreEqual(time, sample.PTS, $"sample {i} start");
            Assert.AreEqual(Samples[i].Duration, sample.Duration, $"sample {i} duration");
            CollectionAssert.AreEqual(Samples[i].Data, sample.Data.ToArray(), $"sample {i}");
            time += Samples[i].Duration;
        }
        Assert.AreEqual(Samples.Length, i);
    }

    /// <summary>A track read and written again writes the entry it read, byte for byte, and its reference to the track it describes.</summary>
    [TestMethod]
    [DataRow(URIMetaSampleEntry.TYPE)]
    [DataRow(TextMetaDataSampleEntry.TYPE)]
    [DataRow(XMLMetaDataSampleEntry.TYPE)]
    public void CopiesTheTrackAsItWasRead(string kind)
    {
        var (container, reader) = Read(Build(kind, fragmented: false, quickTime: false));

        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        var ids = new Dictionary<uint, uint>();
        foreach (var (id, context) in reader.Tracks.OrderBy(x => x.Key))
        {
            var clone = context.Track.Clone();
            builder.AddTrack(clone);
            ids[id] = clone.TrackID;
        }
        foreach (var id in reader.Tracks.Keys)
        {
            if (((IMp4Builder)builder).GetTrack(ids[id]) is MetadataTrack metadata)
                metadata.DescribedTrackIDs = metadata.DescribedTrackIDs.Select(x => ids[x]).ToList();
            for (var sample = reader.ReadSample(id); sample != null; sample = reader.ReadSample(id))
                builder.ProcessRawSample(ids[id], sample.Data.ToArray(), (int)sample.Duration, sample.IsRandomAccessPoint);
        }
        builder.FinalizeMedia();

        var (copied, copiedReader) = Read(output.ToArray());
        var entry = Boxes.FindAll<SampleDescriptionBox>(container).Select(s => s.Children.Single()).Single(MetadataTrack.IsMetadata);
        var copiedEntry = Boxes.FindAll<SampleDescriptionBox>(copied).Select(s => s.Children.Single()).Single(MetadataTrack.IsMetadata);
        CollectionAssert.AreEqual(TrackTestSupport.BytesOf(entry), TrackTestSupport.BytesOf(copiedEntry));

        var track = (MetadataTrack)copiedReader.Tracks[MetadataTrackID(copiedReader)].Track;
        CollectionAssert.AreEqual(new[] { DescribedTrackID(copiedReader) }, track.DescribedTrackIDs.ToArray());
        int count = 0;
        for (var sample = copiedReader.ReadSample(MetadataTrackID(copiedReader)); sample != null; sample = copiedReader.ReadSample(MetadataTrackID(copiedReader)))
            CollectionAssert.AreEqual(Samples[count++].Data, sample.Data.ToArray());
        Assert.AreEqual(Samples.Length, count);
    }

    /// <summary>A track of nothing to refer to writes no 'tref'.</summary>
    [TestMethod]
    public void WritesNoReferenceOfATrackOfNone()
    {
        var (container, reader, _) = TrackTestSupport.WriteAndRead(new TextMetadataTrack("application/json"), (builder, id) =>
            builder.ProcessTrackSample(id, Samples[0].Data, Samples[0].Duration));
        Assert.IsNull(Boxes.FindOrNull<TrackReferenceBox>(container));
        Assert.AreEqual(0, ((MetadataTrack)reader.Tracks.Single().Value.Track).DescribedTrackIDs.Count);
    }

    /// <summary>
    /// ffmpeg reads the metadata track as data of its sample entry's type, its packets the samples written, at their times.
    /// Fragmented, of samples that are not empty: ffmpeg fails to read the header of a file whose fragment has a sample of
    /// no bytes, though its 'trun' says it rightly; not fragmented, it reads an empty sample as a packet of none.
    /// </summary>
    [TestMethod]
    [DataRow(URIMetaSampleEntry.TYPE, false)]
    [DataRow(TextMetaDataSampleEntry.TYPE, false)]
    [DataRow(XMLMetaDataSampleEntry.TYPE, false)]
    [DataRow(URIMetaSampleEntry.TYPE, true)]
    public void FfmpegReadsTheSamplesWritten(string kind, bool fragmented)
    {
        string? ffmpeg = SharpMP4.Tests.Conformance.ConformanceCorpus.LocateFfmpeg();
        if (ffmpeg == null)
            Assert.Inconclusive("no ffmpeg; put one on the PATH, or set SHARPMP4_FFMPEG");
        string ffprobe = Path.Combine(Path.GetDirectoryName(ffmpeg)!, Path.GetFileName(ffmpeg).Replace("ffmpeg", "ffprobe"));
        if (!File.Exists(ffprobe))
            Assert.Inconclusive("no ffprobe beside ffmpeg");

        string path = Path.Combine(Path.GetTempPath(), $"sharpmp4-meta-{Guid.NewGuid():N}.mp4");
        try
        {
            var samples = fragmented ? Samples.Where(s => s.Data.Length > 0).ToArray() : Samples;
            File.WriteAllBytes(path, Build(kind, fragmented, quickTime: false, samples));

            string streams = Run(ffprobe, "-v", "error", "-show_entries", "stream=index,codec_type,codec_tag_string", "-of", "csv=p=0", path);
            StringAssert.Contains(streams, $"1,data,{kind}", streams);

            // the packets' sizes and times: an empty sample is a packet of none
            string packets = Run(ffprobe, "-v", "error", "-select_streams", "1", "-show_entries", "packet=pts,duration,size", "-of", "csv=p=0", path);
            long time = 0;
            string expected = string.Concat(samples.Select(s =>
            {
                string line = $"{time},{s.Duration},{s.Data.Length}\n";
                time += s.Duration;
                return line;
            }));
            Assert.AreEqual(expected, packets.Replace("\r\n", "\n"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static string Run(string executable, params string[] args)
    {
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (string arg in args)
            start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();
        process.WaitForExit();
        Assert.AreEqual("", errors.Trim(), Path.GetFileName(executable));
        return output;
    }
}
