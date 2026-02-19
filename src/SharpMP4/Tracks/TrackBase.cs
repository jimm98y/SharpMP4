using SharpISOBMFF;
using SharpMP4Common;
using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    public abstract class TrackBase : ITrack
    {
        public abstract string HandlerName { get; }
        public abstract string HandlerType { get; }
        public abstract string Language { get; set; }

        public uint Timescale { get; set; }
        public uint TrackID { get; set; } = 1;
        public string CompatibleBrand { get; set; } = null;

        public int DefaultSampleDuration { get; set; }
        public uint DefaultSampleFlags { get; set; }

        public IMp4Logger Logger { get; set; }

        /// <summary>
        /// Overrides any auto-detected timescale.
        /// </summary>
        public uint TimescaleOverride { get; set; }

        /// <summary>
        /// Overrides any auto-detected frame tick.
        /// </summary>
        public int FrameTickOverride { get; set; }

        /// <summary>
        /// If it is not possible to retrieve timescale from the video, use this value as a fallback.
        /// </summary>
        public uint TimescaleFallback { get; set; }

        /// <summary>
        /// If it is not possible to retrieve frame tick from the video, use this value as a fallback.
        /// </summary>
        public int FrameTickFallback { get; set; }

        public abstract Box CreateSampleEntryBox();

        public abstract void FillTkhdBox(TrackHeaderBox tkhd);

        public virtual void ProcessSample(byte[] sample, out byte[] output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = sample;
        }

        public virtual IEnumerable<byte[]> GetContainerSamples()
        {
            return [];
        }

        public virtual IEnumerable<byte[]> ParseSample(byte[] sample)
        {
            return [ sample ];
        }

        public abstract ITrack Clone();
    }
}
