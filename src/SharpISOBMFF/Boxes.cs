using System;
using System.Collections.Generic;

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

    public partial class ALSSpecificConfig
    {
        /// <summary>
        /// The bits of each chan_pos (ISO/IEC 14496-3 11.4.1, Table 11.1): ChBits = ceil[log2(channels+1)],
        /// channels being the number of channels less one.
        /// </summary>
        private static int ChBits(ushort channels)
        {
            int count = channels + 1, bits = 0;
            while ((1 << bits) < count)
                bits++;
            return bits == 0 ? 1 : bits;
        }
    }

    public partial class AudioSpecificConfig
    {
        /// <summary>
        /// An explicit SBR or PS config (ISO/IEC 14496-3 1.6.2.1, Table 1.15) codes audioObjectType twice: first
        /// the signalled type, 5 (SBR) or 29 (PS), then the core type, which <see cref="AudioObjectType"/> is.
        /// This is the first; null where there is only one.
        /// </summary>
        protected GetAudioObjectType signalledAudioObjectType;
        public GetAudioObjectType SignalledAudioObjectType { get { return signalledAudioObjectType; } set { signalledAudioObjectType = value; } }

        /// <summary>
        /// What follows the last field the syntax reads: zero bytes a writer padded with, or the config of an
        /// object type it does not define (USAC's UsacConfig, ISO/IEC 23003-3). Written back as it was.
        /// </summary>
        protected RemainingBits remainder;
        public RemainingBits Remainder { get { return remainder; } set { remainder = value; } }
    }

    public partial class AV2CodecConfigurationBox
    {
        /// <summary>
        /// The OBUs the box carries. The draft's syntax (AV2 Codec ISO Media File Format Binding, 'av2C')
        /// reads config_obu as bytes to the end of the box, which keeps them as they were; its text has each
        /// config_obu a leb128() num_bytes_in_obu and the OBU that many bytes long (AV2 Annex B), which is
        /// how they are split here.
        /// </summary>
        public IReadOnlyList<byte[]> ConfigObus
        {
            get
            {
                var bytes = new List<byte>();
                if (config_obu != null)
                {
                    foreach (var part in config_obu)
                    {
                        if (part != null)
                            bytes.AddRange(part);
                    }
                }

                var obus = new List<byte[]>();
                int position = 0;
                while (position < bytes.Count)
                {
                    long length = 0;
                    int shift = 0;
                    while (position < bytes.Count)
                    {
                        byte b = bytes[position++];
                        length |= (long)(b & 0x7f) << shift;
                        shift += 7;
                        if ((b & 0x80) == 0)
                            break;
                    }

                    int count = (int)System.Math.Min(length, bytes.Count - position);
                    obus.Add(bytes.GetRange(position, count).ToArray());
                    position += count;
                }
                return obus;
            }
        }
    }
}
