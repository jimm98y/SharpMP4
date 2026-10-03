using SharpISOBMFF;
using SharpMP4.Tracks;
using SharpMP4.Common;
using SharpMP4.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Readers
{
    public class MediaSample
    {
        // audio/video specific
        public long PTS { get; set; }
        public long DTS { get; set; }

        private long _duration = -1;

        /// <summary>
        /// The duration, in the track's timescale; -1 where it is not known. A duration of the file is 32 bits unsigned, more
        /// than this holds: one that does not fit is int.MaxValue here, and whole in <see cref="LongDuration"/>.
        /// </summary>
        public int Duration
        {
            get { return _duration > int.MaxValue ? int.MaxValue : (int)_duration; }
            set { _duration = value; }
        }

        /// <summary>The duration, in the track's timescale, whole; -1 where it is not known.</summary>
        public long LongDuration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public bool IsRandomAccessPoint { get; set; }

        /// <summary>
        /// The sample, where it lies in the reader's buffer for its track. It is valid until the next sample of the same
        /// track is read, and no longer: that one is read over it, or into a larger buffer that replaces it, whichever the
        /// reader needs - so it is used, or copied (<c>Data.ToArray()</c>), before then. The samples of other tracks have
        /// buffers of their own.
        /// </summary>
        public ArraySegment<byte> Data { get; set; }

        /// <summary>
        /// How the sample is protected, where its track is: its key ID, IV and subsamples. Null for a sample in the
        /// clear, as a sample the reader has decrypted is.
        /// </summary>
        public SampleEncryption Encryption { get; set; }

        public MediaSample(long pts, long dts, long duration, ArraySegment<byte> data, bool isRandomAccessPoint = true)
        {
            this.PTS = pts;
            this.DTS = dts;
            this.LongDuration = duration;
            this.Data = data;
            this.IsRandomAccessPoint = isRandomAccessPoint;
        }
    }

    /// <summary>
    /// Reads MP4 and Fragmented MP4. Disposed, it disposes the tracks it made: a track hands out entries it reads from a
    /// stream of its own, which a clone of it has its own of too.
    /// </summary>
    public class VideoReader : IDisposable
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
        /// saying how they are protected; so does a key the provider does not have (null), unless <see cref="ThrowOnMissingKey"/>.
        /// </summary>
        public Func<byte[], byte[]> KeyProvider { get; set; }

        /// <summary>
        /// Whether a sample whose key the <see cref="KeyProvider"/> does not have is an error (a <see cref="KeyNotFoundException"/>)
        /// rather than left protected, as it is by default. Without a <see cref="KeyProvider"/>, every sample is left protected.
        /// </summary>
        public bool ThrowOnMissingKey { get; set; }

        public bool IsQuickTime { get; set; } = false;

        /// <summary>
        /// Whether the file has fragments, or says it may have ('mvex'). The samples its 'moov' has, if any, are read first,
        /// then those of the fragments.
        /// </summary>
        public bool IsFragmented { get; set; } = false;

        // where the samples are, of a stream that seeks and of one that does not alike
        private FileDataLocator _data;

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
            this._data = FileDataLocator.Of(container);

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
                            sampleEntry = CodecConfigurationOf(sampleEntry);
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

                        // samples all of one size are of that size, looked up as each is read, rather than an array of as
                        // many of it as the box says there are
                        trackContext.ConstantSampleSize = stsz.SampleSize;
                        trackContext.SizesList = stsz.SampleSize > 0 ? null : stsz.EntrySize ?? Array.Empty<uint>();
                        trackContext.SampleCount = stsz.SampleSize > 0 ? stsz.SampleCount : (uint)trackContext.SizesList.Length;

                        trackContext.ChunkAddressList = stco != null ? stco.ChunkOffset.Select(x => (ulong)x).ToArray() : co64?.ChunkOffset ?? Array.Empty<ulong>();
                        trackContext.FramesInChunkList = new uint[trackContext.ChunkAddressList.Length];

                        int stscIndex = 0;
                        uint stscNextRun = 0;
                        uint stscSamplesPerChunk = 0;
                        uint stscSampleDescriptionIndex = 1;
                        trackContext.EntryOfChunk = new uint[trackContext.ChunkAddressList.Length];

                        int chunkIndex;
                        for (chunkIndex = 1; chunkIndex <= trackContext.ChunkAddressList.Length; chunkIndex++)
                        {
                            if (chunkIndex >= stscNextRun && stscIndex < stsc.FirstChunk.Length)
                            {
                                stscSamplesPerChunk = stsc.SamplesPerChunk[stscIndex];
                                stscSampleDescriptionIndex = stsc.SampleDescriptionIndex[stscIndex];
                                stscIndex += 1;
                                stscNextRun = (stscIndex < stsc.FirstChunk.Length) ? stsc.FirstChunk[stscIndex] : uint.MaxValue;
                            }

                            trackContext.FramesInChunkList[chunkIndex - 1] = stscSamplesPerChunk;
                            trackContext.EntryOfChunk[chunkIndex - 1] = stscSampleDescriptionIndex;
                        }

                        // the fragments, which may follow samples of the 'moov', start where those end, without a 'tfdt'
                        trackContext.FragmentEndDts = trackContext.SampleTableDuration();
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

        // the boxes of a sample entry that configure its codec, which its other boxes - 'pasp', 'colr', 'btrt', 'sinf' - do not
        private static readonly HashSet<uint> CodecConfigurations = new HashSet<uint>(new[]
        {
            "avcC", "hvcC", "vvcC", "av1C", "av2C", "vpcC", "d263", // video
            "esds", "wave", "dOps", "dac3", "dec3", "dac4", "dfLa", "alac", "mhaC", // audio
        }.Select(IsoStream.FromFourCC));

        /// <summary>
        /// The box of a sample entry its track is made from: its codec configuration ('avcC', 'esds', ...) wherever it is
        /// among the entry's boxes - after a 'pasp' or a 'colr', as some write it - else its first box that is not its
        /// protection, else its first.
        /// </summary>
        private static Box CodecConfigurationOf(Box sampleEntry) =>
            sampleEntry.Children.FirstOrDefault(x => CodecConfigurations.Contains(x.FourCC))
            ?? sampleEntry.Children.FirstOrDefault(x => x is not ProtectionSchemeInfoBox)
            ?? sampleEntry.Children.First();

        private TrackContext TrackContextOf(uint trackID)
        {
            if (this.Moov == null)
                throw new InvalidOperationException("No 'moov' has been read: Parse a container that has one first.");

            if (!this.Tracks.TryGetValue(trackID, out var trackContext))
                throw new ArgumentOutOfRangeException(nameof(trackID), trackID, $"The file has no track of ID {trackID}.");

            return trackContext;
        }

        private void ReadFragment(uint trackID, TrackContext trackContext)
        {
            var container = this.Container;

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
                                sampleCount += trackContext.Truns[k]._TrunEntry?.Length ?? 0;
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
                            trackContext.FragmentLargestSample = 0;

                            int sampleIndex = 0;
                            long startAddress = startAddressBase;
                            for (int k = 0; k < trackContext.Truns.Length; k++)
                            {
                                // a run without a data offset starts where the one before it ends, the first at the base (8.8.8.3)
                                if ((trackContext.Truns[k].Flags & 0x1) == 0x1)
                                    startAddress = startAddressBase + trackContext.Truns[k].DataOffset;

                                var trunEntries = trackContext.Truns[k]._TrunEntry ?? Array.Empty<TrunEntry>();
                                for (int j = 0; j < trunEntries.Length; j++)
                                {
                                    var trunEntry = trunEntries[j];
                                    uint trunEntryDuration = RequiredDurationOf(trackID, trunEntry, trackContext.Tfhd, trackContext.Trex);
                                    uint trunEntrySize = RequiredSizeOf(trackID, trunEntry, trackContext.Tfhd, trackContext.Trex);

                                    trackContext.FragmentSampleStartAddress[sampleIndex] = startAddress;
                                    startAddress += trunEntrySize;
                                    trackContext.FragmentLargestSample = Math.Max(trackContext.FragmentLargestSample, trunEntrySize);

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
                            trackContext.FragmentProtection = protection;
                            trackContext.FragmentEncryption = protection == null ? null :
                                SampleEncryptionReader.ForFragment(protection, trackContext.Traf, trackContext.Stbl, trackContext.FragmentSampleCount,
                                    trackContext.FragmentBaseAddress, ReadAt);

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
        /// The duration of a sample of a track fragment (14496-12 8.8.8): its own in the 'trun' where it has one, else the
        /// 'tfhd's default where it has one, else the 'trex's. Null where none of them gives it.
        /// </summary>
        private static uint? DurationOf(TrunEntry entry, TrackFragmentHeaderBox tfhd, TrackExtendsBox trex)
        {
            if (entry != null && (entry.Flags & 0x100) == 0x100)
                return entry.SampleDuration;
            if (tfhd != null && (tfhd.Flags & 0x8) == 0x8)
                return tfhd.DefaultSampleDuration;
            return trex?.DefaultSampleDuration;
        }

        /// <summary>The size of a sample of a track fragment, as <see cref="DurationOf"/> its duration. Null where none gives it.</summary>
        private static uint? SizeOf(TrunEntry entry, TrackFragmentHeaderBox tfhd, TrackExtendsBox trex)
        {
            if (entry != null && (entry.Flags & 0x200) == 0x200)
                return entry.SampleSize;
            if (tfhd != null && (tfhd.Flags & 0x10) == 0x10)
                return tfhd.DefaultSampleSize;
            return trex?.DefaultSampleSize;
        }

        private static uint RequiredDurationOf(uint trackID, TrunEntry entry, TrackFragmentHeaderBox tfhd, TrackExtendsBox trex) =>
            DurationOf(entry, tfhd, trex) ?? throw new InvalidDataException(
                $"A sample of a fragment of track {trackID} has no duration: none in its 'trun', and no default in the 'tfhd' or a 'trex'.");

        private static uint RequiredSizeOf(uint trackID, TrunEntry entry, TrackFragmentHeaderBox tfhd, TrackExtendsBox trex) =>
            SizeOf(entry, tfhd, trex) ?? throw new InvalidDataException(
                $"A sample of a fragment of track {trackID} has no size: none in its 'trun', and no default in the 'tfhd' or a 'trex'.");

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
                    return DurationOf(entry, tfhd, trex) ?? 0;
                }
            }
            return trex?.DefaultSampleDuration ?? 0;
        }

        /// <summary>
        /// Where a track fragment's data offsets count from (ISO/IEC 14496-12 8.8.7.1): its base-data-offset where it has
        /// one; else the start of the 'moof' where default-base-is-moof is set, or it is the first track fragment; else
        /// the end of the data of the track fragment before it.
        /// </summary>
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
                    foreach (var entry in trun._TrunEntry ?? Array.Empty<TrunEntry>())
                        position += SizeOf(entry, tfhd, trex) ?? 0;
                    previousEnd = Math.Max(previousEnd, position);
                }
                first = false;
            }
            return moofOffset;
        }

        /// <summary>
        /// The next sample of a track: of its 'moov' first, then of its fragments. Null after the last.
        /// </summary>
        public MediaSample ReadSample(uint trackID)
        {
            var trackContext = TrackContextOf(trackID);

            // the samples of the 'moov' - all of a file that is not fragmented, and any a fragmented one has before its
            // fragments - then those of the fragments
            if (!trackContext.IsReadingFragments)
            {
                var sample = ReadMp4Sample(trackID, trackContext);
                if (sample != null || !this.IsFragmented)
                    return sample;

                trackContext.IsReadingFragments = true;
                trackContext.SampleIndex = 0;
            }

            return ReadFragmentedMp4Sample(trackID, trackContext);
        }

        private MediaSample ReadMp4Sample(uint trackID, TrackContext trackContext)
        {
            uint sampleIndex = trackContext.SampleIndex;
            if (trackContext.SampleCount <= sampleIndex)
                return null;

            // where the sample is, and its times: of the sample table's runs, walked along with the samples rather than
            // from the start for each - a sample in no chunk is past the end
            if (!trackContext.MoveTo(sampleIndex))
                return null;

            long startAddress = trackContext.CursorAddress;
            uint sampleSize = trackContext.SampleSizeAt(sampleIndex);

            // The decode time is the sum of the durations of every sample before this one, and the duration is the one
            // given by the run this sample falls in. The composition offset says how far the sample is shown from where it
            // is decoded. It belongs to the presentation time alone: adding it to the decode time as well leaves the decode
            // times of a reordered stream jumping back and forth.
            long dts = trackContext.CursorDts;
            long duration = trackContext.CursorDuration;
            long pts = dts + trackContext.CursorCompositionOffset;
            bool isRandomAccessPoint = trackContext.CursorIsSyncSample;

            // Read into the track's own buffer, which is reused from one sample to the next: a track of any length costs
            // one buffer rather than one array per sample. The sample table says how large the largest sample is, so it is
            // made that size at once rather than grown into.
            trackContext.SampleBuffer = BufferFor(trackContext.SampleBuffer, sampleSize, trackContext.LargestSampleSize);

            // the protection of every sample of the track, once, where it is protected
            if (trackContext.Protection != null && trackContext.SampleEncryptions == null)
                trackContext.SampleEncryptions = SampleEncryptionsOf(trackContext);

            ReadSampleData(startAddress, sampleSize, trackContext.SampleBuffer);

            var mediaSample = new MediaSample(pts, dts, duration,
                new ArraySegment<byte>(trackContext.SampleBuffer, 0, (int)sampleSize), isRandomAccessPoint);
            mediaSample.Encryption = trackContext.SampleEncryptions != null && sampleIndex < trackContext.SampleEncryptions.Length ? trackContext.SampleEncryptions[sampleIndex] : null;
            Decrypt(trackContext.ProtectionOfEntry(trackContext.EntryOfChunk[trackContext.CursorChunk]), mediaSample);

            trackContext.SampleIndex++;

            return mediaSample;
        }

        /// <summary>
        /// The protection of each sample of a track that is not fragmented: that its sample entry - its chunk's - says, none
        /// for the samples of an entry in the clear.
        /// </summary>
        private SampleEncryption[] SampleEncryptionsOf(TrackContext trackContext)
        {
            var byProtection = new Dictionary<TrackProtection, SampleEncryption[]>();
            var encryptions = new SampleEncryption[trackContext.FramesInChunkList.Sum(x => (long)x)];
            for (int chunk = 0, sample = 0; chunk < trackContext.FramesInChunkList.Length; chunk++)
            {
                var protection = trackContext.ProtectionOfEntry(trackContext.EntryOfChunk[chunk]);
                if (protection != null && !byProtection.ContainsKey(protection))
                    byProtection[protection] = SampleEncryptionReader.ForTrack(protection, trackContext.Stbl, trackContext.FramesInChunkList, ReadAt);

                var ofEntry = protection == null ? null : byProtection[protection];
                for (int k = 0; k < trackContext.FramesInChunkList[chunk]; k++, sample++)
                    encryptions[sample] = ofEntry != null && sample < ofEntry.Length ? ofEntry[sample] : null;
            }
            return encryptions;
        }

        private MediaSample ReadFragmentedMp4Sample(uint trackID, TrackContext trackContext)
        {
            if (trackContext.Moof == null || trackContext.SampleIndex >= trackContext.FragmentSampleCount) // TODO: sample streaming backwards
            {
                trackContext.SampleIndex = 0;
                trackContext.Moof = null;
                trackContext.Mdat = null;

                ReadFragment(trackID, trackContext);

                if (trackContext.Moof == null || trackContext.Mdat == null) // no more fragments available
                {
                    return null;
                }

                // a fragment of no samples is followed by the next
                if (trackContext.FragmentSampleCount == 0)
                    return ReadFragmentedMp4Sample(trackID, trackContext);
            }

            var trun = trackContext.Truns[trackContext.FragmentSampleTrunIndex[trackContext.SampleIndex]];
            int trunEntryIndex = trackContext.FragmentSampleTrunEntryIndex[trackContext.SampleIndex];

            var entry = trun._TrunEntry[trunEntryIndex];

            uint sampleDuration = RequiredDurationOf(trackID, entry, trackContext.Tfhd, trackContext.Trex);
            uint sampleSize = RequiredSizeOf(trackID, entry, trackContext.Tfhd, trackContext.Trex);

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

            // Into the track's own buffer, as in the unfragmented case: made the size of the fragment's largest sample at
            // once, rather than grown into.
            trackContext.SampleBuffer = BufferFor(trackContext.SampleBuffer, sampleSize, trackContext.FragmentLargestSample);

            // an empty sample has nothing to read, where an mdat of its header alone may have no stream
            ReadSampleData(startAddress, sampleSize, trackContext.SampleBuffer);

            var mediaSample = new MediaSample(pts, dts, sampleDuration,
                new ArraySegment<byte>(trackContext.SampleBuffer, 0, (int)sampleSize), isRandomAccessPoint);
            mediaSample.Encryption = trackContext.FragmentEncryption != null && trackContext.SampleIndex < trackContext.FragmentEncryption.Length ? trackContext.FragmentEncryption[trackContext.SampleIndex] : null;
            Decrypt(trackContext.FragmentProtection, mediaSample);

            trackContext.SampleIndex++;
            return mediaSample;
        }

        // the most an array of bytes holds
        private const int MaxBufferLength = 0x7FFFFFC7;

        /// <summary>
        /// A track's buffer, to read a sample of so many bytes into: the one it has where that is large enough, else one as
        /// large as the largest sample there is to read, so that it is not grown sample by sample.
        /// </summary>
        private static byte[] BufferFor(byte[] buffer, uint size, long largest)
        {
            if (size > MaxBufferLength)
                throw new NotSupportedException($"A sample of {size} bytes is larger than an array holds.");

            if (buffer != null && buffer.Length >= size)
                return buffer;

            return new byte[Math.Min(MaxBufferLength, Math.Max(size, largest))];
        }

        /// <summary>So many bytes of a sample, at an offset of the file, into the buffer.</summary>
        private void ReadSampleData(long offset, uint size, byte[] buffer)
        {
            if (size == 0)
                return;

            if (!_data.TryLocate(offset, size, out IsoStream stream, out long position))
                throw new InvalidDataException($"The {size} bytes at {offset} of the file are in no 'mdat' that was read.");

            if (stream.GetCurrentOffset() != position)
                stream.SeekFromBeginning(position);

            stream.ReadBytes(size, buffer, 0);
        }

        /// <summary>
        /// A sample of a protected track, decrypted in place, in the scheme of its own sample entry, where the
        /// <see cref="KeyProvider"/> has its key.
        /// </summary>
        private void Decrypt(TrackProtection protection, MediaSample sample)
        {
            if (KeyProvider == null || protection == null || sample.Encryption == null || !sample.Encryption.IsProtected || sample.Encryption.KeyId == null)
                return;

            byte[] key = KeyProvider(sample.Encryption.KeyId);
            if (key == null)
            {
                if (ThrowOnMissingKey)
                    throw new KeyNotFoundException($"No key for the key ID {ConvertEx.ToHexString(sample.Encryption.KeyId)} the sample is protected with.");
                return;
            }

            CommonEncryption.Decrypt(protection.Scheme, key, sample.Encryption, sample.Data.Array, sample.Data.Offset, sample.Data.Count);
            sample.Encryption = null; // it is in the clear now
        }

        /// <summary>So many bytes at an offset of the file, the stream they are read from left where it was.</summary>
        private byte[] ReadAt(long offset, int length)
        {
            if (!_data.TryLocate(offset, length, out IsoStream stream, out long position))
                throw new InvalidDataException($"The {length} bytes at {offset} of the file are in no 'mdat' that was read.");

            long current = stream.GetCurrentOffset();
            try
            {
                stream.SeekFromBeginning(position);
                stream.ReadBytes((ulong)length, out byte[] bytes);
                return bytes;
            }
            finally
            {
                stream.SeekFromBeginning(current);
            }
        }

        public IEnumerable<ArraySegment<byte>> ParseSample(uint trackID, ArraySegment<byte> sample)
        {
            var trackContext = TrackContextOf(trackID);
            return trackContext.Track.ParseSample(sample.Array, sample.Offset, sample.Count);
        }

        /// <summary>Disposes the tracks the reader made. The container, and the stream it was read from, are the caller's.</summary>
        public void Dispose()
        {
            foreach (var context in Tracks.Values)
                context.Track?.Dispose();
            Tracks.Clear();
        }
    }

    public class TrackContext
    {
        /// <summary>
        /// The sample to read next: of the 'moov', from the first; then, once those are read, of the current fragment. Moved
        /// back, or anywhere, the 'moov's sample table is walked again from its start.
        /// </summary>
        public uint SampleIndex { get; set; }
        public ITrack Track { get; set; }

        /// <summary>
        /// Where this track's samples are read, one after another. It is made as large as the largest sample of the track,
        /// or of the fragment, and reused; it is replaced, by one larger, only where a sample does not fit. Each sample is a
        /// slice of it rather than an array of its own.
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

        /// <summary>The size of each sample, from 'stsz'; null where they are all of one, <see cref="ConstantSampleSize"/>.</summary>
        public uint[] SizesList { get; set; }

        /// <summary>The size of every sample, where 'stsz' gives one for all; 0 where it gives each its own.</summary>
        public uint ConstantSampleSize { get; set; }

        /// <summary>How many samples 'stsz' says the track has.</summary>
        public uint SampleCount { get; set; }

        public ulong[] ChunkAddressList { get; set; }
        public uint[] FramesInChunkList { get; set; }

        /// <summary>Whether the samples of the 'moov' are read, and those of the fragments are being read.</summary>
        public bool IsReadingFragments { get; set; }

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

        /// <summary>The protection of the current fragment's sample entry: null where it is in the clear.</summary>
        public TrackProtection FragmentProtection { get; set; }
        public TrackExtendsBox Trex { get; set; }
        public int[] FragmentSampleTrunIndex { get; set; }
        public int[] FragmentSampleTrunEntryIndex { get; set; }
        public long[] FragmentSampleStartAddress { get; set; }
        public long[] FragmentSampleDts { get; set; }

        /// <summary>How many samples the current fragment has; an array of them, so no more than an int counts.</summary>
        public int FragmentSampleCount { get; set; }

        /// <summary>The size of the largest sample of the current fragment.</summary>
        public long FragmentLargestSample { get; set; }

        /// <summary>The size of a sample of the 'moov', from its index from 0.</summary>
        public uint SampleSizeAt(uint index) => ConstantSampleSize > 0 ? ConstantSampleSize : SizesList[index];

        private long _largestSampleSize = -1;

        /// <summary>The size of the largest sample of the 'moov'.</summary>
        public long LargestSampleSize
        {
            get
            {
                if (_largestSampleSize < 0)
                {
                    uint largest = ConstantSampleSize;
                    if (SizesList != null)
                    {
                        foreach (uint size in SizesList)
                        {
                            if (size > largest)
                                largest = size;
                        }
                    }
                    _largestSampleSize = largest;
                }
                return _largestSampleSize;
            }
        }

        /// <summary>The duration of the samples of the 'moov': the sum of the runs of its 'stts'.</summary>
        public long SampleTableDuration()
        {
            long duration = 0;
            for (int i = 0; Stts?.SampleCount != null && i < Stts.SampleCount.Length; i++)
                duration += (long)Stts.SampleCount[i] * Stts.SampleDelta[i];
            return duration;
        }

        #region Sample table cursor

        // Where a sample of the 'moov' is, and its times, are of runs - of chunks in 'stsc', of durations in 'stts', of
        // composition offsets in 'ctts' - and of the sync samples of 'stss', each of which is found from the start of its
        // table. The cursor is at a sample, and has the run of each table that sample is in, and where that run starts: the
        // next sample is found from there, a step, and only a sample before it from the start again.

        private bool _cursorValid;
        private uint _cursorSample;

        // the chunk the sample is in, the first sample of that chunk, and where the sample is in the file
        private int _cursorChunk;
        private long _cursorChunkFirstSample;
        private long _cursorAddress;

        // the 'stts' run the sample is in, its first sample, and the decode time of that sample
        private int _sttsRun;
        private long _sttsRunStart;
        private long _sttsRunDts;

        // the 'ctts' run the sample is in, and its first sample
        private int _cttsRun;
        private long _cttsRunStart;

        // the sync samples, in order, and the first of them not before the sample
        private uint[] _syncSamples;
        private int _syncSample;

        /// <summary>The chunk of the sample the cursor is at, from 0.</summary>
        internal int CursorChunk => _cursorChunk;

        /// <summary>Where in the file the sample the cursor is at is.</summary>
        internal long CursorAddress => _cursorAddress;

        /// <summary>The decode time of the sample the cursor is at: the durations of all before it.</summary>
        internal long CursorDts
        {
            get
            {
                if (Stts?.SampleCount != null && _sttsRun < Stts.SampleCount.Length)
                    return _sttsRunDts + (_cursorSample - _sttsRunStart) * (long)Stts.SampleDelta[_sttsRun];
                return _sttsRunDts; // past the last run, as all the runs' durations
            }
        }

        /// <summary>The duration of the sample the cursor is at; past the last run, the last run's.</summary>
        internal long CursorDuration
        {
            get
            {
                if (Stts?.SampleDelta == null || Stts.SampleDelta.Length == 0)
                    return 0;
                return Stts.SampleDelta[Math.Min(_sttsRun, Stts.SampleDelta.Length - 1)];
            }
        }

        /// <summary>The composition offset of the sample the cursor is at; 0 where 'ctts' has none for it.</summary>
        internal int CursorCompositionOffset
        {
            get
            {
                if (Ctts?.SampleCount == null || _cttsRun >= Ctts.SampleCount.Length)
                    return 0;
                return Ctts.Version == 0 ? (int)Ctts.SampleOffset[_cttsRun] : Ctts.SampleOffset0[_cttsRun];
            }
        }

        /// <summary>Whether the sample the cursor is at is a sync sample: every one is, without an 'stss'.</summary>
        internal bool CursorIsSyncSample =>
            _syncSamples == null || (_syncSample < _syncSamples.Length && _syncSamples[_syncSample] == _cursorSample + 1);

        /// <summary>
        /// The cursor at a sample of the 'moov': stepped on from where it is, or, for one before it, from the start. False
        /// where the sample is in no chunk.
        /// </summary>
        internal bool MoveTo(uint sampleIndex)
        {
            if (!_cursorValid || sampleIndex < _cursorSample)
                ResetCursor();

            while (_cursorSample < sampleIndex && _cursorChunk < FramesInChunkList.Length)
                StepCursor();

            return _cursorChunk < FramesInChunkList.Length;
        }

        private void ResetCursor()
        {
            _cursorValid = true;
            _cursorSample = 0;

            _cursorChunk = 0;
            _cursorChunkFirstSample = 0;
            SkipFinishedChunks();
            _cursorAddress = _cursorChunk < ChunkAddressList.Length ? (long)ChunkAddressList[_cursorChunk] : 0;

            _sttsRun = 0;
            _sttsRunStart = 0;
            _sttsRunDts = 0;
            SkipFinishedSttsRuns();

            _cttsRun = 0;
            _cttsRunStart = 0;
            SkipFinishedCttsRuns();

            // in order, as they are to be, so that they are found by stepping
            if (Stss?.SampleNumber != null && _syncSamples == null)
            {
                _syncSamples = (uint[])Stss.SampleNumber.Clone();
                Array.Sort(_syncSamples);
            }
            _syncSample = 0;
            SkipPassedSyncSamples();
        }

        private void StepCursor()
        {
            _cursorAddress += SampleSizeAt(_cursorSample);
            _cursorSample++;

            int chunk = _cursorChunk;
            SkipFinishedChunks();
            if (_cursorChunk != chunk && _cursorChunk < ChunkAddressList.Length)
                _cursorAddress = (long)ChunkAddressList[_cursorChunk];

            SkipFinishedSttsRuns();
            SkipFinishedCttsRuns();
            SkipPassedSyncSamples();
        }

        private void SkipFinishedChunks()
        {
            while (_cursorChunk < FramesInChunkList.Length && _cursorSample >= _cursorChunkFirstSample + FramesInChunkList[_cursorChunk])
            {
                _cursorChunkFirstSample += FramesInChunkList[_cursorChunk];
                _cursorChunk++;
            }
        }

        private void SkipFinishedSttsRuns()
        {
            while (Stts?.SampleCount != null && _sttsRun < Stts.SampleCount.Length && _cursorSample >= _sttsRunStart + Stts.SampleCount[_sttsRun])
            {
                _sttsRunDts += (long)Stts.SampleCount[_sttsRun] * Stts.SampleDelta[_sttsRun];
                _sttsRunStart += Stts.SampleCount[_sttsRun];
                _sttsRun++;
            }
        }

        private void SkipFinishedCttsRuns()
        {
            while (Ctts?.SampleCount != null && _cttsRun < Ctts.SampleCount.Length && _cursorSample >= _cttsRunStart + Ctts.SampleCount[_cttsRun])
            {
                _cttsRunStart += Ctts.SampleCount[_cttsRun];
                _cttsRun++;
            }
        }

        private void SkipPassedSyncSamples()
        {
            while (_syncSamples != null && _syncSample < _syncSamples.Length && _syncSamples[_syncSample] < _cursorSample + 1)
                _syncSample++;
        }

        #endregion // Sample table cursor
    }
}
