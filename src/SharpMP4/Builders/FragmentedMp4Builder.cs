using SharpISOBMFF;
using SharpMP4.Common;
using SharpMP4.Encryption;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Builders
{
    /// <summary>
    /// Creates Fragmented MP4.
    /// </summary>
    public class FragmentedMp4Builder : IMp4Builder, IDisposable
    {
        private class MediaFragment
        {
            public MediaFragment(IStorage storage, ulong startTime, ulong endTime, uint[] sampleSizes, uint[] sampleDurations, int[] compositionOffsets, bool[] randomAccessPoints, SampleEncryption[] encryptions = null)
            {
                this.RandomAccessPoints = randomAccessPoints;
                this.Encryptions = encryptions;
                this.Storage = storage;
                this.StartTime = startTime;
                this.EndTime = endTime;
                this.SampleSizes = sampleSizes;
                this.SampleDurations = sampleDurations;
                this.CompositionOffsets = compositionOffsets;
            }

            public ulong StartTime { get; set; }
            public ulong EndTime { get; set; }
            public uint[] SampleSizes { get; set; }
            public uint[] SampleDurations { get; set; }

            /// <summary>Composition time minus decode time, per sample.</summary>
            public int[] CompositionOffsets { get; set; }

            /// <summary>Whether each sample is a random access point: a sync sample.</summary>
            public bool[] RandomAccessPoints { get; set; }

            public IStorage Storage { get; set; }

            /// <summary>Where the fragment's samples start in its storage: after those of fragments split off before it.</summary>
            public long Offset { get; set; }

            /// <summary>The size of the fragment's samples, one after another in its storage.</summary>
            public long Length => SampleSizes.Sum(size => (long)size);

            /// <summary>Of a protected track: how each sample is protected.</summary>
            public SampleEncryption[] Encryptions { get; set; }
        }

        /// <summary>An entry of a track's 'tfra': where a fragment with a sync sample is, and which sample of it that is.</summary>
        private struct RandomAccessEntry
        {
            public ulong Time;
            public ulong MoofOffset;
            public uint SampleNumber;
        }

        private class TrackContext
        {
            public TrackContext(ITrack track)
            {
                Track = track;
            }

            public ITrack Track { get; set; }

            public ulong StartTime { get; set; }
            public ulong EndTime { get; set; }
            public List<uint> SampleSizes { get; set; } = new List<uint>();
            public List<uint> SampleDurations { get; set; } = new List<uint>();

            /// <summary>Composition time minus decode time, per sample. Non-zero for B pictures.</summary>
            public List<int> CompositionOffsets { get; set; } = new List<int>();

            /// <summary>Whether each sample is a random access point.</summary>
            public List<bool> RandomAccessPoints { get; set; } = new List<bool>();

            public uint FragmentCounts { get; set; }

            /// <summary>
            /// Where the samples not yet in a fragment are written, and those of the fragments split off them waiting to be
            /// written; null until the first of a new fragment comes. A storage is disposed once its last fragment is written.
            /// </summary>
            public IStorage CurrentFragments { get; set; }
            public Queue<MediaFragment> ReadyFragments { get; set; } = new Queue<MediaFragment>();

            /// <summary>The 'tfra' entries of the fragments written.</summary>
            public List<RandomAccessEntry> RandomAccessEntries { get; } = new List<RandomAccessEntry>();

            /// <summary>What protects the track; null where it is not protected.</summary>
            public TrackEncryptor Encryptor { get; set; }

            /// <summary>How the track is protected; null where it is not.</summary>
            public TrackProtection Protection => Encryptor?.Protection;

            /// <summary>How each sample of the fragment being assembled is protected.</summary>
            public List<SampleEncryption> Encryptions { get; set; } = new List<SampleEncryption>();

            /// <summary>Of a track cut where the video is: the next of the video's fragment boundaries it is to be cut at.</summary>
            public int NextBoundary { get; set; }

            /// <summary>Where the samples not yet in a fragment start in <see cref="CurrentFragments"/>.</summary>
            public long CurrentOffset { get; set; }

            /// <summary>The timing given for the sample the track is assembling.</summary>
            public PendingSampleTiming Timing { get; } = new PendingSampleTiming();

            /// <summary>Whether the track has had a sample: until it has, it may have one of any time, and its sample entry may not be known.</summary>
            public bool HasSamples { get; set; }
        }

        /// <summary>The flags of a sync sample: one that depends on no other (14496-12 8.8.3.1).</summary>
        private static readonly uint SyncSampleFlags = new SampleFlags() { SampleDependsOn = 2 };

        /// <summary>Of a time in a track's timescale, the seconds.</summary>
        private static double Seconds(TrackContext track, ulong time) => track.Track.Timescale == 0 ? 0 : (double)time / track.Track.Timescale;

        public uint MovieTimescale { get; set; } = 1000;

        /// <inheritdoc/>
        public Mp4FileFormat FileFormat { get; set; } = Mp4FileFormat.Mp4;

        /// <summary>Where the samples of each fragment wait until it is written. Temporary files by default.</summary>
        public ITemporaryStorageFactory TemporaryStorageFactory { get; set; } = new TemporaryFileStorageFactory();

        /// <summary>
        /// How far, in milliseconds, the fragments ready to be written may run past where a track that has none ready has
        /// got to before they are written without it. Fragments are written in the order of their time, so a fragment waits
        /// for the tracks that may yet have an earlier one; a track that falls silent - or is slow - would hold them all back,
        /// and nothing would be written until the end. Past this, the waiting track's samples are made a fragment of their
        /// own, and its later ones come after the others' of their time. Twice the fragment length by default.
        /// <see cref="ulong.MaxValue"/> waits however long it takes: for a remuxer that feeds one track after another, whose
        /// fragments otherwise come out of the order of their time.
        /// </summary>
        public ulong MaxFragmentDelayInMs { get; set; }

        /// <summary>
        /// The creation and modification time the movie, track and media headers say. Null - the default - for the time
        /// the initialization segment is written; set it for output that is the same each time it is written.
        /// </summary>
        public DateTime? CreationTime { get; set; }

        private readonly IMp4Output _output;

        private readonly ulong _maxFragmentLengthInMs;
        private readonly ulong _durationInMs = 0;
        private readonly bool _appendMovieFragmentRandomAccessBox;

        private readonly Dictionary<uint, TrackContext> _trackContexts = new Dictionary<uint, TrackContext>();

        private uint _moofSequenceNumber = 1;

        // Whether the initialization segment - the 'ftyp' and 'moov' - has been written: after it, no track can be added.
        private bool _isInitialized;
        private bool _isFinalized;
        private bool _isDisposed;

        // How many bytes have been written, of all the streams of the output one after another: where the next 'moof' is in
        // the file they make, as the 'tfra' gives it. Of a SingleStreamOutput, the file itself; of an output of a stream per
        // fragment, the file the streams make put together in the order of their sequence numbers.
        private ulong _bytesWritten;

        // Where the fragments of the leading video track start, but for the first, in its timescale: the other tracks' are
        // cut there too, so that each span of time has a fragment of each track, video first - as a player reading the
        // file a fragment at a time wants them. Cut on a clock of their own, every 2 s, against video cut at its key
        // frames every 2.67 s, no two tracks' fragments met, and VLC, which reads the fragments in order, reached each
        // video fragment only as its first pictures were due: they were late, and playback stuttered.
        private readonly List<ulong> _videoBoundaries = new List<ulong>();

        public IMp4Logger Logger { get; set; } = DefaultMp4Logger.Instance;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="output">Output stream. Will be progressively written while recording. <see cref="IMp4Output"/>.</param>
        /// <param name="maxFragmentLengthInMs">
        /// How long a fragment runs at least, in milliseconds: once it has, the next starts at the next sync sample of the
        /// video - of a track without video, at the next sample - so fragments are of about this length or longer.
        /// </param>
        /// <param name="durationInMs">Duration of the movie. Default value is 0 for live recordings.</param>
        /// <param name="appendMovieFragmentRandomAccessBox">Append Movie Fragment Random Access box at the end of the fragmented MP4.</param>
        public FragmentedMp4Builder(IMp4Output output, ulong maxFragmentLengthInMs, ulong durationInMs = 0, bool appendMovieFragmentRandomAccessBox = true)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            _maxFragmentLengthInMs = maxFragmentLengthInMs;
            _durationInMs = durationInMs;
            _appendMovieFragmentRandomAccessBox = appendMovieFragmentRandomAccessBox;
            MaxFragmentDelayInMs = maxFragmentLengthInMs > ulong.MaxValue / 2 ? ulong.MaxValue : 2 * maxFragmentLengthInMs;
        }

        /// <summary>
        /// Add a track to the Fragmented MP4.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        public void AddTrack(ITrack track)
        {
            if (track == null)
                throw new ArgumentNullException(nameof(track));
            ThrowIfFinished();
            if (_isInitialized)
                throw new InvalidOperationException("A track cannot be added once the initialization segment, which lists the tracks, has been written: add every track before the first fragment is complete.");

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
        /// of H.264, H.265 and H.266 video, its NAL units' slice headers left clear. The sample entry becomes 'encv' or
        /// 'enca', with a 'sinf' saying what it was and how it is protected, and each fragment says how its samples are
        /// in 'senc', 'saiz' and 'saio'. <see cref="TrackProtection.Create"/> makes a protection with a scheme's defaults.
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
                throw new ObjectDisposedException(nameof(FragmentedMp4Builder));
            if (_isFinalized)
                throw new InvalidOperationException("The fragmented MP4 was already finalized: FinalizeMedia has written its last fragment, and it takes nothing more.");
        }

        private TrackContext GetTrack(uint trackID)
        {
            ThrowIfFinished();
            if (!_trackContexts.TryGetValue(trackID, out var track))
                throw new ArgumentException($"There is no track {trackID}: a track is added with AddTrack, which gives it its ID.", nameof(trackID));
            return track;
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
            track.Track.ProcessSample(sample.Array, sample.Offset, sample.Count,
                out var processedSample, out var isRandomAccessPoint);

            // A track that gives back the access unit before the one it was given gives it the timing given with it.
            track.Timing.Resolve(track.Track, sample.Array == null, processedSample.Array != null, ref sampleDuration, ref compositionOffset);

            if (processedSample.Array != null)
            {
                AppendSample(track, processedSample, sampleDuration, isRandomAccessPoint, compositionOffset);
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
            AppendSample(GetTrack(trackID), new ArraySegment<byte>(sample), sampleDuration, isRandomAccessPoint, 0);
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset)
        {
            AppendSample(GetTrack(trackID), new ArraySegment<byte>(sample), sampleDuration, isRandomAccessPoint, compositionOffset);
        }

        /// <summary>
        /// The same, for a sample that sits inside a larger buffer - one a caller fills again for
        /// each sample - so it does not have to be copied out into an array of its own first.
        /// </summary>
        public void ProcessRawSample(uint trackID, ArraySegment<byte> sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset = 0)
        {
            AppendSample(GetTrack(trackID), sample, sampleDuration, isRandomAccessPoint, compositionOffset);
        }

        private void AppendSample(TrackContext track, ArraySegment<byte> sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset)
        {
            // A negative duration is the track's default; 0 is a sample of no duration, as a text track's can be.
            uint currentSampleDuration = sampleDuration < 0 ? (uint)track.Track.DefaultSampleDuration : (uint)sampleDuration;

            var leader = LeadingVideo();
            if (leader != null && leader != track)
            {
                CutAtVideoBoundaries(track, leader);
            }
            else if (StartsFragment(track, isRandomAccessPoint))
            {
                track.ReadyFragments.Enqueue(CreateNewFragment(track));
                track.FragmentCounts++;

                if (leader == track)
                {
                    // where the other tracks' fragments are cut too - of those already past it, as a track fed before the
                    // video is, what they have
                    _videoBoundaries.Add(track.StartTime);
                    foreach (var other in _trackContexts.Values)
                    {
                        if (other != track)
                            CutAtVideoBoundaries(other, leader);
                    }
                }
            }

            if (track.CurrentFragments == null)
            {
                track.CurrentFragments = TemporaryStorageFactory.Create(Logger);
                track.CurrentOffset = 0;
            }

            if (track.Encryptor != null)
            {
                sample = track.Encryptor.Protect(track.Track, sample, out var encryption);
                track.Encryptions.Add(encryption);
            }

            track.CurrentFragments.Write(sample.Array, sample.Offset, sample.Count);
            track.SampleSizes.Add((uint)sample.Count);
            track.SampleDurations.Add(currentSampleDuration);
            track.CompositionOffsets.Add(compositionOffset);
            track.RandomAccessPoints.Add(isRandomAccessPoint);
            track.EndTime += currentSampleDuration;
            track.HasSamples = true;

            WriteFragment();
        }

        /// <summary>The video track the others' fragments are cut with: the first; null where there is none.</summary>
        private TrackContext LeadingVideo()
        {
            return _trackContexts.Values.Where(t => t.Track.HandlerType == HandlerTypes.Video).OrderBy(t => t.Track.TrackID).FirstOrDefault();
        }

        /// <summary>
        /// Whether the sample about to be added to the leading video track - or to any track, where there is no video - starts
        /// a fragment: once the fragment has run its length, a video fragment at a random access point, where it can be
        /// decoded from, as a player seeking to it takes it (tfra).
        /// </summary>
        private bool StartsFragment(TrackContext track, bool isRandomAccessPoint)
        {
            if (track.SampleSizes.Count == 0)
                return false;

            ulong nextFragmentTime = track.Track.Timescale * _maxFragmentLengthInMs * (track.FragmentCounts + 1);
            ulong currentFragmentTime = track.EndTime * 1000;
            return nextFragmentTime <= currentFragmentTime && (isRandomAccessPoint || track.Track.HandlerType != HandlerTypes.Video);
        }

        /// <summary>
        /// A track other than the leading video's cut where the video's fragments start, as far as they are known: its samples
        /// not yet in a fragment split at the first to start at or past each boundary. A track fed before the video - a
        /// remuxer's, one track after another - is cut as the video's boundaries come; one fed after, as its samples do.
        /// </summary>
        private void CutAtVideoBoundaries(TrackContext track, TrackContext leader)
        {
            bool Reached(ulong time, int boundary) =>  (decimal)time * leader.Track.Timescale >= (decimal)_videoBoundaries[boundary] * track.Track.Timescale;

            while (track.NextBoundary < _videoBoundaries.Count && track.SampleSizes.Count > 0)
            {
                // the first sample at or past the boundary; after all of them, the next to come, which starts at the end
                int count = 0;
                ulong time = track.StartTime;
                while (count < track.SampleSizes.Count && !Reached(time, track.NextBoundary))
                {
                    time += track.SampleDurations[count++];
                }

                if (count == track.SampleSizes.Count && !Reached(time, track.NextBoundary))
                    return; // not there yet

                if (count > 0)
                {
                    SplitFragment(track, count);
                }

                track.NextBoundary++;
            }
        }

        /// <summary>The first samples of those not yet in a fragment made a fragment of their own, in the same storage.</summary>
        private static void SplitFragment(TrackContext track, int count)
        {
            ulong duration = 0;
            for (int i = 0; i < count; i++)
            {
                duration += track.SampleDurations[i];
            }

            var fragment = new MediaFragment(track.CurrentFragments, track.StartTime, track.StartTime + duration,
                track.SampleSizes.Take(count).ToArray(), track.SampleDurations.Take(count).ToArray(), track.CompositionOffsets.Take(count).ToArray(),
                track.RandomAccessPoints.Take(count).ToArray(), track.Protection != null ? track.Encryptions.Take(count).ToArray() : null)
            { 
                Offset = track.CurrentOffset 
            };

            track.ReadyFragments.Enqueue(fragment);

            track.CurrentOffset += fragment.Length;
            track.StartTime += duration;
            track.SampleSizes.RemoveRange(0, count);
            track.SampleDurations.RemoveRange(0, count);
            track.CompositionOffsets.RemoveRange(0, count);
            track.RandomAccessPoints.RemoveRange(0, count);

            if (track.Encryptions.Count >= count)
            {
                track.Encryptions.RemoveRange(0, count);
            }

            track.FragmentCounts++;
        }

        /// <summary>
        /// The samples not yet in a fragment made one: the next sample starts a fragment of its own, in a storage of its own.
        /// </summary>
        private static MediaFragment CreateNewFragment(TrackContext track)
        {
            var ret = new MediaFragment(
                track.CurrentFragments, 
                track.StartTime, 
                track.EndTime, 
                track.SampleSizes.ToArray(),
                track.SampleDurations.ToArray(),
                track.CompositionOffsets.ToArray(),
                track.RandomAccessPoints.ToArray(),
                track.Protection != null ? track.Encryptions.ToArray() : null) 
            { 
                Offset = track.CurrentOffset 
            };

            track.SampleSizes.Clear();
            track.SampleDurations.Clear();
            track.CompositionOffsets.Clear();
            track.RandomAccessPoints.Clear();
            track.Encryptions.Clear();
            track.StartTime = track.EndTime;
            track.CurrentFragments = null;
            track.CurrentOffset = 0;

            return ret;
        }


        private void WriteFragment(bool isFlushing = false)
        {
            if (isFlushing)
            {
                // what is left of each track: its last fragment
                foreach (var track in _trackContexts.Values)
                {
                    if (track.SampleSizes.Count > 0)
                    {
                        track.ReadyFragments.Enqueue(CreateNewFragment(track));
                    }
                }
            }

            // The fragments in the order of their time, whatever their lengths - a video fragment is a group of pictures,
            // an audio one the samples of its time - so that a player reading the file finds each track's samples near the
            // others' of the same time: written a fragment of each track in turn, tracks whose fragments are longer ran ahead
            // of the others, seconds apart by the end, and playback stalled.
            while (true)
            {
                TrackContext next = null;
                foreach (var track in _trackContexts.Values)
                {
                    if (track.ReadyFragments.Count > 0 && (next == null || Precedes(track, next)))
                    {
                        next = track;
                    }
                }

                if (next == null)
                    return;

                if (!isFlushing && !MayWrite(next, out bool retry))
                {
                    if (retry)
                    {
                        continue;
                    }
                    
                    return;
                }

                if (!_isInitialized)
                    WriteInitialization();

                var fragment = next.ReadyFragments.Dequeue();
                WriteTrackFragment(next, fragment);

                // the storage, once the last fragment in it is written
                if (fragment.Storage != null && fragment.Storage != next.CurrentFragments && !next.ReadyFragments.Any(x => x.Storage == fragment.Storage))
                    fragment.Storage.Dispose();
            }
        }

        /// <summary>
        /// Whether a track's next fragment - the earliest ready - is known to be the earliest there will be: where no other
        /// track can still have one earlier. A track without a fragment ready has its next start where its samples not yet in
        /// one start, or where its last ended; one that has had no sample yet - whose sample entry may not even be known, as
        /// the initialization segment written with the first fragment needs it - may have one at any time. A track waited on
        /// longer than <see cref="MaxFragmentDelayInMs"/> is waited on no more: what it has is made a fragment, which is then
        /// the earliest (<paramref name="retry"/>), or, where it has nothing, it is passed over.
        /// </summary>
        private bool MayWrite(TrackContext next, out bool retry)
        {
            retry = false;
            double start = Seconds(next, next.ReadyFragments.Peek().StartTime);
            double readyEnd = Seconds(next, next.ReadyFragments.Last().EndTime);

            foreach (var other in _trackContexts.Values)
            {
                if (other == next || other.ReadyFragments.Count > 0)
                    continue;

                // where its next fragment starts, of the order of two that start together (Precedes)
                double otherStart = Seconds(other, other.StartTime);
                if (other.HasSamples && (otherStart > start || (otherStart == start && Rank(other) >= Rank(next))))
                    continue;

                double behind = readyEnd - Seconds(other, other.EndTime);
                if (MaxFragmentDelayInMs == ulong.MaxValue || behind * 1000 <= MaxFragmentDelayInMs)
                    return false;

                if (Logger.IsWarningEnabled)
                    Logger.LogWarning($"Track {other.Track.TrackID} is {behind:0.00} s behind track {next.Track.TrackID}: the fragments ready are written without waiting for it");

                if (other.SampleSizes.Count > 0)
                {
                    other.ReadyFragments.Enqueue(CreateNewFragment(other));
                    retry = true;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether a track's next fragment goes before another's: the earlier, and of two starting together, video before
        /// audio before the rest - VLC requires the first 'moof' to be video, otherwise playback is choppy.
        /// </summary>
        private static bool Precedes(TrackContext track, TrackContext other)
        {
            double start = Seconds(track, track.ReadyFragments.Peek().StartTime);
            double otherStart = Seconds(other, other.ReadyFragments.Peek().StartTime);
            if (start != otherStart)
                return start < otherStart;
            return Rank(track) < Rank(other);
        }

        private static int Rank(TrackContext track) =>
            track.Track.HandlerType == HandlerTypes.Video ? 0 : track.Track.HandlerType == HandlerTypes.Sound ? 1 : 2;

        private void WriteInitialization()
        {
            Container init = new Container();
            CreateMediaInitialization(init);

            const uint initializationSegmentNumber = 0; // sequence ID 0 is used to indicate "initialization"
            var str = _output.GetStream(initializationSegmentNumber);
            IsoStream initializationStream = new IsoStream(str);
            _bytesWritten += init.Write(initializationStream) >> 3;
            _output.Flush(str, initializationSegmentNumber);
            _isInitialized = true;
        }

        private void WriteTrackFragment(TrackContext track, MediaFragment fragment)
        {
            Container fmp4 = new Container();
            uint sequenceNumber = _moofSequenceNumber++;

            CreateMediaFragment(fmp4, track, fragment, sequenceNumber);

            // where a player seeking finds the fragment: its first sync sample, at its presentation time (14496-12 8.8.10)
            ulong decodeTime = fragment.StartTime;
            for (int i = 0; i < fragment.SampleSizes.Length; i++)
            {
                if (fragment.RandomAccessPoints[i])
                {
                    long presentationTime = (long)decodeTime + fragment.CompositionOffsets[i];
                    track.RandomAccessEntries.Add(new RandomAccessEntry
                    {
                        Time = (ulong)Math.Max(0, presentationTime),
                        MoofOffset = _bytesWritten,
                        SampleNumber = (uint)i + 1,
                    });
                    break;
                }
                decodeTime += fragment.SampleDurations[i];
            }

            var outputStream = _output.GetStream(sequenceNumber);
            var fragmentStream = new IsoStream(outputStream);
            _bytesWritten += fmp4.Write(fragmentStream) >> 3;
            _output.Flush(outputStream, sequenceNumber);
        }

        /// <summary>
        /// Of the samples of a track known when the initialization segment is written - its first fragments - the earliest
        /// composition time: where its presentation starts, which the edit list maps to the start of the movie.
        /// </summary>
        private static long FirstCompositionTime(TrackContext track)
        {
            long first = long.MaxValue;
            foreach (var fragment in track.ReadyFragments)
            {
                long decodeTime = (long)fragment.StartTime;
                for (int i = 0; i < fragment.SampleSizes.Length; i++)
                {
                    first = Math.Min(first, decodeTime + fragment.CompositionOffsets[i]);
                    decodeTime += fragment.SampleDurations[i];
                }
            }
            {
                long decodeTime = (long)track.StartTime;
                for (int i = 0; i < track.SampleSizes.Count; i++)
                {
                    first = Math.Min(first, decodeTime + track.CompositionOffsets[i]);
                    decodeTime += track.SampleDurations[i];
                }
            }
            return first == long.MaxValue ? 0 : first;
        }

        private void CreateMediaInitialization(Container fmp4)
        {
            ulong creationTime = MovieTime.ToIsoTime(CreationTime ?? DateTime.UtcNow);
            ulong movieDuration = _durationInMs * MovieTimescale / 1000;

            var ftyp = FileBrands.Create(FileFormat, _trackContexts.Values.Select(x => x.Track).ToList(), fragmented: true,
                isProtected: _trackContexts.Values.Any(x => x.Encryptor != null),
                hasSubtitleMediaHeader: _trackContexts.Values.Any(x => x.Track.HandlerType == HandlerTypes.Subtitle), Logger);
            ftyp.SetParent(fmp4);
            fmp4.Children.Add(ftyp);

            var moov = new MovieBox();
            moov.SetParent(fmp4);
            fmp4.Children.Add(moov);

            var mvhd = new MovieHeaderBox();
            mvhd.SetParent(moov);
            moov.Children = new List<Box>();
            moov.Children.Add(mvhd);
            mvhd.Duration = movieDuration;
            mvhd.NextTrackID = 0xFFFFFFFF; // TODO simplify API
            mvhd.Timescale = MovieTimescale; // just for movie time: https://stackoverflow.com/questions/77803940/diffrence-between-mvhd-box-timescale-and-mdhd-box-timescale-in-isobmff-format
            mvhd.CreationTime = creationTime;
            mvhd.ModificationTime = creationTime;
            mvhd.Version = MovieTime.Version(movieDuration, creationTime);
            mvhd.Reserved0 = new uint[2]; // TODO simplify API
            mvhd.PreDefined = new uint[6]; // TODO simplify API

            foreach (var track in _trackContexts.Values)
            {
                var trak = new TrackBox();
                trak.SetParent(moov);
                trak.Children = new List<Box>();
                moov.Children.Add(trak);

                var tkhd = new TrackHeaderBox();
                tkhd.SetParent(trak);
                trak.Children.Add(tkhd);
                tkhd.TrackID = track.Track.TrackID;
                tkhd.Reserved1 = new uint[2]; // TODO simplify API
                tkhd.Flags = 0x07;
                track.Track.FillTkhdBox(tkhd);
                tkhd.Duration = movieDuration;
                tkhd.CreationTime = creationTime;
                tkhd.ModificationTime = creationTime;
                tkhd.Version = MovieTime.Version(movieDuration, creationTime);

                // Where the presentation starts in the media, mapped to the start of the movie, as in the non fragmented
                // file - of pictures coded out of order, the first is shown at 0, not as late as it is reordered by. The
                // edit runs to the end of the media: of a duration of 0, as ffmpeg writes it, where that is not known yet.
                long firstCompositionTime = FirstCompositionTime(track);
                if (firstCompositionTime > 0)
                {
                    var edts = new EditBox();
                    edts.SetParent(trak);
                    edts.Children = new List<Box>();
                    trak.Children.Add(edts);

                    ulong editDuration = movieDuration == 0 ? 0 : (ulong)Math.Max(0, (long)movieDuration - (long)MovieTime.Rescale((ulong)firstCompositionTime, track.Track.Timescale, MovieTimescale));
                    var elst = new EditListBox();
                    elst.SetParent(edts);
                    edts.Children.Add(elst);
                    elst.EntryCount = 1;
                    elst.EditDuration = new ulong[] { editDuration };
                    elst.MediaTime = new long[] { firstCompositionTime };
                    elst.MediaRateInteger = new short[] { 1 };
                    elst.MediaRateFraction = new short[] { 0 };
                    elst.Version = MovieTime.Version(editDuration, (ulong)firstCompositionTime);
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
                mdhd.Duration = 0; // the media is in the fragments
                mdhd.Timescale = track.Track.Timescale;
                mdhd.Language = track.Track.Language;
                mdhd.CreationTime = creationTime;
                mdhd.ModificationTime = creationTime;
                mdhd.Version = MovieTime.Version(creationTime);

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

                DataInformationBox dinf = new DataInformationBox();
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

                SampleTableBox stbl = new SampleTableBox();
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

                var stsz = new SampleSizeBox();
                stsz.SetParent(stbl);
                stbl.Children.Add(stsz);

                var stsc = new SampleToChunkBox();
                stsc.SetParent(stbl);
                stbl.Children.Add(stsc);

                var stts = new TimeToSampleBox();
                stts.SetParent(stbl);
                stbl.Children.Add(stts);

                var stco = new ChunkOffsetBox();
                stco.SetParent(stbl);
                stbl.Children.Add(stco);
            }

            // the protection systems' headers, each once, of all the protected tracks
            foreach (var pssh in TrackEncryptor.ProtectionSystemHeaders(_trackContexts.Values.Where(x => x.Protection != null).Select(x => x.Protection)))
            {
                pssh.SetParent(moov);
                moov.Children.Add(pssh);
            }

            var mvex = new MovieExtendsBox();
            mvex.SetParent(moov);
            moov.Children.Add(mvex);
            mvex.Children = new List<Box>();

            // optional
            if (_durationInMs != 0)
            {
                var mehd = new MovieExtendsHeaderBox();
                mehd.SetParent(mvex);
                mvex.Children.Add(mehd);
                mehd.FragmentDuration = movieDuration;
                mehd.Version = MovieTime.Version(movieDuration);
            }

            foreach (var track in _trackContexts.Values)
            {
                TrackExtendsBox trex = new TrackExtendsBox();
                trex.SetParent(mvex);
                mvex.Children.Add(trex);

                trex.TrackID = track.Track.TrackID;
                trex.DefaultSampleDescriptionIndex = 1;
                trex.DefaultSampleDuration = 0;
                trex.DefaultSampleSize = 0;
            }
        }

        /// <summary>
        /// How the samples of a fragment are protected, in its 'traf': each sample's IV and subsamples in 'senc', and the
        /// same as sample auxiliary information - their sizes in 'saiz', where they are in 'saio', which points into the
        /// 'senc' - unless there is none, as of samples with a constant IV and no subsamples (7.1, 10.4.1).
        /// </summary>
        private static void AddSampleEncryption(MovieFragmentBox moof, TrackFragmentBox traf, MediaFragment fragment, TrackEncryptor encryptor)
        {
            var (senc, saiz, saio) = encryptor.CreateSampleEncryptionBoxes(fragment.Encryptions, fragment.SampleSizes);
            foreach (var box in new Box[] { saiz, saio, senc }.Where(x => x != null))
            {
                box.SetParent(traf);
                traf.Children.Add(box);
            }

            if (saio != null)
            {
                // the auxiliary information is the 'senc's samples: where they start, from the start of the 'moof', the
                // base of the track fragment's offsets (default-base-is-moof)
                ulong offset = 8 + moof.Children.TakeWhile(x => x != traf).Aggregate(0ul, (total, box) => total + (box.CalculateSize() >> 3)) + 8;
                offset += traf.Children.TakeWhile(x => x != senc).Aggregate(0ul, (total, box) => total + (box.CalculateSize() >> 3));
                saio.Offset[0] = offset + 12 + 4; // the 'senc's header, version and flags, then its sample_count
            }
        }

        private static void CreateMediaFragment(Container fmp4, TrackContext trackContext, MediaFragment fragment, uint sequenceNumber)
        {
            ITrack track = trackContext.Track;
            MovieFragmentBox moof = new MovieFragmentBox();
            moof.SetParent(fmp4);
            fmp4.Children.Add(moof);
            moof.Children = new List<Box>();

            MovieFragmentHeaderBox mfhd = new MovieFragmentHeaderBox();
            mfhd.SetParent(moof);
            moof.Children.Add(mfhd);
            mfhd.SequenceNumber = sequenceNumber;

            TrackFragmentBox traf = new TrackFragmentBox();
            traf.SetParent(moof);
            moof.Children.Add(traf);
            traf.Children = new List<Box>();

            TrackFragmentHeaderBox tfhd = new TrackFragmentHeaderBox();
            tfhd.SetParent(traf);
            traf.Children.Add(tfhd);
            tfhd.TrackID = track.TrackID;
            tfhd.DefaultSampleFlags = track.DefaultSampleFlags;
            tfhd.Flags = tfhd.Flags | 0x20000u; // DefaultBaseIsMoof

            if(track.HandlerType == HandlerTypes.Video)
            {
                tfhd.Flags = tfhd.Flags | 0x20;
            }

            TrackFragmentBaseMediaDecodeTimeBox tfdt = new TrackFragmentBaseMediaDecodeTimeBox();
            tfdt.SetParent(traf);
            traf.Children.Add(tfdt);
            tfdt.Version = 1;
            tfdt.BaseMediaDecodeTime = fragment.StartTime; // BaseMediaDecodeTime must be in the timescale of the track

            TrackRunBox trun = new TrackRunBox();
            trun.SetParent(traf);
            trun.Flags = 0x301;

            // of video, which sample is a sync sample - the rest take the default flags, of a sample that is not: of the
            // first alone where it is the only one, else of each sample
            bool[] randomAccessPoints = fragment.RandomAccessPoints;
            bool perSampleFlags = false;
            if (track.HandlerType == HandlerTypes.Video && randomAccessPoints != null)
            {
                perSampleFlags = randomAccessPoints.Skip(1).Any(x => x);
                if (perSampleFlags)
                {
                    trun.Flags |= 0x400;
                }
                else if (randomAccessPoints.Length > 0 && randomAccessPoints[0])
                {
                    trun.FirstSampleFlags = SyncSampleFlags;
                    trun.Flags |= 0x4;
                }
            }

            // Pictures coded out of presentation order need the difference between composition and
            // decode time recorded, the same way the non fragmented builder writes ctts. Version 1
            // of the box carries it signed, so the offsets do not have to be biased.
            bool hasCompositionOffsets = fragment.CompositionOffsets != null &&
                fragment.CompositionOffsets.Any(offset => offset != 0);
            if (hasCompositionOffsets)
            {
                trun.Flags |= 0x800;
                trun.Version = 1;
            }

            trun.DataOffset = 0;
            trun._TrunEntry = new TrunEntry[fragment.SampleSizes.Length];
            for (int k = 0; k < fragment.SampleSizes.Length; k++)
            {
                trun._TrunEntry[k] = new TrunEntry(trun.Version, trun.Flags)
                {
                    Flags = trun.Flags,
                    SampleDuration = fragment.SampleDurations[k],
                    SampleSize = fragment.SampleSizes[k],
                    SampleFlags = perSampleFlags ? (randomAccessPoints[k] ? SyncSampleFlags : track.DefaultSampleFlags) : 0,
                    SampleCompositionTimeOffset0 = hasCompositionOffsets ? fragment.CompositionOffsets[k] : 0
                };
            }
            trun.SampleCount = (uint)trun._TrunEntry.Length;
            traf.Children.Add(trun);

            if (trackContext.Encryptor != null && fragment.Encryptions != null)
            {
                AddSampleEncryption(moof, traf, fragment, trackContext.Encryptor);
            }

            var mdat = new MediaDataBox();
            mdat.SetParent(fmp4);
            fmp4.Children.Add(mdat);
            mdat.Data = new StreamMarker(fragment.Offset, fragment.Length, new IsoStream(fragment.Storage));

            // the samples start after the 'moof' and the 'mdat' header - a largesize past 4 GB - as the boxes are written
            ulong mdatHeaderSize = (IsoStream.CalculateBoxSize(mdat) >> 3) - (ulong)fragment.Length;
            ulong offset = (IsoStream.CalculateBoxSize(moof) >> 3) + mdatHeaderSize;
            trun.DataOffset = (int)offset;
        }

        /// <summary>
        /// Writes what is left: the access unit each track is still assembling, the fragments not yet written and, unless
        /// it was turned off, the 'mfra' that says where each track's sync samples are. Given no samples, it writes the
        /// initialization segment alone. It is called once.
        /// </summary>
        public void FinalizeMedia()
        {
            ThrowIfFinished();

            // the access unit each track that gives back the previous one is still assembling - lost, before
            foreach (var track in _trackContexts.Values)
            {
                ProcessTrackSample(track, default, -1, 0);
            }

            // what is left: samples not yet in a fragment - of no bytes too, a text track's gaps - and fragments not written
            WriteFragment(true);
            if (!_isInitialized)
                WriteInitialization();
            _isFinalized = true;

            if (_appendMovieFragmentRandomAccessBox)
            {
                Container fmp4 = new Container();

                var mfra = new MovieFragmentRandomAccessBox();
                mfra.SetParent(fmp4);
                fmp4.Children.Add(mfra);
                mfra.Children = new List<Box>();

                foreach (var track in _trackContexts.Values)
                {
                    var entries = track.RandomAccessEntries;
                    var tfra = new TrackFragmentRandomAccessBox();
                    tfra.SetParent(mfra);
                    mfra.Children.Add(tfra);

                    // Each 'moof' holds the one 'traf' of the track, with one 'trun': the entry's sample is of those.
                    uint maxSampleNumber = entries.Count == 0 ? 1 : entries.Max(x => x.SampleNumber);
                    int sampleNumberSize = maxSampleNumber > 0xFFFFFF ? 4 : maxSampleNumber > 0xFFFF ? 3 : maxSampleNumber > 0xFF ? 2 : 1;
                    tfra.LengthSizeOfTrafNum = 0;
                    tfra.LengthSizeOfTrunNum = 0;
                    tfra.LengthSizeOfSampleNum = (byte)(sampleNumberSize - 1);
                    tfra.TrackID = track.Track.TrackID;
                    tfra.MoofOffset = entries.Select(x => x.MoofOffset).ToArray();
                    tfra.Time = entries.Select(x => x.Time).ToArray();
                    tfra.TrafNumber = entries.Select(x => new byte[] { 1 }).ToArray();
                    tfra.TrunNumber = entries.Select(x => new byte[] { 1 }).ToArray();
                    tfra.SampleDelta = entries.Select(x => Utils.BigEndian(x.SampleNumber, sampleNumberSize)).ToArray();
                    tfra.Version = MovieTime.Version(tfra.MoofOffset.Concat(tfra.Time).ToArray());

                    tfra.NumberOfEntry = (uint)entries.Count;
                }

                var mfro = new MovieFragmentRandomAccessOffsetBox();
                mfro.SetParent(mfra);
                mfra.Children.Add(mfro);
                mfro.ParentSize = (uint)(mfra.CalculateSize() >> 3);

                var fstr = _output.GetStream(_moofSequenceNumber);
                var fragmentStream = new IsoStream(fstr);
                _bytesWritten += fmp4.Write(fragmentStream) >> 3;
                _output.Flush(fstr, _moofSequenceNumber);
            }

            DisposeStorages();
        }

        private void DisposeStorages()
        {
            foreach (var track in _trackContexts.Values)
            {
                var storages = track.ReadyFragments.Select(x => x.Storage).Append(track.CurrentFragments).Where(x => x != null).Distinct().ToList();
                foreach (var storage in storages)
                    storage.Dispose();
                track.ReadyFragments.Clear();
                track.CurrentFragments = null;
            }
        }

        /// <summary>
        /// Lets go of where the samples of fragments not yet written wait. Without <see cref="FinalizeMedia"/> before it,
        /// nothing more is written: the fragments written are all there is.
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
