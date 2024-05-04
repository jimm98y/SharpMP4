using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpMp4
{
#if !NET7_0_OR_GREATER
    public static class Mp4StreamExtensions
    {
        public static Task<int> ReadExactlyAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            return ReadExactlyAsync(stream, buffer, offset, count, CancellationToken.None);
        }
         
        public static async Task<int> ReadExactlyAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = await stream.ReadAsync(buffer, offset + totalRead, count - totalRead, cancellationToken).ConfigureAwait(false);
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

    public static class IsoReaderWriter
    {
        public static readonly DateTime DateTimeBase = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDate(ulong seconds)
        {
            return DateTimeBase.AddSeconds(seconds);
        }

        public static ulong FromDate(DateTime date)
        {
            return (ulong)date.Subtract(DateTimeBase).TotalSeconds;
        }

        public static uint WriteByte(Stream stream, byte value)
        {
            //if (stream.Position == 28)
            //{ }
            stream.WriteByte(value);
            return 1;
        }

        public static byte ReadByte(Stream stream)
        {
            //if (stream.Position == 28)
            //{ }
            int read = stream.ReadByte();
            if (read == -1) throw new EndOfStreamException();
            byte value = (byte)(read & 0xff);
            return value;
        }

        public static async Task<uint> WriteBytesAsync(Stream stream, byte[] bytes, int offset, int length)
        {
            //if (stream.Position == 28)
            //{ }
            await stream.WriteAsync(bytes, offset, length);
            return (uint)length;
        }

        public static async Task ReadBytesAsync(Stream stream, byte[] bytes, int offset, int length)
        {
            //if (stream.Position == 28)
            //{ }
            await stream.ReadExactlyAsync(bytes, offset, length);
        }

        public static (int sizeOfInstance, int sizeBytes) ReadPackedNumber(Stream stream)
        {
            int sizeOfInstance;
            int sizeBytes;

            int i = 0;
            byte tmp = ReadByte(stream);
            i++;
            sizeOfInstance = tmp & 0x7f;
            while ((int)((uint)tmp >> 7) == 1)
            {
                tmp = ReadByte(stream);
                i++;
                sizeOfInstance = sizeOfInstance << 7 | tmp & 0x7f;
            }
            sizeBytes = i;

            return (sizeOfInstance, sizeBytes);
        }

        public static uint WritePackedNumber(Stream stream, uint size)
        {
            uint sizeBytesCount = CalculatePackedNumberLength(size);

            int i = 0;
            byte[] buffer = new byte[sizeBytesCount];
            while (size > 0)
            {
                i++;
                if (size > 0)
                {
                    buffer[sizeBytesCount - i] = (byte)(size & 0x7f);
                }
                else
                {
                    buffer[sizeBytesCount - i] = 0x80;
                }
                size = size >> 7;
            }

            foreach (byte b in buffer)
            {
                WriteByte(stream, b);
            }

            return sizeBytesCount;
        }

        public static uint CalculatePackedNumberLength(uint size)
        {
            uint sizeBytesCount = 0;
            while (size > 0)
            {
                size = size >> 7;
                sizeBytesCount++;
            }
            return sizeBytesCount;
        }

        public static uint WriteFixedPoint88(Stream stream, double value)
        {
            short result = (short)(value * 256);
            WriteByte(stream, (byte)((result & 0xFF00) >> 8));
            WriteByte(stream, (byte)(result & 0x00FF));
            return 2;
        }

        public static float ReadFixedPoint88(Stream stream)
        {
            short result = 0;
            result |= (short)(ReadByte(stream) << 8 & 0xFF00);
            result |= (short)(ReadByte(stream) & 0xFF);
            return (float)result / 256;
        }

        public static uint WriteFixedPoint1616(Stream stream, double value)
        {
            int result = (int)(value * 65536);
            WriteByte(stream, (byte)((result & 0xFF000000) >> 24));
            WriteByte(stream, (byte)((result & 0x00FF0000) >> 16));
            WriteByte(stream, (byte)((result & 0x0000FF00) >> 8));
            WriteByte(stream, (byte)(result & 0x000000FF));
            return 4;
        }

        public static double ReadFixedPoint1616(Stream stream)
        {
            int result = 0;
            result |= (int)(ReadByte(stream) << 24 & 0xFF000000);
            result |= ReadByte(stream) << 16 & 0xFF0000;
            result |= ReadByte(stream) << 8 & 0xFF00;
            result |= ReadByte(stream) & 0xFF;
            return (double)result / 65536;
        }

        public static uint WriteFixedPoint0230(Stream stream, double value)
        {
            int result = (int)(value * (1 << 30));
            WriteByte(stream, (byte)((result & 0xFF000000) >> 24));
            WriteByte(stream, (byte)((result & 0x00FF0000) >> 16));
            WriteByte(stream, (byte)((result & 0x0000FF00) >> 8));
            WriteByte(stream, (byte)(result & 0x000000FF));
            return 4;
        }

        public static double ReadFixedPoint0230(Stream stream)
        {
            int result = 0;
            result |= (int)(ReadByte(stream) << 24 & 0xFF000000);
            result |= ReadByte(stream) << 16 & 0xFF0000;
            result |= ReadByte(stream) << 8 & 0xFF00;
            result |= ReadByte(stream) & 0xFF;
            return (double)result / (1 << 30);
        }

        public static uint Write4cc(Stream stream, string type)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(type);
            if (buffer.Length != 4)
                throw new Exception("Invalid 4cc!");
            stream.Write(buffer, 0, 4);
            return 4;
        }

        public static string Read4cc(Stream stream)
        {
            byte[] buffer = new byte[]
            {
                ReadByte(stream),
                ReadByte(stream),
                ReadByte(stream),
                ReadByte(stream)
            };
            return Encoding.ASCII.GetString(buffer);
        }

        public static uint WriteUInt16(Stream stream, ushort value)
        {
            WriteByte(stream, (byte)(value >> 8 & 0xFF));
            WriteByte(stream, (byte)(value & 0xFF));
            return 2;
        }

        public static ushort ReadUInt16(Stream stream)
        {
            return (ushort)((ReadByte(stream) << 8) + (ReadByte(stream) << 0));
        }

        public static uint WriteUInt24(Stream stream, uint value)
        {
            value = value & 0xFFFFFF;
            WriteByte(stream, (byte)(value >> 16 & 0xFF));
            WriteByte(stream, (byte)(value >> 8 & 0xFF));
            WriteByte(stream, (byte)(value & 0xFF));
            return 3;
        }

        public static uint ReadUInt24(Stream stream)
        {
            return (uint)((ReadByte(stream) << 16) + (ReadByte(stream) << 8) + (ReadByte(stream) << 0));
        }

        public static uint WriteUInt32(Stream stream, uint value)
        {
            WriteByte(stream, (byte)(value >> 24 & 0xFF));
            WriteByte(stream, (byte)(value >> 16 & 0xFF));
            WriteByte(stream, (byte)(value >> 8 & 0xFF));
            WriteByte(stream, (byte)(value & 0xFF));
            return 4;
        }

        public static uint ReadUInt32(Stream stream)
        {
            return (uint)((ReadByte(stream) << 24) + (ReadByte(stream) << 16) + (ReadByte(stream) << 8) + (ReadByte(stream) << 0));
        }

        public static uint WriteUInt64(Stream stream, ulong value)
        {
            WriteByte(stream, (byte)(value >> 56 & 0xFF));
            WriteByte(stream, (byte)(value >> 48 & 0xFF));
            WriteByte(stream, (byte)(value >> 40 & 0xFF));
            WriteByte(stream, (byte)(value >> 32 & 0xFF));
            WriteByte(stream, (byte)(value >> 24 & 0xFF));
            WriteByte(stream, (byte)(value >> 16 & 0xFF));
            WriteByte(stream, (byte)(value >> 8 & 0xFF));
            WriteByte(stream, (byte)(value & 0xFF));
            return 8;
        }

        public static ulong ReadUInt64(Stream stream)
        {
            return (ulong)((ReadByte(stream) << 56) + (ReadByte(stream) << 48) + (ReadByte(stream) << 40) + (ReadByte(stream) << 32) + (ReadByte(stream) << 24) + (ReadByte(stream) << 16) + (ReadByte(stream) << 8) + (ReadByte(stream) << 0));
        }

        public static uint WriteIso639(Stream stream, string language)
        {
            if (Encoding.UTF8.GetBytes(language).Length != 3)
            {
                throw new ArgumentException($"\"{language}\" language string must be 3 characters long!");
            }
            int bits = 0;
            for (int i = 0; i < 3; i++)
            {
                bits += Encoding.UTF8.GetBytes(language)[i] - 0x60 << (2 - i) * 5;
            }
            return WriteUInt16(stream, (ushort)bits);
        }

        public static string ReadIso639(Stream stream)
        {
            int bits = ReadUInt16(stream);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                int c = bits >> (2 - i) * 5 & 0x1f;
                result.Append((char)(c + 0x60));
            }
            return result.ToString();
        }

        public static uint WriteString(Stream stream, string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            stream.Write(buffer, 0, buffer.Length);
            return (uint)buffer.Length;
        }

        public static async Task<string> ReadStringAsync(Stream stream, uint remainingSize)
        {
            byte[] buffer = new byte[remainingSize];
            await ReadBytesAsync(stream, buffer, 0, (int)remainingSize);
            string str = Encoding.UTF8.GetString(buffer);
            return str;
        }

        public static uint WriteInt32(Stream stream, int value)
        {
            // TODO!!!
            return WriteUInt32(stream, (uint)value);
        }

        public static int ReadInt32(Stream stream)
        {
            // TODO!!!
            return (int)ReadUInt32(stream);
        }

        public static ulong ReadUInt48(Stream stream)
        {
            return (ulong)((ReadByte(stream) << 40) + (ReadByte(stream) << 32) + (ReadByte(stream) << 24) + (ReadByte(stream) << 16) + (ReadByte(stream) << 8) + (ReadByte(stream) << 0));
        }

        public static uint WriteUInt48(Stream stream, ulong value)
        {
            WriteByte(stream, (byte)(value >> 40 & 0xFF));
            WriteByte(stream, (byte)(value >> 32 & 0xFF));
            WriteByte(stream, (byte)(value >> 24 & 0xFF));
            WriteByte(stream, (byte)(value >> 16 & 0xFF));
            WriteByte(stream, (byte)(value >> 8 & 0xFF));
            WriteByte(stream, (byte)(value & 0xFF));
            return 6;
        }
    }

    /// <summary>
    /// Consumes bits, reading and caching 1 byte at a time.
    /// </summary>
    public class BitStreamReader : IDisposable
    {
        private readonly bool _disposeStream;
        private Stream _stream;
        private int _bitsPosition;
        private int _currentBytePosition = -1;
        private byte _currentByte = 0;
        private bool _disposedValue;

        private int _prevByte = -1;

        public bool ShouldUnescapeNals { get; set; } = true;

        public BitStreamReader(byte[] bytes) : this(new MemoryStream(bytes), true)
        { }

        public BitStreamReader(Stream stream, bool disposeUnderlyingStream = false)
        {
            _disposeStream = disposeUnderlyingStream;
            _stream = stream;
            _bitsPosition = 0;
        }

        public int ReadBit()
        {
            int bytePos = _bitsPosition / 8;

            if (_currentBytePosition != bytePos)
            {
                byte b = IsoReaderWriter.ReadByte(_stream);

                // remove emulation prevention byte
                if(ShouldUnescapeNals && _prevByte == 0 && _currentByte == 0 && b == 0x03)
                {
                    _prevByte = b;
                    b = IsoReaderWriter.ReadByte(_stream);
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

        public int ReadBits(int count)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));

            int res = 0;
            while (count > 0)
            {
                res = res << 1;
                int u1 = ReadBit();

                if (u1 == -1)
                    return u1;

                res |= u1;
                count--;
            }

            return res;
        }

        public long ReadBitsLong(int count)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            int res = 0;
            while (count > 0)
            {
                res = res << 1;
                int u1 = ReadBit();

                if (u1 == -1)
                    return u1;

                res |= u1;
                count--;
            }

            return res;
        }

        public int ReadUE()
        {
            int cnt = 0;
            while (ReadBit() == 0)
            {
                cnt++;
            }

            int res = 0;
            if (cnt > 0)
            {
                res = (1 << cnt) - 1 + ReadBits(cnt);
            }

            return res;
        }

        public int ReadSE()
        {
            int val = ReadUE();
            int sign = ((val & 0x1) << 1) - 1;
            val = ((val >> 1) + (val & 0x1)) * sign;
            return val;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_disposeStream)
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

        public uint RemainingBits(uint size)
        {
            uint totalBitSize = size * 8;
            return totalBitSize - (uint)_bitsPosition;
        }

        public bool HasMoreRBSPData(uint size)
        {
            uint remaining = RemainingBits(size);
            if (remaining <= 8)
                return false;

            int tail = 1 << (8 - _currentBytePosition - 1);
            int mask = (tail << 1) - 1;
            bool hasTail = (_currentByte & mask) == tail;
            return !hasTail;
        }

        public void ReadTrailingBits()
        {
            // TODO
        }
    }

    /// <summary>
    /// Writes bits, caching 1 byte at a time.
    /// </summary>
    public class BitStreamWriter : IDisposable
    {
        private readonly bool _disposeStream;
        private Stream _stream;
        private uint _bitsPosition;
        private uint _currentBytePosition = 0;
        private byte _currentByte = 0;
        private bool _disposedValue;

        private int _prevByte = -1;
        private int _prevPrevByte = -1;

        public bool ShouldEscapeNals { get; set; } = true;

        public uint WrittenBytes { get { return _currentBytePosition; } }

        public BitStreamWriter(Stream stream, bool disposeUnderlyingStream = false)
        {
            _disposeStream = disposeUnderlyingStream;
            _stream = stream;
            _bitsPosition = 0;
        }

        public void WriteBit(bool value)
        {
            WriteBit(value ? 1 : 0);
        }

        public void WriteBit(int value)
        {
            int posInByte = 7 - (int)_bitsPosition % 8;
            int bit = (value & 1) << posInByte;
            _currentByte = (byte)(_currentByte | bit);
            ++_bitsPosition;

            uint bytePos = _bitsPosition / 8;
            if (_currentBytePosition != bytePos)
            {
                if (ShouldEscapeNals)
                {
                    // write emulation prevention byte to properly escape sequences of 0x000000, 0x000001, 0x000002, 0x000003 into 0x00000300, 0x00000301, 0x00000302 and 0x00000303 respectively
                    if (_prevByte == 0x00 && _prevPrevByte == 0x00 && (_currentByte == 0x00 || _currentByte == 0x01 || _currentByte == 0x02 || _currentByte == 0x03))
                    {
                        IsoReaderWriter.WriteByte(_stream, 0x03);
                        bytePos++;
                        _bitsPosition += 8;
                        _prevByte = 0x03;
                    }
                }
                
                IsoReaderWriter.WriteByte(_stream, _currentByte);
                _currentBytePosition = bytePos;

                _prevPrevByte = _prevByte;
                _prevByte = _currentByte;

                _currentByte = 0;
            }
        }

        public void WriteBits(uint count, int value)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count > 0)
            {
                int bits = (int)count - 1;
                int mask = 0x1 << bits;
                WriteBit((value & mask) >> bits);
                count--;
            }
        }

        public void WriteBitsLong(uint count, long value)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count > 0)
            {
                int bits = (int)count - 1;
                long mask = 0x1 << bits;
                WriteBit(((value & mask) >> bits) != 0);
                count--;
            }
        }

        public void WriteTrailingBit()
        {
            WriteBit(1);
        }

        public void Flush()
        {
            uint bytePos = 0;
            while ((bytePos = _bitsPosition % 8) != 0)
            {
                WriteBit(0);
            }
        }

        public void WriteUE(uint value)
        {
            uint bits = 0;
            int cumul = 0;
            for (int i = 0; i < 31; i++)
            {
                if (value < cumul + (1 << i))
                {
                    bits = (uint)i;
                    break;
                }
                cumul += (1 << i);
            }
            WriteBits(bits, 0);
            WriteBit(1);
            WriteBits(bits, (int)(value - cumul));
        }

        public void WriteSE(int value)
        {
            WriteUE((uint)((value << 1) * (value < 0 ? -1 : 1) + (value > 0 ? 1 : 0)));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_disposeStream)
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
    }

    public class Matrix
    {
        public static readonly Matrix ROTATE_0 =   new Matrix( 1,  0, 0,  0,  1, 0, 0, 0, 1);
        public static readonly Matrix ROTATE_90 =  new Matrix( 0,  1, 0, -1,  0, 0, 0, 0, 1);
        public static readonly Matrix ROTATE_180 = new Matrix(-1,  0, 0,  0, -1, 0, 0, 0, 1);
        public static readonly Matrix ROTATE_270 = new Matrix( 0, -1, 0,  1,  0, 0, 0, 0, 1);

        public double U { get; set; }
        public double V { get; set; }

        [DefaultValue(1d)]
        public double W { get; set; } = 1d;

        [DefaultValue(1d)]
        public double A { get; set; } = 1d;
        public double B { get; set; }
        public double C { get; set; }

        [DefaultValue(1d)]
        public double D { get; set; } = 1d;
        public double Tx { get; set; }
        public double Ty { get; set; }

        public Matrix(double a, double b, double u, double c, double d, double v, double tx, double ty, double w)
        {
            U = u;
            V = v;
            W = w;
            A = a;
            B = b;
            C = c;
            D = d;
            Tx = tx;
            Ty = ty;
        }

        public static uint Build(Stream stream, Matrix matrix)
        {
            uint size = 0;
            size += IsoReaderWriter.WriteFixedPoint1616(stream, matrix.A);
            size += IsoReaderWriter.WriteFixedPoint1616(stream, matrix.B);
            size += IsoReaderWriter.WriteFixedPoint0230(stream, matrix.U);
            size += IsoReaderWriter.WriteFixedPoint1616(stream, matrix.C);
            size += IsoReaderWriter.WriteFixedPoint1616(stream, matrix.D);
            size += IsoReaderWriter.WriteFixedPoint0230(stream, matrix.V);
            size += IsoReaderWriter.WriteFixedPoint1616(stream, matrix.Tx);
            size += IsoReaderWriter.WriteFixedPoint1616(stream, matrix.Ty);
            size += IsoReaderWriter.WriteFixedPoint0230(stream, matrix.W);
            return size;
        }

        public static Matrix Parse(Stream stream)
        {
            return new Matrix(
                IsoReaderWriter.ReadFixedPoint1616(stream),
                IsoReaderWriter.ReadFixedPoint1616(stream),
                IsoReaderWriter.ReadFixedPoint0230(stream),
                IsoReaderWriter.ReadFixedPoint1616(stream),
                IsoReaderWriter.ReadFixedPoint1616(stream),
                IsoReaderWriter.ReadFixedPoint0230(stream),
                IsoReaderWriter.ReadFixedPoint1616(stream),
                IsoReaderWriter.ReadFixedPoint1616(stream),
                IsoReaderWriter.ReadFixedPoint0230(stream)
                );
        }
    }

    public abstract class Mp4Box
    {
        public string Type { get; set; }

        private uint _originalSize;
        internal uint GetSize() { return _originalSize; }
        internal void SetSize(uint size) { _originalSize = size; }

        private Mp4Box _parent;
        public Mp4Box GetParent() { return _parent; }

        protected Mp4Box(uint size, string type, Mp4Box parent)
        {
            _originalSize = size;
            _parent = parent;
            Type = type;
        }

        public virtual uint CalculateSize()
        {
            return 8; // box header
        }
    }

    public abstract class ContainerMp4Box : Mp4Box
    {
        public List<Mp4Box> Children { get; set; } = new List<Mp4Box>();

        public ContainerMp4Box(uint size, string type, Mp4Box parent) : base(size, type, parent)
        { }

        public override uint CalculateSize()
        {
            uint size = 0;
            foreach (var child in Children)
            {
                size += child.CalculateSize();
            }
            return base.CalculateSize() + size;
        }
    }

    public class FtypBox : Mp4Box
    {
        public const string TYPE = "ftyp";

        public string MajorBrand { get; set; }
        public uint MinorVersion { get; set; }
        public List<string> CompatibleBrands { get; set; } = new List<string>();

        public FtypBox(uint size, Mp4Box parent, string majorBrand, uint minorVersion, List<string> compatibleBrands) : base(size, TYPE, parent)
        {
            MajorBrand = majorBrand;
            MinorVersion = minorVersion;
            CompatibleBrands = compatibleBrands;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            string majorBrand = IsoReaderWriter.Read4cc(stream);
            uint minorVersion = IsoReaderWriter.ReadUInt32(stream);
            uint compatibleBrandsCount = (size - 4 - 4 - 8) / 4;
            List<string> compatibleBrands = new List<string>();
            for (int i = 0; i < compatibleBrandsCount; i++)
            {
                compatibleBrands.Add(IsoReaderWriter.Read4cc(stream));
            }

            var box = new FtypBox(size, parent, majorBrand, minorVersion, compatibleBrands);
            return Task.FromResult((Mp4Box)box);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            uint size = 0;
            var b = (FtypBox)box;
            size += IsoReaderWriter.Write4cc(stream, b.MajorBrand);
            size += IsoReaderWriter.WriteUInt32(stream, b.MinorVersion);
            foreach (string compatibleBrand in b.CompatibleBrands)
            {
                size += IsoReaderWriter.Write4cc(stream, compatibleBrand);
            }
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + 8 + CompatibleBrands.Count * 4);
        }
    }

    public class MoovBox : ContainerMp4Box
    {
        public const string TYPE = "moov";
        public MoovBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public MvhdBox GetMvhd() { return Children.SingleOrDefault(x => x.Type == MvhdBox.TYPE) as MvhdBox; } 
        public MvexBox GetMvex() { return Children.SingleOrDefault(x => x.Type == MvexBox.TYPE) as MvexBox; } 
        public IEnumerable<TrakBox> GetTrak() { return Children.Where(x => x.Type == TrakBox.TYPE).Select(x => x as TrakBox); } 

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            ContainerMp4Box ret = new MoovBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }

            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MoovBox ret = (MoovBox)box;
            uint size = 0;
            foreach (var child in ret.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class MvhdBox : Mp4Box
    {
        public const string TYPE = "mvhd";

        public byte Version { get; set; }
        public uint Flags { get; set; }

        public DateTime CreationTime { get; set; } = IsoReaderWriter.DateTimeBase;
        public DateTime ModificationTime { get; set; } = IsoReaderWriter.DateTimeBase;
        public uint Timescale { get; set; }
        public ulong Duration { get; set; }
        public double Rate { get; set; } = 1.0;
        public float Volume { get; set; } = 1.0f;
        public Matrix Matrix { get; set; } = Matrix.ROTATE_0;
        public uint NextTrackId { get; set; } = 0xFFFFFFFF;
        public uint PreviewTime { get; set; }
        public uint PreviewDuration { get; set; }
        public uint PosterTime { get; set; }
        public uint SelectionTime { get; set; }
        public uint SelectionDuration { get; set; }
        public uint CurrentTime { get; set; }
        public ushort Dummy1 { get; set; } = 0;
        public uint Dummy2 { get; set; } = 0;
        public uint Dummy3 { get; set; } = 0;

        public MvhdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public MvhdBox(
            uint size,
            Mp4Box parent,
            byte version,
            uint flags,
            DateTime creationTime,
            DateTime modificationTime,
            uint timescale,
            ulong duration,
            double rate,
            float volume,
            ushort dummy1,
            uint dummy2,
            uint dummy3,
            Matrix matrix,
            uint previewTime,
            uint previewDuration,
            uint posterTime,
            uint selectionTime,
            uint selectionDuration,
            uint currentTime,
            uint nextTrackID) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            CreationTime = creationTime;
            ModificationTime = modificationTime;
            Timescale = timescale;
            Duration = duration;
            Rate = rate;
            Volume = volume;
            Dummy1 = dummy1;
            Dummy2 = dummy2;
            Dummy3 = dummy3;
            Matrix = matrix;
            PreviewTime = previewTime;
            PreviewDuration = previewDuration;
            PosterTime = posterTime;
            SelectionTime = selectionTime;
            SelectionDuration = selectionDuration;
            CurrentTime = currentTime;
            NextTrackId = nextTrackID;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            DateTime creationTime;
            DateTime modificationTime;
            uint timescale;
            ulong duration;

            if (version == 1)
            {
                creationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt64(stream));
                modificationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt64(stream));
                timescale = IsoReaderWriter.ReadUInt32(stream);
                duration = IsoReaderWriter.ReadUInt64(stream);
            }
            else
            {
                creationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt32(stream));
                modificationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt32(stream));
                timescale = IsoReaderWriter.ReadUInt32(stream);
                duration = IsoReaderWriter.ReadUInt32(stream);
            }

            double rate = IsoReaderWriter.ReadFixedPoint1616(stream);
            float volume = IsoReaderWriter.ReadFixedPoint88(stream);

            ushort dummy1 = IsoReaderWriter.ReadUInt16(stream);
            uint dummy2 = IsoReaderWriter.ReadUInt32(stream);
            uint dummy3 = IsoReaderWriter.ReadUInt32(stream);

            Matrix matrix = Matrix.Parse(stream);

            uint previewTime = IsoReaderWriter.ReadUInt32(stream);
            uint previewDuration = IsoReaderWriter.ReadUInt32(stream);
            uint posterTime = IsoReaderWriter.ReadUInt32(stream);
            uint selectionTime = IsoReaderWriter.ReadUInt32(stream);
            uint selectionDuration = IsoReaderWriter.ReadUInt32(stream);
            uint currentTime = IsoReaderWriter.ReadUInt32(stream);

            uint nextTrackID = IsoReaderWriter.ReadUInt32(stream);

            MvhdBox mvhd = new MvhdBox(
                size,
                parent,
                version,
                flags,
                creationTime,
                modificationTime,
                timescale,
                duration,
                rate,
                volume,
                dummy1,
                dummy2,
                dummy3,
                matrix,
                previewTime,
                previewDuration,
                posterTime,
                selectionTime,
                selectionDuration,
                currentTime,
                nextTrackID);

            return Task.FromResult((Mp4Box)mvhd);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MvhdBox b = (MvhdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            if (b.Version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, IsoReaderWriter.FromDate(b.CreationTime));
                size += IsoReaderWriter.WriteUInt64(stream, IsoReaderWriter.FromDate(b.ModificationTime));
                size += IsoReaderWriter.WriteUInt32(stream, b.Timescale);
                size += IsoReaderWriter.WriteUInt64(stream, b.Duration);
            }
            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)IsoReaderWriter.FromDate(b.CreationTime));
                size += IsoReaderWriter.WriteUInt32(stream, (uint)IsoReaderWriter.FromDate(b.ModificationTime));
                size += IsoReaderWriter.WriteUInt32(stream, b.Timescale);
                size += IsoReaderWriter.WriteUInt32(stream, (uint)b.Duration);
            }
            size += IsoReaderWriter.WriteFixedPoint1616(stream, b.Rate);
            size += IsoReaderWriter.WriteFixedPoint88(stream, b.Volume);
            size += IsoReaderWriter.WriteUInt16(stream, 0);
            size += IsoReaderWriter.WriteUInt32(stream, 0);
            size += IsoReaderWriter.WriteUInt32(stream, 0);

            size += Matrix.Build(stream, b.Matrix);

            size += IsoReaderWriter.WriteUInt32(stream, b.PreviewTime);
            size += IsoReaderWriter.WriteUInt32(stream, b.PreviewDuration);
            size += IsoReaderWriter.WriteUInt32(stream, b.PosterTime);
            size += IsoReaderWriter.WriteUInt32(stream, b.SelectionTime);
            size += IsoReaderWriter.WriteUInt32(stream, b.SelectionDuration);
            size += IsoReaderWriter.WriteUInt32(stream, b.CurrentTime);

            size += IsoReaderWriter.WriteUInt32(stream, b.NextTrackId);

            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            uint contentSize = 4;
            if (Version == 1)
            {
                contentSize += 28;
            }
            else
            {
                contentSize += 16;
            }
            contentSize += 80;
            return base.CalculateSize() + contentSize;
        }
    }

    public class TrakBox : ContainerMp4Box
    {
        public const string TYPE = "trak";
        public TrakBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public TkhdBox GetTkhd() { return Children.SingleOrDefault(x => x.Type == TkhdBox.TYPE) as TkhdBox; } 
        public MdiaBox GetMdia() { return Children.SingleOrDefault(x => x.Type == MdiaBox.TYPE) as MdiaBox; } 

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            ContainerMp4Box ret = new TrakBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }
            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            TrakBox ret = (TrakBox)box;
            uint size = 0;
            foreach (var child in ret.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class TkhdBox : Mp4Box
    {
        public const string TYPE = "tkhd";

        public byte Version { get; set; }
        public uint Flags { get; set; } = 7; //  The default value of the track header flags for media tracks is 7 (track_enabled, track_in_movie, track_in_preview).

        public DateTime CreationTime { get; set; } = IsoReaderWriter.DateTimeBase;
        public DateTime ModificationTime { get; set; } = IsoReaderWriter.DateTimeBase;
        public uint TrackId { get; set; }
        public ulong Duration { get; set; }
        public ushort Layer { get; set; }
        public ushort AlternateGroup { get; set; }
        public float Volume { get; set; }
        public Matrix Matrix { get; set; } = Matrix.ROTATE_0;
        public double Width { get; set; }
        public double Height { get; set; }
        public uint Dummy1 { get; set; } = 0;
        public uint Dummy2 { get; set; } = 0;
        public uint Dummy3 { get; set; } = 0;
        public ushort Dummy4 { get; set; } = 0;

        public TkhdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public TkhdBox(
            uint size,
            Mp4Box parent,
            byte version,
            uint flags,
            DateTime creationTime,
            DateTime modificationTime,
            uint trackId,
            uint dummy1,
            ulong duration,
            uint dummy2,
            uint dummy3,
            ushort layer,
            ushort alternateGroup,
            float volume,
            ushort dummy4,
            Matrix matrix,
            double width,
            double height) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            CreationTime = creationTime;
            ModificationTime = modificationTime;
            TrackId = trackId;
            Dummy1 = dummy1;
            Duration = duration;
            Dummy2 = dummy2;
            Dummy3 = dummy3;
            Layer = layer;
            AlternateGroup = alternateGroup;
            Volume = volume;
            Dummy4 = dummy4;
            Matrix = matrix;
            Width = width;
            Height = height;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            DateTime creationTime;
            DateTime modificationTime;
            uint trackId;
            uint dummy1;
            ulong duration;
            if (version == 1)
            {
                creationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt64(stream));
                modificationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt64(stream));
                trackId = IsoReaderWriter.ReadUInt32(stream);
                dummy1 = IsoReaderWriter.ReadUInt32(stream);
                duration = IsoReaderWriter.ReadUInt64(stream);
            }
            else
            {
                creationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt32(stream));
                modificationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt32(stream));
                trackId = IsoReaderWriter.ReadUInt32(stream);
                dummy1 = IsoReaderWriter.ReadUInt32(stream);
                duration = IsoReaderWriter.ReadUInt32(stream);
            }

            uint dummy2 = IsoReaderWriter.ReadUInt32(stream);
            uint dummy3 = IsoReaderWriter.ReadUInt32(stream);

            ushort layer = IsoReaderWriter.ReadUInt16(stream);
            ushort alternateGroup = IsoReaderWriter.ReadUInt16(stream);
            float volume = IsoReaderWriter.ReadFixedPoint88(stream);
            ushort dummy4 = IsoReaderWriter.ReadUInt16(stream);
            Matrix matrix = Matrix.Parse(stream);
            double width = IsoReaderWriter.ReadFixedPoint1616(stream);
            double height = IsoReaderWriter.ReadFixedPoint1616(stream);

            TkhdBox ret = new TkhdBox(
                size,
                parent,
                version,
                flags,
                creationTime,
                modificationTime,
                trackId,
                dummy1,
                duration,
                dummy2,
                dummy3,
                layer,
                alternateGroup,
                volume,
                dummy4,
                matrix,
                width,
                height);

            return Task.FromResult((Mp4Box)ret);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            TkhdBox b = (TkhdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);

            if (b.Version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, IsoReaderWriter.FromDate(b.CreationTime));
                size += IsoReaderWriter.WriteUInt64(stream, IsoReaderWriter.FromDate(b.ModificationTime));
                size += IsoReaderWriter.WriteUInt32(stream, b.TrackId);
                size += IsoReaderWriter.WriteUInt32(stream, 0);
                size += IsoReaderWriter.WriteUInt64(stream, b.Duration);
            }
            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)IsoReaderWriter.FromDate(b.CreationTime));
                size += IsoReaderWriter.WriteUInt32(stream, (uint)IsoReaderWriter.FromDate(b.ModificationTime));
                size += IsoReaderWriter.WriteUInt32(stream, b.TrackId);
                size += IsoReaderWriter.WriteUInt32(stream, 0);
                size += IsoReaderWriter.WriteUInt32(stream, (uint)b.Duration);
            }
            size += IsoReaderWriter.WriteUInt32(stream, 0);
            size += IsoReaderWriter.WriteUInt32(stream, 0);
            size += IsoReaderWriter.WriteUInt16(stream, b.Layer);
            size += IsoReaderWriter.WriteUInt16(stream, b.AlternateGroup);
            size += IsoReaderWriter.WriteFixedPoint88(stream, b.Volume);
            size += IsoReaderWriter.WriteUInt16(stream, 0);

            size += Matrix.Build(stream, b.Matrix);

            size += IsoReaderWriter.WriteFixedPoint1616(stream, b.Width);
            size += IsoReaderWriter.WriteFixedPoint1616(stream, b.Height);

            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            uint contentSize = 4;
            if (Version == 1)
            {
                contentSize += 32;
            }
            else
            {
                contentSize += 20;
            }
            contentSize += 60;
            return base.CalculateSize() + contentSize;
        }
    }

    public class MdiaBox : ContainerMp4Box
    {
        public const string TYPE = "mdia";

        public MdhdBox GetMdhd() { return Children.SingleOrDefault(x => x.Type == MdhdBox.TYPE) as MdhdBox; } 
        public HdlrBox GetHdlr() { return Children.SingleOrDefault(x => x.Type == HdlrBox.TYPE) as HdlrBox; } 
        public MinfBox GetMinf() { return Children.SingleOrDefault(x => x.Type == MinfBox.TYPE) as MinfBox; } 

        public MdiaBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            ContainerMp4Box ret = new MdiaBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }
            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MdiaBox ret = (MdiaBox)box;
            uint size = 0;
            foreach (var child in ret.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class MdhdBox : Mp4Box
    {
        public const string TYPE = "mdhd";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public DateTime CreationTime { get; set; } = IsoReaderWriter.DateTimeBase;
        public DateTime ModificationTime { get; set; } = IsoReaderWriter.DateTimeBase;
        public uint Timescale { get; set; }
        public ulong Duration { get; set; }
        public string Language { get; set; } = "und";
        public ushort Dummy1 { get; set; } = 0;

        public MdhdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public MdhdBox(
            uint size,
            Mp4Box parent,
            byte version,
            uint flags,
            DateTime creationTime,
            DateTime modificationTime,
            uint timescale,
            ulong duration,
            string language,
            ushort dummy1) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            CreationTime = creationTime;
            ModificationTime = modificationTime;
            Timescale = timescale;
            Duration = duration;
            Language = language;
            Dummy1 = dummy1;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            DateTime creationTime;
            DateTime modificationTime;
            uint timescale;
            ulong duration;

            if (version == 1)
            {
                creationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt64(stream));
                modificationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt64(stream));
                timescale = IsoReaderWriter.ReadUInt32(stream);
                duration = IsoReaderWriter.ReadUInt64(stream);
            }
            else
            {
                creationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt32(stream));
                modificationTime = IsoReaderWriter.ToDate(IsoReaderWriter.ReadUInt32(stream));
                timescale = IsoReaderWriter.ReadUInt32(stream);
                duration = IsoReaderWriter.ReadUInt32(stream);
            }

            string language = IsoReaderWriter.ReadIso639(stream);
            ushort dummy1 = IsoReaderWriter.ReadUInt16(stream);

            MdhdBox mdhd = new MdhdBox(
                size,
                parent,
                version,
                flags,
                creationTime,
                modificationTime,
                timescale,
                duration,
                language,
                dummy1);

            return Task.FromResult((Mp4Box)mdhd);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MdhdBox b = (MdhdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            if (b.Version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, IsoReaderWriter.FromDate(b.CreationTime));
                size += IsoReaderWriter.WriteUInt64(stream, IsoReaderWriter.FromDate(b.ModificationTime));
                size += IsoReaderWriter.WriteUInt32(stream, b.Timescale);
                size += IsoReaderWriter.WriteUInt64(stream, b.Duration);
            }
            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)IsoReaderWriter.FromDate(b.CreationTime));
                size += IsoReaderWriter.WriteUInt32(stream, (uint)IsoReaderWriter.FromDate(b.ModificationTime));
                size += IsoReaderWriter.WriteUInt32(stream, b.Timescale);
                size += IsoReaderWriter.WriteUInt32(stream, (uint)b.Duration);
            }
            size += IsoReaderWriter.WriteIso639(stream, b.Language);
            size += IsoReaderWriter.WriteUInt16(stream, 0);

            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            uint contentSize = 4;
            if (Version == 1)
            {
                contentSize += 8 + 8 + 4 + 8;
            }
            else
            {
                contentSize += 4 + 4 + 4 + 4;
            }
            contentSize += 2;
            contentSize += 2;
            return base.CalculateSize() + contentSize;
        }
    }

    public class HdlrBox : Mp4Box
    {
        public const string TYPE = "hdlr";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public uint Dummy1 { get; set; } = 0;
        public string HandlerType { get; set; }
        public uint A { get; set; }
        public uint B { get; set; }
        public uint C { get; set; }
        public string Name { get; set; }

        public HdlrBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public HdlrBox(uint size, Mp4Box parent, byte version, uint flags, uint dummy1, string handlerType, uint a, uint b, uint c, string name) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            Dummy1 = dummy1;
            HandlerType = handlerType;
            A = a;
            B = b;
            C = c;
            Name = name;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint dummy1 = IsoReaderWriter.ReadUInt32(stream);
            string handlerType = IsoReaderWriter.Read4cc(stream);

            uint a = IsoReaderWriter.ReadUInt32(stream);
            uint b = IsoReaderWriter.ReadUInt32(stream);
            uint c = IsoReaderWriter.ReadUInt32(stream);

            uint remainingSize = size - 8 - 1 - 3 - 4 - 4 - 4 - 4 - 4;
            string name = "";
            if (remainingSize > 0)
            {
                name = await IsoReaderWriter.ReadStringAsync(stream, remainingSize);
            }

            HdlrBox hdlr = new HdlrBox(
                size,
                parent,
                version,
                flags,
                dummy1,
                handlerType,
                a,
                b,
                c,
                name);

            return hdlr;
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            HdlrBox b = (HdlrBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt32(stream, b.Dummy1); // might not be 0
            size += IsoReaderWriter.Write4cc(stream, b.HandlerType);
            size += IsoReaderWriter.WriteUInt32(stream, b.A);
            size += IsoReaderWriter.WriteUInt32(stream, b.B);
            size += IsoReaderWriter.WriteUInt32(stream, b.C);
            if (b.Name != null)
            {
                size += IsoReaderWriter.WriteString(stream, b.Name);
            }
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + 24 + Encoding.UTF8.GetBytes(Name).Length);
        }
    }

    public class MinfBox : ContainerMp4Box
    {
        public const string TYPE = "minf";

        public VmhdBox GetVmhd() { return Children.SingleOrDefault(x => x.Type == VmhdBox.TYPE) as VmhdBox; } 
        public SmhdBox GetSmhd() { return Children.SingleOrDefault(x => x.Type == SmhdBox.TYPE) as SmhdBox; } 
        public DinfBox GetDinf() { return Children.SingleOrDefault(x => x.Type == DinfBox.TYPE) as DinfBox; } 
        public StblBox GetStbl() { return Children.SingleOrDefault(x => x.Type == StblBox.TYPE) as StblBox; }

        public MinfBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            ContainerMp4Box ret = new MinfBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }
            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MinfBox ret = (MinfBox)box;
            uint size = 0;
            foreach (var child in ret.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class SmhdBox : Mp4Box
    {
        public const string TYPE = "smhd";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public float Balance { get; set; }
        public ushort Dummy1 { get; set; } = 0;

        public SmhdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public SmhdBox(uint size, Mp4Box parent, byte version, uint flags, float balance, ushort dummy1) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            Balance = balance;
            Dummy1 = dummy1;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            float balance = IsoReaderWriter.ReadFixedPoint88(stream);
            ushort dummy1 = IsoReaderWriter.ReadUInt16(stream);

            SmhdBox smhd = new SmhdBox(
                size,
                parent,
                version,
                flags,
                balance,
                dummy1);

            return Task.FromResult((Mp4Box)smhd);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            SmhdBox b = (SmhdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteFixedPoint88(stream, b.Balance);
            size += IsoReaderWriter.WriteUInt16(stream, 0);
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + 8;
        }
    }

    public class DinfBox : ContainerMp4Box
    {
        public const string TYPE = "dinf";
        public DrefBox GetDref() { return Children.SingleOrDefault(x => x.Type == DrefBox.TYPE) as DrefBox; } 

        public DinfBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            ContainerMp4Box ret = new DinfBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }
            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            DinfBox b = (DinfBox)box;
            uint size = 0;
            foreach (var child in b.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class DrefBox : ContainerMp4Box
    {
        public const string TYPE = "dref";

        public byte Version { get; set; }
        public uint Flags { get; set; }

        public DrefBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public DrefBox(uint size, Mp4Box parent, byte version, uint flags) : this(size, parent)
        {
            Version = version;
            Flags = flags;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint numOfChildBoxes = IsoReaderWriter.ReadUInt32(stream);

            DrefBox dref = new DrefBox(
                size,
                parent,
                version,
                flags);

            for (int i = 0; i < numOfChildBoxes; i++)
            {
                var box = await Mp4Parser.ReadBox(dref, stream);
                dref.Children.Add(box);
            }

            return dref;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            DrefBox b = (DrefBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);

            size += IsoReaderWriter.WriteUInt32(stream, (uint)b.Children.Count);

            foreach (var child in b.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }

            return size;
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + 8;
        }
    }

    public class VmhdBox : Mp4Box
    {
        public const string TYPE = "vmhd";

        public byte Version { get; set; }
        public uint Flags { get; set; } = 1;
        public ushort GraphicsMode { get; set; } = 0;
        public ushort OpColor0 { get; set; } = 0;
        public ushort OpColor1 { get; set; } = 0;
        public ushort OpColor2 { get; set; } = 0;

        public VmhdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public VmhdBox(uint size, Mp4Box parent, byte version, uint flags, ushort graphicsMode, ushort opColor0, ushort opColor1, ushort opColor2) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            GraphicsMode = graphicsMode;
            OpColor0 = opColor0;
            OpColor1 = opColor1;
            OpColor2 = opColor2;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            ushort graphicsMode = IsoReaderWriter.ReadUInt16(stream);
            ushort opColor0 = IsoReaderWriter.ReadUInt16(stream);
            ushort opColor1 = IsoReaderWriter.ReadUInt16(stream);
            ushort opColor2 = IsoReaderWriter.ReadUInt16(stream);

            VmhdBox vmhd = new VmhdBox(
                size,
                parent,
                version,
                flags,
                graphicsMode,
                opColor0,
                opColor1,
                opColor2);

            return Task.FromResult((Mp4Box)vmhd);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            VmhdBox b = (VmhdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt16(stream, b.GraphicsMode);
            size += IsoReaderWriter.WriteUInt16(stream, b.OpColor0);
            size += IsoReaderWriter.WriteUInt16(stream, b.OpColor1);
            size += IsoReaderWriter.WriteUInt16(stream, b.OpColor2);
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + 12;
        }
    }

    public class StblBox : ContainerMp4Box
    {
        public const string TYPE = "stbl";

        public StsdBox GetStsd() { return Children.SingleOrDefault(x => x.Type == StsdBox.TYPE) as StsdBox; } 
        public StszBox GetStsz() { return Children.SingleOrDefault(x => x.Type == StszBox.TYPE) as StszBox; }
        public StscBox GetStsc() { return Children.SingleOrDefault(x => x.Type == StscBox.TYPE) as StscBox; } 
        public SttsBox GetStts() { return Children.SingleOrDefault(x => x.Type == SttsBox.TYPE) as SttsBox; } 
        public StcoBox GetStco() { return Children.SingleOrDefault(x => x.Type == StcoBox.TYPE) as StcoBox; } 

        public StblBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            ContainerMp4Box ret = new StblBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }
            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            StblBox b = (StblBox)box;
            uint size = 0;
            foreach (var child in b.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class StsdBox : ContainerMp4Box
    {
        public const string TYPE = "stsd";

        public byte Version { get; set; }
        public uint Flags { get; set; }

        public StsdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public StsdBox(uint size, Mp4Box parent, byte version, uint flags) : this(size, parent)
        {
            Version = version;
            Flags = flags;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint numOfChildBoxes = IsoReaderWriter.ReadUInt32(stream);

            StsdBox stsd = new StsdBox(
                size,
                parent,
                version,
                flags);

            for (int i = 0; i < numOfChildBoxes; i++)
            {
                var box = await Mp4Parser.ReadBox(stsd, stream);
                stsd.Children.Add(box);
            }

            return stsd;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            StsdBox b = (StsdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);

            size += IsoReaderWriter.WriteUInt32(stream, (uint)b.Children.Count);

            foreach (var child in b.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }

        public override uint CalculateSize()
        {
            return 8 + base.CalculateSize();
        }
    }

    public class StszBox : Mp4Box
    {
        public const string TYPE = "stsz";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public uint SampleSize { get; set; }
        public uint SampleCount { get; set; }
        public uint[] SampleSizes { get; set; } = new uint[0];

        public StszBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public StszBox(uint size, Mp4Box parent, byte version, uint flags, uint sampleSize, uint sampleCount, uint[] sampleSizes) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            SampleSize = sampleSize;
            SampleCount = sampleCount;
            SampleSizes = sampleSizes;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint sampleSize = IsoReaderWriter.ReadUInt32(stream);
            uint sampleCount = IsoReaderWriter.ReadUInt32(stream);
            uint[] sampleSizes = null;

            if (sampleSize == 0)
            {
                sampleSizes = new uint[sampleCount];
                for (int i = 0; i < sampleCount; i++)
                {
                    sampleSizes[i] = IsoReaderWriter.ReadUInt32(stream);
                }
            }

            StszBox stsz = new StszBox(
                size,
                parent,
                version,
                flags,
                sampleSize,
                sampleCount,
                sampleSizes);

            return Task.FromResult((Mp4Box)stsz);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            StszBox b = (StszBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt32(stream, b.SampleSize);

            if (b.SampleSize == 0)
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)b.SampleSizes.Length);
                foreach (long sampleSize1 in b.SampleSizes)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, b.SampleSize);
                }
            }
            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, b.SampleCount);
            }

            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + 12 + (SampleSize == 0 ? SampleSizes.Length * 4 : 0));
        }
    }

    public class StscBox : Mp4Box
    {
        public const string TYPE = "stsc";

        public class Entry
        {
            public uint FirstChunk { get; set; }
            public uint SamplesPerChunk { get; set; }
            public uint SampleDescriptionIndex { get; set; }

            public Entry()
            { }

            public Entry(uint firstChunk, uint samplesPerChunk, uint sampleDescriptionIndex)
            {
                FirstChunk = firstChunk;
                SamplesPerChunk = samplesPerChunk;
                SampleDescriptionIndex = sampleDescriptionIndex;
            }
        }

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();

        public StscBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public StscBox(uint size, Mp4Box parent, byte version, uint flags, List<Entry> entries) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            Entries = entries;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint entryCount = IsoReaderWriter.ReadUInt32(stream);
            List<Entry> entries = new List<Entry>();
            for (int i = 0; i < entryCount; i++)
            {
                var entry = new Entry(
                    IsoReaderWriter.ReadUInt32(stream),
                    IsoReaderWriter.ReadUInt32(stream),
                    IsoReaderWriter.ReadUInt32(stream)
                );
                entries.Add(entry);
            }

            StscBox stsc = new StscBox(
                size,
                parent,
                version,
                flags,
                entries);

            return Task.FromResult((Mp4Box)stsc);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            StscBox b = (StscBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt32(stream, (uint)b.Entries.Count);
            foreach (Entry entry in b.Entries)
            {
                size += IsoReaderWriter.WriteUInt32(stream, entry.FirstChunk);
                size += IsoReaderWriter.WriteUInt32(stream, entry.SamplesPerChunk);
                size += IsoReaderWriter.WriteUInt32(stream, entry.SampleDescriptionIndex);
            }
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + Entries.Count * 12 + 8);
        }
    }

    /// <summary>
    /// STTS box allows indexing from decoding time to sample number.
    /// </summary>
    public class SttsBox : Mp4Box
    {
        public const string TYPE = "stts";

        public class Entry
        {
            public uint Count { get; set; }
            public uint Delta { get; set; }

            public Entry()
            { }

            public Entry(uint count, uint delta)
            {
                Count = count;
                Delta = delta;
            }
        }

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();

        public SttsBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public SttsBox(uint size, Mp4Box parent, byte version, uint flags, List<Entry> entries) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            Entries = entries;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint entryCount = IsoReaderWriter.ReadUInt32(stream);
            List<Entry> entries = new List<Entry>();
            for (int i = 0; i < entryCount; i++)
            {
                var entry = new Entry(
                    IsoReaderWriter.ReadUInt32(stream),
                    IsoReaderWriter.ReadUInt32(stream)
                );
                entries.Add(entry);
            }

            SttsBox stts = new SttsBox(
                size,
                parent,
                version,
                flags,
                entries);

            return Task.FromResult((Mp4Box)stts);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            SttsBox b = (SttsBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt32(stream, (uint)b.Entries.Count);
            foreach (Entry entry in b.Entries)
            {
                size += IsoReaderWriter.WriteUInt32(stream, entry.Count);
                size += IsoReaderWriter.WriteUInt32(stream, entry.Delta);
            }
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + 8 + Entries.Count * 8);
        }
    }

    public class StcoBox : Mp4Box
    {
        public const string TYPE = "stco";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public uint[] ChunkOffsets { get; set; } = new uint[0];

        public StcoBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public StcoBox(uint size, Mp4Box parent, byte version, uint flags, uint[] chunkOffsets) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            ChunkOffsets = chunkOffsets;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint entryCount = IsoReaderWriter.ReadUInt32(stream);
            uint[] chunkOffsets = new uint[entryCount];
            for (int i = 0; i < entryCount; i++)
            {
                chunkOffsets[i] = IsoReaderWriter.ReadUInt32(stream);
            }

            StcoBox stco = new StcoBox(
                size,
                parent,
                version,
                flags,
                chunkOffsets);

            return Task.FromResult((Mp4Box)stco);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            StcoBox b = (StcoBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt32(stream, (uint)b.ChunkOffsets.Length);
            foreach (uint chunkOffset in b.ChunkOffsets)
            {
                size += IsoReaderWriter.WriteUInt32(stream, chunkOffset);
            }
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + 8 + ChunkOffsets.Length * 4);
        }
    }

    public class MvexBox : ContainerMp4Box
    {
        public const string TYPE = "mvex";

        public MehdBox GetMehd() { return Children.SingleOrDefault(x => x.Type == MehdBox.TYPE) as MehdBox; } 
        public IEnumerable<TrexBox> GetTrex() { return Children.Where(x => x.Type == TrexBox.TYPE).Select(x => x as TrexBox); } 

        public MvexBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            MvexBox ret = new MvexBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }
            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MvexBox b = (MvexBox)box;
            uint size = 0;
            foreach (var child in b.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class MehdBox : Mp4Box
    {
        public const string TYPE = "mehd";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public ulong FragmentDuration { get; set; }

        public MehdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public MehdBox(uint size, Mp4Box parent, byte version, uint flags, ulong fragmentDuration) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            FragmentDuration = fragmentDuration;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            ulong fragmentDuration;
            if (version == 1)
            {
                fragmentDuration = IsoReaderWriter.ReadUInt64(stream);
            }
            else
            {
                fragmentDuration = IsoReaderWriter.ReadUInt32(stream);
            }

            MehdBox mehd = new MehdBox(
                size,
                parent,
                version,
                flags,
                fragmentDuration);

            return Task.FromResult((Mp4Box)mehd);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MehdBox b = (MehdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            if (b.Version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, b.FragmentDuration);
            }
            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)b.FragmentDuration);
            }
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + (Version == 1 ? 12 : 8));
        }
    }

    public class TrexBox : Mp4Box
    {
        public const string TYPE = "trex";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public uint TrackId { get; set; }
        public uint DefaultSampleDescriptionIndex { get; set; } = 1;
        public uint DefaultSampleDuration { get; set; }
        public uint DefaultSampleSize { get; set; }
        public uint A { get; set; }

        public TrexBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public TrexBox(
            uint size,
            Mp4Box parent,
            byte version,
            uint flags,
            uint trackId,
            uint defaultSampleDescriptionIndex,
            uint defaultSampleDuration,
            uint defaultSampleSize,
            uint a) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            TrackId = trackId;
            DefaultSampleDescriptionIndex = defaultSampleDescriptionIndex;
            DefaultSampleDuration = defaultSampleDuration;
            DefaultSampleSize = defaultSampleSize;
            A = a;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint trackId = IsoReaderWriter.ReadUInt32(stream);
            uint defaultSampleDescriptionIndex = IsoReaderWriter.ReadUInt32(stream);
            uint defaultSampleDuration = IsoReaderWriter.ReadUInt32(stream);
            uint defaultSampleSize = IsoReaderWriter.ReadUInt32(stream);
            uint a = IsoReaderWriter.ReadUInt32(stream);
            // TODO
            /*
            reserved = (byte)((a & 0xF0000000) >> 28);
            isLeading = (byte)((a & 0x0C000000) >> 26);
            sampleDependsOn = (byte)((a & 0x03000000) >> 24);
            sampleIsDependedOn = (byte)((a & 0x00C00000) >> 22);
            sampleHasRedundancy = (byte)((a & 0x00300000) >> 20);
            samplePaddingValue = (byte)((a & 0x000e0000) >> 17);
            sampleIsDifferenceSample = (a & 0x00010000) >> 16 > 0;
            sampleDegradationPriority = (int)(a & 0x0000ffff);
             */

            TrexBox trex = new TrexBox(
                size,
                parent,
                version,
                flags,
                trackId,
                defaultSampleDescriptionIndex,
                defaultSampleDuration,
                defaultSampleSize,
                a);

            return Task.FromResult((Mp4Box)trex);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            TrexBox b = (TrexBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt32(stream, b.TrackId);
            size += IsoReaderWriter.WriteUInt32(stream, b.DefaultSampleDescriptionIndex);
            size += IsoReaderWriter.WriteUInt32(stream, b.DefaultSampleDuration);
            size += IsoReaderWriter.WriteUInt32(stream, b.DefaultSampleSize);
            size += IsoReaderWriter.WriteUInt32(stream, b.A);
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + 5 * 4 + 4;
        }
    }

    public class MoofBox : ContainerMp4Box
    {
        public const string TYPE = "moof";

        public MfhdBox GetMfhd() { return Children.SingleOrDefault(x => x.Type == MfhdBox.TYPE) as MfhdBox; } 
        public IEnumerable<TrafBox> GetTraf() { return Children.Where(x => x.Type == TrafBox.TYPE).Select(x => x as TrafBox); } 

        public MoofBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            ContainerMp4Box ret = new MoofBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }
            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MoofBox ret = (MoofBox)box;
            uint size = 0;
            foreach (var child in ret.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class MfhdBox : Mp4Box
    {
        public const string TYPE = "mfhd";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public uint SequenceNumber { get; set; }

        public MfhdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public MfhdBox(uint size, Mp4Box parent, byte version, uint flags, uint sequenceNumber) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            SequenceNumber = sequenceNumber;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint sequenceNumber = IsoReaderWriter.ReadUInt32(stream);
            MfhdBox mfhd = new MfhdBox(
                size,
                parent,
                version,
                flags,
                sequenceNumber);

            return Task.FromResult((Mp4Box)mfhd);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MfhdBox b = (MfhdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt32(stream, b.SequenceNumber);
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + 8;
        }
    }

    public class TrafBox : ContainerMp4Box
    {
        public const string TYPE = "traf";

        public TfhdBox GetTfhd() { return Children.SingleOrDefault(x => x.Type == TfhdBox.TYPE) as TfhdBox; } 
        public TfdtBox GetTfdt() { return Children.SingleOrDefault(x => x.Type == TfdtBox.TYPE) as TfdtBox; } 
        public IEnumerable<TrunBox> GetTrun() { return Children.Where(x => x.Type == TrunBox.TYPE).Select(x => x as TrunBox); } 

        public TrafBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            ContainerMp4Box ret = new TrafBox(size, parent);
            uint parsedSize = 8;
            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(ret, stream);
                parsedSize += box.GetSize();
                ret.Children.Add(box);
            }
            return ret;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            TrafBox ret = (TrafBox)box;
            uint size = 0;
            foreach (var child in ret.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }
            return size;
        }
    }

    public class TfhdBox : Mp4Box
    {
        public const string TYPE = "tfhd";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public uint TrackId { get; set; }
        public ulong BaseDataOffset { get; set; }
        public uint SampleDescriptionIndex { get; set; }
        public uint DefaultSampleDuration { get; set; }
        public uint DefaultSampleSize { get; set; }
        public uint DefaultSampleFlags { get; set; } = 16842752; 
        public bool DurationIsEmpty { get; set; }
        public bool DefaultBaseIsMoof { get; set; } = true;

        public TfhdBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public TfhdBox(
            uint size,
            Mp4Box parent,
            byte version,
            uint flags,
            uint trackId,
            ulong baseDataOffset,
            uint sampleDescriptionIndex,
            uint defaultSampleDuration,
            uint defaultSampleSize,
            uint defaultSampleFlags,
            bool durationIsEmpty,
            bool defaultBaseIsMoof) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            TrackId = trackId;
            BaseDataOffset = baseDataOffset;
            SampleDescriptionIndex = sampleDescriptionIndex;
            DefaultSampleDuration = defaultSampleDuration;
            DefaultSampleSize = defaultSampleSize;
            DefaultSampleFlags = defaultSampleFlags;
            DurationIsEmpty = durationIsEmpty;
            DefaultBaseIsMoof = defaultBaseIsMoof;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint trackId = IsoReaderWriter.ReadUInt32(stream);

            ulong baseDataOffset = 0;
            if ((flags & 0x1) == 0x1)
            {
                baseDataOffset = IsoReaderWriter.ReadUInt64(stream);
            }

            uint sampleDescriptionIndex = 0;
            if ((flags & 0x2) == 0x2)
            {
                sampleDescriptionIndex = IsoReaderWriter.ReadUInt32(stream);
            }

            uint defaultSampleDuration = 0;
            if ((flags & 0x8) == 0x8)
            {
                defaultSampleDuration = IsoReaderWriter.ReadUInt32(stream);
            }

            uint defaultSampleSize = 0;
            if ((flags & 0x10) == 0x10)
            {
                defaultSampleSize = IsoReaderWriter.ReadUInt32(stream);
            }

            uint defaultSampleFlags = 0;
            if ((flags & 0x20) == 0x20)
            {
                defaultSampleFlags = IsoReaderWriter.ReadUInt32(stream);
            }

            bool durationIsEmpty = false;
            if ((flags & 0x10000) == 0x10000)
            {
                durationIsEmpty = true;
            }

            bool defaultBaseIsMoof = false;
            if ((flags & 0x20000) == 0x20000)
            {
                defaultBaseIsMoof = true;
            }

            TfhdBox tfhd = new TfhdBox(
                size,
                parent,
                version,
                flags,
                trackId,
                baseDataOffset,
                sampleDescriptionIndex,
                defaultSampleDuration,
                defaultSampleSize,
                defaultSampleFlags,
                durationIsEmpty,
                defaultBaseIsMoof);

            return Task.FromResult((Mp4Box)tfhd);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            TfhdBox b = (TfhdBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            size += IsoReaderWriter.WriteUInt32(stream, b.TrackId);

            if ((b.Flags & 0x1) == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, b.BaseDataOffset);
            }
            if ((b.Flags & 0x2) == 0x2)
            {
                size += IsoReaderWriter.WriteUInt32(stream, b.SampleDescriptionIndex);
            }
            if ((b.Flags & 0x8) == 0x8)
            {
                size += IsoReaderWriter.WriteUInt32(stream, b.DefaultSampleDuration);
            }
            if ((b.Flags & 0x10) == 0x10)
            {
                size += IsoReaderWriter.WriteUInt32(stream, b.DefaultSampleSize);
            }
            if ((b.Flags & 0x20) == 0x20)
            {
                size += IsoReaderWriter.WriteUInt32(stream, b.DefaultSampleFlags);
            }
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            uint size = 8;
            if ((Flags & 0x1) == 1)
            {
                size += 8;
            }
            if ((Flags & 0x2) == 0x2)
            {
                size += 4;
            }
            if ((Flags & 0x8) == 0x8)
            {
                size += 4;
            }
            if ((Flags & 0x10) == 0x10)
            {
                size += 4;
            }
            if ((Flags & 0x20) == 0x20)
            {
                size += 4;
            }
            return base.CalculateSize() + size;
        }
    }

    public class TfdtBox : Mp4Box
    {
        public const string TYPE = "tfdt";

        public byte Version { get; set; } = 1;
        public uint Flags { get; set; }
        public ulong BaseMediaDecodeTime { get; set; }

        public TfdtBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public TfdtBox(uint size, Mp4Box parent, byte version, uint flags, ulong baseMediaDecodeTime) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            BaseMediaDecodeTime = baseMediaDecodeTime;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            ulong baseMediaDecodeTime;
            if (version == 1)
            {
                baseMediaDecodeTime = IsoReaderWriter.ReadUInt64(stream);
            }
            else
            {
                baseMediaDecodeTime = IsoReaderWriter.ReadUInt32(stream);
            }

            TfdtBox tfdt = new TfdtBox(
                size,
                parent,
                version,
                flags,
                baseMediaDecodeTime);

            return Task.FromResult((Mp4Box)tfdt);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            TfdtBox b = (TfdtBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);

            if (b.Version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, b.BaseMediaDecodeTime);
            }
            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)b.BaseMediaDecodeTime);
            }

            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + (Version == 0 ? 8 : 12));
        }
    }

    public class TrunBox : Mp4Box
    {
        public const string TYPE = "trun";

        public byte Version { get; set; }
        public uint Flags { get; set; }
        public int DataOffset { get; set; }
        public uint FirstSampleFlags { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();

        public sealed class Entry
        {
            public uint SampleDuration { get; set; }
            public uint SampleSize { get; set; }
            public uint SampleFlags { get; set; }
            public int SampleCompositionTimeOffset { get; set; }

            public Entry(uint sampleDuration, uint sampleSize, uint sampleFlags, int sampleCompositionTimeOffset)
            {
                SampleDuration = sampleDuration;
                SampleSize = sampleSize;
                SampleFlags = sampleFlags;
                SampleCompositionTimeOffset = sampleCompositionTimeOffset;
            }

            public Entry()
            { }
        }

        public TrunBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public TrunBox(uint size, Mp4Box parent, byte version, uint flags, int dataOffset, uint firstSampleFlags, List<Entry> entries) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            DataOffset = dataOffset;
            FirstSampleFlags = firstSampleFlags;
            Entries = entries;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint sampleCount = IsoReaderWriter.ReadUInt32(stream);
            int dataOffset;
            if ((flags & 0x1) == 0x1)
            {
                dataOffset = IsoReaderWriter.ReadInt32(stream);
            }
            else
            {
                dataOffset = -1;
            }

            uint firstSampleFlags = 0;
            if ((flags & 0x4) == 0x4)
            {
                firstSampleFlags = IsoReaderWriter.ReadUInt32(stream);
            }

            List<Entry> entries = new List<Entry>();
            for (int i = 0; i < sampleCount; i++)
            {
                Entry entry = new Entry();
                if ((flags & 0x100) == 0x100)
                {
                    entry.SampleDuration = IsoReaderWriter.ReadUInt32(stream);
                }

                if ((flags & 0x200) == 0x200)
                {
                    entry.SampleSize = IsoReaderWriter.ReadUInt32(stream);
                }

                if ((flags & 0x400) == 0x400)
                {
                    entry.SampleFlags = IsoReaderWriter.ReadUInt32(stream);
                }

                if ((flags & 0x800) == 0x800)
                {
                    if (version == 0)
                    {
                        entry.SampleCompositionTimeOffset = (int)IsoReaderWriter.ReadUInt32(stream);
                    }
                    else
                    {
                        entry.SampleCompositionTimeOffset = IsoReaderWriter.ReadInt32(stream);
                    }
                }

                entries.Add(entry);
            }

            TrunBox trun = new TrunBox(
                size,
                parent,
                version,
                flags,
                dataOffset,
                firstSampleFlags,
                entries);

            return Task.FromResult((Mp4Box)trun);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            TrunBox b = (TrunBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);

            size += IsoReaderWriter.WriteUInt32(stream, (uint)b.Entries.Count);

            if ((b.Flags & 0x1) == 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)b.DataOffset);
            }
            if ((b.Flags & 0x4) == 0x4)
            {
                size += IsoReaderWriter.WriteUInt32(stream, b.FirstSampleFlags);
            }

            foreach (Entry entry in b.Entries)
            {
                if ((b.Flags & 0x100) == 0x100)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, entry.SampleDuration);
                }
                if ((b.Flags & 0x200) == 0x200)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, entry.SampleSize);
                }
                if ((b.Flags & 0x400) == 0x400)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, entry.SampleFlags);
                }
                if ((b.Flags & 0x800) == 0x800)
                {
                    if (b.Version == 0)
                    {
                        size += IsoReaderWriter.WriteUInt32(stream, (uint)entry.SampleCompositionTimeOffset);
                    }
                    else
                    {
                        size += IsoReaderWriter.WriteInt32(stream, entry.SampleCompositionTimeOffset);
                    }
                }
            }

            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            uint size = 8;

            if ((Flags & 0x1) == 0x1)
            {
                size += 4;
            }
            if ((Flags & 0x4) == 0x4)
            {
                size += 4;
            }

            long entrySize = 0;
            if ((Flags & 0x100) == 0x100)
            {
                entrySize += 4;
            }
            if ((Flags & 0x200) == 0x200)
            {
                entrySize += 4;
            }
            if ((Flags & 0x400) == 0x400)
            {
                entrySize += 4;
            }
            if ((Flags & 0x800) == 0x800)
            {
                entrySize += 4;
            }
            size += (uint)(entrySize * Entries.Count);
            return base.CalculateSize() + size;
        }
    }

    public class UrlBox : Mp4Box
    {
        public const string TYPE = "url ";

        public byte Version { get; set; }
        public uint Flags { get; set; } = 1;

        public UrlBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public UrlBox(uint size, Mp4Box parent, byte version, uint flags) : this(size, parent)
        {
            Version = version;
            Flags = flags;
        }

        public static Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            UrlBox url = new UrlBox(
                size,
                parent,
                version,
                flags);

            return Task.FromResult((Mp4Box)url);
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            UrlBox b = (UrlBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + 4;
        }
    }

    public class UnknownBox : Mp4Box
    {
        public byte[] Bytes { get; set; }

        public UnknownBox(uint size, string type, Mp4Box parent, byte[] bytes) : base(size, type, parent)
        {
            Bytes = bytes;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte[] bytes = new byte[size - 8];

            if (size - 8 > 0)
            {
                await IsoReaderWriter.ReadBytesAsync(stream, bytes, 0, bytes.Length);
            }

            UnknownBox b = new UnknownBox(
                size,
                type,
                parent,
                bytes);

            return b;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            UnknownBox b = (UnknownBox)box;
            uint size = 0;

            if (b.Bytes.Length > 0)
            {
                size += await IsoReaderWriter.WriteBytesAsync(stream, b.Bytes, 0, b.Bytes.Length);
            }

            return size;
        }

        public override uint CalculateSize()
        {
            return (uint)(base.CalculateSize() + Bytes.Length);
        }
    }

    public class AudioSampleEntryBox : ContainerMp4Box
    {
        public const string TYPE1 = "samr";
        public const string TYPE2 = "sawb";
        public const string TYPE3 = "mp4a";
        public const string TYPE4 = "drms";
        public const string TYPE5 = "alac";
        public const string TYPE6 = "owma";
        public const string TYPE7 = "ac-3";
        public const string TYPE8 = "ec-3";
        public const string TYPE9 = "mlpa";
        public const string TYPE10 = "dtsl";
        public const string TYPE11 = "dtsh";
        public const string TYPE12 = "dtse";
        public const string TYPE_ENCRYPTED = "enca";

        public AudioSampleEntryBox(
            uint size,
            Mp4Box parent,
            string type,
            uint dummy1,
            ushort dummy2,
            ushort dataReferenceIndex,
            ushort soundVersion,
            ushort dummy3,
            uint dummy4,
            ushort channelCount,
            ushort sampleSize,
            long sampleRate,
            ushort compressionId,
            ushort packetSize,
            uint samplesPerPacket,
            uint bytesPerPacket,
            uint bytesPerFrame,
            uint bytesPerSample,
            byte[] soundVersionData) : base(size, type, parent)
        {
            Dummy1 = dummy1;
            Dummy2 = dummy2;
            DataReferenceIndex = dataReferenceIndex;
            SoundVersion = soundVersion;
            Dummy3 = dummy3;
            Dummy4 = dummy4;
            ChannelCount = channelCount;
            SampleRate = sampleRate;
            SampleSize = sampleSize;
            CompressionId = compressionId;
            PacketSize = packetSize;
            SamplesPerPacket = samplesPerPacket;
            BytesPerPacket = bytesPerPacket;
            BytesPerFrame = bytesPerFrame;
            BytesPerSample = bytesPerSample;
            SoundVersionData = soundVersionData;
        }

        public AudioSampleEntryBox(uint size, Mp4Box parent, string type) : base(size, type, parent)
        { }

        public uint Dummy1 { get; set; } = 0;
        public ushort Dummy2 { get; set; } = 0;
        public ushort DataReferenceIndex { get; set; }
        public ushort SoundVersion { get; set; }
        public ushort Dummy3 { get; set; } = 0;
        public uint Dummy4 { get; set; } = 0;
        public ushort ChannelCount { get; set; }
        public long SampleRate { get; set; }
        public ushort SampleSize { get; set; }
        public ushort CompressionId { get; set; }
        public ushort PacketSize { get; set; }
        public uint SamplesPerPacket { get; set; }
        public uint BytesPerPacket { get; set; }
        public uint BytesPerFrame { get; set; }
        public uint BytesPerSample { get; set; }
        public byte[] SoundVersionData { get; set; }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            // first 6 bytes are 0
            uint dummy1 = IsoReaderWriter.ReadUInt32(stream);
            ushort dummy2 = IsoReaderWriter.ReadUInt16(stream);

            ushort dataReferenceIndex = IsoReaderWriter.ReadUInt16(stream);
            ushort soundVersion = IsoReaderWriter.ReadUInt16(stream);

            // reserved bytes
            ushort dummy3 = IsoReaderWriter.ReadUInt16(stream);
            uint dummy4 = IsoReaderWriter.ReadUInt32(stream);

            ushort channelCount = IsoReaderWriter.ReadUInt16(stream);
            ushort sampleSize = IsoReaderWriter.ReadUInt16(stream);
            ushort compressionId = IsoReaderWriter.ReadUInt16(stream);
            ushort packetSize = IsoReaderWriter.ReadUInt16(stream);
            long sampleRate = IsoReaderWriter.ReadUInt32(stream);
            if (!type.Equals(TYPE9))
            {
                sampleRate = (long)(ulong)sampleRate >> 16;
            }

            uint samplesPerPacket = 0;
            uint bytesPerPacket = 0;
            uint bytesPerFrame = 0;
            uint bytesPerSample = 0;
            byte[] soundVersionData = null;

            if (soundVersion == 1)
            {
                samplesPerPacket = IsoReaderWriter.ReadUInt32(stream);
                bytesPerPacket = IsoReaderWriter.ReadUInt32(stream);
                bytesPerFrame = IsoReaderWriter.ReadUInt32(stream);
                bytesPerSample = IsoReaderWriter.ReadUInt32(stream);
            }
            else if (soundVersion == 2)
            {
                samplesPerPacket = IsoReaderWriter.ReadUInt32(stream);
                bytesPerPacket = IsoReaderWriter.ReadUInt32(stream);
                bytesPerFrame = IsoReaderWriter.ReadUInt32(stream);
                bytesPerSample = IsoReaderWriter.ReadUInt32(stream);
                soundVersionData = new byte[20];
                await IsoReaderWriter.ReadBytesAsync(stream, soundVersionData, 0, soundVersionData.Length);
            }

            AudioSampleEntryBox audio = new AudioSampleEntryBox(
                size,
                parent,
                type,
                dummy1,
                dummy2,
                dataReferenceIndex,
                soundVersion,
                dummy3,
                dummy4,
                channelCount,
                sampleSize,
                sampleRate,
                compressionId,
                packetSize,
                samplesPerPacket,
                bytesPerPacket,
                bytesPerFrame,
                bytesPerSample,
                soundVersionData);

            uint parsedSize = (uint)(8 + 28
                    + (soundVersion == 1 ? 16 : 0)
                    + (soundVersion == 2 ? 36 : 0));

            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(audio, stream);
                parsedSize += box.GetSize();
                audio.Children.Add(box);
            }

            return audio;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            AudioSampleEntryBox b = (AudioSampleEntryBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteUInt32(stream, b.Dummy1);
            size += IsoReaderWriter.WriteUInt16(stream, b.Dummy2);

            size += IsoReaderWriter.WriteUInt16(stream, b.DataReferenceIndex);
            size += IsoReaderWriter.WriteUInt16(stream, b.SoundVersion);
            size += IsoReaderWriter.WriteUInt16(stream, b.Dummy3);
            size += IsoReaderWriter.WriteUInt32(stream, b.Dummy4);
            size += IsoReaderWriter.WriteUInt16(stream, b.ChannelCount);
            size += IsoReaderWriter.WriteUInt16(stream, b.SampleSize);
            size += IsoReaderWriter.WriteUInt16(stream, b.CompressionId);
            size += IsoReaderWriter.WriteUInt16(stream, b.PacketSize);

            if ("mlpa".CompareTo(b.Type) == 0)
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)b.SampleRate);
            }
            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, (uint)(b.SampleRate << 16));
            }

            if (b.SoundVersion == 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, b.SamplesPerPacket);
                size += IsoReaderWriter.WriteUInt32(stream, b.BytesPerPacket);
                size += IsoReaderWriter.WriteUInt32(stream, b.BytesPerFrame);
                size += IsoReaderWriter.WriteUInt32(stream, b.BytesPerSample);
            }

            if (b.SoundVersion == 2)
            {
                size += IsoReaderWriter.WriteUInt32(stream, b.SamplesPerPacket);
                size += IsoReaderWriter.WriteUInt32(stream, b.BytesPerPacket);
                size += IsoReaderWriter.WriteUInt32(stream, b.BytesPerFrame);
                size += IsoReaderWriter.WriteUInt32(stream, b.BytesPerSample);
                size += await IsoReaderWriter.WriteBytesAsync(stream, b.SoundVersionData, 0, b.SoundVersionData.Length);
            }

            foreach (var child in b.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }

            return size;
        }

        public override uint CalculateSize()
        {
            return (uint)(28 + base.CalculateSize() + (SoundVersion == 1 ? 16 : 0) + (SoundVersion == 2 ? 16 + SoundVersionData.Length : 0));
        }
    }

    public class EsdsBox : Mp4Box
    {
        public const string TYPE = "esds";

        public byte Version { get; set; }
        public uint Flags { get; set; }

        public ESDescriptor ESDescriptor { get; set; }

        public EsdsBox(uint size, Mp4Box parent) : base(size, TYPE, parent)
        { }

        public EsdsBox(uint size, Mp4Box parent, byte version, uint flags, ESDescriptor esDescriptor) : this(size, parent)
        {
            Version = version;
            Flags = flags;
            ESDescriptor = esDescriptor;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            uint flags = IsoReaderWriter.ReadUInt24(stream);

            uint remaining = size - 8 - 4;

            ESDescriptor descriptor = (ESDescriptor)await Mp4Parser.ReadDescriptor(-1, remaining, stream);
            EsdsBox esds = new EsdsBox(
                size,
                parent,
                version,
                flags,
                descriptor
                );

            return esds;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            EsdsBox b = (EsdsBox)box;
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteUInt24(stream, b.Flags);

            size += await Mp4Parser.WriteDescriptor(stream, -1, ESDescriptor.DESCRIPTOR, b.ESDescriptor);

            return size;
        }

        public override uint CalculateSize()
        {
            uint esDescriptorSize = ESDescriptor.CalculateSize();
            return base.CalculateSize() + 4 + 1 + IsoReaderWriter.CalculatePackedNumberLength(esDescriptorSize) + esDescriptorSize;
        }
    }

    public class VisualSampleEntryBox : ContainerMp4Box
    {
        public const string TYPE1 = "mp4v";
        public const string TYPE2 = "s263";
        public const string TYPE3 = "avc1";
        public const string TYPE4 = "avc3";
        public const string TYPE5 = "drmi";
        public const string TYPE6 = "hvc1";
        public const string TYPE7 = "hev1";
        public const string TYPE_ENCRYPTED = "encv";

        public uint Dummy1 { get; set; } = 0;
        public ushort Dummy2 { get; set; } = 0;
        public ushort DataReferenceIndex { get; set; }
        public ushort Dummy3 { get; set; } = 0;
        public ushort Dummy4 { get; set; } = 0;
        public uint Predefined1 { get; set; } = 0;
        public uint Predefined2 { get; set; } = 0;
        public uint Predefined3 { get; set; } = 0;
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public double HorizontalResolution { get; set; }
        public double VerticalResolution { get; set; }
        public uint Dummy5 { get; set; } = 0;
        public ushort FrameCount { get; set; }
        public byte CompressorNameDisplayableData { get; set; }
        public string CompressorName { get; set; }
        public ushort Depth { get; set; }
        public ushort DummyTerminator { get; set; } = 0xFFFF;

        public VisualSampleEntryBox(
            uint size,
            Mp4Box parent,
            string type,
            uint dummy1,
            ushort dummy2,
            ushort dataReferenceIndex,
            ushort dummy3,
            ushort dummy4,
            uint predefined1,
            uint predefined2,
            uint predefined3,
            ushort width,
            ushort height,
            double horizontalResolution,
            double verticalResolution,
            uint dummy5,
            ushort frameCount,
            byte compressorNameDisplayableData,
            string compressorName,
            ushort depth,
            ushort dummyTerminator) : base(size, type, parent)
        {
            Dummy1 = dummy1;
            Dummy2 = dummy2;
            DataReferenceIndex = dataReferenceIndex;
            Dummy3 = dummy3;
            Dummy4 = dummy4;
            Predefined1 = predefined1;
            Predefined2 = predefined2;
            Predefined3 = predefined3;
            Width = width;
            Height = height;
            HorizontalResolution = horizontalResolution;
            VerticalResolution = verticalResolution;
            Dummy5 = dummy5;
            FrameCount = frameCount;
            CompressorNameDisplayableData = compressorNameDisplayableData;
            CompressorName = compressorName;
            Depth = depth;
            DummyTerminator = dummyTerminator;
        }

        public VisualSampleEntryBox(uint size, Mp4Box parent, string type) : base(size, type, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            // first 6 bytes are 0
            uint dummy1 = IsoReaderWriter.ReadUInt32(stream);
            ushort dummy2 = IsoReaderWriter.ReadUInt16(stream);

            ushort dataReferenceIndex = IsoReaderWriter.ReadUInt16(stream);

            // reserved bytes
            ushort dummy3 = IsoReaderWriter.ReadUInt16(stream);
            ushort dummy4 = IsoReaderWriter.ReadUInt16(stream);

            // should also be 0
            uint predefined1 = IsoReaderWriter.ReadUInt32(stream);
            uint predefined2 = IsoReaderWriter.ReadUInt32(stream);
            uint predefined3 = IsoReaderWriter.ReadUInt32(stream);

            ushort width = IsoReaderWriter.ReadUInt16(stream);
            ushort height = IsoReaderWriter.ReadUInt16(stream);
            double horizontalResolution = IsoReaderWriter.ReadFixedPoint1616(stream);
            double verticalResolution = IsoReaderWriter.ReadFixedPoint1616(stream);

            // reserved, 0
            uint dummy5 = IsoReaderWriter.ReadUInt32(stream);

            ushort frameCount = IsoReaderWriter.ReadUInt16(stream);
            byte compressorNameDisplayableData = IsoReaderWriter.ReadByte(stream);
            if (compressorNameDisplayableData > 31)
            {
                compressorNameDisplayableData = 31;
            }
            // now read 31 bytes - string padded with 0
            byte[] compressorNameBytes = new byte[31];
            await IsoReaderWriter.ReadBytesAsync(stream, compressorNameBytes, 0, compressorNameBytes.Length);
            string compressorName = Encoding.UTF8.GetString(compressorNameBytes);

            ushort depth = IsoReaderWriter.ReadUInt16(stream);
            ushort dummyTerminator = IsoReaderWriter.ReadUInt16(stream); // should be 0xFFFF

            VisualSampleEntryBox visual = new VisualSampleEntryBox(
                size,
                parent,
                type,
                dummy1,
                dummy2,
                dataReferenceIndex,
                dummy3,
                dummy4,
                predefined1,
                predefined2,
                predefined3,
                width,
                height,
                horizontalResolution,
                verticalResolution,
                dummy5,
                frameCount,
                compressorNameDisplayableData,
                compressorName,
                depth,
                dummyTerminator);

            uint parsedSize = 8 + 78; // 78 bytes up until now

            while (parsedSize < size)
            {
                var box = await Mp4Parser.ReadBox(visual, stream);
                parsedSize += box.GetSize();
                visual.Children.Add(box);
            }

            return visual;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            VisualSampleEntryBox b = (VisualSampleEntryBox)box;
            uint size = 0;

            size += IsoReaderWriter.WriteUInt32(stream, b.Dummy1);
            size += IsoReaderWriter.WriteUInt16(stream, b.Dummy2);

            size += IsoReaderWriter.WriteUInt16(stream, b.DataReferenceIndex);
            size += IsoReaderWriter.WriteUInt16(stream, 0);
            size += IsoReaderWriter.WriteUInt16(stream, 0);
            size += IsoReaderWriter.WriteUInt32(stream, b.Predefined1);
            size += IsoReaderWriter.WriteUInt32(stream, b.Predefined2);
            size += IsoReaderWriter.WriteUInt32(stream, b.Predefined3);

            size += IsoReaderWriter.WriteUInt16(stream, b.Width);
            size += IsoReaderWriter.WriteUInt16(stream, b.Height);

            size += IsoReaderWriter.WriteFixedPoint1616(stream, b.HorizontalResolution);
            size += IsoReaderWriter.WriteFixedPoint1616(stream, b.VerticalResolution);

            size += IsoReaderWriter.WriteUInt32(stream, 0);
            size += IsoReaderWriter.WriteUInt16(stream, b.FrameCount);

            size += IsoReaderWriter.WriteByte(stream, b.CompressorNameDisplayableData);
            byte[] compressorNameBytes = Encoding.UTF8.GetBytes(b.CompressorName);
            if (compressorNameBytes.Length < 31)
            {
                compressorNameBytes = compressorNameBytes.Concat(new byte[31 - compressorNameBytes.Length]).ToArray();
            }
            else if (compressorNameBytes.Length > 31)
            {
                compressorNameBytes = compressorNameBytes.Take(31).ToArray();
            }

            size += await IsoReaderWriter.WriteBytesAsync(stream, compressorNameBytes, 0, compressorNameBytes.Length);

            size += IsoReaderWriter.WriteUInt16(stream, b.Depth);
            size += IsoReaderWriter.WriteUInt16(stream, b.DummyTerminator);

            foreach (var child in b.Children)
            {
                size += await Mp4Parser.WriteBox(stream, child);
            }

            return size;
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + 78;
        }
    }

    public class MdatBox : Mp4Box, IDisposable
    {
        public const string TYPE = "mdat";

        private ITemporaryStorage _storage;
        private bool _disposedValue;

        public ITemporaryStorage GetStorage() { return _storage; }

        public MdatBox(uint size, Mp4Box parent, ITemporaryStorage storage) : base(size, TYPE, parent)
        {
            _storage = storage;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            byte[] buffer = new byte[1024];
            int remaining = (int)size - 8;

            var storage = TemporaryStorage.Factory.Create();

            int read = 0;
            while (remaining > 0)
            {
                int count = Math.Min(buffer.Length, remaining);
                read = stream.Read(buffer, 0, count);

                if (read > 0) await storage.Stream.WriteAsync(buffer, 0, read);

                if (read == 0)
                {
                    if(Log.InfoEnabled) Log.Info($"Error, end of stream reached! Missing {remaining} bytes.");
                    break;
                }

                remaining -= read;
            }

            await storage.Stream.FlushAsync();
            storage.Stream.Seek(0, SeekOrigin.Begin);

            MdatBox mdat = new MdatBox(
                size,
                parent,
                storage);

            return mdat;
        }

        public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            MdatBox mdat = (MdatBox)box;
            var mdatStorage = mdat.GetStorage();
            mdatStorage.Stream.Seek(0, SeekOrigin.Begin);
            uint size = (uint)mdatStorage.Stream.Length;
            await mdatStorage.Stream.CopyToAsync(stream);
            return size;
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + (uint)_storage.Stream.Length;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_storage != null)
                        _storage.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class ScalingMatrix
    {
        public ScalingList[] ScalingList4x4 { get; set; }
        public ScalingList[] ScalingList8x8 { get; set; }

        public static ScalingMatrix Parse(BitStreamReader bitstream)
        {
            ScalingMatrix scalingMatrix = new ScalingMatrix();
            for (int i = 0; i < 8; i++)
            {
                int seqScalingListPresentFlag = bitstream.ReadBit();
                if (seqScalingListPresentFlag != 0)
                {
                    scalingMatrix.ScalingList4x4 = new ScalingList[8];
                    scalingMatrix.ScalingList8x8 = new ScalingList[8];
                    if (i < 6)
                    {
                        scalingMatrix.ScalingList4x4[i] = ScalingList.Parse(bitstream, 16);
                    }
                    else
                    {
                        scalingMatrix.ScalingList8x8[i - 6] = ScalingList.Parse(bitstream, 64);
                    }
                }
            }

            return scalingMatrix;
        }

        public static void Build(BitStreamWriter bitstream, ScalingMatrix scalingMatrix)
        {
            for (int i = 0; i < 8; i++)
            {
                int seqScalingListPresentFlag = 0;
                if ((scalingMatrix.ScalingList4x4 != null && scalingMatrix.ScalingList4x4[i] != null) ||
                    (scalingMatrix.ScalingList8x8 != null && scalingMatrix.ScalingList8x8[i] != null))
                {
                    seqScalingListPresentFlag = 1;
                }

                bitstream.WriteBit(seqScalingListPresentFlag);

                if (seqScalingListPresentFlag != 0)
                {
                    if (i < 6)
                    {
                        ScalingList.Build(bitstream, scalingMatrix.ScalingList4x4[i]);
                    }
                    else
                    {
                        ScalingList.Build(bitstream, scalingMatrix.ScalingList8x8[i - 6]);
                    }
                }
            }
        }
    }

    public class ScalingList
    {
        public int[] List { get; set; }
        public bool UseDefaultScalingMatrixFlag { get; set; }

        public static ScalingList Parse(BitStreamReader bitstream, int size)
        {
            ScalingList sl = new ScalingList();
            sl.List = new int[size];
            int lastScale = 8;
            int nextScale = 8;

            for (int j = 0; j < size; j++)
            {
                if (nextScale != 0)
                {
                    int deltaScale = bitstream.ReadSE();
                    nextScale = (lastScale + deltaScale + 256) % 256;
                    sl.UseDefaultScalingMatrixFlag = j == 0 && nextScale == 0;
                }
                sl.List[j] = nextScale == 0 ? lastScale : nextScale;
                lastScale = sl.List[j];
            }

            return sl;
        }

        public static void Build(BitStreamWriter bitstream, ScalingList sl)
        {
            if (sl.UseDefaultScalingMatrixFlag)
            {
                bitstream.WriteSE(0);
                return;
            }

            int lastScale = 8;
            int nextScale = 8;
            for (int j = 0; j < sl.List.Length; j++)
            {
                if (nextScale != 0)
                {
                    int deltaScale = sl.List[j] - lastScale - 256;
                    bitstream.WriteSE(deltaScale);
                }
                lastScale = sl.List[j];
            }
        }
    }

    public class UnknownDescriptor : DescriptorBase
    {
        public const int DESCRIPTOR = 0xFF;
        public const int OBJECT_TYPE_INDICATION = -1;

        public byte[] Buffer { get; set; }

        public UnknownDescriptor() : base(OBJECT_TYPE_INDICATION, DESCRIPTOR)
        { }

        public UnknownDescriptor(int objectTypeIndication, byte tag) : base(objectTypeIndication, tag)
        { }

        public UnknownDescriptor(int objectTypeIndication, byte tag, byte[] buffer) : this(objectTypeIndication, tag)
        {
            Buffer = buffer;
        }

        public static async Task<DescriptorBase> ParseAsync(uint size, Stream stream, int objectTypeIndication, byte tag)
        {
            byte[] buffer = new byte[size];
            await IsoReaderWriter.ReadBytesAsync(stream, buffer, 0, (int)size);
            UnknownDescriptor unknown = new UnknownDescriptor(objectTypeIndication, tag, buffer);
            return unknown;
        }

        public static async Task<uint> BuildAsync(Stream stream, int objectIndicationIdentifier, byte tag, DescriptorBase descriptor)
        {
            UnknownDescriptor b = (UnknownDescriptor)descriptor;
            uint size = 0;
            size += await IsoReaderWriter.WriteBytesAsync(stream, b.Buffer, 0, b.Buffer.Length);
            return size;
        }

        public override uint CalculateSize()
        {
            return (uint)Buffer.Length;
        }
    }

    public class ESDescriptor : DescriptorBase
    {
        public const int DESCRIPTOR = 0x3;
        public const int OBJECT_TYPE_INDICATION = -1;

        public ESDescriptor() : base(OBJECT_TYPE_INDICATION, DESCRIPTOR)
        { }

        public ESDescriptor(
            ushort esId,
            int streamDependenceFlag,
            int urlFlag,
            int ocrStreamFlag,
            int streamPriority,
            ushort dependsOnEsId,
            byte urlLength,
            string urlString,
            ushort ocrEsId) : this()
        {
            EsId = esId;
            StreamDependenceFlag = streamDependenceFlag;
            UrlFlag = urlFlag;
            OcrStreamFlag = ocrStreamFlag;
            StreamPriority = streamPriority;
            DependsOnEsId = dependsOnEsId;
            UrlLength = urlLength;
            UrlString = urlString;
            OcrEsId = ocrEsId;
        }

        public ushort EsId { get; set; }
        public int StreamDependenceFlag { get; set; }
        public int UrlFlag { get; set; }
        public int OcrStreamFlag { get; set; }
        public int StreamPriority { get; set; }
        public ushort DependsOnEsId { get; set; }
        public byte UrlLength { get; set; }
        public string UrlString { get; set; } = string.Empty;
        public ushort OcrEsId { get; set; }
        public List<DescriptorBase> Descriptors { get; set; } = new List<DescriptorBase>();

        public static async Task<DescriptorBase> ParseAsync(uint size, Stream stream)
        {
            ushort esId = IsoReaderWriter.ReadUInt16(stream);

            byte data = IsoReaderWriter.ReadByte(stream);
            int streamDependenceFlag = (int)((uint)data >> 7);
            int urlFlag = (int)((uint)data >> 6) & 0x1;
            int ocrStreamFlag = (int)((uint)data >> 5) & 0x1;
            int streamPriority = data & 0x1f;

            uint consumedLength = 3; // so far we've read 3 bytes

            ushort dependsOnEsId = 0;
            if (streamDependenceFlag == 1)
            {
                dependsOnEsId = IsoReaderWriter.ReadUInt16(stream);
                consumedLength = consumedLength + 2;
            }

            byte urlLength = 0;
            string urlString = "";
            if (urlFlag == 1)
            {
                urlLength = IsoReaderWriter.ReadByte(stream);
                urlString = await IsoReaderWriter.ReadStringAsync(stream, urlLength);
                consumedLength = consumedLength + 1 + urlLength;
            }

            ushort ocrEsId = 0;
            if (ocrStreamFlag == 1)
            {
                ocrEsId = IsoReaderWriter.ReadUInt16(stream);
                consumedLength = consumedLength + 2;
            }

            uint remaining = size - consumedLength;

            ESDescriptor es = new ESDescriptor(
                esId,
                streamDependenceFlag,
                urlFlag,
                ocrStreamFlag,
                streamPriority,
                dependsOnEsId,
                urlLength,
                urlString,
                ocrEsId);

            while (remaining > 1)
            {
                var descriptor = await Mp4Parser.ReadDescriptor(-1, remaining, stream);
                es.Descriptors.Add(descriptor);
                remaining = remaining - descriptor.GetSize();
            }

            return es;
        }

        public static async Task<uint> BuildAsync(Stream stream, int objectIndicationIdentifier, byte tag, DescriptorBase descriptor)
        {
            uint size = 0;
            ESDescriptor esd = (ESDescriptor)descriptor;
            size += IsoReaderWriter.WriteUInt16(stream, esd.EsId);
            int flags = esd.StreamDependenceFlag << 7 | esd.UrlFlag << 6 | esd.OcrStreamFlag << 5 | esd.StreamPriority & 0x1f;
            size += IsoReaderWriter.WriteByte(stream, (byte)flags);
            if (esd.StreamDependenceFlag == 1)
            {
                size += IsoReaderWriter.WriteUInt16(stream, esd.DependsOnEsId);
            }
            if (esd.UrlFlag == 1)
            {
                size += IsoReaderWriter.WriteByte(stream, esd.UrlLength);
                size += IsoReaderWriter.WriteString(stream, esd.UrlString);
            }
            if (esd.OcrStreamFlag == 1)
            {
                size += IsoReaderWriter.WriteUInt16(stream, esd.OcrEsId);
            }

            for (int i = 0; i < esd.Descriptors.Count; i++)
            {
                var child = esd.Descriptors[i];
                size += await Mp4Parser.WriteDescriptor(stream, -1, child.DescriptorType, child);
            }

            return size;
        }

        public override uint CalculateSize()
        {
            uint output = 3;
            if (StreamDependenceFlag == 1)
            {
                output += 2;
            }
            if (UrlFlag == 1)
            {
                output = output + 1 + UrlLength;
            }
            if (OcrStreamFlag == 1)
            {
                output += 2;
            }

            foreach (var descriptor in Descriptors)
            {
                uint descriptorContentSize = descriptor.CalculateSize();
                output += 1 + IsoReaderWriter.CalculatePackedNumberLength(descriptorContentSize) + descriptorContentSize;
            }

            return output;
        }
    }

    public class SLConfigDescriptor : DescriptorBase
    {
        public const int DESCRIPTOR = 0x6;
        public const int OBJECT_TYPE_INDICATION = 0x40;

        public SLConfigDescriptor() : base(OBJECT_TYPE_INDICATION, DESCRIPTOR)
        { }

        public SLConfigDescriptor(byte predefined) : this()
        {
            Predefined = predefined;
        }

        public byte Predefined { get; set; }

        public static Task<DescriptorBase> ParseAsync(uint size, Stream stream)
        {
            byte predefined = IsoReaderWriter.ReadByte(stream);
            SLConfigDescriptor slConfig = new SLConfigDescriptor(predefined);
            return Task.FromResult((DescriptorBase)slConfig);
        }

        public static Task<uint> BuildAsync(Stream stream, int objectIndicationIdentifier, byte tag, DescriptorBase descriptor)
        {
            uint size = 0;
            SLConfigDescriptor slc = (SLConfigDescriptor)descriptor;
            size += IsoReaderWriter.WriteByte(stream, slc.Predefined);
            return Task.FromResult(size);
        }

        public override uint CalculateSize()
        {
            return 1;
        }
    }

    public class DecoderConfigDescriptor : DescriptorBase
    {
        public const int DESCRIPTOR = 0x4;
        public const int OBJECT_TYPE_INDICATION = -1;

        public DecoderConfigDescriptor(int objectTypeIndication) : base(objectTypeIndication, DESCRIPTOR)
        { }

        public DecoderConfigDescriptor(
            int objectTypeIndication,
            int streamType,
            int upStream,
            uint bufferSizeDB,
            uint maxBitRate,
            uint avgBitRate,
            DecoderSpecificInfoDescriptor decoderSpecificInfo,
            AudioSpecificConfigDescriptor audioSpecificConfig,
            List<ProfileLevelIndicationDescriptor> profileLevelIndicationDescriptors) : this(objectTypeIndication)
        {
            StreamType = streamType;
            UpStream = upStream;
            BufferSizeDB = bufferSizeDB;
            MaxBitRate = maxBitRate;
            AvgBitRate = avgBitRate;
            DecoderSpecificInfo = decoderSpecificInfo;
            AudioSpecificConfig = audioSpecificConfig;
            ProfileLevelIndicationDescriptors = profileLevelIndicationDescriptors;
        }

        public int StreamType { get; set; }
        public int UpStream { get; set; }
        public uint BufferSizeDB { get; set; }
        public uint MaxBitRate { get; set; }
        public uint AvgBitRate { get; set; }
        public DecoderSpecificInfoDescriptor DecoderSpecificInfo { get; set; }
        public AudioSpecificConfigDescriptor AudioSpecificConfig { get; set; }
        public List<ProfileLevelIndicationDescriptor> ProfileLevelIndicationDescriptors { get; set; } = new List<ProfileLevelIndicationDescriptor>();

        public static async Task<DescriptorBase> ParseAsync(uint size, Stream stream)
        {
            byte objectTypeIndication = IsoReaderWriter.ReadByte(stream);
            byte data = IsoReaderWriter.ReadByte(stream);

            int streamType = (int)((uint)data >> 2);
            int upStream = data >> 1 & 0x1;

            uint bufferSizeDB = IsoReaderWriter.ReadUInt24(stream);
            uint maxBitRate = IsoReaderWriter.ReadUInt32(stream);
            uint avgBitRate = IsoReaderWriter.ReadUInt32(stream);

            uint consumedLength = 13;
            uint remaining = size - consumedLength;
            DecoderSpecificInfoDescriptor decoderSpecificInfo = null;
            AudioSpecificConfigDescriptor audioSpecificConfig = null;
            List<ProfileLevelIndicationDescriptor> profileLevelIndicationDescriptors = new List<ProfileLevelIndicationDescriptor>();

            while (remaining > 2)
            {
                var descriptor = await Mp4Parser.ReadDescriptor(objectTypeIndication, remaining, stream);
                remaining = remaining - descriptor.GetSize();

                if (descriptor is DecoderSpecificInfoDescriptor)
                {
                    decoderSpecificInfo = (DecoderSpecificInfoDescriptor)descriptor;
                }
                else if (descriptor is AudioSpecificConfigDescriptor)
                {
                    audioSpecificConfig = (AudioSpecificConfigDescriptor)descriptor;
                }
                else if (descriptor is ProfileLevelIndicationDescriptor)
                {
                    profileLevelIndicationDescriptors.Add((ProfileLevelIndicationDescriptor)descriptor);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            DecoderConfigDescriptor decoderConfig = new DecoderConfigDescriptor(
                objectTypeIndication,
                streamType,
                upStream,
                bufferSizeDB,
                maxBitRate,
                avgBitRate,
                decoderSpecificInfo,
                audioSpecificConfig,
                profileLevelIndicationDescriptors
            );

            return decoderConfig;
        }

        public static async Task<uint> BuildAsync(Stream stream, int objectIndicationIdentifier, byte tag, DescriptorBase descriptor)
        {
            uint size = 0;
            DecoderConfigDescriptor dcd = (DecoderConfigDescriptor)descriptor;
            size += IsoReaderWriter.WriteByte(stream, (byte)descriptor.ObjectTypeIndication);
            int flags = dcd.StreamType << 2 | dcd.UpStream << 1 | 1;
            size += IsoReaderWriter.WriteByte(stream, (byte)flags);
            size += IsoReaderWriter.WriteUInt24(stream, dcd.BufferSizeDB);
            size += IsoReaderWriter.WriteUInt32(stream, dcd.MaxBitRate);
            size += IsoReaderWriter.WriteUInt32(stream, dcd.AvgBitRate);

            if (dcd.DecoderSpecificInfo != null)
            {
                var child = dcd.DecoderSpecificInfo;
                size += await Mp4Parser.WriteDescriptor(stream, child.ObjectTypeIndication, child.DescriptorType, child);
            }

            if (dcd.AudioSpecificConfig != null)
            {
                var child = dcd.AudioSpecificConfig;
                size += await Mp4Parser.WriteDescriptor(stream, child.ObjectTypeIndication, child.DescriptorType, child);
            }

            foreach (ProfileLevelIndicationDescriptor plid in dcd.ProfileLevelIndicationDescriptors)
            {
                var child = plid;
                size += await Mp4Parser.WriteDescriptor(stream, child.ObjectTypeIndication, child.DescriptorType, child);
            }

            return size;
        }

        public override uint CalculateSize()
        {
            uint output = 13;
            if (AudioSpecificConfig != null)
            {
                uint descriptorContentSize = AudioSpecificConfig.CalculateSize();
                output += 1 + IsoReaderWriter.CalculatePackedNumberLength(descriptorContentSize) + descriptorContentSize;
            }

            if (DecoderSpecificInfo != null)
            {
                uint descriptorContentSize = DecoderSpecificInfo.CalculateSize();
                output += 1 + IsoReaderWriter.CalculatePackedNumberLength(descriptorContentSize) + descriptorContentSize;
            }

            foreach (ProfileLevelIndicationDescriptor profileLevelIndicationDescriptor in ProfileLevelIndicationDescriptors)
            {
                uint descriptorContentSize = profileLevelIndicationDescriptor.CalculateSize();
                output += 1 + IsoReaderWriter.CalculatePackedNumberLength(descriptorContentSize) + descriptorContentSize;
            }
            return output;
        }
    }

    public class AudioSpecificConfigDescriptor : DescriptorBase
    {
        public const int DESCRIPTOR = 0x5;
        public const int OBJECT_TYPE_INDICATION = 0x40;

        public AudioSpecificConfigDescriptor() : base(OBJECT_TYPE_INDICATION, DESCRIPTOR)
        { }

        public AudioSpecificConfigDescriptor(
            int originalAudioObjectType,
            int samplingFrequencyIndex,
            int samplingFrequency,
            int channelConfiguration,
            int extensionAudioObjectType,
            int extensionChannelConfiguration,
            int extensionSamplingFrequency,
            int extensionSamplingFrequencyIndex,
            bool sbrPresentFlag,
            bool psPresentFlag,
            bool gaSpecificConfig,
            int frameLengthFlag,
            int dependsOnCoreCoder,
            int coreCoderDelay,
            int extensionFlag,
            int layerNr,
            int numOfSubFrame,
            int layerLength,
            int aacSectionDataResilienceFlag,
            int aacScalefactorDataResilienceFlag,
            int aacSpectralDataResilienceFlag,
            int extensionFlag3,
            int outerSyncExtensionType,
            int syncExtensionType,
            int innerSyncExtensionType) : this()
        {
            OriginalAudioObjectType = originalAudioObjectType;
            SamplingFrequencyIndex = samplingFrequencyIndex;
            SamplingFrequency = samplingFrequency;
            ChannelConfiguration = channelConfiguration;
            ExtensionAudioObjectType = extensionAudioObjectType;
            ExtensionChannelConfiguration = extensionChannelConfiguration;
            ExtensionSamplingFrequency = extensionSamplingFrequency;
            ExtensionSamplingFrequencyIndex = extensionSamplingFrequencyIndex;
            SbrPresentFlag = sbrPresentFlag;
            PsPresentFlag = psPresentFlag;
            GaSpecificConfig = gaSpecificConfig;
            FrameLengthFlag = frameLengthFlag;
            DependsOnCoreCoder = dependsOnCoreCoder;
            CoreCoderDelay = coreCoderDelay;
            ExtensionFlag = extensionFlag;
            LayerNr = layerNr;
            NumOfSubFrame = numOfSubFrame;
            LayerLength = layerLength;
            AacSectionDataResilienceFlag = aacSectionDataResilienceFlag;
            AacScalefactorDataResilienceFlag = aacScalefactorDataResilienceFlag;
            AacSpectralDataResilienceFlag = aacSpectralDataResilienceFlag;
            ExtensionFlag3 = extensionFlag3;
            OuterSyncExtensionType = outerSyncExtensionType;
            SyncExtensionType = syncExtensionType;
            InnerSyncExtensionType = innerSyncExtensionType;
        }

        // currently not needed
        public int OriginalAudioObjectType { get; set; }
        public int SamplingFrequencyIndex { get; set; }
        public int SamplingFrequency { get; set; }
        public int ChannelConfiguration { get; set; }
        public int ExtensionAudioObjectType { get; set; }
        public int ExtensionChannelConfiguration { get; set; }
        public int ExtensionSamplingFrequency { get; set; }
        public int ExtensionSamplingFrequencyIndex { get; set; } = -1;
        public bool SbrPresentFlag { get; set; }
        public bool PsPresentFlag { get; set; }
        public bool GaSpecificConfig { get; set; }
        public int FrameLengthFlag { get; set; }
        public int DependsOnCoreCoder { get; set; }
        public int CoreCoderDelay { get; set; }
        public int ExtensionFlag { get; set; }
        public int LayerNr { get; set; }
        public int NumOfSubFrame { get; set; }
        public int LayerLength { get; set; }
        public int AacSectionDataResilienceFlag { get; set; }
        public int AacScalefactorDataResilienceFlag { get; set; }
        public int AacSpectralDataResilienceFlag { get; set; }
        public int ExtensionFlag3 { get; set; }
        public int OuterSyncExtensionType { get; set; } = -1;
        public int SyncExtensionType { get; set; } = -1;
        public int InnerSyncExtensionType { get; set; } = -1;

        public override uint CalculateSize()
        {
            uint sizeInBits = 5;
            if (OriginalAudioObjectType > 30)
            {
                sizeInBits += 6;
            }
            sizeInBits += 4;
            if (SamplingFrequencyIndex == 0xf)
            {
                sizeInBits += 24;
            }
            sizeInBits += 4;
            if (OriginalAudioObjectType == 5 || OriginalAudioObjectType == 29)
            {
                sizeInBits += 4;
                if (ExtensionSamplingFrequencyIndex == 0xf)
                {
                    sizeInBits += 24;
                }
            }

            if (OriginalAudioObjectType == 22)
            {
                sizeInBits += 4;
            }

            if (GaSpecificConfig)
            {
                sizeInBits += CalculateGaSpecificConfigSize();
            }
            if (OuterSyncExtensionType >= 0)
            {
                sizeInBits += 11;
                if (OuterSyncExtensionType == 0x2b7)
                {
                    sizeInBits += 5;
                    if (ExtensionAudioObjectType > 30)
                    {
                        sizeInBits += 6;
                    }
                    if (ExtensionAudioObjectType == 5)
                    {
                        sizeInBits += 1;
                        if (SbrPresentFlag)
                        {
                            sizeInBits += 4;
                            if (ExtensionSamplingFrequencyIndex == 0xf)
                            {
                                sizeInBits += 24;
                            }
                            if (InnerSyncExtensionType >= 0)
                            {
                                sizeInBits += 11;
                                if (InnerSyncExtensionType == 0x548)
                                {
                                    sizeInBits += 1;
                                }
                            }
                        }
                    }
                    if (ExtensionAudioObjectType == 22)
                    {
                        sizeInBits += 1;
                        if (SbrPresentFlag)
                        {
                            sizeInBits += 4;
                            if (ExtensionSamplingFrequencyIndex == 0xf)
                            {
                                sizeInBits += 24;
                            }
                        }
                        sizeInBits += 4;
                    }
                }
            }
            return (uint)Math.Ceiling((double)sizeInBits / 8);
        }

        private uint CalculateGaSpecificConfigSize()
        {
            uint n = 0;
            n += 1;
            n += 1;
            if (DependsOnCoreCoder == 1)
            {
                n += 14;
            }
            n += 1;
            if (ChannelConfiguration == 0)
            {
                throw new NotSupportedException("Cannot parse program_config_element yet");
            }
            if (OriginalAudioObjectType == 6 || OriginalAudioObjectType == 20)
            {
                n += 3;
            }
            if (ExtensionFlag == 1)
            {
                if (OriginalAudioObjectType == 22)
                {
                    n += 5;
                    n += 11;
                }
                if (OriginalAudioObjectType == 17 || OriginalAudioObjectType == 19 || OriginalAudioObjectType == 20 || OriginalAudioObjectType == 23)
                {
                    n += 1;
                    n += 1;
                    n += 1;
                }
                n += 1;
                if (ExtensionFlag3 == 1)
                {
                    throw new NotImplementedException();
                }
            }
            return n;
        }

        public static Task<DescriptorBase> ParseAsync(uint size, Stream stream)
        {
            BitStreamReader bitstream = new BitStreamReader(stream) { ShouldUnescapeNals = false }; // TODO: New bitstream?
            int originalAudioObjectType = ReadAudioObjectType(bitstream);
            int audioObjectType = originalAudioObjectType;
            int samplingFrequencyIndex = bitstream.ReadBits(4);

            int samplingFrequency = 0;
            if (samplingFrequencyIndex == 0xf)
            {
                samplingFrequency = bitstream.ReadBits(24);
            }

            int channelConfiguration = bitstream.ReadBits(4);

            int extensionAudioObjectType = 0;
            int extensionChannelConfiguration = 0;
            int extensionSamplingFrequency = 0;
            int extensionSamplingFrequencyIndex = -1;
            bool sbrPresentFlag = false;
            bool psPresentFlag = false;
            if (audioObjectType == 5 || audioObjectType == 29)
            {
                extensionAudioObjectType = 5;
                sbrPresentFlag = true;
                if (audioObjectType == 29)
                {
                    psPresentFlag = true;
                }
                extensionSamplingFrequencyIndex = bitstream.ReadBits(4);
                if (extensionSamplingFrequencyIndex == 0xf)
                {
                    extensionSamplingFrequency = bitstream.ReadBits(24);
                }
                audioObjectType = ReadAudioObjectType(bitstream);
                if (audioObjectType == 22)
                {
                    extensionChannelConfiguration = bitstream.ReadBits(4);
                }
            }

            bool gaSpecificConfig = false;
            int frameLengthFlag = 0;
            int dependsOnCoreCoder = 0;
            int coreCoderDelay = 0;
            int extensionFlag = 0;
            int layerNr = 0;
            int numOfSubFrame = 0;
            int layerLength = 0;
            int aacSectionDataResilienceFlag = 0;
            int aacScalefactorDataResilienceFlag = 0;
            int aacSpectralDataResilienceFlag = 0;
            int extensionFlag3 = 0;

            switch (audioObjectType)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 6:
                case 7:
                case 17:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    {
                        frameLengthFlag = bitstream.ReadBit();
                        dependsOnCoreCoder = bitstream.ReadBit();
                        if (dependsOnCoreCoder == 1)
                        {
                            coreCoderDelay = bitstream.ReadBits(14);
                        }
                        extensionFlag = bitstream.ReadBit();
                        if (channelConfiguration == 0)
                        {
                            throw new NotSupportedException("Cannot parse program_config_element yet");
                        }
                        if (audioObjectType == 6 || audioObjectType == 20)
                        {
                            layerNr = bitstream.ReadBits(3);
                        }
                        if (extensionFlag == 1)
                        {
                            if (audioObjectType == 22)
                            {
                                numOfSubFrame = bitstream.ReadBits(5);
                                layerLength = bitstream.ReadBits(11);
                            }
                            if (audioObjectType == 17 || audioObjectType == 19 || audioObjectType == 20 || audioObjectType == 23)
                            {
                                aacSectionDataResilienceFlag = bitstream.ReadBit();
                                aacScalefactorDataResilienceFlag = bitstream.ReadBit();
                                aacSpectralDataResilienceFlag = bitstream.ReadBit();
                            }
                            extensionFlag3 = bitstream.ReadBit();
                            if (extensionFlag3 == 1)
                            {
                                throw new NotImplementedException();
                            }
                        }

                        gaSpecificConfig = true;
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }

            int outerSyncExtensionType = -1;
            int syncExtensionType = -1;
            int innerSyncExtensionType = -1;
            if (extensionAudioObjectType != 5 && bitstream.RemainingBits(size) >= 16)
            {
                outerSyncExtensionType = syncExtensionType = bitstream.ReadBits(11);
                if (syncExtensionType == 0x2b7)
                {
                    extensionAudioObjectType = ReadAudioObjectType(bitstream);
                    if (extensionAudioObjectType == 5)
                    {
                        sbrPresentFlag = bitstream.ReadBit() == 1;
                        if (sbrPresentFlag)
                        {
                            extensionSamplingFrequencyIndex = bitstream.ReadBits(4);
                            if (extensionSamplingFrequencyIndex == 0xf)
                            {
                                extensionSamplingFrequency = bitstream.ReadBits(24);
                            }
                            if (bitstream.RemainingBits(size) >= 12)
                            {
                                innerSyncExtensionType = syncExtensionType = bitstream.ReadBits(11);
                                if (syncExtensionType == 0x548)
                                {
                                    psPresentFlag = bitstream.ReadBit() == 1;
                                }
                            }
                        }
                    }
                    if (extensionAudioObjectType == 22)
                    {
                        sbrPresentFlag = bitstream.ReadBit() == 1;
                        if (sbrPresentFlag)
                        {
                            extensionSamplingFrequencyIndex = bitstream.ReadBits(4);
                            if (extensionSamplingFrequencyIndex == 0xf)
                            {
                                extensionSamplingFrequency = bitstream.ReadBits(24);
                            }
                        }
                        extensionChannelConfiguration = bitstream.ReadBits(4);
                    }
                }
            }

            AudioSpecificConfigDescriptor audioSpecificConfig = new AudioSpecificConfigDescriptor(
                originalAudioObjectType,
                samplingFrequencyIndex,
                samplingFrequency,
                channelConfiguration,
                extensionAudioObjectType,
                extensionChannelConfiguration,
                extensionSamplingFrequency,
                extensionSamplingFrequencyIndex,
                sbrPresentFlag,
                psPresentFlag,
                gaSpecificConfig,
                frameLengthFlag,
                dependsOnCoreCoder,
                coreCoderDelay,
                extensionFlag,
                layerNr,
                numOfSubFrame,
                layerLength,
                aacSectionDataResilienceFlag,
                aacScalefactorDataResilienceFlag,
                aacSpectralDataResilienceFlag,
                extensionFlag3,
                outerSyncExtensionType,
                syncExtensionType,
                innerSyncExtensionType
                );
            return Task.FromResult((DescriptorBase)audioSpecificConfig);
        }

        public static Task<uint> BuildAsync(Stream stream, int objectIndicationIdentifier, byte tag, DescriptorBase descriptor)
        {
            uint size = 0;
            AudioSpecificConfigDescriptor asc = (AudioSpecificConfigDescriptor)descriptor;
            BitStreamWriter bitstream = new BitStreamWriter(stream) { ShouldEscapeNals = false };
            WriteAudioObjectType(bitstream, asc.OriginalAudioObjectType);
            bitstream.WriteBits(4, asc.SamplingFrequencyIndex);

            if (asc.SamplingFrequencyIndex == 0xf)
            {
                bitstream.WriteBits(24, asc.SamplingFrequency);
            }

            bitstream.WriteBits(4, asc.ChannelConfiguration);

            if (asc.OriginalAudioObjectType == 5 || asc.OriginalAudioObjectType == 29)
            {
                asc.ExtensionAudioObjectType = 5;
                asc.SbrPresentFlag = true;
                if (asc.OriginalAudioObjectType == 29)
                {
                    asc.PsPresentFlag = true;
                }
                bitstream.WriteBits(4, asc.ExtensionSamplingFrequencyIndex);
                if (asc.ExtensionSamplingFrequencyIndex == 0xf)
                {
                    bitstream.WriteBits(24, asc.ExtensionSamplingFrequency);
                }
                WriteAudioObjectType(bitstream, asc.OriginalAudioObjectType);
                if (asc.OriginalAudioObjectType == 22)
                {
                    bitstream.WriteBits(4, asc.ExtensionChannelConfiguration);
                }
            }
            switch (asc.OriginalAudioObjectType)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 6:
                case 7:
                case 17:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    bitstream.WriteBit(asc.FrameLengthFlag);
                    bitstream.WriteBit(asc.DependsOnCoreCoder);
                    if (asc.DependsOnCoreCoder != 0)
                    {
                        bitstream.WriteBits(14, asc.CoreCoderDelay);
                    }
                    bitstream.WriteBit(asc.ExtensionFlag);
                    if (asc.ChannelConfiguration == 0)
                    {
                        throw new NotSupportedException("can't write program_config_element yet");
                    }
                    if (asc.OriginalAudioObjectType == 6 || asc.OriginalAudioObjectType == 20)
                    {
                        bitstream.WriteBits(3, asc.LayerNr);
                    }
                    if (asc.ExtensionFlag == 1)
                    {
                        if (asc.OriginalAudioObjectType == 22)
                        {
                            bitstream.WriteBits(5, asc.NumOfSubFrame);
                            bitstream.WriteBits(11, asc.LayerLength);
                        }
                        if (asc.OriginalAudioObjectType == 17 || asc.OriginalAudioObjectType == 19 || asc.OriginalAudioObjectType == 20 || asc.OriginalAudioObjectType == 23)
                        {
                            bitstream.WriteBit(asc.AacSectionDataResilienceFlag);
                            bitstream.WriteBit(asc.AacScalefactorDataResilienceFlag);
                            bitstream.WriteBit(asc.AacSpectralDataResilienceFlag);
                        }
                        bitstream.WriteBits(1, asc.ExtensionFlag3);
                        if (asc.ExtensionFlag3 == 1)
                        {
                            throw new NotImplementedException();
                        }
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }

            if (asc.OuterSyncExtensionType >= 0)
            {
                bitstream.WriteBits(11, asc.OuterSyncExtensionType);
                if (asc.OuterSyncExtensionType == 0x2b7)
                {
                    WriteAudioObjectType(bitstream, asc.ExtensionAudioObjectType);
                    if (asc.ExtensionAudioObjectType == 5)
                    {
                        bitstream.WriteBit(asc.SbrPresentFlag);
                        if (asc.SbrPresentFlag)
                        {
                            bitstream.WriteBits(4, asc.ExtensionSamplingFrequencyIndex);
                            if (asc.ExtensionSamplingFrequencyIndex == 0xf)
                            {
                                bitstream.WriteBits(24, asc.ExtensionSamplingFrequency);
                            }
                            if (asc.InnerSyncExtensionType >= 0)
                            {
                                bitstream.WriteBits(11, asc.InnerSyncExtensionType);
                                if (asc.SyncExtensionType == 0x548)
                                {
                                    bitstream.WriteBit(asc.PsPresentFlag);
                                }
                            }
                        }
                    }

                    if (asc.ExtensionAudioObjectType == 22)
                    {
                        bitstream.WriteBit(asc.SbrPresentFlag);
                        if (asc.SbrPresentFlag)
                        {
                            bitstream.WriteBits(4, asc.ExtensionSamplingFrequencyIndex);
                            if (asc.ExtensionSamplingFrequencyIndex == 0xf)
                            {
                                bitstream.WriteBits(24, asc.ExtensionSamplingFrequency);
                            }
                        }
                        bitstream.WriteBits(4, asc.ExtensionChannelConfiguration);
                    }
                }
            }

            bitstream.Flush();
            size += bitstream.WrittenBytes;

            return Task.FromResult(size);
        }

        private static int ReadAudioObjectType(BitStreamReader bitstream)
        {
            int audioObjectType = bitstream.ReadBits(5);
            if (audioObjectType == 31)
            {
                audioObjectType = 32 + bitstream.ReadBits(6);
            }
            return audioObjectType;
        }

        private static void WriteAudioObjectType(BitStreamWriter bitstream, int audioObjectType)
        {
            if (audioObjectType >= 32)
            {
                bitstream.WriteBits(5, 31);
                bitstream.WriteBits(6, audioObjectType - 32);
            }
            else
            {
                bitstream.WriteBits(5, audioObjectType);
            }
        }
    }

    public class DecoderSpecificInfoDescriptor : DescriptorBase
    {
        public const int DESCRIPTOR = 0x5;
        public const int OBJECT_TYPE_INDICATION = -1;

        public DecoderSpecificInfoDescriptor(byte[] bytes) : base(OBJECT_TYPE_INDICATION, DESCRIPTOR)
        {
            Bytes = bytes;
        }

        // currently not needed
        public byte[] Bytes { get; set; }

        public static async Task<DescriptorBase> ParseAsync(uint size, Stream stream)
        {
            byte[] buffer = new byte[size];
            await IsoReaderWriter.ReadBytesAsync(stream, buffer, 0, (int)size);
            DecoderSpecificInfoDescriptor decoderSpecificInfo = new DecoderSpecificInfoDescriptor(buffer);
            return decoderSpecificInfo;
        }

        public static async Task<uint> BuildAsync(Stream stream, int objectIndicationIdentifier, byte tag, DescriptorBase descriptor)
        {
            DecoderSpecificInfoDescriptor b = (DecoderSpecificInfoDescriptor)descriptor;
            uint size = 0;
            size += await IsoReaderWriter.WriteBytesAsync(stream, b.Bytes, 0, b.Bytes.Length);
            return size;
        }

        public override uint CalculateSize()
        {
            return (uint)Bytes.Length;
        }
    }

    public class ProfileLevelIndicationDescriptor : DescriptorBase
    {
        public const int DESCRIPTOR = 0x14;
        public const int OBJECT_TYPE_INDICATION = -1;

        public ProfileLevelIndicationDescriptor(byte[] bytes) : base(OBJECT_TYPE_INDICATION, DESCRIPTOR)
        {
            Bytes = bytes;
        }

        // currently not needed
        public byte[] Bytes { get; set; }

        public static async Task<DescriptorBase> ParseAsync(uint size, Stream stream)
        {
            byte[] buffer = new byte[size];
            await IsoReaderWriter.ReadBytesAsync(stream, buffer, 0, (int)size);
            ProfileLevelIndicationDescriptor profileLevelIndication = new ProfileLevelIndicationDescriptor(buffer);
            return profileLevelIndication;
        }

        public static async Task<uint> BuildAsync(Stream stream, int objectIndicationIdentifier, byte tag, DescriptorBase descriptor)
        {
            ProfileLevelIndicationDescriptor b = (ProfileLevelIndicationDescriptor)descriptor;
            uint size = 0;
            size += await IsoReaderWriter.WriteBytesAsync(stream, b.Bytes, 0, b.Bytes.Length);
            return size;
        }

        public override uint CalculateSize()
        {
            return (uint)Bytes.Length;
        }
    }

    public abstract class DescriptorBase
    {
        public byte DescriptorType { get; }

        public int ObjectTypeIndication { get; }

        private uint _originalSize;
        public uint GetSize() { return _originalSize; }
        public void SetSize(uint size) { _originalSize = size; }

        public abstract uint CalculateSize();

        public DescriptorBase(int objectTypeIndication, byte descriptorType)
        {
            ObjectTypeIndication = objectTypeIndication;
            DescriptorType = descriptorType;
        }
    }

    /// <summary>
    /// Fragmented MP4.
    /// </summary>
    /// <remarks>https://stackoverflow.com/questions/35177797/what-exactly-is-fragmented-mp4fmp4-how-is-it-different-from-normal-mp4</remarks>
    public class FragmentedMp4
    {
        public FtypBox GetFtyp() { return Children.SingleOrDefault(x => x.Type == FtypBox.TYPE) as FtypBox; } 
        public MoovBox GetMoov() { return Children.SingleOrDefault(x => x.Type == MoovBox.TYPE) as MoovBox; } 
        public IEnumerable<MoofBox> GetMoof() { return Children.Where(x => x.Type == MoofBox.TYPE).Select(x => (MoofBox)x); } 
        public IEnumerable<MdatBox> GetMdat() { return Children.Where(x => x.Type == MdatBox.TYPE).Select(x => (MdatBox)x); } 

        public List<Mp4Box> Children { get; set; } = new List<Mp4Box>();

        public static async Task<FragmentedMp4> ParseAsync(Stream stream)
        {
            FragmentedMp4 fmp4 = new FragmentedMp4();
            while (stream.Position < stream.Length)
            {
                var box = await Mp4Parser.ReadBox(null, stream);
                fmp4.Children.Add(box);
            }
            return fmp4;
        }

        public static async Task BuildAsync(FragmentedMp4 fmp4, Stream stream)
        {
            foreach (var child in fmp4.Children)
            {
                await Mp4Parser.WriteBox(stream, child);
            }
            await stream.FlushAsync();
        }
    }

    /// <summary>
    /// MP4 Parser.
    /// </summary>
    /// <remarks>https://atomicparsley.sourceforge.net/mpeg-4files.html</remarks>
    public static class Mp4Parser
    {
        private static Dictionary<string, Func<uint, string, Mp4Box, Stream, Task<Mp4Box>>> _boxParsers = new Dictionary<string, Func<uint, string, Mp4Box, Stream, Task<Mp4Box>>>();
        private static Dictionary<string, Func<Mp4Box, Stream, Task<uint>>> _boxBuilders = new Dictionary<string, Func<Mp4Box, Stream, Task<uint>>>();

        private static Dictionary<int, Dictionary<int, Func<uint, Stream, Task<DescriptorBase>>>> _descriptorParsers = new Dictionary<int, Dictionary<int, Func<uint, Stream, Task<DescriptorBase>>>>();
        private static Dictionary<int, Dictionary<int, Func<Stream, int, byte, DescriptorBase, Task<uint>>>> _descriptorBuilders = new Dictionary<int, Dictionary<int, Func<Stream, int, byte, DescriptorBase, Task<uint>>>>();

        static Mp4Parser()
        {
            // box parsers
            _boxParsers.Add(DinfBox.TYPE, DinfBox.ParseAsync);
            _boxParsers.Add(DrefBox.TYPE, DrefBox.ParseAsync);
            _boxParsers.Add(FtypBox.TYPE, FtypBox.ParseAsync);
            _boxParsers.Add(HdlrBox.TYPE, HdlrBox.ParseAsync);
            _boxParsers.Add(MdatBox.TYPE, MdatBox.ParseAsync);
            _boxParsers.Add(MdhdBox.TYPE, MdhdBox.ParseAsync);
            _boxParsers.Add(MdiaBox.TYPE, MdiaBox.ParseAsync);
            _boxParsers.Add(MehdBox.TYPE, MehdBox.ParseAsync);
            _boxParsers.Add(MfhdBox.TYPE, MfhdBox.ParseAsync);
            _boxParsers.Add(MinfBox.TYPE, MinfBox.ParseAsync);
            _boxParsers.Add(MoofBox.TYPE, MoofBox.ParseAsync);
            _boxParsers.Add(MoovBox.TYPE, MoovBox.ParseAsync);
            _boxParsers.Add(MvexBox.TYPE, MvexBox.ParseAsync);
            _boxParsers.Add(MvhdBox.TYPE, MvhdBox.ParseAsync);
            _boxParsers.Add(SmhdBox.TYPE, SmhdBox.ParseAsync);
            _boxParsers.Add(StblBox.TYPE, StblBox.ParseAsync);
            _boxParsers.Add(StcoBox.TYPE, StcoBox.ParseAsync);
            _boxParsers.Add(StscBox.TYPE, StscBox.ParseAsync);
            _boxParsers.Add(StsdBox.TYPE, StsdBox.ParseAsync);
            _boxParsers.Add(StszBox.TYPE, StszBox.ParseAsync);
            _boxParsers.Add(SttsBox.TYPE, SttsBox.ParseAsync);
            _boxParsers.Add(TfdtBox.TYPE, TfdtBox.ParseAsync);
            _boxParsers.Add(TfhdBox.TYPE, TfhdBox.ParseAsync);
            _boxParsers.Add(TkhdBox.TYPE, TkhdBox.ParseAsync);
            _boxParsers.Add(TrafBox.TYPE, TrafBox.ParseAsync);
            _boxParsers.Add(TrakBox.TYPE, TrakBox.ParseAsync);
            _boxParsers.Add(TrexBox.TYPE, TrexBox.ParseAsync);
            _boxParsers.Add(TrunBox.TYPE, TrunBox.ParseAsync);
            _boxParsers.Add(VmhdBox.TYPE, VmhdBox.ParseAsync);
            _boxParsers.Add(UrlBox.TYPE, UrlBox.ParseAsync);

            _boxParsers.Add(VisualSampleEntryBox.TYPE1, VisualSampleEntryBox.ParseAsync);
            _boxParsers.Add(VisualSampleEntryBox.TYPE2, VisualSampleEntryBox.ParseAsync);
            _boxParsers.Add(VisualSampleEntryBox.TYPE3, VisualSampleEntryBox.ParseAsync);
            _boxParsers.Add(VisualSampleEntryBox.TYPE4, VisualSampleEntryBox.ParseAsync);
            _boxParsers.Add(VisualSampleEntryBox.TYPE5, VisualSampleEntryBox.ParseAsync);
            _boxParsers.Add(VisualSampleEntryBox.TYPE6, VisualSampleEntryBox.ParseAsync);
            _boxParsers.Add(VisualSampleEntryBox.TYPE7, VisualSampleEntryBox.ParseAsync);
            _boxParsers.Add(VisualSampleEntryBox.TYPE_ENCRYPTED, VisualSampleEntryBox.ParseAsync);

            _boxParsers.Add(AudioSampleEntryBox.TYPE1, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE2, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE3, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE4, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE5, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE6, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE7, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE8, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE9, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE10, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE11, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE12, AudioSampleEntryBox.ParseAsync);
            _boxParsers.Add(AudioSampleEntryBox.TYPE_ENCRYPTED, AudioSampleEntryBox.ParseAsync);

            _boxParsers.Add(AvcConfigurationBox.TYPE, AvcConfigurationBox.ParseAsync);
            _boxParsers.Add(HevcConfigurationBox.TYPE, HevcConfigurationBox.ParseAsync);
            _boxParsers.Add(EsdsBox.TYPE, EsdsBox.ParseAsync);

            // descriptor parsers
            Dictionary<int, Func<uint, Stream, Task<DescriptorBase>>> audioParserDictionary = new Dictionary<int, Func<uint, Stream, Task<DescriptorBase>>>();
            audioParserDictionary.Add(AudioSpecificConfigDescriptor.DESCRIPTOR, AudioSpecificConfigDescriptor.ParseAsync);
            _descriptorParsers.Add(AudioSpecificConfigDescriptor.OBJECT_TYPE_INDICATION, audioParserDictionary);
            Dictionary<int, Func<uint, Stream, Task<DescriptorBase>>> defaultParserDictionary = new Dictionary<int, Func<uint, Stream, Task<DescriptorBase>>>();
            defaultParserDictionary.Add(ESDescriptor.DESCRIPTOR, ESDescriptor.ParseAsync);
            defaultParserDictionary.Add(SLConfigDescriptor.DESCRIPTOR, SLConfigDescriptor.ParseAsync);
            defaultParserDictionary.Add(DecoderConfigDescriptor.DESCRIPTOR, DecoderConfigDescriptor.ParseAsync);
            defaultParserDictionary.Add(ProfileLevelIndicationDescriptor.DESCRIPTOR, ProfileLevelIndicationDescriptor.ParseAsync);
            defaultParserDictionary.Add(DecoderSpecificInfoDescriptor.DESCRIPTOR, DecoderSpecificInfoDescriptor.ParseAsync);
            _descriptorParsers.Add(-1, defaultParserDictionary);

            // box builders
            _boxBuilders.Add(DinfBox.TYPE, DinfBox.BuildAsync);
            _boxBuilders.Add(DrefBox.TYPE, DrefBox.BuildAsync);
            _boxBuilders.Add(FtypBox.TYPE, FtypBox.BuildAsync);
            _boxBuilders.Add(HdlrBox.TYPE, HdlrBox.BuildAsync);
            _boxBuilders.Add(MdatBox.TYPE, MdatBox.BuildAsync);
            _boxBuilders.Add(MdhdBox.TYPE, MdhdBox.BuildAsync);
            _boxBuilders.Add(MdiaBox.TYPE, MdiaBox.BuildAsync);
            _boxBuilders.Add(MehdBox.TYPE, MehdBox.BuildAsync);
            _boxBuilders.Add(MfhdBox.TYPE, MfhdBox.BuildAsync);
            _boxBuilders.Add(MinfBox.TYPE, MinfBox.BuildAsync);
            _boxBuilders.Add(MoofBox.TYPE, MoofBox.BuildAsync);
            _boxBuilders.Add(MoovBox.TYPE, MoovBox.BuildAsync);
            _boxBuilders.Add(MvexBox.TYPE, MvexBox.BuildAsync);
            _boxBuilders.Add(MvhdBox.TYPE, MvhdBox.BuildAsync);
            _boxBuilders.Add(SmhdBox.TYPE, SmhdBox.BuildAsync);
            _boxBuilders.Add(StblBox.TYPE, StblBox.BuildAsync);
            _boxBuilders.Add(StcoBox.TYPE, StcoBox.BuildAsync);
            _boxBuilders.Add(StscBox.TYPE, StscBox.BuildAsync);
            _boxBuilders.Add(StsdBox.TYPE, StsdBox.BuildAsync);
            _boxBuilders.Add(StszBox.TYPE, StszBox.BuildAsync);
            _boxBuilders.Add(SttsBox.TYPE, SttsBox.BuildAsync);
            _boxBuilders.Add(TfdtBox.TYPE, TfdtBox.BuildAsync);
            _boxBuilders.Add(TfhdBox.TYPE, TfhdBox.BuildAsync);
            _boxBuilders.Add(TkhdBox.TYPE, TkhdBox.BuildAsync);
            _boxBuilders.Add(TrafBox.TYPE, TrafBox.BuildAsync);
            _boxBuilders.Add(TrakBox.TYPE, TrakBox.BuildAsync);
            _boxBuilders.Add(TrexBox.TYPE, TrexBox.BuildAsync);
            _boxBuilders.Add(TrunBox.TYPE, TrunBox.BuildAsync);
            _boxBuilders.Add(VmhdBox.TYPE, VmhdBox.BuildAsync);
            _boxBuilders.Add(UrlBox.TYPE, UrlBox.BuildAsync);

            _boxBuilders.Add(VisualSampleEntryBox.TYPE1, VisualSampleEntryBox.BuildAsync);
            _boxBuilders.Add(VisualSampleEntryBox.TYPE2, VisualSampleEntryBox.BuildAsync);
            _boxBuilders.Add(VisualSampleEntryBox.TYPE3, VisualSampleEntryBox.BuildAsync);
            _boxBuilders.Add(VisualSampleEntryBox.TYPE4, VisualSampleEntryBox.BuildAsync);
            _boxBuilders.Add(VisualSampleEntryBox.TYPE5, VisualSampleEntryBox.BuildAsync);
            _boxBuilders.Add(VisualSampleEntryBox.TYPE6, VisualSampleEntryBox.BuildAsync);
            _boxBuilders.Add(VisualSampleEntryBox.TYPE7, VisualSampleEntryBox.BuildAsync);
            _boxBuilders.Add(VisualSampleEntryBox.TYPE_ENCRYPTED, VisualSampleEntryBox.BuildAsync);

            _boxBuilders.Add(AudioSampleEntryBox.TYPE1, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE2, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE3, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE4, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE5, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE6, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE7, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE8, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE9, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE10, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE11, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE12, AudioSampleEntryBox.BuildAsync);
            _boxBuilders.Add(AudioSampleEntryBox.TYPE_ENCRYPTED, AudioSampleEntryBox.BuildAsync);

            _boxBuilders.Add(AvcConfigurationBox.TYPE, AvcConfigurationBox.BuildAsync);
            _boxBuilders.Add(HevcConfigurationBox.TYPE, HevcConfigurationBox.BuildAsync);
            _boxBuilders.Add(EsdsBox.TYPE, EsdsBox.BuildAsync);

            // descriptor builders
            Dictionary<int, Func<Stream, int, byte, DescriptorBase, Task<uint>>> audioBuilderDictionary = new Dictionary<int, Func<Stream, int, byte, DescriptorBase, Task<uint>>>();
            audioBuilderDictionary.Add(AudioSpecificConfigDescriptor.DESCRIPTOR, AudioSpecificConfigDescriptor.BuildAsync);
            _descriptorBuilders.Add(AudioSpecificConfigDescriptor.OBJECT_TYPE_INDICATION, audioBuilderDictionary);
            Dictionary<int, Func<Stream, int, byte, DescriptorBase, Task<uint>>> defaultBuilderDictionary = new Dictionary<int, Func<Stream, int, byte, DescriptorBase, Task<uint>>>();
            defaultBuilderDictionary.Add(ESDescriptor.DESCRIPTOR, ESDescriptor.BuildAsync);
            defaultBuilderDictionary.Add(SLConfigDescriptor.DESCRIPTOR, SLConfigDescriptor.BuildAsync);
            defaultBuilderDictionary.Add(DecoderConfigDescriptor.DESCRIPTOR, DecoderConfigDescriptor.BuildAsync);
            defaultBuilderDictionary.Add(ProfileLevelIndicationDescriptor.DESCRIPTOR, ProfileLevelIndicationDescriptor.BuildAsync);
            defaultBuilderDictionary.Add(DecoderSpecificInfoDescriptor.DESCRIPTOR, DecoderSpecificInfoDescriptor.BuildAsync);
            _descriptorBuilders.Add(-1, defaultBuilderDictionary);
        }

        public static async Task<Mp4Box> ReadBox(Mp4Box parent, Stream stream)
        {
            uint size = IsoReaderWriter.ReadUInt32(stream);
            string type = IsoReaderWriter.Read4cc(stream);

            if (Log.InfoEnabled)
            {
                StringBuilder log = new StringBuilder();
                Mp4Box p = parent;
                while (p != null)
                {
                    log.Append("-");
                    p = p.GetParent();
                }
                log.Append($" {type}, size {size}, Position {stream.Position - 8}");
                Log.Info(log.ToString());
            }

            Mp4Box box;
            if (_boxParsers.ContainsKey(type))
            {
                box = await _boxParsers[type].Invoke(size, type, parent, stream);
            }
            else
            {
                if (Log.WarnEnabled) Log.Warn($"--- {type} is unknown.");
                box = await UnknownBox.ParseAsync(size, type, parent, stream);
            }

            return box;
        }

        public static async Task<uint> WriteBox(Stream stream, Mp4Box box)
        {
            Mp4Box p = box.GetParent();

            if (Log.InfoEnabled)
            {
                StringBuilder log = new StringBuilder();
                while (p != null)
                {
                    log.Append("-");
                    p = p.GetParent();
                }
                log.Append($" {box.Type}, size {box.CalculateSize()}, Position {stream.Position}");
                Log.Info(log.ToString());
            }

            uint size = 0;

            size += IsoReaderWriter.WriteUInt32(stream, box.CalculateSize());
            size += IsoReaderWriter.Write4cc(stream, box.Type);

            if (_boxBuilders.ContainsKey(box.Type))
            {
                size += await _boxBuilders[box.Type].Invoke(box, stream);
            }
            else if (box is UnknownBox)
            {
                size += await UnknownBox.BuildAsync(box, stream);
            }
            else
            {
                throw new NotSupportedException($"Writing box {box.Type} is not supported!");
            }

            uint calculatedSize = box.CalculateSize();
            if (size != calculatedSize)
            {
                throw new Exception($"Box {box.Type} size mismatch!");
            }

            return size;
        }

        public static async Task<DescriptorBase> ReadDescriptor(int objectTypeIndication, uint size, Stream stream)
        {
            List<DescriptorBase> descriptors = new List<DescriptorBase>();
            int tag = IsoReaderWriter.ReadByte(stream);

            var descriptorSize = IsoReaderWriter.ReadPackedNumber(stream);

            DescriptorBase descriptor = null;

            if (_descriptorParsers[objectTypeIndication].ContainsKey(tag))
            {
                if (Log.InfoEnabled) Log.Info($"-------- Descriptor - objectTypeIndication: {objectTypeIndication}, tag {tag}");
                descriptor = await _descriptorParsers[objectTypeIndication][tag].Invoke((uint)descriptorSize.sizeOfInstance, stream);
            }
            else
            {
                if (Log.WarnEnabled) Log.Warn($"-------- Unknown Descriptor - objectTypeIndication: {objectTypeIndication}, tag {tag}");
                descriptor = await UnknownDescriptor.ParseAsync((uint)descriptorSize.sizeOfInstance, stream, objectTypeIndication, (byte)tag);
            }

            descriptor.SetSize((uint)(1 + descriptorSize.sizeBytes + descriptorSize.sizeOfInstance));

            return descriptor;
        }

        public static async Task<uint> WriteDescriptor(Stream stream, int objectTypeIndication, byte tag, DescriptorBase descriptor)
        {
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, tag);

            uint descriptorSize = descriptor.CalculateSize();
            size += IsoReaderWriter.WritePackedNumber(stream, descriptorSize);

            if (Log.InfoEnabled) Log.Info($"-------- Descriptor - objectTypeIndication: {objectTypeIndication}, tag {tag}");
            if (_descriptorBuilders.ContainsKey(objectTypeIndication) && _descriptorBuilders[objectTypeIndication].ContainsKey(tag))
            {
                size += await _descriptorBuilders[objectTypeIndication][tag].Invoke(stream, objectTypeIndication, tag, descriptor);
            }
            else if (descriptor is UnknownDescriptor)
            {
                size += await UnknownDescriptor.BuildAsync(stream, objectTypeIndication, tag, descriptor);
            }
            else
            {
                if (Log.ErrorEnabled) Log.Error($"Descriptor {objectTypeIndication}, {tag} is not supported!");
            }

            uint calculatedSize = descriptor.CalculateSize() + IsoReaderWriter.CalculatePackedNumberLength(descriptorSize) + 1;
            if (size != calculatedSize)
            {
                if (Log.WarnEnabled) Log.Warn($"Descriptor size mismatch! {objectTypeIndication}, tag {tag}, {(int)size - (int)calculatedSize} bytes");
            }

            return size;
        }
    }


    public struct StreamSample
    {
        public byte[] Sample { get; set; }
        public uint Duration { get; set; }
    }

    public class StreamFragment
    {
        public StreamFragment(List<TrackBase> tracks, ulong timescale, ulong _lastFragmentEndTime)
        {
            Timescale = timescale;
            StartTime = _lastFragmentEndTime;

            for (int i = 0; i < tracks.Count; i++)
            {
                Samples.Add(tracks[i], new List<StreamSample>());
            }
        }

        public Dictionary<TrackBase, List<StreamSample>> Samples { get; } = new Dictionary<TrackBase, List<StreamSample>>();
        public ulong Timescale { get; internal set; }
        public ulong StartTime { get; internal set; }
    }

    public abstract class TrackBase
    {
        private ulong _nextFragmentCreateStartTime = 0;
        public uint Timescale { get; set; }
        public uint TrackID { get; set; } = 1;
        public string CompatibleBrand { get; set; } = null;
        public string Language { get; set; } = "und";
        public string Handler { get; protected set; }

        public ConcurrentQueue<StreamSample> _samples = new ConcurrentQueue<StreamSample>();
        private long _queuedSamplesLength = 0;
        private FragmentedMp4Builder _sink;

        public TrackBase(uint trackID)
        {
            if (trackID == 0)
                throw new ArgumentException("TrackID must start at 1!");

            TrackID = trackID;
        }

        public virtual async Task ProcessSampleAsync(byte[] sample, uint duration)
        {
            _nextFragmentCreateStartTime = _nextFragmentCreateStartTime + duration;
            Interlocked.Add(ref _queuedSamplesLength, duration);

            if (Log.DebugEnabled) Log.Debug($"{this.Handler}: {_nextFragmentCreateStartTime / (double)Timescale}");

            var s = new StreamSample()
            {
                Sample = sample,
                Duration = duration
            };
            _samples.Enqueue(s);

            await _sink.NotifySampleAdded(this);
        }
       
        public static string ToHexString(byte[] data)
        {
#if !NETCOREAPP
            string hexString = BitConverter.ToString(data);
            hexString = hexString.Replace("-", "");
            return hexString;
#else
            return Convert.ToHexString(data);
#endif
        }

        internal StreamSample ReadSample()
        {
            if (_samples.TryDequeue(out var a))
            {
                Interlocked.Add(ref _queuedSamplesLength, -1 * a.Duration);
                return a;
            }

            throw new Exception();
        }

        internal void SetSink(FragmentedMp4Builder fmp4)
        {
            this._sink = fmp4;
        }

        internal bool ContainsEnoughSamples(double durationInSeconds)
        {
            return _queuedSamplesLength > durationInSeconds * Timescale;
        }

        internal bool HasSamples()
        {
            return _samples.Count > 0;
        }
    }

    public class FragmentedMp4Builder
    {
        private Stream _output;
        private double _maxFragmentLengthInSeconds;
        private int _maxFragmentsPerMoof;
        private ulong _lcm = 1000;
        private uint _sequenceNumber = 1;
        private ulong _lastFragmentEndTime = 0;

        public FragmentedMp4Builder(Stream output, double maxFragmentLengthInSeconds, int maxFragmentsPerMoof)
        {
            this._output = output;
            this._maxFragmentLengthInSeconds = maxFragmentLengthInSeconds;
            this._maxFragmentsPerMoof = maxFragmentsPerMoof;
        }

        private readonly List<TrackBase> _tracks = new List<TrackBase>();

        public void AddTrack(TrackBase track)
        {
            _tracks.Add(track);
            track.SetSink(this);
            _lcm = (ulong)Mp4Math.LCM(_tracks.Select(x => (long)x.Timescale).ToArray());
        }

        internal async Task NotifySampleAdded(TrackBase track)
        {
            double duration = _maxFragmentLengthInSeconds * _maxFragmentsPerMoof;
            if (!track.ContainsEnoughSamples(duration))
                return;

            for (int i = 1; i < _tracks.Count; i++)
            {
                if (!_tracks[i].ContainsEnoughSamples(duration))
                    return;
            }

            FragmentedMp4 fmp4 = new FragmentedMp4();

            // first check if we need to produce the media initialization segment
            if (_sequenceNumber == 1)
            {
                await CreateMediaInitialization(fmp4);
                await FragmentedMp4.BuildAsync(fmp4, _output);

                fmp4 = new FragmentedMp4();
            }

            // all tracks have enough samples to produce a fragment
            List<StreamFragment> fragments = new List<StreamFragment>();
            for (int j = 0; j < _maxFragmentsPerMoof; j++)
            {
                var fragment = new StreamFragment(_tracks, _lcm, _lastFragmentEndTime);
                long totalDuration = 0;

                for (int i = 0; i < _tracks.Count; i++)
                {
                    double targetDuration = _tracks[i].Timescale * _maxFragmentLengthInSeconds;
                    long currentDuration = 0;

                    while (currentDuration < targetDuration && _tracks[i].HasSamples())
                    {
                        var sample = _tracks[i].ReadSample();
                        currentDuration += sample.Duration;
                        fragment.Samples[_tracks[i]].Add(sample);
                    }

                    if (i == 0)
                    {
                        totalDuration += currentDuration;
                    }
                }

                _lastFragmentEndTime = _lastFragmentEndTime + ((ulong)totalDuration * _lcm / _tracks[0].Timescale);

                fragments.Add(fragment);
            }

            await CreateMediaFragment(fmp4, fragments, _sequenceNumber++);
            await FragmentedMp4.BuildAsync(fmp4, _output);
        }

        private async Task CreateMediaInitialization(FragmentedMp4 fmp4)
        {
            var ftyp = CreateFtypBox();
            fmp4.Children.Add(ftyp);

            for (int i = 0; i < _tracks.Count; i++)
            {
                if (!string.IsNullOrEmpty(_tracks[i].CompatibleBrand))
                {
                    ftyp.CompatibleBrands.Add(_tracks[i].CompatibleBrand);
                }
            }

            var moov = CreateMoovBox();
            fmp4.Children.Add(moov);
        }

        private async Task CreateMediaFragment(FragmentedMp4 fmp4, List<StreamFragment> fragments, uint sequenceNumber)
        {
            var moof = CreateMoofBox(sequenceNumber, fragments);
            fmp4.Children.Add(moof);

            var mdat = await CreateMdatBox(fragments);
            fmp4.Children.Add(mdat);
        }

        private async Task<MdatBox> CreateMdatBox(List<StreamFragment> fragments)
        {
            var storage = TemporaryStorage.Factory.Create();
            var mdat = new MdatBox(0, null, storage);

            for (int i = 0; i < fragments.Count; i++)
            {
                foreach(var track in fragments[i].Samples)
                {
                    for (int k = 0; k < track.Value.Count; k++)
                    {
                        var sample = track.Value[k];
                        await storage.Stream.WriteAsync(sample.Sample, 0, sample.Sample.Length);
                    }
                } 
            }

            return mdat;
        }

        private static MoofBox CreateMoofBox(uint sequenceNumber, List<StreamFragment> fragments)
        {
            MoofBox moof = new MoofBox(0, null);

            MfhdBox mfhd = CreateMfhdBox(moof, sequenceNumber);
            moof.Children.Add(mfhd);

            List<TrafBox> trafs = new List<TrafBox>();
            var tracks = fragments[0].Samples.Keys.ToList();
            for (int i = 0; i < tracks.Count; i++)
            {        
                TrafBox traf = CreateTrafBox(moof, tracks[i], fragments, i == 0);
                moof.Children.Add(traf);
                trafs.Add(traf);
            }

            long offset = moof.CalculateSize() + 8;

            for(int j = 0; j < trafs[0].GetTrun().Count(); j++)
            {
                for(int i = 0; i < trafs.Count; i++)
                {
                    var trun = trafs[i].GetTrun().ElementAt(j);
                    trun.DataOffset = (int)offset;
                    offset += (int)trun.Entries.Sum(x => x.SampleSize);
                }
            }

            return moof;
        }

        private static MfhdBox CreateMfhdBox(Mp4Box parent, uint sequenceNumber)
        {
            MfhdBox mfhd = new MfhdBox(0, parent);
            mfhd.SequenceNumber = sequenceNumber;
            return mfhd;
        }

        private static TrafBox CreateTrafBox(Mp4Box parent, TrackBase track, List<StreamFragment> fragments, bool isFirst)
        {
            TrafBox traf = new TrafBox(0, parent);
            TfhdBox tfhd = CreateTfhdBox(traf, track);
            traf.Children.Add(tfhd);
            TfdtBox tfdt = CreateTfdtBox(traf, track, fragments);
            traf.Children.Add(tfdt);

            for (int i = 0; i < fragments.Count; i++)
            {
                TrunBox trun = CreateTrunBox(traf, track, fragments[i], isFirst && i == 0);
                traf.Children.Add(trun);
            }
            return traf;
        }

        private static TfhdBox CreateTfhdBox(Mp4Box parent, TrackBase track)
        {
            TfhdBox tfhd = new TfhdBox(0, parent);
            tfhd.TrackId = track.TrackID;
            // TODO: DefaultSampleFlags
            if (track.TrackID == 1)
            {
                tfhd.Flags = 131104;
                tfhd.DefaultSampleFlags = 16842752;
            }
            else
            {
                tfhd.Flags = 131072;
            }
            tfhd.DefaultBaseIsMoof = true;
            return tfhd;
        }

        private static TfdtBox CreateTfdtBox(Mp4Box parent, TrackBase track, List<StreamFragment> fragments)
        {
            TfdtBox tfdt = new TfdtBox(0, parent);
            tfdt.BaseMediaDecodeTime = fragments[0].StartTime * track.Timescale / fragments[0].Timescale; // BaseMediaDecodeTime must be in the timescale of the track
            return tfdt;
        }

        private static TrunBox CreateTrunBox(Mp4Box parent, TrackBase track, StreamFragment fragment, bool isFirst)
        {
            TrunBox trun = new TrunBox(0, parent);

            // TODO
            if(isFirst)
            {
                trun.FirstSampleFlags = 33554432;
            }
            trun.DataOffset = 0;
            trun.Flags = (uint)(isFirst ? 773 : 769);

            for (int i = 0; i < fragment.Samples[track].Count; i++)
            {
                var sample = fragment.Samples[track][i];
                trun.Entries.Add(new TrunBox.Entry(sample.Duration, (uint)sample.Sample.Length, 0, 0));
            }            

            return trun;
        }

        public static FtypBox CreateFtypBox()
        {
            var compatibleBrands = new List<string>()
            {
                "isom",
                "mp42"
            };
            var ftyp = new FtypBox(0, null, "mp42", 1, compatibleBrands);
            return ftyp;
        }

        private MoovBox CreateMoovBox()
        {
            var moov = new MoovBox(0, null);
            var mvhd = CreateMvhdBox(moov);
            moov.Children.Add(mvhd);

            for (int i = 0; i < this._tracks.Count; i++)
            { 
                var trak = CreateTrakBox(moov, this._tracks[i]);
                moov.Children.Add(trak);
            }

            var mvex = CreateMvexBox(moov);
            moov.Children.Add(mvex);

            return moov;
        }

        private MvhdBox CreateMvhdBox(Mp4Box parent)
        {
            var mvhd = new MvhdBox(0, parent);
            mvhd.Duration = 0;
            mvhd.Timescale = 1000; // just for movie time: https://stackoverflow.com/questions/77803940/diffrence-between-mvhd-box-timescale-and-mdhd-box-timescale-in-isobmff-format
            return mvhd;
        }

        private TrakBox CreateTrakBox(Mp4Box parent, TrackBase track)
        {
            var trak = new TrakBox(0, parent);
            var tkhd = CreateTkhdBox(trak, track);
            var mdia = CreateMdiaBox(trak, track);
            trak.Children.Add(tkhd);
            trak.Children.Add(mdia);
            return trak;
        }

        private TkhdBox CreateTkhdBox(Mp4Box parent, TrackBase track)
        {
            var tkhd = new TkhdBox(0, parent);
            tkhd.TrackId = track.TrackID;

            if(track is H264Track h264Track)
            {
                var dim = h264Track.Sps.FirstOrDefault().Value.CalculateDimensions();
                tkhd.Width = dim.Width;
                tkhd.Height = dim.Height;
            }
            else if (track is H265Track h265Track)
            {
                var dim = h265Track.Sps.FirstOrDefault().Value.CalculateDimensions();
                tkhd.Width = dim.Width;
                tkhd.Height = dim.Height;
            }
            else if(track is AACTrack aacTrack)
            {
                tkhd.Volume = 1;
            }
            else
            {
                throw new NotSupportedException();
            }

            return tkhd;
        }

        private MdiaBox CreateMdiaBox(Mp4Box parent, TrackBase track)
        {
            var mdia = new MdiaBox(0, parent);
            var mdhd = CreateMdhdBox(mdia, track);
            mdia.Children.Add(mdhd);
            var hdlr = CreateHdlrBox(mdia, track);
            mdia.Children.Add(hdlr);
            var minf = CreateMinfBox(mdia, track);
            mdia.Children.Add(minf);
            return mdia;
        }

        private MdhdBox CreateMdhdBox(Mp4Box parent, TrackBase track)
        {
            MdhdBox mdhd = new MdhdBox(0, parent);
            mdhd.Duration = 0;
            mdhd.Timescale = track.Timescale;
            mdhd.Language = track.Language;
            return mdhd;
        }

        private HdlrBox CreateHdlrBox(Mp4Box parent, TrackBase track)
        {
            HdlrBox hdlr = new HdlrBox(0, parent);
            hdlr.HandlerType = track.Handler;

            if(track is H264Track h264Track)
            {
                hdlr.Name = "Video Handler\0";
            }
            else if (track is H265Track h265Track)
            {
                hdlr.Name = "Video Handler\0";
            }
            else if(track is AACTrack aacTrack)
            {
                hdlr.Name = "Sound Handler\0";
            }
            else
            {
                throw new NotSupportedException();
            }

            return hdlr;
        }

        private MinfBox CreateMinfBox(Mp4Box parent, TrackBase track)
        {
            MinfBox minf = new MinfBox(0, parent);
            switch(track.Handler)
            {
                case "vide":
                    {
                        minf.Children.Add(new VmhdBox(0, minf));
                    }
                    break;

                case "soun":
                    {
                        minf.Children.Add(new SmhdBox(0, minf));
                    }
                    break;

                default:
                    throw new NotSupportedException(track.Handler);
            }

            var dinf = CreateDinfBox(minf);
            minf.Children.Add(dinf);

            var stbl = CreateStblBox(minf, track);
            minf.Children.Add(stbl);

            return minf;
        }

        private DinfBox CreateDinfBox(Mp4Box parent)
        {
            DinfBox dinf = new DinfBox(0, parent);
            var dref = new DrefBox(0, dinf);
            dinf.Children.Add(dref);
            var url = new UrlBox(0, dref);
            dref.Children.Add(url);
            return dinf;
        }

        private StblBox CreateStblBox(Mp4Box parent, TrackBase track)
        {
            StblBox stbl = new StblBox(0, parent);

            var stsd = CreateStsdBox(stbl, track);
            stbl.Children.Add(stsd);

            var stsz = CreateStszBox(stbl, track);
            stbl.Children.Add(stsz);

            var stsc = CreateStscBox(stbl, track);
            stbl.Children.Add(stsc);

            var stts = CreateSttsBox(stbl, track);
            stbl.Children.Add(stts);

            var stco = CreateStcoBox(stbl, track);
            stbl.Children.Add(stco);

            return stbl;
        }

        private StsdBox CreateStsdBox(Mp4Box parent, TrackBase track)
        {
            var stsd = new StsdBox(0, parent);
           
            if(track is H264Track h264Track)
            {
                var video = H264BoxBuilder.CreateVisualSampleEntryBox(parent, h264Track);
                stsd.Children.Add(video);
            }
            else if (track is H265Track h265Track)
            {
                var video = H265BoxBuilder.CreateVisualSampleEntryBox(parent, h265Track);
                stsd.Children.Add(video);
            }
            else if (track is AACTrack aacTrack)
            {
                var audio = AACBoxBuilder.CreateAudioSampleEntryBox(parent, aacTrack);
                stsd.Children.Add(audio);
            }
            else
            {
                throw new NotSupportedException();
            }

            return stsd;
        }

        private SttsBox CreateSttsBox(Mp4Box parent, TrackBase track)
        {
            var stts = new SttsBox(0, parent);
            return stts;
        }

        private StscBox CreateStscBox(Mp4Box parent, TrackBase track)
        {
            var stsc = new StscBox(0, parent);
            return stsc;
        }

        private StszBox CreateStszBox(Mp4Box parent, TrackBase track)
        {
            var stsz = new StszBox(0, parent);
            return stsz;
        }

        private StcoBox CreateStcoBox(Mp4Box parent, TrackBase track)
        {
            var stco = new StcoBox(0, parent);
            return stco;
        }

        private MvexBox CreateMvexBox(Mp4Box parent)
        {
            var mvex = new MvexBox(0, parent);
            
            var mehd = new MehdBox(0, mvex);
            mehd.FragmentDuration = 0;
            mvex.Children.Add(mehd);

            for (int i = 0; i < this._tracks.Count; i++)
            {
                var trex = CreateTrexBox(mvex, this._tracks[i]);
                mvex.Children.Add(trex);
            }

            return mvex;
        }

        private Mp4Box CreateTrexBox(Mp4Box parent, TrackBase track)
        {
            TrexBox trex = new TrexBox(0, parent);

            trex.TrackId = track.TrackID;
            trex.DefaultSampleDescriptionIndex = 1;
            trex.DefaultSampleDuration = 0;
            trex.DefaultSampleSize = 0;
            // TODO: trex.A = SampleFlags;

            return trex;
        }
    }

    public static class Mp4Math
    {
        public static long GCD(long a, long b)
        {
            while (b > 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static long LCM(long a, long b)
        {
            return a * (b / GCD(a, b));
        }

        public static long LCM(long[] input)
        {
            long result = input[0];
            for (int i = 1; i < input.Length; i++)
            {
                result = LCM(result, input[i]);
            }
            return result;
        }
    }
}
