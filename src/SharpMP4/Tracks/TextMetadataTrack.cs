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

namespace SharpMP4.Tracks
{
    /// <summary>
    /// Timed metadata of text of a MIME type (ISO/IEC 14496-12 12.3.3): a 'mett' sample entry - "application/json".
    /// </summary>
    public class TextMetadataTrack : MetadataTrack
    {
        public override string SampleEntryType => TextMetaDataSampleEntry.TYPE;

        /// <summary>The MIME type of the samples' text.</summary>
        public string MimeFormat { get; set; }

        /// <summary>The MIME type of how the samples are encoded - "application/zip" - null where they are as they are.</summary>
        public string ContentEncoding { get; set; }

        /// <summary>A track of samples of text of the MIME type: "application/json".</summary>
        public TextMetadataTrack(string mimeFormat, string contentEncoding = null, uint timescale = 1000) : base(timescale)
        {
            if (string.IsNullOrEmpty(mimeFormat))
                throw new ArgumentException("A 'mett' entry says the MIME type of its samples.", nameof(mimeFormat));
            MimeFormat = mimeFormat;
            ContentEncoding = contentEncoding;
        }

        /// <summary>The track of a 'mett' entry read: of the handler the file has it of. The entry is written back as it was.</summary>
        public TextMetadataTrack(TextMetaDataSampleEntry entry, uint timescale, string handlerType = HandlerTypes.Metadata) : base(timescale, handlerType)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            MimeFormat = entry.MimeFormat.Text;
            ContentEncoding = NullIfEmpty(entry.ContentEncodingPresent ? entry.ContentEncoding.Text : null);
            SampleEntry = entry;
        }

        private TextMetadataTrack(uint timescale) : base(timescale) { }

        // content_encoding is always there, an empty string where there is none (14496-12 12.3.3.2): a reader takes a
        // string before the MIME type for the encoding
        protected override MetaDataSampleEntry CreateMetadataSampleEntry() => new TextMetaDataSampleEntry
        {
            ContentEncoding = Terminated(ContentEncoding ?? ""),
            MimeFormat = Terminated(MimeFormat),
            Children = new List<Box>(),
        };

        public override ITrack Clone() =>
            CopyMetadataTo(new TextMetadataTrack(Timescale) { MimeFormat = MimeFormat, ContentEncoding = ContentEncoding });
    }
}
