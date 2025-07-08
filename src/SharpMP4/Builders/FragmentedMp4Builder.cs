using SharpISOBMFF;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Builders
{
    /// <summary>
    /// Fragmented MP4 (fMP4) builder.
    /// </summary>
    public class FragmentedMp4Builder : IMp4Builder
    {
        public class Mp4Fragment
        {
            public Mp4Fragment(IStorage storage, ulong startTime, ulong endTime, uint[] sampleSizes, uint[] sampleDurations)
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

        public uint MovieTimescale { get; set; } = 1000;

        private readonly IMp4Output _output;

        private readonly ulong _maxFragmentLengthInMs;
        private readonly ulong _durationInMs = 0;
        private readonly bool _appendMovieFragmentRandomAccessBox;

        private readonly List<ITrack> _tracks = new List<ITrack>();
        private readonly List<ulong> _trackStartTimes = new List<ulong>();
        private readonly List<ulong> _trackEndTimes = new List<ulong>();
        private readonly List<List<uint>> _trackSampleSizes = new List<List<uint>>();
        private readonly List<List<uint>> _trackSampleDurations = new List<List<uint>>();
        private readonly List<uint> _trackFragmentCounts = new List<uint>();
        private readonly List<IStorage> _trackCurrentFragments = new List<IStorage>();
        private readonly List<Queue<Mp4Fragment>> _trackReadyFragments = new List<Queue<Mp4Fragment>>();
                
        private readonly List<List<ulong>> _trackMoofOffsets = new List<List<ulong>>();
        private readonly List<List<ulong>> _trackMoofTime = new List<List<ulong>>();
        
        private uint _moofSequenceNumber = 1;

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
        /// Add a track to the fMP4.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        public void AddTrack(ITrack track)
        {
            _tracks.Add(track);
            _trackStartTimes.Add(0);
            _trackEndTimes.Add(0);
            _trackFragmentCounts.Add(0);
            _trackSampleSizes.Add(new List<uint>());
            _trackSampleDurations.Add(new List<uint>());
            _trackCurrentFragments.Add(TemporaryStorage.Factory.Create());
            _trackReadyFragments.Add(new Queue<Mp4Fragment>());
            _trackMoofOffsets.Add(new List<ulong>());
            _trackMoofTime.Add(new List<ulong>() { 0 }); // initial time is 0
            track.TrackID = (uint)_tracks.IndexOf(track) + 1;
        }

        public void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration = -1)
        {
            _tracks[(int)trackID - 1].ProcessSample(sample, out var processedSample, out var isRandomAccessPoint);

            if (processedSample != null)
            {
                ProcessMp4Sample(trackID, processedSample, sampleDuration, isRandomAccessPoint);
            }
        }

        public void ProcessMp4Sample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint)
        {
            uint currentSampleDuration = sampleDuration < 0 ? (uint)_tracks[(int)trackID - 1].DefaultSampleDuration : (uint)sampleDuration;
            ulong nextFragmentTime = _tracks[(int)trackID - 1].Timescale * _maxFragmentLengthInMs * (_trackFragmentCounts[(int)trackID - 1] + 1);
            ulong currentFragmentTime = _trackEndTimes[(int)trackID - 1] * 1000;

            if (_trackSampleSizes[(int)trackID - 1].Count > 0 && nextFragmentTime <= currentFragmentTime)
            {
                var fragment = CreateNewFragment(trackID);
                _trackReadyFragments[(int)trackID - 1].Enqueue(fragment);

                _trackCurrentFragments[(int)trackID - 1] = TemporaryStorage.Factory.Create();
                _trackStartTimes[(int)trackID - 1] = _trackEndTimes[(int)trackID - 1];
                _trackSampleSizes[(int)trackID - 1].Clear();
                _trackSampleDurations[(int)trackID - 1].Clear();
                _trackFragmentCounts[(int)trackID - 1]++;
            }

            _trackCurrentFragments[(int)trackID - 1].Write(sample, 0, sample.Length);
            _trackSampleSizes[(int)trackID - 1].Add((uint)sample.Length);
            _trackSampleDurations[(int)trackID - 1].Add(currentSampleDuration);
            _trackEndTimes[(int)trackID - 1] += currentSampleDuration;

            bool isFragmentReady = true;
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_trackReadyFragments[i].Count == 0)
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

        private Mp4Fragment CreateNewFragment(uint trackID)
        {
            int index = (int)trackID - 1;
            var ret = new Mp4Fragment(_trackCurrentFragments[index], _trackStartTimes[index], _trackEndTimes[index], _trackSampleSizes[index].ToArray(), _trackSampleDurations[index].ToArray());
            _trackSampleSizes[index].Clear();
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
                for (int i = 0; i < _tracks.Count; i++)
                {
                    if (_tracks[i].HandlerType == HandlerTypes.Video)
                    {
                        WriteTrackFragment(isFlushing, i);
                    }
                }

                // then audio tracks
                for (int i = 0; i < _tracks.Count; i++)
                {
                    if (_tracks[i].HandlerType == HandlerTypes.Sound)
                    {
                        WriteTrackFragment(isFlushing, i);
                    }
                }

                // then the rest
                for (int i = 0; i < _tracks.Count; i++)
                {
                    if (_tracks[i].HandlerType != HandlerTypes.Video && _tracks[i].HandlerType != HandlerTypes.Sound)
                    {
                        WriteTrackFragment(isFlushing, i);
                    }
                }
            }
            while (isFlushing && (_trackReadyFragments.Any(x => x.Count > 0) || _trackSampleSizes.Any(x => x.Count > 0)));
        }

        private void WriteTrackFragment(bool isFlushing, int index)
        {
            Mp4Fragment fragment;

            if (isFlushing)
            {
                if (_trackReadyFragments[index].Count > 0)
                {
                    fragment = _trackReadyFragments[index].Dequeue();
                }
                else if (_trackSampleSizes[index].Count > 0)
                {
                    fragment = CreateNewFragment((uint)index + 1);
                }
                else
                {
                    // at the very end we might not have any samples for a track
                    // continue;
                    return;
                }
            }
            else
            {
                fragment = _trackReadyFragments[index].Dequeue();
            }

            Container fmp4 = new Container();
            uint sequenceNumber = _moofSequenceNumber++;

            CreateMediaFragmentAsync(fmp4, _tracks[index], fragment, sequenceNumber);

            var outputStream = _output.GetStream(sequenceNumber);
            var fragmentStream = new IsoStream(outputStream);

            _trackMoofOffsets[index].Add((ulong)fragmentStream.GetCurrentOffset());
            _trackMoofTime[index].Add(fragment.EndTime);

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

            for (int i = 0; i < _tracks.Count; i++)
            {
                if (!string.IsNullOrEmpty(_tracks[i].CompatibleBrand))
                {
                    compatibleBrands.Insert(1, _tracks[i].CompatibleBrand);
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

            for (int i = 0; i < _tracks.Count; i++)
            {
                var trak = new TrackBox();
                trak.SetParent(moov);
                trak.Children = new List<Box>();
                moov.Children.Add(trak);

                var tkhd = new TrackHeaderBox();
                tkhd.SetParent(trak);
                trak.Children.Add(tkhd);
                tkhd.TrackID = _tracks[i].TrackID;
                tkhd.Reserved1 = new uint[2]; // TODO simplify API
                tkhd.Flags = 0x07;
                _tracks[i].FillTkhdBox(tkhd);
                tkhd.Duration = _durationInMs * MovieTimescale / 1000; 

                var mdia = new MediaBox();
                mdia.SetParent(trak);
                trak.Children.Add(mdia);

                MediaHeaderBox mdhd = new MediaHeaderBox();
                mdhd.SetParent(mdia);
                mdia.Children = new List<Box>();
                mdia.Children.Add(mdhd);
                mdhd.Duration = 0;
                mdhd.Timescale = _tracks[i].Timescale;
                mdhd.Language = _tracks[i].Language;

                HandlerBox hdlr = new HandlerBox();
                hdlr.SetParent(mdia);
                mdia.Children.Add(hdlr);
                hdlr.HandlerType = IsoStream.FromFourCC(_tracks[i].HandlerType);
                hdlr.Name = new BinaryUTF8String(_tracks[i].HandlerName);
                hdlr.Reserved = new uint[3]; // TODO simplify API

                MediaInformationBox minf = new MediaInformationBox();
                minf.SetParent(mdia);
                mdia.Children.Add(minf);
                minf.Children = new List<Box>();

                switch (_tracks[i].HandlerType)
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
                        throw new NotSupportedException(_tracks[i].HandlerType);
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
                
                var sampleEntryBox = _tracks[i].CreateSampleEntryBox();
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

            for (int i = 0; i < _tracks.Count; i++)
            {
                TrackExtendsBox trex = new TrackExtendsBox();
                trex.SetParent(mvex);
                mvex.Children.Add(trex);

                trex.TrackID = _tracks[i].TrackID;
                trex.DefaultSampleDescriptionIndex = 1;
                trex.DefaultSampleDuration = 0;
                trex.DefaultSampleSize = 0;
            }
        }

        private void CreateMediaFragmentAsync(Container fmp4, ITrack track, Mp4Fragment fragment, uint sequenceNumber)
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
            tfhd.Flags = tfhd.Flags | 0x20000u; // DefaultBaseIsMoof - TODO
            
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

        public void FinalizeMp4()
        {
            if (_trackCurrentFragments.Any(x => x.GetLength() > 0))
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

                for (int i = 0; i < _tracks.Count; i++)
                {
                    var track = _tracks[i];

                    var tfra = new TrackFragmentRandomAccessBox();
                    tfra.SetParent(mfra);
                    mfra.Children.Add(tfra);
                    tfra.LengthSizeOfSampleNum = 1;
                    tfra.LengthSizeOfTrafNum = 1;
                    tfra.LengthSizeOfTrunNum = 1;
                    tfra.TrackID = _tracks[i].TrackID;
                    tfra.MoofOffset = _trackMoofOffsets[i].ToArray();
                    tfra.Time = _trackMoofTime[i].ToArray();
                    tfra.TrafNumber = _trackMoofOffsets[i].Select(x => new byte[] { 0, 1 }).ToArray();
                    tfra.TrunNumber = _trackMoofOffsets[i].Select(x => new byte[] { 0, 1 }).ToArray();
                    tfra.SampleDelta = _trackMoofOffsets[i].Select(x => new byte[] { 0, 1 }).ToArray();

                    tfra.NumberOfEntry = (uint)_trackMoofOffsets[i].Count;
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
