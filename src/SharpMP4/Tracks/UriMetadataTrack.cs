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
    /// Timed metadata of a format a URI names (ISO/IEC 14496-12 12.3.3): a 'urim' sample entry with the URI in its 'uri '
    /// and what a reader of the format is initialized with in its 'uriI'.
    /// </summary>
    public class UriMetadataTrack : MetadataTrack
    {
        public override string SampleEntryType => URIMetaSampleEntry.TYPE;

        /// <summary>The URI that names the format of the samples.</summary>
        public string Uri { get; set; }

        /// <summary>What a reader of the format is initialized with, the entry's 'uriI'; null for none.</summary>
        public byte[] InitializationData { get; set; }

        /// <summary>A track of samples of a format the URI names, its reader initialized with what is given.</summary>
        public UriMetadataTrack(string uri, byte[] initializationData = null, uint timescale = 1000) : base(timescale)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException("A 'urim' entry names the format of its samples.", nameof(uri));
            Uri = uri;
            InitializationData = initializationData;
        }

        /// <summary>The track of a 'urim' entry read: of the handler the file has it of. The entry is written back as it was.</summary>
        public UriMetadataTrack(URIMetaSampleEntry entry, uint timescale, string handlerType = HandlerTypes.Metadata) : base(timescale, handlerType)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            Uri = entry.TheLabel?.TheURI.Text;
            InitializationData = entry.Init?.UriInitializationData;
            SampleEntry = entry;
        }

        private UriMetadataTrack(uint timescale) : base(timescale) { }

        protected override MetaDataSampleEntry CreateMetadataSampleEntry()
        {
            var urim = new URIMetaSampleEntry { Children = new List<Box>() };
            urim.Children.Add(new URIBox { TheURI = Terminated(Uri) });
            if (InitializationData != null)
                urim.Children.Add(new URIInitBox { UriInitializationData = InitializationData });
            return urim;
        }

        public override ITrack Clone() =>
            CopyMetadataTo(new UriMetadataTrack(Timescale) { Uri = Uri, InitializationData = InitializationData });
    }
}
