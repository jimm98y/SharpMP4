using System;

namespace SharpMP4
{
    public class SpatialSpecificConfig : IMp4Serializable
    {
        protected byte[] padding = null;
        public byte[] Padding { get { return padding; } set { padding = value; } }

        public SpatialSpecificConfig()
        { }

        public virtual ulong Read(IsoStream stream, ulong readSize)
        {
            throw new NotImplementedException();
        }

        public virtual ulong Write(IsoStream stream)
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

        public virtual ulong Read(IsoStream stream, ulong readSize)
        {
            throw new NotImplementedException();
        }

        public virtual ulong Write(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public virtual ulong CalculateSize()
        {
            throw new NotImplementedException();
        }
    }

    public class GenericDecoderSpecificInfo : DecoderSpecificInfo
    {
        protected byte[] bytes = null;
        public byte[] Bytes { get { return bytes; } set { bytes = value; } }

        public override ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = base.Read(stream, readSize);
            boxSize += stream.ReadUInt8Array((uint)(readSize >> 3), out bytes);
            return boxSize;
        }

        public override ulong Write(IsoStream stream)
        {
            ulong boxSize = base.Write(stream);
            boxSize += stream.WriteUInt8Array((uint)bytes.Length, bytes);
            return boxSize;
        }
        public override ulong CalculateSize()
        {
            ulong boxSize = base.CalculateSize();
            boxSize += (ulong)bytes.Length << 3;
            return boxSize;
        }
    }
}
