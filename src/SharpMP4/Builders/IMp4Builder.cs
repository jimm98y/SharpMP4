using SharpMP4.Tracks;
using System.Threading.Tasks;

namespace SharpMP4.Builders
{
    public interface IMp4Builder
    {
        void AddTrack(ITrack track);
        Task ProcessSampleAsync(uint trackID, byte[] sample, int sampleDuration = -1);
        Task FinalizeAsync();
    }
}