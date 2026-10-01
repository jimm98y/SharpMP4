using SharpH261;
using SharpISOBMFF;
using System;
using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H.261 track (ITU-T Rec. H.261): its samples pictures, each from its picture start code to the next; in QuickTime's
    /// 'H261' sample entry, which has no configuration - neither ISO/IEC 14496-12 nor 3GPP gives H.261 one, so it is of
    /// QuickTime files only. Its sync samples are the pictures of a freeze picture release (4.3.3), which are the first
    /// coded in INTRA mode after a fast update request (4.3.2) - as ffmpeg's decoder keys them; the stream says no more of
    /// which pictures are intra but in its macroblocks.
    /// </summary>
    public class H261Track : TrackBase
    {
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";

        private readonly H261Context _context = new H261Context();

        private int _width, _height;

        public H261Track()
        {
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 30000;
            FrameTickFallback = 1001;
        }

        public H261Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        /// <summary>A track of an 'H261' entry, as a file has it: its size the entry's.</summary>
        public H261Track(Box entry, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            if (entry is not VisualSampleEntry visual)
                throw new ArgumentException($"Invalid H.261 sample entry: {entry?.FourCC}");
            _width = visual.Width;
            _height = visual.Height;
        }

        /// <summary>
        /// A picture, from its picture start code to the next (<see cref="H261Context.Pictures"/>): a sample of its own, a
        /// sync sample where its header releases a frozen picture. Its header is read for the picture's size.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            output = default;
            isRandomAccessPoint = false;
            if (buffer == null || length == 0)
                return;

            output = new ArraySegment<byte>(buffer, offset, length);
            try
            {
                var picture = _context.ReadPicture(H261Context.StreamOf(buffer, offset, length, Logger));
                isRandomAccessPoint = picture.FreezePictureRelease == 1;
                _width = _context.Width;
                _height = _context.Height;
            }
            catch (Exception ex)
            {
                if (Logger.IsWarningEnabled) Logger.LogWarning($"An H.261 picture header could not be read: {ex.Message}");
            }
        }

        public override Box CreateSampleEntryBox()
        {
            var entry = new VisualSampleEntry(IsoStream.FromFourCC("H261"));
            entry.Children = new List<Box>();
            entry.ReservedSampleEntry = new byte[6];
            entry.PreDefined0 = new uint[3];
            entry.DataReferenceIndex = 1;
            entry.Depth = 24;
            entry.FrameCount = 1;
            entry.Horizresolution = 72 << 16;
            entry.Vertresolution = 72 << 16;
            entry.Width = (ushort)_width;
            entry.Height = (ushort)_height;
            entry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            // CIF and QCIF cover a picture of 4:3 (5): samples of 12:11
            var pasp = new PixelAspectRatioBox { HSpacing = 12, VSpacing = 11 };
            pasp.SetParent(entry);
            entry.Children.Add(pasp);
            return entry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // the width at which the samples are shown square
            tkhd.Width = (uint)(_width * 12 / 11) << 16;
            tkhd.Height = (uint)_height << 16;
        }

        public override ITrack Clone()
        {
            return new H261Track(Timescale, DefaultSampleDuration)
            {
                _width = _width,
                _height = _height,
            };
        }
    }
}
