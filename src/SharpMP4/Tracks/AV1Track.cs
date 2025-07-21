using SharpISOBMFF;
using SharpAV1;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AV1 Track.
    /// </summary>
    /// <remarks>
    /// https://aomedia.org/specifications/av1/
    /// </remarks>
    public class AV1Track : TrackBase, IVideoTrack
    {
        public const string BRAND = "av01";

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        /// <summary>
        /// Overrides any auto-detected timescale.
        /// </summary>
        public uint TimescaleOverride { get; set; } = 0;

        /// <summary>
        /// Overrides any auto-detected frame tick.
        /// </summary>
        public int FrameTickOverride { get; set; } = 0;

        /// <summary>
        /// If it is not possible to retrieve timescale from the video, use this value as a fallback.
        /// </summary>
        public uint TimescaleFallback { get; set; } = 600;

        /// <summary>
        /// If it is not possible to retrieve frame tick from the video, use this value as a fallback.
        /// </summary>
        public int FrameTickFallback { get; set; } = 25;

        private AV1Context _context = new AV1Context();

        public AV1Track()
        {
            CompatibleBrand = BRAND; // av01
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
        }

        public AV1Track(Box sampleEntry, uint timescale, int sampleDuration)
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;

            VisualSampleEntry visualSampleEntry = (VisualSampleEntry)sampleEntry;
            AV1CodecConfigurationBox av01 = visualSampleEntry.Children.OfType<AV1CodecConfigurationBox>().Single();

            // TODO
        }

        public override Box CreateSampleEntryBox()
        {
            VisualSampleEntry visualSampleEntry = new VisualSampleEntry(IsoStream.FromFourCC(BRAND));
            visualSampleEntry.Children = new List<Box>();
            visualSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            visualSampleEntry.PreDefined0 = new uint[3]; // TODO simplify API

            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            // convert to fixed point 1616
            visualSampleEntry.Horizresolution = 72 << 16; // TODO simplify API
            visualSampleEntry.Vertresolution = 72 << 16; // TODO simplify API

            visualSampleEntry.Width = (ushort)_context._RenderWidth;
            visualSampleEntry.Height = (ushort)_context._RenderHeight;
            visualSampleEntry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            AV1CodecConfigurationBox av01ConfigurationBox = new AV1CodecConfigurationBox();
            av01ConfigurationBox.SetParent(visualSampleEntry);

            av01ConfigurationBox.Av1Config = new AV1CodecConfigurationRecord();

            // TODO
            throw new NotImplementedException();
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Width = (uint)_context._RenderWidth;
            tkhd.Height = (uint)_context._RenderHeight;
        }

        public byte[][] GetVideoUnits()
        {
            throw new NotImplementedException();
        }
    }
}
