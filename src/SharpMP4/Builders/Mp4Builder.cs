using SharpISOBMFF;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpMP4.Builders
{
    /// <summary>
    /// MP4 builder.
    /// </summary>
    public class Mp4Builder : IDisposable, IMp4Builder
    {
        public uint MovieTimescale { get; set; } = 1000;

        private IMp4Output _output;

        private readonly ulong _durationInMs = 5312;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private readonly List<TrackBase> _tracks = new List<TrackBase>();
        private readonly List<ulong> _trackEndTimes = new List<ulong>();
        private readonly ulong _fragmentCount;
        private readonly ulong _maxFragmentLengthInMs = 1000;
        private bool _writeInitialization = true;

        private readonly List<List<uint>> _sampleSizes = new List<List<uint>>();
        private readonly List<List<uint>> _sampleOffsets = new List<List<uint>>();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="output">Output stream. Will be progressively written while recording. <see cref="IMp4Output"/>.</param>
        public Mp4Builder(IMp4Output output)
        {
            _output = output;
        }

        /// <summary>
        /// Add a track to the fMP4.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        public void AddTrack(TrackBase track)
        {
            _tracks.Add(track);
            _trackEndTimes.Add(0);
            _sampleSizes.Add(new List<uint>());
            _sampleOffsets.Add(new List<uint>());
            track.TrackID = (uint)_tracks.IndexOf(track) + 1;
            track.SetSink(this);
        }

        public async Task NotifySampleAddedAsync(uint trackID, byte[] sample)
        {
            // make sure only 1 thread at a time can write
            await _semaphore.WaitAsync();
            try
            {
                ulong nextFragmentTime = _tracks[(int)trackID - 1].Timescale * _maxFragmentLengthInMs * (_fragmentCount + 1);
                ulong currentFragmentTime = _trackEndTimes[(int)trackID - 1] * 1000;

                await WriteSample(trackID, sample);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task WriteSample(uint trackID, byte[] sample)
        {
            Mp4 mp4 = new Mp4();

            if (_writeInitialization)
            {
                var ftyp = new FileTypeBox();
                ftyp.SetParent(mp4);
                mp4.Children.Add(ftyp);

                ftyp.MajorBrand = IsoStream.FromFourCC("isom");
                ftyp.MinorVersion = 512;

                var compatibleBrands = new List<string>()
                {
                    "mp41",
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

                //FreeSpaceBox free = new FreeSpaceBox();
                //var storage = TemporaryStorage.Factory.Create();
                //free.Data = new StreamMarker(0, 1024 * 1024 * 13, new IsoStream(((TemporaryMemory)storage).Stream)); // should be enough space to fit the moov box (7 GB file has roughly 4 MB)
                //free.SetParent(mp4);
                //mp4.Children.Add(free);

                _storage = TemporaryStorage.Factory.Create();
                _mdat = new MediaDataBox();
                _mdat.SetParent(mp4);

                // write
                var fstr = await _output.GetStreamAsync(1);
                var fragmentStream = new IsoStream(fstr);
                _size = mp4.Write(fragmentStream) >> 3;
                await _output.FlushAsync(fstr, 1);

                _writeInitialization = false;
            }

            // TODO: review

            var track = _tracks[(int)trackID - 1];
            ulong currentMovieTimeMs = track.Timescale * _maxFragmentLengthInMs * _fragmentCount;
            ulong targetDurationMs = track.Timescale * _maxFragmentLengthInMs;

            var readSample = track.ReadSample();            
            _sampleOffsets[(int)trackID - 1].Add((uint)(_size + 8 + (uint)_storage.GetPosition()));

            await _storage.WriteAsync(readSample, 0, readSample.Length);

            _sampleSizes[(int)trackID - 1].Add((uint)readSample.Length);

            _trackEndTimes[(int)trackID - 1] += track.SampleDuration;
        }

        private async Task WriteMoovAsync(Mp4 mp4)
        {
            var moov = new MovieBox();
            moov.SetParent(mp4);
            mp4.Children.Add(moov);

            var mvhd = new MovieHeaderBox();
            mvhd.SetParent(moov);
            moov.Children = new List<Box>();
            moov.Children.Add(mvhd);
            mvhd.Duration = _durationInMs * MovieTimescale / 1000;
            mvhd.NextTrackID = 2; // TODO 
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
                tkhd.Flags = 0x0F; // 0x1 0x2 0x4 0x8
                _tracks[i].FillTkhdBox(tkhd);
                tkhd.Duration = _durationInMs * MovieTimescale / 1000;

                // optional Edit Box
                EditBox edts = new EditBox();
                edts.SetParent(trak);
                trak.Children.Add(edts);
                edts.Children = new List<Box>();

                EditListBox elst = new EditListBox();
                elst.SetParent(edts);
                edts.Children.Add(elst);
                // TODO
                elst.EditDuration = new ulong[] { _durationInMs };
                elst.MediaTime = new long[] { 0 };
                elst.MediaRateInteger = new short[] { 1 };
                elst.MediaRateFraction = new short[] { 0 };
                elst.EntryCount = 1;

                var mdia = new MediaBox();
                mdia.SetParent(trak);
                trak.Children.Add(mdia);

                MediaHeaderBox mdhd = new MediaHeaderBox();
                mdhd.SetParent(mdia);
                mdia.Children = new List<Box>();
                mdia.Children.Add(mdhd);
                mdhd.Duration = _durationInMs * _tracks[i].Timescale / 1000;
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

                var stts = new TimeToSampleBox();
                stts.SetParent(stbl);
                stbl.Children.Add(stts);
                // TODO: hardcoded now!!!
                stts.SampleCount = new uint[] { (uint)_sampleOffsets[i].Count };
                stts.SampleDelta = new uint[] { (_tracks[i].HandlerType == HandlerTypes.Video) ? 512u : 1024u };
                stts.EntryCount = (uint)stts.SampleCount.Length;

                if (_tracks[i].HandlerType == HandlerTypes.Video)
                {
                    // this box is optional, but it allows for seeking without picture artifacts
                    var stss = new SyncSampleBox();
                    stss.SetParent(stbl);
                    stbl.Children.Add(stss);
                    stss.SampleNumber = new uint[] { 1, }; // TODO: hardcoded now!!! 
                    stss.EntryCount = stss.SampleNumber != null ? (uint)stss.SampleNumber.Length : 0;
                }

                //var ctts = new CompositionOffsetBox();
                //ctts.SetParent(stbl);
                //stbl.Children.Add(ctts);
                // TODO: fill values

                var stsc = new SampleToChunkBox();
                stsc.SetParent(stbl);
                stbl.Children.Add(stsc);

                stsc.FirstChunk = new uint[] { 1 };
                stsc.SamplesPerChunk = new uint[] { 1 };
                stsc.SampleDescriptionIndex = new uint[] { 1 };
                
                stsc.EntryCount = (uint)stsc.FirstChunk.Length;

                var stsz = new SampleSizeBox();
                stsz.SetParent(stbl);
                stbl.Children.Add(stsz);
                stsz.EntrySize = _sampleSizes[i].ToArray();
                stsz.SampleCount = (uint)_sampleSizes[i].Count;

                var stco = new ChunkOffsetBox();
                stco.SetParent(stbl);
                stbl.Children.Add(stco);
                stco.ChunkOffset = _sampleOffsets[i].ToArray();
                stco.EntryCount = (uint)_sampleOffsets[i].Count;
            }
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public async Task FinalizeAsync()
        {
            var mp4 = new Mp4();
            mp4.Children.Add(_mdat);
            _mdat.Data = new StreamMarker(0, _storage.GetLength(), new IsoStream(((TemporaryMemory)_storage).Stream)); // TODO: Fix this ugly workaround!

            await WriteMoovAsync(mp4);
            var fstr = await _output.GetStreamAsync(1);
            var fragmentStream = new IsoStream(fstr);
            mp4.Write(fragmentStream);
            await _output.FlushAsync(fstr, 1);
        }

        #region IDisposable implementation

        private bool disposedValue;
        private ITemporaryStorage _storage;
        private MediaDataBox _mdat;
        private ulong _size;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _semaphore.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable implementation
    }
}
