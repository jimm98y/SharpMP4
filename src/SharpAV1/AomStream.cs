using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpAV1
{
    public class AomStream : IDisposable
    {
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

        public AomStream(Stream stream)
        {
            this._stream = stream ?? new MemoryStream();
        }

        public int GetPosition()
        {
            return _bitsPosition;
        }

        public void Skip(long bits)
        {
            while (_bitsPosition % 8 != 0)
            {
                ReadBit();
                bits--;
            }

            _bitsPosition += (int)bits;

            long bytes = (bits >> 3);
            _stream.Seek(bytes, SeekOrigin.Current);
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
                _prevByte = _currentByte;

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
                if (_currentBytePosition < 0)
                {
                    _currentBytePosition = bytePos;
                    return;
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

        public ulong ReadLeb128(out int v, string name)
        {
            int value = 0;
            int Leb128Bytes = 0;
            for (int i = 0; i < 8; i++)
            {
                int leb128_byte = ReadByte();
                value |= ((leb128_byte & 0x7f) << (i * 7));
                Leb128Bytes += 1;
                if((leb128_byte & 0x80) == 0)
                {
                    break;
                }
            }

            if (value <= int.MaxValue)
            {
                v = value;
            }
            else
            {
                throw new InvalidDataException($"Invalid LEB128 value: {value}");
            }

            LogEnd(name, (ulong)Leb128Bytes << 3, value);

            return (ulong)Leb128Bytes << 3;
        }

        public ulong WriteLeb128(int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadFixed(int count, out int value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(count, out uint v, name);
            value = (int)v;
            return read;
        }

        public ulong WriteFixed(int count, int value, string name)
        {
            return WriteUnsignedInt(count, (uint)value, name);
        }

        public ulong ReadVariable(long length, out int value, string name)
        {
            var size = ReadUnsignedInt((int)length, out uint v, name);
            value = (int)v;
            return size;
        }

        public ulong WriteVariable(long length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadUvlc(out uint value, string name)
        {
            ulong size = 0;
            int leadingZeros = 0;
            while (true) 
            {
                int done = ReadBit();
                if(done == -1)
                    throw new EndOfStreamException();

                size++;

                if (done != 0)
                    break;

                leadingZeros++;
             }
            if (leadingZeros >= 32)
            {
                value = (1 << 32) - 1;
                LogEnd(name, size, value);
                return size;
            }
            else
            {
                long v = ReadBits(leadingZeros);
                size += (ulong)leadingZeros;
                value = (uint)(v + (1 << leadingZeros) - 1);
                LogEnd(name, size, value);
                return size;
            }
        }

        public ulong WriteUvlc(uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadSignedIntVar(int n, out int value, string name)
        {
            ulong size = ReadUnsignedInt(n, out uint v, name);
            long signMask = 1 << (n - 1);
            if ((v & signMask) > 0)
                value = (int)(v - 2 * signMask);
            else
                value = (int)v;
            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteSignedIntVar(int length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadUnsignedInt(int count, out uint value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            long ret = ReadBits(count);
            if (ret == -1)
                throw new EndOfStreamException();
            value = (uint)ret;
            LogEnd(name, (ulong)count, value);
            return (ulong)count;
        }

        public ulong WriteUnsignedInt(int length, uint value, string name)
        {
            WriteBits(length, value);
            LogEnd(name, (ulong)length, value);
            return (ulong)length;
        }

        public ulong ReadLeVar(int n, out int value, string name)
        {
            uint size = 0;
            int t = 0;
            for (int i = 0; i < n; i++)
            {
                int b = ReadByte();
                size++;
                t += (b << (i * 8));
            }
            value = t;
            LogEnd(name, (ulong)size << 3, value);
            return size << 3;
        }

        public ulong WriteLeVar(int length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadBytes(int n, out byte[] value, string name)
        {
            byte[] bytes = new byte[n / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                long bb = ReadBits(8);
                if (bb == -1)
                    throw new EndOfStreamException();

                bytes[i] = (byte)bb;
            }
            value = bytes.ToArray();
            LogEnd(name, (ulong)(bytes.Length * 8), "byte[]");
            return (ulong)(bytes.Length * 8);
        }

        public ulong WriteBytes(int n, byte[] value, string name)
        {
            ulong size = 0;
            byte[] bytes = value;
            for (int i = 0; i < bytes.Length; i++)
            {
                size += WriteUnsignedInt(8, bytes[i], name);
            }
            return size;
        }

        public ulong Read_ns(int n, out uint value, string name)
        {
            int w = (int)Math.Floor(Math.Log2(n)) + 1;
            int m = (1 << w) - n;
            ulong size = ReadUnsignedInt(w - 1, out var v, name);
            if (v < m)
            {
                value = v;
                return size;
            }

            int extraBit = ReadBit();
            value = (uint)((v << 1) - m + extraBit);
            return size;
        }

        public ulong Write_ns(int n, uint value, string name)
        {
            throw new NotImplementedException();
        }

        public static int Clip3(int low, int high, int value)
        {
            return Math.Clamp(value, low, high);
        }

        private int _logLevel = 0;

        private void LogBegin(string name)
        {
            string padding = "-";
            for (int i = 0; i < _logLevel; i++)
            {
                padding += "-";
            }

            Log.Info($"{padding} {name}");
        }

        private void LogEnd<T>(string name, ulong size, T value)
        {
            string padding = "-";
            for (int i = 0; i < _logLevel; i++)
            {
                padding += "-";
            }

            string endPadding = "";
            for (int i = 0; i < 64 - padding.Length - name.Length - size.ToString().Length - 2; i++)
            {
                endPadding += " ";
            }

            Log.Info($"{padding} {name}{endPadding}{size}   {value}");
        }

        #region IDisposable implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion // Disposable implementation
    }
}
