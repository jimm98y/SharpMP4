namespace BoxGenerator2
{
    public abstract class CompressedBox : Box { }
    public class ICC_profile { } // ISO 15076‐1 or ICC.1:2010
    public class DRCCoefficientsBasic { } // ISO/IEC 23003‐4
    public class DRCInstructionsBasic { } // ISO/IEC 23003‐4
    public class DRCCoefficientsUniDRC { } // ISO/IEC 23003‐4
    public class DRCInstructionsUniDRC { } // ISO/IEC 23003‐4
    public abstract class UniDrcConfigExtension : Box { } // ISO/IEC 23003‐4
    public abstract class DataEntryBaseBox : FullBox { } // ISO/IEC 14496-12:2022, Section 8.7.2.2
    public abstract class RtpReceptionHintSampleEntry : Box { }
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
