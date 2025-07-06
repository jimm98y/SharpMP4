using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    public interface ITrack
    {
        string HandlerName { get; }
        string HandlerType { get; }
        string Language { get; set; }

        uint Timescale { get; set; }
        uint TrackID { get; set; }
        string CompatibleBrand { get; set; }

        int DefaultSampleDuration { get; set; }
        uint DefaultSampleFlags { get; set; }

        Box CreateSampleEntryBox();

        void FillTkhdBox(TrackHeaderBox tkhd);

        void ProcessSample(byte[] sample, out byte[] output, out bool isRandomAccessPoint);
    }
}
