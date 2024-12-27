namespace BoxGenerator2
{
    public abstract class CompressedBox : Box 
    {
        public CompressedBox(string boxtype) :base(boxtype) {  }
    }
    public class ICC_profile { } // ISO 15076‐1 or ICC.1:2010, or https://github.com/xcorail/metadata-extractor/blob/master/Source/com/drew/metadata/icc/IccReader.java#L50
    public class DRCCoefficientsBasic : Box { } // ISO/IEC 23003‐4
    public class DRCInstructionsBasic : Box { } // ISO/IEC 23003‐4
    public class DRCCoefficientsUniDRC : Box { } // ISO/IEC 23003‐4
    public class DRCInstructionsUniDRC : Box { } // ISO/IEC 23003‐4
    public abstract class UniDrcConfigExtension : Box { } // ISO/IEC 23003‐4
    public abstract class DataEntryBaseBox : FullBox 
    {
        protected DataEntryBaseBox(string boxType, uint flags) : base(boxType, 0, flags) { }
    } // ISO/IEC 14496-12:2022, Section 8.7.2.2

    public abstract class RtpReceptionHintSampleEntry : Box 
    {
        public RtpReceptionHintSampleEntry(string boxtype) : base(boxtype) { }
    }
    public class MetaDataDatatypeBox : Box { } // missing info
    public class OperatingPointsRecord { }

    public abstract class SampleConstructor : Box { }
    public abstract class InlineConstructor : Box { }
    public abstract class NALUStartInlineConstructor : Box { }
    public abstract class SampleConstructorFromTrackGroup : Box { }
    public abstract class IPMPControlBox : Box { }
    public class HEVCTileTierLevelConfigurationRecord { }
    public class EVCSliceComponentTrackConfigurationRecord { }
    public class VVCSubpicIDRewritingInfomationStruct { }
}
