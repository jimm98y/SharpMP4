using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    /// <summary>A track of subtitles or timed text: its samples are cues.</summary>
    public interface ISubtitleTrack : ITrack
    {
        /// <summary>
        /// The cues of a sample shown from <paramref name="start"/> for <paramref name="duration"/>: none of a sample that
        /// shows nothing - an empty 3GPP timed text sample, a WebVTT 'vtte'.
        /// </summary>
        IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration);

        /// <summary>
        /// The sample of the cues shown together over one span of time, all of them from its start to its end - none for
        /// a gap between them.
        /// </summary>
        byte[] CreateSample(IReadOnlyList<SubtitleCue> cues);
    }
}
