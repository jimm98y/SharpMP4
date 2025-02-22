using System.Collections.Generic;
using System.Linq;

namespace SharpMP4
{
    public interface IHasBoxChildren : IMp4Serializable
    {
        List<Box> Children { get; set; }
    }

    public interface IMp4Serializable
    {
        StreamMarker Padding { get; set; }
        IMp4Serializable GetParent();
        void SetParent(IMp4Serializable parent);
        string DisplayName { get; }

        ulong Read(IsoStream stream, ulong readSize);
        ulong Write(IsoStream stream);
        ulong CalculateSize();
    }

    public abstract class Box : IMp4Serializable, IHasBoxChildren
    {
        public virtual uint FourCC { get; set; }
        public abstract string DisplayName { get; }

        public SafeBoxHeader Header { get; set; } 


        protected byte[] uuid = null;
        public byte[] Uuid { get { return uuid; } set { uuid = value; } }

        protected ulong size = 0;
        public ulong Size { get { return size; } set { size = value; } }

        protected bool hasLargeSize = false;
        public bool HasLargeSize { get { return hasLargeSize; } set { hasLargeSize = value; } }

        protected ulong offset = 0;
        public ulong Offset { get { return offset; } set { offset = value; } }

        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }

        protected List<Box> children = null;
        public List<Box> Children { get { return children; } set { children = value; } }
        protected StreamMarker padding = null;
        public StreamMarker Padding { get { return padding; } set { padding = value; } }

        public Box() {  }

        public Box(uint boxType) 
        {
            FourCC = boxType;    
        }

        public Box(uint boxType, byte[] uuid)
        {
            FourCC = boxType;
            Uuid = uuid;    
        }

        public Box(uint boxType, ulong size) : this(boxType)
        {
            Size = size;
        }

        public virtual ulong Read(IsoStream stream, ulong readSize)
        {
            return 0;
        }

        public virtual ulong Write(IsoStream stream)
        {
            return 0;
        }

