using SharpMP4.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SharpH26X
{
    public class ItuStream : IDisposable
    {
        private Stream _stream;

        private int _rbspDataCounter = -1;
        private int _readNextBitsCounter = -1;
        private int _readNextBitsIndex = 0;

        private bool _disposedValue;

        public IMp4Logger Logger { get; set; }
        public RbspBitstream Bitstream { get; set; }

        public ItuStream(RbspBitstream bitstream, IMp4Logger logger)
        {
            this._stream = bitstream.BaseStream;
            this.Logger = logger;
            this.Bitstream = bitstream;
        }

        public ItuStream(Stream stream, IMp4Logger logger)
            : this(new RbspBitstream(stream), logger)
        {
        }

        public ItuStream(Stream stream)
            : this(stream, DefaultMp4Logger.Instance)
        {
        }

        /// <summary>
        /// Reads a NAL unit where it lies in an array. Made once and <see cref="Reset"/> for each NAL unit, reading a
        /// stream of them allocates no stream per unit.
        /// </summary>
        public ItuStream(byte[] buffer, int offset, int length, IMp4Logger logger = null)
            : this(new RbspBitstream(buffer, offset, length), logger ?? DefaultMp4Logger.Instance)
        {
        }

        /// <summary>Starts over on another NAL unit, in an array, as a stream made for it would.</summary>
        public void Reset(byte[] buffer, int offset, int length)
        {
            if (_isLookahead)
                throw new InvalidOperationException("A lookahead stream reads the stream it was taken from.");

            this.Bitstream.Reset(buffer, offset, length);
            _stream = null;
            _rbspDataCounter = -1;
            _readNextBitsCounter = -1;
            _readNextBitsIndex = 0;
        }

        // Of a stream Lookahead gives: the bits of the stream it was taken from, to go back to when it is disposed.
        private readonly bool _isLookahead;
        private readonly RbspBitstream.PeekState _peek;

        private ItuStream(RbspBitstream bitstream, RbspBitstream.PeekState peek)
            : this(bitstream, null)
        {
            _isLookahead = true;
            _peek = peek;
        }

        /// <summary>
        /// Verifies that an array of <paramref name="count"/> entries could actually be read from
        /// what is left of the stream, before the array is allocated.
        /// </summary>
        /// <remarks>
        /// Several array lengths in the H.26x syntax - num_negative_pics, num_entry_point_offsets
        /// and their neighbours - are Exp-Golomb coded, so a corrupt or hostile stream can put an
        /// enormous value there. Allocating straight from one lets a NAL unit of a few kilobytes
        /// ask for gigabytes. Every entry occupies at least one bit, so a count can never exceed
        /// the number of bits left; that bound rejects nothing a conforming stream produces.
        /// </remarks>
        public void CheckArrayAllocation(ulong count, string name)
        {
            long remaining;
            try
            {
                remaining = this.Bitstream.RemainingBytes;
            }
            catch (NotSupportedException)
            {
                return; // not seekable, so there is nothing to bound against
            }

            // The bit reader buffers a byte ahead, so the stream can already be at its end while bits are still to be
            // handed out. Allow for that, plus a byte for the one an emulation prevention scan may have pulled in; the
            // bound is there to stop counts in the millions, and a couple of bytes of slack costs nothing.
            ulong remainingBits = (ulong)Math.Max(0, remaining) * 8 + 16;
            if (count > remainingBits)
            {
                string message = $"Invalid count of '{name}': {count} entries do not fit into the remaining {Math.Max(0, remaining)} bytes";
                Logger?.LogDebug(message);
                throw new ItuEndOfStreamException(message);
            }
        }

        #region Bit read/write


        private ulong WriteByte(byte value)
        {
            _stream.WriteByte(value);
            return 8;
        }

        private int ReadBit() => this.Bitstream.ReadBit();

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

        public void WriteBit(int value) => this.Bitstream.WriteBit(value);

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

        public void MarkCurrentBitsPosition() => this.Bitstream.Mark();

        public ulong GetBitsPositionSinceLastMark() => (ulong)this.Bitstream.GetBitsSinceMark();

        /// <summary>
        /// Puts back a mark set before another: GetBitsPositionSinceLastMark then counts from the
        /// earlier one again, which was bitsSinceMark bits before the later one.
        /// </summary>
        public void RestoreBitsMark(ulong bitsSinceMark) => this.Bitstream.MoveMarkBack((long)bitsSinceMark);

        public bool ByteAligned()
        {
            return this.Bitstream.BitsPosition % 8 == 0;
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

        /// <summary>
        /// Reads whole bytes from a byte aligned position, emulation prevention bytes dropped.
        /// The same as reading them eight bits at a time, for the bulk of a NAL unit - see
        /// <see cref="RbspBitstream.ReadBytes"/>.
        /// </summary>
        /// <returns>How many bytes were read: fewer than asked for only at the end of the stream.</returns>
        public int ReadBytes(byte[] buffer, int offset, int count) => Bitstream.ReadBytes(buffer, offset, count);

        /// <summary>
        /// Writes whole bytes at a byte aligned position, emulation prevention bytes put in. The
        /// same as writing them eight bits at a time - see <see cref="RbspBitstream.WriteBytes"/>.
        /// </summary>
        public void WriteBytes(byte[] buffer, int offset, int count) => Bitstream.WriteBytes(buffer, offset, count);

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
            // two's complement of count bits: H.262's simsbf of 7 to 22 bits, H.26x's i(32)
            if (count < 64 && (ret & (1L << ((int)count - 1))) != 0)
                ret -= 1L << (int)count;
            value = unchecked((int)ret);
            LogEnd(name, count, value);
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

        /// <summary>
        /// A stream to read ahead on from where this one is: reading it logs nothing, and disposing it puts this stream
        /// back where it was. It reads the same bits, the bytes it takes kept to be read again, so nothing is copied and
        /// the stream need not seek. This stream is not read while it is in use.
        /// </summary>
        public ItuStream Lookahead() => new ItuStream(this.Bitstream, this.Bitstream.BeginPeek());

        /// <summary>
        /// more_rbsp_data(): whether there is more data before the RBSP's stop bit - a 1 bit with nothing but zero bits
        /// after it - read ahead and gone back from, so it costs no copy and the stream need not seek.
        /// </summary>
        public bool ReadMoreRbspData(IItuSerializable serializable, ulong maxPayloadSize = ulong.MaxValue)
        {
            ulong sinceMark = GetBitsPositionSinceLastMark();

            bool more;
            var state = this.Bitstream.BeginPeek();
            try
            {
                more = MoreRbspDataAhead();
            }
            finally
            {
                this.Bitstream.EndPeek(state);
            }

            if (!more || (maxPayloadSize != ulong.MaxValue && sinceMark >= maxPayloadSize * 8))
                return false;

            serializable.HasMoreRbspData++;
            return true;
        }

        /// <summary>Whether a bit that is not the stop bit is ahead: a 0 bit, or a 1 bit with another 1 bit after it.</summary>
        private bool MoreRbspDataAhead()
        {
            int one = ReadBit();
            if (one == -1)
                return false;
            if (one == 0)
                return true;

            int lastBit = ReadBit();
            while (lastBit == 0)
                lastBit = ReadBit();

            // -1: nothing but zeros up to the end, so the 1 was the stop bit
            return lastBit != -1;
        }

        public bool WriteMoreRbspData(IItuSerializable serializable, ulong maxPayloadSize = ulong.MaxValue) // TODO
        {
            if (serializable.HasMoreRbspData == 0 || _rbspDataCounter == 0)
                return false;
            else if (_rbspDataCounter == -1)
                _rbspDataCounter = serializable.HasMoreRbspData;
            return _rbspDataCounter-- != 0;
        }

        public int ReadNextBits(IItuSerializable serializable, ulong count)
        {
            // next_bits(): the byte ahead, read and gone back from
            int ret;
            var state = this.Bitstream.BeginPeek();
            try
            {
                ret = (int)ReadBits(8);
            }
            finally
            {
                this.Bitstream.EndPeek(state);
            }

            {
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

                if (_readNextBitsIndex >= serializable.ReadNextBits.Length)
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
            if (bits == -1)
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
            while ((b = (int)ReadBits(8)) != -1) // as every other read: through the bits, emulation prevention bytes skipped
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
            // Checked before the message is built. These run on every syntax element read, so
            // formatting first and discarding inside the logger costs the allocations and the
            // string work on the hottest path in the parser even when nothing is being logged.
            if (this.Logger == null || !this.Logger.IsInfoEnabled)
                return;

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

        #endregion // Logging

        #region IDisposable 

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_isLookahead)
                    {
                        // the stream it was taken from goes on from where it was; its stream is not this one's to close
                        this.Bitstream.EndPeek(_peek);
                    }
                    else if (_stream != null)
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
