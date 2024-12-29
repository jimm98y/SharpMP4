using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMP4
{
    public static class BoxHeaderExtensions
    {
        public static ulong GetBoxSizeInBits(this BoxHeader header)
        {
            if (header.Size == 1)
                return header.Largesize * 8;

            return header.Size * 8;
        }
    }
}
