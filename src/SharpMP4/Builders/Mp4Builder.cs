using SharpISOBMFF;
using SharpISOBMFF.Extensions;
using SharpMP4.Common;
using SharpMP4.Encryption;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Builders
{
    /// <summary>
    /// Creates MP4.
    /// </summary>
    public class Mp4Builder : IMp4Builder, IDisposable
    {
        private class TrackContext
        {
            public ITrack Track { get; set; }

            /// <summary>Where the track's samples wait until <see cref="FinalizeMedia"/> writes them; of its own unless <see cref="Mp4Builder.Storage"/> is set.</summary>
            public IStorage Storage { get; set; }

            /// <summary>Whether the builder made <see cref="Storage"/>, and so disposes it.</summary>
            public bool OwnsStorage { get; set; }

            public List<uint> SampleSizes { get; set; } = new List<uint>();

            /// <summary>Where each sample lies in <see cref="Storage"/>.</summary>
            public List<long> SampleOffsets { get; set; } = new List<long>();

            public List<uint> RandomAccessPoints { get; set; } = new List<uint>();

            /// <summary>
            /// Composition time minus decode time, per sample. Non-zero whenever pictures are
            /// coded out of presentation order, as with B pictures.
            /// </summary>
            public List<int> CompositionOffsets { get; set; } = new List<int>();

            /// <summary>Decode duration of each sample, in track timescale units.</summary>
            public List<uint> SampleDurations { get; set; } = new List<uint>();

            /// <summary>The sum of <see cref="SampleDurations"/>: the track's duration, in its timescale.</summary>
            public ulong Duration { get; set; }

            /// <summary>The timing given for the sample the track is assembling.</summary>
            public PendingSampleTiming Timing { get; } = new PendingSampleTiming();

            /// <summary>Of the file being written: the number of samples in each of the track's chunks, and where each starts in the media data.</summary>
            public List<uint> ChunkSampleCounts { get; } = new List<uint>();
            public List<long> ChunkOffsets { get; } = new List<long>();

            public TrackContext(ITrack track)
            {
                Track = track;
            }

            /// <summary>What protects the track; null where it is not protected.</summary>
            public TrackEncryptor Encryptor { get; set; }

            /// <summary>How each sample is protected.</summary>
            public List<SampleEncryption> Encryptions { get; set; } = new List<SampleEncryption>();

            /// <summary>Of the 'moov' last built: the track's 'senc', and the 'saio' in its 'stbl' that points at it.</summary>
            public SampleEncryptionBox Senc { get; set; }
            public SampleAuxiliaryInformationOffsetsBox Saio { get; set; }
        }

        /// <summary>A run of samples of one track, one after another in the media data.</summary>
        private class Chunk
        {
            public TrackContext Track { get; set; }
            public int FirstSample { get; set; }
            public int SampleCount { get; set; }

            /// <summary>The decode time of the first sample, in the track's timescale.</summary>
            public ulong StartTime { get; set; }
        }

        public uint MovieTimescale { get; set; } = 1000;

        /// <summary>
        /// How composition offsets are written where any is negative: shifted, the default, as Apple writes them, or in
        /// version 1 of 'ctts'. See <see cref="SharpMP4.Builders.NegativeCompositionOffsets"/>.
        /// </summary>
        public NegativeCompositionOffsets NegativeCompositionOffsets { get; set; } = NegativeCompositionOffsets.Shifted;

        /// <inheritdoc/>
        public Mp4FileFormat FileFormat { get; set; } = Mp4FileFormat.Mp4;

        /// <summary>
        /// How long the samples of a track run in one chunk before the next track's samples of the same time follow them,
        /// in milliseconds. A player reads the tracks' samples of the same time from near each other, and a file written as
        /// it arrives - a remuxer's, one track after another - would make it seek from one end of the file to the other.
        /// </summary>
        public uint ChunkDurationInMs { get; set; } = 500;

        /// <summary>
        /// The creation and modification time the movie, track and media headers say. Null - the default - for the time
        /// <see cref="FinalizeMedia"/> is called; set it for output that is the same each time it is written.
        /// </summary>
        public DateTime? CreationTime { get; set; }

        private readonly IMp4Output _output;

        private IStorage _storage;

        private readonly Dictionary<uint, TrackContext> _trackContexts = new Dictionary<uint, TrackContext>();

        private bool _isFinalized;
        private bool _isDisposed;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="output">Output stream, written at <see cref="FinalizeMedia"/>. <see cref="IMp4Output"/>.</param>
        public Mp4Builder(IMp4Output output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        /// <summary>
        /// One storage all the tracks' samples wait in until <see cref="FinalizeMedia"/> writes them, interleaved; it stays
        /// the caller's to dispose. Where it is not set, each track has one of its own from <see cref="TemporaryStorageFactory"/>.
        /// </summary>
        public IStorage Storage
        {
            get => _storage;
            set => _storage = value;
        }

        public ITemporaryStorageFactory TemporaryStorageFactory { get; set; } = new TemporaryFileStorageFactory();
        public IMp4Logger Logger { get; set; } = DefaultMp4Logger.Instance;

        /// <summary>
        /// Add a track to the MP4.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        public void AddTrack(ITrack track)
        {
            if (track == null)
                throw new ArgumentNullException(nameof(track));
            ThrowIfFinished();

            // A track logs where the builder does, unless it was given a logger of its own: TrackBase starts with the
            // default one, so it is replaced too.
            if (track.Logger == null || track.Logger == DefaultMp4Logger.Instance)
                track.Logger = this.Logger;

            // the format's constraints, before the track is the builder's
            FileBrands.Validate(FileFormat, _trackContexts.Values.Select(x => x.Track).Append(track).ToList(), Logger);

            uint trackID = GetNextTrackId();
            track.TrackID = trackID;
            _trackContexts.Add(trackID, new TrackContext(track));
        }

        /// <summary>
        /// Adds a track whose samples are protected as they are written (ISO/IEC 23001-7): each with the key, its IV and,
        /// of H.264, H.265 and H.266 video, its NAL units' slice headers left clear. The sample entry becomes 'encv', 'enca'
        /// or 'enct', with a 'sinf' saying what it was and how it is protected; how each sample is protected is in a 'senc'
        /// in the 'trak', and in the 'saiz' and 'saio' of its 'stbl', which point into the 'senc'.
        /// <see cref="TrackProtection.Create"/> makes a protection with a scheme's defaults.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        /// <param name="protection">How the track is protected: its scheme, key ID, IVs and pattern, and the protection systems' headers.</param>
        /// <param name="key">The 16 byte key; or 32, for AES-256, as the draft of 23001-7:2023 Amendment 1 allows, 'tenc' version 2 saying so.</param>
        public void AddTrack(ITrack track, TrackProtection protection, byte[] key)
        {
            if (track == null)
                throw new ArgumentNullException(nameof(track));
            ThrowIfFinished();

            var encryptor = TrackEncryptor.Create(track, protection, key, Logger);
            AddTrack(track);
            _trackContexts[track.TrackID].Encryptor = encryptor;
        }

        private uint GetNextTrackId()
        {
            uint trackID = 1;
            while (_trackContexts.ContainsKey(trackID))
                trackID++;
            return trackID;
        }

        private void ThrowIfFinished()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(Mp4Builder));
            if (_isFinalized)
                throw new InvalidOperationException("The MP4 was already finalized: FinalizeMedia has written it, and it takes nothing more.");
        }

        private TrackContext GetTrack(uint trackID)
        {
            ThrowIfFinished();
            if (!_trackContexts.TryGetValue(trackID, out var track))
                throw new ArgumentException($"There is no track {trackID}: a track is added with AddTrack, which gives it its ID.", nameof(trackID));
            return track;
        }

        private void WriteSample(TrackContext track, ArraySegment<byte> sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset = 0)
        {
            if (track.Storage == null)
            {
                track.OwnsStorage = _storage == null;
                track.Storage = _storage ?? this.TemporaryStorageFactory.Create(Logger);
            }

            // A negative duration is the track's default; 0 is a sample of no duration, as the last of a text track can be.
            uint currentSampleDuration = sampleDuration < 0 ? (uint)track.Track.DefaultSampleDuration : (uint)sampleDuration;
            if (track.Encryptor != null)
            {
                sample = track.Encryptor.Protect(track.Track, sample, out var encryption);
                track.Encryptions.Add(encryption);
            }

            track.SampleOffsets.Add(track.Storage.GetPosition());
            track.Storage.Write(sample.Array, sample.Offset, sample.Count);
            track.SampleSizes.Add((uint)sample.Count);
            track.CompositionOffsets.Add(compositionOffset);
            track.SampleDurations.Add(currentSampleDuration);
            track.Duration += currentSampleDuration;

            if(isRandomAccessPoint)
            {
                track.RandomAccessPoints.Add((uint)track.SampleSizes.Count);
            }
        }

        /// <summary>
        /// The samples of every track in chunks of about <see cref="ChunkDurationInMs"/>, in the order of their time, which
        /// is how they are written: each track's chunks, and where each starts in the media data, in its context.
        /// </summary>
        private List<Chunk> Interleave()
        {
            var chunks = new List<Chunk>();
            foreach (var track in _trackContexts.Values)
            {
                track.ChunkSampleCounts.Clear();
                track.ChunkOffsets.Clear();

                ulong limit = Math.Max(1, (ulong)track.Track.Timescale * ChunkDurationInMs / 1000);
                ulong time = 0;
                Chunk chunk = null;
                ulong chunkDuration = 0;
                for (int i = 0; i < track.SampleSizes.Count; i++)
                {
                    if (chunk == null)
                    {
                        chunk = new Chunk { Track = track, FirstSample = i, StartTime = time };
                        chunks.Add(chunk);
                        chunkDuration = 0;
                    }

                    chunk.SampleCount++;
                    chunkDuration += track.SampleDurations[i];
                    time += track.SampleDurations[i];
                    if (chunkDuration >= limit)
                        chunk = null;
                }
            }

            // Of two chunks that start together, the track added first first; OrderBy keeps the order of equal keys.
            var trackOrder = _trackContexts.Values.Select((track, index) => (track, index)).ToDictionary(x => x.track, x => x.index);
            var ordered = chunks
                .OrderBy(chunk => chunk.Track.Track.Timescale == 0 ? 0m : (decimal)chunk.StartTime / chunk.Track.Track.Timescale)
                .ThenBy(chunk => trackOrder[chunk.Track])
                .ToList();

            long offset = 0;
            foreach (var chunk in ordered)
            {
                chunk.Track.ChunkSampleCounts.Add((uint)chunk.SampleCount);
                chunk.Track.ChunkOffsets.Add(offset);
                for (int i = 0; i < chunk.SampleCount; i++)
                    offset += chunk.Track.SampleSizes[chunk.FirstSample + i];
            }

            return ordered;
        }

        private MovieBox BuildMoov(bool largeOffsets, ulong creationTime)
        {
            var moov = new MovieBox();

            var mvhd = new MovieHeaderBox();
            mvhd.SetParent(moov);
            moov.Children = new List<Box>();
            moov.Children.Add(mvhd);

            ulong movieDuration = 0;

            mvhd.NextTrackID = 0xFFFFFFFF;
            mvhd.Timescale = MovieTimescale; // just for movie time: https://stackoverflow.com/questions/77803940/diffrence-between-mvhd-box-timescale-and-mdhd-box-timescale-in-isobmff-format
            mvhd.CreationTime = creationTime;
            mvhd.ModificationTime = creationTime;
            mvhd.Reserved0 = new uint[2]; // TODO simplify API
            mvhd.PreDefined = new uint[6]; // TODO simplify API

            foreach (var track in _trackContexts.Values)
            {
                var trak = new TrackBox();
                trak.SetParent(moov);
                trak.Children = new List<Box>();
                moov.Children.Add(trak);

                // Version 0 of 'ctts' carries unsigned offsets, so where any is negative they are all shifted by the most
                // negative one, unless they are to be written as they are. The composition times move with them, which the
                // edit list below takes back out.
                int bias = NegativeCompositionOffsets == NegativeCompositionOffsets.Shifted
                    ? Math.Min(0, track.CompositionOffsets.Count > 0 ? track.CompositionOffsets.Min() : 0)
                    : 0;

                // The earliest composition time: where the presentation starts in the media, which the edit list maps to
                // the start of the movie, as ffmpeg does. Without it, the first picture of a stream with B pictures is shown
                // as late as it is reordered by, and the track is out of step with the others.
                long firstCompositionTime = 0;
                {
                    long decodeTime = 0;
                    for (int i = 0; i < track.SampleSizes.Count; i++)
                    {
                        long compositionTime = decodeTime + track.CompositionOffsets[i] - bias;
                        if (i == 0 || compositionTime < firstCompositionTime)
                            firstCompositionTime = compositionTime;
                        decodeTime += track.SampleDurations[i];
                    }
                }

                // Of the track's own duration: each track ends where its samples do, not where the longest track does.
                ulong trackDuration = MovieTime.Rescale(track.Duration, track.Track.Timescale, MovieTimescale);
                movieDuration = Math.Max(movieDuration, trackDuration);

                var tkhd = new TrackHeaderBox();
                tkhd.SetParent(trak);
                trak.Children.Add(tkhd);
                tkhd.TrackID = track.Track.TrackID;
                tkhd.Reserved1 = new uint[2]; // TODO simplify API
                if (track.Track.HandlerType == HandlerTypes.Video)
                {
                    // 0x1 - track enabled, 0x2 - track is used in the movie, 0x4 - track is used in movie's preview, 0x8 - track is used in movie poster
                    tkhd.Flags = 0x01 | 0x02 | 0x04 | 0x08;
                }
                else
                {
                    tkhd.Flags = 0x01 | 0x02;
                }
                track.Track.FillTkhdBox(tkhd);
                tkhd.Duration = trackDuration;
                tkhd.CreationTime = creationTime;
                tkhd.ModificationTime = creationTime;
                tkhd.Version = MovieTime.Version(tkhd.Duration, creationTime);

                // Apple writes an edit list from the media's start even where it starts where the media does; a
                // presentation starting before the media's start - an offset as it is, of the first picture shown - cannot
                // be said, and starts at it.
                bool asApple = NegativeCompositionOffsets == NegativeCompositionOffsets.AsApple;
                firstCompositionTime = Math.Max(0, firstCompositionTime);
                if (firstCompositionTime != 0 || (asApple && track.Track.HandlerType == HandlerTypes.Video))
                {
                    var edts = new EditBox();
                    edts.SetParent(trak);
                    edts.Children = new List<Box>();
                    trak.Children.Add(edts);

                    var elst = new EditListBox();
                    elst.SetParent(edts);
                    edts.Children.Add(elst);
                    elst.EntryCount = 1;
                    elst.EditDuration = new ulong[] { trackDuration };
                    elst.MediaTime = new long[] { firstCompositionTime };
                    elst.MediaRateInteger = new short[] { 1 };
                    elst.MediaRateFraction = new short[] { 0 };
                    elst.Version = MovieTime.Version(trackDuration, (ulong)firstCompositionTime);
                }

                var mdia = new MediaBox();
                mdia.SetParent(trak);
                trak.Children.Add(mdia);

                // what is said of the track - of a forced subtitle track, its 'kind' - after its media (14496-12 8.10.1)
                var udta = SubtitleTrackBase.CreateUserDataBox(track.Track);
                if (udta != null)
                {
                    udta.SetParent(trak);
                    trak.Children.Add(udta);
                }

                MediaHeaderBox mdhd = new MediaHeaderBox();
                mdhd.SetParent(mdia);
                mdia.Children = new List<Box>();
                mdia.Children.Add(mdhd);
                mdhd.Duration = track.Duration;
                mdhd.Timescale = track.Track.Timescale;
                mdhd.Language = track.Track.Language;
                mdhd.CreationTime = creationTime;
                mdhd.ModificationTime = creationTime;
                mdhd.Version = MovieTime.Version(mdhd.Duration, creationTime);

                HandlerBox hdlr = new HandlerBox();
                hdlr.SetParent(mdia);
                mdia.Children.Add(hdlr);
                hdlr.HandlerType = IsoStream.FromFourCC(track.Track.HandlerType);
                hdlr.Name = new BinaryUTF8String(track.Track.HandlerName);
                hdlr.Reserved = new uint[3]; // TODO simplify API

                MediaInformationBox minf = new MediaInformationBox();
                minf.SetParent(mdia);
                mdia.Children.Add(minf);
                minf.Children = new List<Box>();

                switch (track.Track.HandlerType)
                {
                    case HandlerTypes.Video:
                        {
                            var vmhd = new VideoMediaHeaderBox();
                            vmhd.SetParent(mdia);
                            minf.Children.Add(vmhd);
                        }
                        break;

                    case HandlerTypes.Sound:
                        {
                            var smhd = new SoundMediaHeaderBox();
                            smhd.SetParent(mdia);
                            minf.Children.Add(smhd);
                        }
                        break;

                    case HandlerTypes.Hint:
                        {
                            var nmhd = new NullMediaHeaderBox();
                            nmhd.SetParent(mdia);
                            minf.Children.Add(nmhd);
                        }
                        break;

                    case HandlerTypes.Subtitle:
                        {
                            // subtitles, as TTML's (14496-12 12.6.2)
                            var sthd = new SubtitleMediaHeaderBox();
                            sthd.SetParent(minf);
                            minf.Children.Add(sthd);
                        }
                        break;

                    case HandlerTypes.Text:
                    case HandlerTypes.AppleSubtitle:
                        {
                            // timed text, WebVTT's and 3GPP's, has no media header of its own (14496-12 12.5.2)
                            var nmhd = new NullMediaHeaderBox();
                            nmhd.SetParent(minf);
                            minf.Children.Add(nmhd);
                        }
                        break;

                    default:
                        throw new NotSupportedException(track.Track.HandlerType);
                }

                var dinf = new DataInformationBox();
                dinf.SetParent(minf);
                minf.Children.Add(dinf);
                dinf.Children = new List<Box>();

                var dref = new DataReferenceBox();
                dref.SetParent(dinf);
                dinf.Children.Add(dref);
                dref.Children = new List<Box>();
                dref.EntryCount = 1;

                var url = new DataEntryUrlBox();
                url.Flags = 1;
                url.SetParent(dref);
                dref.Children.Add(url);

                var stbl = new SampleTableBox();
                stbl.SetParent(minf);
                minf.Children.Add(stbl);
                stbl.Children = new List<Box>();

                var stsd = new SampleDescriptionBox();
                stsd.SetParent(stbl);
                stbl.Children.Add(stsd);
                stsd.Children = new List<Box>();

                var sampleEntryBox = track.Track.CreateSampleEntryBox();
                track.Encryptor?.ProtectSampleEntry(track.Track, sampleEntryBox);
                sampleEntryBox.SetParent(stsd);
                stsd.Children.Add(sampleEntryBox);
                stsd.EntryCount = 1;

                // Run-length encode the real per-sample durations. Averaging them into a single
                // entry only holds for constant frame rate content, and loses the track duration
                // whenever samples differ in length.
                var sttsCounts = new List<uint>();
                var sttsDeltas = new List<uint>();
                foreach (var duration in track.SampleDurations)
                {
                    if (sttsDeltas.Count > 0 && sttsDeltas[sttsDeltas.Count - 1] == duration)
                        sttsCounts[sttsCounts.Count - 1]++;
                    else
                    {
                        sttsCounts.Add(1);
                        sttsDeltas.Add(duration);
                    }
                }

                var stts = new TimeToSampleBox();
                stts.SetParent(stbl);
                stbl.Children.Add(stts);
                stts.SampleCount = sttsCounts.ToArray();
                stts.SampleDelta = sttsDeltas.ToArray();
                stts.EntryCount = (uint)sttsCounts.Count;

                if (track.Track.HandlerType == HandlerTypes.Video)
                {
                    // this box is optional, but it allows for seeking without picture artifacts
                    var stss = new SyncSampleBox();
                    stss.SetParent(stbl);
                    stbl.Children.Add(stss);
                    stss.SampleNumber = track.RandomAccessPoints.ToArray();
                    stss.EntryCount = stss.SampleNumber != null ? (uint)stss.SampleNumber.Length : 0;
                }

                if (track.CompositionOffsets.Any(offset => offset - bias != 0))
                {
                    // Pictures are coded out of presentation order, so the difference between
                    // composition and decode time has to be recorded: in version 1, signed, or in version 0 - of
                    // offsets shifted to be positive, or as Apple writes them, a negative one in two's complement.
                    bool signed = NegativeCompositionOffsets == NegativeCompositionOffsets.Version1;
                    var ctts = new CompositionOffsetBox(signed ? (byte)1 : (byte)0);
                    ctts.SetParent(stbl);
                    stbl.Children.Add(ctts);

                    var counts = new List<uint>();
                    var offsets = new List<int>();
                    foreach (var offset in track.CompositionOffsets)
                    {
                        int value = offset - bias;
                        if (offsets.Count > 0 && offsets[offsets.Count - 1] == value)
                            counts[counts.Count - 1]++;
                        else
                        {
                            counts.Add(1);
                            offsets.Add(value);
                        }
                    }

                    ctts.SampleCount = counts.ToArray();
                    if (signed)
                        ctts.SampleOffset0 = offsets.ToArray();
                    else
                        ctts.SampleOffset = offsets.Select(offset => unchecked((uint)offset)).ToArray();
                    ctts.EntryCount = (uint)counts.Count;
                }

                // The samples are in chunks, each of the track's samples of about ChunkDurationInMs, between those of the
                // other tracks: an entry where the number of samples in a chunk changes.
                var firstChunks = new List<uint>();
                var samplesPerChunk = new List<uint>();
                for (int i = 0; i < track.ChunkSampleCounts.Count; i++)
                {
                    if (samplesPerChunk.Count == 0 || samplesPerChunk[samplesPerChunk.Count - 1] != track.ChunkSampleCounts[i])
                    {
                        firstChunks.Add((uint)i + 1);
                        samplesPerChunk.Add(track.ChunkSampleCounts[i]);
                    }
                }

                var stsc = new SampleToChunkBox();
                stsc.SetParent(stbl);
                stbl.Children.Add(stsc);
                stsc.FirstChunk = firstChunks.ToArray();
                stsc.SamplesPerChunk = samplesPerChunk.ToArray();
                stsc.SampleDescriptionIndex = firstChunks.Select(_ => 1u).ToArray();
                stsc.EntryCount = (uint)stsc.FirstChunk.Length;

                var stsz = new SampleSizeBox();
                stsz.SetParent(stbl);
                stbl.Children.Add(stsz);
                // of PCM, whose frames are all of a size, that size alone, as QuickTime writes it: not an entry of each frame
                if (track.Track is PcmTrack && track.SampleSizes.Count > 0 && track.SampleSizes.All(size => size == track.SampleSizes[0]))
                    stsz.SampleSize = track.SampleSizes[0];
                else
                    stsz.EntrySize = track.SampleSizes.ToArray();
                stsz.SampleCount = (uint)track.SampleSizes.Count;

                // Offsets are still relative to the start of the media data here; the header size
                // is added once it is known. Past 4 GB they no longer fit in a 32-bit stco, and
                // truncating them would silently produce a file that cannot be played, so switch
                // to the 64-bit co64 instead. largeOffsets is decided in FinalizeMedia, which is
                // the first point at which the header size is known.
                if (largeOffsets)
                {
                    var co64 = new ChunkLargeOffsetBox();
                    co64.SetParent(stbl);
                    stbl.Children.Add(co64);
                    co64.ChunkOffset = track.ChunkOffsets.Select(offset => (ulong)offset).ToArray();
                    co64.EntryCount = (uint)track.ChunkOffsets.Count;
                }
                else
                {
                    var stco = new ChunkOffsetBox();
                    stco.SetParent(stbl);
                    stbl.Children.Add(stco);
                    stco.ChunkOffset = track.ChunkOffsets.Select(offset => (uint)offset).ToArray();
                    stco.EntryCount = (uint)track.ChunkOffsets.Count;
                }

                // how each sample is protected: in a 'senc' of the 'trak' (23001-7, 7.2.1), its samples the auxiliary
                // information the 'saiz' and 'saio' of the 'stbl' give, once FinalizeMedia knows where the 'senc' lies
                track.Senc = null;
                track.Saio = null;
                if (track.Encryptor != null)
                {
                    var (senc, saiz, saio) = track.Encryptor.CreateSampleEncryptionBoxes(track.Encryptions, track.SampleSizes);
                    foreach (var box in new Box[] { saiz, saio }.Where(x => x != null))
                    {
                        box.SetParent(stbl);
                        stbl.Children.Add(box);
                    }
                    senc.SetParent(trak);
                    trak.Children.Add(senc);
                    track.Senc = senc;
                    track.Saio = saio;
                }
            }

            mvhd.Duration = movieDuration;
            mvhd.Version = MovieTime.Version(movieDuration, creationTime);

            // the protection systems' headers, each once, of all the protected tracks
            foreach (var pssh in TrackEncryptor.ProtectionSystemHeaders(_trackContexts.Values.Where(x => x.Encryptor != null).Select(x => x.Encryptor.Protection)))
            {
                pssh.SetParent(moov);
                moov.Children.Add(pssh);
            }

            return moov;
        }

        /// <summary>The same, for a sample that sits inside a larger buffer.</summary>
        public void ProcessTrackSample(uint trackID, ArraySegment<byte> sample, int sampleDuration = -1, int compositionOffset = 0) =>
            ProcessTrackSample(GetTrack(trackID), sample, sampleDuration, compositionOffset);

        /// <summary>
        /// Appends a sample, recording how far its composition time sits from its decode time.
        /// </summary>
        /// <param name="sampleDuration">The duration of the sample - of the access unit the NAL unit is of, for video given a NAL unit at a time - in the track's timescale; negative for the track's default.</param>
        /// <param name="compositionOffset">
        /// Composition time minus decode time, in track timescale units. Needed whenever pictures
        /// are coded out of presentation order; leave at 0 for streams that are not reordered.
        /// </param>
        public void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration = -1, int compositionOffset = 0) =>
            ProcessTrackSample(GetTrack(trackID), sample == null ? default : new ArraySegment<byte>(sample), sampleDuration, compositionOffset);

        private void ProcessTrackSample(TrackContext track, ArraySegment<byte> sample, int sampleDuration, int compositionOffset)
        {
            track.Track.ProcessSample(sample.Array, sample.Offset, sample.Count, out var processedSample, out var isRandomAccessPoint);

            // A track that gives back the access unit before the one it was given gives it the timing given with it.
            track.Timing.Resolve(track.Track, sample.Array == null, processedSample.Array != null, ref sampleDuration, ref compositionOffset);

            if (processedSample.Array != null)
            {
                foreach (var (frame, duration) in PcmFrames.SamplesOf(track.Track, processedSample, sampleDuration))
                    WriteSample(track, frame, duration, isRandomAccessPoint, compositionOffset);
            }
        }

        /// <inheritdoc/>
        public void ProcessAnnexBTrackSample(uint trackID, byte[] sample, int sampleDuration = -1, int compositionOffset = 0) =>
            ProcessAnnexBTrackSample(trackID, new ArraySegment<byte>(sample), sampleDuration, compositionOffset);

        /// <inheritdoc/>
        public void ProcessAnnexBTrackSample(uint trackID, ArraySegment<byte> sample, int sampleDuration = -1, int compositionOffset = 0)
        {
            foreach (var nalUnit in Utils.NalUnits(GetTrack(trackID).Track, trackID, sample))
                ProcessTrackSample(trackID, nalUnit, sampleDuration, compositionOffset);
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint)
        {
            WriteSample(GetTrack(trackID), new ArraySegment<byte>(sample), sampleDuration, isRandomAccessPoint);
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset)
        {
            WriteSample(GetTrack(trackID), new ArraySegment<byte>(sample), sampleDuration, isRandomAccessPoint, compositionOffset);
        }

        /// <summary>
        /// The same, for a sample that sits inside a larger buffer - one a caller fills again for
        /// each sample - so it does not have to be copied out into an array of its own first.
        /// </summary>
        public void ProcessRawSample(uint trackID, ArraySegment<byte> sample, int sampleDuration,
            bool isRandomAccessPoint, int compositionOffset = 0)
        {
            WriteSample(GetTrack(trackID), sample, sampleDuration, isRandomAccessPoint, compositionOffset);
        }

        /// <summary>
        /// Writes the MP4: the samples each track still holds, then the 'ftyp', the 'moov' and the samples, interleaved, in
        /// the 'mdat'. A builder given no samples writes a 'moov' of tracks that have none. It is called once.
        /// </summary>
        public void FinalizeMedia()
        {
            ThrowIfFinished();

            // the access unit each track that gives back the previous one is still assembling
            foreach (var track in _trackContexts.Values)
            {
                ProcessTrackSample(track, default, -1, 0);
            }
            _isFinalized = true;

            var mp4 = new Container();

            var ftyp = FileBrands.Create(FileFormat, _trackContexts.Values.Select(x => x.Track).ToList(), fragmented: false,
                isProtected: _trackContexts.Values.Any(x => x.Encryptor != null),
                hasSubtitleMediaHeader: _trackContexts.Values.Any(x => x.Track.HandlerType == HandlerTypes.Subtitle), Logger);
            ftyp.SetParent(mp4);
            mp4.Children.Add(ftyp);

            var chunks = Interleave();
            long mediaLength = _trackContexts.Values.Sum(track => track.SampleSizes.Sum(size => (long)size));

            // the samples, interleaved, as the 'mdat's data: its header - a largesize past 4 GB - as the box is written
            using var media = new IsoStream(new InterleavedStorage(RunsOf(chunks), Logger));
            var mdat = new MediaDataBox { Data = new StreamMarker(0, mediaLength, media) };
            long mdatHeaderSize = (long)(IsoStream.CalculateBoxSize(mdat) >> 3) - mediaLength;

            ulong creationTime = MovieTime.ToIsoTime(CreationTime ?? DateTime.UtcNow);

            // create moov at the beginning of the file (faststart, allowing to play video early while still streaming)
            // The offsets are relative to the media data until the header size is known, and the
            // header size depends on which offset box is used. Size it with 32-bit offsets first,
            // then redo it with 64-bit ones if the real offsets would not fit.
            bool largeOffsets = false;
            MovieBox moov;
            long mdatOffset;
            while (true)
            {
                moov = BuildMoov(largeOffsets, creationTime);
                mdatOffset = (long)((ftyp.CalculateSize() + moov.CalculateSize()) >> 3) + mdatHeaderSize;

                long highestOffset = 0;
                foreach (var track in _trackContexts.Values)
                    if (track.ChunkOffsets.Count > 0)
                        highestOffset = Math.Max(highestOffset, track.ChunkOffsets[track.ChunkOffsets.Count - 1]);

                if (largeOffsets || highestOffset + mdatOffset <= uint.MaxValue)
                    break;

                largeOffsets = true;
            }

            moov.SetParent(mp4);
            mp4.Children.Add(moov);

            // a builder given no samples writes no 'mdat'
            if (mediaLength > 0)
            {
                mdat.SetParent(mp4);
                mp4.Children.Add(mdat);
            }

            moov.ModifyChunkOffsets(mdatOffset);

            // each 'saio' at its 'senc's samples, by where they lie in the file: the 'moov' comes right after the 'ftyp'
            foreach (var track in _trackContexts.Values.Where(x => x.Saio != null))
            {
                var trak = (Box)track.Senc.GetParent();
                ulong offset = (ftyp.CalculateSize() >> 3) + 8;
                offset += moov.Children.TakeWhile(x => x != trak).Aggregate(0ul, (total, box) => total + (box.CalculateSize() >> 3)) + 8;
                offset += trak.Children.TakeWhile(x => x != track.Senc).Aggregate(0ul, (total, box) => total + (box.CalculateSize() >> 3));
                track.Saio.Offset[0] = offset + 12 + 4; // the 'senc's header, version and flags, then its sample_count
            }

            var stream = _output.GetStream(0);
            var outputStream = new IsoStream(stream);
            mp4.Write(outputStream);

            _output.Flush(stream, 0);

            // the samples are in the file now
            DisposeStorages();
        }

        /// <summary>The bytes of the chunks, in order: of each run of samples one after another in a storage, one run.</summary>
        private static List<InterleavedStorage.Run> RunsOf(List<Chunk> chunks)
        {
            var runs = new List<InterleavedStorage.Run>();
            foreach (var chunk in chunks)
            {
                var track = chunk.Track;
                int i = chunk.FirstSample;
                int end = chunk.FirstSample + chunk.SampleCount;
                while (i < end)
                {
                    long start = track.SampleOffsets[i];
                    long length = track.SampleSizes[i++];
                    while (i < end && track.SampleOffsets[i] == start + length)
                        length += track.SampleSizes[i++];

                    if (length > 0)
                        runs.Add(new InterleavedStorage.Run(track.Storage, start, length));
                }
            }
            return runs;
        }

        private void DisposeStorages()
        {
            foreach (var track in _trackContexts.Values)
            {
                if (track.OwnsStorage)
                    track.Storage?.Dispose();
                track.Storage = null;
                track.OwnsStorage = false;
            }
        }

        /// <summary>
        /// Lets go of where the samples wait. Without <see cref="FinalizeMedia"/> before it nothing is written: the samples
        /// are dropped, as an MP4 is of no use without its 'moov'.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                DisposeStorages();
            _isDisposed = true;
        }
    }
}
