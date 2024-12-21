using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public abstract class Box
    {
        public virtual string FourCC { get; set; }

        protected ulong size = 0;
        public ulong Size { get { return size; } set { size = value; } }

        protected List<Box> children = null;
        public List<Box> Children { get; set; }

        public async virtual Task<ulong> ReadAsync(Stream stream)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<ulong> WriteAsync(Stream stream)
        {
            throw new NotImplementedException();
        }

        public virtual ulong CalculateSize()
        {
            throw new NotImplementedException();
        }
    }

    public class Descriptor : Box
    { }

    public class ICC_profile { } // ISO 15076‐1 or ICC.1:2010
    public class DRCCoefficientsBasic { } // ISO/IEC 23003‐4
    public class DRCInstructionsBasic { } // ISO/IEC 23003‐4
    public class DRCCoefficientsUniDRC { } // ISO/IEC 23003‐4
    public class DRCInstructionsUniDRC { } // ISO/IEC 23003‐4
    public abstract class UniDrcConfigExtension : Box { } // ISO/IEC 23003‐4
    public abstract class DataEntryBaseBox : FullBox { } // ISO/IEC 14496-12:2022, Section 8.7.2.2
    public abstract class CompressedBox : Box { }
    public abstract class RtpReceptionHintSampleEntry : Box { }
    public class OperatingPointsRecord { }
    public class MetaDataDatatypeBox : Box { }

    public abstract class SampleConstructor : Box { }
    public abstract class InlineConstructor : Box { }
    public abstract class SampleConstructorFromTrackGroup : Box { }
    public abstract class IPMPControlBox : Box { }
    public class LHEVCDecoderConfigurationRecord { }    
    public class HEVCTileTierLevelConfigurationRecord { }
    public class EVCDecoderConfigurationRecord { }
    public class EVCSliceComponentTrackConfigurationRecord { }
    public class VvcOperatingPointsRecord { }
    public class VVCSubpicIDRewritingInfomationStruct { }
    public abstract class NALUStartInlineConstructor : Box { }
}
