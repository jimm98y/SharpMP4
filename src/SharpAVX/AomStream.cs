using SharpMP4.Common;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpAVX
{
    public class AomStream : IDisposable
    {
        private bool _disposedValue;

        public IMp4Logger Logger { get; set; }
        public Bitstream Bitstream { get; set; }

        /// <summary>
        /// Called with each fixed-width element once it is read, to reject a value the syntax does
        /// not allow; null, nothing is checked.
        /// </summary>
        public Action<string, long> Validate { get; set; }

        public AomStream(Bitstream bitstream, IMp4Logger logger)
        {
            this.Bitstream = bitstream;
            this.Logger = logger ?? DefaultMp4Logger.Instance;
        }

        public AomStream(Stream stream, IMp4Logger logger)
            : this(new Bitstream(stream), logger)
        {
        }

        public AomStream(Stream stream)
            : this(stream, null)
        {
        }

        // TODO: support long for GetPosition()?
        public int GetPosition()
        {
            return (int)this.Bitstream.BitsPosition;
        }

        public void Skip(long bits)
        {
            while (this.Bitstream.BitsPosition % 8 != 0)
            {
                ReadBit();
                bits--;
            }

            this.Bitstream.BitsPosition += bits;

            long bytes = (bits >> 3);
            this.Bitstream.BaseStream.Seek(bytes, SeekOrigin.Current);
        }

        #region Bit read/write

        /// <summary>
        /// A byte of a leb128(), read through the bitstream so that get_position() counts it. Read
        /// from the underlying stream, it was not counted: a leb128 inside an OBU's payload -
        /// metadata_type, say - left payloadBits short by its length, and an OBU without
        /// obu_size had that many more trailing bits read, past its end.
        /// </summary>
        private int ReadByte() => (int)ReadBits(8);

        private int ReadBit() => this.Bitstream.ReadBit();

        private long ReadBits(int count)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            long res = 0;
            while (count > 0)
            {
                res <<= 1;
                int u1 = ReadBit();

                if (u1 == -1)
                    return -1;

                res |= (byte)u1;
                count--;
            }

            return res;
        }

        #endregion // Bit read/write

        /// <summary>The number of bytes the last leb128() read (AV2 4.11.6).</summary>
        public int Leb128Bytes { get; private set; }

        public ulong ReadLeb128(out int value, string name)
        {
            // Gathered in 64 bits: up to eight bytes of seven bits each. Gathered in an int, the
            // bytes past the fourth were shifted round into the low bits and the value was garbage.
            long v = 0;
            int Leb128Bytes = 0;
            var raw = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                int leb128_byte = ReadByte();
                raw[i] = (byte)leb128_byte;
                v |= (long)(leb128_byte & 0x7f) << (i * 7);
                Leb128Bytes += 1;
                if((leb128_byte & 0x80) == 0)
                {
                    break;
                }
            }

            value = unchecked((int)v);
            this.Leb128Bytes = Leb128Bytes;
            // Recorded as its bytes too: an eighth byte may say more follow, which the value does not keep
            Array.Resize(ref raw, Leb128Bytes);
            _codedAs = raw;
            LogEnd(name, (ulong)Leb128Bytes << 3, v);
            Validate?.Invoke(name, v);

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
                // (1 << 32) - 1 (4.10.3); as C#'s int, 1 << 32 is 1, and the value was 0
                value = uint.MaxValue;
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
            // Read without logging, then logged once as the signed value; read through
            // ReadUnsignedInt, it was logged twice, first unsigned - two elements for one.
            uint v = (uint)ReadBits(count);
            long signMask = 1L << (count - 1);
            if ((v & signMask) > 0)
                value = (int)(v - 2 * signMask);
            else
                value = (int)v;
            LogEnd(name, (ulong)count, value);
            return (ulong)count;
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
            Validate?.Invoke(name, value);
            return (ulong)count;
        }

        public ulong ReadBytes(int count, out byte[] value, string name)
        {
            byte[] bytes = new byte[count / 8];
            if (this.Bitstream.BitsPosition % 8 == 0)
            {
                // Whole bytes, read as bytes: tile data runs to megabytes
                this.Bitstream.ReadAlignedBytes(bytes, 0, bytes.Length);
            }
            else
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    long bb = ReadBits(8);
                    if (bb == -1)
                        throw new EndOfStreamException();

                    bytes[i] = (byte)bb;
                }
            }
            value = bytes;
            if (Record != null && !string.IsNullOrEmpty(name))
                Record.Add(name, new AomSyntaxValue(0, bytes.Length * 8, value));
            LogEnd(name, (ulong)(bytes.Length * 8), "byte[]");
            return (ulong)(bytes.Length * 8);
        }

        public ulong Read_ns(int count, out uint value, string name)
        {
            // Logged once it is whole: logged as its first w - 1 bits were read, an element with the
            // extra bit showed a bit short, and with the value before the bit.
            int w = (int)Math.Floor(MathEx.Log2(count)) + 1;
            int m = (1 << w) - count;
            uint v = (uint)ReadBits(w - 1);
            if (v < m)
            {
                value = v;
                LogEnd(name, (ulong)(w - 1), value);
                return (ulong)(w - 1);
            }

            int extraBit = ReadBit();
            value = (uint)((v << 1) - m + extraBit);
            LogEnd(name, (ulong)w, value);
            return (ulong)w;
        }        

        /// <summary>ns(n) as an int, as AV2 keeps every element.</summary>
        public ulong Read_ns(int count, out int value, string name)
        {
            ulong size = Read_ns(count, out uint v, name);
            value = (int)v;
            return size;
        }

        /// <summary>uvlc() as an int; a value past what an int holds is not one a header has.</summary>
        public ulong ReadUvlc(out int value, string name)
        {
            ulong size = ReadUvlc(out uint v, name);
            value = checked((int)v);
            return size;
        }

        /// <summary>svlc(), AV2 4.11.4: a uvlc() mapped to 0, 1, -1, 2, -2 and so on.</summary>
        public ulong ReadSvlc(out int value, string name)
        {
            ulong size = ReadUvlc(out uint v, "");
            long half = ((long)v + 1) >> 1;
            value = (int)((v & 1) != 0 ? half : -half);
            LogEnd(name, size, value);
            return size;
        }

        /// <summary>le(n), AV2 4.11.5: an unsigned little-endian number of n bytes, up to 4 of them.</summary>
        public ulong ReadLe(int count, out int value, string name)
        {
            if (count > 4)
                throw new ArgumentOutOfRangeException(nameof(count));
            long t = 0;
            for (int i = 0; i < count; i++)
            {
                long b = ReadBits(8);
                if (b == -1)
                    throw new EndOfStreamException();
                t += b << (i * 8);
            }
            value = unchecked((int)t);
            LogEnd(name, (ulong)count << 3, t);
            return (ulong)count << 3;
        }

        /// <summary>tu(mx), AV2 4.11.9: a number from 0 to mx in truncated unary.</summary>
        public ulong ReadTu(int max, out int value, string name)
        {
            ulong size = 0;
            value = max;
            for (int idx = 0; idx < max; idx++)
            {
                int bit = ReadBit();
                if (bit == -1)
                    throw new EndOfStreamException();
                size++;
                if (bit == 0)
                {
                    value = idx;
                    break;
                }
            }
            LogEnd(name, size, value);
            return size;
        }

        /// <summary>rg(n), AV2 4.11.10: Rice-Golomb, a unary quotient then an n bit remainder; -1 past 32 ones.</summary>
        public ulong ReadRg(int count, out int value, string name)
        {
            ulong size = 0;
            value = -1;
            for (int q = 0; q < 32; q++)
            {
                int bit = ReadBit();
                if (bit == -1)
                    throw new EndOfStreamException();
                size++;
                if (bit == 0)
                {
                    long remainder = ReadBits(count);
                    if (remainder == -1)
                        throw new EndOfStreamException();
                    size += (ulong)count;
                    value = (int)(((long)q << count) + remainder);
                    break;
                }
            }
            LogEnd(name, size, value);
            return size;
        }

        #region Writing

        /// <summary>Reading, where each syntax element read is recorded; null, nothing is.</summary>
        public AomSyntaxRecord Record { get; set; }

        /// <summary>
        /// Writing, the elements as they were read: an element whose value was not changed is written
        /// as it was recorded (<see cref="Pick(string, long, long)"/>).
        /// </summary>
        public AomSyntaxRecord Source
        {
            get => _source;
            set
            {
                _source = value;
                _cursor = 0;
                _occurrences = null;
            }
        }

        private AomSyntaxRecord _source;

        // Written as it was read, the elements come in the order they were recorded: each is the next.
        // Once one does not - a change took another path - each is found by name, as the how-manyth of its
        // name, counted from those taken so far.
        private int _cursor;
        private System.Collections.Generic.Dictionary<string, int> _occurrences;
        private string _pickedName;
        private AomSyntaxValue _picked;

        /// <summary>The recorded occurrence the next element of this name is written as; false, there is none.</summary>
        private bool TakeRecorded(string name, out AomSyntaxValue recorded)
        {
            if (_source == null)
            {
                recorded = default;
                return false;
            }

            if (_occurrences == null)
            {
                if (_cursor < _source.Count && _source.NameAt(_cursor) == name)
                {
                    recorded = _source.ValueAt(_cursor++);
                    return true;
                }

                _occurrences = new System.Collections.Generic.Dictionary<string, int>();
                for (int i = 0; i < _cursor; i++)
                {
                    string taken = _source.NameAt(i);
                    _occurrences.TryGetValue(taken, out int count);
                    _occurrences[taken] = count + 1;
                }
            }

            _occurrences.TryGetValue(name, out int occurrence);
            _occurrences[name] = occurrence + 1;
            return _source.TryGet(name, occurrence, out recorded);
        }

        /// <summary>
        /// The value to write of the next occurrence of an element: where what the writer's state gives
        /// for it is what it gave for the state the element was read into (<paramref name="original"/> is
        /// <paramref name="edited"/>), the value recorded as it was read - what the state alone may not say;
        /// where the state was changed, what it gives now.
        /// </summary>
        public long Pick(string name, long original, long edited)
        {
            bool found = TakeRecorded(name, out var recorded);
            _pickedName = null;
            if (found && original == edited)
            {
                if (recorded.Value != original && RecordOnly != null)
                    RecordOnly[name] = RecordOnly.TryGetValue(name, out int count) ? count + 1 : 1;
                _pickedName = name;
                _picked = recorded;
                return recorded.Value;
            }
            return edited;
        }

        /// <summary>
        /// If set, counts for each element the occurrences written as they were recorded that the state they
        /// were read into gives otherwise: what the state alone does not say, and only the record does.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, int> RecordOnly { get; set; }

        public int Pick(string name, int original, int edited) => unchecked((int)Pick(name, (long)original, (long)edited));

        public uint Pick(string name, uint original, uint edited) => unchecked((uint)Pick(name, (long)original, (long)edited));

        public byte[] Pick(string name, byte[] original, byte[] edited)
        {
            bool found = TakeRecorded(name, out var recorded);
            _pickedName = null;
            bool same = original == edited || original != null && edited != null && original.SequenceEqual(edited);
            if (found && same && recorded.Bytes != null)
                return recorded.Bytes;
            return edited;
        }

        /// <summary>The bits the element just picked took as it was read, if it is written with that value; 0 if not.</summary>
        private int RecordedBits(string name, long value)
        {
            int bits = _pickedName == name && _picked.Value == value ? _picked.Bits : 0;
            _pickedName = null;
            return bits;
        }

        /// <summary>
        /// The bit position writing may not pass - the end of the OBU being written - or -1 for none. A
        /// write past it throws: an OBU written other than it was read would otherwise run on.
        /// </summary>
        public long WriteLimit { get; set; } = -1;

        private void WriteBits(int count, long value)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (WriteLimit >= 0 && this.Bitstream.BitsPosition + count > WriteLimit)
                throw new InvalidOperationException($"Writing {count} bits at {this.Bitstream.BitsPosition} passes the end of the OBU at {WriteLimit}.");
            for (int bit = count - 1; bit >= 0; bit--)
                this.Bitstream.WriteBit((int)((value >> bit) & 1));
        }

        public ulong WriteUnsignedInt(int count, uint value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            RecordedBits(name, value);
            WriteBits(count, value);
            LogEnd(name, (ulong)count, value);
            return (ulong)count;
        }

        public ulong WriteFixed(int count, int value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteUnsignedInt(count, unchecked((uint)value), name);
        }

        public ulong WriteVariable(long count, int value, string name) => WriteUnsignedInt((int)count, unchecked((uint)value), name);

        public ulong WriteSignedIntVar(int count, int value, string name)
        {
            RecordedBits(name, value);
            WriteBits(count, value);
            LogEnd(name, (ulong)count, value);
            return (ulong)count;
        }

        public ulong WriteLeb128(int value, string name) => WriteLeb128((long)value, name);

        public ulong WriteLeb128(long value, string name)
        {
            ulong v = unchecked((ulong)value) & 0xFFFFFFFFUL;
            // Recorded, the value is written whole: a leb128() of 8 bytes holds 56 bits, past what an int
            // keeps, and a stream may code one (a value past 2^32 - 1 is not allowed, but is read).
            int recorded = 0;
            if (_pickedName == name && (_picked.Value & 0xFFFFFFFFL) == (value & 0xFFFFFFFFL))
            {
                if (_picked.Bytes != null)
                {
                    // Its bytes as they were coded
                    byte[] coded = _picked.Bytes;
                    _pickedName = null;
                    foreach (byte b in coded)
                        WriteBits(8, b);
                    this.Leb128Bytes = coded.Length;
                    LogEnd(name, (ulong)coded.Length << 3, value);
                    return (ulong)coded.Length << 3;
                }
                v = (ulong)_picked.Value;
                recorded = _picked.Bits / 8;
            }
            _pickedName = null;
            int bytes = 1;
            while (bytes < 8 && (v >> (7 * bytes)) != 0)
                bytes++;
            // As many bytes as it was read in: a leb128() may be padded to a length the encoder chose.
            if (recorded > bytes && recorded <= 8)
                bytes = recorded;
            for (int i = 0; i < bytes; i++)
            {
                long b = (long)((v >> (7 * i)) & 0x7f);
                if (i < bytes - 1)
                    b |= 0x80;
                WriteBits(8, b);
            }
            this.Leb128Bytes = bytes;
            LogEnd(name, (ulong)bytes << 3, value);
            return (ulong)bytes << 3;
        }

        public ulong WriteUvlc(int value, string name) => WriteUvlc(unchecked((uint)value), name);

        public ulong WriteUvlc(uint value, string name)
        {
            int recorded = RecordedBits(name, value);
            ulong size;
            if (value == uint.MaxValue && recorded > 32)
            {
                // 32 leading zeros or more: read as 2^32 - 1, with no bits after the one.
                WriteBits(recorded - 1, 0);
                WriteBits(1, 1);
                size = (ulong)recorded;
            }
            else
            {
                ulong v = (ulong)value + 1;
                int leadingZeros = 0;
                while ((v >> (leadingZeros + 1)) != 0)
                    leadingZeros++;
                WriteBits(leadingZeros, 0);
                WriteBits(1, 1);
                WriteBits(leadingZeros, (long)(v - (1UL << leadingZeros)));
                size = (ulong)(2 * leadingZeros + 1);
            }
            LogEnd(name, size, value);
            return size;
        }

        /// <summary>svlc(), AV2 4.11.4.</summary>
        public ulong WriteSvlc(int value, string name)
        {
            RecordedBits(name, value);
            uint v = value > 0 ? (uint)(2L * value - 1) : (uint)(-2L * value);
            ulong size = WriteUvlc(v, "");
            LogEnd(name, size, value);
            return size;
        }

        /// <summary>le(n), AV2 4.11.5.</summary>
        public ulong WriteLe(int count, int value, string name)
        {
            if (count > 4)
                throw new ArgumentOutOfRangeException(nameof(count));
            RecordedBits(name, value);
            long t = unchecked((uint)value);
            for (int i = 0; i < count; i++)
                WriteBits(8, (t >> (i * 8)) & 0xFF);
            LogEnd(name, (ulong)count << 3, t);
            return (ulong)count << 3;
        }

        public ulong WriteBytes(int count, byte[] value, string name)
        {
            _pickedName = null;
            if (value == null || value.Length != count / 8)
                throw new ArgumentException($"{name} is {count / 8} bytes, not {value?.Length}.", nameof(value));
            if (this.Bitstream.BitsPosition % 8 == 0)
            {
                if (WriteLimit >= 0 && this.Bitstream.BitsPosition + (long)value.Length * 8 > WriteLimit)
                    throw new InvalidOperationException($"Writing {value.Length} bytes at {this.Bitstream.BitsPosition} passes the end of the OBU at {WriteLimit}.");
                this.Bitstream.WriteAlignedBytes(value, 0, value.Length);
            }
            else
            {
                foreach (byte b in value)
                    WriteBits(8, b);
            }
            LogEnd(name, (ulong)(value.Length * 8), "byte[]");
            return (ulong)(value.Length * 8);
        }

        public ulong Write_ns(int count, uint value, string name) => Write_ns(count, (int)value, name);

        public ulong Write_ns(int count, int value, string name)
        {
            RecordedBits(name, value);
            int w = (int)Math.Floor(MathEx.Log2(count)) + 1;
            int m = (1 << w) - count;
            if (value < m)
            {
                WriteBits(w - 1, value);
                LogEnd(name, (ulong)(w - 1), (uint)value);
                return (ulong)(w - 1);
            }
            long coded = (long)value + m;
            WriteBits(w - 1, coded >> 1);
            WriteBits(1, coded & 1);
            LogEnd(name, (ulong)w, (uint)value);
            return (ulong)w;
        }

        /// <summary>tu(mx), AV2 4.11.9.</summary>
        public ulong WriteTu(int max, int value, string name)
        {
            RecordedBits(name, value);
            ulong size = 0;
            for (int idx = 0; idx < max; idx++)
            {
                int bit = idx < value ? 1 : 0;
                WriteBits(1, bit);
                size++;
                if (bit == 0)
                    break;
            }
            LogEnd(name, size, value);
            return size;
        }

        /// <summary>rg(n), AV2 4.11.10; -1 is 32 ones.</summary>
        public ulong WriteRg(int count, int value, string name)
        {
            RecordedBits(name, value);
            ulong size;
            if (value < 0)
            {
                WriteBits(32, 0xFFFFFFFFL);
                size = 32;
            }
            else
            {
                long q = (long)value >> count;
                if (q >= 32)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{name} of {value} takes more than 32 ones in rg({count}).");
                for (long i = 0; i < q; i++)
                    WriteBits(1, 1);
                WriteBits(1, 0);
                WriteBits(count, value & ((1L << count) - 1));
                size = (ulong)(q + 1 + count);
            }
            LogEnd(name, size, value);
            return size;
        }

        #endregion // Writing

        private int _logLevel = 0;

        // Note: This function was not used, so I commented it out.
        //private void LogBegin(string name)
        //{
        //    string padding = "-";
        //    for (int i = 0; i < _logLevel; i++)
        //    {
        //        padding += "-";
        //    }

        //    this.Logger.LogInfo($"{padding} {name}");
        //}

        // The bytes the element being logged was coded in, where they say more than its value
        private byte[] _codedAs;

        /// <summary>
        /// Called with each element as it ends, and the bit it ends at: where each element is, without
        /// a log line made for it.
        /// </summary>
        public Action<string, long> ElementEnded { get; set; }

        private void LogEnd<T>(string name, ulong size, T value)
        {
            byte[] codedAs = _codedAs;
            _codedAs = null;
            if (ElementEnded != null && !string.IsNullOrEmpty(name))
                ElementEnded(name, this.Bitstream.BitsPosition);
            if (Record != null && !string.IsNullOrEmpty(name))
            {
                switch (value)
                {
                    case long l: Record.Add(name, new AomSyntaxValue(l, (int)size, codedAs)); break;
                    case int i: Record.Add(name, new AomSyntaxValue(i, (int)size)); break;
                    case uint u: Record.Add(name, new AomSyntaxValue(u, (int)size)); break;
                }
            }

            // Formatted only for a logger that takes it, as ItuStream and IsoStream do: made for every
            // element read and written, the line nobody asked for was a third of the time taken.
            if (this.Logger == null || !this.Logger.IsInfoEnabled)
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

        #region IDisposable implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this.Bitstream.BaseStream.Dispose();
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
