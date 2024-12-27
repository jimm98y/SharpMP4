using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public interface IMp4Serializable
    {
        Task<ulong> ReadAsync(IsoStream stream);
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

        public async virtual Task<ulong> ReadAsync(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<ulong> WriteAsync(IsoStream stream)
        {
            throw new NotImplementedException();
        }

        public virtual ulong CalculateSize()
        {
            throw new NotImplementedException();
        }
    }

    public class UnknownBox : Box  {  }
}
