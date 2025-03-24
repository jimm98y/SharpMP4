using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SharpH264
{
    public interface IItuSerializable
    {
        ulong Read(ItuStream stream);
        ulong Write(ItuStream stream);
        int HasMoreRbspData { get; set; }
    }

    public class ItuStream : IDisposable
    {
        private bool _shouldEscapeNals = true;
        private int _bitsPosition;
        private int _currentBytePosition = -1;
        private byte _currentByte = 0;
        private int _prevByte = -1;
        private int _prevPrevByte = -1;

        private readonly Stream _stream;

        private int _rbspDataCounter = 0;

        private bool _disposedValue;

        public ItuStream(Stream stream)
        {
            this._stream = stream;
        }

        #region Bit read/write

        private int ReadByte()
        {
            int ret = _stream.ReadByte();
            return ret;
        }

        private long WriteByte(byte value)
        {
            _stream.WriteByte(value);
            return 8;
        }

        private int ReadBit()
        {
            int bytePos = _bitsPosition / 8;

            if (_currentBytePosition != bytePos)
            {
                byte b = (byte)ReadByte();

                // remove emulation prevention byte
                if (_shouldEscapeNals && _prevByte == 0 && _currentByte == 0 && b == 0x03)
                {
                    _prevByte = b;
                    b = (byte)ReadByte();
                    _bitsPosition += 8;
                    bytePos++;
                }
                else
                {
                    _prevByte = _currentByte;
                }

                _currentByte = b;
                _currentBytePosition = bytePos;
            }

            int posInByte = 7 - _bitsPosition % 8;
            int bit = _currentByte >> posInByte & 1;
            ++_bitsPosition;
            return bit;
        }

        private long ReadBits(int count)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            long res = 0;
            while (count > 0)
            {
                res = res << 1;
                int u1 = ReadBit();

                if (u1 == -1)
                    return u1;

                res |= (byte)u1;
                count--;
            }

            return res;
        }

        public void WriteBit(int value)
        {
            int posInByte = 7 - (int)_bitsPosition % 8;
            int bit = (value & 1) << posInByte;
            _currentByte = (byte)(_currentByte | bit);
            ++_bitsPosition;

            int bytePos = _bitsPosition / 8;
            if (_currentBytePosition != bytePos)
            {
                if(_currentBytePosition < 0)
                {
                    _currentBytePosition = bytePos;
                    return;
                }

                if (_shouldEscapeNals)
                {
                    // write emulation prevention byte to properly escape sequences of 0x000000, 0x000001, 0x000002, 0x000003 into 0x00000300, 0x00000301, 0x00000302 and 0x00000303 respectively
                    if (_prevByte == 0x00 && _prevPrevByte == 0x00 && (_currentByte == 0x00 || _currentByte == 0x01 || _currentByte == 0x02 || _currentByte == 0x03))
                    {
                        WriteByte(0x03);
                        bytePos++;
                        _bitsPosition += 8;
                        _prevByte = 0x03;
                    }
                }

                WriteByte(_currentByte);
                _currentBytePosition = bytePos;

                _prevPrevByte = _prevByte;
                _prevByte = _currentByte;

                _currentByte = 0;
            }
        }

        private void WriteBits(int count, long value)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count > 0)
            {
                int bits = count - 1;
                long mask = 0x1 << bits;
                WriteBit((int)((value & mask) >> bits));
                count--;
            }
        }

        #endregion // Bit read/write

        internal bool ByteAligned()
        {
            return _bitsPosition % 8 == 0;
        }

        internal ulong ReadClass<T>(ulong size, Func<T> factory, out T value) where T : IItuSerializable
        {
            T c = factory();
            ulong read = c.Read(this);
            value = c;
            return read;
        }

        internal ulong WriteClass<T>(T value) where T : IItuSerializable
        {
            ulong size = value.Write(this);
            return size;
        }

        internal ulong ReadUnsignedInt(ulong size, int count, out byte value)
        {
            if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(size, count, out uint v);
            value = (byte)v;
            return read;
        }

        internal ulong WriteUnsignedInt(int count, byte value)
        {
            if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong size = WriteUnsignedInt(count, (uint)value);
            return size;
        }

        internal ulong ReadFixed(ulong size, int count, out uint value)
        {
            return ReadUnsignedInt(size, count, out value);
        }

        internal ulong WriteFixed(int count, uint value)
        {
            return WriteUnsignedInt(count, value);
        }

        internal ulong ReadUnsignedInt(ulong size, int count, out uint value)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            long ret = ReadBits(count);
            value = (uint)ret;
            return (ulong)count;
        }

        internal ulong WriteUnsignedInt(int count, uint value)
        {
            WriteBits(count, value);
            return (ulong)count;
        }

        internal ulong ReadSignedInt(ulong size, int count, out int value)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            long ret = ReadBits(count);
            value = unchecked((int)ret);
            return (ulong)count;
        }

        internal ulong WriteSignedInt(int count, int value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadUnsignedIntGolomb(ulong size, out uint value)
        {
            int cnt = 0;
            while (ReadBit() == 0)
            {
                cnt++;
            }

            if (cnt > 0)
            {
                value = (uint)((1 << cnt) - 1 + ReadBits(cnt));
            }
            else
            {
                value = 0;
            }

            return (ulong)(cnt + 1 + cnt);
        }

        internal ulong WriteUnsignedIntGolomb(uint value)
        {
            int bits = 0;
            int cumul = 0;
            for (int i = 0; i < 31; i++)
            {
                if (value < cumul + (1 << i))
                {
                    bits = i;
                    break;
                }
                cumul += (1 << i);
            }
            WriteBits(bits, 0);
            WriteBit(1);
            WriteBits(bits, (int)(value - cumul));
            return (ulong)(bits + 1 + bits);
        }

        internal ulong ReadSignedIntGolomb(ulong size, out int value)
        {
            uint val;
            ulong read = ReadUnsignedIntGolomb(size, out val);
            int sign = (((int)val & 0x1) << 1) - 1;
            value = (((int)val >> 1) + ((int)val & 0x1)) * sign;
            return read;
        }

        internal ulong WriteSignedIntGolomb(int value)
        {
            uint mapped = (uint)((value << 1) * (value < 0 ? -1 : 1) - (value > 0 ? 1 : 0));
            return WriteUnsignedIntGolomb(mapped);
        }

        internal bool ReadMoreRbspData(IItuSerializable serializable)
        {
            if (_stream.Position == _stream.Length && _bitsPosition % 8 == 0)
                return false;

            // now we have to look ahead - TODO
            var bytes = (_stream as MemoryStream).ToArray().Skip(_bitsPosition / 8).ToArray();
            using (var ituStream = new ItuStream(new MemoryStream(bytes)))
            {
                ituStream.ReadBits(_bitsPosition % 8);
                
                int one = ituStream.ReadBit();
                if (one == 0)
                {
                    serializable.HasMoreRbspData++;
                    return true;
                }

                int lastBit = ituStream.ReadBit();
                while (lastBit == 0)
                {
                    lastBit = ituStream.ReadBit();
                }

                // -1 means that up until the end there are zeros
                if (lastBit < 0)
                {
                    return false;
                }
                else
                {
                    serializable.HasMoreRbspData++;
                    return true;
                }
            }
        }

        internal bool WriteMoreRbspData(IItuSerializable serializable)
        {
            if (serializable.HasMoreRbspData == 0)
                return false;
            else if(_rbspDataCounter == 0)
                _rbspDataCounter = serializable.HasMoreRbspData;
            return _rbspDataCounter-- != 0;
        }

        internal int ReadNextBits(IItuSerializable serializable, int count)
        {
            var bytes = (_stream as MemoryStream).ToArray().Skip(_bitsPosition / 8).ToArray();
            using (var ituStream = new ItuStream(new MemoryStream(bytes)))
            {
                ituStream.ReadBits(_bitsPosition % 8);
                return (int)ituStream.ReadBits(8);
            }
        }

        internal int WriteNextBits(IItuSerializable serializable, int count)
        {
            // TODO: same trick as in WriteMoreRbspData
            return 0xFF; // 0xFF;
        }

        internal ulong ReadBits(ulong size, int count, out byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadUnsignedInt(ulong size, int count, out BigInteger value)
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

        internal ulong ReadSignedIntT(ulong size, out int value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteBits(int count, byte value)
        {
            WriteBits(count, (long)value);
            return (ulong)count;
        }
        
        internal ulong WriteUnsignedInt(int count, BigInteger value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteSignedIntT(int value)
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

        #region IDisposable 

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if(_stream != null)
                    {
                        _stream.Dispose();
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable
    }
}
