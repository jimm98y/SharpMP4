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
    /// What every builder does whatever the layout of the file it writes: it keeps the tracks, takes their samples - through
    /// the track's codec logic, or as they are - with their timing, and writes the boxes that say what each track is. Where a
    /// sample goes once it is timed, and how the file is laid out, is the builder's own: <see cref="Mp4Builder"/> writes one
    /// 'moov' and one 'mdat', and <see cref="FragmentedMp4Builder"/> an initialization segment and fragments.
    /// </summary>
    public abstract class Mp4BuilderBase : IMp4Builder, IDisposable
    {
        /// <summary>What a builder keeps of a track, of every layout.</summary>
        private protected class BuilderTrack
        {
            public BuilderTrack(ITrack track)
            {
                Track = track;
            }

            public ITrack Track { get; }

            /// <summary>What protects the track; null where it is not protected.</summary>
            public TrackEncryptor Encryptor { get; set; }

            /// <summary>How the track is protected; null where it is not.</summary>
            public TrackProtection Protection => Encryptor?.Protection;

            /// <summary>The timing given for the sample the track is assembling.</summary>
            public PendingSampleTiming Timing { get; } = new PendingSampleTiming();
        }

        private readonly Dictionary<uint, BuilderTrack> _tracks = new Dictionary<uint, BuilderTrack>();
        private bool _isDisposed;

        private protected Mp4BuilderBase(IMp4Output output)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
        }

        /// <inheritdoc/>
        public uint MovieTimescale { get; set; } = 1000;

        /// <inheritdoc/>
        public Mp4FileFormat FileFormat { get; set; } = Mp4FileFormat.Mp4;

        /// <inheritdoc/>
        public IMp4Logger Logger { get; set; } = DefaultMp4Logger.Instance;

        /// <summary>Where the samples wait until they are written. Temporary files by default.</summary>
        public ITemporaryStorageFactory TemporaryStorageFactory { get; set; } = new TemporaryFileStorageFactory();

        /// <summary>
        /// The creation and modification time the movie, track and media headers say. Null - the default - for the time
        /// the 'moov' is written; set it for output that is the same each time it is written.
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>Where the file is written.</summary>
        private protected IMp4Output Output { get; }

        /// <summary>The tracks, in the order they were added.</summary>
        private protected IEnumerable<BuilderTrack> Tracks => _tracks.Values;

        /// <summary>Whether <see cref="FinalizeMedia"/> has written the file: after it, nothing more is taken.</summary>
        private protected bool IsFinalized { get; set; }

        /// <summary>What a builder that was finalized says when it is given more.</summary>
        private protected abstract string FinalizedMessage { get; }

        /// <summary>What the builder keeps of a track it is given.</summary>
        private protected abstract BuilderTrack CreateTrack(ITrack track);

        /// <summary>Called as a track is added, before it is the builder's: throws where the builder takes no more tracks.</summary>
        private protected virtual void OnAddingTrack()
        {
        }

        /// <summary>
        /// A sample, timed: a negative duration is the track's default, and 0 a sample of none.
        /// </summary>
        private protected abstract void AppendSample(BuilderTrack track, ArraySegment<byte> sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset);

        /// <summary>Writes what is left of the file, once each track's last sample is in, and marks the builder finalized.</summary>
        private protected abstract void WriteMedia();

        /// <summary>Lets go of where the samples wait.</summary>
        private protected abstract void DisposeStorages();

        /// <summary>
        /// Add a track.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        public void AddTrack(ITrack track)
        {
            if (track == null)
                throw new ArgumentNullException(nameof(track));
            ThrowIfFinished();
            OnAddingTrack();

            // A track logs where the builder does, unless it was given a logger of its own: TrackBase starts with the
            // default one, so it is replaced too.
            if (track.Logger == null || track.Logger == DefaultMp4Logger.Instance)
                track.Logger = this.Logger;

            // the format's constraints, before the track is the builder's
            FileBrands.Validate(FileFormat, _tracks.Values.Select(x => x.Track).Append(track).ToList(), Logger);

            uint trackID = GetNextTrackId();
            track.TrackID = trackID;
            _tracks.Add(trackID, CreateTrack(track));
        }

        /// <summary>
        /// Adds a track whose samples are protected as they are written (ISO/IEC 23001-7): each with the key, its IV and,
        /// of H.264, H.265 and H.266 video, its NAL units' slice headers left clear. The sample entry becomes 'encv', 'enca'
        /// or 'enct', with a 'sinf' saying what it was and how it is protected; how each sample is protected is said in a
        /// 'senc', 'saiz' and 'saio' - of the 'trak' and its 'stbl' in an MP4, of each fragment's 'traf' in a fragmented one.
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
            _tracks[track.TrackID].Encryptor = encryptor;
        }

        private uint GetNextTrackId()
        {
            uint trackID = 1;
            while (_tracks.ContainsKey(trackID))
                trackID++;
            return trackID;
        }

        private protected void ThrowIfFinished()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
            if (IsFinalized)
                throw new InvalidOperationException(FinalizedMessage);
        }

        private BuilderTrack GetBuilderTrack(uint trackID)
        {
            ThrowIfFinished();
            if (!_tracks.TryGetValue(trackID, out var track))
                throw new ArgumentException($"There is no track {trackID}: a track is added with AddTrack, which gives it its ID.", nameof(trackID));
            return track;
        }

        /// <inheritdoc/>
        ITrack IMp4Builder.GetTrack(uint trackID) => GetBuilderTrack(trackID).Track;

        /// <summary>The same, for a sample that sits inside a larger buffer.</summary>
        public void ProcessTrackSample(uint trackID, ArraySegment<byte> sample, int sampleDuration = -1, int compositionOffset = 0) =>
            ProcessTrackSample(GetBuilderTrack(trackID), sample, sampleDuration, compositionOffset);

        /// <summary>
        /// Appends a sample, recording how far its composition time sits from its decode time.
        /// </summary>
        /// <param name="sampleDuration">The duration of the sample - of the access unit the NAL unit is of, for video given a NAL unit at a time - in the track's timescale; negative for the track's default.</param>
        /// <param name="compositionOffset">
        /// Composition time minus decode time, in track timescale units. Needed whenever pictures
        /// are coded out of presentation order; leave at 0 for streams that are not reordered.
        /// </param>
        public void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration = -1, int compositionOffset = 0) =>
            ProcessTrackSample(GetBuilderTrack(trackID), sample == null ? default : new ArraySegment<byte>(sample), sampleDuration, compositionOffset);

        private void ProcessTrackSample(BuilderTrack track, ArraySegment<byte> sample, int sampleDuration, int compositionOffset)
        {
            track.Track.ProcessSample(sample.Array, sample.Offset, sample.Count, out var processedSample, out var isRandomAccessPoint);

            // A track that gives back the access unit before the one it was given gives it the timing given with it.
            track.Timing.Resolve(track.Track, sample.Array == null, processedSample.Array != null, ref sampleDuration, ref compositionOffset);

            if (processedSample.Array != null)
            {
                foreach (var (frame, duration) in PcmFrames.SamplesOf(track.Track, processedSample, sampleDuration))
                    AppendSample(track, frame, duration, isRandomAccessPoint, compositionOffset);
            }
        }

        /// <inheritdoc/>
        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration = -1, bool isRandomAccessPoint = true, int compositionOffset = 0)
        {
            AppendSample(GetBuilderTrack(trackID), new ArraySegment<byte>(sample), sampleDuration, isRandomAccessPoint, compositionOffset);
        }

        /// <inheritdoc/>
        public void ProcessRawSample(uint trackID, ArraySegment<byte> sample, int sampleDuration = -1, bool isRandomAccessPoint = true, int compositionOffset = 0)
        {
            AppendSample(GetBuilderTrack(trackID), sample, sampleDuration, isRandomAccessPoint, compositionOffset);
        }

        /// <summary>
        /// Writes what is left: the access unit each track is still assembling, then the rest of the file. It is called once.
        /// </summary>
        public void FinalizeMedia()
        {
            ThrowIfFinished();

            // the access unit each track that gives back the previous one is still assembling
            foreach (var track in _tracks.Values)
            {
                ProcessTrackSample(track, default, -1, 0);
            }

            WriteMedia();
        }

        /// <summary>The 'ftyp': of the format, and of what the tracks are.</summary>
        private protected FileTypeBox CreateFileType(bool fragmented)
        {
            return FileBrands.Create(FileFormat, _tracks.Values.Select(x => x.Track).ToList(), fragmented: fragmented,
                isProtected: _tracks.Values.Any(x => x.Encryptor != null),
                hasSubtitleMediaHeader: _tracks.Values.Any(x => x.Track.HandlerType == HandlerTypes.Subtitle), Logger);
        }

        /// <summary>The 'mvhd' of a 'moov' being built, its duration and version left to the builder.</summary>
        private protected MovieHeaderBox AddMovieHeader(MovieBox moov, ulong creationTime)
        {
            var mvhd = new MovieHeaderBox();
            mvhd.SetParent(moov);
            moov.Children = new List<Box>();
            moov.Children.Add(mvhd);
            mvhd.NextTrackID = 0xFFFFFFFF; // TODO simplify API
            mvhd.Timescale = MovieTimescale; // just for movie time: https://stackoverflow.com/questions/77803940/diffrence-between-mvhd-box-timescale-and-mdhd-box-timescale-in-isobmff-format
            mvhd.CreationTime = creationTime;
            mvhd.ModificationTime = creationTime;
            mvhd.Reserved0 = new uint[2]; // TODO simplify API
            mvhd.PreDefined = new uint[6]; // TODO simplify API
            return mvhd;
        }

        /// <summary>
        /// A 'trak' of a 'moov' being built: its 'tkhd', its edit list where it has one, and its media down to the 'stsd',
        /// whose 'stbl' is given back for the builder to fill with the sample tables.
        /// </summary>
        /// <param name="tkhdFlags">The 'tkhd' flags: whether the track is enabled, in the movie, in its preview and in its poster.</param>
        /// <param name="trackDuration">The track's duration, in the movie's timescale.</param>
        /// <param name="mediaDuration">The media's duration, in the track's timescale: 0 where it is in fragments.</param>
        /// <param name="edit">The edit that maps the media to the movie, as its duration in the movie's timescale and where it starts in the media; null for none.</param>
        private protected static SampleTableBox AddTrackBox(MovieBox moov, BuilderTrack track, uint tkhdFlags, ulong trackDuration, ulong mediaDuration,
            ulong creationTime, (ulong Duration, long MediaTime)? edit, out TrackBox trak)
        {
            trak = new TrackBox();
            trak.SetParent(moov);
            trak.Children = new List<Box>();
            moov.Children.Add(trak);

            var tkhd = new TrackHeaderBox();
            tkhd.SetParent(trak);
            trak.Children.Add(tkhd);
            tkhd.TrackID = track.Track.TrackID;
            tkhd.Reserved1 = new uint[2]; // TODO simplify API
            tkhd.Flags = tkhdFlags;
            track.Track.FillTkhdBox(tkhd);
            tkhd.Duration = trackDuration;
            tkhd.CreationTime = creationTime;
            tkhd.ModificationTime = creationTime;
            tkhd.Version = MovieTime.Version(trackDuration, creationTime);

            if (edit.HasValue)
            {
                var (editDuration, mediaTime) = edit.Value;

                var edts = new EditBox();
                edts.SetParent(trak);
                edts.Children = new List<Box>();
                trak.Children.Add(edts);

                var elst = new EditListBox();
                elst.SetParent(edts);
                edts.Children.Add(elst);
                elst.EntryCount = 1;
                elst.EditDuration = new ulong[] { editDuration };
                elst.MediaTime = new long[] { mediaTime };
                elst.MediaRateInteger = new short[] { 1 };
                elst.MediaRateFraction = new short[] { 0 };
                elst.Version = MovieTime.Version(editDuration, (ulong)mediaTime);
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
            mdhd.Duration = mediaDuration;
            mdhd.Timescale = track.Track.Timescale;
            mdhd.Language = track.Track.Language;
            mdhd.CreationTime = creationTime;
            mdhd.ModificationTime = creationTime;
            mdhd.Version = MovieTime.Version(mediaDuration, creationTime);

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

            return stbl;
        }

        /// <summary>The protection systems' headers, each once, of all the protected tracks, at the end of the 'moov'.</summary>
        private protected void AddProtectionSystemHeaders(MovieBox moov)
        {
            foreach (var pssh in TrackEncryptor.ProtectionSystemHeaders(_tracks.Values.Where(x => x.Protection != null).Select(x => x.Protection)))
            {
                pssh.SetParent(moov);
                moov.Children.Add(pssh);
            }
        }

        /// <summary>
        /// Lets go of where the samples wait. What <see cref="FinalizeMedia"/> has not written is not written.
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
