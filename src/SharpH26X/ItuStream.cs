﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SharpH26X
{
    public class ItuStream : IDisposable
    {
        private bool _shouldEscapeNals = true;
        private int _bitsPosition;
        private int _currentBytePosition = -1;
        private byte _currentByte = 0;
        private int _prevByte = -1;
        private int _prevPrevByte = -1;

        private readonly Stream _stream;

        private int _rbspDataCounter = -1;
        private int _readNextBitsCounter = -1;
        private int _readNextBitsIndex = 0;

        private bool _disposedValue;

        public bool PayloadExtensionPresent()
        {
            throw new NotImplementedException();
        }

        public bool MoreDataInPayload()
        {
            throw new NotImplementedException();
        }

        public ItuStream(Stream stream)
        {
            this._stream = stream;
        }

        public ItuStream(Stream stream, int bitsPosition, int currentBytePosition, byte currentByte, int prevByte, int prevPrevByte) : this(stream)
        {
            this._bitsPosition = bitsPosition;
            this._currentBytePosition = currentBytePosition;
            this._currentByte = currentByte;
            this._prevByte = prevByte;
            this._prevPrevByte = prevPrevByte;
        }

        #region Bit read/write

        private int ReadByte()
        {
            int ret = _stream.ReadByte();
            return ret;
        }

        private ulong WriteByte(byte value)
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

        private void WriteBits(int count, ulong value)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count > 0)
            {
                int bits = count - 1;
                ulong mask = 0x1u << bits;
                WriteBit((int)((value & mask) >> bits));
                count--;
            }
        }

        #endregion // Bit read/write

        public bool ByteAligned()
        {
            return _bitsPosition % 8 == 0;
        }

        public ulong ReadClass<T>(ulong size, IItuContext context, T value) where T : IItuSerializable
        {
            return value.Read(context, this);
        }

        public ulong WriteClass<T>(IItuContext context, T value) where T : IItuSerializable
        {
            ulong size = value.Write(context, this);
            return size;
        }

        public ulong WriteClass<T>(IItuContext context, T[] value) where T : IItuSerializable
        {
            ulong size = value.Single().Write(context, this);
            return size;
        }

        public ulong ReadUnsignedInt(ulong size, int count, out byte value)
        {
            if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(size, count, out uint v);
            value = (byte)v;
            return read;
        }

        public ulong WriteUnsignedInt(int count, byte value)
        {
            if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong size = WriteUnsignedInt(count, (uint)value);
            return size;
        }

        public ulong ReadFixed(ulong size, int count, out uint value)
        {
            return ReadUnsignedInt(size, count, out value);
        }

        public ulong WriteFixed(int count, uint value)
        {
            return WriteUnsignedInt(count, value);
        }

        public ulong ReadUnsignedInt(ulong size, int count, out uint value)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(size, count, out ulong v);
            value = (uint)v;
            return read;
        }

        public ulong ReadUnsignedInt(ulong size, int count, out ulong value)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));
            long ret = ReadBits(count);
            if (ret == -1)
                throw new EndOfStreamException();
            value = (ulong)ret;
            return (ulong)count;
        }

        public ulong WriteUnsignedInt(int count, ulong value)
        {
            WriteBits(count, value);
            return (ulong)count;
        }

        public ulong ReadSignedInt(ulong size, int count, out int value)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            long ret = ReadBits(count);
            if (ret == -1)
                throw new EndOfStreamException();
            value = unchecked((int)ret);
            return (ulong)count;
        }

        public ulong WriteSignedInt(int count, int value)
        {
            WriteBits(count, unchecked((ulong)value));
            return (ulong)count;
        }

        public ulong ReadUnsignedIntGolomb(ulong size, out uint value)
        {
            int cnt = 0;
            int bit = -1;
            while ((bit = ReadBit()) == 0)
            {
                cnt++;
            }

            if(bit == -1)
                throw new EndOfStreamException();

            if (cnt > 0)
            {
                long bits = ReadBits(cnt);
                if (bits == -1)
                    throw new EndOfStreamException();
                value = (uint)((1 << cnt) - 1 + bits);
            }
            else
            {
                value = 0;
            }

            return (ulong)(cnt + 1 + cnt);
        }

        public ulong WriteUnsignedIntGolomb(uint value)
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
            WriteBits(bits, (ulong)(value - cumul));
            return (ulong)(bits + 1 + bits);
        }

        public ulong ReadSignedIntGolomb(ulong size, out int value)
        {
            uint val;
            ulong read = ReadUnsignedIntGolomb(size, out val);
            int sign = (((int)val & 0x1) << 1) - 1;
            value = (((int)val >> 1) + ((int)val & 0x1)) * sign;
            return read;
        }

        public ulong WriteSignedIntGolomb(int value)
        {
            uint mapped = (uint)((value << 1) * (value < 0 ? -1 : 1) - (value > 0 ? 1 : 0));
            return WriteUnsignedIntGolomb(mapped);
        }

        public bool ReadMoreRbspData(IItuSerializable serializable)
        {
            if (_stream.Position == _stream.Length && _bitsPosition % 8 == 0)
                return false;

            // now we have to look ahead - TODO
            var bytes = (_stream as MemoryStream).ToArray();
            var msstream = new MemoryStream(bytes);
            msstream.Seek(_stream.Position, SeekOrigin.Begin);
            using (var ituStream = new ItuStream(msstream, _bitsPosition, _currentBytePosition, _currentByte, _prevByte, _prevPrevByte))
            {               
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
                if (lastBit == -1)
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

        public bool WriteMoreRbspData(IItuSerializable serializable)
        {
            if (serializable.HasMoreRbspData == 0 || _rbspDataCounter == 0)
                return false;
            else if(_rbspDataCounter == -1)
                _rbspDataCounter = serializable.HasMoreRbspData;
            return _rbspDataCounter-- != 0;
        }

        public int ReadNextBits(IItuSerializable serializable, int count)
        {
            var bytes = (_stream as MemoryStream).ToArray();
            var msstream = new MemoryStream(bytes);
            msstream.Seek(_stream.Position, SeekOrigin.Begin);
            using (var ituStream = new ItuStream(msstream, _bitsPosition, _currentBytePosition, _currentByte, _prevByte, _prevPrevByte))
            {
                if (serializable.ReadNextBits == null)
                {
                    serializable.ReadNextBits = new int[1];
                }

                int ret = (int)ituStream.ReadBits(8);

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

        public int WriteNextBits(IItuSerializable serializable, int count)
        {
            if (serializable.ReadNextBits == null)
                return 0;

            if (_readNextBitsCounter == -1 && _readNextBitsIndex == 0)
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

                if(_readNextBitsIndex >= serializable.ReadNextBits.Length)
                {
                    _readNextBitsIndex = 0;
                }

                _readNextBitsCounter = serializable.ReadNextBits[_readNextBitsIndex];
                return 0;
            }
        }

        public ulong ReadUnsignedIntVariable(ulong size, uint count, out uint value)
        {
            return ReadUnsignedInt(size, (int)count, out value);
        }

        public ulong WriteUnsignedIntVariable(uint count, uint value)
        {
            return WriteUnsignedInt((int)count, value);
        }

        public ulong ReadSignedIntVariable(ulong size, uint count, out int value)
        {
            return ReadSignedInt(size, (int)count, out value);
        }

        public ulong WriteSignedIntVariable(uint count, int value)
        {
            return WriteSignedInt((int)count, value);
        }

        public ulong ReadBits(ulong size, int count, out byte value)
        {
            long bits = ReadBits(count);
            if(bits == -1)
                throw new EndOfStreamException();
            value = (byte)bits;
            return (ulong)count;
        }

        public ulong WriteBits(int count, byte value)
        {
            WriteBits(count, (ulong)value);
            return (ulong)count;
        }

        public ulong ReadUnsignedInt(ulong size, int count, out BigInteger value)
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

        public ulong WriteUnsignedInt(int count, BigInteger value)
        {
            ulong size = 0;
            byte[] bytes = value.ToByteArray();
            for (int i = 0; i < bytes.Length; i++)
            {
                size += WriteByte(bytes[i]);
            }
            return size;
        }

        public ulong ReadUtf8String(ulong size, out byte[] value)
        {
            List<byte> bytes = new List<byte>();
            int b = -1;
            while((b = ReadByte()) != -1)
            {
                if (b == 0)
                    break;
                bytes.Add((byte)b);
            }
            value = bytes.ToArray();
            return (ulong)((bytes.Count + 1) * 8);
        }

        public ulong WriteUtf8String(byte[] value)
        {
            ulong size = 0;
            for (int i = 0; i < value.Length; i++)
            {
                size += WriteByte(value[i]);
            }
            size += WriteByte(0); // null terminator
            return size;
        }

        #region Lists in do/while loops

        public ulong ReadFixed(ulong size, int count, int whileIndex, Dictionary<int, uint> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong read = ReadFixed(size, count, out var value);
            list.Add(whileIndex, value);
            return read;
        }

        public ulong ReadUnsignedIntGolomb(ulong size, int whileIndex, Dictionary<int, uint> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong read = ReadUnsignedIntGolomb(size, out var value);
            list.Add(whileIndex, value);
            return read;
        }

        public ulong ReadUnsignedInt(ulong size, int count, int whileIndex, Dictionary<int, byte> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong read = ReadUnsignedInt(size, count, out byte value);
            list.Add(whileIndex, value);
            return read;
        }

        public ulong ReadBits(ulong size, int count, int whileIndex, Dictionary<int, byte> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong read = ReadBits(size, count, out byte value);
            list.Add(whileIndex, value);
            return read;
        }

        public ulong WriteFixed(int count, int whileIndex, Dictionary<int, uint> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!list.ContainsKey(whileIndex))
                throw new ArgumentOutOfRangeException(nameof(whileIndex));

            ulong size = WriteFixed(count, list[whileIndex]);
            return size;
        }

        public ulong WriteUnsignedInt(int count, int whileIndex, Dictionary<int, byte> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!list.ContainsKey(whileIndex))
                throw new ArgumentOutOfRangeException(nameof(whileIndex));

            ulong size = WriteUnsignedInt(count, list[whileIndex]);
            return size;
        }

        public ulong WriteUnsignedIntGolomb(int whileIndex, Dictionary<int, uint> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!list.ContainsKey(whileIndex))
                throw new ArgumentOutOfRangeException(nameof(whileIndex));

            ulong size = WriteUnsignedIntGolomb(list[whileIndex]);
            return size;
        }

        public ulong WriteBits(int count, int whileIndex, Dictionary<int, byte> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!list.ContainsKey(whileIndex))
                throw new ArgumentOutOfRangeException(nameof(whileIndex));

            ulong size = WriteBits(count, list[whileIndex]);
            return size;
        }

        public ulong WriteClass<T>(IItuContext context, int whileIndex, Dictionary<int, T> list) where T : IItuSerializable
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!list.ContainsKey(whileIndex))
                throw new ArgumentOutOfRangeException(nameof(whileIndex));

            ulong size = WriteClass(context, list[whileIndex]);
            return size;
        }

        #endregion // Lists in do/while loops

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

        internal bool MoreRbspTrailingData()
        {
            throw new NotImplementedException();
        }

        #endregion // IDisposable
    }
}