        public virtual ulong CalculateSize()
        {
            return (ulong)(32 + 32 + ((ulong)(size >> 3) > uint.MaxValue || hasLargeSize ? 64 : 0)) /* + IsoStream.CalculateBoxArray(this) */ + (ulong)(padding != null ? 8 * padding.Length : 0) + 8 * (ulong)(uuid != null ? uuid.Length : 0);
        }
    }

    public class SafeBoxHeader : BoxHeader
    {
        public override ulong Read(IsoStream stream, ulong readSize)
        {
            byte[] bytes = new byte[32];
            // reading it byte after byte so that we can "roll back" in case of forward-only reading
            int i = 0;
            int j = 0;
            for (i = 0; i < 8; i++)
            {
                int tmp = stream.ReadByteInternal();
                if (tmp == -1)
                    throw new IsoEndOfStreamException(bytes.Take(i).ToArray());
                bytes[i] = (byte)tmp;
            }

            size = (uint)(
                ((uint)bytes[0] << 24) +
                ((uint)bytes[1] << 16) +
                ((uint)bytes[2] << 8) +
                ((uint)bytes[3])
            );

            type = (uint)(
                ((uint)bytes[4] << 24) +
                ((uint)bytes[5] << 16) +
                ((uint)bytes[6] << 8) +
                ((uint)bytes[7])
            );

            if (size == 1)
            {
                for (i = 8; i < 16; i++)
                {
                    int tmp = stream.ReadByteInternal();
                    if (tmp == -1)
                        throw new IsoEndOfStreamException(bytes.Take(i).ToArray());
                    bytes[i] = (byte)tmp;
                }

                largesize = (ulong)(
                    ((ulong)bytes[8] << 56) +
                    ((ulong)bytes[9] << 48) +
                    ((ulong)bytes[10] << 40) +
                    ((ulong)bytes[11] << 32) +
                    ((ulong)bytes[12] << 24) +
                    ((ulong)bytes[13] << 16) +
                    ((ulong)bytes[14] << 8) +
                    ((ulong)bytes[15])
                );
            }

            else if (size == 0)
            {
                /*  box extends to end of file */
            }

            if (type == IsoStream.FromFourCC("uuid"))
            {
                usertype = new byte[16];
                for (j = 0; j < 16; j++)
                {
                    int tmp = stream.ReadByteInternal();
                    if (tmp == -1)
                        throw new IsoEndOfStreamException(bytes.Take(i + j).ToArray());
                    bytes[i + j] = (byte)tmp;
                    usertype[j] = (byte)tmp;
                }
            }
            return (ulong)((i + j) << 3);
        }
    }

    /// <summary>
    /// Unknown box - basically a pass-through.
    /// </summary>
    public class UnknownBox : Box
    {
        public override string DisplayName { get { return nameof(UnknownBox); } }

        // use the StreamMarker so that de do not allocate large amounts of memory in case we have invalid input data
        protected StreamMarker data;
        public StreamMarker Data { get { return this.data; } set { this.data = value; } }

        public UnknownBox()
        {                
        }

        public UnknownBox(uint boxType) : base(boxType)
        {
        }

        public override ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = 0;
            boxSize += base.Read(stream, readSize);
            boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize, out this.data);
            return boxSize;
        }

        public override ulong Write(IsoStream stream)
        {
            ulong boxSize = 0;
            boxSize += base.Write(stream);
            boxSize += stream.WriteUInt8ArrayTillEnd(this.data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)data.Length << 3; // data
            return boxSize;
        }
    }

    public class InvalidBox : UnknownBox
    {
        public InvalidBox(uint fourCC) : base(fourCC)
        {
        }
    }

    public class UnknownEntry : SampleGroupDescriptionEntry
    {
        public override string DisplayName { get { return nameof(UnknownEntry); } }

        protected StreamMarker data;
        public StreamMarker Data { get { return data; } set { data = value; } }

        public UnknownEntry(uint format) : base(format)
        {
        }

        public override ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = 0;
            boxSize += base.Read(stream, readSize);
            boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize, out this.data);
            return boxSize;
        }

        public override ulong Write(IsoStream stream)
        {
            ulong boxSize = 0;
            boxSize += base.Write(stream);
            boxSize += stream.WriteUInt8ArrayTillEnd(this.data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)data.Length << 3; // data
            return boxSize;
        }
    }

    public class UnknownClass : IMp4Serializable
    {
        public virtual string DisplayName { get { return nameof(UnknownClass); } }

        protected StreamMarker data;
        public StreamMarker Data { get { return data; } set { data = value; } }

        protected StreamMarker padding = null;
        public StreamMarker Padding { get { return padding; } set { padding = value; } }

        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }

        public UnknownClass()
        {
        }

        public ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = 0;
            boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize, out this.data);
            return boxSize;
        }

        public ulong Write(IsoStream stream)
        {
            ulong boxSize = 0;
            boxSize += stream.WriteUInt8ArrayTillEnd(this.data);
            return boxSize;
        }

        public ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += (ulong)data.Length << 3; // data
            return boxSize;
        }
    }

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

    public abstract class Descriptor : IMp4Serializable
    {
        public abstract string DisplayName { get; }
        protected List<Descriptor> children = null;
        public List<Descriptor> Children { get { return children; } set { children = value; } }

        protected ulong sizeOfInstance;
        public ulong SizeOfInstance { get { return sizeOfInstance; } set { sizeOfInstance = value; } }

        protected StreamMarker padding = null;
        public StreamMarker Padding { get { return padding; } set { padding = value; } }

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

        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }

        public virtual ulong Read(IsoStream stream, ulong readSize)
        {
            return 0;
        }

        public virtual ulong Write(IsoStream stream)
        {
            return 0;
        }

        public virtual ulong CalculateSize()
        {
            return 0;
        }
    }

    public class InvalidDescriptor : UnknownDescriptor
    {
        public InvalidDescriptor(byte tag) : base(tag)
        {
        }
    }

    public class UnknownDescriptor : Descriptor
    {
        public override string DisplayName { get { return nameof(UnknownDescriptor); } }
        protected StreamMarker data = null;
        public StreamMarker Data { get { return data; } set { data = value; } }

        public UnknownDescriptor(byte tag) : base(tag)
        { }

        public override ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = base.Read(stream, readSize);
            boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize, out this.data);
            return boxSize;
        }

        public override ulong Write(IsoStream stream)
        {
            ulong boxSize = base.Write(stream);
            boxSize += stream.WriteUInt8ArrayTillEnd(this.data);
            return boxSize;
        }
        public override ulong CalculateSize()
        {
            ulong boxSize = base.CalculateSize();
            boxSize += (ulong)data.Length << 3;
            return boxSize;
        }
    }

    public static class BoxHeaderExtensions
    {
        public static ulong GetBoxSizeInBits(this SafeBoxHeader header)
        {
            if (header.Size == 1)
                return header.Largesize << 3;

            ulong size = (ulong)header.Size << 3;
            return size;
        }

        public static ulong GetHeaderSizeInBits(this SafeBoxHeader header)
        {
            return header.CalculateSize();
        }
    }

}
