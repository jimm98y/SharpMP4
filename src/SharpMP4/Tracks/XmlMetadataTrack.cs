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
    /// Timed metadata of XML (ISO/IEC 14496-12 12.3.3): a 'metx' sample entry with the namespaces of the samples' XML and
    /// where their schemas are.
    /// </summary>
    public class XmlMetadataTrack : MetadataTrack
    {
        public override string SampleEntryType => XMLMetaDataSampleEntry.TYPE;

        /// <summary>The namespaces of the samples' XML, separated by spaces.</summary>
        public string Namespace { get; set; }

        /// <summary>Where the schemas of its namespaces are, separated by spaces; null for none.</summary>
        public string SchemaLocation { get; set; }

        /// <summary>The MIME type of how the samples are encoded - "application/zip" - null where they are as they are.</summary>
        public string ContentEncoding { get; set; }

        /// <summary>A track of samples of XML of the namespaces, separated by spaces.</summary>
        public XmlMetadataTrack(string ns, string schemaLocation = null, string contentEncoding = null, uint timescale = 1000) : base(timescale)
        {
            if (string.IsNullOrEmpty(ns))
                throw new ArgumentException("A 'metx' entry says the namespace of its samples.", nameof(ns));
            Namespace = ns;
            SchemaLocation = schemaLocation;
            ContentEncoding = contentEncoding;
        }

        /// <summary>The track of a 'metx' entry read: of the handler the file has it of. The entry is written back as it was.</summary>
        public XmlMetadataTrack(XMLMetaDataSampleEntry entry, uint timescale, string handlerType = HandlerTypes.Metadata) : base(timescale, handlerType)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            Namespace = entry.Ns.Text;
            SchemaLocation = NullIfEmpty(entry.SchemaLocationPresent ? entry.SchemaLocation.Text : null);
            ContentEncoding = NullIfEmpty(entry.ContentEncodingPresent ? entry.ContentEncoding.Text : null);
            SampleEntry = entry;
        }

        private XmlMetadataTrack(uint timescale) : base(timescale) { }

        // content_encoding and the schema's location are always there, an empty string where there are none (14496-12 12.3.3.2)
        protected override MetaDataSampleEntry CreateMetadataSampleEntry() => new XMLMetaDataSampleEntry
        {
            ContentEncoding = Terminated(ContentEncoding ?? ""),
            Ns = Terminated(Namespace),
            SchemaLocation = Terminated(SchemaLocation ?? ""),
            Children = new List<Box>(),
        };

        public override ITrack Clone() =>
            CopyMetadataTo(new XmlMetadataTrack(Timescale) { Namespace = Namespace, SchemaLocation = SchemaLocation, ContentEncoding = ContentEncoding });
    }
}
