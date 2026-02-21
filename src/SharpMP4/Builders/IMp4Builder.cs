using SharpMP4.Common;
using SharpMP4.Tracks;

namespace SharpMP4.Builders
{
    /// <summary>
    /// MP4 builders implementing this interface create MP4 files from individual media samples.
    /// </summary>
    public interface IMp4Builder
    {
        /// <summary>
        /// Timescale of the movie.
        /// </summary>
        uint MovieTimescale { get; set; }

        /// <summary>
        /// MP4 logger. The logger is also passed to the tracks and can be used for logging inside the track logic. The logger can be set at any time, but it is recommended to set it before adding any tracks or processing any samples.
        /// The default logger is <see cref="DefaultMp4Logger"/>.
        /// </summary>
        IMp4Logger Logger { get; set; }

        /// <summary>
        /// Adds a new track to the MP4 container.
        /// </summary>
        /// <param name="track">Track to add. <see cref="TrackBase"/>.</param>
        void AddTrack(ITrack track);

        /// <summary>
        /// Process the sample before storing it in the MP4 container. This calls codec-specific logic inside the track and results in 0 or 1 sample to store in the MP4 container. For video, 
        ///  this call expects the individual NALUs. For AAC audio, this call expects samples with or without ADTS headers.
        /// </summary>
        /// <param name="trackID">Track ID</param>
        /// <param name="sample">Sample bytes.</param>
        /// <param name="sampleDuration">Duration of the sample in the timescale of the track.</param>
        void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration = -1);

        /// <summary>
        /// Store the sample bytes into the MP4 container "as is". For video, this expects the entire AU consisting of multiple NALUs each prefixed by the length.
        /// </summary>
        /// <param name="trackID">Track ID</param>
        /// <param name="sample">Sample bytes.</param>
        /// <param name="sampleDuration">Duration of the sample in the timescale of the track.</param>
        /// <param name="isRandomAccessPoint">true if the sample is a random access point that can be used while seeking. For video this means keyframes, for audio in most cases all the samples are random access points.</param>
        void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration = -1, bool isRandomAccessPoint = true);

        /// <summary>
        /// Called at the very end when there are no more samples. Writes the final boxes and finalizes the output file.
        /// </summary>
        void FinalizeMedia();
    }
}