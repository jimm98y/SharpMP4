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

namespace SharpMP4.Tracks
{
    /// <summary>
    /// Timed metadata (ISO/IEC 14496-12 12.3): a track of the handler 'meta' whose samples are the application's own, each
    /// over its span of time - what was seen in a video's pictures, where a camera was. Its sample entry says what the
    /// samples are: <see cref="UriMetadataTrack"/> of a format a URI names, <see cref="TextMetadataTrack"/> of text of a
    /// MIME type, <see cref="XmlMetadataTrack"/> of XML of a namespace. A track it describes is named by its 'cdsc'
    /// reference (8.3.3.3). Its samples go to the file as they are, each a random access point.
    /// </summary>
    /// <remarks>
    /// A sample may be empty - nothing to say over its span - but ffmpeg fails to read a fragmented file with an empty
    /// sample in a fragment: of a fragmented file, a sample the format reads as saying nothing is the safer choice.
    /// </remarks>
    public abstract class MetadataTrack : TrackBase
    {
        /// <summary>The track reference of a metadata track to the tracks it describes (14496-12 8.3.3.3).</summary>
        public const string DescribesReference = "cdsc";

        private string _handlerType;

        public override string HandlerName => HandlerNames.Metadata;

        /// <summary>'meta', as 14496-12 has it, unless the track was read of another.</summary>
        public override string HandlerType => _handlerType ?? HandlerTypes.Metadata;

        public override string Language { get; set; } = "und";

        /// <summary>The sample entry: 'urim', 'mett' or 'metx'.</summary>
        public abstract string SampleEntryType { get; }

        /// <summary>The tracks the metadata is of: the IDs its 'cdsc' reference names. None where it is of the whole file.</summary>
        public IList<uint> DescribedTrackIDs { get; set; } = new List<uint>();

        /// <summary>A track of the timescale; of the handler given, where it was read of one other than 'meta'.</summary>
        protected MetadataTrack(uint timescale, string handlerType = HandlerTypes.Metadata)
        {
            Timescale = timescale;
            _handlerType = handlerType == HandlerTypes.Metadata ? null : handlerType;
        }

        /// <summary>Whether a sample entry is of timed metadata a <see cref="MetadataTrack"/> reads: 'urim', 'mett' or 'metx'.</summary>
        public static bool IsMetadata(Box entry) => entry is URIMetaSampleEntry or TextMetaDataSampleEntry or XMLMetaDataSampleEntry;

        /// <summary>The track of a metadata sample entry read, of the handler the file has it of; null of an entry no track reads.</summary>
        public static MetadataTrack Create(Box entry, uint timescale, string handlerType = HandlerTypes.Metadata) => entry switch
        {
            URIMetaSampleEntry urim => new UriMetadataTrack(urim, timescale, handlerType),
            TextMetaDataSampleEntry mett => new TextMetadataTrack(mett, timescale, handlerType),
            XMLMetaDataSampleEntry metx => new XmlMetadataTrack(metx, timescale, handlerType),
            _ => null,
        };

        /// <summary>The sample entry the track was read with, which it writes back as it was; null of a track made to be written.</summary>
        public Box SampleEntry
        {
            get => _sampleEntry;
            protected set
            {
                _sampleEntryCopy?.Dispose();
                _sampleEntry = value;
                _sampleEntryCopy = SampleEntryCopy.Of(value);
            }
        }

        private Box _sampleEntry;
        private SampleEntryCopy _sampleEntryCopy;

        public sealed override Box CreateSampleEntryBox()
        {
            if (_sampleEntry != null)
                return _sampleEntryCopy?.Create() ?? _sampleEntry;

            MetaDataSampleEntry entry = CreateMetadataSampleEntry();
            entry.DataReferenceIndex = 1;
            entry.ReservedSampleEntry = new byte[6];
            entry.Children ??= new List<Box>();
            foreach (var child in entry.Children)
                child.SetParent(entry);
            return entry;
        }

        /// <summary>The sample entry of a track made to be written, its fields of the track's; its data reference is set after.</summary>
        protected abstract MetaDataSampleEntry CreateMetadataSampleEntry();

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // no picture: no width and height
        }

        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        /// <summary>Copies what every metadata track has - its handler, the tracks it describes, the entry it was read with - and the track's settings to a clone.</summary>
        protected T CopyMetadataTo<T>(T clone) where T : MetadataTrack
        {
            clone._handlerType = _handlerType;
            clone.DescribedTrackIDs = DescribedTrackIDs?.ToList() ?? new List<uint>();
            clone._sampleEntry = _sampleEntry;
            clone._sampleEntryCopy = _sampleEntryCopy?.Clone();
            return CopySettingsTo(clone);
        }

        /// <summary>
        /// The 'tref' a track's 'trak' holds: of a metadata track of other tracks, its 'cdsc' reference to them. Null where
        /// there is nothing to refer to.
        /// </summary>
        public static TrackReferenceBox CreateTrackReferenceBox(ITrack track)
        {
            if (track is not MetadataTrack metadata || metadata.DescribedTrackIDs == null || metadata.DescribedTrackIDs.Count == 0)
                return null;

            var tref = new TrackReferenceBox { Children = new List<Box>() };
            var cdsc = new TrackReferenceTypeBox(IsoStream.FromFourCC(DescribesReference)) { TrackIDs = metadata.DescribedTrackIDs.ToArray() };
            cdsc.SetParent(tref);
            tref.Children.Add(cdsc);
            return tref;
        }

        /// <summary>The tracks a 'trak' says its track describes: the IDs of its 'cdsc' references.</summary>
        public static List<uint> DescribedTrackIDsOf(TrackBox trak) =>
            trak?.Children?.OfType<TrackReferenceBox>()
                .SelectMany(tref => tref.Children ?? new List<Box>()).OfType<TrackReferenceTypeBox>()
                .Where(reference => reference.FourCC == IsoStream.FromFourCC(DescribesReference))
                .SelectMany(reference => reference.TrackIDs ?? Array.Empty<uint>())
                .ToList() ?? new List<uint>();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sampleEntryCopy?.Dispose();
                _sampleEntryCopy = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>A string of a sample entry: zero terminated.</summary>
        private protected static BinaryUTF8String Terminated(string text) => new BinaryUTF8String(text + "\0");

        /// <summary>A string read of a sample entry: null where it is empty.</summary>
        private protected static string NullIfEmpty(string text) => string.IsNullOrEmpty(text) ? null : text;
    }
}
