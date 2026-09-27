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
            this.Logger = logger ?? new DefaultMp4Logger();
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

        public ulong ReadLeb128(out int value, string name)
        {
            // Gathered in 64 bits: up to eight bytes of seven bits each. Gathered in an int, the
            // bytes past the fourth were shifted round into the low bits and the value was garbage.
            long v = 0;
            int Leb128Bytes = 0;
            for (int i = 0; i < 8; i++)
            {
                int leb128_byte = ReadByte();
                v |= (long)(leb128_byte & 0x7f) << (i * 7);
                Leb128Bytes += 1;
                if((leb128_byte & 0x80) == 0)
                {
                    break;
                }
            }

            value = unchecked((int)v);
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

        private void LogEnd<T>(string name, ulong size, T value)
        {
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
