using SharpMP4.Tracks;
using System.Threading.Tasks;

namespace SharpMP4.Builders
{
    public interface IMp4Builder
    {
        void AddTrack(ITrack track);
        Task ProcessTrackSampleAsync(uint trackID, byte[] sample, int sampleDuration = -1);
        Task ProcessMp4SampleAsync(uint trackID, byte[] sample, int sampleDuration = -1, bool isRandomAccessPoint = true);
        Task FinalizeAsync();
    }
}