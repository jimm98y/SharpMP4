using System;

namespace SharpMP4.Readers
{
    public class MediaSample
    {
        // audio/video specific
        public long PTS { get; set; }
        public long DTS { get; set; }
        public int Duration { get; set; } = -1;
        public bool IsRandomAccessPoint { get; set; }

        /// <summary>
        /// The sample, where it lies in the reader's buffer. The next sample of the track is read
        /// over it, so it is used or copied before then.
        /// </summary>
        public ArraySegment<byte> Data { get; set; }

        public MediaSample(long pts, long dts, int duration, ArraySegment<byte> data, bool isRandomAccessPoint = true)
        {
            this.PTS = pts;
            this.DTS = dts;
            this.Duration = duration;
            this.Data = data;
            this.IsRandomAccessPoint = isRandomAccessPoint;
        }
    }
}
