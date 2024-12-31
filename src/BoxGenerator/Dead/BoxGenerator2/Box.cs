using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpMP4
{
    public interface IMp4Serializable
    {
        Task<ulong> ReadAsync(IsoStream stream, ulong readSize);
        Task<ulong> WriteAsync(IsoStream stream);
        ulong CalculateSize();
    }

    public abstract class Box : IMp4Serializable
    {
        public virtual string FourCC { get; set; }

        protected ulong size = 0;
        public ulong Size { get { return size; } set { size = value; } }

        protected ulong offset = 0;
        public ulong Offset { get { return offset; } set { offset = value; } }

        protected Box parent = null;
        public Box Parent { get { return parent; } set { parent = value; } }
        protected List<Box> children = null;
        public List<Box> Children { get { return children; } set { children = value; } }

        public Box() {  }

        public Box(string boxType) 
        {
            FourCC = boxType;    
        }

        public Box(string boxType, ulong size) : this(boxType)
        {
            Size = size;
        }

        public virtual Task<ulong> ReadAsync(IsoStream stream, ulong readSize)
        {
            return Task.FromResult<ulong>(0);
        }


        public virtual Task<ulong> WriteAsync(IsoStream stream)
        {
            return Task.FromResult<ulong>(0);
        }

        public virtual ulong CalculateSize()
        {
            return IsoStream.CalculateBoxArray(this);
        }
    }

    public class UnknownBox : Box
    {
        protected byte[] bytes;

        public UnknownBox()
        {                
        }

        public UnknownBox(string boxType) : base(boxType)
        {
        }

        public byte[] Bytes { get { return bytes; } set { bytes = value; } }

        public override async Task<ulong> ReadAsync(IsoStream stream, ulong readSize)
        {
            ulong boxSize = await base.ReadAsync(stream, readSize);
            boxSize += stream.ReadBytes(readSize >> 3, out bytes);
            return boxSize;
        }

        public override async Task<ulong> WriteAsync(IsoStream stream)
        {
            ulong boxSize = await base.WriteAsync(stream);
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

    public class UnknownEntry : IMp4Serializable
    {
        protected byte[] bytes;

        public UnknownEntry()
        {
        }

        public UnknownEntry(string boxType) 
        {
        }

        public byte[] Bytes { get { return bytes; } set { bytes = value; } }

        public virtual async Task<ulong> ReadAsync(IsoStream stream, ulong readSize)
        {
            ulong boxSize = 0;
            boxSize += stream.ReadBytes(readSize >> 3, out bytes);
            return boxSize;
        }

        public virtual async Task<ulong> WriteAsync(IsoStream stream)
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

    public class UnknownClass : IMp4Serializable
    {
        protected byte[] bytes;

        public UnknownClass()
        {
        }

        public byte[] Bytes { get { return bytes; } set { bytes = value; } }

        public virtual async Task<ulong> ReadAsync(IsoStream stream, ulong readSize)
        {
            ulong boxSize = 0;
            boxSize += stream.ReadBytes(readSize >> 3, out bytes);
            return boxSize;
        }

        public virtual async Task<ulong> WriteAsync(IsoStream stream)
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
