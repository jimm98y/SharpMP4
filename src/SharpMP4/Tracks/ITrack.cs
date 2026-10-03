using System;
using SharpISOBMFF;
using SharpMP4.Common;
using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// A track: what makes its samples and sample entry. Disposed, it lets go of what it keeps - of a track that writes back
    /// the entry it was read with, the stream the entries it hands out are read from.
    /// </summary>
    public interface ITrack : IDisposable
    {
        string HandlerName { get; }
        string HandlerType { get; }
        string Language { get; set; }

        uint Timescale { get; set; }
        uint TrackID { get; set; }
        string CompatibleBrand { get; set; }

        int DefaultSampleDuration { get; set; }

        /// <summary>
        /// Whether <see cref="ProcessSample(byte[], out ArraySegment{byte}, out bool)"/> gives a sample back only once the
        /// next one starts - the previous access unit, as only what follows it says where it ends - rather than the one
        /// it was given. A builder writes the sample with the timing given for it, not with that of the call it came back from.
        /// </summary>
        bool ReturnsPreviousSample { get; }
        uint DefaultSampleFlags { get; set; }

        uint TimescaleOverride { get; set; }
        int FrameTickOverride { get; set; }
        uint TimescaleFallback { get; set; }
        int FrameTickFallback { get; set; }
        IEnumerable<byte[]> GetContainerSamples();
        /// <summary>
        /// The units a sample is made of, each as a slice of where it already lies. Nothing is
        /// copied, so a unit is used before the next one is asked for.
        /// </summary>
        IEnumerable<ArraySegment<byte>> ParseSample(byte[] sample);

        /// <summary>The same, for a sample that sits inside a larger buffer.</summary>
        IEnumerable<ArraySegment<byte>> ParseSample(byte[] buffer, int offset, int length);

        Box CreateSampleEntryBox();

        void FillTkhdBox(TrackHeaderBox tkhd);

        void ProcessSample(byte[] sample, out ArraySegment<byte> output, out bool isRandomAccessPoint);

        /// <summary>
        /// The same, for a sample that sits inside a larger buffer - the buffer a reader fills and
        /// then writes over - so it does not have to be copied out into an array of its own first.
        /// </summary>
        /// <remarks>
        /// What comes back points into a buffer the track owns, or into the one passed in, so it
        /// is written or copied before the next call rather than kept.
        /// </remarks>
        void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint);

        ITrack Clone();

        IMp4Logger Logger { get; set; }
    }
}
