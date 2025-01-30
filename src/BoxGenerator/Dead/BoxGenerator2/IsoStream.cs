
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpMP4
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

        public IsoStream(Stream stream, ITemporaryStorageFactory storageFactory = null) : this(new StreamWrapper(stream), storageFactory)
        { }

        public IsoStream(IStorage stream, ITemporaryStorageFactory storageFactory = null)
        {
            _stream = stream;
            _storageFactory = storageFactory ?? new TemporaryMemoryStorageFactory();
        }

        private IsoStream GetOrCreateTemporaryStorage()
        {
            if (_temp == null)
                _temp = new IsoStream(_storageFactory.Create());
            return _temp;
        }

        #region Stream operations

        internal bool CanStreamSeek()
        {
            return _stream.CanStreamSeek();
        }

        internal long GetCurrentOffset()
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
                Log.Debug($"Getting the current stream offset failed: {e.Message}");
                return -1;
            }
        }

        internal long GetStreamLength()
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
                Log.Debug($"Getting the current stream length failed: {e.Message}");
                return -1;
            }
        }

        private void SeekFromEnd(long offset)
        {
            _stream.SeekFromEnd(offset);
        }

        private void SeekFromCurrent(long offset)
        {
            _stream.SeekFromCurrent(offset);
        }

        private void SeekFromBeginning(long offset)
        {
            _stream.SeekFromBeginning(offset);
        }

        #endregion // Stream operations

        #region Basic read/write operations

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

        internal ulong ReadBytes(ulong length, out byte[] value)
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

        internal ulong WriteBits(uint count, short value)
        {
            if (count > 16)
                throw new ArgumentOutOfRangeException(nameof(count));
            return WriteBits(count, unchecked((ushort)value));
        }

        internal ulong WriteBits(uint count, uint value)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            uint originalCount = count;
            while (count > 0)
            {
                int bits = (int)count - 1;
                uint mask = 0x1u << bits;
                WriteBit((int)((value & mask) >> bits));
                count--;
            }
            return originalCount;
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

        #endregion // Bits

        #region Strings

        internal ulong WriteStringZeroTerminated(BinaryUTF8String value)
        {
            if (value.Length == 0)
                return 0;

            byte[] buffer = value.Bytes;
            for (int i = 0; i < value.Length; i++)
            {
                WriteByte(buffer[i]);
            }
            return (ulong)value.Length << 3;
        }

        internal ulong ReadStringZeroTerminated(ulong boxSize, ulong readSize, out BinaryUTF8String value)
        {
            ulong remaining = readSize - boxSize;
            if (remaining == 0)
            {
                value = new BinaryUTF8String();
                return 0;
            }

            ulong read = ReadBytes(remaining >> 3, out byte[] buffer);
            value = new BinaryUTF8String(buffer);
            Log.Debug($"ReadString: {value}");
            Log.Debug("");
            return read;
        }

        internal ulong ReadStringSizePrefixed(ulong boxSize, ulong readSize, out byte[] value)
        {
            ulong remaining = readSize - boxSize;
            if (remaining == 0)
            {
                value = [];
                return 0;
            }

            List<byte> buffer = new List<byte>();
            int size = ReadByte();
            int read = 0;
            while (read < size && (read + 1 < (int)(remaining >> 3)))
            {
                buffer.Add(ReadByte());
                read++;
            }
            value = buffer.ToArray();
            return (ulong)(read + 1) << 3;
        }

        internal ulong WriteStringSizePrefixed(string value)
        {
            ulong size = 0;
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            if (bytes.Length > 255)
                throw new ArgumentOutOfRangeException(nameof(value));
            size += WriteByte((byte)bytes.Length);
            size += WriteBytes((ulong)bytes.Length, bytes);
            return size;
        }

        internal static ulong CalculateStringSize(BinaryUTF8String value)
        {
            ulong count = (ulong)value.Length;
            return count << 3;
        }

        internal static ulong CalculateStringSize(BinaryUTF8String[] value)
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

        internal static uint FromFourCC(string input)
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

        #endregion // FourCC

        #region Boxes

        public Box ReadBox()
        {
            SafeBoxHeader header = ReadBoxHeader();
            ulong readSize = header.GetBoxSizeInBits();
            Box box = ReadBoxContent(header);
            return box;
        }

        public ulong WriteBox(Box value)
        {
            ulong writtenSize = WriteBoxHeader(value);
            writtenSize += WriteBoxContent(value);
            if (value.Padding != null)
            {
                writtenSize += WritePadding(value.Padding);
            }
            return writtenSize;
        }

        public SafeBoxHeader ReadBoxHeader()
        {
            SafeBoxHeader header = new SafeBoxHeader();
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
            return header;
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
                header.Type = FromFourCC(value.FourCC);
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

        public Box ReadBoxContent(SafeBoxHeader header)
        {
            if (header.Type == 0 && header.Size == 0 && header.Largesize == 0)
                return null; // all zeros, looks like 8 zero bytes padding at the end of the file

            string fourCC = ToFourCC(header.Type);
            var box = BoxFactory.CreateBox(fourCC, null, header.Usertype);
            ReadBox(header, box, GetBoxSize(header) - GetHeaderSize(header));
            return box;
        }

        public ulong WriteBoxContent(Box box)
        {
            ulong size = box.Write(this);
            return size;
        }

        internal ulong ReadBox<T>(ulong boxSize, ulong readSize, Func<SafeBoxHeader, Box> factory, IMp4Serializable parent, out T value) where T : Box
        {
            var header = ReadBoxHeader();
            ulong availableSize = readSize - boxSize - GetHeaderSize(header);
            Box box;
            ulong size;

            if (GetBoxSize(header) - GetHeaderSize(header) > availableSize)
            {
                // make sure we do not modify any bytes
                box = new InvalidBox();
                box.Parent = parent;
                box.Header = header;
                Log.Debug($"BOX:{GetIndentation(box)}\'{box.FourCC}\'");
                size = box.Read(this, availableSize) + GetHeaderSize(header);
            }
            else
            {
                box = factory(header);
                box.Parent = parent;
                Log.Debug($"BOX:{GetIndentation(box)}\'{box.FourCC}\'");
                size = ReadBox(header, box, availableSize);
            }

            value = (T)box;
            return size;
        }

        private static Box DefaultBoxFactory(IMp4Serializable parent, SafeBoxHeader header)
        {
            return BoxFactory.CreateBox(ToFourCC(header.Type), (parent as Box)?.FourCC, header.Usertype);
        }

        internal ulong ReadBox<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, out T value) where T : Box
        {
            return ReadBox(boxSize, readSize, 
                (header) => DefaultBoxFactory(parent, header),
                parent,
                out value);    
        }

        internal ulong ReadBox<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, out T[] value) where T : Box
        {
            return ReadBox<T>(boxSize, readSize,
                (header) => DefaultBoxFactory(parent, header),
                parent,
                out value);
        }

        internal ulong ReadBox<T>(ulong boxSize, ulong readSize, Func<SafeBoxHeader, Box> factory, IMp4Serializable parent, out T[] value) where T : Box
        {
            var boxes = new List<T>();

            ulong consumed = 0;

            if (readSize == 0)
            {
                List<Box> values = new List<Box>();
                // consume till the end of the stream
                try
                {
                    while (true)
                    {
                        T v;
                        consumed += ReadBox<T>(consumed, readSize, factory, parent, out v);
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
                consumed += ReadBox<T>(consumed, readSize, factory, parent, out v);
                boxes.Add(v);
            }
            value = boxes.ToArray();
            return consumed;
        }

        internal ulong WriteBox(Box[] value)
        {
            ulong size = 0;
            foreach (var box in value)
            {
                size += WriteBox(box);
            }
            return size;
        }

        internal ulong ReadBoxArrayTillEnd(ulong boxSize, ulong readSize, Box box)
        {
            if (box.Children != null)
            {
                Log.Debug($"Box reading {box.FourCC} repeated Children read");
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
                        ulong readBoxSize = ReadBox(consumed, readSize, box, out v);
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
                consumed += ReadBox(consumed, remaining, box, out v);
                if (consumed > readSize)
                {
                    Log.Debug($"Box \'{v.FourCC}\' read through!");
                    break;
                }
                box.Children.Add(v);
            }
            return consumed;
        }

        internal ulong WriteBoxArrayTillEnd(Box box)
        {
            if (box.Children == null)
                return 0;

            ulong written = 0;
            foreach (var v in box.Children)
            {
                written += WriteBox(v);
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
                    Log.Debug($"Box \'{box.FourCC}\' has extra padding of {missing.Length} bytes");
                }
                else
                {
                    throw new Exception($"Box \'{box.FourCC}\' read through!");
                }
            }

            ulong calculatedSize = box.CalculateSize();
            if (calculatedSize != GetBoxSize(header))
            {
                if (box.FourCC != "mdat")
                    Log.Debug($"Calculated \'{box.FourCC}\' size: {calculatedSize / 8}, read: {GetBoxSize(header) / 8}");
            }

            return size + GetHeaderSize(header);
        }

        private static string GetIndentation(IMp4Serializable b, string c = "-")
        {
            string indent = "";
            while (b != null)
            { 
                b = b.Parent; 
                indent += c;
            }
            return indent;
        }

        internal static ulong CalculateBoxSize(IEnumerable<Box> boxes)
        {
            ulong size = 0;
            if (boxes == null)
                return 0;

            foreach (Box box in boxes)
            {
                size += CalculateBoxSize(box);
            }
            return size;
        }

        #endregion // Boxes

        #region Classes

        internal ulong ReadClass<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, T c, out T value) where T : IMp4Serializable
        {
            ulong size = c.Read(this, readSize - boxSize);
            value = c;
            c.Parent = parent;
            Log.Debug($"CLS:{GetIndentation(c)}{c.DisplayName}");
            return size;
        }

        internal ulong ReadClass<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, out T[] value) where T : IMp4Serializable, new()
        {
            ulong consumed = 0;
            ulong remaining = readSize - boxSize;
            List<T> ret = new List<T>();
            while (consumed < remaining)
            {
                T c;
                consumed += ReadClass<T>(boxSize + consumed, remaining, parent, new T(), out c);
                if (consumed > readSize)
                {
                    throw new Exception($"Class read through!");
                }
                c.Parent = parent;
                ret.Add(c);
            }
            value = ret.ToArray();
            return consumed;
        }

        internal ulong WriteClass(IMp4Serializable value)
        {
            ulong writtenSize = 0;
            writtenSize += value.Write(this);
            if (value.Padding != null)
            {
                writtenSize += WritePadding(value.Padding);
            }
            return writtenSize;
        }

        internal ulong WriteClass(IMp4Serializable[] value)
        {
            ulong size = 0;
            foreach (var cls in value)
            {
                size += WriteClass(cls);
            }
            return size;
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

        #endregion // Classes

        #region Entries

        internal ulong WriteEntry(IMp4Serializable value)
        {
            ulong writtenSize = 0;
            writtenSize += value.Write(this);
            if (value.Padding != null)
            {
                writtenSize += WritePadding(value.Padding);
            }
            return writtenSize;
        }

        internal ulong ReadEntry<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, string fourCC, out T entry) where T : SampleGroupDescriptionEntry
        {
            var res = BoxFactory.CreateEntry(fourCC);
            res.Parent = parent;
            Log.Debug($"ENT:{GetIndentation(res)}\'{res.DisplayName}\'");
            ulong size = res.Read(this, readSize);
            entry = (T)res;
            return size;
        }

        internal static ulong CalculateEntrySize(IMp4Serializable[] entry)
        {
            ulong size = 0;
            foreach (var e in entry)
            {
                size += CalculateEntrySize(e);
            }
            return size;
        }

        #endregion // Entries

        #region Descriptors

        internal ulong ReadDescriptor<T>(ulong boxSize, ulong readSize, IMp4Serializable parent, out T descriptor) where T : Descriptor
        {
            long availableSize = (long)readSize - (long)boxSize;
            if(availableSize == 0)
            {
                descriptor = null;
                return 0;
            }

            byte tag;
            ulong size = ReadUInt8(out tag);
            if (tag == 0)
            {
                descriptor = null;
                return 8;
            }

            ulong sizeOfSize = ReadDescriptorSize(out int sizeOfInstance);
            size += sizeOfSize;
            long sizeOfInstanceBits = (long)sizeOfInstance << 3;
            descriptor = (T)DescriptorFactory.CreateDescriptor(tag);
            descriptor.SizeOfSize = sizeOfSize;
            descriptor.Parent = parent;

            availableSize -= (long)size;
            if (availableSize < sizeOfInstanceBits)
            {
                descriptor = new InvalidDescriptor(tag) as T;
                Log.Debug($"DES:{GetIndentation(descriptor)}\'{descriptor.DisplayName}\'");
                size += descriptor.Read(this, (ulong) availableSize);
                return size;
            }

            Log.Debug($"DES:{GetIndentation(descriptor)}\'{descriptor.DisplayName}\'");

            ulong readInstanceSizeBits = descriptor.Read(this, (ulong)sizeOfInstanceBits);
            if (readInstanceSizeBits != (ulong)sizeOfInstanceBits)
            {
                if (readInstanceSizeBits < (ulong)sizeOfInstanceBits)
                {
                    StreamMarker missing;
                    size += ReadPadding((ulong)sizeOfInstanceBits, readInstanceSizeBits, out missing);
                    descriptor.Padding = missing;
                    Log.Debug($"Descriptor \'{tag}\' has extra padding of {missing.Length} bytes");
                }
                else
                {
                    Log.Debug($"Descriptor \'{tag}\' read through!");
                }
            }
            size += readInstanceSizeBits;

            ulong calculatedSize = descriptor.CalculateSize();
            if (calculatedSize != (ulong)sizeOfInstanceBits)
            {
                Log.Debug($"Calculated descriptor \'{tag}\' size: {calculatedSize >> 3}, read: {sizeOfInstanceBits >> 3}");
            }

            return size;
        }

        internal ulong ReadDescriptorsTillEnd(ulong boxSize, ulong readSize, Descriptor descriptor, int objectTypeIndication = -1)
        {
            if (descriptor.Children != null)
            {
                Log.Debug($"Descriptor reading repeated Children read");
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
                        consumed += ReadDescriptor(consumed, readSize, descriptor, out v);
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
                consumed += ReadDescriptor(consumed, remaining, descriptor, out v);
                descriptor.Children.Add(v);
            }
            return consumed;
        }

        internal static ulong CalculateDescriptors(Descriptor descriptor, int objectTypeIndication = -1)
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

        internal static ulong CalculateDescriptorSize(Descriptor descriptor)
        {
            if (descriptor == null)
                return 8;

            ulong descriptorContentSize = descriptor.CalculateSize();
            ulong descriptorSizeLength = CalculatePackedNumberLength(descriptorContentSize >> 3, descriptor.SizeOfSize >> 3);
            return 8 * (1 + descriptorSizeLength) + descriptorContentSize + 8 * (ulong)(descriptor.Padding != null ? descriptor.Padding.Length : 0);
        }

        internal ulong WriteDescriptorsTillEnd(Descriptor descriptor, int objectTypeIndication = -1)
        {
            ulong size = 0;
            foreach (var d in descriptor.Children)
            {
                size += WriteDescriptor(d);
            }
            return size;
        }

        internal ulong WriteDescriptor(Descriptor descriptor)
        {
            ulong size = 0;
            if (descriptor == null || descriptor.Tag == 0)
            {
                return size;
            }
            size += WriteUInt8(descriptor.Tag);

            ulong sizeOfInstance = descriptor.CalculateSize() + 8 * (ulong)(descriptor.Padding != null ? descriptor.Padding.Length : 0);
            size += WriteDescriptorSize(sizeOfInstance >> 3, descriptor.SizeOfSize >> 3);
            size += descriptor.Write(this);
            if (descriptor.Padding != null)
            {
                size += WritePadding(descriptor.Padding);
            }
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

        internal ulong ReadUInt8ArrayTillEnd(ulong boxSize, ulong readSize, out StreamMarker value)
        {
            StreamMarker marker;
            if (CanStreamSeek())
            {
                if (readSize == ulong.MaxValue)
                {
                    marker = new StreamMarker(GetCurrentOffset(), GetStreamLength() - GetCurrentOffset(), this);
                    SeekFromEnd(0);
                    value = marker;
                    return (ulong)(marker.Length << 3);
                }
                else
                {
                    long remaining = (long)(readSize - boxSize);
                    long count = remaining >> 3;
                    marker = new StreamMarker(GetCurrentOffset(), count, this);
                    SeekFromCurrent(count);
                    value = marker;
                    return (ulong)(marker.Length << 3);
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

                    return count << 3;
                }
                else
                {
                    long remaining = (long)(readSize - boxSize);
                    long count = remaining >> 3;
                    ulong copied = CopyStream(_stream, storage._stream, count);

                    marker = new StreamMarker(offset, count, storage);
                    value = marker;

                    return copied << 3;
                }
            }
        }

        internal ulong WriteUInt8ArrayTillEnd(StreamMarker data)
        {
            IsoStream readStream = data.Stream;
            long originalPosition = readStream.GetCurrentOffset();
            readStream.SeekFromBeginning(data.Position);
            ulong size = CopyStream(readStream._stream, _stream, data.Length) << 3;
            readStream.SeekFromBeginning(originalPosition); // because in our test app we're reading and writing at the same time from the same thread, we have to restore the original position
            return size;
        }

        #endregion // Long number arrays

        #region Number arrays

        internal ulong ReadUInt8ArrayTillEnd(ulong boxSize, ulong readSize, out byte[] value)
        {
            if (readSize == ulong.MaxValue)
            {
                return ReadBytes(readSize >> 3, out value);
            }

            ulong remaining = readSize - boxSize;
            ulong count = remaining >> 3;
            return ReadBytes(count, out value);
        }

        internal ulong ReadUInt32ArrayTillEnd(ulong boxSize, ulong readSize, out uint[] value)
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
            uint count = (uint)(remaining >> 5);
            value = new uint[count];
            for (uint i = 0; i < count; i++)
            {
                consumed += ReadUInt32(out value[i]);
            }
            return consumed;
        }

        internal ulong ReadUInt16Array(uint count, out ushort[] value)
        {
            ulong size = 0;
            value = new ushort[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt16(out value[i]);
            }
            return size;
        }

        internal ulong WriteUInt16Array(uint count, ushort[] value)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt16(value[i]);
            }
            return size;
        }

        internal ulong ReadUInt16Array(uint count, out uint[] value)
        {
            ulong size = 0;
            value = new uint[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt16(out value[i]);
            }
            return size;
        }
        
        internal ulong WriteUInt16Array(uint count, uint[] value)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt16(value[i]);
            }
            return size;
        }

        internal ulong ReadUInt32Array(uint count, out uint[] value)
        {
            ulong size = 0;
            value = new uint[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt32(out value[i]);
            }
            return size;
        }

        internal ulong WriteUInt32Array(uint count, uint[] value)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt32(value[i]);
            }
            return size;
        }

        internal ulong ReadUInt32Array(uint count, out ulong[] value)
        {
            ulong size = 0;
            value = new ulong[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt32(out value[i]);
            }
            return size;
        }

        internal ulong WriteUInt32Array(uint count, ulong[] value)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt32(value[i]);
            }
            return size;
        }

        internal ulong ReadUInt64Array(uint count, out ulong[] value)
        {
            ulong size = 0;
            value = new ulong[count];
            for (uint i = 0; i < count; i++)
            {
                size += ReadUInt64(out value[i]);
            }
            return size;
        }

        internal ulong WriteUInt64Array(uint count, ulong[] value)
        {
            ulong size = 0;
            for (uint i = 0; i < count; i++)
            {
                size += WriteUInt64(value[i]);
            }
            return size;
        }

        #endregion // Number arrays

        #region Numbers 

        internal ulong ReadInt8(out sbyte value)
        {
            ulong count = unchecked(ReadUInt8(out byte v));
            value = unchecked((sbyte)v);
            return count;
        }

        internal ulong WriteInt8(sbyte value)
        {
            return WriteUInt8(unchecked((byte)value));
        }

        internal ulong ReadUInt8(out byte value)
        {
            value = ReadByte();
            return 8;
        }

        internal ulong WriteUInt8(byte value)
        {
            WriteByte(value);
            return 8;
        }

        internal ulong ReadUInt8(out ushort value)
        {
            value = ReadByte();
            return 8;
        }

        internal ulong WriteUInt8(ushort value)
        {
            return WriteByte((byte)value);
        }

        internal ulong ReadInt16(out short value)
        {
            ulong count = ReadUInt16(out ushort v);
            value = unchecked((short)v);
            return count;
        }

        internal ulong WriteInt16(short value)
        {
            return WriteUInt16(unchecked((ushort)value));
        }

        internal ulong ReadUInt16(out ushort value)
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
            return 16;
        }

        internal ulong WriteUInt16(ushort value)
        {
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 16;
        }

        internal ulong ReadUInt16(out uint value)
        {
            ushort v;
            ulong size = ReadUInt16(out v);
            value = v;
            return size;
        }

        internal ulong WriteUInt16(uint value)
        {
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 16;
        }

        internal ulong ReadUInt24(out uint value)
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
            return 24;
        }

        internal ulong WriteUInt24(uint value)
        {
            value = value & 0xFFFFFF;
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 24;
        }

        internal ulong ReadInt32(out int value)
        {
            ulong count = ReadUInt32(out uint v);
            value = unchecked((int)v);
            return count;
        }

        internal ulong WriteInt32(int value)
        {
            return WriteUInt32(unchecked((uint)value));
        }

        internal ulong ReadInt32(out long value)
        {
            ulong count = unchecked(ReadUInt32(out uint v));
            value = unchecked((int)v);
            return count;
        }

        internal ulong WriteInt32(long value)
        {
            return WriteUInt32(unchecked((uint)value));
        }

        internal ulong ReadUInt32(out uint value)
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
            return 32;
        }

        internal ulong WriteUInt32(uint value)
        {
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 32;
        }

        internal ulong ReadUInt32(out ulong value)
        {
            uint v;
            ulong size = ReadUInt32(out v);
            value = v;
            return size;
        }

        internal ulong WriteUInt32(ulong value)
        {
            WriteByte((byte)(value >> 24 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return 32;
        }

        internal ulong ReadUInt48(out ulong value)
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
            return 48;
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

        internal ulong ReadInt64(out long value)
        {
            ulong count = unchecked(ReadUInt64(out ulong v));
            value = unchecked((long)v);
            return count;
        }

        internal ulong WriteInt64(long value)
        {
            return WriteUInt64(unchecked((ulong)value));
        }

        internal ulong ReadUInt64(out ulong value)
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
            return 64;
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

        #endregion // Numbers

        #region Iso639

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

        internal ulong WriteIso639(string value)
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
            return WriteBits(15, (ushort)bits);
        }

        #endregion // Iso639

        #region Proxy

        internal ulong ReadPadding(ulong size, ulong availableSize, out StreamMarker padding)
        {
            return ReadUInt8ArrayTillEnd(size, availableSize, out padding);
        }

        public ulong WritePadding(StreamMarker padding)
        {
            return WriteUInt8ArrayTillEnd(padding);
        }

        public ulong WritePadding(byte[] padding)
        {
            return WriteUInt8ArrayTillEnd(padding);
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

        internal static ulong CalculateEntrySize(IMp4Serializable entry)
        {
            return entry.CalculateSize();
        }

        internal static ulong CalculateClassSize(IMp4Serializable value)
        {
            return value.CalculateSize();
        }

        internal static ulong CalculateBoxSize(Box value)
        {
            return value.CalculateSize();
        }

        internal static ulong CalculateBoxArray(Box value)
        {
            return CalculateBoxSize(value.Children);
        }

        internal ulong WriteUInt8ArrayTillEnd(byte[] value)
        {
            return WriteUInt8Array((uint)value.Length, value);
        }

        internal ulong WriteUInt32ArrayTillEnd(uint[] value)
        {
            return WriteUInt32Array((uint)value.Length, value);
        }

        internal ulong ReadUInt8Array(uint count, out byte[] value)
        {
            return ReadBytes(count, out value);
        }

        internal ulong WriteUInt8Array(uint count, byte[] value)
        {
            return WriteBytes(count, value);
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

        internal ulong ReadBslbf(ulong count, out byte[] value)
        {
            return ReadBytes(count >> 3, out value);
        }

        internal ulong WriteBslbf(ulong count, byte[] value)
        {
            return WriteBytes(count >> 3, value);
        }

        #endregion Proxy

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

        #region TODO

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

        internal ulong ReadDouble32(out double value)
        {
            throw new NotImplementedException();
        }

        internal ulong WriteDouble32(double value)
        {
            throw new NotImplementedException();
        }

        internal static int BitsToDecode()
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

        #endregion // TODO
    }

    public class StreamMarker
    {
        public long Position { get; set; }
        public long Length { get; set; }
        public IsoStream Stream { get; set; }
        public StreamMarker(long position, long length, IsoStream stream)
        {
            Position = position;
            Length = length;
            Stream = stream;
        }
    }

    public class IsoEndOfStreamException : EndOfStreamException
    {
        public StreamMarker Padding { get; set; }
        public byte[] PaddingBytes { get; set; }

        public IsoEndOfStreamException()
        {
            
        }

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
        public int Length { get { return Bytes == null ? 0 : Bytes.Length; } }
        public byte[] Bytes { get; set; }
        public BinaryUTF8String(byte[] bytes)
        {
            this.Bytes = bytes;
        }
        public BinaryUTF8String(string text)
        {
            Bytes = Encoding.UTF8.GetBytes(text);
        }
        public override string ToString()
        {
            return Encoding.UTF8.GetString(Bytes);
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