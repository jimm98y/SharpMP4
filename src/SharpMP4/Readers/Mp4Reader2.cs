using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Readers
{
    public class Mp4Sample2
    {
        public long PTS { get; set; }
        public long DTS { get; set; }
        public uint Duration { get; set; }
        public byte[] Data { get; set; }
        public bool IsRandomAccessPoint { get; set; }
        public Mp4Sample2(long pts, long dts, uint duration, byte[] data, bool isRandomAccessPoint = true)
        {
            this.PTS = pts;
            this.DTS = dts;
            this.Duration = duration;
            this.Data = data;
            this.IsRandomAccessPoint = isRandomAccessPoint;
        }
    }

    public class Mp4Reader2
    {
        public static ContainerContext Parse(Container container, ContainerContext context = null)
        {
            if (container.Children.Count == 0)
                return null;

            if (context == null)
            {
                context = new ContainerContext();
            }
            context.Container = container;

            context.Ftyp = (FileTypeBox)container.Children[0];
            for (int i = 1; i < container.Children.Count; i++)
            {
                if (container.Children[i] is MovieBox)
                {
                    context.Moov = (MovieBox)container.Children[i];
                    context.Track = context.Moov.Children.OfType<TrackBox>().ToArray();
                    context.Tracks = new TrackContext[context.Track.Length];

                    foreach (var track in context.Track)
                    {
                        HandlerBox hdlr = track.Children.OfType<MediaBox>().Single().Children.OfType<HandlerBox>().Single();
                        uint trackID = track.Children.OfType<TrackHeaderBox>().First().TrackID;

                        var trackContext = new TrackContext();
                        context.Tracks[(int)(trackID - 1)] = trackContext;

                        if (hdlr.HandlerType == IsoStream.FromFourCC(HandlerTypes.Sound))
                        {
                            context.Tracks[(int)(trackID - 1)].TrackType = TrackTypes.Audio;
                        }
                        else if (hdlr.HandlerType == IsoStream.FromFourCC(HandlerTypes.Video))
                        {
                            context.Tracks[(int)(trackID - 1)].TrackType = TrackTypes.Video;

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
                                context.Tracks[(int)(trackID - 1)].NalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
                                {
                                    try
                                    {
                                        context.Tracks[(int)(trackID - 1)].VideoNals.Add(spsBinary);
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
                                        context.Tracks[(int)(trackID - 1)].VideoNals.Add(ppsBinary);
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
                                context.Tracks[(int)(trackID - 1)].NalLengthSize = hvcC._HEVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var nalus in hvcC._HEVCConfig.NalUnit)
                                {
                                    foreach (var nalu in nalus)
                                    {
                                        try
                                        {
                                            context.Tracks[(int)(trackID - 1)].VideoNals.Add(nalu);
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
                                context.Tracks[(int)(trackID - 1)].NalLengthSize = vvcC._VvcConfig._LengthSizeMinusOne + 1; // usually 4 bytes

                                foreach (var nalus in vvcC._VvcConfig.NalUnit)
                                {
                                    foreach (var nalu in nalus)
                                    {
                                        try
                                        {
                                            context.Tracks[(int)(trackID - 1)].VideoNals.Add(nalu);
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
                        trackContext.SizesList = trackContext.Stsz.SampleSize > 0 ? Enumerable.Repeat(trackContext.Stsz.SampleSize, (int)trackContext.Stsz.SampleCount).ToArray() : trackContext.Stsz.EntrySize;
                        trackContext.ChunkAddressList = trackContext.Stco != null ? trackContext.Stco.ChunkOffset.Select(x => (ulong)x).ToArray() : trackContext.Co64.ChunkOffset;

                        // build FramesInChunkList
                        trackContext.FramesInChunkList = new uint[trackContext.ChunkAddressList.Length];

                        int stscIndex = 0;
                        uint stscNextRun = 0;
                        uint stscSamplesPerChunk = 0;

                        int chunkIndex;
                        for (chunkIndex = 1; chunkIndex <= trackContext.ChunkAddressList.Length; chunkIndex++)
                        {
                            if (chunkIndex >= stscNextRun)
                            {
                                stscSamplesPerChunk = trackContext.Stsc.SamplesPerChunk[stscIndex];
                                stscIndex += 1;
                                stscNextRun = (stscIndex < trackContext.Stsc.FirstChunk.Length) ? trackContext.Stsc.FirstChunk[stscIndex] : uint.MaxValue;
                            }

                            trackContext.FramesInChunkList[chunkIndex - 1] = stscSamplesPerChunk;
                        }
                    }
                }
                else if (container.Children[i] is MediaDataBox)
                {
                    var currentMdat = (MediaDataBox)container.Children[i];

                    if (currentMdat.Size > 8)
                    {
                        context.Mdat = currentMdat;
                    }
                }
            }

            return context;
        }

        public static Mp4Sample2 ReadSample(ContainerContext context, int trackID)
        {
            ulong size = 0;
            if (context.Moov == null || context.Mdat == null)
                throw new InvalidOperationException();

            var trackContext = context.Tracks[trackID - 1];

            int sttsIndex = 0;
            uint sttsNextRun = 0;
            uint sttsSampleDelta = 0;

            int cttsIndex = 0;
            uint cttsNextRun = 0;
            int cttsSampleDelta = 0;

            uint sampleIndex = trackContext.SampleIndex;
            if (trackContext.SizesList.Length <= sampleIndex)
                return null;

            int nextChunkIndex = 0;
            long totalFrames = 0;
            do
            {
                totalFrames = totalFrames + trackContext.FramesInChunkList[nextChunkIndex];
                nextChunkIndex++;
            } 
            while (totalFrames <= sampleIndex && nextChunkIndex < trackContext.FramesInChunkList.Length);

            int chunkIndex = nextChunkIndex - 1;
            if (chunkIndex >= trackContext.ChunkAddressList.Length)
                return null;

            long numFramesInChunk = trackContext.FramesInChunkList[chunkIndex];
            long firstFrameInChunk = totalFrames - numFramesInChunk;
            ulong startAddr = trackContext.ChunkAddressList[chunkIndex];
            
            for (int k = 0; k < numFramesInChunk; k++)
            {
                if (firstFrameInChunk + k == sampleIndex)
                {
                    break;
                }

                startAddr += trackContext.SizesList[firstFrameInChunk + k];
            }

            long dts = 0;
            while (sampleIndex > sttsNextRun)
            {
                sttsSampleDelta = trackContext.Stts.SampleDelta[sttsIndex];
                uint sampleCount = 0;
                if (sttsIndex < trackContext.Stts.SampleCount.Length)
                    sampleCount = trackContext.Stts.SampleCount[sttsIndex];
                sttsIndex += 1;

                uint sttsPreviousRun = sttsNextRun;
                sttsNextRun += sampleCount;

                if (sampleIndex >= sttsNextRun)
                {
                    dts += sampleCount * sttsSampleDelta;
                }
                else
                {
                    dts += (sampleIndex - sttsPreviousRun) * sttsSampleDelta;
                }
            }

            if (trackContext.Ctts != null)
            {
                while (sampleIndex >= cttsNextRun)
                {
                    cttsSampleDelta = trackContext.Ctts.Version == 0 ? (int)trackContext.Ctts.SampleOffset[cttsIndex] : trackContext.Ctts.SampleOffset0[cttsIndex];
                    uint sampleCount = 0;
                    if (cttsIndex < trackContext.Ctts.SampleCount.Length) 
                        sampleCount = trackContext.Ctts.SampleCount[cttsIndex];
                    cttsIndex += 1;

                    var cttsPreviousRun = cttsNextRun;
                    cttsNextRun += sampleCount;

                    if (sampleIndex >= cttsNextRun)
                    {
                        dts += sampleCount * cttsSampleDelta;
                    }
                    else
                    {
                        dts += (sampleIndex - cttsPreviousRun) * cttsSampleDelta;
                    }
                }
            }

            bool isRandomAccessPoint = true;
            if (trackContext.Stss != null)
            {
                isRandomAccessPoint = trackContext.Stss.SampleNumber.Contains(sampleIndex + 1);
            }

            uint sampleSize = trackContext.SizesList[sampleIndex];
            long pts = dts + cttsSampleDelta;

            // seek to the chunk offset
            context.Mdat.Data.Stream.SeekFromBeginning((long)startAddr);

            size += context.Mdat.Data.Stream.ReadBytes(sampleSize, out byte[] sampleData);

            trackContext.SampleIndex++;

            return new Mp4Sample2(pts, dts, sttsSampleDelta, sampleData, isRandomAccessPoint);
        }
    }

    public class TrackContext
    {
        public TrackTypes TrackType { get; internal set; }
        public int NalLengthSize { get; internal set; } = 4;
        public List<byte[]> VideoNals { get; internal set; } = new List<byte[]>();
        public SampleTableBox Stbl { get; internal set; }
        public ChunkOffsetBox Stco { get; internal set; }
        public ChunkLargeOffsetBox Co64 { get; internal set; }
        public SampleToChunkBox Stsc { get; internal set; }
        public SampleSizeBox Stsz { get; internal set; }
        public TimeToSampleBox Stts { get; internal set; }
        public CompositionOffsetBox Ctts { get; internal set; }
        public SyncSampleBox Stss { get; internal set; }
        public uint[] SizesList { get; internal set; }
        public ulong[] ChunkAddressList { get; internal set; }

        public uint SampleIndex { get; set; }
        public uint[] FramesInChunkList { get; set; }
    }

    public enum TrackTypes
    {
        Unknown = 0,
        Video = 1,
        Audio = 2
    }

    public class ContainerContext
    {
        public FileTypeBox Ftyp { get; internal set; }
        public Container Container { get; internal set; }
        public MovieBox Moov { get; internal set; }
        public TrackBox[] Track { get; internal set; }
        public MediaDataBox Mdat { get; internal set; }
        public TrackContext[] Tracks { get; set; }
    }
}
