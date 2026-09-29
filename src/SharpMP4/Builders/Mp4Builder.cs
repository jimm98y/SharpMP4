using SharpISOBMFF;
using SharpISOBMFF.Extensions;
using SharpMP4.Common;
using SharpMP4.Encryption;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Builders
{
    /// <summary>
    /// Creates MP4.
    /// </summary>
    public class Mp4Builder : IMp4Builder
    {
        private class TrackContext
        {
            public ITrack Track { get; set; }
            public ulong EndTime { get; set; }
            public List<uint> SampleSizes { get; set; } = new List<uint>();
            public List<long> SampleOffsets { get; set; } = new List<long>();
            public List<uint> RandomAccessPoints { get; set; } = new List<uint>();

            /// <summary>
            /// Composition time minus decode time, per sample. Non-zero whenever pictures are
            /// coded out of presentation order, as with B pictures.
            /// </summary>
            public List<int> CompositionOffsets { get; set; } = new List<int>();

            /// <summary>Decode duration of each sample, in track timescale units.</summary>
            public List<uint> SampleDurations { get; set; } = new List<uint>();

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

        public uint MovieTimescale { get; set; } = 1000;

        private readonly IMp4Output _output;

        private IStorage _storage;

        private readonly Dictionary<uint, TrackContext> _trackContexts = new Dictionary<uint, TrackContext>();        

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="output">Output stream. Will be progressively written while recording. <see cref="IMp4Output"/>.</param>
        public Mp4Builder(IMp4Output output)
        {
            _output = output;
        }

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
            track.Logger ??= this.Logger;

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

        private void WriteSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset = 0) =>
            WriteSample(trackID, new ArraySegment<byte>(sample), sampleDuration, isRandomAccessPoint, compositionOffset);

        private void WriteSample(uint trackID, ArraySegment<byte> sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset = 0)
        {
            if (_storage == null)
            {
                _storage = this.TemporaryStorageFactory.Create();
            }

            uint currentSampleDuration = sampleDuration <= 0 ? (uint)_trackContexts[trackID].Track.DefaultSampleDuration : (uint)sampleDuration;
            var track = _trackContexts[trackID];
            if (track.Encryptor != null)
            {
                sample = track.Encryptor.Protect(track.Track, sample, out var encryption);
                track.Encryptions.Add(encryption);
            }
            track.SampleOffsets.Add(_storage.GetPosition());
            _storage.Write(sample.Array, sample.Offset, sample.Count);
            track.SampleSizes.Add((uint)sample.Count);
            track.CompositionOffsets.Add(compositionOffset);
            track.SampleDurations.Add(currentSampleDuration);
            track.EndTime += currentSampleDuration;

            if(isRandomAccessPoint)
            {
                track.RandomAccessPoints.Add((uint)track.SampleSizes.Count);
            }
        }

        private MovieBox BuildMoov(bool largeOffsets)
        {
            var moov = new MovieBox();

            var mvhd = new MovieHeaderBox();
            mvhd.SetParent(moov);
            moov.Children = new List<Box>();
            moov.Children.Add(mvhd);

            // maximum duration of all the tracks
            ulong movieDuration = 0;
            foreach(var track in _trackContexts.Values)
            {
                ulong trackDuration = (ulong)Math.Ceiling(track.EndTime * 1000 / (double)track.Track.Timescale);
                if (trackDuration > movieDuration)
                {
                    movieDuration = trackDuration;
                }
            }

            mvhd.Duration = movieDuration * MovieTimescale / 1000;
            mvhd.NextTrackID = 0xFFFFFFFF;
            mvhd.Timescale = MovieTimescale; // just for movie time: https://stackoverflow.com/questions/77803940/diffrence-between-mvhd-box-timescale-and-mdhd-box-timescale-in-isobmff-format
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
                tkhd.Duration = movieDuration * MovieTimescale / 1000;

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
                mdhd.Duration = movieDuration * track.Track.Timescale / 1000;
                mdhd.Timescale = track.Track.Timescale;
                mdhd.Language = track.Track.Language;

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

                if (track.CompositionOffsets.Any(offset => offset != 0))
                {
                    // Pictures are coded out of presentation order, so the difference between
                    // composition and decode time has to be recorded.
                    var ctts = new CompositionOffsetBox();
                    ctts.SetParent(stbl);
                    stbl.Children.Add(ctts);

                    // Version 0 carries unsigned offsets, so shift them all by the most negative
                    // one. That delays every composition time by the same amount, which leaves the
                    // presentation order untouched.
                    int bias = track.CompositionOffsets.Min();

                    var counts = new List<uint>();
                    var offsets = new List<uint>();
                    foreach (var offset in track.CompositionOffsets)
                    {
                        uint value = (uint)(offset - bias);
                        if (offsets.Count > 0 && offsets[offsets.Count - 1] == value)
                            counts[counts.Count - 1]++;
                        else
                        {
                            counts.Add(1);
                            offsets.Add(value);
                        }
                    }

                    ctts.SampleCount = counts.ToArray();
                    ctts.SampleOffset = offsets.ToArray();
                    ctts.EntryCount = (uint)counts.Count;
                }

                var stsc = new SampleToChunkBox();
                stsc.SetParent(stbl);
                stbl.Children.Add(stsc);
                // defaults: 1 sample per chunk
                stsc.FirstChunk = new uint[] { 1 };
                stsc.SamplesPerChunk = new uint[] { 1 };
                stsc.SampleDescriptionIndex = new uint[] { 1 };                
                stsc.EntryCount = (uint)stsc.FirstChunk.Length;

                var stsz = new SampleSizeBox();
                stsz.SetParent(stbl);
                stbl.Children.Add(stsz);
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
                    co64.ChunkOffset = track.SampleOffsets.Select(offset => (ulong)offset).ToArray();
                    co64.EntryCount = (uint)track.SampleOffsets.Count;
                }
                else
                {
                    var stco = new ChunkOffsetBox();
                    stco.SetParent(stbl);
                    stbl.Children.Add(stco);
                    stco.ChunkOffset = track.SampleOffsets.Select(offset => (uint)offset).ToArray();
                    stco.EntryCount = (uint)track.SampleOffsets.Count;
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

            // the protection systems' headers, each once, of all the protected tracks
            foreach (var pssh in TrackEncryptor.ProtectionSystemHeaders(_trackContexts.Values.Where(x => x.Encryptor != null).Select(x => x.Encryptor.Protection)))
            {
                pssh.SetParent(moov);
                moov.Children.Add(pssh);
            }

            return moov;
        }

        /// <summary>The same, for a sample that sits inside a larger buffer.</summary>
        public void ProcessTrackSample(uint trackID, ArraySegment<byte> sample, int sampleDuration = -1, int compositionOffset = 0)
        {
            _trackContexts[trackID].Track.ProcessSample(sample.Array, sample.Offset, sample.Count,
                out var processedSample, out var isRandomAccessPoint);

            if (processedSample.Array != null)
            {
                WriteSample(trackID, processedSample, sampleDuration, isRandomAccessPoint, compositionOffset);
            }
        }

        /// <summary>
        /// Appends a sample, recording how far its composition time sits from its decode time.
        /// </summary>
        /// <param name="compositionOffset">
        /// Composition time minus decode time, in track timescale units. Needed whenever pictures
        /// are coded out of presentation order; leave at 0 for streams that are not reordered.
        /// </param>
        public void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration = -1, int compositionOffset = 0)
        {
            _trackContexts[trackID].Track.ProcessSample(sample, out var processedSample, out var isRandomAccessPoint);

            if (processedSample.Array != null)
            {
                WriteSample(trackID, processedSample, sampleDuration, isRandomAccessPoint, compositionOffset);
            }
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint)
        {
            WriteSample(trackID, sample, sampleDuration, isRandomAccessPoint);
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint, int compositionOffset)
        {
            WriteSample(trackID, sample, sampleDuration, isRandomAccessPoint, compositionOffset);
        }

        /// <summary>
        /// The same, for a sample that sits inside a larger buffer - one a caller fills again for
        /// each sample - so it does not have to be copied out into an array of its own first.
        /// </summary>
        public void ProcessRawSample(uint trackID, ArraySegment<byte> sample, int sampleDuration,
            bool isRandomAccessPoint, int compositionOffset = 0)
        {
            WriteSample(trackID, sample, sampleDuration, isRandomAccessPoint, compositionOffset);
        }

        public void FinalizeMedia()
        {
            foreach(var track in _trackContexts.Values)
            {
                ProcessTrackSample(track.Track.TrackID, null, -1);
            }

            var mp4 = new Container();

            var ftyp = new FileTypeBox();
            ftyp.SetParent(mp4);
            mp4.Children.Add(ftyp);
            ftyp.MajorBrand = IsoStream.FromFourCC("isom");
            ftyp.MinorVersion = 512;
            var compatibleBrands = new List<string>() { "mp41", "isom" };
            foreach(var track in _trackContexts.Values)
            {
                if (!string.IsNullOrEmpty(track.Track.CompatibleBrand))
                {
                    compatibleBrands.Insert(1, track.Track.CompatibleBrand);
                }
            }
            ftyp.CompatibleBrands = compatibleBrands.Distinct().Select(IsoStream.FromFourCC).ToArray();

            // create moov at the beginning of the file (faststart, allowing to play video early while still streaming)
            var mdat = new MediaDataBox();
            mdat.Data = new StreamMarker(0, _storage.GetLength(), new IsoStream(_storage));

            // The offsets are relative to the media data until the header size is known, and the
            // header size depends on which offset box is used. Size it with 32-bit offsets first,
            // then redo it with 64-bit ones if the real offsets would not fit.
            bool largeOffsets = false;
            MovieBox moov;
            long mdatOffset;
            while (true)
            {
                moov = BuildMoov(largeOffsets);
                mdatOffset = ((long)(ftyp.CalculateSize() + moov.CalculateSize()) >> 3) + 4 + (mdat.HasLargeSize ? 8 : 4);

                long highestOffset = 0;
                foreach (var track in _trackContexts.Values)
                    if (track.SampleOffsets.Count > 0)
                        highestOffset = Math.Max(highestOffset, track.SampleOffsets[track.SampleOffsets.Count - 1]);

                if (largeOffsets || highestOffset + mdatOffset <= uint.MaxValue)
                    break;

                largeOffsets = true;
            }

            moov.SetParent(mp4);
            mp4.Children.Add(moov);
            mp4.Children.Add(mdat);

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
        }
    }
}
