using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public abstract class Box
    {
        public abstract string FourCC { get; }

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

    public abstract class ItemInfoExtension : Box
    {

    }

    public abstract class PlainTextSampleEntry : Box
    {

    }

    public abstract class MetadataSampleEntry : Box
    {

    }

    public abstract class MetadataKeyBox : Box
    {

    }

    public abstract class UniDrcConfigExtension : Box
    {

    }

    public class ICC_profile
    {
        // ISO 15076‐1 or ICC.1:2010
    }

    public class DRCCoefficientsBasic
    {
        // ISO/IEC 23003‐4
    }

    public class DRCInstructionsBasic
    {
        // ISO/IEC 23003‐4
    }

    public class DRCCoefficientsUniDRC
    {
        // ISO/IEC 23003‐4
    }

    public class DRCInstructionsUniDRC
    {
        // ISO/IEC 23003‐4
    }

    public abstract class ExtrinsicCameraParametersBox : Box { }
    public abstract class IntrinsicCameraParametersBox : Box { }
    public abstract class ItemFullProperty : Box  { } 
    public abstract class ItemProperty : Box { }
    public abstract class DataEntryBaseBox : Box { }
    public abstract class GeneralTypeBox : Box { }
    public abstract class CompressedBox : Box { }
    public abstract class RtpReceptionHintSampleEntry : Box { }
    public class Descriptor { }
    public class OperatingPointsRecord { }
    public class LHEVCDecoderConfigurationRecord { }
    public class HEVCDecoderConfigurationRecord { }
    public class AVCDecoderConfigurationRecord { }
    public class VvcPTLRecord { }
    public class MVDDecoderConfigurationRecord { }
    public class MVCDecoderConfigurationRecord { }
    public class SVCDecoderConfigurationRecord { }
    public class HEVCTileTierLevelConfigurationRecord { }
    public class EVCDecoderConfigurationRecord { }
    public class VvcDecoderConfigurationRecord { }
    public class EVCSliceComponentTrackConfigurationRecord { }
    public class VvcOperatingPointsRecord { }
    public abstract class MPEG4BitRateBox : Box { }
    public abstract class ViewPriorityBox : Box { }
    public class DependencyInfo { }
    public abstract class SampleConstructor : Box { }
    public abstract class InlineConstructor : Box { }
    public abstract class NALUStartInlineConstructor : Box { }
    public abstract class SampleConstructorFromTrackGroup : Box { }
    public class VVCSubpicIDRewritingInfomationStruct { }
    public abstract class IPMPControlBox : Box { }
}
