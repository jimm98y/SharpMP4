using System;
using System.IO;
using System.Linq;

namespace SharpAV1
{
    public class AomStream : IDisposable
    {
        private int _bitsPosition;
        private int _currentBytePosition = -1;
        private byte _currentByte = 0;

        private readonly Stream _stream;

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

        #endregion // Bit read/write

        public ulong ReadLeb128(out int value, string name)
        {
            int v = 0;
            int Leb128Bytes = 0;
            for (int i = 0; i < 8; i++)
            {
                int leb128_byte = ReadByte();
                v = v | ((leb128_byte & 0x7f) << (i * 7));
                Leb128Bytes += 1;
                if((leb128_byte & 0x80) == 0)
                {
                    break;
                }
            }

            if (v <= int.MaxValue)
            {
                value = v;
            }
            else
            {
                throw new InvalidDataException($"Invalid LEB128 value: {v}");
            }

            LogEnd(name, (ulong)Leb128Bytes << 3, v);

            return (ulong)Leb128Bytes << 3;
        }

        public ulong ReadFixed(int count, out int value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(count, out uint v, name);
            value = (int)v;
            return read;
        }

        public ulong ReadVariable(long count, out int value, string name)
        {
            var size = ReadUnsignedInt((int)count, out uint v, name);
            value = (int)v;
            return size;
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

        public ulong ReadSignedIntVar(int count, out int value, string name)
        {
            ulong size = ReadUnsignedInt(count, out uint v, name);
            long signMask = 1 << (count - 1);
            if ((v & signMask) > 0)
                value = (int)(v - 2 * signMask);
            else
                value = (int)v;
            LogEnd(name, size, value);
            return size;
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

        public ulong ReadBytes(int count, out byte[] value, string name)
        {
            byte[] bytes = new byte[count / 8];
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

        public ulong Read_ns(int count, out uint value, string name)
        {
            int w = (int)Math.Floor(Math.Log2(count)) + 1;
            int m = (1 << w) - count;
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
                    if (_stream != null)
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

        #endregion // Disposable implementation
    }
}
