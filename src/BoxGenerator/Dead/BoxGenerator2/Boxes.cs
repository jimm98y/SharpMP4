using System.Threading.Tasks;

namespace SharpMP4
{
    public abstract class CompressedBox : Box 
    {
        public CompressedBox(string boxtype) :base(boxtype) {  }
    }
    public class ICC_profile : IMp4Serializable 
    {
        public virtual ulong CalculateSize() { throw new System.NotImplementedException(); }
        public virtual Task<ulong> ReadAsync(IsoStream stream, ulong readSize) { throw new System.NotImplementedException(); }
        public virtual Task<ulong> WriteAsync(IsoStream stream) { throw new System.NotImplementedException(); }
    } // ISO 15076‐1 or ICC.1:2010, or https://github.com/xcorail/metadata-extractor/blob/master/Source/com/drew/metadata/icc/IccReader.java#L50
    public class DRCCoefficientsBasic : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCInstructionsBasic : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCCoefficientsUniDRC : UnknownBox { } // ISO/IEC 23003‐4
    public class DRCInstructionsUniDRC : UnknownBox { } // ISO/IEC 23003‐4
    public abstract class UniDrcConfigExtension : UnknownBox { } // ISO/IEC 23003‐4
    public abstract class DataEntryBaseBox : FullBox 
    {
        protected DataEntryBaseBox(string boxType, uint flags) : base(boxType, 0, flags) { }
    } // ISO/IEC 14496-12:2022, Section 8.7.2.2

    public abstract class RtpReceptionHintSampleEntry : Box 
    {
        public RtpReceptionHintSampleEntry(string boxtype) : base(boxtype) { }
    }
    public class OperatingPointsRecord : IMp4Serializable
    {
        public virtual ulong CalculateSize() { throw new System.NotImplementedException(); }
        public virtual Task<ulong> ReadAsync(IsoStream stream, ulong readSize) { throw new System.NotImplementedException(); }
        public virtual Task<ulong> WriteAsync(IsoStream stream) { throw new System.NotImplementedException(); }
    }

    public class MetaDataDatatypeBox : UnknownBox { } // missing info
    public abstract class SampleConstructor : Box { }
    public abstract class InlineConstructor : Box { }
    public abstract class NALUStartInlineConstructor : Box { }
    public abstract class SampleConstructorFromTrackGroup : Box { }
    public abstract class IPMPControlBox : Box { }
    public class HEVCTileTierLevelConfigurationRecord : IMp4Serializable 
    {
        public virtual ulong CalculateSize() { throw new System.NotImplementedException(); }
        public virtual Task<ulong> ReadAsync(IsoStream stream, ulong readSize) { throw new System.NotImplementedException(); }
        public virtual Task<ulong> WriteAsync(IsoStream stream) { throw new System.NotImplementedException(); }
    }

    public class EVCSliceComponentTrackConfigurationRecord : IMp4Serializable
    {
        public virtual ulong CalculateSize() { throw new System.NotImplementedException(); }
        public virtual Task<ulong> ReadAsync(IsoStream stream, ulong readSize) { throw new System.NotImplementedException(); }
        public virtual Task<ulong> WriteAsync(IsoStream stream) { throw new System.NotImplementedException(); }
    }

    public class VVCSubpicIDRewritingInfomationStruct : IMp4Serializable
    {
        public virtual ulong CalculateSize() { throw new System.NotImplementedException(); }
        public virtual Task<ulong> ReadAsync(IsoStream stream, ulong readSize) { throw new System.NotImplementedException(); }
        public virtual Task<ulong> WriteAsync(IsoStream stream) { throw new System.NotImplementedException(); }
    }
}
