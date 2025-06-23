using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Readers
{
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
        public IList<byte[]> VideoNals { get; set; } = new List<byte[]>();
        public int NalLengthSize { get; set; } = 4;
        public IList<byte[]> Samples { get; set; } = new List<byte[]>();

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
        public MetaBox Meta { get; set; }
        public TrackBox[] Track { get; set; }

        public TrackParserContext[] Tracks { get; set; }
    }

    public static class Mp4Reader
    {
        public static ContainerParserContext Parse(Container inputMp4)
        {
            if (inputMp4.Children.Count == 0)
                return null;

            long[] dts = null;
            long[] pts = null;

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
                    ret.Ftyp = inputMp4.Children.OfType<FileTypeBox>().Single();
                    ret.Track = ret.Moov.Children.OfType<TrackBox>().ToArray();

                    ret.Tracks = new TrackParserContext[ret.Track.Length];
                    dts = new long[ret.Track.Length];
                    pts = new long[ret.Track.Length];

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
                else if (inputMp4.Children[i] is MetaBox)
                {
                    ret.Meta = (MetaBox)inputMp4.Children[i];
                }

                if (ret.Moov != null && ret.Mdat != null)
                {
                    ret.Mdat.Data.Stream.SeekFromBeginning(ret.Mdat.Data.Position);

                    if (ret.Moof != null)
                    {
                        FragmentParserContext fragmentContext = new FragmentParserContext();

                        // fmp4
                        fragmentContext.Trafs = ret.Moof.Children.OfType<TrackFragmentBox>();
                        foreach (var traf in fragmentContext.Trafs)
                        {
                            fragmentContext.Truns = traf.Children.OfType<TrackRunBox>(); // there can be 1 or multiple trun boxes, depending upon the encoder
                            fragmentContext.Mvex = ret.Moov.Children.OfType<MovieExtendsBox>().SingleOrDefault();

                            foreach (var trun in fragmentContext.Truns)
                            {
                                fragmentContext.Tfhd = traf.Children.OfType<TrackFragmentHeaderBox>().Single();
                                uint trackID = fragmentContext.Tfhd.TrackID;
                                ret.Tracks[trackID - 1].Fragment = fragmentContext;

                                ret.Tracks[trackID - 1].Fragment.Tfdt = traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().SingleOrDefault();
                                ret.Tracks[trackID - 1].Fragment.Trex = ret.Tracks[trackID - 1].Fragment.Mvex?.Children.OfType<TrackExtendsBox>().SingleOrDefault(x => x.TrackID == trackID);

                                bool isVideo = ret.VideoTracks.Contains(trackID);
                                if (ret.Tracks[trackID - 1].Fragment.Tfdt != null)
                                {
                                    dts[trackID - 1] = (long)ret.Tracks[trackID - 1].Fragment.Tfdt.BaseMediaDecodeTime;
                                }

                                uint firstSampleFlags = trun.FirstSampleFlags;
                                if ((trun.Flags & 0x4) != 0x4)
                                    firstSampleFlags = ret.Tracks[trackID - 1].Fragment.Tfhd.DefaultSampleFlags;

                                for (int k = 1; k <= trun._TrunEntry.Length; k++)
                                {
                                    var entry = trun._TrunEntry[k - 1];

                                    uint sampleDuration = ret.Tracks[trackID - 1].Fragment.Tfhd.DefaultSampleDuration;
                                    if ((entry.Flags & 0x100) == 0x100)
                                        sampleDuration = entry.SampleDuration;
                                    else if ((ret.Tracks[trackID - 1].Fragment.Tfhd.Flags & 0x8) == 0x8)
                                        sampleDuration = ret.Tracks[trackID - 1].Fragment.Tfhd.DefaultSampleDuration;
                                    else if (ret.Tracks[trackID - 1].Fragment.Trex != null)
                                        sampleDuration = ret.Tracks[trackID - 1].Fragment.Trex.DefaultSampleDuration;
                                    else
                                        throw new Exception("Cannot get sample duration");

                                    uint sampleSize = ret.Tracks[trackID - 1].Fragment.Tfhd.DefaultSampleSize;
                                    if ((entry.Flags & 0x200) == 0x200)
                                        sampleSize = entry.SampleSize;
                                    else if ((ret.Tracks[trackID - 1].Fragment.Tfhd.Flags & 0x10) == 0x10)
                                        sampleSize = ret.Tracks[trackID - 1].Fragment.Tfhd.DefaultSampleSize;
                                    else if (ret.Tracks[trackID - 1].Fragment.Trex != null)
                                        sampleSize = ret.Tracks[trackID - 1].Fragment.Trex.DefaultSampleSize;
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

                                    pts[trackID - 1] = dts[trackID - 1] + sampleCompositionTime;

                                    size += ret.Mdat.Data.Stream.ReadUInt8Array(size, (ulong)ret.Mdat.Data.Length, sampleSize, out byte[] sampleData);
                                    ret.Tracks[(int)(trackID - 1)].Samples.Add(sampleData);

                                    dts[trackID - 1] += sampleDuration;
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

                            ret.Tracks[trackID - 1].Stbl = track
                                .Children.OfType<MediaBox>().Single()
                                .Children.OfType<MediaInformationBox>().Single()
                                .Children.OfType<SampleTableBox>().Single();
                            ret.Tracks[trackID - 1].Stco = ret.Tracks[trackID - 1].Stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                            ret.Tracks[trackID - 1].Co64 = ret.Tracks[trackID - 1].Stbl.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();
                            ret.Tracks[trackID - 1].Stsc = ret.Tracks[trackID - 1].Stbl.Children.OfType<SampleToChunkBox>().Single();
                            ret.Tracks[trackID - 1].Stsz = ret.Tracks[trackID - 1].Stbl.Children.OfType<SampleSizeBox>().Single();
                            ret.Tracks[trackID - 1].Stts = ret.Tracks[trackID - 1].Stbl.Children.OfType<TimeToSampleBox>().Single();
                            ret.Tracks[trackID - 1].Ctts = ret.Tracks[trackID - 1].Stbl.Children.OfType<CompositionOffsetBox>().SingleOrDefault();
                            ret.Tracks[trackID - 1].Stss = ret.Tracks[trackID - 1].Stbl.Children.OfType<SyncSampleBox>().SingleOrDefault(); // optional
                            uint[] sampleSizes = ret.Tracks[trackID - 1].Stsz.SampleSize > 0 ? Enumerable.Repeat(ret.Tracks[trackID - 1].Stsz.SampleSize, (int)ret.Tracks[trackID - 1].Stsz.SampleCount).ToArray() : ret.Tracks[trackID - 1].Stsz.EntrySize;
                            ulong[] chunkOffsets = ret.Tracks[trackID - 1].Stco != null ? ret.Tracks[trackID - 1].Stco.ChunkOffset.Select(x => (ulong)x).ToArray() : ret.Tracks[trackID - 1].Co64.ChunkOffset;

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
                                    stscSamplesPerChunk = ret.Tracks[trackID - 1].Stsc.SamplesPerChunk[stscIndex];
                                    stscIndex += 1;
                                    stscNextRun = (stscIndex < ret.Tracks[trackID - 1].Stsc.FirstChunk.Length) ? ret.Tracks[trackID - 1].Stsc.FirstChunk[stscIndex] : (uint)(chunkOffsets.Length + 1);
                                }

                                long chunkOffset = (long)chunkOffsets[k - 1];

                                // seek to the chunk offset
                                ret.Mdat.Data.Stream.SeekFromBeginning(chunkOffset);

                                // read samples in this chunk                            
                                for (int l = 1; l <= stscSamplesPerChunk; l++)
                                {
                                    if (sampleIndex >= sttsNextRun)
                                    {
                                        sttsSampleDelta = ret.Tracks[trackID - 1].Stts.SampleDelta[sttsIndex];
                                        sttsNextRun += (sttsIndex < ret.Tracks[trackID - 1].Stts.SampleCount.Length) ? ret.Tracks[trackID - 1].Stts.SampleCount[sttsIndex] : (uint)(chunkOffsets.Length + 1);
                                        sttsIndex += 1;
                                    }

                                    if (ret.Tracks[trackID - 1].Ctts != null && sampleIndex >= cttsNextRun)
                                    {
                                        cttsSampleDelta = ret.Tracks[trackID - 1].Ctts.Version == 0 ? (int)ret.Tracks[trackID - 1].Ctts.SampleOffset[cttsIndex] : ret.Tracks[trackID - 1].Ctts.SampleOffset0[cttsIndex];
                                        cttsNextRun += (cttsIndex < ret.Tracks[trackID - 1].Ctts.SampleCount.Length) ? ret.Tracks[trackID - 1].Ctts.SampleCount[cttsIndex] : (uint)(chunkOffsets.Length + 1);
                                        cttsIndex += 1;
                                    }

                                    bool isRandomAccessPoint = true;
                                    if (ret.Tracks[trackID - 1].Stss != null)
                                    {
                                        isRandomAccessPoint = ret.Tracks[trackID - 1].Stss.SampleNumber.Contains((uint)sampleIndex + 1);
                                    }

                                    uint sampleSize = sampleSizes[sampleIndex++];
                                    pts[trackID - 1] = dts[trackID - 1] + cttsSampleDelta;

                                    size += ret.Mdat.Data.Stream.ReadUInt8Array(size, (ulong)ret.Mdat.Data.Length, sampleSize, out byte[] sampleData);
                                    ret.Tracks[(int)(trackID - 1)].Samples.Add(sampleData);

                                    dts[trackID - 1] += sttsSampleDelta;
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
