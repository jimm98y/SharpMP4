using System;
using System.Threading.Tasks;

namespace SharpMP4
{
    public class SpatialSpecificConfig : IMp4Serializable
    {
        protected byte[] padding = null;
        public byte[] Padding { get { return padding; } set { padding = value; } }

        public SpatialSpecificConfig()
        { }

        public async virtual Task<ulong> ReadAsync(IsoStream stream, ulong readSize)
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
        protected byte[] padding = null;
        public byte[] Padding { get { return padding; } set { padding = value; } }

        public StructuredAudioSpecificConfig()
        { }

        public async virtual Task<ulong> ReadAsync(IsoStream stream, ulong readSize)
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
