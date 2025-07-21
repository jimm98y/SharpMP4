using SharpISOBMFF;
using SharpAV1;
using System;
using System.Linq;

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
            // TODO
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
