using System;
using System.Collections.Generic;

namespace SharpMP4
{
    public interface IMp4Serializable
    {
        byte[] Padding { get; set; }

        ulong Read(IsoStream stream, ulong readSize);
        ulong Write(IsoStream stream);
        ulong CalculateSize();
    }

    public abstract class Box : IMp4Serializable
    {
        public virtual string FourCC { get; set; }

        protected byte[] uuid = null;
        public byte[] Uuid { get { return uuid; } set { uuid = value; } }

        protected ulong size = 0;
        public ulong Size { get { return size; } set { size = value; } }

        protected bool hasLargeSize = false;
        public bool HasLargeSize { get { return hasLargeSize; } set { hasLargeSize = value; } }

        protected ulong offset = 0;
        public ulong Offset { get { return offset; } set { offset = value; } }

        protected Box parent = null;
        public Box Parent { get { return parent; } set { parent = value; } }
        protected List<Box> children = null;
        public List<Box> Children { get { return children; } set { children = value; } }
        protected byte[] padding = null;
        public byte[] Padding { get { return padding; } set { padding = value; } }

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

    /// <summary>
    /// Unknown box - basically a pass-through.
    /// </summary>
    public class UnknownBox : Box
    {
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
            boxSize += (ulong)data.Length * 8; // data
            return boxSize;
        }
    }

    public class UnknownEntry : SampleEntry
    {
        protected byte[] bytes;

        public UnknownEntry(string format) : base(format)
        {
        }

        public byte[] Bytes { get { return bytes; } set { bytes = value; } }

        public override ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = base.Read(stream, readSize);
            boxSize += stream.ReadBytes((readSize - boxSize) >> 3, out bytes);
            return boxSize;
        }

        public override ulong Write(IsoStream stream)
        {
            ulong boxSize = base.Write(stream);
            boxSize += stream.WriteBytes((uint)bytes.Length, bytes);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = base.CalculateSize();
            boxSize += (ulong)(bytes.Length * 8);
            return boxSize;
        }
    }

    public class UnknownClass : IMp4Serializable
    {
        protected byte[] bytes;
        public byte[] Bytes { get { return bytes; } set { bytes = value; } }

        protected byte[] padding = null;
        public byte[] Padding { get { return padding; } set { padding = value; } }

        public UnknownClass()
        {
        }

        public virtual ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = 0;
            boxSize += stream.ReadBytes(readSize >> 3, out bytes);
            return boxSize;
        }

        public virtual ulong Write(IsoStream stream)
        {
            ulong boxSize = 0;
            boxSize += stream.WriteBytes((uint)bytes.Length, bytes);
            return boxSize;
        }

        public virtual ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += (ulong)(bytes.Length * 8);
            return boxSize;
        }
    }
}
