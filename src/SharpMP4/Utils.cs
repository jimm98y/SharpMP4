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
