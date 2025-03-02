using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        internal ulong ReadFixed(ulong size, int count, out ushort value)
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

        internal ulong ReadUnsignedInt(ulong size, int count, out BigInteger value)
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
        
        internal ulong ReadSignedInt(ulong size, int count, out int value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadUnsignedIntGolomb(ulong size, out uint value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadSignedIntGolomb(ulong size, out int value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadClass<T>(ulong size, out T value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteBits(int count, byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteFixed(int count, byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteFixed(int count, ushort value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteFixed(int count, bool value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteClass<T>(T value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteUnsignedInt(int count, bool value)
        {
            throw new NotImplementedException();
        }
        
        internal ulong WriteUnsignedInt(int count, BigInteger value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteSignedIntT(int value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteSignedInt(int count, int value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteUnsignedIntT(uint value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadSignedIntT(ulong size, out int value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadUnsignedIntT(ulong size, out uint value)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateSignedIntT(int value)
        {
            throw new NotImplementedException();
        }

        internal ulong CalculateUnsignedIntT(uint value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteUnsignedInt(int count, byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteUnsignedInt(int count, uint value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteUnsignedIntGolomb(uint value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteSignedIntGolomb(int value)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateClassSize<T>(T value)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateUnsignedIntGolomb(uint value)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateSignedIntGolomb(int value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadUnsignedIntVariable(ulong size, out uint value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadSignedIntVariable(ulong size, out int value)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateUnsignedIntVariable(uint value)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateSignedIntVariable(int value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteUnsignedIntVariable(uint value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteSignedIntVariable(int value)
        {
            throw new NotImplementedException();
        }
    }
}
