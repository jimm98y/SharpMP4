using SharpMP4.Common;
using System;
using SharpMP4.Encryption;
using SharpMP4.Tracks;

namespace SharpMP4.Builders
{
    /// <summary>
    /// MP4 builders implementing this interface create MP4 files from individual media samples. Disposing a builder lets go
    /// of the temporary storage its samples wait in; it writes nothing that <see cref="FinalizeMedia"/> has not.
    /// </summary>
    public interface IMp4Builder : IDisposable
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
        /// Adds a track whose samples are protected as they are written (ISO/IEC 23001-7). <see cref="TrackProtection.Create"/>
        /// makes a protection with a scheme's defaults.
        /// </summary>
        /// <param name="track">Track to add. <see cref="TrackBase"/>.</param>
        /// <param name="protection">How the track is protected: its scheme, key ID, IVs and pattern, and the protection systems' headers.</param>
        /// <param name="key">The 16 byte key; or 32, for AES-256.</param>
        void AddTrack(ITrack track, TrackProtection protection, byte[] key);

        /// <summary>
        /// Process the sample before storing it in the MP4 container. This calls codec-specific logic inside the track and results in 0 or 1 sample to store in the MP4 container. For video, 
        ///  this call expects the individual NALUs. For AAC audio, this call expects samples with or without ADTS headers.
        /// </summary>
        /// <param name="trackID">Track ID</param>
        /// <param name="sample">Sample bytes.</param>
        /// <param name="sampleDuration">
        /// Duration of the sample in the timescale of the track; negative for the track's default, 0 for a sample of none. Of
        /// video given a NAL unit at a time, the duration of the access unit the NAL unit is of: the builder writes each access
        /// unit with the timing given with its units, though the track gives it back only once the next one starts.
        /// </param>
        /// <param name="compositionOffset">
        /// Composition time minus decode time, in track timescale units. Needed whenever pictures
        /// are coded out of presentation order; leave at 0 for streams that are not reordered.
        /// </param>
        void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration = -1, int compositionOffset = 0);

        /// <summary>
        /// The same, for a sample that sits inside a larger buffer - the buffer a reader fills and
        /// then writes over - so it does not have to be copied out into an array of its own first.
        /// </summary>
        void ProcessTrackSample(uint trackID, ArraySegment<byte> sample, int sampleDuration = -1, int compositionOffset = 0);

        /// <summary>
        /// Processes NAL units of the Annex B byte stream - each behind a start code of three or four bytes, as an
        /// encoder hands them out and a .264, .265 or .266 file holds them - as <see cref="ProcessTrackSample(uint, byte[], int, int)"/>
        /// processes one: the start codes stripped, each NAL unit given to the track in turn, with the duration and
        /// composition offset given. The buffer may hold one NAL unit or many - an access unit, or any part of the stream
        /// that ends where a NAL unit ends. Of H.264, H.265 and H.266 tracks only; the end of the stream is still told
        /// with <see cref="ProcessTrackSample(uint, byte[], int, int)"/> and no sample.
        /// </summary>
        /// <param name="trackID">Track ID</param>
        /// <param name="sample">NAL units, each behind its start code.</param>
        /// <param name="sampleDuration">Duration of the sample in the timescale of the track.</param>
        /// <param name="compositionOffset">Composition time minus decode time, in track timescale units.</param>
        /// <exception cref="NotSupportedException">The track is not of H.264, H.265 or H.266.</exception>
        void ProcessAnnexBTrackSample(uint trackID, byte[] sample, int sampleDuration = -1, int compositionOffset = 0);

        /// <summary>
        /// The same, for NAL units that sit inside a larger buffer, so they do not have to be copied out first.
        /// </summary>
        void ProcessAnnexBTrackSample(uint trackID, ArraySegment<byte> sample, int sampleDuration = -1, int compositionOffset = 0);

        /// <summary>
        /// Store the sample bytes into the MP4 container "as is". For video, this expects the entire AU consisting of multiple NALUs each prefixed by the length.
        /// </summary>
        /// <param name="trackID">Track ID</param>
        /// <param name="sample">Sample bytes.</param>
        /// <param name="sampleDuration">Duration of the sample in the timescale of the track; negative for the track's default, 0 for a sample of none.</param>
        /// <param name="isRandomAccessPoint">true if the sample is a random access point that can be used while seeking. For video this means keyframes, for audio in most cases all the samples are random access points.</param>
        void ProcessRawSample(uint trackID, byte[] sample, int sampleDuration = -1, bool isRandomAccessPoint = true);

        /// <summary>
        /// Called at the very end when there are no more samples. Writes the final boxes and finalizes the output file. It is
        /// called once; samples and tracks added after it are refused.
        /// </summary>
        void FinalizeMedia();
    }

    /// <summary>
    /// What the builders' movie, track and media headers say of time.
    /// </summary>
    internal static class MovieTime
    {
        private static readonly DateTime Epoch = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// A creation or modification time as the headers have it: in seconds since midnight, January 1, 1904, in UTC
        /// (ISO/IEC 14496-12 8.2.2.3).
        /// </summary>
        public static ulong ToIsoTime(DateTime time)
        {
            var utc = time.Kind == DateTimeKind.Local ? time.ToUniversalTime() : DateTime.SpecifyKind(time, DateTimeKind.Utc);
            return utc <= Epoch ? 0 : (ulong)((utc - Epoch).Ticks / TimeSpan.TicksPerSecond);
        }

        /// <summary>
        /// A duration in one timescale in another, rounded up so that a track is never said to end before its last sample
        /// does. In decimal, as the product of a long duration and a timescale overflows 64 bits.
        /// </summary>
        public static ulong Rescale(ulong duration, uint fromTimescale, uint toTimescale)
        {
            if (fromTimescale == 0 || fromTimescale == toTimescale)
                return duration;

            return (ulong)Math.Ceiling((decimal)duration * toTimescale / fromTimescale);
        }

        /// <summary>The version of a box whose times are 32 bits in version 0 and 64 bits in version 1: 1 only where a value needs it.</summary>
        public static byte Version(params ulong[] values)
        {
            foreach (var value in values)
            {
                if (value > uint.MaxValue)
                    return 1;
            }
            return 0;
        }
    }

    /// <summary>
    /// The timing given for the sample a track is assembling. A track that takes a stream a unit at a time and gives back
    /// an access unit only once the next one starts (<see cref="ITrack.ReturnsPreviousSample"/>) gives it back from a call
    /// that brought the next access unit's first unit, with the next one's timing. Written with that, every duration and
    /// composition offset landed one sample late, and the last sample, given back by the flush at the end, had none.
    /// </summary>
    internal sealed class PendingSampleTiming
    {
        private bool _isSet;
        private bool _isGiven;
        private int _duration;
        private int _compositionOffset;

        /// <summary>
        /// The timing to write a sample with that a call to the track gave back, if it did; and what is remembered of the
        /// call for the sample that is being assembled.
        /// </summary>
        /// <param name="track">The track the call went to.</param>
        /// <param name="isFlush">Whether the call had no sample, the flush at the end: nothing is being assembled after it.</param>
        /// <param name="hasOutput">Whether the call gave a sample back.</param>
        /// <param name="sampleDuration">The duration given with the call; that of the sample given back, on return.</param>
        /// <param name="compositionOffset">The composition offset given with the call; that of the sample given back, on return.</param>
        public void Resolve(ITrack track, bool isFlush, bool hasOutput, ref int sampleDuration, ref int compositionOffset)
        {
            // A track that gives back what it was given: the timing is the call's own.
            if (!track.ReturnsPreviousSample)
                return;

            int duration = sampleDuration;
            int offset = compositionOffset;

            // A duration of its own, or an offset: what a caller giving the timing of an access unit with only some of its
            // units - its slices, not its parameter sets - gives with those.
            bool isGiven = duration >= 0 || offset != 0;

            if (hasOutput && _isSet)
            {
                sampleDuration = _duration;
                compositionOffset = _compositionOffset;
            }

            if (isFlush)
            {
                _isSet = false;
                _isGiven = false;
            }
            else if (hasOutput || !_isSet || (!_isGiven && isGiven))
            {
                // The call that ends one access unit brings the first unit of the next, so its timing is the next one's;
                // after it, the units of the access unit being assembled - and those that come after its last picture,
                // which belong to whichever access unit the next picture says - keep it, unless it was only the default.
                _isSet = true;
                _isGiven = isGiven;
                _duration = duration;
                _compositionOffset = offset;
            }
        }
    }
}