using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    public abstract class TrackBase : ITrack
    {
        public abstract string HandlerName { get; }
        public abstract string HandlerType { get; }
        public abstract string Language { get; set; }

        public uint Timescale { get; set; }
        public uint TrackID { get; set; } = 1;
        public string CompatibleBrand { get; set; } = null;

        public int DefaultSampleDuration { get; set; }
        public uint DefaultSampleFlags { get; set; }

        public abstract Box CreateSampleEntryBox();

        public abstract void FillTkhdBox(TrackHeaderBox tkhd);

        public virtual void ProcessSample(byte[] sample, out byte[] output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = sample;
        }
    }
}
