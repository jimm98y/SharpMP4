using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public abstract class Box
    {
        public abstract string FourCC { get; }

        public ulong size { get; set; }

        public async virtual Task ReadAsync(Stream stream)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<ulong> WriteAsync(Stream stream)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class FullBox : Box
    {
        public int version { get; set; }
        public int flags { get; set; }

        public int sample_count { get; set; }
    }

    public abstract class VisualSampleGroupEntry : Box
    {

    }
    
    public abstract class SampleEntry : Box
    {

    }
    
    public abstract class ItemFullProperty : Box
    {

    } 
    
    public abstract class ItemProperty : Box
    {

    }
    
    public abstract class LoudnessBaseBox : Box
    {

    }
    
    public abstract class DataEntryBaseBox : Box
    {

    }
    
    public abstract class GeneralTypeBox : Box
    {

    }
    
    public abstract class ItemInfoExtension : Box
    {

    }
    
    public abstract class CompressedBox : Box
    {

    }
    
    public abstract class PlainTextSampleEntry : Box
    {

    }
    
    public abstract class MetadataSampleEntry : Box
    {

    }
    
    public abstract class MPEG2TSSampleEntry : Box
    {

    }
    
    public abstract class RtpReceptionHintSampleEntry : Box
    {

    }
    
    public abstract class SubtitleSampleEntry : Box
    {

    }
    
    public abstract class AudioSampleGroupEntry : Box
    {

    }
    
    public abstract class SampleGroupDescriptionEntry : Box
    {

    }
    
    public abstract class ExtrinsicCameraParametersBox : Box
    {
        // TODO
    }
    
    public abstract class IntrinsicCameraParametersBox : Box
    {
        // TODO
    }

    public class Descriptor
    {

    }
    
    public class OperatingPointsRecord
    {

    }
    
    public class ICC_profile
    {

    }
    
    public class LHEVCDecoderConfigurationRecord
    {

    }
    
    public class HEVCDecoderConfigurationRecord
    {

    }
    
    public class AVCDecoderConfigurationRecord
    {

    }
    
    public class VvcPTLRecord
    {

    }
    
    public class MVDDecoderConfigurationRecord
    {

    }
    
    public class MVCDecoderConfigurationRecord
    {

    }
    
    public class SVCDecoderConfigurationRecord
    {

    }
    
    public class HEVCTileTierLevelConfigurationRecord
    {

    }
    
    public class EVCDecoderConfigurationRecord
    {

    }
    
    public class VvcDecoderConfigurationRecord
    {

    }
    
    public class EVCSliceComponentTrackConfigurationRecord
    {

    }
    
    public class DRCCoefficientsBasic
    {

    }
    
    public class DRCInstructionsBasic
    {

    }
    
    public class DRCCoefficientsUniDRC
    {

    } 
    
    public class DRCInstructionsUniDRC
    {

    }
    
    public abstract class IPMPControlBox : Box
    {

    }
    
    public abstract class MetadataKeyBox : Box
    {

    }
}
