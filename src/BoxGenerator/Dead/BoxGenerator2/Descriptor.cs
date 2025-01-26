using System.Collections.Generic;

namespace SharpMP4
{
    public static class DescriptorTags
    {
        public const byte Forbidden0 = 0x00;
        public const byte ObjectDescrTag = 0x01;
        public const byte InitialObjectDescrTag = 0x02;
        public const byte ES_DescrTag = 0x03;
        public const byte DecoderConfigDescrTag = 0x04;
        public const byte DecSpecificInfoTag = 0x05;
        public const byte SLConfigDescrTag = 0x06;
        public const byte ContentIdentDescrTag = 0x07;
        public const byte SupplContentIdentDescrTag = 0x08;
        public const byte IPI_DescrPointerTag = 0x09;
        public const byte IPMP_DescrPointerTag = 0x0A;
        public const byte IPMP_DescrTag = 0x0B;
        public const byte QoS_DescrTag = 0x0C;
        public const byte RegistrationDescrTag = 0x0D;
        public const byte ES_ID_IncTag = 0x0E;
        public const byte ES_ID_RefTag = 0x0F;
        public const byte MP4_IOD_Tag = 0x10;
        public const byte MP4_OD_Tag = 0x11;
        public const byte IPL_DescrPointerRefTag = 0x12;
        public const byte ExtendedProfileLevelDescrTag = 0x13;
        public const byte ProfileLevelIndicationIndexDescrTag = 0x14;
        // 0x15-0x3F Reserved for ISO use
        public const byte ContentClassificationDescrTag = 0x40;
        public const byte KeyWordDescrTag = 0x41;
        public const byte RatingDescrTag = 0x42;
        public const byte LanguageDescrTag = 0x43;
        public const byte ShortTextualDescrTag = 0x44;
        public const byte ExpandedTextualDescrTag = 0x45;
        public const byte ContentCreatorNameDescrTag = 0x46;
        public const byte ContentCreationDateDescrTag = 0x47;
        public const byte OCICreatorNameDescrTag = 0x48;
        public const byte OCICreationDateDescrTag = 0x49;
        public const byte SmpteCameraPositionDescrTag = 0x4A;
        // 0x4B-0x5F Reserved for ISO use (OCI Ext.)
        // 0x60-0xBF Reserved for ISO use
        // 0xC0-0xFE User Private
        public const byte ForbiddenF = 0xFF;

        public const byte ExtDescrTagStartRange = 0x80;
        public const byte ExtDescrTagEndRange = 0xFE;
        public const byte OCIDescrTagStartRange = 0x40;
        public const byte OCIDescrTagEndRange = 0x50;
    }

    public class DescriptorFactory
    {
        internal static Descriptor CreateDescriptor(byte tag)
        {
            switch (tag)
            {
                case DescriptorTags.ES_DescrTag:
                    return new ES_Descriptor();

                case DescriptorTags.DecoderConfigDescrTag:
                    return new DecoderConfigDescriptor();

                case DescriptorTags.DecSpecificInfoTag:
                    return new GenericDecoderSpecificInfo();

                case DescriptorTags.SLConfigDescrTag:
                    return new SLConfigDescriptor();

                default:
                    throw new System.NotImplementedException();
                    //return new UnknownDescriptor();
            }
        }
    }

    public class Descriptor : IMp4Serializable
    {
        protected List<Descriptor> children = null;
        public List<Descriptor> Children { get { return children; } set { children = value; } }

        protected ulong sizeOfInstance;
        public ulong SizeOfInstance {  get { return sizeOfInstance; } set {  sizeOfInstance = value; } }

        protected byte[] padding = null;
        public byte[] Padding { get { return padding; } set { padding = value; } }

        protected byte tag = 0;
        public byte Tag { get { return tag; } set { tag = value; } }

        public Descriptor(byte tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// This is here to store the original descriptor length size, because sometimes it is 
        ///  aligned to 4 or 8 bytes instead of just using the shortest one to accommodate the value.
        /// </summary>
        public ulong SizeOfSize { get; set; } = 0;

        public virtual ulong Read(IsoStream stream, ulong readSize)
        {
            return 0;
        }

        public virtual ulong Write(IsoStream stream)
        {
            return (ulong)0;
        }

        public virtual ulong CalculateSize()
        {
            return 0;
        }
    }

    public class UnknownDescriptor : Descriptor
    {
        protected byte[] bytes = null;
        public byte[] Bytes { get { return bytes; } set { bytes = value; } }

        public UnknownDescriptor(byte tag) : base(tag)
        {  }

        public override ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = base.Read(stream, readSize);
            boxSize += stream.ReadUInt8Array((uint)(readSize >> 3), out bytes);
            return boxSize;
        }

        public override ulong Write(IsoStream stream)
        {
            ulong boxSize = base.Write(stream);
            boxSize += stream.WriteUInt8Array((uint)bytes.Length, bytes);
            return boxSize;
        }
        public override ulong CalculateSize()
        {
            ulong boxSize = base.CalculateSize();
            boxSize += (ulong)bytes.Length << 3;
            return boxSize;
        }
    }
}
