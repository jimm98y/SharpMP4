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
    /// The format of the file a builder writes, of the ISO base media file format's family: what its 'ftyp' says it is, and
    /// what of the format's constraints the builder holds the tracks to as they are added.
    /// </summary>
    public enum Mp4FileFormat
    {
        /// <summary>
        /// MP4 (ISO/IEC 14496-14): 'isom', or 'iso6' of fragments. A track of a sample entry only QuickTime has - 'H261',
        /// 'xd5c' - makes it a QuickTime file.
        /// </summary>
        Mp4,

        /// <summary>QuickTime: 'qt  ', and nothing else, as ffmpeg's .mov has it. The boxes are those of the ISO base media file format.</summary>
        QuickTime,

        /// <summary>Apple's video: 'M4V '.</summary>
        M4V,

        /// <summary>Apple's audio: 'M4A ', of audio tracks - and text, of chapters - only.</summary>
        M4A,

        /// <summary>Apple's audio books: 'M4B ', and 'M4A ', of audio tracks - and text, of chapters - only.</summary>
        M4B,

        /// <summary>
        /// 3GP (3GPP TS 26.244), as its Release 6: '3gp6' - its Basic profile - with '3gp5', '3gp4' and 'isom' as the
        /// specification's branding guidelines list them, but of fragments, which Releases 4 and 5 do not allow, '3gp6'
        /// alone. Of the Basic profile: one track of video, one of audio and one of text at most.
        /// </summary>
        ThreeGpp,

        /// <summary>3GPP2: '3g2a', as ffmpeg's .3g2 has it.</summary>
        ThreeGpp2,
    }
}
