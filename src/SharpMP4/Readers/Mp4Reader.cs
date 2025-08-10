using SharpISOBMFF;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Readers
{
    /// <summary>
    /// Reads MP4 and Fragmented MP4.
    /// </summary>
    public class Mp4Reader
    {
        public FileTypeBox Ftyp { get; set; }
        public Container Container { get; set; }
        public MovieBox Moov { get; set; }
        public TrackBox[] Track { get; set; }
        public MediaDataBox Mdat { get; set; }
        public TrackContext[] Tracks { get; set; }

        public bool IsFragmented { get; set; } = false;
        public MovieExtendsBox Mvex { get; set; }


        public IEnumerable<ITrack> GetTracks()
        {
            return Tracks.Select(x => x.Track);
        }

        public void Parse(Container container)
        {
            if (container.Children.Count == 0)
                return;

            this.Container = container;
   
            for (int i = 0; i < container.Children.Count; i++)
            {
                if (container.Children[i] is FileTypeBox)
                {
                    this.Ftyp = (FileTypeBox)container.Children[i];
                }
                else if (container.Children[i] is MovieBox)
                {
                    this.Moov = (MovieBox)container.Children[i];
                    this.Track = this.Moov.Children.OfType<TrackBox>().ToArray();
                    this.Tracks = new TrackContext[this.Track.Length];

                    this.Mvex = this.Moov.Children.OfType<MovieExtendsBox>().SingleOrDefault(); // fmp4
                    this.IsFragmented = this.Mvex != null;

                    foreach (var track in this.Track)
                    {
                        HandlerBox hdlr = track.Children.OfType<MediaBox>().Single().Children.OfType<HandlerBox>().Single();
                        TrackHeaderBox tkhd = track.Children.OfType<TrackHeaderBox>().First();
                        MediaBox mdia = track.Children.OfType<MediaBox>().Single();
                        MediaHeaderBox mdhd = mdia.Children.OfType<MediaHeaderBox>().Single();
                        uint trackID = tkhd.TrackID;
                        int trackIndex = Mp4Utils.TrackIdToTrackIndex(trackID);
                        uint trackTimescale = mdhd.Timescale;

                        var trackContext = new TrackContext();
                        this.Tracks[trackIndex] = trackContext;

                        trackContext.Trex = this.Mvex?.Children.OfType<TrackExtendsBox>().SingleOrDefault(x => x.TrackID == trackID); // fmp4
                        
                        SampleTableBox stbl = mdia
                            .Children.OfType<MediaInformationBox>().Single()
                            .Children.OfType<SampleTableBox>().Single();

                        Box sampleEntry = stbl
                            .Children.OfType<SampleDescriptionBox>().Single()
                            .Children.Single(); // VisualSampleEntry/AudioSampleEntry/RtpHint

                        // in case of RtpHint, this box has no children
                        if (sampleEntry.Children.Count > 0)
                        {
                            sampleEntry = sampleEntry.Children.First(); // avcC/hvcC/vvcC/esds/dOps...
                        }

                        trackContext.Stts = stbl.Children.OfType<TimeToSampleBox>().Single();

                        // TODO: review, this is needed because of AV1 where we cannot calculate the sample rate
                        int defaultSampleDuration = trackContext.Stts.SampleDelta != null && trackContext.Stts.SampleDelta.Length > 0 ? (int)trackContext.Stts.SampleDelta[0] : 0;

                        ITrack trackImpl = null;
                        try
                        {
                            trackImpl = TrackFactory.CreateTrack(trackID, sampleEntry, trackTimescale, defaultSampleDuration, hdlr.HandlerType, hdlr.DisplayName);
                        }
                        catch (NotSupportedException ex)
                        {
                            if(Log.ErrorEnabled) Log.Error($"Unsupported track type: {hdlr.HandlerType} ({hdlr.DisplayName}) for track ID {trackID}. Exception: {ex.Message}");
                            trackImpl = TrackFactory.CreateGenericTrack(trackID, sampleEntry, trackTimescale, defaultSampleDuration, hdlr.HandlerType, hdlr.DisplayName);
                        }
                        this.Tracks[trackIndex].Track = trackImpl;

                        var stco = stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                        var co64 = stbl.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();
                        var stsc = stbl.Children.OfType<SampleToChunkBox>().Single();
                        var stsz = stbl.Children.OfType<SampleSizeBox>().Single();
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
                        this.Mdat = currentMdat;
                    }
                    
                    // in case of multiple MDAT boxes, the offset is determined by the MOOV
                }
                else if (container.Children[i] is MovieFragmentBox)
                {
                    this.IsFragmented = true;
                    break;
                }
            }
        }

        private void ReadFragment(uint trackID)
        {
            if (this.Moov == null)
                throw new InvalidOperationException();

            int trackIndex = Mp4Utils.TrackIdToTrackIndex(trackID);
            var container = this.Container;
            var trackContext = this.Tracks[trackIndex];

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
                            trackContext.Tfhd = tfhd;
                            trackContext.Truns = traf.Children.OfType<TrackRunBox>().ToArray(); // there can be 1 or multiple trun boxes, depending upon the encoder
                            var tfdt = traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().SingleOrDefault();

                            // pre-calculate DTS and address for the fragment
                            int sampleCount = 0;
                            for (int k = 0; k < trackContext.Truns.Length; k++)
                            {
                                for (int j = 0; j < trackContext.Truns[k]._TrunEntry.Length; j++)
                                {
                                    sampleCount++;
                                }
                            }

                            trackContext.FragmentSampleCount = sampleCount;

                            long dts = (long)tfdt.BaseMediaDecodeTime;
                            if (tfdt != null)
                            {
                                dts = (long)tfdt.BaseMediaDecodeTime;
                            }

                            long startAddressBase = 0; // TODO: default?
                            bool defaultBaseIsMoof = (trackContext.Tfhd.Flags & 0x20000u) == 0x20000u;
                            if ((trackContext.Tfhd.Flags & 0x1) == 0x1)
                            {
                                startAddressBase = (long)trackContext.Tfhd.BaseDataOffset;
                            }
                            else if (defaultBaseIsMoof)
                            {
                                startAddressBase = trackContext.Moof.GetBoxOffset();
                            }
                            else
                            {
                                // TODO: review, possibly move to MDAT...
                                throw new NotSupportedException();
                            }

                            trackContext.FragmentSampleStartAddress = new long[sampleCount];
                            trackContext.FragmentSampleDts = new long[sampleCount];
                            trackContext.FragmentSampleTrunIndex = new int[sampleCount];
                            trackContext.FragmentSampleTrunEntryIndex = new int[sampleCount];

                            int sampleIndex = 0;
                            long startAddress = 0;
                            for (int k = 0; k < trackContext.Truns.Length; k++)
                            {
                                startAddress = startAddressBase + trackContext.Truns[k].DataOffset;

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
                            Log.Error($"Fragmented MP4 with empty MDAT box");
                        }
                        break;
                    }
                }
            }
        }

        public Mp4Sample ReadSample(uint trackID)
        {
            if (this.Moov == null)
                throw new InvalidOperationException();

            if (this.IsFragmented)
            {
                return ReadFragmentedMp4Sample(trackID);
            }
            else
            {
                return ReadMp4Sample(trackID);
            }
        }

        private Mp4Sample ReadMp4Sample(uint trackID)
        {
            int trackIndex = Mp4Utils.TrackIdToTrackIndex(trackID);
            var trackContext = this.Tracks[trackIndex];

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
            long startAddress = (long)trackContext.ChunkAddressList[chunkIndex];

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

            if (this.Mdat.Data.Stream.GetCurrentOffset() != startAddress)
            {
                this.Mdat.Data.Stream.SeekFromBeginning(startAddress);
            }

            ulong size = this.Mdat.Data.Stream.ReadBytes(sampleSize, out byte[] sampleData);

            trackContext.SampleIndex++;

            return new Mp4Sample(pts, dts, (int)sttsSampleDelta, sampleData, isRandomAccessPoint);
        }

        private Mp4Sample ReadFragmentedMp4Sample(uint trackID)
        {
            int trackIndex = Mp4Utils.TrackIdToTrackIndex(trackID);
            var trackContext = this.Tracks[trackIndex];

            if (trackContext.Moof == null || trackContext.SampleIndex >= trackContext.FragmentSampleCount || trackContext.SampleIndex < 0) // TODO: sample streaming backwards
            {
                trackContext.SampleIndex = 0;
                trackContext.Moof = null;
                trackContext.Mdat = null;

                ReadFragment(trackID);

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

            long startAddress = trackContext.FragmentSampleStartAddress[trackContext.SampleIndex];
            if (trackContext.Mdat.Data.Stream.GetCurrentOffset() != startAddress)
            {
                trackContext.Mdat.Data.Stream.SeekFromBeginning(startAddress);
            }

            ulong size = trackContext.Mdat.Data.Stream.ReadBytes(sampleSize, out byte[] sampleData);
                        
            trackContext.SampleIndex++;
            return new Mp4Sample(pts, dts, (int)sampleDuration, sampleData);
        }

        public IEnumerable<byte[]> ParseSample(uint trackID, byte[] sample)
        {
            int trackIndex = Mp4Utils.TrackIdToTrackIndex(trackID);
            var trackContext = this.Tracks[trackIndex];
            return trackContext.Track.ParseSample(sample);
        }
    }    

    public class Mp4Sample
    {
        public long PTS { get; set; }
        public long DTS { get; set; }
        public int Duration { get; set; } = -1;
        public byte[] Data { get; set; }
        public bool IsRandomAccessPoint { get; set; }
        public Mp4Sample(long pts, long dts, int duration, byte[] data, bool isRandomAccessPoint = true)
        {
            this.PTS = pts;
            this.DTS = dts;
            this.Duration = duration;
            this.Data = data;
            this.IsRandomAccessPoint = isRandomAccessPoint;
        }
    }

    public class TrackContext
    {
        public uint SampleIndex { get; set; }
        public ITrack Track { get; set; }

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
