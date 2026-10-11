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
    public class Mp4Builder : Mp4BuilderBase
    {
        private class TrackContext : BuilderTrack
        {
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

            /// <summary>Of the file being written: the number of samples in each of the track's chunks, and where each starts in the media data.</summary>
            public List<uint> ChunkSampleCounts { get; } = new List<uint>();
            public List<long> ChunkOffsets { get; } = new List<long>();

            public TrackContext(ITrack track) : base(track)
            {
            }

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

        /// <summary>
        /// How composition offsets are written where any is negative: shifted, the default, as Apple writes them, or in
        /// version 1 of 'ctts'. See <see cref="SharpMP4.Builders.NegativeCompositionOffsets"/>.
        /// </summary>
        public NegativeCompositionOffsets NegativeCompositionOffsets { get; set; } = NegativeCompositionOffsets.Shifted;

        /// <summary>
        /// How long the samples of a track run in one chunk before the next track's samples of the same time follow them,
        /// in milliseconds. A player reads the tracks' samples of the same time from near each other, and a file written as
        /// it arrives - a remuxer's, one track after another - would make it seek from one end of the file to the other.
        /// </summary>
        public uint ChunkDurationInMs { get; set; } = 500;

        private IStorage _storage;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="output">Output stream, written at <see cref="Mp4BuilderBase.FinalizeMedia"/>. <see cref="IMp4Output"/>.</param>
        public Mp4Builder(IMp4Output output) : base(output)
        {
        }

        /// <summary>
        /// One storage all the tracks' samples wait in until <see cref="Mp4BuilderBase.FinalizeMedia"/> writes them,
        /// interleaved; it stays the caller's to dispose. Where it is not set, each track has one of its own from
        /// <see cref="Mp4BuilderBase.TemporaryStorageFactory"/>.
        /// </summary>
        public IStorage Storage
        {
            get => _storage;
            set => _storage = value;
        }

        private protected override string FinalizedMessage => "The MP4 was already finalized: FinalizeMedia has written it, and it takes nothing more.";

        private IEnumerable<TrackContext> TrackContexts => Tracks.Cast<TrackContext>();

        private protected override BuilderTrack CreateTrack(ITrack track) => new TrackContext(track);

        private protected override void AppendSample(BuilderTrack builderTrack, ArraySegment<byte> sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset)
        {
            var track = (TrackContext)builderTrack;
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
            foreach (var track in TrackContexts)
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
            var trackOrder = TrackContexts.Select((track, index) => (track, index)).ToDictionary(x => x.track, x => x.index);
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
            var mvhd = AddMovieHeader(moov, creationTime);

            ulong movieDuration = 0;

            foreach (var track in TrackContexts)
            {
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

                // 0x1 - track enabled, 0x2 - track is used in the movie, 0x4 - track is used in movie's preview, 0x8 - track is used in movie poster
                uint tkhdFlags = track.Track.HandlerType == HandlerTypes.Video ? 0x01u | 0x02 | 0x04 | 0x08 : 0x01u | 0x02;

                // Apple writes an edit list from the media's start even where it starts where the media does; a
                // presentation starting before the media's start - an offset as it is, of the first picture shown - cannot
                // be said, and starts at it.
                bool asApple = NegativeCompositionOffsets == NegativeCompositionOffsets.AsApple;
                firstCompositionTime = Math.Max(0, firstCompositionTime);
                (ulong, long)? edit = firstCompositionTime != 0 || (asApple && track.Track.HandlerType == HandlerTypes.Video)
                    ? (trackDuration, firstCompositionTime)
                    : null;

                var stbl = AddTrackBox(moov, track, tkhdFlags, trackDuration, track.Duration, creationTime, edit, out var trak);

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

                    // Of offsets written negative, how far they run, and the composition times they make: Apple's readers
                    // take the shift that makes every composition time come after its decode time from the 'cslg'
                    // (14496-12 8.6.1.4), as Apple writes it with them.
                    int least = offsets.Min();
                    if (least < 0)
                    {
                        long start = long.MaxValue, end = long.MinValue, decodeTime = 0;
                        for (int i = 0; i < track.SampleSizes.Count; i++)
                        {
                            long compositionTime = decodeTime + track.CompositionOffsets[i] - bias;
                            start = Math.Min(start, compositionTime);
                            end = Math.Max(end, compositionTime + track.SampleDurations[i]);
                            decodeTime += track.SampleDurations[i];
                        }

                        int greatest = offsets.Max();
                        bool large = start < int.MinValue || end > int.MaxValue;
                        var cslg = new CompositionToDecodeBox(large ? (byte)1 : (byte)0)
                        {
                            CompositionToDTSShift = -least,
                            LeastDecodeToDisplayDelta = least,
                            GreatestDecodeToDisplayDelta = greatest,
                            CompositionStartTime = start,
                            CompositionEndTime = end,
                        };
                        cslg.SetParent(stbl);
                        stbl.Children.Add(cslg);
                    }
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

            AddProtectionSystemHeaders(moov);

            return moov;
        }

        /// <summary>
        /// Writes the MP4: the 'ftyp', the 'moov' and the samples, interleaved, in the 'mdat'. A builder given no samples
        /// writes a 'moov' of tracks that have none.
        /// </summary>
        private protected override void WriteMedia()
        {
            IsFinalized = true;

            var mp4 = new Container();

            var ftyp = CreateFileType(fragmented: false);
            ftyp.SetParent(mp4);
            mp4.Children.Add(ftyp);

            var chunks = Interleave();
            long mediaLength = TrackContexts.Sum(track => track.SampleSizes.Sum(size => (long)size));

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
                foreach (var track in TrackContexts)
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
            foreach (var track in TrackContexts.Where(x => x.Saio != null))
            {
                var trak = (Box)track.Senc.GetParent();
                ulong offset = (ftyp.CalculateSize() >> 3) + 8;
                offset += moov.Children.TakeWhile(x => x != trak).Aggregate(0ul, (total, box) => total + (box.CalculateSize() >> 3)) + 8;
                offset += trak.Children.TakeWhile(x => x != track.Senc).Aggregate(0ul, (total, box) => total + (box.CalculateSize() >> 3));
                track.Saio.Offset[0] = offset + 12 + 4; // the 'senc's header, version and flags, then its sample_count
            }

            var stream = Output.GetStream(0);
            var outputStream = new IsoStream(stream);
            mp4.Write(outputStream);

            Output.Flush(stream, 0);

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

        /// <summary>
        /// Lets go of where the samples wait. Without <see cref="Mp4BuilderBase.FinalizeMedia"/> before it nothing is
        /// written: the samples are dropped, as an MP4 is of no use without its 'moov'.
        /// </summary>
        private protected override void DisposeStorages()
        {
            foreach (var track in TrackContexts)
            {
                if (track.OwnsStorage)
                    track.Storage?.Dispose();
                track.Storage = null;
                track.OwnsStorage = false;
            }
        }
    }
}
