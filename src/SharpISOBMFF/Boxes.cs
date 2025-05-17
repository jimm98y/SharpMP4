using System;

namespace SharpISOBMFF
{
    /// <summary>
    /// Apple ilst->data dataType.
    /// </summary>
    public enum DataType : int
    {
        DataTypeBinary = 0,
        DataTypeStringUTF8 = 1,
        DataTypeStringUTF16 = 2,
        DataTypeStringMac = 3,
        DataTypeStringJPEG = 14,
        DataTypeSignedIntBigEndian = 21,
        DataTypeFloat32BigEndian = 22,
        DataTypeFloat64BigEndian = 23,
    }

    public abstract class CompressedBox : Box 
    {
        public CompressedBox(uint boxtype) : base(boxtype) {  }
    }

    public class ICC_profile : UnknownClass { } // ISO 15076‐1 or ICC.1:2010, or https://github.com/xcorail/metadata-extractor/blob/master/Source/com/drew/metadata/icc/IccReader.java#L50
    public class DRCCoefficientsBasic : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCInstructionsBasic : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCCoefficientsUniDRC : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCInstructionsUniDRC : UnknownBox { } // ISO/IEC 23003‐4
    public abstract class UniDrcConfigExtension : UnknownBox { } // ISO/IEC 23003‐4
    public abstract class RtpReceptionHintSampleEntry : Box 
    {
        public RtpReceptionHintSampleEntry(uint boxtype) : base(boxtype) { }
    }
    public class OperatingPointsRecord : UnknownClass { }

    public class MetaDataDatatypeBox : UnknownBox { } // missing info
    public abstract class SampleConstructor : Box { }
    public abstract class InlineConstructor : Box { }
    public abstract class NALUStartInlineConstructor : Box { }
    public abstract class SampleConstructorFromTrackGroup : Box { }
    public class HEVCTileTierLevelConfigurationRecord : UnknownClass {  }
    public class EVCSliceComponentTrackConfigurationRecord : UnknownClass {  }
    public class VVCSubpicIDRewritingInfomationStruct : UnknownClass { }

    public class SpatialSpecificConfig : IMp4Serializable
    {
        public virtual string DisplayName { get { return nameof(SpatialSpecificConfig); } }
        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }
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
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }
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
