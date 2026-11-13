// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
