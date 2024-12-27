using System;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public class SymbolicMusicSpecificConfig  : IMp4Serializable
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

    public class SpatialSpecificConfig : IMp4Serializable
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

    public class StructuredAudioSpecificConfig : IMp4Serializable
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
