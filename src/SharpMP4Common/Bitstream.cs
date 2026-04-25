using System;
using System.IO;

namespace SharpMP4.Common
{
    public class Bitstream
    {
        protected long _bitsPosition;
        protected long _currentBytePosition = -1;
        protected byte _currentByte;

        protected Stream _stream;

        public Bitstream(Stream stream)
        {
            _stream = stream;
        }

        public long BitsPosition
        {
            get => _bitsPosition;
            set => _bitsPosition = value;
        }

        public byte CurrentByte
        {
            get => _currentByte;
            set => _currentByte = value;
        }

        public Stream BaseStream
        {
            get => _stream;
        }

        private int ReadBitInternal()
        {
            long bytePos = _bitsPosition >> 3;

            if (_currentBytePosition != bytePos)
            {
                byte b = ReadByte();
                _currentByte = b;
                _currentBytePosition = bytePos;
            }

            long posInByte = 7 - _bitsPosition % 8;
            int bit = _currentByte >> (int)posInByte & 1;
            ++_bitsPosition;
            return bit;
        }

        private void WriteBitInternal(int value)
        {
            long posInByte = 7 - _bitsPosition % 8;
            int bit = (value & 1) << (int)posInByte;
            _currentByte = (byte)(_currentByte | bit);
            ++_bitsPosition;

            long bytePos = _bitsPosition >> 3;
            if (_currentBytePosition != bytePos)
            {
                if (_currentBytePosition != -1) // special case for the first bit
                {
                    WriteByte(_currentByte);
                    _currentByte = 0;
                }
                _currentBytePosition = bytePos;
            }
        }

        public virtual int ReadBit() => ReadBitInternal();
        public virtual void WriteBit(int bit) => WriteBitInternal(bit);

        private int ReadByteInternal()
        {
            return _stream.ReadByte();
        }

        public virtual byte ReadByte()
        {
            int read = ReadByteInternal();
            if (read == -1) throw new EndOfStreamException();
            return (byte)(read & 0xff);
        }

        public virtual ulong WriteByte(byte value)
        {
            _stream.WriteByte(value);
            return 8;
        }

        public virtual ulong ReadBits(uint count, out uint value)
        {
            if (count > 32)
                throw new ArgumentException("'count' cannot be greater than 32", nameof(count));

            uint originalCount = count;
            int res = 0;

            while (count > 0)
            {
                res <<= 1;
                int u1 = ReadBitInternal();

                if (u1 == -1)
                {
                    throw new EndOfStreamException();
                }

                res |= u1;
                count--;
            }

            value = (uint)res;
            return originalCount;
        }

        public virtual uint ReadBits(uint count)
        {
            ReadBits(count, out uint value);
            return value;
        }

        public virtual ulong ReadBits(uint count, out ulong value)
        {
            if (count > 64)
                throw new ArgumentException("'count' cannot be greater than 64", nameof(count));

            uint originalCount = count;
            long res = 0;
            while (count > 0)
            {
                res <<= 1;
                long u1 = ReadBitInternal();

                if (u1 == -1)
                {
                    throw new EndOfStreamException();
                }

                res |= u1;
                count--;
            }

            value = (ulong)res;
            return originalCount;
        }

        public virtual ulong ReadBits(ulong count)
        {
            ReadBits((uint)count, out ulong value);
            return value;
        }

        public virtual ulong WriteBits(uint count, ulong value)
        {
            if (count > 64)
                throw new ArgumentException("'count' cannot be greater than 64", nameof(count));

            uint originalCount = count;
            while (count > 0)
            {
                int bits = (int)count - 1;
                ulong mask = 0x1u << bits;
                WriteBitInternal((int)((value & mask) >> bits));
                count--;
            }

            return originalCount;
        }

        public virtual uint WriteBits(uint count, uint value)
        {
            if (count > 32)
                throw new ArgumentException("'count' cannot be greater than 32", nameof(count));

            uint originalCount = count;
            while (count > 0)
            {
                int bits = (int)count - 1;
                ulong mask = 0x1u << bits;
                WriteBitInternal((int)((value & mask) >> bits));
                count--;
            }

            return originalCount;
        }
    }
}
