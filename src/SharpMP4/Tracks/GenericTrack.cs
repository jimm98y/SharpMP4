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

using System;
using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// A track of a codec no other track reads: its samples go to the file as they are, and its sample entry as it was.
    /// </summary>
    public class GenericTrack : TrackBase
    {
        public override string HandlerName { get; }
        public override string HandlerType { get; }
        public override string Language { get; set; } = "und";

        /// <summary>The box the track was made of: of a file, its sample entry's configuration, as a reader hands it.</summary>
        public Box Config { get; }

        // The whole sample entry the configuration is in, as it was read, and the bytes of it the track writes; of an entry
        // that does not write back, no bytes, and the entry itself is written
        private readonly Box _entry;
        private readonly SampleEntryCopy _entryCopy;

        public GenericTrack(Box config, uint timescale, int sampleDuration, uint handlerType, string handlerName) : base()
        {
            Config = config;
            _entry = EntryOf(config);
            _entryCopy = SampleEntryCopy.Of(_entry);
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
            HandlerType = IsoStream.ToFourCC(handlerType);
            HandlerName = handlerName;
        }

        private GenericTrack(GenericTrack track) : base()
        {
            Config = track.Config;
            _entry = track._entry;
            _entryCopy = track._entryCopy?.Clone();
            HandlerType = track.HandlerType;
            HandlerName = track.HandlerName;
        }

        /// <summary>
        /// The sample entry a box is of: a reader hands a track the entry's first box - its configuration, 'dac3', 'dfLa' -
        /// or the entry itself where it has none, so the entry is the box whose parent is the 'stsd'. A box of no 'stsd' is
        /// taken as the entry.
        /// </summary>
        private static Box EntryOf(Box config)
        {
            Box entry = config;
            while (entry != null && entry.GetParent() is Box parent && parent is not SampleDescriptionBox)
                entry = parent;
            return entry;
        }

        /// <summary>
        /// The sample goes to the file as it arrives, so this hands back exactly what it was
        /// given, without copying it anywhere.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        /// <summary>
        /// The sample entry as it was read, a box of its own each time: the writer puts what it is given in its 'stsd'.
        /// </summary>
        public override Box CreateSampleEntryBox()
        {
            return _entryCopy?.Create() ?? _entry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // nothing to do
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new GenericTrack(this));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _entryCopy?.Dispose();
            base.Dispose(disposing);
        }
    }
}
