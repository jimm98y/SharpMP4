using System;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public class SymbolicMusicSpecificConfig 
    {
        public SymbolicMusicSpecificConfig()
        { }

        public async virtual Task<ulong> ReadAsync(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<ulong> WriteAsync(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public virtual ulong CalculateSize()
        {
            throw new NotImplementedException();
        }
    }

    public class SpatialSpecificConfig 
    {
        public SpatialSpecificConfig()
        { }

        public async virtual Task<ulong> ReadAsync(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<ulong> WriteAsync(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public virtual ulong CalculateSize()
        {
            throw new NotImplementedException();
        }
    }

    public class StructuredAudioSpecificConfig
    {
        public StructuredAudioSpecificConfig()
        { }

        public async virtual Task<ulong> ReadAsync(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<ulong> WriteAsync(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public virtual ulong CalculateSize()
        {
            throw new NotImplementedException();
        }
    }
}
