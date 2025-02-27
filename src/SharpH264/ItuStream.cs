using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpH264
{
    public interface IItuSerializable
    {
        ulong Read(ItuStream stream);
    }

    public class ItuStream
    {
        internal int NextBits(int count)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadBits(ulong size, int count, out byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadFixed(ulong size, int count, out bool value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadFixed(ulong size, int count, out byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadUnsignedInt(ulong size, int count, out bool value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadUnsignedInt(ulong size, int count, out byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadUnsignedInt(ulong size, int count, out uint value)
        {
            throw new NotImplementedException();
        }
    }
}
