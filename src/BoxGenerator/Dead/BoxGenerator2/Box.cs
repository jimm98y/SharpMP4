using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public abstract class Box
    {
        public virtual string FourCC { get; set; }

        protected ulong size = 0;
        public ulong Size { get { return size; } set { size = value; } }
        protected ulong offset = 0;
        public ulong Offset { get { return offset; } set { offset = value; } }

        protected List<Box> children = null;
        public List<Box> Children { get; set; }

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
}
