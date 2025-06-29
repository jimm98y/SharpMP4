using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Readers
{
    public class Mp4Reader
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
   
            for (int i = 0; i < container.Children.Count; i++)
            {
                if (container.Children[i] is FileTypeBox)
                {
                    context.Ftyp = (FileTypeBox)container.Children[i];
                }
                else if (container.Children[i] is MovieBox)
                {
                    context.Moov = (MovieBox)container.Children[i];
                    context.Track = context.Moov.Children.OfType<TrackBox>().ToArray();
                    context.Tracks = new TrackContext[context.Track.Length];

                    context.Mvex = context.Moov.Children.OfType<MovieExtendsBox>().SingleOrDefault(); // fmp4
                    context.IsFragmented = context.Mvex != null;

                    foreach (var track in context.Track)
                    {
                        HandlerBox hdlr = track.Children.OfType<MediaBox>().Single().Children.OfType<HandlerBox>().Single();
                        uint trackID = track.Children.OfType<TrackHeaderBox>().First().TrackID;

                        var trackContext = new TrackContext();
                        context.Tracks[(int)(trackID - 1)] = trackContext;

                        trackContext.Trex = context.Mvex?.Children.OfType<TrackExtendsBox>().SingleOrDefault(x => x.TrackID == trackID); // fmp4

                        if (hdlr.HandlerType == IsoStream.FromFourCC(HandlerTypes.Sound))
                        {
                            context.Tracks[(int)(trackID - 1)].TrackType = Mp4TrackTypes.Audio;
                        }
                        else if (hdlr.HandlerType == IsoStream.FromFourCC(HandlerTypes.Video))
                        {
                            context.Tracks[(int)(trackID - 1)].TrackType = Mp4TrackTypes.Video;

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

                        var stbl = track
                           .Children.OfType<MediaBox>().Single()
                           .Children.OfType<MediaInformationBox>().Single()
                           .Children.OfType<SampleTableBox>().Single();
                        var stco = stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                        var co64 = stbl.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();
                        var stsc = stbl.Children.OfType<SampleToChunkBox>().Single();
                        var stsz = stbl.Children.OfType<SampleSizeBox>().Single();
                        trackContext.Stts = stbl.Children.OfType<TimeToSampleBox>().Single();
                        trackContext.Ctts = stbl.Children.OfType<CompositionOffsetBox>().SingleOrDefault();
                        trackContext.Stss = stbl.Children.OfType<SyncSampleBox>().SingleOrDefault(); // optional
                        trackContext.SizesList = stsz.SampleSize > 0 ? Enumerable.Repeat(stsz.SampleSize, (int)stsz.SampleCount).ToArray() : stsz.EntrySize;
                        trackContext.ChunkAddressList = stco != null ? stco.ChunkOffset.Select(x => (ulong)x).ToArray() : co64.ChunkOffset;
                        trackContext.FramesInChunkList = new uint[trackContext.ChunkAddressList.Length];

                        int stscIndex = 0;
                        uint stscNextRun = 0;
                        uint stscSamplesPerChunk = 0;

                        int chunkIndex;
                        for (chunkIndex = 1; chunkIndex <= trackContext.ChunkAddressList.Length; chunkIndex++)
                        {
                            if (chunkIndex >= stscNextRun)
                            {
                                stscSamplesPerChunk = stsc.SamplesPerChunk[stscIndex];
                                stscIndex += 1;
                                stscNextRun = (stscIndex < stsc.FirstChunk.Length) ? stsc.FirstChunk[stscIndex] : uint.MaxValue;
                            }

                            trackContext.FramesInChunkList[chunkIndex - 1] = stscSamplesPerChunk;
                        }
                    }
                }
                else if (container.Children[i] is MediaDataBox)
                {
                    var currentMdat = (MediaDataBox)container.Children[i];

                    if (currentMdat.Size > 8) // mdat smaller than 8 bytes is empty and invalid
                    {
                        context.Mdat = currentMdat;
                    }
                    else
                    {
                        // TODO: trace error
                    }
                }
                else if (container.Children[i] is MovieFragmentBox)
                {
                    context.IsFragmented = true;
                    break;
                }
            }

            return context;
        }

        public static ContainerContext ReadNextFragment(ContainerContext context, int trackID)
        {
            if (context.Moov == null)
                throw new InvalidOperationException();

            var container = context.Container;
            var trackContext = context.Tracks[trackID - 1];

            MovieFragmentBox moof = null;

            for (int i = trackContext.FragmentIndex; i < container.Children.Count; i++)
            {
                if (container.Children[i] is MovieFragmentBox)
                {
                    var currentMoof = (MovieFragmentBox)container.Children[i];
                    var mfhd = currentMoof.Children.OfType<MovieFragmentHeaderBox>().Single();
                    var trafs = currentMoof.Children.OfType<TrackFragmentBox>();
                    foreach (var traf in trafs)
                    {
                        var tfhd = traf.Children.OfType<TrackFragmentHeaderBox>().Single();
                        if(tfhd.TrackID == trackID)
                        {
                            moof = currentMoof;
                            trackContext.Moof = moof;
                            trackContext.Mfhd = mfhd;
                            trackContext.Tfhd = tfhd;
                            trackContext.Truns = traf.Children.OfType<TrackRunBox>().ToArray(); // there can be 1 or multiple trun boxes, depending upon the encoder
                            var tfdt = traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().SingleOrDefault();

                            // pre-calculate DTS and address
                            long dts = (long)tfdt.BaseMediaDecodeTime;
                            if (tfdt != null)
                            {
                                dts = (long)tfdt.BaseMediaDecodeTime;
                            }

                            long startAddress = trackContext.Moof.GetBoxOffset();
                            if ((trackContext.Tfhd.Flags & 0x1) == 0x1)
                            {
                                startAddress = (long)trackContext.Tfhd.BaseDataOffset;
                            }

                            int sampleCount = 0;
                            for (int k = 0; k < trackContext.Truns.Length; k++)
                            {
                                for (int j = 0; j < trackContext.Truns[k]._TrunEntry.Length; j++)
                                {
                                    sampleCount++;
                                }
                            }

                            trackContext.FragmentSampleCount = sampleCount;
                            trackContext.FragmentSampleStartAddress = new long[sampleCount];
                            trackContext.FragmentSampleDts = new long[sampleCount];
                            trackContext.FragmentSampleTrunIndex = new int[sampleCount];
                            trackContext.FragmentSampleTrunEntryIndex = new int[sampleCount];

                            int sampleIndex = 0;
                            for (int k = 0; k < trackContext.Truns.Length; k++)
                            {
                                for (int j = 0; j < trackContext.Truns[k]._TrunEntry.Length; j++)
                                {
                                    var trunEntry = trackContext.Truns[k]._TrunEntry[j];

                                    uint trunEntryDuration = trackContext.Tfhd.DefaultSampleDuration;
                                    if ((trunEntry.Flags & 0x100) == 0x100)
                                        trunEntryDuration = trunEntry.SampleDuration;
                                    else if ((trackContext.Tfhd.Flags & 0x8) == 0x8)
                                        trunEntryDuration = trackContext.Tfhd.DefaultSampleDuration;
                                    else if (trackContext.Trex != null)
                                        trunEntryDuration = trackContext.Trex.DefaultSampleDuration;
                                    else
                                        throw new Exception("Cannot get sample duration");

                                    uint trunEntrySize = trackContext.Tfhd.DefaultSampleSize;
                                    if ((trunEntry.Flags & 0x200) == 0x200)
                                        trunEntrySize = trunEntry.SampleSize;
                                    else if ((trackContext.Tfhd.Flags & 0x10) == 0x10)
                                        trunEntrySize = trackContext.Tfhd.DefaultSampleSize;
                                    else if (trackContext.Trex != null)
                                        trunEntrySize = trackContext.Trex.DefaultSampleSize;
                                    else
                                        throw new Exception("Cannot get sample size");

                                    trackContext.FragmentSampleStartAddress[sampleIndex] = startAddress;
                                    startAddress += trunEntrySize;

                                    trackContext.FragmentSampleDts[sampleIndex] = dts;
                                    dts += trunEntryDuration;

                                    trackContext.FragmentSampleTrunIndex[sampleIndex] = k;
                                    trackContext.FragmentSampleTrunEntryIndex[sampleIndex] = j;
                                    sampleIndex++;
                                }
                            }
                        }
                    }
                }
                else if (container.Children[i] is MediaDataBox)
                {
                    if (moof != null) // we only care about the mdat after we found a corresponding moof
                    {
                        var currentMdat = (MediaDataBox)container.Children[i];

                        if (currentMdat.Size > 8) // mdat smaller than 8 bytes is empty and invalid
                        {
                            trackContext.Mdat = currentMdat;
                            currentMdat.Data.Stream.SeekFromBeginning(currentMdat.Data.Position);

                            // this makes sure next time we call this it will read the next fragment
                            trackContext.FragmentIndex = i;
                        }
                        else
                        {
                            // TODO: trace error
                        }
                        break;
                    }
                }
            }

            return context;
        }

        public static Mp4Sample ReadSample(ContainerContext context, int trackID)
        {
            if (context.Moov == null)
                throw new InvalidOperationException();

            if (context.IsFragmented)
            {
                return ReadFragmentedMp4Sample(context, trackID);
            }
            else
            {
                return ReadMp4Sample(context, trackID);
            }
        }

        private static Mp4Sample ReadMp4Sample(ContainerContext context, int trackID)
        {
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
            ulong startAddress = trackContext.ChunkAddressList[chunkIndex];

            for (int k = 0; k < numFramesInChunk; k++)
            {
                if (firstFrameInChunk + k == sampleIndex)
                {
                    break;
                }

                startAddress += trackContext.SizesList[firstFrameInChunk + k];
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
                    if (trackContext.Ctts.Version == 0)
                        cttsSampleDelta = (int)trackContext.Ctts.SampleOffset[cttsIndex];
                    else
                        cttsSampleDelta = trackContext.Ctts.SampleOffset0[cttsIndex];

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
            context.Mdat.Data.Stream.SeekFromBeginning((long)startAddress);

            ulong size = context.Mdat.Data.Stream.ReadBytes(sampleSize, out byte[] sampleData);

            trackContext.SampleIndex++;

            return new Mp4Sample(pts, dts, sttsSampleDelta, sampleData, isRandomAccessPoint);
        }

        private static Mp4Sample ReadFragmentedMp4Sample(ContainerContext context, int trackID)
        {
            var trackContext = context.Tracks[trackID - 1];

            if (trackContext.Moof == null || trackContext.SampleIndex >= trackContext.FragmentSampleCount || trackContext.SampleIndex < 0) // TODO: sample streaming backwards
            {
                trackContext.SampleIndex = 0;
                trackContext.Moof = null;
                trackContext.Mdat = null;

                context = ReadNextFragment(context, trackID);

                if (trackContext.Moof == null || trackContext.Mdat == null) // no more fragments available
                {
                    return null;
                }
            }

            var trun = trackContext.Truns[trackContext.FragmentSampleTrunIndex[trackContext.SampleIndex]];
            int trunEntryIndex = trackContext.FragmentSampleTrunEntryIndex[trackContext.SampleIndex];

            uint firstSampleFlags = trun.FirstSampleFlags;
            if ((trun.Flags & 0x4) != 0x4)
                firstSampleFlags = trackContext.Tfhd.DefaultSampleFlags;

            var entry = trun._TrunEntry[trunEntryIndex];

            uint sampleDuration = trackContext.Tfhd.DefaultSampleDuration;
            if ((entry.Flags & 0x100) == 0x100)
                sampleDuration = entry.SampleDuration;
            else if ((trackContext.Tfhd.Flags & 0x8) == 0x8)
                sampleDuration = trackContext.Tfhd.DefaultSampleDuration;
            else if (trackContext.Trex != null)
                sampleDuration = trackContext.Trex.DefaultSampleDuration;
            else
                throw new Exception("Cannot get sample duration");

            uint sampleSize = trackContext.Tfhd.DefaultSampleSize;
            if ((entry.Flags & 0x200) == 0x200)
                sampleSize = entry.SampleSize;
            else if ((trackContext.Tfhd.Flags & 0x10) == 0x10)
                sampleSize = trackContext.Tfhd.DefaultSampleSize;
            else if (trackContext.Trex != null)
                sampleSize = trackContext.Trex.DefaultSampleSize;
            else
                throw new Exception("Cannot get sample size");

            uint sampleFlags = trackContext.Tfhd.DefaultSampleFlags;
            if (trunEntryIndex == 0)
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

            long dts = trackContext.FragmentSampleDts[trackContext.SampleIndex];
            long pts = dts + sampleCompositionTime;

            ulong size = trackContext.Mdat.Data.Stream.ReadBytes(sampleSize, out byte[] sampleData);
                        
            trackContext.SampleIndex++;
            return new Mp4Sample(pts, dts, sampleDuration, sampleData);
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

    public enum Mp4TrackTypes
    {
        Unknown = 0,
        Video = 1,
        Audio = 2
    }

    public class Mp4Sample
    {
        public long PTS { get; set; }
        public long DTS { get; set; }
        public uint Duration { get; set; }
        public byte[] Data { get; set; }
        public bool IsRandomAccessPoint { get; set; }
        public Mp4Sample(long pts, long dts, uint duration, byte[] data, bool isRandomAccessPoint = true)
        {
            this.PTS = pts;
            this.DTS = dts;
            this.Duration = duration;
            this.Data = data;
            this.IsRandomAccessPoint = isRandomAccessPoint;
        }
    }

    public class ContainerContext
    {
        public FileTypeBox Ftyp { get; set; }
        public Container Container { get; set; }
        public MovieBox Moov { get; set; }
        public TrackBox[] Track { get; set; }
        public MediaDataBox Mdat { get; set; }
        public TrackContext[] Tracks { get; set; }

        public bool IsFragmented { get; set; } = false; 
        public MovieExtendsBox Mvex { get; set; }
    }

    public class TrackContext
    {
        public Mp4TrackTypes TrackType { get; set; }

        public int NalLengthSize { get; set; } = 4;
        public uint SampleIndex { get; set; }
        public List<byte[]> VideoNals { get; set; } = new List<byte[]>();

        // mp4
        public TimeToSampleBox Stts { get; set; }
        public CompositionOffsetBox Ctts { get; set; }
        public SyncSampleBox Stss { get; set; }
        public uint[] SizesList { get; set; }
        public ulong[] ChunkAddressList { get; set; }
        public uint[] FramesInChunkList { get; set; }

        // fmp4
        public int FragmentIndex { get; set; }
        public MovieFragmentBox Moof { get; set; }
        public MovieFragmentHeaderBox Mfhd { get; set; }
        public MediaDataBox Mdat { get; set; }
        public TrackRunBox[] Truns { get; set; }
        public TrackFragmentHeaderBox Tfhd { get; set; }
        public TrackExtendsBox Trex { get; set; }
        public int[] FragmentSampleTrunIndex { get; set; }
        public int[] FragmentSampleTrunEntryIndex { get; set; }
        public long[] FragmentSampleStartAddress { get; set; }
        public long[] FragmentSampleDts { get; set; }
        public int FragmentSampleCount { get; set; }
    }
}
