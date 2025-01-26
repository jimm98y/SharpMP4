using System;

namespace SharpMP4
{
    public class SpatialSpecificConfig : IMp4Serializable
    {
        protected StreamMarker padding = null;
        public StreamMarker Padding { get { return padding; } set { padding = value; } }

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
        protected StreamMarker padding = null;
        public StreamMarker Padding { get { return padding; } set { padding = value; } }

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
        protected StreamMarker data = null;
        public StreamMarker Data { get { return data; } set { data = value; } }

        public override ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = base.Read(stream, readSize);
            boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize, out this.data);
            return boxSize;
        }

        public override ulong Write(IsoStream stream)
        {
            ulong boxSize = base.Write(stream);
            boxSize += stream.WriteUInt8ArrayTillEnd(this.data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = base.CalculateSize();
            boxSize += (ulong)data.Length << 3;
            return boxSize;
        }
    }
}
