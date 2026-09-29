using SharpISOBMFF;
using SharpMP4.Tracks;
using SharpMP4.Common;
using SharpMP4.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Readers
{
    /// <summary>
    /// Reads MP4 and Fragmented MP4.
    /// </summary>
    public class VideoReader
    {
        public FileTypeBox Ftyp { get; set; }
        public Container Container { get; set; }
        public MovieBox Moov { get; set; }
        public TrackBox[] Track { get; set; }
        public MovieExtendsBox Mvex { get; set; }
        public MediaDataBox Mdat { get; set; }
        public Dictionary<uint, TrackContext> Tracks { get; set; } = new Dictionary<uint, TrackContext>();
        public TrackFactory TrackFactory { get; set; } = new();

        public IMp4Logger Logger { get; set; }

        /// <summary>
        /// The key of a key ID, for protected tracks to be read in the clear: each sample of a protected track is then
        /// decrypted in place as it is read. Null leaves the samples as they are, their <see cref="MediaSample.Encryption"/>
        /// saying how they are protected; so does a key the provider does not have (null).
        /// </summary>
        public Func<byte[], byte[]> KeyProvider { get; set; }

        public bool IsQuickTime { get; set; } = false;
        public bool IsFragmented { get; set; } = false;

        public VideoReader(IMp4Logger logger)
        {
            this.Logger = logger ?? DefaultMp4Logger.Instance;
        }

        public VideoReader() : this(DefaultMp4Logger.Instance)
        {
        }

        public IEnumerable<ITrack> GetTracks()
        {
            return Tracks.Select(x => x.Value.Track);
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

                    if (this.Ftyp.MajorBrand == IsoStream.FromFourCC("qt  "))
                        this.IsQuickTime = true;
                }
                else if (container.Children[i] is MovieBox)
                {
                    this.Moov = (MovieBox)container.Children[i];
                    this.Track = this.Moov.Children.OfType<TrackBox>().ToArray();

                    this.Mvex = this.Moov.Children.OfType<MovieExtendsBox>().SingleOrDefault(); // fmp4
                    this.IsFragmented = this.Mvex != null;

                    foreach (var track in this.Track)
                    {
                        HandlerBox hdlr = track.Children.OfType<MediaBox>().Single().Children.OfType<HandlerBox>().Single();
                        TrackHeaderBox tkhd = track.Children.OfType<TrackHeaderBox>().First();
                        MediaBox mdia = track.Children.OfType<MediaBox>().Single();
                        MediaHeaderBox mdhd = mdia.Children.OfType<MediaHeaderBox>().Single();
                        uint trackID = tkhd.TrackID;
                        uint trackTimescale = mdhd.Timescale;

                        var trackContext = new TrackContext();
                        this.Tracks.Add(trackID, trackContext);

                        trackContext.Trex = this.Mvex?.Children.OfType<TrackExtendsBox>().SingleOrDefault(x => x.TrackID == trackID); // fmp4
                        
                        SampleTableBox stbl = mdia
                            .Children.OfType<MediaInformationBox>().Single()
                            .Children.OfType<SampleTableBox>().Single();

                        // VisualSampleEntry/AudioSampleEntry/RtpHint: the first of them, as a track's codec is one, though
                        // a protected track may have a second, in the clear, for its samples in the clear ('encv' and 'avc1')
                        var sampleEntries = stbl.Children.OfType<SampleDescriptionBox>().Single().Children;
                        Box sampleEntry = sampleEntries.First();

                        // a protected track's sample entry ('encv', 'enca', ...) holds its 'sinf' beside its configuration
                        trackContext.Stbl = stbl;
                        trackContext.EntryProtections = sampleEntries.Select(TrackProtection.FromSampleEntry).ToArray();
                        trackContext.Protection = trackContext.EntryProtections.FirstOrDefault(x => x != null);
                        foreach (var protection in trackContext.EntryProtections.Where(x => x != null))
                        {
                            protection.Systems.AddRange(this.Moov.Children.Select(ProtectionSystemHeader.From).Where(x => x != null));
                        }

                        // in case of RtpHint, this box has no children
                        if (sampleEntry.Children != null && sampleEntry.Children.Count > 0)
                        {
                            sampleEntry = sampleEntry.Children.FirstOrDefault(x => x is not ProtectionSchemeInfoBox) ?? sampleEntry.Children.First(); // avcC/hvcC/vvcC/esds/dOps...
                        }

                        trackContext.Stts = stbl.Children.OfType<TimeToSampleBox>().Single();

                        // TODO: review, this is needed because of AV1 where we cannot calculate the sample rate
                        int defaultSampleDuration = trackContext.Stts.SampleDelta != null && trackContext.Stts.SampleDelta.Length > 0 ? (int)trackContext.Stts.SampleDelta[0] : 0;
                        // of a fragmented file, whose 'stts' is empty: the first fragment's first sample's - without it, a track
                        // cloned to be written again took a duration of its own, an H.264 track of no timing in its SPS 23.976
                        // frames a second whatever the file's
                        if (defaultSampleDuration == 0)
                            defaultSampleDuration = (int)FirstFragmentSampleDuration(container, trackID, trackContext.Trex);

                        ITrack trackImpl = null;
                        try
                        {
                            trackImpl = TrackFactory.CreateTrack(trackID, sampleEntry, trackTimescale, defaultSampleDuration, hdlr.HandlerType, hdlr.DisplayName, Logger);
                        }
                        catch (NotSupportedException ex)
                        {
                            if (Logger.IsErrorEnabled)
                                Logger.LogError($"Unsupported track type: {hdlr.HandlerType} ({hdlr.DisplayName}) for track ID {trackID}. Exception: {ex.Message}");

                            trackImpl = TrackFactory.CreateGenericTrack(trackID, sampleEntry, trackTimescale, defaultSampleDuration, hdlr.HandlerType, hdlr.DisplayName);
                        }

                        trackImpl?.Logger ??= this.Logger;

                        // a subtitle track forced, as its 'kind' says - of 3GPP timed text, or its sample entry
                        if (trackImpl is ISubtitleTrack subtitles && SubtitleTrackBase.IsForced(track))
                            subtitles.Forced = true;

                        this.Tracks[trackID].Track = trackImpl;

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
                        uint stscSampleDescriptionIndex = 1;
                        trackContext.EntryOfChunk = new uint[trackContext.ChunkAddressList.Length];

                        int chunkIndex;
                        for (chunkIndex = 1; chunkIndex <= trackContext.ChunkAddressList.Length; chunkIndex++)
                        {
                            if (chunkIndex >= stscNextRun)
                            {
                                stscSamplesPerChunk = stsc.SamplesPerChunk[stscIndex];
                                stscSampleDescriptionIndex = stsc.SampleDescriptionIndex[stscIndex];
                                stscIndex += 1;
                                stscNextRun = (stscIndex < stsc.FirstChunk.Length) ? stsc.FirstChunk[stscIndex] : uint.MaxValue;
                            }

                            trackContext.FramesInChunkList[chunkIndex - 1] = stscSamplesPerChunk;
                            trackContext.EntryOfChunk[chunkIndex - 1] = stscSampleDescriptionIndex;
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
                else if (container.Children[i] is FreeSpaceBox)
                {
                    this.IsQuickTime = true; // wide atom in the root is only in QuickTime
                }
            }
        }

        private void ReadFragment(uint trackID)
        {
            if (this.Moov == null)
                throw new InvalidOperationException();

            var container = this.Container;
            var trackContext = this.Tracks[trackID];

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
                            trackContext.Traf = traf;
                            trackContext.Tfhd = tfhd;
                            trackContext.Truns = traf.Children.OfType<TrackRunBox>().ToArray(); // there can be 1 or multiple trun boxes, depending upon the encoder
                            foreach (var trun in trackContext.Truns)
                            {
                                // a run whose samples have none of their own fields - every one the defaults - has entries of
                                // no bytes, which reading the box, as it ends, does not make: sample_count of them
                                var entries = trun._TrunEntry ?? Array.Empty<TrunEntry>();
                                if (entries.Length < trun.SampleCount && (trun.Flags & 0xF00) == 0)
                                {
                                    var all = new TrunEntry[trun.SampleCount];
                                    Array.Copy(entries, all, entries.Length);
                                    for (int e = entries.Length; e < all.Length; e++)
                                    {
                                        all[e] = new TrunEntry(trun.Version, trun.Flags) { Flags = trun.Flags };
                                        all[e].SetParent(trun);
                                    }
                                    trun._TrunEntry = all;
                                }
                            }
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

                            // without a 'tfdt', the fragment starts where the track's previous one ended
                            long dts = tfdt != null ? (long)tfdt.BaseMediaDecodeTime : trackContext.FragmentEndDts;

                            long startAddressBase = BaseDataOffsetOf(currentMoof, traf);

                            trackContext.FragmentBaseAddress = startAddressBase;
                            trackContext.FragmentSampleStartAddress = new long[sampleCount];
                            trackContext.FragmentSampleDts = new long[sampleCount];
                            trackContext.FragmentSampleTrunIndex = new int[sampleCount];
                            trackContext.FragmentSampleTrunEntryIndex = new int[sampleCount];

                            int sampleIndex = 0;
                            long startAddress = startAddressBase;
                            for (int k = 0; k < trackContext.Truns.Length; k++)
                            {
                                // a run without a data offset starts where the one before it ends, the first at the base (8.8.8.3)
                                if ((trackContext.Truns[k].Flags & 0x1) == 0x1)
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
                            trackContext.FragmentEndDts = dts;
                        }
                    }
                }
                else if (container.Children[i] is MediaDataBox)
                {
                    if (moof != null) // we only care about the mdat after we found a corresponding moof
                    {
                        var currentMdat = (MediaDataBox)container.Children[i];

                        // an mdat of its header alone is of a fragment whose samples are all empty - a gap in a text
                        // track - or are elsewhere; one smaller is invalid
                        if (currentMdat.Size >= 8)
                        {
                            trackContext.Mdat = currentMdat;

                            // the protection of each of the fragment's samples, whose auxiliary information may be anywhere in the
                            // file: none where the fragment's sample entry is one in the clear
                            uint entry = (trackContext.Tfhd.Flags & 0x2) == 0x2 ? trackContext.Tfhd.SampleDescriptionIndex : trackContext.Trex?.DefaultSampleDescriptionIndex ?? 1;
                            var protection = trackContext.ProtectionOfEntry(entry);
                            trackContext.FragmentEncryption = protection == null ? null :
                                SampleEncryptionReader.ForFragment(protection, trackContext.Traf, trackContext.Stbl, trackContext.FragmentSampleCount,
                                    trackContext.FragmentBaseAddress, (offset, length) => ReadAt(currentMdat.Data.Stream, offset, length));

                            currentMdat.Data?.Stream?.SeekFromBeginning(currentMdat.Data.Position);

                            // this makes sure next time we call this it will read the next fragment
                            trackContext.FragmentIndex = i;
                        }
                        else
                        {
                            this.Logger.LogError("Fragmented MP4 with empty MDAT box");
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Where a track fragment's data offsets count from (ISO/IEC 14496-12 8.8.7.1): its base-data-offset where it has
        /// one; else the start of the 'moof' where default-base-is-moof is set, or it is the first track fragment; else
        /// the end of the data of the track fragment before it.
        /// </summary>
        /// <summary>
        /// The duration of the first sample of a track's first fragment (14496-12 8.8.8): its own in the 'trun', else the
        /// 'tfhd's default, else the 'trex's. 0 where the track has no fragment, or no duration is given.
        /// </summary>
        private static uint FirstFragmentSampleDuration(Container container, uint trackID, TrackExtendsBox trex)
        {
            foreach (var moof in container.Children.OfType<MovieFragmentBox>())
            {
                foreach (var traf in moof.Children.OfType<TrackFragmentBox>())
                {
                    var tfhd = traf.Children.OfType<TrackFragmentHeaderBox>().FirstOrDefault();
                    if (tfhd == null || tfhd.TrackID != trackID)
                        continue;

                    var entry = traf.Children.OfType<TrackRunBox>().FirstOrDefault(t => t.SampleCount > 0)?._TrunEntry?.FirstOrDefault();
                    if (entry != null && (entry.Flags & 0x100) == 0x100)
                        return entry.SampleDuration;
                    if ((tfhd.Flags & 0x8) == 0x8)
                        return tfhd.DefaultSampleDuration;
                    return trex?.DefaultSampleDuration ?? 0;
                }
            }
            return trex?.DefaultSampleDuration ?? 0;
        }

        private long BaseDataOffsetOf(MovieFragmentBox moof, TrackFragmentBox target)
        {
            long moofOffset = moof.GetBoxOffset();
            long previousEnd = moofOffset;
            bool first = true;
            foreach (var traf in moof.Children.OfType<TrackFragmentBox>())
            {
                var tfhd = traf.Children.OfType<TrackFragmentHeaderBox>().Single();
                long baseOffset = (tfhd.Flags & 0x1) == 0x1 ? (long)tfhd.BaseDataOffset
                    : (tfhd.Flags & 0x20000) == 0x20000 || first ? moofOffset
                    : previousEnd;
                if (traf == target)
                    return baseOffset;

                // where this track fragment's data ends: after the last byte of its runs
                var trex = this.Mvex?.Children.OfType<TrackExtendsBox>().FirstOrDefault(x => x.TrackID == tfhd.TrackID);
                long position = baseOffset;
                previousEnd = baseOffset;
                foreach (var trun in traf.Children.OfType<TrackRunBox>())
                {
                    if ((trun.Flags & 0x1) == 0x1)
                        position = baseOffset + trun.DataOffset;
                    foreach (var entry in trun._TrunEntry)
                        position += (entry.Flags & 0x200) == 0x200 ? entry.SampleSize : (tfhd.Flags & 0x10) == 0x10 ? tfhd.DefaultSampleSize : trex?.DefaultSampleSize ?? 0;
                    previousEnd = Math.Max(previousEnd, position);
                }
                first = false;
            }
            return moofOffset;
        }

        public MediaSample ReadSample(uint trackID)
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

        private MediaSample ReadMp4Sample(uint trackID)
        {
            var trackContext = this.Tracks[trackID];

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

            // The decode time is the sum of the durations of every sample before this one, and the
            // duration is the one given by the run this sample falls in. Both tables store runs of
            // (count, value), so each is walked from the start until the run holding the sample.
            long dts = 0;
            uint sttsSampleDelta = 0;
            uint sttsRunStart = 0;
            for (int i = 0; trackContext.Stts != null && i < trackContext.Stts.SampleCount.Length; i++)
            {
                uint sampleCount = trackContext.Stts.SampleCount[i];
                sttsSampleDelta = trackContext.Stts.SampleDelta[i];

                if (sampleIndex < sttsRunStart + sampleCount)
                {
                    dts += (sampleIndex - sttsRunStart) * sttsSampleDelta;
                    break;
                }

                dts += sampleCount * sttsSampleDelta;
                sttsRunStart += sampleCount;
            }

            // The composition offset says how far the sample is shown from where it is decoded. It
            // belongs to the presentation time alone: adding it to the decode time as well leaves
            // the decode times of a reordered stream jumping back and forth.
            int cttsSampleDelta = 0;
            uint cttsRunStart = 0;
            for (int i = 0; trackContext.Ctts != null && i < trackContext.Ctts.SampleCount.Length; i++)
            {
                uint sampleCount = trackContext.Ctts.SampleCount[i];

                if (sampleIndex < cttsRunStart + sampleCount)
                {
                    cttsSampleDelta = trackContext.Ctts.Version == 0
                        ? (int)trackContext.Ctts.SampleOffset[i]
                        : trackContext.Ctts.SampleOffset0[i];
                    break;
                }

                cttsRunStart += sampleCount;
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

            // Read into the track's own buffer, which is reused from one sample to the next: a
            // track of any length costs one buffer rather than one array per sample. The sample
            // table says how large the largest sample is, so it is made that size at once rather
            // than grown into.
            if (trackContext.SampleBuffer == null || trackContext.SampleBuffer.Length < sampleSize)
            {
                uint capacity = sampleSize;
                foreach (uint sampleLength in trackContext.SizesList)
                {
                    if (sampleLength > capacity)
                        capacity = sampleLength;
                }

                trackContext.SampleBuffer = new byte[capacity];
            }

            // the protection of every sample of the track, once, where it is protected
            if (trackContext.Protection != null && trackContext.SampleEncryptions == null)
            {
                var stream = this.Mdat.Data.Stream;
                trackContext.SampleEncryptions = SampleEncryptionReader.ForTrack(trackContext.Protection, trackContext.Stbl, trackContext.FramesInChunkList,
                    (offset, length) => ReadAt(stream, offset, length));
                stream.SeekFromBeginning(startAddress);

                // none for the samples of a chunk of a sample entry in the clear
                for (int chunk = 0, sample = 0; chunk < trackContext.FramesInChunkList.Length; chunk++)
                {
                    bool clear = trackContext.ProtectionOfEntry(trackContext.EntryOfChunk[chunk]) == null;
                    for (int k = 0; k < trackContext.FramesInChunkList[chunk] && sample < trackContext.SampleEncryptions.Length; k++, sample++)
                    {
                        if (clear)
                            trackContext.SampleEncryptions[sample] = null;
                    }
                }
            }

            ulong size = this.Mdat.Data.Stream.ReadBytes(sampleSize, trackContext.SampleBuffer, 0);

            var mediaSample = new MediaSample(pts, dts, (int)sttsSampleDelta,
                new ArraySegment<byte>(trackContext.SampleBuffer, 0, (int)sampleSize), isRandomAccessPoint);
            mediaSample.Encryption = trackContext.SampleEncryptions != null && sampleIndex < trackContext.SampleEncryptions.Length ? trackContext.SampleEncryptions[sampleIndex] : null;
            Decrypt(trackContext, mediaSample);

            trackContext.SampleIndex++;

            return mediaSample;
        }

        private MediaSample ReadFragmentedMp4Sample(uint trackID)
        {
            var trackContext = this.Tracks[trackID];

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

            // the first sample's own flags where the run has them, else each sample's, else the track fragment's default,
            // else the track's (14496-12 8.8.8.3)
            uint sampleFlags;
            if (trunEntryIndex == 0 && (trun.Flags & 0x4) == 0x4)
                sampleFlags = trun.FirstSampleFlags;
            else if ((entry.Flags & 0x400) == 0x400)
                sampleFlags = entry.SampleFlags;
            else if ((trackContext.Tfhd.Flags & 0x20) == 0x20)
                sampleFlags = trackContext.Tfhd.DefaultSampleFlags;
            else
                sampleFlags = trackContext.Trex?.DefaultSampleFlags ?? 0;

            // a sync sample: sample_is_non_sync_sample 0
            bool isRandomAccessPoint = (sampleFlags & 0x10000) == 0;

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
            // an empty sample has nothing to read, where an mdat of its header alone may have no stream
            if (sampleSize > 0 && trackContext.Mdat.Data.Stream.GetCurrentOffset() != startAddress)
            {
                trackContext.Mdat.Data.Stream.SeekFromBeginning(startAddress);
            }

            // Into the track's own buffer, as in the unfragmented case.
            if (trackContext.SampleBuffer == null || trackContext.SampleBuffer.Length < sampleSize)
            {
                int capacity = trackContext.SampleBuffer == null ? 64 * 1024 : trackContext.SampleBuffer.Length;
                while (capacity < sampleSize)
                    capacity *= 2;

                trackContext.SampleBuffer = new byte[capacity];
            }

            ulong size = sampleSize == 0 ? 0 : trackContext.Mdat.Data.Stream.ReadBytes(sampleSize, trackContext.SampleBuffer, 0);

            var mediaSample = new MediaSample(pts, dts, (int)sampleDuration,
                new ArraySegment<byte>(trackContext.SampleBuffer, 0, (int)sampleSize), isRandomAccessPoint);
            mediaSample.Encryption = trackContext.FragmentEncryption != null && trackContext.SampleIndex < trackContext.FragmentEncryption.Length ? trackContext.FragmentEncryption[trackContext.SampleIndex] : null;
            Decrypt(trackContext, mediaSample);

            trackContext.SampleIndex++;
            return mediaSample;
        }

        /// <summary>A sample of a protected track, decrypted in place where the <see cref="KeyProvider"/> has its key.</summary>
        private void Decrypt(TrackContext trackContext, MediaSample sample)
        {
            if (KeyProvider == null || sample.Encryption == null || !sample.Encryption.IsProtected || sample.Encryption.KeyId == null)
                return;

            byte[] key = KeyProvider(sample.Encryption.KeyId);
            if (key == null)
                return;

            CommonEncryption.Decrypt(trackContext.Protection.Scheme, key, sample.Encryption, sample.Data.Array, sample.Data.Offset, sample.Data.Count);
            sample.Encryption = null; // it is in the clear now
        }

        /// <summary>So many bytes at an offset of the file, the stream left where it was.</summary>
        private static byte[] ReadAt(IsoStream stream, long offset, int length)
        {
            long position = stream.GetCurrentOffset();
            try
            {
                stream.SeekFromBeginning(offset);
                stream.ReadBytes((ulong)length, out byte[] bytes);
                return bytes;
            }
            finally
            {
                stream.SeekFromBeginning(position);
            }
        }

        public IEnumerable<ArraySegment<byte>> ParseSample(uint trackID, ArraySegment<byte> sample)
        {
            var trackContext = this.Tracks[trackID];
            return trackContext.Track.ParseSample(sample.Array, sample.Offset, sample.Count);
        }
    }    

    public class TrackContext
    {
        public uint SampleIndex { get; set; }
        public ITrack Track { get; set; }

        /// <summary>
        /// Where this track's samples are read, one after another. It grows to the largest sample
        /// and is then reused, so each sample is a slice of it rather than an array of its own.
        /// </summary>
        public byte[] SampleBuffer { get; set; }

        /// <summary>How the track is protected, from its sample entry's 'sinf'; null where it is not.</summary>
        public TrackProtection Protection { get; set; }

        public SampleTableBox Stbl { get; set; }

        /// <summary>Of a protected track that is not fragmented: each sample's protection.</summary>
        public SampleEncryption[] SampleEncryptions { get; set; }

        /// <summary>The protection of each of the track's sample entries, null for one in the clear.</summary>
        public TrackProtection[] EntryProtections { get; set; }

        /// <summary>The sample entry of each chunk, by its index from 1.</summary>
        public uint[] EntryOfChunk { get; set; }

        /// <summary>The protection of the sample entry of an index from 1: null where it is in the clear.</summary>
        public TrackProtection ProtectionOfEntry(uint index) =>
            EntryProtections != null && index >= 1 && index <= EntryProtections.Length ? EntryProtections[index - 1] : Protection;

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
        public TrackFragmentBox Traf { get; set; }
        public TrackFragmentHeaderBox Tfhd { get; set; }
        public long FragmentBaseAddress { get; set; }

        /// <summary>Where the decode time of the track's fragments has got to: the start of the next, without a 'tfdt'.</summary>
        public long FragmentEndDts { get; set; }
        public SampleEncryption[] FragmentEncryption { get; set; }
        public TrackExtendsBox Trex { get; set; }
        public int[] FragmentSampleTrunIndex { get; set; }
        public int[] FragmentSampleTrunEntryIndex { get; set; }
        public long[] FragmentSampleStartAddress { get; set; }
        public long[] FragmentSampleDts { get; set; }
        public int FragmentSampleCount { get; set; }
    }
}
