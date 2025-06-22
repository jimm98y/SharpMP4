using SharpISOBMFF;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMP4.Builders
{
    public class Mp4Fragment
    {
        public Mp4Fragment(ITemporaryStorage storage, ulong startTime, ulong endTime, uint[] sampleSizes)
        {
            this.Storage = storage;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.SampleSizes = sampleSizes;
        }

        public ulong StartTime { get; set; }
        public ulong EndTime { get; set; }
        public uint[] SampleSizes { get; private set; }
        public ITemporaryStorage Storage { get; set; }
    }

    /// <summary>
    /// Fragmented MP4 (fMP4) builder.
    /// </summary>
    public class FragmentedMp4Builder : IMp4Builder
    {
        public uint MovieTimescale { get; set; } = 1000;

        private IMp4Output _output;

        private readonly ulong _maxFragmentLengthInMs;
        private readonly ulong _durationInMs;
        private readonly bool _appendMovieFragmentRandomAccessBox;

        private readonly List<TrackBase> _tracks = new List<TrackBase>();
        private readonly List<ulong> _trackStartTimes = new List<ulong>();
        private readonly List<ulong> _trackEndTimes = new List<ulong>();
        private readonly List<List<uint>> _sampleSizes = new List<List<uint>>();

        private readonly List<Queue<Mp4Fragment>> _readyFragments = new List<Queue<Mp4Fragment>>();
        private List<uint> _trackFragmentCounts = new List<uint>();
        
        private uint _moofSequenceNumber = 1;
        private readonly List<List<ulong>> _moofOffsets = new List<List<ulong>>();
        private readonly List<List<ulong>> _moofTime = new List<List<ulong>>();
        private List<ITemporaryStorage> _trackFragments = new List<ITemporaryStorage>();

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
        public void AddTrack(TrackBase track)
        {
            _tracks.Add(track);
            _trackStartTimes.Add(0);
            _trackEndTimes.Add(0);
            _trackFragmentCounts.Add(0);

            _sampleSizes.Add(new List<uint>());

            var storage = TemporaryStorage.Factory.Create();
            _trackFragments.Add(storage);
            _readyFragments.Add(new Queue<Mp4Fragment>());

            track.TrackID = (uint)_tracks.IndexOf(track) + 1;

            _moofOffsets.Add(new List<ulong>());
            _moofTime.Add(new List<ulong>() { 0 }); // initial time is 0
        }

        public async Task ProcessSampleAsync(uint trackID, byte[] sample)
        {
            _tracks[(int)trackID - 1].ProcessSample(sample, out var processedSample, out var isRandomAccessPoint);
            if (processedSample != null)
            {
                await _trackFragments[(int)trackID - 1].WriteAsync(processedSample, 0, processedSample.Length);
                _sampleSizes[(int)trackID - 1].Add((uint)processedSample.Length);
                _trackEndTimes[(int)trackID - 1] += _tracks[(int)trackID - 1].SampleDuration;

                ulong nextFragmentTime = _tracks[(int)trackID - 1].Timescale * _maxFragmentLengthInMs * (_trackFragmentCounts[(int)trackID - 1] + 1);
                ulong currentFragmentTime = _trackEndTimes[(int)trackID - 1] * 1000;

                if (nextFragmentTime <= currentFragmentTime)
                {
                    var fragment = CreateNewFragment(trackID);
                    _readyFragments[(int)trackID - 1].Enqueue(fragment);

                    var storage = TemporaryStorage.Factory.Create();
                    _trackFragments[(int)trackID - 1] = storage;
                    _trackStartTimes[(int)trackID - 1] = _trackEndTimes[(int)trackID - 1];
                    _sampleSizes[(int)trackID - 1].Clear();
                    _trackFragmentCounts[(int)trackID - 1]++;
                }
            }

            bool isFragmentReady = true;
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_readyFragments[i].Count == 0)
                {
                    isFragmentReady = false;
                    break;
                }
            }
            if (isFragmentReady)
            {
                await WriteFragmentAsync();
            }
        }

        private Mp4Fragment CreateNewFragment(uint trackID)
        {
            var fragment = new Mp4Fragment(_trackFragments[(int)trackID - 1], _trackStartTimes[(int)trackID - 1], _trackEndTimes[(int)trackID - 1], _sampleSizes[(int)trackID - 1].ToArray());
            return fragment;
        }

        private async Task WriteFragmentAsync(bool isFlushing = false)
        {
            // first check if we need to produce the media initialization segment
            if (_moofSequenceNumber == 1)
            {
                Mp4 init = new Mp4();
                await CreateMediaInitializationAsync(init);

                const uint initializationSegmentNumber = 0; // sequence ID 0 is used to indicate "initialization"
                var str = await _output.GetStreamAsync(initializationSegmentNumber);
                IsoStream initializationStream = new IsoStream(str);
                init.Write(initializationStream);
                await _output.FlushAsync(str, initializationSegmentNumber);
            }

            // all tracks have enough samples to produce a fragment
            do
            {
                for (int i = 0; i < _tracks.Count; i++)
                {
                    Mp4Fragment fragment;

                    if (isFlushing)
                    {
                        if (_readyFragments[i].Count > 0)
                            fragment = _readyFragments[i].Dequeue();
                        else if (_sampleSizes[i].Count > 0)
                        {
                            fragment = CreateNewFragment((uint)i + 1);
                            _sampleSizes[i].Clear();
                        }
                        else
                            continue; // at the very end we might not have any samples for a track
                    }
                    else
                    {
                        fragment = _readyFragments[i].Dequeue();
                    }

                    Mp4 fmp4 = new Mp4();
                    uint sequenceNumber = _moofSequenceNumber++;

                    CreateMediaFragmentAsync(fmp4, _tracks[i], fragment, sequenceNumber);

                    var outputStream = await _output.GetStreamAsync(sequenceNumber);
                    var fragmentStream = new IsoStream(outputStream);

                    _moofOffsets[i].Add((ulong)fragmentStream.GetCurrentOffset());
                    _moofTime[i].Add(fragment.EndTime);

                    fmp4.Write(fragmentStream);
                    await _output.FlushAsync(outputStream, sequenceNumber);
                }
            }
            while (isFlushing && (_readyFragments.Any(x => x.Count > 0) || _sampleSizes.Any(x => x.Count > 0)));
        }

        private Task CreateMediaInitializationAsync(Mp4 fmp4)
        {
            var ftyp = new FileTypeBox();
            ftyp.SetParent(fmp4);
            fmp4.Children.Add(ftyp);

            ftyp.MajorBrand = IsoStream.FromFourCC("mp42");
            ftyp.MinorVersion = 1;
          
            var compatibleBrands = new List<string>()
            {
                "mp42",
                "iso5", // "isom",
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

                // minf
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

            var mehd = new MovieExtendsHeaderBox();
            mehd.SetParent(mvex);
            mvex.Children.Add(mehd);

            mehd.FragmentDuration = _durationInMs * MovieTimescale / 1000;

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

            return Task.CompletedTask;
        }

        private void CreateMediaFragmentAsync(Mp4 fmp4, TrackBase track, Mp4Fragment fragment, uint sequenceNumber)
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
                // TODO
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
                // TODO
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
                    SampleDuration = track.SampleDuration,
                    SampleSize = fragment.SampleSizes[k]
                };
            }
            trun.SampleCount = (uint)trun._TrunEntry.Length;

            traf.Children.Add(trun);

            // recalculate offsets
            ulong offset = (moof.CalculateSize() >> 3) + 8;

            trun.DataOffset = (int)offset;
                        
            var mdat = new MediaDataBox();
            fmp4.Children.Add(mdat);

            mdat.Data = new StreamMarker(0, fragment.Storage.GetLength(), new IsoStream(((TemporaryMemory)fragment.Storage).Stream)); // TODO: Fix this ugly workaround!
        }

        public async Task FinalizeAsync()
        {
            if (_trackFragments.Any(x => x.GetLength() > 0))
            {
                await WriteFragmentAsync(true);
            }

            if (_appendMovieFragmentRandomAccessBox)
            {
                Mp4 fmp4 = new Mp4();

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

                    tfra.MoofOffset = _moofOffsets[i].ToArray();
                    tfra.Time = _moofTime[i].ToArray();
                    tfra.TrafNumber = _moofOffsets[i].Select(x => new byte[] { 0, 1 }).ToArray();
                    tfra.TrunNumber = _moofOffsets[i].Select(x => new byte[] { 0, 1 }).ToArray();
                    tfra.SampleDelta = _moofOffsets[i].Select(x => new byte[] { 0, 1 }).ToArray();

                    tfra.NumberOfEntry = (uint)_moofOffsets[i].Count;
                }

                var mfro = new MovieFragmentRandomAccessOffsetBox();
                mfra.SetParent(mfra);
                mfra.Children.Add(mfro);
                mfro.ParentSize = (uint)mfra.CalculateSize() >> 3;

                var fstr = await _output.GetStreamAsync(_moofSequenceNumber);
                var fragmentStream = new IsoStream(fstr);
                fmp4.Write(fragmentStream);
                await _output.FlushAsync(fstr, _moofSequenceNumber);
            }
        }
    }
}
