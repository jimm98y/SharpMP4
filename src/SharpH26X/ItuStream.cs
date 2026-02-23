using SharpMP4.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SharpH26X
{
    public class ItuStream : IDisposable
    {
        private readonly bool _shouldEscapeNals = true;
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
        private int _lastMarkBeginPosition;

        public IMp4Logger Logger { get; set; }

        public ItuStream(Stream stream, IMp4Logger logger)
        {
            this._stream = stream;
            this.Logger = logger;
        }

        public ItuStream(Stream stream)
            : this(stream, new DefaultMp4Logger())
        {
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
                    _lastMarkBeginPosition += 8; // compensate for emulation prevention bytes
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
                        _lastMarkBeginPosition += 8; // compensate for emulation prevention
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
                ulong mask = 0x1ul << bits;
                WriteBit((int)((value & mask) >> bits));
                count--;
            }
        }

        #endregion // Bit read/write

        public void MarkCurrentBitsPosition()
        {
            _lastMarkBeginPosition = _bitsPosition;
        }

        public ulong GetBitsPositionSinceLastMark()
        {
            return (ulong)(_bitsPosition - _lastMarkBeginPosition);
        }

        public bool ByteAligned()
        {
            return _bitsPosition % 8 == 0;
        }

        // H265
        public ulong ReadClass<T>(ulong size, IItuContext context, T value, string name) where T : IItuSerializable
        {
            LogBegin(name);

            _logLevel++;
            ulong ret = value.Read(context, this);
            _logLevel--;

            LogEnd(name, ret, value);

            return ret;
        }

        public ulong WriteClass<T>(IItuContext context, T value, string name) where T : IItuSerializable
        {
            LogBegin(name);

            _logLevel++;
            ulong size = value.Write(context, this);
            _logLevel--;

            LogEnd(name, size, value);

            return size;
        }

        public ulong WriteClass<T>(IItuContext context, T[] value, string name) where T : IItuSerializable
        {
            return WriteClass(context, value.Single(), name);
        }

        public ulong ReadUnsignedInt(ulong size, ulong count, out byte value, string name)
        {
            if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(size, count, out uint v, name);
            value = (byte)v;
            return read;
        }
        
        public ulong ReadUnsignedInt(ulong size, ulong count, int index, Dictionary<int, uint> value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(size, count, out uint v, name);
            value.Add(index, v);
            return read;
        }

        public ulong WriteUnsignedInt(ulong count, byte value, string name)
        {
            if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong size = WriteUnsignedInt(count, (uint)value, name);
            return size;
        }

        public ulong WriteUnsignedInt(ulong count, int index, Dictionary<int, uint> value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong size = WriteUnsignedInt(count, value[index], name);
            return size;
        }

        public ulong ReadFixed(ulong size, ulong count, out uint value, string name)
        {
            return ReadUnsignedInt(size, count, out value, name);
        }

        public ulong WriteFixed(ulong count, uint value, string name)
        {
            return WriteUnsignedInt(count, value, name);
        }

        public ulong ReadUnsignedInt(ulong size, ulong count, out uint value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(size, count, out ulong v, name);
            value = (uint)v;
            return read;
        }

        public ulong ReadUnsignedInt(ulong size, ulong count, out ulong value, string name)
        {
            //LogBegin(name);
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));
            long ret = ReadBits((int)count);
            if (ret == -1)
                throw new EndOfStreamException();
            value = (ulong)ret;
            LogEnd(name, (ulong)count, value);
            return (ulong)count;    
        }

        public ulong WriteUnsignedInt(ulong count, ulong value, string name)
        {
            //LogBegin(name);
            WriteBits((int)count, value);
            LogEnd(name, (ulong)count, value);
            return (ulong)count;
        }

        public ulong ReadSignedInt(ulong size, ulong count, out int value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            long ret = ReadBits((int)count);
            if (ret == -1)
                throw new EndOfStreamException();
            value = unchecked((int)ret);
            return (ulong)count;
        }

        public ulong WriteSignedInt(ulong count, int value, string name)
        {
            //LogBegin(name);
            WriteBits((int)count, unchecked((ulong)value));
            LogEnd(name, (ulong)count, value);
            return (ulong)count;
        }

        public ulong ReadUnsignedIntGolomb(ulong size, out ulong value, string name)
        {
            //LogBegin(name);
            int cnt = 0;
            int bit = -1;
            while ((bit = ReadBit()) == 0)
            {
                cnt++;
            }

            if (bit == -1)
                throw new EndOfStreamException();

            if (cnt > 0)
            {
                long bits = ReadBits(cnt);
                if (bits == -1)
                    throw new EndOfStreamException();
                value = (1ul << cnt) - 1ul + (ulong)bits;
            }
            else
            {
                value = 0;
            }

            LogEnd(name, (ulong)(cnt + 1 + cnt), (long)value);
            return (ulong)(cnt + 1 + cnt);
        }

        public ulong WriteUnsignedIntGolomb(ulong value, string name)
        {
            //LogBegin(name);
            int cnt = 0;
            for (int i = 0; i < 64; i++)
            {
                if (((value + 1ul) >> i) > 0)
                {
                    cnt = i;
                }
            }
            WriteBits(cnt, 0);
            WriteBit(1);
            WriteBits(cnt, value - (1ul << cnt) + 1);

            LogEnd(name, (ulong)(cnt + 1 + cnt), (long)value);
            return (ulong)(cnt + 1 + cnt);
        }

        public ulong ReadSignedIntGolomb(ulong size, out long value, string name)
        {
            ulong val;
            ulong read = ReadUnsignedIntGolomb(size, out val, "");
            //value = (val % 2 == 0 ? -1L : 1L) * (long)((val + 1) / 2);
            long sign = (((long)val & 0x1) << 1) - 1;
            value = (((long)val >> 1) + ((long)val & 0x1)) * sign;
            LogEnd(name, read, value);
            return read;
        }

        public ulong WriteSignedIntGolomb(long value, string name)
        {
            //ulong mapped = (ulong)(value <= 0 ? -2 * value : 2 * value - 1);
            ulong mapped = (ulong)((value << 1) * (value < 0 ? -1 : 1) - (value > 0 ? 1 : 0));
            var size = WriteUnsignedIntGolomb(mapped, "");
            LogEnd(name, size, value);
            return size;
        }

        public bool ReadMoreRbspData(IItuSerializable serializable, ulong maxPayloadSize = ulong.MaxValue)
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
                    
                    if (maxPayloadSize == ulong.MaxValue || GetBitsPositionSinceLastMark() < (maxPayloadSize * 8))
                    {
                        serializable.HasMoreRbspData++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
                    
                    if (maxPayloadSize == ulong.MaxValue || GetBitsPositionSinceLastMark() < (maxPayloadSize * 8))
                    {
                        serializable.HasMoreRbspData++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool WriteMoreRbspData(IItuSerializable serializable, ulong maxPayloadSize = ulong.MaxValue) // TODO
        {
            if (serializable.HasMoreRbspData == 0 || _rbspDataCounter == 0)
                return false;
            else if(_rbspDataCounter == -1)
                _rbspDataCounter = serializable.HasMoreRbspData;
            return _rbspDataCounter-- != 0;
        }

        public int ReadNextBits(IItuSerializable serializable, ulong count)
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

        public int WriteNextBits(IItuSerializable serializable, ulong count)
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

        public ulong ReadUnsignedIntVariable(ulong size, ulong count, out uint value, string name)
        {
            return ReadUnsignedInt(size, count, out value, name);
        }

        public ulong WriteUnsignedIntVariable(ulong count, uint value, string name)
        {
            return WriteUnsignedInt(count, value, name);
        }

        public ulong ReadUnsignedIntVariable(ulong size, ulong count, out ulong value, string name)
        {
            return ReadUnsignedInt(size, count, out value, name);
        }

        public ulong WriteUnsignedIntVariable(ulong count, ulong value, string name)
        {
            return WriteUnsignedInt(count, value, name);
        }

        public ulong ReadSignedIntVariable(ulong size, ulong count, out int value, string name)
        {
            return ReadSignedInt(size, count, out value, name);
        }

        public ulong WriteSignedIntVariable(ulong count, int value, string name)
        {
            return WriteSignedInt(count, value, name);
        }

        public ulong ReadBits(ulong size, ulong count, out byte value, string name)
        {
            //LogBegin(name);
            long bits = ReadBits((int)count);
            if(bits == -1)
                throw new EndOfStreamException();
            value = (byte)bits;
            LogEnd(name, (ulong)count, (long)value);
            return (ulong)count;
        }

        public ulong WriteBits(ulong count, byte value, string name)
        {
            //LogBegin(name);
            WriteBits((int)count, value);
            LogEnd(name, count, (long)value);
            return count;
        }

        public ulong ReadUnsignedInt(ulong size, ulong count, out BigInteger value, string name)
        {
            //LogBegin(name);
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
            LogEnd(name, count, value);
            return count;
        }

        public ulong WriteUnsignedInt(ulong count, BigInteger value, string name)
        {
            ulong size = 0;
            byte[] bytes = value.ToByteArray();
            for (int i = 0; i < bytes.Length; i++)
            {
                size += WriteUnsignedInt(8, bytes[i], name);
            }
            return size;
        }

        public ulong ReadUtf8String(ulong size, out byte[] value, string name)
        {
            //LogBegin(name);
            List<byte> bytes = new List<byte>();
            int b = -1;
            while((b = ReadByte()) != -1)
            {
                if (b == 0)
                    break;
                bytes.Add((byte)b);
            }
            value = bytes.ToArray();
            LogEnd(name, (ulong)((bytes.Count + 1) * 8), Encoding.UTF8.GetString(value));
            return (ulong)((bytes.Count + 1) * 8);
        }

        public ulong WriteUtf8String(byte[] value, string name)
        {
            //LogBegin(name);
            ulong size = 0;
            for (int i = 0; i < value.Length; i++)
            {
                size += WriteBits(8, value[i], name);
            }
            size += WriteBits(8, 0, name); // null terminator
            LogEnd(name, size, Encoding.UTF8.GetString(value));
            return size;
        }

        #region Lists in do/while loops

        public ulong ReadFixed(ulong size, ulong count, int whileIndex, Dictionary<int, uint> list, string name)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong read = ReadFixed(size, count, out var value, name);
            list.Add(whileIndex, value);
            return read;
        }

        public ulong ReadUnsignedIntGolomb(ulong size, int whileIndex, Dictionary<int, ulong> list, string name)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong read = ReadUnsignedIntGolomb(size, out ulong value, name);
            list.Add(whileIndex, value);
            return read;
        }

        public ulong ReadUnsignedInt(ulong size, ulong count, int whileIndex, Dictionary<int, byte> list, string name)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong read = ReadUnsignedInt(size, count, out byte value, name);
            list.Add(whileIndex, value);
            return read;
        }

        public ulong ReadBits(ulong size, ulong count, int whileIndex, Dictionary<int, byte> list, string name)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong read = ReadBits(size, count, out byte value, name);
            list.Add(whileIndex, value);
            return read;
        }

        public ulong WriteFixed(ulong count, int whileIndex, Dictionary<int, uint> list, string name)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            uint value = 0;
            if (list.ContainsKey(whileIndex))
                value = list[whileIndex];

            ulong size = WriteFixed(count, value, name);
            return size;
        }

        public ulong WriteUnsignedInt(ulong count, int whileIndex, Dictionary<int, byte> list, string name)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            byte value = 0;
            if (list.ContainsKey(whileIndex))
                value = list[whileIndex];

            ulong size = WriteUnsignedInt(count, value, name);
            return size;
        }

        public ulong WriteUnsignedIntGolomb(int whileIndex, Dictionary<int, ulong> list, string name)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            ulong value = 0;
            if (list.ContainsKey(whileIndex))
                value = list[whileIndex];

            ulong size = WriteUnsignedIntGolomb(value, name);
            return size;
        }

        public ulong WriteBits(ulong count, int whileIndex, Dictionary<int, byte> list, string name)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            byte value = 0;
            if (list.ContainsKey(whileIndex))
                value = list[whileIndex];

            ulong size = WriteBits(count, value, name);
            return size;
        }

        public ulong WriteClass<T>(IItuContext context, int whileIndex, Dictionary<int, T> list, string name) where T : IItuSerializable
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!list.ContainsKey(whileIndex))
                throw new ArgumentOutOfRangeException(nameof(whileIndex));

            ulong size = WriteClass(context, list[whileIndex], name);
            return size;
        }

        #endregion // Lists in do/while loops

        #region Logging

        private int _logLevel = 0;

        private void LogBegin(string name)
        {
            var padding = new StringBuilder();
            for (int i = 0; i < _logLevel; i++)
            {
                padding.Append('-');
            }

            this.Logger.LogInfo($"{padding} {name}");
        }

        private void LogEnd<T>(string name, ulong size, T value)
        {
            if (string.IsNullOrEmpty(name))
                return;

            var padding = new StringBuilder();
            for (int i = 0; i < _logLevel; i++)
            {
                padding.Append('-');
            }

            var endPadding = new StringBuilder();
            for (int i = 0; i < 64 - padding.Length - name.Length - size.ToString().Length - 2; i++)
            {
                endPadding.Append(' ');
            }

            this.Logger.LogInfo($"{padding} {name}{endPadding}{size}   {value}");
        }

        #endregion // Logging

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
