using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SharpH264
{
    public interface IItuSerializable
    {
        ulong Read(H264Context context, ItuStream stream);
        ulong Write(H264Context context, ItuStream stream);
        int HasMoreRbspData { get; set; }
        int[] ReadNextBits { get; set; }
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
        private int _readNextBitsCounter = 0;
        private int _readNextBitsIndex = 0;

        private bool _disposedValue;

        public ItuStream(Stream stream)
        {
            this._stream = stream;
        }

        public bool MoreRbspTrailingData()
        {
            throw new NotImplementedException();
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
                int bb = ReadByte();
                if (bb == -1)
                {
                    return -1;
                }

                byte b = (byte)bb;

                // remove emulation prevention byte
                if (_shouldEscapeNals && _prevByte == 0 && _currentByte == 0 && b == 0x03)
                {
                    _prevByte = b;
                    bb = ReadByte();
                    if(bb == -1)
                    {
                        return -1;
                    }
                    b = (byte)bb;
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
                    return -1;

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

        internal ulong ReadClass<T>(ulong size, H264Context context, Func<T> factory, out T value) where T : IItuSerializable
        {
            T c = factory();
            ulong read = c.Read(context, this);
            value = c;
            return read;
        }

        internal ulong ReadClass<T>(ulong size, H264Context context, Func<T> factory, out T[] value) where T : IItuSerializable
        {
            T c;
            ulong read = ReadClass(size, context, factory, out c);
            value = new T[] { c };
            return read;
        }

        internal ulong WriteClass<T>(H264Context context, T value) where T : IItuSerializable
        {
            ulong size = value.Write(context, this);
            return size;
        }

        internal ulong WriteClass<T>(H264Context context, T[] value) where T : IItuSerializable
        {
            ulong size = value.Single().Write(context, this);
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
                if (one == -1)
                    return false;

                if (one == 0)
                {
                    serializable.HasMoreRbspData++;
                    return true;
                }

                int lastBit = ituStream.ReadBit();
                if (lastBit == -1)
                    return false;

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
                int ret = (int)ituStream.ReadBits(8);

                if (serializable.ReadNextBits == null)
                {
                    serializable.ReadNextBits = new int[1];
                }

                if (ret == 0xFF)
                {
                    serializable.ReadNextBits[serializable.ReadNextBits.Length - 1]++;
                }
                else
                {
                    var old = serializable.ReadNextBits;
                    serializable.ReadNextBits = new int[old.Length + 1];
                    Array.Copy(old, serializable.ReadNextBits, old.Length);
                }
                return ret;
            }
        }

        internal int WriteNextBits(IItuSerializable serializable, int count)
        {
            if (serializable.ReadNextBits == null)
                return 0;

            if (_readNextBitsCounter == 0 && _readNextBitsIndex == 0)
            {
                _readNextBitsCounter = serializable.ReadNextBits[_readNextBitsIndex];
            }

            if (_readNextBitsCounter != 0)
            {
                _readNextBitsCounter--;
                return 0xFF;
            }
            else
            {
                _readNextBitsIndex++;
                _readNextBitsCounter = serializable.ReadNextBits[_readNextBitsIndex];
                return 0;
            }
        }

        internal ulong ReadUnsignedIntVariable(ulong size, uint count, out uint value)
        {
            return ReadUnsignedInt(size, (int)count, out value);
        }

        internal ulong WriteUnsignedIntVariable(uint count, uint value)
        {
            return WriteUnsignedInt((int)count, value);
        }

        internal ulong ReadSignedIntVariable(ulong size, uint count, out int value)
        {
            return ReadSignedInt(size, (int)count, out value);
        }

        internal ulong WriteSignedIntVariable(uint count, int value)
        {
            return WriteSignedInt((int)count, value);
        }

        internal ulong ReadBits(ulong size, int count, out byte value)
        {
            value = (byte)ReadBits(count);
            return (ulong)count;
        }

        internal ulong WriteBits(int count, byte value)
        {
            WriteBits(count, (long)value);
            return (ulong)count;
        }

        internal ulong ReadUnsignedInt(ulong size, int count, out BigInteger value)
        {
            if (count % 8 > 0)
                throw new NotSupportedException();

            byte[] bytes = new byte[count / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                long bb = ReadBits(8);
                if (bb == -1)
                    throw new EndOfStreamException();

                bytes[i] = (byte)bb;
            }

            value = new BigInteger(bytes);
            return (ulong)count;
        }

        internal ulong WriteUnsignedInt(int count, BigInteger value)
        {
            long size = 0;
            byte[] bytes = value.ToByteArray();
            for (int i = 0; i < bytes.Length; i++)
            {
                size += WriteByte(bytes[i]);
            }
            return (ulong)size;
        }

        internal ulong ReadSignedIntT(ulong size, out int value)
        {
            throw new NotImplementedException();
        }
        
        internal ulong WriteSignedIntT(int value)
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
