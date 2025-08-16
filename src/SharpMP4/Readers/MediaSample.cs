namespace SharpMP4.Readers
{
    public class MediaSample
    {
        public long PTS { get; set; }
        public long DTS { get; set; }
        public int Duration { get; set; } = -1;
        public byte[] Data { get; set; }
        public bool IsRandomAccessPoint { get; set; }
        public MediaSample(long pts, long dts, int duration, byte[] data, bool isRandomAccessPoint = true)
        {
            this.PTS = pts;
            this.DTS = dts;
            this.Duration = duration;
            this.Data = data;
            this.IsRandomAccessPoint = isRandomAccessPoint;
        }
    }
}
