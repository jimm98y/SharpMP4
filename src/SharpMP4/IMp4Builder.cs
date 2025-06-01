using System.Threading.Tasks;

namespace SharpMP4
{
    public interface IMp4Builder
    {
        Task NotifySampleAdded();
    }
}