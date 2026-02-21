using SharpISOBMFF;
using SharpISOBMFF.Extensions;
using SharpMP4.Common;
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
            public List<uint> SampleOffsets { get; set; } = new List<uint>();
            public List<uint> RandomAccessPoints { get; set; } = new List<uint>();

            public TrackContext(ITrack track)
            {
                Track = track;
            }
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
        public IMp4Logger Logger { get; set; } = new DefaultMp4Logger();

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
        
        private uint GetNextTrackId()
        {
            uint trackID = 1;
            while (_trackContexts.ContainsKey(trackID))
                trackID++;
            return trackID;
        }

        private void WriteSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint)
        {
            if (_storage == null)
            {
                _storage = this.TemporaryStorageFactory.Create();
            }

            uint currentSampleDuration = sampleDuration <= 0 ? (uint)_trackContexts[trackID].Track.DefaultSampleDuration : (uint)sampleDuration;
            var track = _trackContexts[trackID];
            track.SampleOffsets.Add((uint)_storage.GetPosition());
            _storage.Write(sample, 0, sample.Length);
            track.SampleSizes.Add((uint)sample.Length);
            track.EndTime += currentSampleDuration;

            if(isRandomAccessPoint)
            {
                track.RandomAccessPoints.Add((uint)track.SampleSizes.Count);
            }
        }

        private MovieBox BuildMoov()
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
                sampleEntryBox.SetParent(stsd);
                stsd.Children.Add(sampleEntryBox);
                stsd.EntryCount = 1;

                var stts = new TimeToSampleBox();
                stts.SetParent(stbl);
                stbl.Children.Add(stts);
                stts.SampleCount = new uint[] { (uint)track.SampleOffsets.Count };
                stts.SampleDelta = new uint[] { (uint)(track.EndTime / (uint)Math.Max(1, track.SampleSizes.Count)) };
                stts.EntryCount = (uint)stts.SampleCount.Length;

                if (track.Track.HandlerType == HandlerTypes.Video)
                {
                    // this box is optional, but it allows for seeking without picture artifacts
                    var stss = new SyncSampleBox();
                    stss.SetParent(stbl);
                    stbl.Children.Add(stss);
                    stss.SampleNumber = track.RandomAccessPoints.ToArray(); 
                    stss.EntryCount = stss.SampleNumber != null ? (uint)stss.SampleNumber.Length : 0;
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

                var stco = new ChunkOffsetBox();
                stco.SetParent(stbl);
                stbl.Children.Add(stco);
                stco.ChunkOffset = track.SampleOffsets.ToArray(); // Temporary offset from the beginning of raw data, we have to add mdat offset later
                stco.EntryCount = (uint)track.SampleOffsets.Count;
            }

            return moov;
        }

        public void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration)
        {
            _trackContexts[trackID].Track.ProcessSample(sample, out var processedSample, out var isRandomAccessPoint);

            if (processedSample != null)
            {
                ProcessRawSample(trackID, processedSample, sampleDuration, isRandomAccessPoint);
            }
        }

        public void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration, bool isRandomAccessPoint)
        {
            WriteSample(trackID, sample, sampleDuration, isRandomAccessPoint);
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
            var moov = BuildMoov();
            moov.SetParent(mp4);
            mp4.Children.Add(moov);

            var mdat = new MediaDataBox();
            mp4.Children.Add(mdat);
            mdat.Data = new StreamMarker(0, _storage.GetLength(), new IsoStream(_storage));

            // we know the final size of the moov box, we can now adjust the stco offsets
            long mdatOffset = ((int)(ftyp.CalculateSize() + moov.CalculateSize()) >> 3) + 4 + (mdat.HasLargeSize ? 8 : 4);
            moov.ModifyChunkOffsets(mdatOffset);

            var stream = _output.GetStream(0);
            var outputStream = new IsoStream(stream);
            mp4.Write(outputStream);
            _output.Flush(stream, 0);
        }
    }
}
