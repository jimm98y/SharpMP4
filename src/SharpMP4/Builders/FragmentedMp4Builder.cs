using SharpISOBMFF;
using SharpMP4.Common;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Builders
{
    /// <summary>
    /// Creates Fragmented MP4.
    /// </summary>
    public class FragmentedMp4Builder : IMp4Builder
    {
        private class MediaFragment
        {
            public MediaFragment(IStorage storage, ulong startTime, ulong endTime, uint[] sampleSizes, uint[] sampleDurations)
            {
                this.Storage = storage;
                this.StartTime = startTime;
                this.EndTime = endTime;
                this.SampleSizes = sampleSizes;
                this.SampleDurations = sampleDurations;
            }

            public ulong StartTime { get; set; }
            public ulong EndTime { get; set; }
            public uint[] SampleSizes { get; set; }
            public uint[] SampleDurations { get; set; }
            public IStorage Storage { get; set; }
        }

        private class TrackContext
        {
            public TrackContext(ITrack track, IStorage storage)
            {
                Track = track;
                MoofTime.Add(0); // initial time is 0
                CurrentFragments = storage;
            }

            public TrackContext(ITrack track, ITemporaryStorageFactory temporaryStorageFactory)
                : this(track, temporaryStorageFactory.Create())
            {
            }

            public TrackContext(ITrack track)
                : this(track, new TemporaryFileStorageFactory())
            {
            }

            public ITrack Track { get; set; }

            public ulong StartTime { get; set; }
            public ulong EndTime { get; set; }
            public List<uint> SampleSizes { get; set; } = new List<uint>();
            public List<uint> SampleDurations { get; set; } = new List<uint>();
            public uint FragmentCounts { get; set; }

            public IStorage CurrentFragments { get; set; }
            public Queue<MediaFragment> ReadyFragments { get; set; } = new Queue<MediaFragment>();
            public List<ulong> MoofOffsets { get; set; } = new List<ulong>();
            public List<ulong> MoofTime { get; set; } = new List<ulong>();
        }

        public uint MovieTimescale { get; set; } = 1000;

        private readonly IMp4Output _output;

        private readonly ulong _maxFragmentLengthInMs;
        private readonly ulong _durationInMs = 0;
        private readonly bool _appendMovieFragmentRandomAccessBox;

        private readonly Dictionary<uint, TrackContext> _trackContexts = new Dictionary<uint, TrackContext>();
        
        private uint _moofSequenceNumber = 1;

        public IMp4Logger Logger { get; set; } = new DefaultMp4Logger();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="output">Output stream. Will be progressively written while recording. <see cref="IMp4Output"/>.</param>
        /// <param name="maxFragmentLengthInMs">Maximum duration of 1 sample. Default is 2.66 sec.</param>
        /// <param name="durationInMs">Duration of the movie. Default value is 0 for live recordings.</param>
        /// <param name="appendMovieFragmentRandomAccessBox">Append Movie Fragment Random Access box at the end of the fragmented MP4.</param>
        public FragmentedMp4Builder(IMp4Output output, ulong maxFragmentLengthInMs, ulong durationInMs = 0, bool appendMovieFragmentRandomAccessBox = true)
        {
            _output = output;
            _maxFragmentLengthInMs = maxFragmentLengthInMs;
            _durationInMs = durationInMs;
            _appendMovieFragmentRandomAccessBox = appendMovieFragmentRandomAccessBox;
        }

        /// <summary>
        /// Add a track to the Fragmented MP4.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        public void AddTrack(ITrack track)
        {
            uint trackID = GetNextTrackId();
            track.TrackID = trackID;
            _trackContexts.Add(trackID, new TrackContext(track));
        }

        private uint GetNextTrackId()
        {
            uint trackID = 1;
            while (_trackContexts.ContainsKey(trackID))
                trackID++;
            return trackID;
        }

        public void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration = -1)
        {
            _trackContexts[trackID].Track.ProcessSample(sample, out var processedSample, out var isRandomAccessPoint);

            if (processedSample != null)
            {
                ProcessRawSample(trackID, processedSample, sampleDuration, isRandomAccessPoint);
            }
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint)
        {
            ProcessRawSample(trackID, sample, sampleDuration, isRandomAccessPoint, new TemporaryFileStorageFactory());
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint, ITemporaryStorageFactory temporaryStorageFactory)
        {
            ProcessRawSample(trackID, sample, sampleDuration, isRandomAccessPoint, temporaryStorageFactory.Create());
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint, IStorage storage)
        {
            TrackContext track = _trackContexts[trackID];
            uint currentSampleDuration = sampleDuration < 0 ? (uint)track.Track.DefaultSampleDuration : (uint)sampleDuration;
            ulong nextFragmentTime = track.Track.Timescale * _maxFragmentLengthInMs * (track.FragmentCounts + 1);
            ulong currentFragmentTime = track.EndTime * 1000;

            if (track.SampleSizes.Count > 0 && nextFragmentTime <= currentFragmentTime)
            {
                var fragment = CreateNewFragment(track);
                track.ReadyFragments.Enqueue(fragment);

                track.CurrentFragments = storage;
                track.StartTime = _trackContexts[trackID].EndTime;
                track.SampleSizes.Clear();
                track.SampleDurations.Clear();
                track.FragmentCounts++;
            }

            track.CurrentFragments.Write(sample, 0, sample.Length);
            track.SampleSizes.Add((uint)sample.Length);
            track.SampleDurations.Add(currentSampleDuration);
            track.EndTime += currentSampleDuration;

            bool isFragmentReady = true;
            foreach (var ctx in _trackContexts.Values)
            {
                if (ctx.ReadyFragments.Count == 0)
                {
                    isFragmentReady = false;
                    break;
                }
            }

            if (isFragmentReady)
            {
                WriteFragment();
            }
        }

        private MediaFragment CreateNewFragment(TrackContext track)
        {
            var ret = new MediaFragment(track.CurrentFragments, track.StartTime, track.EndTime, track.SampleSizes.ToArray(), track.SampleDurations.ToArray());
            track.SampleSizes.Clear();
            return ret;
        }

        private void WriteFragment(bool isFlushing = false)
        {
            // first check if we need to produce the media initialization segment
            if (_moofSequenceNumber == 1)
            {
                Container init = new Container();
                CreateMediaInitialization(init);

                const uint initializationSegmentNumber = 0; // sequence ID 0 is used to indicate "initialization"
                var str = _output.GetStream(initializationSegmentNumber);
                IsoStream initializationStream = new IsoStream(str);
                init.Write(initializationStream);
                _output.Flush(str, initializationSegmentNumber);
            }

            // all tracks have enough samples to produce a fragment
            do
            {
                // VLC requires the first MOOF to be video, otherwise we get choppy playback
                // first video tracks
                foreach(var track in _trackContexts.Values)
                {
                    if (track.Track.HandlerType == HandlerTypes.Video)
                    {
                        WriteTrackFragment(isFlushing, track);
                    }
                }

                // then audio tracks
                foreach (var track in _trackContexts.Values)
                {
                    if (track.Track.HandlerType == HandlerTypes.Sound)
                    {
                        WriteTrackFragment(isFlushing, track);
                    }
                }

                // then the rest
                foreach (var track in _trackContexts.Values)
                {
                    if (track.Track.HandlerType != HandlerTypes.Video && track.Track.HandlerType != HandlerTypes.Sound)
                    {
                        WriteTrackFragment(isFlushing, track);
                    }
                }
            }
            while (isFlushing && _trackContexts.Values.Any(x => x.ReadyFragments.Count > 0 || x.SampleSizes.Count > 0));
        }

        private void WriteTrackFragment(bool isFlushing, TrackContext track)
        {
            MediaFragment fragment;

            if (isFlushing)
            {
                if (track.ReadyFragments.Count > 0)
                {
                    fragment = track.ReadyFragments.Dequeue();
                }
                else if (track.SampleSizes.Count > 0)
                {
                    fragment = CreateNewFragment(track);
                }
                else
                {
                    // at the very end we might not have any samples for a track
                    return;
                }
            }
            else
            {
                fragment = track.ReadyFragments.Dequeue();
            }

            Container fmp4 = new Container();
            uint sequenceNumber = _moofSequenceNumber++;

            CreateMediaFragmentAsync(fmp4, track.Track, fragment, sequenceNumber);

            var outputStream = _output.GetStream(sequenceNumber);
            var fragmentStream = new IsoStream(outputStream);

            track.MoofOffsets.Add((ulong)fragmentStream.GetCurrentOffset());
            track.MoofTime.Add(fragment.EndTime);

            fmp4.Write(fragmentStream);
            _output.Flush(outputStream, sequenceNumber);
        }

        private void CreateMediaInitialization(Container fmp4)
        {
            var ftyp = new FileTypeBox();
            ftyp.SetParent(fmp4);
            fmp4.Children.Add(ftyp);

            ftyp.MajorBrand = IsoStream.FromFourCC("mp42");
            ftyp.MinorVersion = 1;
          
            var compatibleBrands = new List<string>()
            {
                "mp42",
                "isom",
            };

            foreach (var track in _trackContexts.Values)
            {
                if (!string.IsNullOrEmpty(track.Track.CompatibleBrand))
                {
                    compatibleBrands.Insert(1, track.Track.CompatibleBrand);
                }
            }

            ftyp.CompatibleBrands = compatibleBrands.Distinct().Select(IsoStream.FromFourCC).ToArray();

            var moov = new MovieBox();
            moov.SetParent(fmp4);
            fmp4.Children.Add(moov);

            var mvhd = new MovieHeaderBox();
            mvhd.SetParent(moov);
            moov.Children = new List<Box>();
            moov.Children.Add(mvhd);
            mvhd.Duration = _durationInMs * MovieTimescale / 1000;
            mvhd.NextTrackID = 0xFFFFFFFF; // TODO simplify API
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
                tkhd.Flags = 0x07;
                track.Track.FillTkhdBox(tkhd);
                tkhd.Duration = _durationInMs * MovieTimescale / 1000; 

                var mdia = new MediaBox();
                mdia.SetParent(trak);
                trak.Children.Add(mdia);

                MediaHeaderBox mdhd = new MediaHeaderBox();
                mdhd.SetParent(mdia);
                mdia.Children = new List<Box>();
                mdia.Children.Add(mdhd);
                mdhd.Duration = 0;
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
                mehd.FragmentDuration = _durationInMs * MovieTimescale / 1000;
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

        private void CreateMediaFragmentAsync(Container fmp4, ITrack track, MediaFragment fragment, uint sequenceNumber)
        {
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
            if (track.HandlerType == HandlerTypes.Video)
            {
                trun.FirstSampleFlags = 0x02000000;
                trun.Flags = 0x305;
            }
            else
            {
                trun.Flags = 0x301;
            }                
            trun.DataOffset = 0;
            trun._TrunEntry = new TrunEntry[fragment.SampleSizes.Length];
            for (int k = 0; k < fragment.SampleSizes.Length; k++)
            {
                trun._TrunEntry[k] = new TrunEntry(0, 0)
                {
                    Flags = trun.Flags,
                    SampleDuration = fragment.SampleDurations[k],
                    SampleSize = fragment.SampleSizes[k]
                };
            }
            trun.SampleCount = (uint)trun._TrunEntry.Length;
            traf.Children.Add(trun);

            // recalculate offsets
            ulong offset = (moof.CalculateSize() >> 3) + 8;
            trun.DataOffset = (int)offset;
                        
            var mdat = new MediaDataBox();
            mdat.SetParent(fmp4);
            fmp4.Children.Add(mdat);
            mdat.Data = new StreamMarker(0, fragment.Storage.GetLength(), new IsoStream(fragment.Storage));
        }

        public void FinalizeMedia()
        {
            if (_trackContexts.Values.Any(x => x.CurrentFragments.GetLength() > 0))
            {
                WriteFragment(true);
            }

            if (_appendMovieFragmentRandomAccessBox)
            {
                Container fmp4 = new Container();

                var mfra = new MovieFragmentRandomAccessBox();
                mfra.SetParent(fmp4);
                fmp4.Children.Add(mfra);
                mfra.Children = new List<Box>();

                foreach (var track in _trackContexts.Values)
                {
                    var tfra = new TrackFragmentRandomAccessBox();
                    tfra.SetParent(mfra);
                    mfra.Children.Add(tfra);
                    tfra.LengthSizeOfSampleNum = 1;
                    tfra.LengthSizeOfTrafNum = 1;
                    tfra.LengthSizeOfTrunNum = 1;
                    tfra.TrackID = track.Track.TrackID;
                    tfra.MoofOffset = track.MoofOffsets.ToArray();
                    tfra.Time = track.MoofTime.ToArray();
                    // TODO: hadrcoded for now
                    tfra.TrafNumber = track.MoofOffsets.Select(x => new byte[] { 0, 1 }).ToArray();
                    tfra.TrunNumber = track.MoofOffsets.Select(x => new byte[] { 0, 1 }).ToArray();
                    tfra.SampleDelta = track.MoofOffsets.Select(x => new byte[] { 0, 1 }).ToArray();

                    tfra.NumberOfEntry = (uint)track.MoofOffsets.Count;
                }

                var mfro = new MovieFragmentRandomAccessOffsetBox();
                mfra.SetParent(mfra);
                mfra.Children.Add(mfro);
                mfro.ParentSize = (uint)mfra.CalculateSize() >> 3;

                var fstr = _output.GetStream(_moofSequenceNumber);
                var fragmentStream = new IsoStream(fstr);
                fmp4.Write(fragmentStream);
                _output.Flush(fstr, _moofSequenceNumber);
            }
        }
    }
}
