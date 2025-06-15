using SharpMP4.Tracks;
using System.Threading.Tasks;

namespace SharpMP4.Builders
{
    public interface IMp4Builder
    {
        void AddTrack(TrackBase track);
        Task NotifySampleAddedAsync(uint trackID, byte[] sample);
        Task FinalizeAsync();
        Task FlushAsync();
    }
}