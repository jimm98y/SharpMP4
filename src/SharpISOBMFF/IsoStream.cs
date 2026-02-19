using SharpMP4Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpISOBMFF
{
    public class IsoStream : IDisposable
    {
        protected readonly IStorage _stream;
        protected int _bitsPosition;
        protected int _currentBytePosition = -1;
        protected byte _currentByte = 0;
        private bool _disposedValue;
        private ITemporaryStorageFactory _storageFactory;
        private IsoStream _temp;

        public IMp4Logger Logger { get; set; }

        public IsoStream(Stream stream, ITemporaryStorageFactory storageFactory = null) : this(new StreamWrapper(stream), storageFactory)
        { }

        public IsoStream(IStorage stream, ITemporaryStorageFactory storageFactory = null, IMp4Logger logger = null)
        {
            _stream = stream;
            _storageFactory = storageFactory ?? TemporaryStorage.Factory;

            this.Logger = logger ?? new DefaultMp4Logger();
        }

        // In case users would like to change it later, for example to use memory storage instead of file storage
        public ITemporaryStorageFactory TemporaryStorageFactory
        {
            get => _storageFactory;
            set => _storageFactory = value;
        }

        private IsoStream GetOrCreateTemporaryStorage()
        {
            if (_temp == null)
                _temp = new IsoStream(_storageFactory.Create());
            return _temp;
        }

        #region Stream operations

        public bool HasMoreData(ulong boxSize, ulong readSize)
        {
            if (readSize == ulong.MaxValue)
            {
                return true;
            }

            ulong remaining = readSize - boxSize;
            return remaining > 0;
        }

        public bool CanStreamSeek()
        {
            return _stream.CanStreamSeek();
        }

        public long GetCurrentOffset()
        {
            try
            {
                if (CanStreamSeek())
                    return _stream.GetPosition();
                else
                    return -1;
            }
            catch (Exception e) 
            {
                this.Logger.LogDebug($"Getting the current stream offset failed: {e.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Returns stream length in bytes.
        /// </summary>
        /// <returns></returns>
        public long GetStreamLength()
        {
            try
            {
                if (CanStreamSeek())
                    return _stream.GetLength();
                else
                    return -1;
            }
            catch (Exception e)
            {
                this.Logger.LogDebug($"Getting the current stream length failed: {e.Message}");
                return -1;
            }
        }

        public void SeekFromEnd(long offset)
        {
            _stream.SeekFromEnd(offset);
        }

        public void SeekFromCurrent(long offset)
        {
            _stream.SeekFromCurrent(offset);
        }

        public void SeekFromBeginning(long offset)
        {
            _stream.SeekFromBeginning(offset);
        }

        #endregion // Stream operations

        #region Basic read/write operations

        private int ReadBitInternal()
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

        private void WriteBitInternal(int value)
        {
            int posInByte = 7 - _bitsPosition % 8;
            int bit = (value & 1) << posInByte;
            _currentByte = (byte)(_currentByte | bit);
            ++_bitsPosition;

            int bytePos = _bitsPosition >> 3;
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

        public int ReadByteInternal()
        {
            return _stream.ReadByte();
        }

        private byte ReadByte()
        {
            int read = ReadByteInternal();
            if (read == -1) throw new EndOfStreamException();
            return (byte)(read & 0xff);
        }

        private ulong WriteByte(byte value)
        {
            _stream.WriteByte(value);
            return 8;
        }

        public ulong ReadBytes(ulong length, out byte[] value)
        {
            ulong correctedLength = length;
            if (CanStreamSeek())
            {
                correctedLength = Math.Min(length, (ulong)(GetStreamLength() - GetCurrentOffset()));
            }

            if (correctedLength < length)
            {
                throw new IsoEndOfStreamException(new StreamMarker(GetCurrentOffset(), GetStreamLength(), this));
            }

            value = new byte[correctedLength];
            _stream.ReadExactly(value, 0, (int)correctedLength);

            return correctedLength << 3;
        }

        public ulong WriteBytes(ulong count, byte[] value)
        {
            //for (ulong i = 0; i < count; i++)
            //{
            //    WriteByte(value[i]);
            //}
            _stream.Write(value, 0, (int)count);
            return count << 3;
        }

        private static ulong CopyStream(IStorage input, IStorage output, long bytes = -1)
        {
            byte[] buffer = new byte[32768];
            int read;
            ulong size = 0;
            if (bytes < 0)
            {
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                    bytes -= read;
                    size += (ulong)read;
                }
            }
            else
            {
                while (bytes > 0 && (read = input.Read(buffer, 0, (int)Math.Min(buffer.Length, bytes))) > 0)
                {
                    output.Write(buffer, 0, read);
                    bytes -= read;
                    size += (ulong)read;
                }
            }

            return size;
        }

        #endregion // Basic read/write operations

        #region Bits

        public static int BitsToDecode(ulong boxSize, ulong readSize)
        {
            return (int)(readSize - boxSize);
        }

        public ulong ReadBit(ulong boxSize, ulong readSize, out bool value, string name)
        {
            value = ReadBitInternal() != 0;
            LogEnd(name, 1, value);
            return 1;
        }

        public ulong ReadBits(ulong boxSize, ulong readSize, uint count, out byte value, string name)
        {
            if (count > 8) throw new ArgumentException();
            ulong ret = ReadBits(boxSize, readSize, count, out uint v, name);
            value = (byte)v;
            return ret;
        }

        public ulong ReadBits(ulong boxSize, ulong readSize, uint count, out sbyte value, string name)
        {
            if (count > 8) throw new ArgumentException();
            ulong ret = ReadBits(boxSize, readSize, count, out uint v, name);
            value = (sbyte)v;
            return ret;
        }

        public ulong ReadBits(ulong boxSize, ulong readSize, uint count, out ushort value, string name)
        {
            if (count > 16) throw new ArgumentException();
            ulong ret = ReadBits(boxSize, readSize, count, out uint v, name);
            value = (ushort)v;
            return ret;
        }

        public ulong ReadBits(ulong boxSize, ulong readSize, uint count, out short value, string name)
        {
            if (count > 16) throw new ArgumentException();
            uint originalCount = count;

            int sign = ReadBitInternal() == 1 ? -1 : 1;
            count--;

            int res = 0;
            while (count > 0)
            {
                res = res << 1;
                int u1 = ReadBitInternal();

                if (u1 == -1)
                {
                    throw new EndOfStreamException();
                }

                res |= u1;
                count--;
            }

            value = (short)(res * sign);
            LogEnd(name, originalCount, value);
            return originalCount;
        }

        public ulong ReadBits(ulong boxSize, ulong readSize, uint count, out uint value, string name)
        {
            if (count > 32) throw new ArgumentException();
            uint originalCount = count;
            int res = 0;
            while (count > 0)
            {
                res = res << 1;
                int u1 = ReadBitInternal();

                if (u1 == -1)
                {
                    throw new EndOfStreamException();
                }

                res |= u1;
                count--;
            }

            value = (uint)res;
            LogEnd(name, originalCount, value);
            return originalCount;
        }

        public ulong ReadBits(ulong boxSize, ulong readSize, uint count, out ulong value, string name)
        {
            if (count > 64) throw new ArgumentException();
            uint originalCount = count;
            long res = 0;
            while (count > 0)
            {
                res = res << 1;
                long u1 = ReadBitInternal();

                if (u1 == -1)
                {
                    throw new EndOfStreamException();
                }

                res |= u1;
                count--;
            }

            value = (ulong)res;
            LogEnd(name, originalCount, value);
            return originalCount;
        }

        public ulong WriteBit(bool value, string name)
        {
            WriteBitInternal(value ? 1 : 0);
            LogEnd(name, 1, value);
            return 1;
        }

        public ulong WriteBit(byte value)
        {
            WriteBitInternal(value);
            return 1;
        }

        public ulong WriteBit(byte value, string name)
        {
            WriteBitInternal(value);
            LogEnd(name, 1, value);
            return 1;
        }

        public ulong WriteBits(uint count, ulong value)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteBits(count, (ulong)value, "");
        }

        public ulong WriteBits(uint count, byte value, string name)
        {
            if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteBits(count, (ulong)value, name);
        }

        public ulong WriteBits(uint count, ushort value, string name)
        {
            if (count > 16)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteBits(count, (ulong)value, name);
        }

        public ulong WriteBits(uint count, short value, string name)
        {
            if (count > 16)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteBits(count, unchecked((ushort)value), name);
        }

        public ulong WriteBits(uint count, uint value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteBits(count, (ulong)value, name);
        }

        public ulong WriteBits(uint count, ulong value, string name)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));
            uint originalCount = count;
            while (count > 0)
            {
                int bits = (int)count - 1;
                ulong mask = 0x1u << bits;
                WriteBitInternal((int)((value & mask) >> bits));
                count--;
            }

            LogEnd(name, originalCount, value);
            return originalCount;
        }

        public ulong ReadBits(ulong boxSize, ulong readSize, uint count, out byte[] value, string name)
        {
            value = new byte[(count >> 3) + (count % 8)];
            int i = 0;
            int c = (int)count;
            while (i < value.Length && c > 0)
            {
                byte v = 0;
                c -= (int)ReadBits((ulong)((long)boxSize + count - c), readSize, (uint)Math.Min(c, 8), out v, "");
                value[i] = v;
                i++;
            }

            LogEnd(name, count, value);

            return count;
        }

        public ulong WriteBits(uint count, byte[] value, string name)
        {
            int c = (int)count;
            int i = 0;
            while (i < value.Length && c > 0)
            {
                c -= (int)WriteBits((uint)Math.Min(c, 8), value[i], name);
                i++;
            }
            return count;
        }

        public ulong ReadByteAlignment(ulong boxSize, ulong readSize, out byte value, string name)
        {
            int bytePos = _bitsPosition >> 3;
            int currentBytePos = bytePos << 3;
            uint bitsToRead = (uint)(8 - (_bitsPosition - currentBytePos));
            return ReadBits(boxSize, readSize, bitsToRead, out value, name);
        }

        public ulong WriteByteAlignment(byte value, string name)
        {
            int bytePos = _bitsPosition >> 3;
            int currentBytePos = bytePos << 3;
            uint bitsToWrite = (uint)(8 - (_bitsPosition - currentBytePos));
            return WriteBits(bitsToWrite, value, name);
        }

        public static ulong CalculateByteAlignmentSize(ulong boxSize, byte value)
        {
            return 8 - (boxSize % 8);
        }

        #endregion // Bits

        #region Strings

        public ulong ReadStringZeroTerminatedArray(ulong boxSize, ulong readSize, uint count, out BinaryUTF8String[] value, string name)
        {
            ulong size = 0;
            BinaryUTF8String[] strings = new BinaryUTF8String[count];
            for (int i = 0; i < count; i++)
            {
                size += ReadBytes(count, out byte[] bytes);
                strings[i] = new BinaryUTF8String(bytes) { IsZeroTerminated = false };
            }
            value = strings;

            LogEnd(name, size, strings);
            return size;
        }

        public ulong WriteStringZeroTerminatedArray(uint count, BinaryUTF8String[] values, string name)
        {
            ulong size = 0;
            for (int i = 0; i < count && i < values.Length; i++)
            {
                size += WriteString(values[i], "");
            }

            LogEnd(name, size, values);
            return size;
        }

        public ulong WriteString(BinaryUTF8String value, string name)
        {
            if (value.Length == 0)
                return 0;

            byte[] buffer = value.Bytes;
            for (int i = 0; i < value.Length; i++)
            {
                WriteByte(buffer[i]);
            }

            ulong size = (ulong)value.Length << 3;
            LogEnd(name, size, EscapeString(value.ToString()));
            return size;
        }

        public ulong WriteStringZeroTerminated(BinaryUTF8String value, string name)
        {
            return WriteString(value, name);
        }

        public ulong ReadStringZeroTerminated(ulong boxSize, ulong readSize, out BinaryUTF8String value, string name)
        {
            ulong remaining = readSize - boxSize;
            if (remaining == 0)
            {
                value = new BinaryUTF8String();
                return 0;
            }

            List<byte> buffer = new List<byte>();
            ulong remainingCount = remaining >> 3;
            while(remainingCount > 0)
            {
                int b = ReadByteInternal();
                if (b == -1)
                    break;
                buffer.Add((byte)b);
                remainingCount--;
                if (b == 0)
                    break;
            }

            value = new BinaryUTF8String(buffer.ToArray());

            ulong size = (ulong)buffer.Count * 8;
            LogEnd(name, size, EscapeString(value.ToString()));
            return size;
        }

        public ulong ReadStringSizeLangPrefixed(ulong boxSize, ulong readSize, out MultiLanguageString[] value, string name)
        {
            ulong remaining = readSize - boxSize;
            if (remaining == 0)
            {
                value = [];
                return 0;
            }

            List<MultiLanguageString> items = new List<MultiLanguageString>();

            ulong size = 0;
            while (size < remaining)
            {
                ushort length;
                size += ReadUInt16(boxSize + size, readSize, out length, "");
                ushort lang;
                size += ReadUInt16(boxSize + size, readSize, out lang, "");
                byte[] bytes;
                size += ReadBytes(length, out bytes);

                BinaryUTF8String str = new BinaryUTF8String(bytes) { IsZeroTerminated = false };
                LogEnd(name, size, EscapeString(str.ToString()));

                MultiLanguageString item = new MultiLanguageString(lang, length, str);
                items.Add(item);
            }

            value = items.ToArray();

            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteStringSizeLangPrefixed(MultiLanguageString[] value, string name)
        {
            if (value == null || value.Length == 0)
                return 0;

            ulong size = 0;
            foreach (var item in value)
            {
                size += WriteUInt16(item.Length, "");
                size += WriteUInt16(item.Language, "");
                size += WriteString(item.Value, "");
            }

            LogEnd(name, size, value);

            return size;
        }

        public static ulong CalculateStringSizeLangPrefixed(MultiLanguageString[] value)
        {
            ulong size = 0;
            foreach (var str in value)
            {
                size += 16;
                size += 16;
                size += CalculateStringSize(str.Value);
            }
            return size;
        }

        public static ulong CalculateStringSize(BinaryUTF8String value)
        {
            ulong count = (ulong)value.Length;
            return count << 3;
        }

        public static ulong CalculateStringSize(BinaryUTF8String[] value)
        {
            ulong size = 0;
            foreach (var str in value)
            {
                size += CalculateStringSize(str);
            }
            return size;
        }

        #endregion // Strings

        #region FourCC

        public static uint FromFourCC(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;
            byte[] buffer = Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
            if (buffer.Length != 4)
                throw new Exception("Invalid 4cc!");
            return (uint)(
                ((uint)buffer[0] << 24) +
                ((uint)buffer[1] << 16) +
                ((uint)buffer[2] << 8) +
                ((uint)buffer[3])
            );
        }

        public static string ToFourCC(uint value)
        {
            byte[] buffer = {
                (byte)(value >> 24 & 0xFF),
                (byte)(value >> 16 & 0xFF),
                (byte)(value >> 8 & 0xFF),
                (byte)(value & 0xFF)
            };
            return Encoding.GetEncoding("ISO-8859-1").GetString(buffer);
        }

        #endregion // FourCC

        #region Boxes

        public ulong ReadBox(ulong boxSize, IMp4Serializable parent, out Box box, string name)
        {
            return ReadBox(boxSize, ulong.MaxValue, parent, out box, name);
        }

        public ulong WriteBox(Box value, string name)
        {
            ulong size = WriteBoxHeader(value);
            size += value.Write(this);
            if(value.Padding != null)
            {
                size += WritePadding(value.Padding);
            }
            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadBoxHeader(out SafeBoxHeader header)
        {
            header = new SafeBoxHeader();
            long headerOffset = 0;
            ulong headerSize = 0;

            headerOffset = this.GetCurrentOffset();

            // sometimes there can be a few bytes at the end of the mp4 file that are less than the header size
            ulong remaining = (ulong)(GetStreamLength() - headerOffset);
            if (headerOffset > 0 && remaining == 0)
            {
                throw new EndOfStreamException();
            }

            if (remaining > 0 && remaining < 8)
            {
                throw new IsoEndOfStreamException(new StreamMarker(GetCurrentOffset(), GetStreamLength(), this));
            }

            headerSize = header.Read(this, 0);
            return headerSize;
        }

        public ulong WriteBoxHeader(Box value)
        {
            SafeBoxHeader header;
            if(value.Header == null)
            {
                header = new SafeBoxHeader();
                ulong boxSizeBits = value.CalculateSize();
                ulong boxSize = boxSizeBits >> 3;
                if (boxSize > uint.MaxValue || value.HasLargeSize)
                {
                    header.Size = 1;
                    header.Largesize = boxSize;
                }
                else
                {
                    header.Size = (uint)boxSize;
                    header.Largesize = 0;
                }
                header.Usertype = value.Uuid;
                header.Type = value.FourCC;
                value.Header = header;
            }
            else
            {
                // write the header "as is"
                header = value.Header;
            }        

            ulong writtenSize = 0;
            writtenSize += header.Write(this);
            return writtenSize;
        }

        public ulong ReadBox<T>(ulong boxSize, ulong readSize, Func<SafeBoxHeader, Box> factory, IMp4Serializable parent, out T value, string name) where T : Box
        {
            LogBegin(name);

            SafeBoxHeader header;
            long headerOffset = this.GetCurrentOffset();
            ulong headerSize = ReadBoxHeader(out header);

            ulong availableSize = readSize == ulong.MaxValue ? ulong.MaxValue : (readSize - boxSize - headerSize);
            Box box;
            ulong size;

            if (GetBoxSize(header) - headerSize > availableSize)
            {
                // make sure we do not modify any bytes
                box = new InvalidBox(header.Type);
                box.SetParent(parent);
                box.Header = header;
                LogBox(header, GetIndentation(box));
                size = box.Read(this, availableSize) + headerSize;
            }
            else
            {
                box = factory(header);
                box.SetParent(parent);
                box.Header = header;
                LogBox(header, GetIndentation(box));
                size = ReadBox(header, box, availableSize);
            }

            value = (T)box;

            if (value != null)
            {
                value.SetBoxOffset(headerOffset);
                value.Size = header.GetBoxSizeInBits() >> 3;
            }

            LogEnd(name, size, value);

            return size;
        }

        public void LogBox(SafeBoxHeader header, string indentation = "")
        {
            string uuid = "";
            if (header.Usertype != null)
            {
                uuid = $" (uuid: {ConvertEx.ToHexString(header.Usertype).ToLowerInvariant()})";
            }
            this.Logger.LogDebug($"BOX:{indentation}\'{EscapeString(ToFourCC(header.Type))}\'{uuid}");
        }

        public static string EscapeString(string text)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsControl(text[i]))
                    sb.Append($"\\x{((int)text[i]).ToString("X2")}");
                else
                    sb.Append(text[i]);
            }
            return sb.ToString();
        }

        private Box DefaultBoxFactory(IMp4Serializable parent, SafeBoxHeader header)
        {
            string parentFourCC = "";
            if (parent != null)
                parentFourCC = ToFourCC(((Box)parent).FourCC);
            return BoxFactory.CreateBox(ToFourCC(header.Type), parentFourCC, header.Usertype, this.Logger);
        }

        public ulong ReadBox<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, out T value, string name) where T : Box
        {
            return ReadBox(boxSize, readSize, 
                (header) => DefaultBoxFactory(parent, header),
                parent,
                out value, name);    
        }

        public ulong ReadBox<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, out T[] value, string name) where T : Box
        {
            return ReadBox<T>(boxSize, readSize,
                (header) => DefaultBoxFactory(parent, header),
                parent,
                out value, name);
        }

        public ulong ReadBox<T>(ulong boxSize, ulong readSize, Func<SafeBoxHeader, Box> factory, IMp4Serializable parent, out T[] value, string name) where T : Box
        {
            var boxes = new List<T>();

            ulong consumed = 0;

            LogBegin(name);

            if (readSize == 0)
            {
                List<Box> values = new List<Box>();
                // consume till the end of the stream
                try
                {
                    while (true)
                    {
                        T v;
                        consumed += ReadBox<T>(consumed, readSize, factory, parent, out v, name);
                        boxes.Add(v);
                    }

                }
                catch (EndOfStreamException)
                { }

                value = boxes.ToArray();

                return consumed;
            }

            ulong remaining = readSize - boxSize;
            while (consumed < remaining)
            {
                T v;
                consumed += ReadBox<T>(consumed, readSize, factory, parent, out v, name);
                boxes.Add(v);
            }
            value = boxes.ToArray();

            LogEnd(name, consumed, value);

            return consumed;
        }

        public ulong WriteBox(Box[] value, string name)
        {
            ulong size = 0;

            LogBegin(name);

            foreach (var box in value)
            {
                size += WriteBox(box, "");
            }

            LogEnd(name, size, value);

            return size;
        }

        public ulong ReadBoxArrayTillEnd(ulong boxSize, ulong readSize, IHasBoxChildren box)
        {
            if (box.Children != null)
            {
                this.Logger.LogDebug("Box reading repeated Children read");
                return 0;
            }

            if (readSize == 0)
                return 0;

            box.Children = new List<Box>();

            ulong consumed = 0;

            if (readSize == ulong.MaxValue)
            {
                // consume till the end of the stream
                try
                {
                    while (true)
                    {
                        Box v;
                        ulong readBoxSize = ReadBox(consumed, readSize, box, out v, "");
                        consumed += readBoxSize;

                        if (readBoxSize == 0)
                            break;

                        box.Children.Add(v);
                    }
                }
                catch (EndOfStreamException)
                { }

                return consumed;
            }

            ulong remaining = readSize - boxSize;
            while (consumed < remaining && (remaining - consumed) >= 64) // box header is at least 8 bytes
            {
                Box v;
                consumed += ReadBox(consumed, remaining, box, out v, "");
                if (consumed > readSize)
                {
                    this.Logger.LogDebug($"Box \'{ToFourCC(v.FourCC)}\' read through!");
                    break;
                }
                box.Children.Add(v);
            }
            return consumed;
        }

        public ulong WriteBoxArrayTillEnd(IHasBoxChildren box)
        {
            if (box.Children == null)
                return 0;

            ulong written = 0;
            foreach (var v in box.Children)
            {
                written += WriteBox(v, "");
            }
            return written;
        }

        private static ulong GetBoxSize(SafeBoxHeader header)
        {
            if (header.Size == 0)
            {
                return ulong.MaxValue; // to the end of the file
            }
            else
            {
                return header.GetBoxSizeInBits();
            }
        }

        private static ulong GetHeaderSize(SafeBoxHeader header)
        {
            return header.GetHeaderSizeInBits();
        }

        private ulong ReadBox(SafeBoxHeader header, Box box, ulong readSize)
        {
            ulong availableSize = 0;

            if (GetBoxSize(header) == ulong.MaxValue)
            {
                availableSize = readSize;
            }
            else
            {
                availableSize = GetBoxSize(header) - GetHeaderSize(header);
                if (readSize < availableSize)
                {
                    throw new Exception("Box size mismatch!");
                }
            }

            box.HasLargeSize = header.Size == 1;
            box.Header = header;

            ulong size = box.Read(this, availableSize);

            if (GetBoxSize(header) != 0 && size != availableSize)
            {
                if (size < availableSize)
                {
                    StreamMarker missing;
                    size += ReadPadding(size, availableSize, out missing);
                    box.Padding = missing;
                    this.Logger.LogDebug($"Box \'{ToFourCC(box.FourCC)}\' has extra padding of {missing.Length} bytes");
                }
                else
                {
                    throw new Exception($"Box \'{ToFourCC(box.FourCC)}\' read through!");
                }
            }

            ulong calculatedSize = box.CalculateSize();
            if (calculatedSize != GetBoxSize(header))
            {
                if (box.FourCC != FromFourCC("mdat"))
                    this.Logger.LogDebug($"Calculated \'{ToFourCC(box.FourCC)}\' size: {calculatedSize / 8}, read: {GetBoxSize(header) / 8}");
            }

            return size + GetHeaderSize(header);
        }

        private static string GetIndentation(IMp4Serializable b, string c = "-")
        {
            string indent = "";
            while (b != null)
            { 
                b = b.GetParent(); 
                indent += c;
            }
            return indent;
        }

        public static ulong CalculateBoxSize(IEnumerable<IHasBoxChildren> boxes)
        {
            ulong size = 0;
            if (boxes == null)
                return 0;

            foreach (IHasBoxChildren box in boxes)
            {
                size += box.CalculateSize();
            }
            return size;
        }

        #endregion // Boxes

        #region Classes

        public ulong ReadClass<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, Func<T> factory, out T value, string name) where T : IMp4Serializable
        {
            T c = factory();
            c.SetParent(parent);

            LogBegin(name);
            _logLevel++;

            ulong size = c.Read(this, readSize - boxSize);
            value = c;

            _logLevel--;
            LogEnd(name, size, value);
            
            return size;
        }

        public ulong ReadClass<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, uint count, Func<T> factory, out T[] value, string name) where T : IMp4Serializable
        {
            ulong consumed = 0;
            ulong remaining = readSize - boxSize;
            List<T> ret = new List<T>();
            for (uint i = 0; i < count && consumed < remaining; i++)
            {
                T c;
                consumed += ReadClass<T>(boxSize + consumed, remaining, parent, factory, out c, name);
                if (consumed > readSize)
                {
                    throw new Exception($"Class read through!");
                }
                c.SetParent(parent);
                ret.Add(c);
            }
            value = ret.ToArray();
            return consumed;
        }

        public ulong WriteClass(IMp4Serializable value, string name)
        {
            ulong size = 0;

            LogBegin(name);
            _logLevel++;

            size += value.Write(this);

            _logLevel--;
            LogEnd(name, size, value);

            if (value.Padding != null)
            {
                size += WritePadding(value.Padding);
            }
            return size;
        }

        public ulong WriteClass(IMp4Serializable[] value, string name)
        {
            ulong size = 0;
            foreach (var cls in value)
            {
                size += WriteClass(cls, name);
            }
            return size;
        }

        public static ulong CalculateClassSize(IMp4Serializable[] value)
        {
            ulong size = 0;
            foreach (IMp4Serializable c in value)
            {
                size += c.CalculateSize();
            }
            return size;
        }

        #endregion // Classes

        #region Descriptors

        public ulong ReadDescriptor<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, out T descriptor, string name) where T : Descriptor
        {
            LogBegin(name);

            long availableSize = (long)readSize - (long)boxSize;
            if(availableSize == 0)
            {
                descriptor = null;
                return 0;
            }

            byte tag;
            ulong size = ReadUInt8(boxSize, readSize, out tag, "");
            if (tag == 0)
            {
                descriptor = null;
                return 8;
            }

            ulong sizeOfSize = ReadDescriptorSize(out int sizeOfInstance);
            size += sizeOfSize;
            long sizeOfInstanceBits = (long)sizeOfInstance << 3;
            descriptor = (T)BoxFactory.CreateDescriptor(tag, this.Logger);
            descriptor.SizeOfSize = sizeOfSize;
            descriptor.SetParent(parent);

            availableSize -= (long)size;
            if (availableSize < sizeOfInstanceBits)
            {
                descriptor = new InvalidDescriptor(tag) as T;
                this.Logger.LogDebug($"DES:{GetIndentation(descriptor)}\'{descriptor.DisplayName}\'");
                size += descriptor.Read(this, (ulong) availableSize);
                return size;
            }

            this.Logger.LogDebug($"DES:{GetIndentation(descriptor)}\'{descriptor.DisplayName}\'");

            ulong readInstanceSizeBits = descriptor.Read(this, (ulong)sizeOfInstanceBits);
            if (readInstanceSizeBits != (ulong)sizeOfInstanceBits)
            {
                if (readInstanceSizeBits < (ulong)sizeOfInstanceBits)
                {
                    StreamMarker missing;
                    size += ReadPadding((ulong)sizeOfInstanceBits, readInstanceSizeBits, out missing);
                    descriptor.Padding = missing;
                    this.Logger.LogDebug($"Descriptor \'{tag}\' has extra padding of {missing.Length} bytes");
                }
                else
                {
                    this.Logger.LogDebug($"Descriptor \'{tag}\' read through!");
                }
            }
            size += readInstanceSizeBits;

            ulong calculatedSize = descriptor.CalculateSize();
            if (calculatedSize != (ulong)sizeOfInstanceBits)
            {
                this.Logger.LogDebug($"Calculated descriptor \'{tag}\' size: {calculatedSize >> 3}, read: {sizeOfInstanceBits >> 3}");
            }

            LogEnd(name, size, descriptor);

            return size;
        }

        public ulong ReadDescriptor<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, out T[] descriptor, string name) where T : Descriptor
        {
            ulong consumed = 0;

            List<T> descriptors = new List<T>();

            if (readSize == ulong.MaxValue)
            {
                // consume till the end of the stream
                try
                {
                    while (true)
                    {
                        T v;
                        consumed += ReadDescriptor(consumed, readSize, parent, out v, name);
                        descriptors.Add(v);
                    }
                }
                catch (EndOfStreamException)
                { }

                descriptor = descriptors.ToArray();

                return consumed;
            }

            ulong remaining = readSize - boxSize;
            while (consumed < remaining)
            {
                T v;
                consumed += ReadDescriptor(consumed, remaining, parent, out v, name);
                descriptors.Add(v);
            }

            descriptor = descriptors.ToArray();

            return consumed;
        }

        public ulong ReadDescriptorsTillEnd(ulong boxSize, ulong readSize, Descriptor descriptor, int objectTypeIndication = -1)
        {
            if (descriptor.Children != null)
            {
                this.Logger.LogDebug($"Descriptor reading repeated Children read");
                return 0;
            }

            descriptor.Children = new List<Descriptor>();

            ulong consumed = 0;

            if (readSize == ulong.MaxValue)
            {
                // consume till the end of the stream
                try
                {
                    while (true)
                    {
                        Descriptor v;
                        consumed += ReadDescriptor(consumed, readSize, descriptor, out v, "");
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
                consumed += ReadDescriptor(consumed, remaining, descriptor, out v, "");
                descriptor.Children.Add(v);
            }
            return consumed;
        }

        public static ulong CalculateDescriptors(Descriptor descriptor, int objectTypeIndication = -1)
        {
            ulong size = 0;

            if (descriptor.Children == null)
                return 0;

            foreach (var child in descriptor.Children)
            {
                size += CalculateDescriptorSize(child);
            }

            return size;
        }

        public static ulong CalculateDescriptorSize(Descriptor descriptor)
        {
            if (descriptor == null)
                return 8;

            ulong descriptorContentSize = descriptor.CalculateSize();
            ulong descriptorSizeLength = CalculatePackedNumberLength(descriptorContentSize >> 3, descriptor.SizeOfSize >> 3);
            return 8 * (1 + descriptorSizeLength) + descriptorContentSize + 8 * (ulong)(descriptor.Padding != null ? descriptor.Padding.Length : 0);
        }

        public static ulong CalculateDescriptorSize(Descriptor[] descriptor)
        {
            if (descriptor == null || descriptor.Length == 0)
                return 0;

            ulong size = 0;
            for (int i = 0; i < descriptor.Length; i++)
            {
                size += CalculateDescriptorSize(descriptor[i]);
            }

            return size;
        }

        public ulong WriteDescriptorsTillEnd(Descriptor descriptor, int objectTypeIndication = -1)
        {
            ulong size = 0;
            foreach (var d in descriptor.Children)
            {
                size += WriteDescriptor(d, "");
            }
            return size;
        }

        public ulong WriteDescriptor(Descriptor descriptor, string name)
        {
            LogBegin(name);

            ulong size = 0;
            if (descriptor == null || descriptor.Tag == 0)
            {
                return size;
            }
            size += WriteUInt8(descriptor.Tag, "");

            ulong sizeOfInstance = descriptor.CalculateSize() + 8 * (ulong)(descriptor.Padding != null ? descriptor.Padding.Length : 0);
            size += WriteDescriptorSize(sizeOfInstance >> 3, descriptor.SizeOfSize >> 3);
            size += descriptor.Write(this);
            if (descriptor.Padding != null)
            {
                size += WritePadding(descriptor.Padding);
            }

            LogEnd(name, size, descriptor);

            return size;
        }

        public ulong WriteDescriptor(Descriptor[] descriptor, string name)
        {
            ulong size = 0;
            if (descriptor == null || descriptor.Length == 0)
            {
                return size;
            }
            
            for (int i = 0; i < descriptor.Length; i++)
            {
                size += WriteDescriptor(descriptor[i], "");
            }

            LogEnd(name, size, descriptor);

            return size;
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

            return sizeBytes << 3;
        }

        private ulong WriteDescriptorSize(ulong sizeOfInstance, ulong sizeOfSize)
        {
            uint sizeBytesCount = CalculatePackedNumberLength(sizeOfInstance, sizeOfSize);

            ulong i = 0;
            byte[] buffer = new byte[sizeBytesCount];
            if (sizeOfInstance > 0)
            {
                while (sizeOfInstance > 0 || i < sizeOfSize)
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
            }

            foreach (byte b in buffer)
            {
                WriteByte(b);
            }

            return sizeBytesCount << 3;
        }

        private static uint CalculatePackedNumberLength(ulong sizeInBytes, ulong sizeOfSize)
        {
            int size = (int)sizeInBytes;
            int i = 0;
            while (size > 0 || i < (int)sizeOfSize) // i < (int)sizeOfSize => sometimes the descriptor size is aligned to 4 bytes, so this is to make sure we match the original length
            {
                size = (int)((uint)size >> 7);
                i++;
            }
            return (uint)i;
        }

        #endregion // Descriptors

        #region Long number arrays

        public ulong ReadUInt8ArrayTillEnd(ulong boxSize, ulong readSize, out StreamMarker value, string name)
        {
            StreamMarker marker;
            ulong size;
            if (CanStreamSeek())
            {
                if (readSize == ulong.MaxValue)
                {
                    marker = new StreamMarker(GetCurrentOffset(), GetStreamLength() - GetCurrentOffset(), this);
                    SeekFromEnd(0);
                    value = marker;
                    size = (ulong)(marker.Length << 3);
                    LogEnd(name, size, value);
                    return size;
                }
                else
                {
                    long remaining = (long)(readSize - boxSize);
                    long count = remaining >> 3;
                    marker = new StreamMarker(GetCurrentOffset(), count, this);
                    SeekFromCurrent(count);
                    value = marker;
                    size = (ulong)(marker.Length << 3);
                    LogEnd(name, size, value);
                    return size;
                }
            }
            else
            {
                // we cannot seek, use the temporary storage for offloading
                var storage = GetOrCreateTemporaryStorage();
                storage.SeekFromEnd(0); // move at the end of our storage
                long offset = storage.GetCurrentOffset();

                if (readSize == ulong.MaxValue)
                {
                    ulong count = CopyStream(_stream, storage._stream);

                    marker = new StreamMarker(offset, (long)count, storage);
                    value = marker;
                    size = count << 3;
                    LogEnd(name, size, value);
                    return size;
                }
                else
                {
                    long remaining = (long)(readSize - boxSize);
                    long count = remaining >> 3;
                    ulong copied = CopyStream(_stream, storage._stream, count);

                    marker = new StreamMarker(offset, count, storage);
                    value = marker;
                    size = copied << 3;
                    LogEnd(name, size, value);
                    return size;
                }
            }
        }

        public ulong WriteUInt8ArrayTillEnd(StreamMarker data, string name)
        {
            IsoStream readStream = data.Stream;
            long originalPosition = readStream.GetCurrentOffset();
            readStream.SeekFromBeginning(data.Position);
            ulong size = CopyStream(readStream._stream, _stream, data.Length) << 3;
            readStream.SeekFromBeginning(originalPosition); // because in our test app we're reading and writing at the same time from the same thread, we have to restore the original position
            LogEnd(name, size, data);
            return size;
        }

        #endregion // Long number arrays

        #region Number arrays

        public static ulong CalculateSize<T>(ulong entry_count, T[] entries, int entrySize)
        {
            // there might be no entries at all, even though the size says 1 (e.g. sync box)
            if (entries == null || entries.Length == 0)
                return 0;

            // there might be more entries than the size says
            return Math.Max(entry_count, (ulong)entries.Length) * (ulong)entrySize;
        }

        public ulong ReadUInt8ArrayTillEnd(ulong boxSize, ulong readSize, out byte[] value, string name)
        {
            if (readSize == ulong.MaxValue)
            {
                return ReadBytes(readSize >> 3, out value);
            }

            ulong remaining = readSize - boxSize;
            ulong count = remaining >> 3;
            return ReadBytes(count, out value);
        }

        public ulong ReadUInt32ArrayTillEnd(ulong boxSize, ulong readSize, out uint[] value, string name)
        {
            ulong consumed = 0;

            if (readSize == ulong.MaxValue)
            {
                List<uint> values = new List<uint>();

                try
                {
                    while (true)
                    {
                        uint v;
                        consumed += ReadUInt32(boxSize + consumed, readSize, out v, "");
                        values.Add(v);
                    }
                }
                catch (EndOfStreamException)
                { }

                value = values.ToArray();

                LogEnd(name, consumed, value);
                return consumed;
            }

            ulong remaining = readSize - boxSize;
            uint count = (uint)(remaining >> 5);
            value = new uint[count];
            for (uint i = 0; i < count; i++)
            {
                consumed += ReadUInt32(boxSize + consumed, readSize, out value[i], "");
            }

            LogEnd(name, consumed, value);
            return consumed;
        }

        public ulong ReadUInt16Array(ulong boxSize, ulong readSize, uint count, out ushort[] value, string name)
        {
            ulong size = 0;
            value = new ushort[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt16(boxSize + size, readSize, out value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteUInt16Array(uint count, ushort[] value, string name)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt16(value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadUInt16Array(ulong boxSize, ulong readSize, uint count, out uint[] value, string name)
        {
            ulong size = 0;
            value = new uint[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt16(boxSize + size, readSize, out value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }
        
        public ulong WriteUInt16Array(uint count, uint[] value, string name)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt16(value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadUInt32Array(ulong boxSize, ulong readSize, uint count, out uint[] value, string name)
        {
            ulong size = 0;
            value = new uint[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt32(boxSize + size, readSize, out value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadInt32Array(ulong boxSize, ulong readSize, uint count, out int[] value, string name)
        {
            ulong size = 0;
            value = new int[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadInt32(boxSize + size, readSize, out value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteUInt32Array(uint count, uint[] value, string name)
        {
            ulong size = 0;

            if (value == null)
                return size;

            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt32(value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteInt32Array(uint count, int[] value, string name)
        {
            ulong size = 0;

            if (value == null)
                return size;

            for (uint i = 0; i < count; i++)
            {
                size += WriteInt32(value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadUInt32Array(ulong boxSize, ulong readSize, uint count, out ulong[] value, string name)
        {
            ulong size = 0;
            value = new ulong[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt32(boxSize + size, readSize, out value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteUInt32Array(uint count, ulong[] value, string name)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt32(value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadUInt64Array(ulong boxSize, ulong readSize, uint count, out ulong[] value, string name)
        {
            ulong size = 0;
            value = new ulong[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt64(boxSize + size, readSize, out value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteUInt64Array(uint count, ulong[] value, string name)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt64(value[i], "");
            }

            LogEnd(name, size, value);
            return size;
        }

        #endregion // Number arrays

        #region Numbers 

        public ulong ReadInt8(ulong boxSize, ulong readSize, out sbyte value, string name)
        {
            ulong count = unchecked(ReadUInt8(boxSize, readSize, out byte v, ""));
            value = unchecked((sbyte)v);
            
            LogEnd(name, count, value);
            return count;
        }

        public ulong WriteInt8(sbyte value, string name)
        {
            ulong size = WriteUInt8(unchecked((byte)value), "");
            
            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadUInt8(ulong boxSize, ulong readSize, out byte value, string name)
        {
            value = ReadByte();
            
            LogEnd(name, 8, value);
            return 8;
        }

        public ulong WriteUInt8(byte value, string name)
        {
            WriteByte(value); 
            
            LogEnd(name, 8, value);
            return 8;
        }

        public ulong ReadUInt8(ulong boxSize, ulong readSize, out ushort value, string name)
        {
            value = ReadByte();
           
            LogEnd(name, 8, value);
            return 8;
        }

        public ulong WriteUInt8(ushort value, string name)
        {
            ulong size = WriteByte((byte)value);
            
            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadUInt8(ulong boxSize, ulong readSize, out uint value, string name)
        {
            value = ReadByte();
            
            LogEnd(name, 8, value);
            return 8;
        }

        public ulong ReadInt16(ulong boxSize, ulong readSize, out short value, string name)
        {
            ulong count = ReadUInt16(boxSize, readSize, out ushort v, "");
            value = unchecked((short)v);
            
            LogEnd(name, count, value);
            return count;
        }

        public ulong WriteInt16(short value, string name)
        {
            return WriteUInt16(unchecked((ushort)value), name);
        }

        public ulong ReadUInt16(ulong boxSize, ulong readSize, out ushort value, string name)
        {
            int b1 = ReadByteInternal();
            if(b1 == -1)
            {
                throw new IsoEndOfStreamException();
            }

            int b2 = ReadByteInternal();
            if (b2 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1 });
            }

            value = (ushort)(
                ((ushort)b1 << 8) +
                ((ushort)b2)
            );
           
            LogEnd(name, 16, value);
            return 16;
        }

        public ulong WriteUInt16(ushort value, string name)
        {
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            
            LogEnd(name, 16, value);
            return 16;
        }

        public ulong ReadUInt16(ulong boxSize, ulong readSize, out uint value, string name)
        {
            ushort v;
            ulong size = ReadUInt16(boxSize, readSize, out v, name);
            value = v;
            return size;
        }

        public ulong WriteUInt16(uint value, string name)
        {
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
           
            LogEnd(name, 16, value);
            return 16;
        }

        public ulong ReadUInt24(ulong boxSize, ulong readSize, out uint value, string name)
        {
            int b1 = ReadByteInternal();
            if(b1 == -1)
            {
                throw new IsoEndOfStreamException();
            }

            int b2 = ReadByteInternal();
            if(b2 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1 });
            }

            int b3 = ReadByteInternal();
            if (b3 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2 });
            }

            value = (uint)(
                ((uint)b1 << 16) +
                ((uint)b2 << 8) +
                ((uint)b3)
            );
            
            LogEnd(name, 24, value);
            return 24;
        }

        public ulong WriteUInt24(uint value, string name)
        {
            value = value & 0xFFFFFF;
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            
            LogEnd(name, 24, value);
            return 24;
        }

        public ulong ReadInt32(ulong boxSize, ulong readSize, out int value, string name)
        {
            ulong count = ReadUInt32(boxSize, readSize, out uint v, "");
            value = unchecked((int)v);

            LogEnd(name, count, value);
            return count;
        }

        public ulong ReadInt32(ulong boxSize, ulong readSize, out byte value, string name)
        {
            ulong count = ReadUInt32(boxSize, readSize, out uint v, name);
            value = (byte)v;
            return count;
        }

        public ulong WriteInt32(int value, string name)
        {
            ulong size = WriteUInt32(unchecked((uint)value), "");

            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadInt32(ulong boxSize, ulong readSize, out long value, string name)
        {
            ulong count = unchecked(ReadUInt32(boxSize, readSize, out uint v, ""));
            value = unchecked((int)v);
            
            LogEnd(name, count, value);
            return count;
        }

        public ulong WriteInt32(long value, string name)
        {
            ulong size = WriteUInt32(unchecked((uint)value), "");

            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadUInt32(ulong boxSize, ulong readSize, out uint value, string name)
        {
            int b1 = ReadByteInternal();
            if(b1 == -1) 
            { 
                throw new IsoEndOfStreamException();
            }

            int b2 = ReadByteInternal();
            if(b2 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1 });
            }

            int b3 = ReadByteInternal();
            if (b3 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2 });
            }

            int b4 = ReadByteInternal();
            if (b4 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3 });
            }

            value = (uint)(
                ((uint)b1 << 24) +
                ((uint)b2 << 16) +
                ((uint)b3 << 8) +
                ((uint)b4)
            );

            LogEnd(name, 32, value);
            return 32;
        }

        public ulong WriteUInt32(uint value, string name)
        {
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            LogEnd(name, 32, value);
            return 32;
        }

        public ulong ReadUInt32(ulong boxSize, ulong readSize, out ulong value, string name)
        {
            uint v;
            ulong size = ReadUInt32(boxSize, readSize, out v, name);
            value = v;
            return size;
        }

        public ulong WriteUInt32(ulong value, string name)
        {
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            LogEnd(name, 32, value);
            return 32;
        }

        public ulong ReadUInt48(ulong boxSize, ulong readSize, out ulong value, string name)
        {
            int b1 = ReadByteInternal();
            if (b1 == -1)
            {
                throw new IsoEndOfStreamException();
            }

            int b2 = ReadByteInternal();
            if (b2 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1 });
            }

            int b3 = ReadByteInternal();
            if (b3 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2 });
            }

            int b4 = ReadByteInternal();
            if (b4 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3 });
            }

            int b5 = ReadByteInternal();
            if (b5 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3, (byte)b4 });
            }

            int b6 = ReadByteInternal();
            if (b6 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3, (byte)b4, (byte)b5 });
            }

            value = (ulong)(
                ((ulong)b1 << 40) +
                ((ulong)b2 << 32) +
                ((ulong)b3 << 24) +
                ((ulong)b4 << 16) +
                ((ulong)b5 << 8) +
                ((ulong)b6)
            );
            LogEnd(name, 48, value);
            return 48;
        }

        public ulong WriteUInt48(ulong value, string name)
        {
            WriteByte((byte)(value >> 40 & 0xFF));
            WriteByte((byte)(value >> 32 & 0xFF));
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            LogEnd(name, 48, value);
            return 48;
        }

        public ulong ReadInt64(ulong boxSize, ulong readSize, out long value, string name)
        {
            ulong count = unchecked(ReadUInt64(boxSize, readSize, out ulong v, ""));
            value = unchecked((long)v);
            
            LogEnd(name, count, value);
            return count;
        }

        public ulong WriteInt64(long value, string name)
        {
            ulong size = WriteUInt64(unchecked((ulong)value), "");

            LogEnd(name, size, value);
            return size;
        }

        public ulong ReadUInt64(ulong boxSize, ulong readSize, out ulong value, string name)
        {
            int b1 = ReadByteInternal();
            if (b1 == -1)
            {
                throw new IsoEndOfStreamException();
            }

            int b2 = ReadByteInternal();
            if (b2 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1 });
            }

            int b3 = ReadByteInternal();
            if (b3 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2 });
            }

            int b4 = ReadByteInternal();
            if (b4 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3 });
            }

            int b5 = ReadByteInternal();
            if (b5 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3, (byte)b4 });
            }

            int b6 = ReadByteInternal();
            if (b6 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3, (byte)b4, (byte)b5 });
            }

            int b7 = ReadByteInternal();
            if (b7 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3, (byte)b4, (byte)b5, (byte)b6 });
            }

            int b8 = ReadByteInternal();
            if (b8 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3, (byte)b4, (byte)b5, (byte)b6, (byte)b7 });
            }

            value = (ulong)(
                ((ulong)b1 << 56) +
                ((ulong)b2 << 48) +
                ((ulong)b3 << 40) +
                ((ulong)b4 << 32) +
                ((ulong)b5 << 24) +
                ((ulong)b6 << 16) +
                ((ulong)b7 << 8) +
                ((ulong)b8)
            );

            LogEnd(name, 64, value);
            return 64;
        }

        public ulong WriteUInt64(ulong value, string name)
        {
            WriteByte((byte)(value >> 56 & 0xFF));
            WriteByte((byte)(value >> 48 & 0xFF));
            WriteByte((byte)(value >> 40 & 0xFF));
            WriteByte((byte)(value >> 32 & 0xFF));
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));

            LogEnd(name, 64, value);
            return 64;
        }

        public ulong ReadFixedPoint1616(ulong boxSize, ulong readSize, out double value, string name)
        {
            int b1 = ReadByteInternal();
            if (b1 == -1)
            {
                throw new IsoEndOfStreamException();
            }

            int b2 = ReadByteInternal();
            if (b2 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1 });
            }

            int b3 = ReadByteInternal();
            if (b3 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2 });
            }

            int b4 = ReadByteInternal();
            if (b4 == -1)
            {
                throw new IsoEndOfStreamException(new byte[] { (byte)b1, (byte)b2, (byte)b3 });
            }

            value = (double)(
                ((uint)b1 << 24) +
                ((uint)b2 << 16) +
                ((uint)b3 << 8) +
                ((uint)b4)
            ) / 65536d;

            LogEnd(name, 32, value);
            return 32;
        }

        public ulong WriteFixedPoint1616(double value, string name)
        {
            ulong size = 0;
            int result = (int)(value * 65536);
            size += WriteByte((byte)((result & 0xFF000000) >> 24));
            size += WriteByte((byte)((result & 0x00FF0000) >> 16));
            size += WriteByte((byte)((result & 0x0000FF00) >> 8));
            size += WriteByte((byte)(result & 0x000000FF));

            LogEnd(name, size, value);
            return size;
        }

        public static int GetInt(byte[] bytes)
        {
            if (bytes.Length == 0)
                return 0;
            else if (bytes.Length == 1)
                return bytes[0];
            else if (bytes.Length == 2)
                return (int)(((int)bytes[0] << 8) + ((int)bytes[1]));
            else if (bytes.Length == 4)
                return (int)(((int)bytes[0] << 24) + ((int)bytes[1] << 16) + ((int)bytes[2] << 8) + ((int)bytes[3]));
            else
                throw new NotSupportedException();
        }

        public static int GetInt(int size)
        {
            return size;
        }

        public static int GetInt(uint size)
        {
            return (int)size;
        }

        public static int GetInt(long size)
        {
            return (int)size;
        }

        #endregion // Numbers

        #region Iso639

        public ulong ReadIso639(ulong boxSize, ulong readSize, out string value, string name)
        {
            ushort bits;
            ulong size = ReadBits(boxSize, readSize, 15, out bits, name);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                int c = bits >> (2 - i) * 5 & 0x1f;
                result.Append((char)(c + 0x60));
            }
            value = result.ToString();

            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteIso639(string value, string name)
        {
            if (Encoding.UTF8.GetBytes(value).Length != 3)
            {
                throw new ArgumentException($"\"{value}\" value string must be 3 characters long!");
            }

            int bits = 0;
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            for (int i = 0; i < 3; i++)
            {
                bits += bytes[i] - 0x60 << (2 - i) * 5;
            }
            ulong size = WriteBits(15, (ushort)bits, name);

            LogEnd(name, size, value);
            return size;
        }

        #endregion // Iso639

        #region Proxy

        public ulong ReadDouble32(ulong boxSize, ulong readSize, out double value, string name)
        {
            // TODO?
            return ReadFixedPoint1616(boxSize, readSize, out value, name);
        }

        public ulong WriteDouble32(double value, string name)
        {
            // TODO?
            return WriteFixedPoint1616(value, name);
        }

        public ulong ReadPadding(ulong size, ulong availableSize, out StreamMarker padding)
        {
            return ReadUInt8ArrayTillEnd(size, availableSize, out padding, "");
        }

        public ulong WritePadding(StreamMarker padding)
        {
            return WriteUInt8ArrayTillEnd(padding, "");
        }

        public static ulong CalculateClassSize(IMp4Serializable value)
        {
            return value.CalculateSize();
        }

        public static ulong CalculateBoxArray(IHasBoxChildren value)
        {
            return CalculateBoxSize(value.Children);
        }

        public ulong WriteUInt8ArrayTillEnd(byte[] value, string name)
        {
            return WriteUInt8Array((uint)value.Length, value, name);
        }

        public ulong WriteUInt32ArrayTillEnd(uint[] value, string name)
        {
            return WriteUInt32Array((uint)value.Length, value, name);
        }

        public ulong ReadUInt8Array(ulong boxSize, ulong readSize, uint count, out byte[] value, string name)
        {
            ulong size = ReadBytes(count, out value);
           
            LogEnd(name, size, value);
            return size;
        }

        public ulong WriteUInt8Array(uint count, byte[] value, string name)
        {
            ulong size = WriteBytes(count, value);

            LogEnd(name, size, value);
            return size;
        }

        #endregion Proxy

        #region MDAT

        public ulong ReadVariableLengthSize(uint nalLengthSize, out uint nalUnitLength)
        {
            ulong size = 0;
            switch (nalLengthSize)
            {
                case 1:
                    size += ReadUInt8(size, ulong.MaxValue, out nalUnitLength, "");
                    break;
                case 2:
                    size += ReadUInt16(size, ulong.MaxValue, out nalUnitLength, "");
                    break;
                case 3:
                    size += ReadUInt24(size, ulong.MaxValue, out nalUnitLength, "");
                    break;
                case 4:
                    size += ReadUInt32(size, ulong.MaxValue, out nalUnitLength, "");
                    break;
                default:
                    throw new NotSupportedException($"NAL unit length {nalLengthSize} not supported!");
            }
            return size;
        }

        public ulong WriteVariableLengthSize(uint nalLengthSize, uint nalUnitLength)
        {
            ulong size = 0;
            switch (nalLengthSize)
            {
                case 1:
                    if (nalUnitLength > byte.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                    size += WriteUInt8((byte)nalUnitLength, "");
                    break;
                case 2:
                    if (nalUnitLength > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                    size += WriteUInt16(nalUnitLength, "");
                    break;
                case 3:
                    if (nalUnitLength > 16777215) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                    size += WriteUInt24(nalUnitLength, "");
                    break;
                case 4:
                    if (nalUnitLength > uint.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                    size += WriteUInt32(nalUnitLength, "");
                    break;
                default:
                    throw new NotSupportedException($"NAL unit length {nalLengthSize} not supported!");
            }
            return size;
        }

        #endregion // MDAT

        #region Logging

        private int _logLevel = 0;

        private void LogBegin(string name)
        {
            string padding = "-";
            for (int i = 0; i < _logLevel; i++)
            {
                padding += "-";
            }

            this.Logger.LogInfo($"{padding} {name}");
        }

        private void LogEnd<T>(string name, ulong size, T value)
        {
            if (string.IsNullOrEmpty(name))
                return;

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
                    // TODO: dispose managed state (managed objects)
                    _stream.Dispose();
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

    public class StreamMarker : IDisposable
    {
        private bool disposedValue;

        public long Position { get; set; }
        public long Length { get; set; }
        public IsoStream Stream { get; set; }
        public StreamMarker(long position, long length, IsoStream stream)
        {
            Position = position;
            Length = length;
            Stream = stream;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class IsoEndOfStreamException : EndOfStreamException
    {
        public StreamMarker Padding { get; set; }
        public byte[] PaddingBytes { get; set; }

        public IsoEndOfStreamException()
        {  }

        public IsoEndOfStreamException(StreamMarker padding)
        {
            Padding = padding;
        }

        public IsoEndOfStreamException(byte[] padding)
        {
            PaddingBytes = padding;
        }
    }

    public struct BinaryUTF8String
    {
        public bool IsZeroTerminated { get; set; } = true;
        
        public int Length { get { return Bytes == null ? 0 : Bytes.Length; } }
        
        public byte[] Bytes { get; set; }

        public BinaryUTF8String(byte[] bytes)
        {
            this.Bytes = bytes;
        }
        public BinaryUTF8String(string text)
        {
            Bytes = GetBytes(text);
        }

        public override string ToString()
        {
            return GetString(Bytes);
        }

        public static byte[] GetBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public static string GetString(byte[] text)
        {
            return Encoding.UTF8.GetString(text);
        }
    }

    public class MultiLanguageString
    {
        public ushort Language { get; set; }
        public ushort Length { get; set; }
        public BinaryUTF8String Value { get; set; }

        public MultiLanguageString(ushort language, ushort length, BinaryUTF8String value)
        {
            this.Language = language;
            this.Length = length;
            this.Value = value;
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
}
