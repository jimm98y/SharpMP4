using System.Threading.Tasks;

namespace SharpMP4.Builders
{
    public interface IMp4Builder
    {
        Task NotifySampleAddedAsync();
    }
}