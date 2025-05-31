using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpMP4
{
    /// <summary>
    /// Fragmented MP4 (fMP4) builder.
    /// </summary>
    public class FragmentedMp4Builder : IDisposable
    {
        private const int MOVIE_TIMESCALE = 1000;
        private IMp4Output _output;
        private uint _moofSequenceNumber = 1;
        private ulong _fragments;

        private readonly ulong _maxSampleLengthInMs;
        private readonly ulong _durationInMS;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private readonly List<TrackBase> _tracks = new List<TrackBase>();
        private readonly List<ulong> _trackEndTimes = new List<ulong>();
        private readonly List<List<ulong>> _moofOffsets = new List<List<ulong>>();
        private readonly List<List<ulong>> _moofTime = new List<List<ulong>>();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="output">Output stream. Will be progressively written while recording. <see cref="IMp4Output"/>.</param>
        /// <param name="maxSampleLengthInMs">Maximum duration of 1 sample. Default is 2.66 sec.</param>
        /// <param name="durationInMS">Duration of the movie. Default value is 0 for live recordings.</param>
        public FragmentedMp4Builder(IMp4Output output, ulong maxSampleLengthInMs = 2666, ulong durationInMS = 0)
        {
            this._output = output;
            this._maxSampleLengthInMs = maxSampleLengthInMs;
            this._durationInMS = durationInMS;
        }

        /// <summary>
        /// Add a track to the fMP4.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        public void AddTrack(TrackBase track)
        {
            _tracks.Add(track);
            _trackEndTimes.Add(0);
            track.TrackID = (uint)_tracks.IndexOf(track) + 1;
            track.SetSink(this);

            _moofOffsets.Add(new List<ulong>());
            _moofTime.Add(new List<ulong>() { 0 }); // initial time is 0
        }

        public async Task NotifySampleAdded()
        {
            // make sure only 1 thread at a time can write
            await _semaphore.WaitAsync();
            try
            {
                for (int i = 0; i < _tracks.Count; i++)
                {
                    if (!_tracks[i].ContainsEnoughSamples(_tracks[i].Timescale * _maxSampleLengthInMs * (_fragments + 1) - (_trackEndTimes[i] * 1000)))
                        return;
                }

                await WriteFragment();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Flush all samples and create an incomplete fragment (MOOF).
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task FlushAsync()
        {
            // make sure only 1 thread at a time can write
            await _semaphore.WaitAsync();
            try
            {
                if (_tracks.FirstOrDefault(x => x.HasSamples()) != null)
                {
                    await WriteFragment(true);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task WriteFragment(bool isFlushing = false)
        {
            // first check if we need to produce the media initialization segment
            if (_moofSequenceNumber == 1)
            {
                Mp4 init = new Mp4();
                await CreateMediaInitialization(init);

                const uint initializationSegmentNumber = 0; // sequence ID 0 is used to indicate "initialization"
                var str = await _output.GetStreamAsync(initializationSegmentNumber);
                IsoStream initializationStream = new IsoStream(str);
                init.Write(initializationStream);
                await _output.FlushAsync(str, initializationSegmentNumber);

                // TODO: remove this workaround
                var tmp = this._tracks[1];
                this._tracks[1] = this._tracks[0];
                this._tracks[0] = tmp;
            }

            // all tracks have enough samples to produce a fragment
            for (int i = 0; i < this._tracks.Count; i++)
            {
                List<byte[]> fragments = new List<byte[]>();

                ulong currentMovieTimeMs = _tracks[i].Timescale * _maxSampleLengthInMs * _fragments;
                ulong targetDurationMs = _tracks[i].Timescale * _maxSampleLengthInMs;
                ulong currentDuration = 0;

                while ((((currentDuration + _trackEndTimes[i]) * 1000) < (targetDurationMs + currentMovieTimeMs) || isFlushing) && _tracks[i].HasSamples()) // HasSamples is necessary in case the synchronization of the tracks is not precisely aligned
                {
                    var sample = _tracks[i].ReadSample();
                    currentDuration += _tracks[i].SampleDuration;
                    fragments.Add(sample);
                }

                _trackEndTimes[i] += currentDuration;

                Mp4 fmp4 = new Mp4();
                uint sequenceNumber = _moofSequenceNumber++;
                await CreateMediaFragment(fmp4, _tracks[i], _trackEndTimes[i] - currentDuration, fragments, sequenceNumber);

                var fstr = await _output.GetStreamAsync(sequenceNumber);
                var fragmentStream = new IsoStream(fstr);

                _moofOffsets[i].Add((ulong)fragmentStream.GetCurrentOffset());
                _moofTime[i].Add(_trackEndTimes[i]);

                fmp4.Write(fragmentStream);
                await _output.FlushAsync(fstr, sequenceNumber);
            }

            _fragments++;
        }

        private Task CreateMediaInitialization(Mp4 fmp4)
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
            mvhd.Duration = _durationInMS * MOVIE_TIMESCALE / 1000;
            mvhd.NextTrackID = 0xFFFFFFFF; // TODO simplify API
            mvhd.Timescale = MOVIE_TIMESCALE; // just for movie time: https://stackoverflow.com/questions/77803940/diffrence-between-mvhd-box-timescale-and-mdhd-box-timescale-in-isobmff-format
            mvhd.Reserved0 = new uint[2]; // TODO simplify API
            mvhd.PreDefined = new uint[6]; // TODO simplify API

            for (int i = 0; i < this._tracks.Count; i++)
            {
                var trak = new TrackBox();
                trak.SetParent(moov);
                trak.Children = new List<Box>();
                moov.Children.Add(trak);

                var tkhd = new TrackHeaderBox();
                tkhd.SetParent(trak);
                trak.Children.Add(tkhd);
                tkhd.TrackID = this._tracks[i].TrackID;
                tkhd.Reserved1 = new uint[2]; // TODO simplify API
                tkhd.Duration = _durationInMS * MOVIE_TIMESCALE / 1000; 
                tkhd.Flags = 0x07;
                this._tracks[i].FillTkhdBox(tkhd);

                var mdia = new MediaBox();
                mdia.SetParent(trak);
                trak.Children.Add(mdia);

                MediaHeaderBox mdhd = new MediaHeaderBox();
                mdhd.SetParent(mdia);
                mdia.Children = new List<Box>();
                mdia.Children.Add(mdhd);
                mdhd.Duration = 0;
                mdhd.Timescale = this._tracks[i].Timescale;
                mdhd.Language = this._tracks[i].Language;

                HandlerBox hdlr = new HandlerBox();
                hdlr.SetParent(mdia);
                mdia.Children.Add(hdlr);
                hdlr.HandlerType = IsoStream.FromFourCC(this._tracks[i].HandlerType);
                hdlr.Name = new BinaryUTF8String(this._tracks[i].HandlerName);
                hdlr.Reserved = new uint[3]; // TODO simplify API

                // minf
                MediaInformationBox minf = new MediaInformationBox();
                minf.SetParent(mdia);
                mdia.Children.Add(minf);
                minf.Children = new List<Box>();

                switch (this._tracks[i].HandlerType)
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
                        throw new NotSupportedException(this._tracks[i].HandlerType);
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
                
                var sampleEntryBox = this._tracks[i].CreateSampleEntryBox();
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

            mehd.FragmentDuration = _durationInMS * MOVIE_TIMESCALE / 1000;

            for (int i = 0; i < this._tracks.Count; i++)
            {
                TrackExtendsBox trex = new TrackExtendsBox();
                trex.SetParent(mvex);
                mvex.Children.Add(trex);

                trex.TrackID = this._tracks[i].TrackID;
                trex.DefaultSampleDescriptionIndex = 1;
                trex.DefaultSampleDuration = 0;
                trex.DefaultSampleSize = 0;
                // TODO: trex.A = SampleFlags;
            }

            return Task.CompletedTask;
        }

        private async Task CreateMediaFragment(Mp4 fmp4, TrackBase track, ulong startTime, List<byte[]> fragments, uint sequenceNumber)
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
            tfdt.BaseMediaDecodeTime = startTime; // BaseMediaDecodeTime must be in the timescale of the track

            TrackRunBox trun = new TrackRunBox();
            trun.SetParent(traf);

            // TODO FirstSampleFlags
            if (track.HandlerType == HandlerTypes.Video)
            {
                // TODO
                trun.FirstSampleFlags = 33554432;
                trun.Flags = 0x305;
            }
            else
            {
                trun.Flags = 0x301;
            }
                
            trun.DataOffset = 0;

            trun._TrunEntry = new TrunEntry[fragments.Count];
            for (int k = 0; k < fragments.Count; k++)
            {
                var sample = fragments[k];
                trun._TrunEntry[k] = new TrunEntry(0, 0)
                {
                    Flags = trun.Flags,
                    SampleDuration = track.SampleDuration,
                    SampleSize = (uint)sample.Length
                };
            }
            trun.SampleCount = (uint)trun._TrunEntry.Length;

            traf.Children.Add(trun);

            // recalculate offsets
            ulong offset = (moof.CalculateSize() >> 3) + 8;

            trun.DataOffset = (int)offset;

            var storage = TemporaryStorage.Factory.Create();
            var mdat = new MediaDataBox();
            fmp4.Children.Add(mdat);
            for (int i = 0; i < fragments.Count; i++)
            {
                await storage.WriteAsync(fragments[i], 0, fragments[i].Length);
            }
            mdat.Data = new StreamMarker(0, storage.GetLength(), new IsoStream(((TemporaryMemory)storage).Stream)); // TODO: Fix this ugly workaround!!!!!!!!!!!!!!!!!!!!!!
        }

        public async Task FinalizeAsync()
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

            // final random access box
            var fstr = await _output.GetStreamAsync(_moofSequenceNumber);
            var fragmentStream = new IsoStream(fstr);
            fmp4.Write(fragmentStream);
            await _output.FlushAsync(fstr, _moofSequenceNumber);
        }

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _semaphore.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }    
}
