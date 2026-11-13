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

        /// <summary>
        /// Whether the track is forced: its cues shown whether subtitles are on or not, as a translation of the dialogue in
        /// another language is. Said by a 'kind' box of the DASH role 'forced-subtitle' (ISO/IEC 14496-12 8.10.4), and of
        /// 3GPP timed text by its sample entry too.
        /// </summary>
        bool Forced { get; set; }
    }
}
