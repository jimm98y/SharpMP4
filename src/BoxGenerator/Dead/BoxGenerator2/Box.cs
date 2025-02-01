using System.Collections.Generic;
using System.Linq;

namespace SharpMP4
{
    public interface IHasBoxChildren
    {
        List<Box> Children { get; set; }
    }

    public interface IMp4Serializable
    {
        StreamMarker Padding { get; set; }
        IMp4Serializable Parent { get; set; }
        string DisplayName { get; }

        ulong Read(IsoStream stream, ulong readSize);
        ulong Write(IsoStream stream);
        ulong CalculateSize();
    }

    public abstract class Box : IMp4Serializable, IHasBoxChildren
    {
        public virtual string FourCC { get; set; }
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
        public IMp4Serializable Parent { get { return parent; } set { parent = value; } }
        protected List<Box> children = null;
        public List<Box> Children { get { return children; } set { children = value; } }
        protected StreamMarker padding = null;
        public StreamMarker Padding { get { return padding; } set { padding = value; } }

        public Box() {  }

        public Box(string boxType) 
        {
            FourCC = boxType;    
        }

        public Box(string boxType, byte[] uuid)
        {
            FourCC = boxType;
            Uuid = uuid;    
        }

        public Box(string boxType, ulong size) : this(boxType)
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

        public UnknownBox(string boxType) : base(boxType)
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
        public InvalidBox(string fourCC) : base(fourCC)
        {
        }
    }

    public class UnknownEntry : SampleEntry
    {
        public override string DisplayName { get { return nameof(UnknownEntry); } }

        protected StreamMarker data;
        public StreamMarker Data { get { return data; } set { data = value; } }

        public UnknownEntry(string format) : base(format)
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
        public IMp4Serializable Parent { get { return parent; } set { parent = value; } }

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
}
