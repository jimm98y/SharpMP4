using SharpMP4.Tracks;

namespace SharpMP4.Builders
{
    public interface IMp4Builder
    {
        void AddTrack(ITrack track);
        void ProcessTrackSample(uint trackID, byte[] sample, int sampleDuration = -1);
        void ProcessMp4Sample(uint trackID, byte[] sample, int sampleDuration = -1, bool isRandomAccessPoint = true);
        void FinalizeMp4();
    }
}