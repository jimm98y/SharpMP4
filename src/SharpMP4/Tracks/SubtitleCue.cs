
namespace SharpMP4.Tracks
{
    /// <summary>
    /// A cue of a subtitle track: its text, shown from <see cref="Start"/> to <see cref="End"/>, in the track's timescale.
    /// </summary>
    public class SubtitleCue
    {
        public long Start { get; set; }
        public long End { get; set; }

        /// <summary>The text: of WebVTT the cue's payload, of TTML the document, of 3GPP timed text and simple text the sample's text.</summary>
        public string Text { get; set; }

        /// <summary>Of WebVTT: the cue's identifier, where it has one.</summary>
        public string Id { get; set; }

        /// <summary>Of WebVTT: the cue's settings - position, alignment, and so on - where it has any.</summary>
        public string Settings { get; set; }

        public override string ToString() => $"{Start}-{End}: {Text}";
    }
}
