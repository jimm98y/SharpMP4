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

using SharpMP4.Tracks;
using System;
using System.Collections.Generic;

namespace SharpMP4
{
    public static class Utils
    {
        /// <summary>
        /// The NAL units of a buffer of the Annex B byte stream, without their start codes, as the track's parser reads
        /// them: each one in a buffer the parser reuses, to be used before the next is asked for.
        /// </summary>
        /// <exception cref="NotSupportedException">The track is not of H.264, H.265 or H.266.</exception>
        public static IEnumerable<ArraySegment<byte>> NalUnits(ITrack track, uint trackID, ArraySegment<byte> buffer)
        {
            if (track is not H26XTrackBase h26x)
                throw new NotSupportedException($"The Annex B byte stream is of H.264, H.265 and H.266: track {trackID} is a {track.GetType().Name}.");
            if (buffer.Array == null)
                throw new ArgumentNullException(nameof(buffer));
            return h26x.ParseAnnexB(buffer.Array, buffer.Offset, buffer.Count);
        }

        public static byte[] BigEndian(uint value, int size)
        {
            var bytes = new byte[size];
            for (int i = 0; i < size; i++)
                bytes[i] = (byte)(value >> (8 * (size - 1 - i)));
            return bytes;
        }
    }
}
