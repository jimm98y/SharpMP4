
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SharpMP4
{
    public class IsoStream
    {
        private readonly Stream _stream;
        protected int _bitsPosition;
        protected int _currentBytePosition = -1;
        protected byte _currentByte = 0;

        public IsoStream(Stream stream)
        {
            _stream = stream;
        }

        private byte ReadByte()
        {
            int read = _stream.ReadByte();
            if (read == -1) throw new EndOfStreamException();
            return (byte)(read & 0xff);
        }

        private byte WriteByte(byte value)
        {
            _stream.WriteByte(value);
            return 8;
        }

        private int ReadBit()
        {
            int bytePos = _bitsPosition >> 3;

            if (_currentBytePosition != bytePos)
            {
                byte b = ReadByte();
                _currentByte = b;
                _currentBytePosition = bytePos;
            }

            int posInByte = 7 - _bitsPosition % 8;
            int bit = _currentByte >> posInByte & 1;
            ++_bitsPosition;
            return bit;
        }

        private void WriteBit(int value)
        {
            int posInByte = 7 - _bitsPosition % 8;
            int bit = (value & 1) << posInByte;
            _currentByte = (byte)(_currentByte | bit);
            ++_bitsPosition;

            int bytePos = _bitsPosition >> 3;
            if (_currentBytePosition != bytePos)
            {
                WriteByte(_currentByte);
                _currentBytePosition = bytePos;
                _currentByte = 0;
            }
        }

        private ulong ReadDescriptorSize(out int sizeOfInstance)
        {
            ulong sizeBytes = 0;

            uint i = 0;
            byte tmp = ReadByte();
            i++;
            sizeOfInstance = tmp & 0x7f;
            while (((uint)tmp >> 7) == 1)
            {
                tmp = ReadByte();
                i++;
                sizeOfInstance = sizeOfInstance << 7 | tmp & 0x7f;
            }
            sizeBytes = i;

            return sizeBytes * 8;
        }

        private ulong WriteDescriptorSize(int sizeOfInstance)
        {
            uint sizeBytesCount = CalculateDescriptorSizeLength(sizeOfInstance);

            int i = 0;
            byte[] buffer = new byte[sizeBytesCount];
            while (sizeOfInstance > 0)
            {
                i++;
                if (sizeOfInstance > 0)
                {
                    buffer[sizeBytesCount - i] = (byte)(sizeOfInstance & 0x7f);
                }
                else
                {
                    buffer[sizeBytesCount - i] = 0x80;
                }
                sizeOfInstance = sizeOfInstance >> 7;
            }

            foreach (byte b in buffer)
            {
                WriteByte(b);
            }

            return sizeBytesCount;
        }

        private static uint CalculateDescriptorSizeLength(int sizeOfInstance)
        {
            uint sizeBytesCount = 0;
            while (sizeOfInstance > 0)
            {
                sizeOfInstance = sizeOfInstance >> 7;
                sizeBytesCount++;
            }
            return sizeBytesCount;
        }

        internal ulong ReadBoxArrayTillEnd(ulong boxSize, ulong readSize, Box box)
        {
            box.Children = new List<Box>();

            ulong consumed = 0;

            if (readSize == 0)
            {
                List<Box> values = new List<Box>();
                // consume till the end of the stream
                try
                {
                    while (true)
                    {
                        Box v;
                        consumed += ReadBox(consumed, readSize, out v);
                        box.Children.Add(v);
                    }
                }
                catch (EndOfStreamException)
                { }

                return consumed;
            }

            ulong remaining = readSize - boxSize;
            while(consumed < remaining)
            {
                Box v;
                consumed += ReadBox(consumed, readSize, out v);
                box.Children.Add(v);
            }
            return consumed;
        }

        internal ulong WriteBoxArrayTillEnd(Box box)
        {
            ulong written = 0;
            foreach (var v in box.Children)
            {
                written += WriteBox(v);
            }
            return written;
        }

        internal static ulong CalculateBoxArray(Box value)
        {
            return CalculateBoxSize(value.Children);
        }

        internal ulong ReadBox(ulong boxSize, ulong readSize, out Box value)
        {
            var header = ReadBoxHeaderAsync().Result;
            value = ReadBoxAsync(header).Result;
            return header.HeaderSize + header.BoxSize;
        }

        internal ulong ReadBox(ulong boxSize, ulong readSize, out Box[] value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadClass<T>(ulong boxSize, ulong readSize, T c, out T value) where T : IMp4Serializable
        {
            ulong size = c.ReadAsync(this, readSize - boxSize).Result;
            value = c;
            return size;
        }

        internal ulong ReadClass<T>(ulong boxSize, ulong readSize, out T[] value) where T : IMp4Serializable, new()
        {
            throw new NotImplementedException();
        }

        internal ulong ReadDouble32(out double value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteDouble32(double value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteBits(uint count, short value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteBox(Box value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteBox(Box[] value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteClass(IMp4Serializable value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteClass(IMp4Serializable[] value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteClass(uint count, IMp4Serializable[] value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadString(out string value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteString(string value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadStringArray(uint count, out string[] value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteStringArray(uint count, string[] values)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateSize(string[] values)
        {
            throw new NotImplementedException();
        }        

        internal static ulong CalculateByteAlignmentSize(byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong ReadByteAlignment(out byte value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteByteAlignment(byte value)
        {
            throw new NotImplementedException();
        }

        internal static int BitsToDecode()
        {
            throw new NotImplementedException();
        }



        internal ulong ReadUInt8ArrayTillEnd(ulong boxSize, ulong readSize, out byte[] value)
        {
            ulong consumed = 0;

            if (readSize == 0)
            {
                List<byte> values = new List<byte>();
                // consume till the end of the stream
                try
                {
                    while (true)
                    {
                        byte v;
                        consumed += ReadUInt8(out v);
                        values.Add(v);
                    }
                }
                catch (EndOfStreamException)
                { }

                value = values.ToArray();
                return consumed;
            }

            ulong remaining = readSize - boxSize;
            int count = (int)(remaining >> 3);
            value = new byte[count];
            for (int i = 0; i < count; i++)
            {
                consumed += ReadUInt8(out value[i]);
            }
            return consumed;
        }

        internal ulong ReadUInt32ArrayTillEnd(ulong boxSize, ulong readSize, out uint[] value)
        {
            ulong consumed = 0;

            if (readSize == 0)
            {
                List<uint> values = new List<uint>();
                // consume till the end of the stream
                try
                {
                    while(true)
                    {
                        uint v;
                        consumed += ReadUInt32(out v);
                        values.Add(v);
                    }
                }
                catch (EndOfStreamException)
                { }

                value = values.ToArray();
                return consumed;
            }

            ulong remaining = readSize - boxSize;
            int count = (int)(remaining >> 5);
            value = new uint[count];
            for (int i = 0; i < count; i++)
            {
                consumed += ReadUInt32(out value[i]);
            }
            return consumed;
        }

        internal ulong WriteUInt8ArrayTillEnd(byte[] value)
        {
            return WriteUInt8Array((uint)value.Length, value);
        }

        internal ulong WriteUInt32ArrayTillEnd(uint[] value)
        {
            return WriteUInt32Array((uint)value.Length, value);
        }

        internal ulong ReadBslbf(ulong count, out ushort value)
        {
            return ReadBits((uint)count, out value);
        }

        internal ulong WriteBslbf(ulong count, ushort value)
        {
            return WriteBits((uint)count, value);
        }

        internal ulong ReadBslbf(ulong count, out byte value)
        {
            return ReadBits((uint)count, out value);
        }

        internal ulong WriteBslbf(ulong count, byte value)
        {
            return WriteBits((uint)count, value);
        }

        internal ulong ReadUimsbf(ulong count, out byte value)
        {
            return ReadBits((uint)count, out value);
        }

        internal ulong WriteUimsbf(ulong count, byte value)
        {
            return WriteBits((uint)count, value);
        }

        internal ulong ReadUimsbf(ulong count, out ushort value)
        {
            return ReadBits((uint)count, out value);
        }

        internal ulong WriteUimsbf(ulong count, ushort value)
        {
            return WriteBits((uint)count, value);
        }

        internal ulong ReadUimsbf(ulong count, out uint value)
        {
            return ReadBits((uint)count, out value);
        }

        internal ulong WriteUimsbf(ulong count, uint value)
        {
            return WriteBits((uint)count, value);
        }

        internal ulong ReadBits(uint count, out byte[] value)
        {
            value = new byte[(count >> 3) + (count % 8)];
            int i = 0;
            int c = (int)count;
            while (i < value.Length && c > 0)
            {
                byte v = 0;
                c -= (int)ReadBits((uint)Math.Min(c, 8), out v);
                value[i] = v;
                i++;
            }
            return count;
        }

        internal ulong WriteBits(uint count, byte[] value)
        {
            int c = (int)count;
            int i = 0;
            while (i < value.Length && c > 0)
            {
                c -= (int)WriteBits((uint)Math.Min(c, 8), value[i]);
                i++;
            }
            return count;
        }

        internal ulong ReadUimsbf(out bool value)
        {
            return ReadBit(out value);
        }

        internal ulong WriteUimsbf(bool value)
        {
            return WriteBit(value);
        }

        internal ulong ReadBslbf(out bool value)
        {
            return ReadBit(out value);
        }

        internal ulong WriteBslbf(bool value)
        {
            return WriteBit(value);
        }

        internal ulong ReadBit(out bool value)
        {
            value = ReadBit() != 0;
            return 1;
        }

        internal ulong ReadBits(uint count, out byte value)
        {
            if (count > 8) throw new ArgumentException();
            ulong ret = ReadBits(count, out uint v);
            value = (byte)v;
            return ret;
        }

        internal ulong ReadBits(uint count, out ushort value)
        {
            if (count > 16) throw new ArgumentException();
            ulong ret = ReadBits(count, out uint v);
            value = (ushort)v;
            return ret;
        }

        internal ulong ReadBits(uint count, out short value)
        {
            if (count > 16) throw new ArgumentException();
            uint originalCount = count;

            int sign = ReadBit() == 1 ? -1 : 1;
            count--;

            int res = 0;
            while (count > 0)
            {
                res = res << 1;
                int u1 = ReadBit();

                if (u1 == -1)
                {
                    throw new EndOfStreamException();
                }

                res |= u1;
                count--;
            }

            value = (short)(res * sign);
            return originalCount;
        }

        internal ulong ReadBits(uint count, out uint value)
        {
            if (count > 32) throw new ArgumentException();
            uint originalCount = count;
            int res = 0;
            while (count > 0)
            {
                res = res << 1;
                int u1 = ReadBit();

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

        internal ulong WriteBit(bool value)
        {
            WriteBit(value ? 1 : 0);
            return 1;
        }

        internal ulong WriteBits(uint count, byte value)
        {
            if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteBits(count, (uint)value);
        }

        internal ulong WriteBits(uint count, ushort value)
        {
            if (count > 16)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteBits(count, (uint)value);
        }

        internal ulong WriteBits(uint count, uint value)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            uint originalCount = count;
            while (count > 0)
            {
                int bits = (int)count - 1;
                int mask = 0x1 << bits;
                WriteBit((int)((value & mask) >> bits));
                count--;
            }
            return originalCount;
        }

        internal ulong ReadBytes(ulong length, out byte[] value)
        {
            value = new byte[length];
            _stream.ReadExactly(value, 0, (int)length);
            return length * 8;
        }

        internal ulong WriteBytes(ulong count, byte[] value)
        {
            _stream.WriteAsync(value, 0, (int)count);
            return count * 8;
        }

        internal static uint FromFourCC(string input)
        {
            if(string.IsNullOrEmpty(input))
                return 0;
            byte[] buffer = Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
            if (buffer.Length != 4)
                throw new Exception("Invalid 4cc!");
            return (uint)(
                (buffer[0] << 24) +
                (buffer[1] << 16) +
                (buffer[2] << 8) +
                (buffer[3] << 0)
            );
        }

        internal static string ToFourCC(uint value)
        {
            byte[] buffer = {
            (byte)(value >> 24 & 0xFF),
            (byte)(value >> 16 & 0xFF),
            (byte)(value >> 8 & 0xFF),
            (byte)(value & 0xFF)
        };
            return Encoding.GetEncoding("ISO-8859-1").GetString(buffer);
        }

        internal ulong ReadBslbf(ulong count, out byte[] value)
        {
            return ReadBytes(count / 8, out value);
        }

        internal ulong WriteBslbf(ulong count, byte[] value)
        {
            return WriteBytes(count / 8, value);
        }

        internal ulong ReadStringZeroTerminated(ulong boxSize, ulong readSize, out string value)
        {
            ulong remaining = readSize - boxSize;
            if (remaining == 0)
            {
                value = "";
                return 0;
            }

            List<byte> buffer = new List<byte>();
            byte c;
            while ((c = ReadByte()) != 0)
            {
                buffer.Add(c);
            }
            value = Encoding.UTF8.GetString(buffer.ToArray());
            return (ulong)(buffer.Count + 1) * 8;
        }

        internal ulong ReadInt16(out short value)
        {
            ulong count = unchecked(ReadUInt16(out ushort v));
            value = unchecked((short)v);
            return count;
        }

        internal ulong ReadInt32(out int value)
        {
            ulong count = unchecked(ReadUInt32(out uint v));
            value = unchecked((int)v);
            return count;
        }

        internal ulong ReadInt32(out long value)
        {
            ulong count = unchecked(ReadUInt32(out uint v));
            value = unchecked((int)v);
            return count;
        }

        internal ulong ReadInt64(out long value)
        {
            ulong count = unchecked(ReadUInt64(out ulong v));
            value = unchecked((long)v);
            return count;
        }

        internal ulong ReadInt8(out sbyte value)
        {
            ulong count = unchecked(ReadUInt8(out byte v));
            value = unchecked((sbyte)v);
            return count;
        }

        internal ulong ReadUInt8(out byte value)
        {
            value = ReadByte();
            return 8;
        }

        internal ulong ReadUInt8(out ushort value)
        {
            value = ReadByte();
            return 8;
        }

        internal ulong ReadUInt16(out ushort value)
        {
            value = (ushort)(
                (ReadByte() << 8) +
                (ReadByte() << 0)
            );
            return 16;
        }

        internal ulong ReadUInt16(out uint value)
        {
            value = (uint)(
                (ReadByte() << 8) +
                (ReadByte() << 0)
            );
            return 16;
        }

        internal ulong ReadUInt24(out uint value)
        {
            value = (uint)(
                (ReadByte() << 16) +
                (ReadByte() << 8) +
                (ReadByte() << 0)
            );
            return 24;
        }

        internal ulong ReadUInt32(out uint value)
        {
            value = (uint)(
                (ReadByte() << 24) +
                (ReadByte() << 16) +
                (ReadByte() << 8) +
                (ReadByte() << 0)
            );
            return 32;
        }

        internal ulong ReadUInt32(out ulong value)
        {
            value = (uint)(
                (ReadByte() << 24) +
                (ReadByte() << 16) +
                (ReadByte() << 8) +
                (ReadByte() << 0)
            );
            return 32;
        }

        internal ulong ReadUInt48(out ulong value)
        {
            value = (ulong)(
                (ReadByte() << 40) +
                (ReadByte() << 32) +
                (ReadByte() << 24) +
                (ReadByte() << 16) +
                (ReadByte() << 8) +
                (ReadByte() << 0)
            );
            return 48;
        }

        internal ulong ReadUInt64(out ulong value)
        {
            value = (ulong)(
                (ReadByte() << 56) +
                (ReadByte() << 48) +
                (ReadByte() << 40) +
                (ReadByte() << 32) +
                (ReadByte() << 24) +
                (ReadByte() << 16) +
                (ReadByte() << 8) +
                (ReadByte() << 0)
            );
            return 64;
        }

        internal ulong ReadUInt16Array(int count, out ushort[] value)
        {
            ulong size = 0;
            value = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                size += ReadUInt16(out value[i]);
            }
            return size;
        }

        internal ulong ReadUInt16Array(int count, out uint[] value)
        {
            ulong size = 0;
            value = new uint[count];
            for (int i = 0; i < count; i++)
            {
                size += ReadUInt16(out value[i]);
            }
            return size;
        }

        internal ulong ReadUInt32Array(uint count, out uint[] value)
        {
            ulong size = 0;
            value = new uint[count];
            for (int i = 0; i < count; i++)
            {
                size += ReadUInt32(out value[i]);
            }
            return size;
        }

        internal ulong ReadUInt32Array(uint count, out ulong[] value)
        {
            ulong size = 0;
            value = new ulong[count];
            for (int i = 0; i < count; i++)
            {
                size += ReadUInt32(out value[i]);
            }
            return size;
        }

        internal ulong ReadUInt64Array(uint count, out ulong[] value)
        {
            ulong size = 0;
            value = new ulong[count];
            for (int i = 0; i < count; i++)
            {
                size += ReadUInt64(out value[i]);
            }
            return size;
        }

        internal ulong ReadUInt8Array(uint count, out byte[] value)
        {
            ulong size = 0;
            value = new byte[count];
            for (int i = 0; i < count; i++)
            {
                size += ReadUInt8(out value[i]);
            }
            return size;
        }

        internal ulong WriteStringZeroTerminated(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            byte[] buffer = Encoding.UTF8.GetBytes(value);
            for (int i = 0; i < buffer.Length; i++)
            {
                WriteByte(buffer[i]);
            }
            WriteByte(0);
            return (ulong)(buffer.Length + 1) * 8;
        }

        internal ulong WriteInt16(short value)
        {
            return WriteUInt16(unchecked((ushort)value));
        }

        internal ulong WriteInt32(int value)
        {
            return WriteUInt32(unchecked((uint)value));
        }

        internal ulong WriteInt32(long value)
        {
            return WriteUInt32(unchecked((uint)value));
        }

        internal ulong WriteInt64(long value)
        {
            return WriteUInt64(unchecked((ulong)value));
        }

        internal ulong WriteInt8(sbyte value)
        {
            return WriteUInt8(unchecked((byte)value));
        }

        internal ulong WriteUInt8(byte value)
        {
            _stream.WriteByte(value);
            return 1;
        }

        internal ulong WriteUInt8(ushort value)
        {
            _stream.WriteByte((byte)value);
            return 1;
        }

        internal ulong WriteUInt16(ushort value)
        {
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 16;
        }

        internal ulong WriteUInt16(uint value)
        {
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 16; throw new NotImplementedException();
        }

        internal ulong WriteUInt24(uint value)
        {
            value = value & 0xFFFFFF;
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 24;
        }

        internal ulong WriteUInt32(uint value)
        {
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 32;
        }

        internal ulong WriteUInt32(ulong value)
        {
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 32;
        }

        internal ulong WriteUInt48(ulong value)
        {
            WriteByte((byte)(value >> 40 & 0xFF));
            WriteByte((byte)(value >> 32 & 0xFF));
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 48;
        }

        internal ulong WriteUInt64(ulong value)
        {
            WriteByte((byte)(value >> 56 & 0xFF));
            WriteByte((byte)(value >> 48 & 0xFF));
            WriteByte((byte)(value >> 40 & 0xFF));
            WriteByte((byte)(value >> 32 & 0xFF));
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 64;
        }

        internal ulong WriteUInt16Array(uint count, ushort[] value)
        {
            ulong size = 0;
            for (int i = 0; i < count; i++)
            {
                size += WriteUInt16(value[i]);
            }
            return size;
        }

        internal ulong WriteUInt16Array(uint count, uint[] value)
        {
            ulong size = 0;
            for (int i = 0; i < count; i++)
            {
                size += WriteUInt16(value[i]);
            }
            return size;
        }

        internal ulong WriteUInt8Array(uint count, byte[] value)
        {
            ulong size = 0;
            for (int i = 0; i < count; i++)
            {
                size += WriteUInt8(value[i]);
            }
            return size;
        }

        internal ulong WriteUInt32Array(uint count, uint[] value)
        {
            ulong size = 0;
            for (int i = 0; i < count; i++)
            {
                size += WriteUInt32(value[i]);
            }
            return size;
        }

        internal ulong WriteUInt32Array(uint count, ulong[] value)
        {
            ulong size = 0;
            for (int i = 0; i < count; i++)
            {
                size += WriteUInt32(value[i]);
            }
            return size;
        }

        internal ulong WriteUInt64Array(uint count, ulong[] value)
        {
            ulong size = 0;
            for (int i = 0; i < count; i++)
            {
                size += WriteUInt64(value[i]);
            }
            return size;
        }

        internal static ulong CalculateBoxSize(IEnumerable<Box> boxes)
        {
            ulong size = 0;
            foreach (Box box in boxes)
            {
                size += box.CalculateSize();
            }
            return size;
        }

        internal static ulong CalculateClassSize(IMp4Serializable value)
        {
            return value.CalculateSize();
        }

        internal static ulong CalculateClassSize(IMp4Serializable[] value)
        {
            ulong size = 0;
            foreach (IMp4Serializable c in value)
            {
                size += c.CalculateSize();
            }
            return size;
        }

        internal ulong WriteIso639(string value)
        {
            if (Encoding.UTF8.GetBytes(value).Length != 3)
            {
                throw new ArgumentException($"\"{value}\" value string must be 3 characters long!");
            }
            int bits = 0;
            for (int i = 0; i < 3; i++)
            {
                bits += Encoding.UTF8.GetBytes(value)[i] - 0x60 << (2 - i) * 5;
            }
            return WriteUInt16((ushort)bits);
        }

        internal ulong ReadIso639(out string value)
        {
            ushort bits;
            ulong read = ReadBits(15, out bits);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                int c = bits >> (2 - i) * 5 & 0x1f;
                result.Append((char)(c + 0x60));
            }
            value = result.ToString();
            return read;
        }

        internal ulong ReadAlignedBits(uint count, out bool value)
        {
            return ReadBit(out value);
        }

        internal ulong WriteAlignedBits(uint count, bool value)
        {
            return WriteBit(value);
        }

        internal ulong ReadAlignedBits(uint count, out byte value)
        {
            return ReadBits(count, out value);
        }

        internal ulong WriteAlignedBits(uint count, byte value)
        {
            return WriteBits(count, value);
        }

        internal long GetCurrentOffset()
        {
            return _stream.Position;
        }

        public async Task<Mp4BoxHeader> ReadBoxHeaderAsync()
        {
            BoxHeader header = new BoxHeader();
            long headerOffset = this.GetCurrentOffset();
            ulong headerSize = await header.ReadAsync(this, 0);
            return new Mp4BoxHeader(header, headerOffset, headerSize);
        }

        public async Task<Box> ReadBoxAsync(Mp4BoxHeader header)
        {
            var box = BoxFactory.CreateBox(ToFourCC(header.Header.Type));
            Debug.WriteLine($"--Parsed: {box.FourCC}");
            ulong size = await box.ReadAsync(this, header.BoxSize);

            if (size != header.BoxSize && header.BoxSize != 0)
            {
                if(size < header.BoxSize)
                {
                    // TODO: Investigate and fix
                    Debug.Assert(false);
                    ReadBits((uint)(header.BoxSize - size), out byte[] missing);
                }
                throw new Exception("Box not fully read!");
            }
            return box;
        }

        internal ulong ReadEntry(string fourCC, out SampleGroupDescriptionEntry entry)
        {
            var res = BoxFactory.CreateEntry(fourCC);
            Debug.WriteLine($"--Parsed entry: {fourCC}");
            ulong size = res.ReadAsync(this, 0).Result;
            entry = (SampleGroupDescriptionEntry)res;
            return size;
        }

        internal ulong ReadEntry(out VvcSubpicOrderEntry sampleGroupDescriptionEntry)
        {
            SampleGroupDescriptionEntry ge;
            ulong size = ReadEntry(VvcSubpicOrderEntry.TYPE, out ge);
            sampleGroupDescriptionEntry = (VvcSubpicOrderEntry)ge;
            return size;
        }

        internal ulong ReadEntry(out VvcSubpicIDEntry sampleGroupDescriptionEntry)
        {
            SampleGroupDescriptionEntry ge;
            ulong size = ReadEntry(VvcSubpicIDEntry.TYPE, out ge);
            sampleGroupDescriptionEntry = (VvcSubpicIDEntry)ge;
            return size;
        }

        internal ulong ReadDescriptor(out ES_Descriptor descriptor)
        {
            Descriptor d;
            ulong size = ReadDescriptor(out d);
            descriptor = (ES_Descriptor)d;
            return size;
        }

        internal ulong ReadDescriptor(out Descriptor descriptor)
        {
            byte tag;
            ulong size = ReadUInt8(out tag);
            size += ReadDescriptorSize(out int sizeOfInstance);
            ulong sizeOfInstanceBits = (ulong)sizeOfInstance * 8;
            descriptor = DescriptorFactory.CreateDescriptor(tag);
            ulong readInstanceSizeBits = descriptor.ReadAsync(this, sizeOfInstanceBits).Result;
            if (readInstanceSizeBits != sizeOfInstanceBits)
                throw new Exception("Descriptor not fully read!");
            size += sizeOfInstanceBits;
            return size;
        }

        internal ulong ReadDescriptorsTillEnd(ulong boxSize, ulong readSize, Descriptor descriptor, int objectTypeIndication = -1)
        {
            descriptor.Children = new List<Descriptor>();

            ulong consumed = 0;

            if (readSize == 0)
            {
                List<Box> values = new List<Box>();
                // consume till the end of the stream
                try
                {
                    while (true)
                    {
                        Descriptor v;
                        consumed += ReadDescriptor(out v);
                        descriptor.Children.Add(v);
                    }
                }
                catch (EndOfStreamException)
                { }

                return consumed;
            }

            ulong remaining = readSize - boxSize;
            while (consumed < remaining)
            {
                Descriptor v;
                consumed += ReadDescriptor(out v);
                descriptor.Children.Add(v);
            }
            return consumed;
        }

        internal ulong WriteDescriptorsTillEnd(Descriptor descriptor, int objectTypeIndication = -1)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateDescriptors(Descriptor descriptor, int objectTypeIndication = -1)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteDescriptor(Descriptor descriptor)
        {
            throw new NotImplementedException();
        }

        internal static ulong CalculateDescriptorSize(Descriptor descriptor)
        {
            throw new NotImplementedException();
        }       
    }

#if !NET7_0_OR_GREATER
    public static class StreamExtensions
    {
        public static int ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = stream.Read(buffer, offset + totalRead, count - totalRead);
                if (read == 0)
                {
                    return totalRead;
                }

                totalRead += read;
            }

            return totalRead;
        }
    }
#endif

    public class Mp4BoxHeader
    {
        public BoxHeader Header { get; set; }
        public long HeaderOffset { get; set; }
        public ulong HeaderSize { get; set; }
        public ulong BoxSize { get; set; }

        public Mp4BoxHeader(BoxHeader header, long headerOffset, ulong headerSize)
        {
            this.Header = header;
            this.HeaderOffset = headerOffset;
            this.HeaderSize = headerSize;
            this.BoxSize = header.GetBoxSizeInBits() - headerSize;
        }
    }
}