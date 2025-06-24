using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Readers
{
    public struct Mp4Sample
    {
        public long IDX { get; set; }
        public long PTS { get; set; }
        public long DTS { get; set; }
        public uint Duration { get; set; }
        public byte[] Data { get; set; }
        public bool IsRandomAccessPoint { get; set; }
        public Mp4Sample(long idx, long pts, long dts, uint duration, byte[] data, bool isRandomAccessPoint = true)
        {
            this.IDX = idx;
            this.PTS = pts;
            this.DTS = dts;
            this.Duration = duration;
            this.Data = data;
            this.IsRandomAccessPoint = isRandomAccessPoint;
        }
    }

    public class FragmentParserContext
    {
        // fmp4
        public IEnumerable<TrackRunBox> Truns { get; set; }
        public MovieExtendsBox Mvex { get; set; }
        public TrackFragmentBaseMediaDecodeTimeBox Tfdt { get; set; }
        public TrackFragmentHeaderBox Tfhd { get; set; }
        public TrackExtendsBox Trex { get; set; }
        public IEnumerable<TrackFragmentBox> Trafs { get; set; }
    }

    public class TrackParserContext
    {
        public long DTS { get; set; }
        public long PTS { get; set; }
        public long IDX { get; set; }

        public IList<byte[]> VideoNals { get; set; } = new List<byte[]>();
        public int NalLengthSize { get; set; } = 4;
        public IList<Mp4Sample> Samples { get; set; } = new List<Mp4Sample>();

        // mp4
        public SampleTableBox Stbl { get; set; }
        public ChunkOffsetBox Stco { get; set; }
        public ChunkLargeOffsetBox Co64 { get; set; }
        public SampleToChunkBox Stsc { get; set; }
        public SampleSizeBox Stsz { get; set; }
        public TimeToSampleBox Stts { get; set; }
        public CompositionOffsetBox Ctts { get; set; }
        public SyncSampleBox Stss { get; set; }

        // fmp4
        public FragmentParserContext Fragment { get; set; }
    }

    public class ContainerParserContext
    {
        public HashSet<uint> VideoTracks { get; set; } = new HashSet<uint>();
        public HashSet<uint> AudioTracks { get; set; } = new HashSet<uint>();

        public FileTypeBox Ftyp { get; set; }
        public MovieBox Moov { get; set; }
        public MovieFragmentBox Moof { get; set; }
        public MediaDataBox Mdat { get; set; }
        public TrackBox[] Track { get; set; }

        public TrackParserContext[] Tracks { get; set; }
    }

    public static class Mp4Reader
    {
        public static ContainerParserContext Parse(Container inputMp4)
        {
            if (inputMp4.Children.Count == 0)
                return null;

            ContainerParserContext ret = new ContainerParserContext();
            ulong size = 0;

            for (int i = 0; i < inputMp4.Children.Count; i++)
            {
                if (inputMp4.Children[i] is FileTypeBox)
                {
                    ret.Ftyp = (FileTypeBox)inputMp4.Children[i];
                }
                else if (inputMp4.Children[i] is MovieBox)
                {
                    ret.Moov = (MovieBox)inputMp4.Children[i];
                    ret.Track = ret.Moov.Children.OfType<TrackBox>().ToArray();
                    ret.Tracks = new TrackParserContext[ret.Track.Length];

                    foreach (var track in ret.Track)
                    {
                        HandlerBox hdlr = track.Children.OfType<MediaBox>().Single().Children.OfType<HandlerBox>().Single();
                        uint trackID = track.Children.OfType<TrackHeaderBox>().First().TrackID;

                        ret.Tracks[(int)(trackID - 1)] = new TrackParserContext();

                        if (hdlr.HandlerType == IsoStream.FromFourCC(HandlerTypes.Sound))
                        {
                            ret.AudioTracks.Add(trackID);
                        }
                        else if (hdlr.HandlerType == IsoStream.FromFourCC(HandlerTypes.Video))
                        {
                            ret.VideoTracks.Add(trackID);

                            VisualSampleEntry visualSample = track
                                .Children.OfType<MediaBox>().Single()
                                .Children.OfType<MediaInformationBox>().Single()
                                .Children.OfType<SampleTableBox>().Single()
                                .Children.OfType<SampleDescriptionBox>().Single()
                                .Children.OfType<VisualSampleEntry>().Single();

                            AVCConfigurationBox avcC = visualSample.Children.OfType<AVCConfigurationBox>().SingleOrDefault();
                            HEVCConfigurationBox hvcC = visualSample.Children.OfType<HEVCConfigurationBox>().SingleOrDefault();
                            VvcConfigurationBox vvcC = visualSample.Children.OfType<VvcConfigurationBox>().SingleOrDefault();

                            if (avcC != null)
                            {
                                ret.Tracks[(int)(trackID - 1)].NalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
                                {
                                    try
                                    {
                                        ret.Tracks[(int)(trackID - 1)].VideoNals.Add(spsBinary);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (SharpISOBMFF.Log.ErrorEnabled) SharpISOBMFF.Log.Error($"{nameof(Mp4Extensions)}: Error (1) reading file, exception: {ex.Message}");
                                        throw;
                                    }
                                }

                                foreach (var ppsBinary in avcC._AVCConfig.PictureParameterSetNALUnit)
                                {
                                    try
                                    {
                                        ret.Tracks[(int)(trackID - 1)].VideoNals.Add(ppsBinary);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (SharpISOBMFF.Log.ErrorEnabled) SharpISOBMFF.Log.Error($"{nameof(Mp4Extensions)}: Error (2) reading file, exception: {ex.Message}");
                                        throw;
                                    }
                                }
                            }
                            else if (hvcC != null)
                            {
                                ret.Tracks[(int)(trackID - 1)].NalLengthSize = hvcC._HEVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var nalus in hvcC._HEVCConfig.NalUnit)
                                {
                                    foreach (var nalu in nalus)
                                    {
                                        try
                                        {
                                            ret.Tracks[(int)(trackID - 1)].VideoNals.Add(nalu);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (SharpISOBMFF.Log.ErrorEnabled) SharpISOBMFF.Log.Error($"{nameof(Mp4Extensions)}: Error (3) reading file, exception: {ex.Message}");
                                            throw;
                                        }
                                    }
                                }
                            }
                            else if (vvcC != null)
                            {
                                ret.Tracks[(int)(trackID - 1)].NalLengthSize = vvcC._VvcConfig._LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var nalus in vvcC._VvcConfig.NalUnit)
                                {
                                    foreach (var nalu in nalus)
                                    {
                                        try
                                        {
                                            ret.Tracks[(int)(trackID - 1)].VideoNals.Add(nalu);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (SharpISOBMFF.Log.ErrorEnabled) SharpISOBMFF.Log.Error($"{nameof(Mp4Extensions)}: Error (4) reading file, exception: {ex.Message}");
                                            throw;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (SharpISOBMFF.Log.ErrorEnabled) SharpISOBMFF.Log.Error($"{nameof(Mp4Extensions)}: Error reading file");
                                return null;
                            }
                        }
                    }
                }
                else if (inputMp4.Children[i] is MovieFragmentBox)
                {
                    ret.Moof = (MovieFragmentBox)inputMp4.Children[i];
                }
                else if (inputMp4.Children[i] is MediaDataBox)
                {
                    var currentMdat = (MediaDataBox)inputMp4.Children[i];

                    if (currentMdat.Size > 8)
                    {
                        ret.Mdat = currentMdat;
                    }
                }

                if (ret.Moov != null && ret.Mdat != null)
                {
                    ret.Mdat.Data.Stream.SeekFromBeginning(ret.Mdat.Data.Position);

                    if (ret.Moof != null)
                    {
                        // fmp4
                        FragmentParserContext fragmentContext = new FragmentParserContext();
                        fragmentContext.Trafs = ret.Moof.Children.OfType<TrackFragmentBox>();

                        foreach (var traf in fragmentContext.Trafs)
                        {
                            fragmentContext.Truns = traf.Children.OfType<TrackRunBox>(); // there can be 1 or multiple trun boxes, depending upon the encoder
                            fragmentContext.Mvex = ret.Moov.Children.OfType<MovieExtendsBox>().SingleOrDefault();

                            foreach (var trun in fragmentContext.Truns)
                            {
                                fragmentContext.Tfhd = traf.Children.OfType<TrackFragmentHeaderBox>().Single();
                                uint trackID = fragmentContext.Tfhd.TrackID;

                                var trackContext = ret.Tracks[trackID - 1];
                                trackContext.Fragment = fragmentContext;

                                fragmentContext.Tfdt = traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().SingleOrDefault();
                                fragmentContext.Trex = fragmentContext.Mvex?.Children.OfType<TrackExtendsBox>().SingleOrDefault(x => x.TrackID == trackID);

                                bool isVideo = ret.VideoTracks.Contains(trackID);
                                if (fragmentContext.Tfdt != null)
                                {
                                    trackContext.DTS = (long)fragmentContext.Tfdt.BaseMediaDecodeTime;
                                }

                                uint firstSampleFlags = trun.FirstSampleFlags;
                                if ((trun.Flags & 0x4) != 0x4)
                                    firstSampleFlags = fragmentContext.Tfhd.DefaultSampleFlags;

                                for (int k = 1; k <= trun._TrunEntry.Length; k++)
                                {
                                    var entry = trun._TrunEntry[k - 1];

                                    uint sampleDuration = fragmentContext.Tfhd.DefaultSampleDuration;
                                    if ((entry.Flags & 0x100) == 0x100)
                                        sampleDuration = entry.SampleDuration;
                                    else if ((fragmentContext.Tfhd.Flags & 0x8) == 0x8)
                                        sampleDuration = fragmentContext.Tfhd.DefaultSampleDuration;
                                    else if (fragmentContext.Trex != null)
                                        sampleDuration = fragmentContext.Trex.DefaultSampleDuration;
                                    else
                                        throw new Exception("Cannot get sample duration");

                                    uint sampleSize = fragmentContext.Tfhd.DefaultSampleSize;
                                    if ((entry.Flags & 0x200) == 0x200)
                                        sampleSize = entry.SampleSize;
                                    else if ((fragmentContext.Tfhd.Flags & 0x10) == 0x10)
                                        sampleSize = fragmentContext.Tfhd.DefaultSampleSize;
                                    else if (fragmentContext.Trex != null)
                                        sampleSize = fragmentContext.Trex.DefaultSampleSize;
                                    else
                                        throw new Exception("Cannot get sample size");

                                    uint sampleFlags = fragmentContext.Tfhd.DefaultSampleFlags;
                                    if (k == 1)
                                        sampleFlags = firstSampleFlags;
                                    else if ((entry.Flags & 0x400) == 0x400)
                                        sampleFlags = entry.SampleFlags;

                                    // CTS
                                    int sampleCompositionTime = 0;
                                    if ((entry.Flags & 0x800) == 0x800)
                                    {
                                        if (entry.Version == 0)
                                            sampleCompositionTime = (int)entry.SampleCompositionTimeOffset;
                                        else
                                            sampleCompositionTime = entry.SampleCompositionTimeOffset0;
                                    }

                                    trackContext.PTS = trackContext.DTS + sampleCompositionTime;

                                    size += ret.Mdat.Data.Stream.ReadUInt8Array(size, (ulong)ret.Mdat.Data.Length, sampleSize, out byte[] sampleData);
                                    trackContext.Samples.Add(new Mp4Sample(trackContext.IDX++, trackContext.PTS, trackContext.DTS, sampleDuration, sampleData));

                                    trackContext.DTS += sampleDuration;
                                }
                            }
                        }

                        // start looking for next moof/mdat pair
                        ret.Moof = null;
                        ret.Mdat = null;
                    }
                    else
                    {
                        // mp4
                        foreach (var track in ret.Track)
                        {
                            uint trackID = track.Children.OfType<TrackHeaderBox>().Single().TrackID;
                            var trackContext = ret.Tracks[trackID - 1];

                            trackContext.Stbl = track
                                .Children.OfType<MediaBox>().Single()
                                .Children.OfType<MediaInformationBox>().Single()
                                .Children.OfType<SampleTableBox>().Single();
                            trackContext.Stco = trackContext.Stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                            trackContext.Co64 = trackContext.Stbl.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();
                            trackContext.Stsc = trackContext.Stbl.Children.OfType<SampleToChunkBox>().Single();
                            trackContext.Stsz = trackContext.Stbl.Children.OfType<SampleSizeBox>().Single();
                            trackContext.Stts = trackContext.Stbl.Children.OfType<TimeToSampleBox>().Single();
                            trackContext.Ctts = trackContext.Stbl.Children.OfType<CompositionOffsetBox>().SingleOrDefault();
                            trackContext.Stss = trackContext.Stbl.Children.OfType<SyncSampleBox>().SingleOrDefault(); // optional
                            uint[] sampleSizes = trackContext.Stsz.SampleSize > 0 ? Enumerable.Repeat(trackContext.Stsz.SampleSize, (int)trackContext.Stsz.SampleCount).ToArray() : trackContext.Stsz.EntrySize;
                            ulong[] chunkOffsets = trackContext.Stco != null ? trackContext.Stco.ChunkOffset.Select(x => (ulong)x).ToArray() : trackContext.Co64.ChunkOffset;

                            bool isVideo = ret.VideoTracks.Contains(trackID);

                            // https://developer.apple.com/documentation/quicktime-file-format/sample-to-chunk_atom/sample-to-chunk_table
                            int stscIndex = 0;
                            uint stscNextRun = 0;
                            uint stscSamplesPerChunk = 0;

                            int sttsIndex = 0;
                            uint sttsNextRun = 0;
                            uint sttsSampleDelta = 0;

                            int cttsIndex = 0;
                            uint cttsNextRun = 0;
                            int cttsSampleDelta = 0;

                            int sampleIndex = 0;
                            for (int k = 1; k <= chunkOffsets.Length; k++)
                            {
                                if (k >= stscNextRun)
                                {
                                    stscSamplesPerChunk = trackContext.Stsc.SamplesPerChunk[stscIndex];
                                    stscIndex += 1;
                                    stscNextRun = (stscIndex < trackContext.Stsc.FirstChunk.Length) ? trackContext.Stsc.FirstChunk[stscIndex] : (uint)(chunkOffsets.Length + 1);
                                }

                                long chunkOffset = (long)chunkOffsets[k - 1];

                                // seek to the chunk offset
                                ret.Mdat.Data.Stream.SeekFromBeginning(chunkOffset);

                                // read samples in this chunk                            
                                for (int l = 1; l <= stscSamplesPerChunk; l++)
                                {
                                    if (sampleIndex >= sttsNextRun)
                                    {
                                        sttsSampleDelta = trackContext.Stts.SampleDelta[sttsIndex];
                                        sttsNextRun += (sttsIndex < trackContext.Stts.SampleCount.Length) ? trackContext.Stts.SampleCount[sttsIndex] : (uint)(chunkOffsets.Length + 1);
                                        sttsIndex += 1;
                                    }

                                    if (trackContext.Ctts != null && sampleIndex >= cttsNextRun)
                                    {
                                        cttsSampleDelta = trackContext.Ctts.Version == 0 ? (int)trackContext.Ctts.SampleOffset[cttsIndex] : trackContext.Ctts.SampleOffset0[cttsIndex];
                                        cttsNextRun += (cttsIndex < trackContext.Ctts.SampleCount.Length) ? trackContext.Ctts.SampleCount[cttsIndex] : (uint)(chunkOffsets.Length + 1);
                                        cttsIndex += 1;
                                    }

                                    bool isRandomAccessPoint = true;
                                    if (trackContext.Stss != null)
                                    {
                                        isRandomAccessPoint = trackContext.Stss.SampleNumber.Contains((uint)sampleIndex + 1);
                                    }

                                    uint sampleSize = sampleSizes[sampleIndex++];
                                    trackContext.PTS = trackContext.DTS + cttsSampleDelta;

                                    size += ret.Mdat.Data.Stream.ReadUInt8Array(size, (ulong)ret.Mdat.Data.Length, sampleSize, out byte[] sampleData);
                                    trackContext.Samples.Add(new Mp4Sample(trackContext.IDX++, trackContext.PTS, trackContext.DTS, sttsSampleDelta, sampleData, isRandomAccessPoint));

                                    trackContext.DTS += sttsSampleDelta;
                                }
                            }
                        }
                    }

                    ret.Mdat = null;
                }
            }

            return ret;
        }

        public static List<byte[]> ReadAU(int nalLengthSize, byte[] sample)
        {
            ulong size = 0;
            long offsetInBytes = 0;

            if (SharpISOBMFF.Log.DebugEnabled) SharpISOBMFF.Log.Debug($"{nameof(Mp4Extensions)}: AU begin {sample.Length}");

            List<byte[]> naluList = new List<byte[]>();

            using (var markerStream = new IsoStream(new MemoryStream(sample)))
            {
                do
                {
                    uint nalUnitLength = 0;
                    size += markerStream.ReadVariableLengthSize((uint)nalLengthSize, out nalUnitLength);
                    offsetInBytes += nalLengthSize;

                    if (nalUnitLength > (sample.Length - offsetInBytes))
                    {
                        if (SharpISOBMFF.Log.ErrorEnabled) SharpISOBMFF.Log.Error($"{nameof(Mp4Extensions)}: Invalid NALU size: {nalUnitLength}");
                        nalUnitLength = (uint)(sample.Length - offsetInBytes);
                        size += nalUnitLength;
                        offsetInBytes += nalUnitLength;
                        break;
                    }

                    size += markerStream.ReadUInt8Array(size, (ulong)sample.Length, nalUnitLength, out byte[] sampleData);
                    offsetInBytes += sampleData.Length;
                    naluList.Add(sampleData);
                } while (offsetInBytes < sample.Length);

                if (offsetInBytes != sample.Length)
                    throw new Exception("Mismatch!");

                if (SharpISOBMFF.Log.DebugEnabled) SharpISOBMFF.Log.Debug($"{nameof(Mp4Extensions)}: AU end");

                return naluList;
            }
        }
    }
}
