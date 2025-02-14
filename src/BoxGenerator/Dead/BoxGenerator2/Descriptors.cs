using System;

namespace SharpMP4
{
    public class SpatialSpecificConfig : IMp4Serializable
    {
        public virtual string DisplayName { get { return nameof(SpatialSpecificConfig); } }
        protected IMp4Serializable parent = null;
        public IMp4Serializable Parent { get { return parent; } set { parent = value; } }
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
        public virtual string DisplayName { get { return nameof(StructuredAudioSpecificConfig); } }
        protected IMp4Serializable parent = null;
        public IMp4Serializable Parent { get { return parent; } set { parent = value; } }
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
}
