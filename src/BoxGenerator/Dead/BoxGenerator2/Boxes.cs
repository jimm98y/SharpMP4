namespace SharpMP4
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

    public class IlstKey : Box
    {
        public override string DisplayName { get { return "IlstKey"; } }

        public IlstKey()
        {
        }

        public IlstKey(string boxType) : base(boxType)
        {
        }

        public override ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = 0;
            boxSize += base.Read(stream, readSize);
            boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
            return boxSize;
        }

        public override ulong Write(IsoStream stream)
        {
            ulong boxSize = 0;
            boxSize += base.Write(stream);
            boxSize += stream.WriteBoxArrayTillEnd(this);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoStream.CalculateBoxArray(this);
            return boxSize;
        }
    }

    public sealed class TrunEntry
    {
        public uint SampleDuration;
        public uint SampleSize;
        public uint SampleFlags;
        public uint SampleCompositionTimeOffset0;
        public int SampleCompositionTimeOffset;

        public TrunEntry(uint sampleDuration, uint sampleSize, uint sampleFlags, int sampleCompositionTimeOffset)
        {
            SampleDuration = sampleDuration;
            SampleSize = sampleSize;
            SampleFlags = sampleFlags;
            SampleCompositionTimeOffset = sampleCompositionTimeOffset;
        }

        public TrunEntry()
        { }
    }

    public abstract class CompressedBox : Box 
    {
        public CompressedBox(string boxtype) : base(boxtype) {  }
    }

    public class ICC_profile : UnknownClass { } // ISO 15076‐1 or ICC.1:2010, or https://github.com/xcorail/metadata-extractor/blob/master/Source/com/drew/metadata/icc/IccReader.java#L50
    public class DRCCoefficientsBasic : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCInstructionsBasic : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCCoefficientsUniDRC : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCInstructionsUniDRC : UnknownBox { } // ISO/IEC 23003‐4
    public abstract class UniDrcConfigExtension : UnknownBox { } // ISO/IEC 23003‐4
    public abstract class RtpReceptionHintSampleEntry : Box 
    {
        public RtpReceptionHintSampleEntry(string boxtype) : base(boxtype) { }
    }
    public class OperatingPointsRecord : UnknownClass { }

    public class MetaDataDatatypeBox : UnknownBox { } // missing info
    public abstract class SampleConstructor : Box { }
    public abstract class InlineConstructor : Box { }
    public abstract class NALUStartInlineConstructor : Box { }
    public abstract class SampleConstructorFromTrackGroup : Box { }
    public abstract class IPMPControlBox : Box { }
    public class HEVCTileTierLevelConfigurationRecord : UnknownClass {  }
    public class EVCSliceComponentTrackConfigurationRecord : UnknownClass {  }
    public class VVCSubpicIDRewritingInfomationStruct : UnknownClass { }
}
