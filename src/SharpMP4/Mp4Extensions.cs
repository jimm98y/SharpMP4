using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpH264;
using SharpH265;
using SharpH266;
using SharpH26X;
using SharpISOBMFF;

namespace SharpMP4
{
    public static class Mp4Extensions
    {
        public static IEnumerable<TrackBox> FindVideoTrack(this Container mp4)
        {
            return mp4.GetMovieBox().GetTracks()
               .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Video));
        }

        public static IEnumerable<TrackBox> FindAudioTrack(this Container mp4)
        {
            return mp4.GetMovieBox().GetTracks()
                .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Sound));
        }

        public static IEnumerable<TrackBox> FindHintTrack(this Container mp4)
        {
            return mp4.GetMovieBox().GetTracks()
                .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Hint));
        }

        public static MovieBox GetMovieBox(this Container mp4)
        {
            return mp4.Children.OfType<MovieBox>().Single();
        }

        public static IEnumerable<TrackBox> GetTracks(this MovieBox moov)
        {
            return moov.Children.OfType<TrackBox>();
        }

        public static AudioSampleEntry GetAudioSampleEntryBox(this TrackBox track)
        {
            return track
                .Children.OfType<MediaBox>().Single()
                .Children.OfType<MediaInformationBox>().Single()
                .Children.OfType<SampleTableBox>().Single()
                .Children.OfType<SampleDescriptionBox>().Single()
                .Children.OfType<AudioSampleEntry>().Single();
        }


        public class TrackParserContext
        {
            public IList<byte[]> VideoNALUs { get; set; } = new List<byte[]>();
            public int NalLengthSize { get; set; } = 4;
            public IList<byte[]> Samples { get; set; } = new List<byte[]>();
        }

        public class MdatParserContext
        {
            public TrackParserContext[] Tracks { get; set; }
            public HashSet<uint> VideoTracks { get; set; } = new HashSet<uint>();
            public HashSet<uint> AudioTracks { get; set; } = new HashSet<uint>();
        }

        public static MdatParserContext Parse(this Container inputMp4)
        {
            if (inputMp4.Children.Count == 0)
                return null;

            IItuContext context = null;

            long[] dts = null;
            long[] pts = null; 

            MdatParserContext ret = new MdatParserContext();
            FileTypeBox ftyp = null;
            MovieBox moov = null;
            TrackBox[] tracks = null;
            MovieFragmentBox moof = null;
            MediaDataBox mdat = null;
            MetaBox meta = null;
            ulong size = 0;
            
            for (int i = 0; i < inputMp4.Children.Count; i++)
            {
                if (inputMp4.Children[i] is FileTypeBox)
                {
                    ftyp = (FileTypeBox)inputMp4.Children[i];
                }
                else if (inputMp4.Children[i] is MovieBox)
                {
                    moov = (MovieBox)inputMp4.Children[i];
                    ftyp = inputMp4.Children.OfType<FileTypeBox>().Single();
                    tracks = moov.Children.OfType<TrackBox>().ToArray();

                    ret.Tracks = new TrackParserContext[tracks.Length];
                    dts = new long[tracks.Length];
                    pts = new long[tracks.Length];

                    foreach (var track in tracks)
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
                                context = new H264Context();
                                ret.Tracks[(int)(trackID - 1)].NalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
                                {
                                    try
                                    {
                                        //ParseNALU(context, spsBinary);
                                        ret.Tracks[(int)(trackID - 1)].VideoNALUs.Add(spsBinary);
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
                                        //ParseNALU(context, ppsBinary);
                                        ret.Tracks[(int)(trackID - 1)].VideoNALUs.Add(ppsBinary);
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
                                context = new H265Context();
                                ret.Tracks[(int)(trackID - 1)].NalLengthSize = hvcC._HEVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var nalus in hvcC._HEVCConfig.NalUnit)
                                {
                                    foreach (var nalu in nalus)
                                    {
                                        try
                                        {
                                            //ParseNALU(context, nalu);
                                            ret.Tracks[(int)(trackID - 1)].VideoNALUs.Add(nalu);
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
                                context = new H266Context();
                                ret.Tracks[(int)(trackID - 1)].NalLengthSize = vvcC._VvcConfig._LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var nalus in vvcC._VvcConfig.NalUnit)
                                {
                                    foreach (var nalu in nalus)
                                    {
                                        try
                                        {
                                            //ParseNALU(context, nalu);
                                            ret.Tracks[(int)(trackID - 1)].VideoNALUs.Add(nalu);
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
                    moof = (MovieFragmentBox)inputMp4.Children[i];
                }
                else if (inputMp4.Children[i] is MediaDataBox)
                {
                    var currentMdat = (MediaDataBox)inputMp4.Children[i];

                    if (currentMdat.Size > 8)
                    {
                        mdat = currentMdat;
                    }
                }
                else if (inputMp4.Children[i] is MetaBox)
                {
                    meta = (MetaBox)inputMp4.Children[i];
                }

                if (moov != null && mdat != null)
                {
                    mdat.Data.Stream.SeekFromBeginning(mdat.Data.Position);

                    if (moof != null)
                    {
                        // fmp4
                        IOrderedEnumerable<TrackRunBox> plan = moof.Children.OfType<TrackFragmentBox>().SelectMany(x => x.Children.OfType<TrackRunBox>()).OrderBy(x => x.DataOffset);
                        MovieExtendsBox mvex = moov.Children.OfType<MovieExtendsBox>().SingleOrDefault();

                        foreach (var trun in plan)
                        {
                            TrackFragmentBox traf = trun.GetParent() as TrackFragmentBox;
                            TrackFragmentBaseMediaDecodeTimeBox tfdt = traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().SingleOrDefault();
                            TrackFragmentHeaderBox tfhd = traf.Children.OfType<TrackFragmentHeaderBox>().Single();
                            uint trackID = tfhd.TrackID;

                            TrackExtendsBox trex = mvex?.Children.OfType<TrackExtendsBox>().SingleOrDefault(x => x.TrackID == trackID);

                            bool isVideo = ret.VideoTracks.Contains(trackID);
                            if (tfdt != null)
                            {
                                dts[trackID - 1] = (long)tfdt.BaseMediaDecodeTime;
                            }

                            uint firstSampleFlags = trun.FirstSampleFlags;
                            if ((trun.Flags & 0x4) != 0x4)
                                firstSampleFlags = tfhd.DefaultSampleFlags;

                            for (int k = 1; k <= trun._TrunEntry.Length; k++)
                            {
                                var entry = trun._TrunEntry[k - 1];

                                uint sampleDuration = tfhd.DefaultSampleDuration;
                                if ((entry.Flags & 0x100) == 0x100)
                                    sampleDuration = entry.SampleDuration;
                                else if ((tfhd.Flags & 0x8) == 0x8)
                                    sampleDuration = tfhd.DefaultSampleDuration;
                                else if (trex != null)
                                    sampleDuration = trex.DefaultSampleDuration;
                                else
                                    throw new Exception("Cannot get sample duration");

                                uint sampleSize = tfhd.DefaultSampleSize;
                                if ((entry.Flags & 0x200) == 0x200)
                                    sampleSize = entry.SampleSize;
                                else if ((tfhd.Flags & 0x10) == 0x10)
                                    sampleSize = tfhd.DefaultSampleSize;
                                else if (trex != null)
                                    sampleSize = trex.DefaultSampleSize;
                                else
                                    throw new Exception("Cannot get sample size");

                                uint sampleFlags = tfhd.DefaultSampleFlags;
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

                                size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, sampleSize, out byte[] sampleData);
                                ret.Tracks[(int)(trackID - 1)].Samples.Add(sampleData);

                                dts[trackID - 1] += sampleDuration;
                            }
                        }

                        // start looking for next moof/mdat pair
                        moof = null;
                        mdat = null;
                    }
                    else
                    {
                        // mp4
                        foreach (var track in tracks)
                        {
                            uint trackID = track.Children.OfType<TrackHeaderBox>().Single().TrackID;

                            SampleTableBox stbl = track
                                .Children.OfType<MediaBox>().Single()
                                .Children.OfType<MediaInformationBox>().Single()
                                .Children.OfType<SampleTableBox>().Single();
                            ChunkOffsetBox stco = stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                            ChunkLargeOffsetBox co64 = stbl.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();
                            SampleToChunkBox stsc = stbl.Children.OfType<SampleToChunkBox>().Single();
                            SampleSizeBox stsz = stbl.Children.OfType<SampleSizeBox>().Single();
                            TimeToSampleBox stts = stbl.Children.OfType<TimeToSampleBox>().Single();
                            CompositionOffsetBox ctts = stbl.Children.OfType<CompositionOffsetBox>().SingleOrDefault();
                            uint[] sampleSizes = stsz.SampleSize > 0 ? Enumerable.Repeat(stsz.SampleSize, (int)stsz.SampleCount).ToArray() : stsz.EntrySize;
                            ulong[] chunkOffsets = stco != null ? stco.ChunkOffset.Select(x => (ulong)x).ToArray() : co64.ChunkOffset;

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

                            int sample_idx = 0;
                            for (int k = 1; k <= chunkOffsets.Length; k++)
                            {
                                if (k >= stscNextRun)
                                {
                                    stscSamplesPerChunk = stsc.SamplesPerChunk[stscIndex];
                                    stscIndex += 1;
                                    stscNextRun = (stscIndex < stsc.FirstChunk.Length) ? stsc.FirstChunk[stscIndex] : (uint)(chunkOffsets.Length + 1);
                                }

                                long chunkOffset = (long)chunkOffsets[k - 1];

                                // seek to the chunk offset
                                mdat.Data.Stream.SeekFromBeginning(chunkOffset);

                                // read samples in this chunk                            
                                for (int l = 1; l <= stscSamplesPerChunk; l++)
                                {
                                    if (sample_idx >= sttsNextRun)
                                    {
                                        sttsSampleDelta = stts.SampleDelta[sttsIndex];
                                        sttsNextRun += (sttsIndex < stts.SampleCount.Length) ? stts.SampleCount[sttsIndex] : (uint)(chunkOffsets.Length + 1);
                                        sttsIndex += 1;
                                    }

                                    if (ctts != null && sample_idx >= cttsNextRun)
                                    {
                                        cttsSampleDelta = ctts.Version == 0 ? (int)ctts.SampleOffset[cttsIndex] : ctts.SampleOffset0[cttsIndex];
                                        cttsNextRun += (cttsIndex < ctts.SampleCount.Length) ? ctts.SampleCount[cttsIndex] : (uint)(chunkOffsets.Length + 1);
                                        cttsIndex += 1;
                                    }

                                    uint sampleSize = sampleSizes[sample_idx++];
                                    pts[trackID - 1] = dts[trackID - 1] + cttsSampleDelta;

                                    size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, sampleSize, out byte[] sampleData);
                                    ret.Tracks[(int)(trackID - 1)].Samples.Add(sampleData);

                                    dts[trackID - 1] += sttsSampleDelta;
                                }
                            }
                        }
                    }

                    mdat = null;
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
