using System.Threading.Tasks;

namespace SharpMP4
{
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
        public CompressedBox(string boxtype) :base(boxtype) {  }
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
