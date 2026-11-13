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

namespace SharpMP4.Builders
{
    /// <summary>
    /// How <see cref="Mp4Builder"/> writes composition offsets where any is negative - a picture shown before it is
    /// decoded, measured on the decode clock - which version 0 of the 'ctts' box, unsigned, cannot hold as they are.
    /// </summary>
    public enum NegativeCompositionOffsets
    {
        /// <summary>
        /// Every offset shifted by the most negative one, so that all are positive, in version 0; an edit list takes the
        /// shift back out, the presentation starting where the earliest picture is shown. What a reader that honours
        /// edit lists shows is the same as of the offsets as they were.
        /// </summary>
        Shifted,

        /// <summary>
        /// The offsets as they are, in version 0, a negative one in two's complement: as Apple writes them, and as its
        /// readers and ffmpeg read them, as signed. Not as ISO/IEC 14496-12 defines the version; the file's timing is
        /// Apple's own, box for box. The edit list starts at the media's start, as Apple writes it.
        /// </summary>
        AsApple,

        /// <summary>The offsets as they are, in version 1 of 'ctts', which ISO/IEC 14496-12 defines as signed.</summary>
        Version1,
    }
}
