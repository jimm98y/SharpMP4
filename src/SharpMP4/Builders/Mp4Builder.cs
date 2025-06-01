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
        public uint MovieTimescale { get; set; } = 15360;

        private IMp4Output _output;

        private readonly ulong _durationInMs = 10000;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private readonly List<TrackBase> _tracks = new List<TrackBase>();
        private readonly List<ulong> _trackEndTimes = new List<ulong>();

        private bool _writeInitialization = true;

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
            track.TrackID = (uint)_tracks.IndexOf(track) + 1;
            track.SetSink(this);
        }

        public async Task NotifySampleAddedAsync()
        {
            // make sure only 1 thread at a time can write
            await _semaphore.WaitAsync();
            try
            {
                for (int i = 0; i < _tracks.Count; i++)
                {
                    if (!_tracks[i].ContainsEnoughSamples(1)) // TODO: this should now return true for any sample
                        return;
                }

                await WriteSample();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task WriteSample()
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

                await WriteMoovAsync(mp4);

                _writeInitialization = false;
            }

            // write
            var fstr = await _output.GetStreamAsync(1);
            var fragmentStream = new IsoStream(fstr);
            mp4.Write(fragmentStream);
            await _output.FlushAsync(fstr, 1);
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
                tkhd.Duration = _durationInMs * _tracks[i].Timescale / 1000;
                tkhd.Flags = 0x03;
                _tracks[i].FillTkhdBox(tkhd);

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
                elst.MediaTime = new long[] { 1024 };
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
                mdhd.Duration = _durationInMs * MovieTimescale / 1000;
                mdhd.Timescale = MovieTimescale; // TODO: is this supposed ot be movie timescale, or track timescale?
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
                stts.EntryCount = 1;
                stts.SampleCount = new uint[] { 300 };
                stts.SampleDelta = new uint[] { 512 };

                var stss = new SyncSampleBox();
                stss.SetParent(stbl);
                stbl.Children.Add(stss);
                stss.SampleNumber = new uint[] { 1, 247 }; // TODO: hardcoded now!!!
                stss.EntryCount = stss.SampleNumber != null ? (uint)stss.SampleNumber.Length : 0;

                var sdtp = new SampleDependencyTypeBox();
                sdtp.SetParent(stbl);
                stbl.Children.Add(sdtp);
                // TODO: fill values
                sdtp.IsLeading = new byte[300];
                sdtp.SampleDependsOn = new byte[300];
                sdtp.SampleIsDependedOn = new byte[300];
                sdtp.SampleHasRedundancy = new byte[300];

                var ctts = new CompositionOffsetBox();
                ctts.SetParent(stbl);
                stbl.Children.Add(ctts);
                // TODO: fill values

                var stsc = new SampleToChunkBox();
                stsc.SetParent(stbl);
                stbl.Children.Add(stsc);
                // TODO: fill values

                var stsz = new SampleSizeBox();
                stsz.SetParent(stbl);
                stbl.Children.Add(stsz);
                // TODO: fill values

                var stco = new ChunkOffsetBox();
                stco.SetParent(stbl);
                stbl.Children.Add(stco);
                // TODO: fill values
            }
        }

        public async Task FlushAsync()
        {
            // make sure only 1 thread at a time can write
            await _semaphore.WaitAsync();
            try
            {
                if (_tracks.FirstOrDefault(x => x.HasSamples()) != null)
                {
                    await WriteSample();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }

        #region IDisposable implementation

        private bool disposedValue;

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
