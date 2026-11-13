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

using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// Simple text (ISO/IEC 14496-30, 5): an 'stxt' sample entry with the text's media type, and each sample the text.
    /// </summary>
    public class SimpleTextTrack : SubtitleTrackBase
    {
        public override string HandlerName => HandlerNames.Text;
        public override string HandlerType => HandlerTypes.Text;

        /// <summary>The media type of the text: "text/plain", or another's.</summary>
        public string MimeFormat { get; set; } = "text/plain";

        /// <summary>How the text is encoded as a whole - "gzip", say - null for as it is.</summary>
        public string ContentEncoding { get; set; }

        /// <summary>What every sample's text begins with, in 'txtC': of an SVG track, the document the samples animate; null for nothing.</summary>
        public string TextConfig { get; set; }

        public SimpleTextTrack(uint timescale = 1000)
        {
            Timescale = timescale;
        }

        public SimpleTextTrack(SimpleTextSampleEntry entry, uint timescale) : this(timescale)
        {
            SampleEntry = entry;
            MimeFormat = entry.MimeFormat.Length > 0 ? entry.MimeFormat.Text : MimeFormat;
            ContentEncoding = entry.ContentEncodingPresent && entry.ContentEncoding.Length > 0 ? entry.ContentEncoding.Text : null;
            TextConfig = entry._TextConfigBox?.TextConfig.Text;
        }

        public override IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration) =>
            length == 0 ? Array.Empty<SubtitleCue>() : Cue(Encoding.UTF8.GetString(buffer, offset, length), start, duration);

        public override byte[] CreateSample(IReadOnlyList<SubtitleCue> cues) =>
            Encoding.UTF8.GetBytes(string.Concat((cues ?? Array.Empty<SubtitleCue>()).Select(cue => cue.Text)));

        public override Box CreateSampleEntryBox()
        {
            if (SampleEntry != null)
                return CopyOfSampleEntry();

            var entry = new SimpleTextSampleEntry
            {
                DataReferenceIndex = 1,
                ReservedSampleEntry = new byte[6],
                ContentEncoding = Terminated(ContentEncoding ?? ""),
                ContentEncodingPresent = true,
                MimeFormat = Terminated(MimeFormat),
                Children = new List<Box>(),
            };
            if (TextConfig != null)
            {
                var txtC = new TextConfigBox { TextConfig = Terminated(TextConfig) };
                txtC.SetParent(entry);
                entry.Children.Add(txtC);
            }
            return entry;
        }

        public override ITrack Clone() => CopySettingsTo(WithSampleEntryOf(new SimpleTextTrack(Timescale) { MimeFormat = MimeFormat, ContentEncoding = ContentEncoding, TextConfig = TextConfig, Forced = Forced }));
    }
}
