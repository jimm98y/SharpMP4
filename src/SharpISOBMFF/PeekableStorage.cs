using SharpMP4.Common;
using System;

namespace SharpISOBMFF
{
    /// <summary>
    /// A storage whose next bytes can be looked at before they are read, and whose bytes read can be read again: what a
    /// box's layout depends on - an 'enct' entry's 'frma', whether a string or a box comes next - is looked at ahead, and a
    /// box whose syntax does not fit what is in it is read again as its bytes, the same way on a stream that seeks and on
    /// one that does not. The bytes looked at are kept, and are what the reads after give first; the bytes read since a
    /// <see cref="Mark"/> are kept, where the stream cannot seek back to them, until it is released.
    /// </summary>
    public sealed class PeekableStorage : IStorage
    {
        private readonly IStorage _inner;

        // the bytes looked at, or put back, and not yet read: _held[_start.._start + _count]
        private byte[] _held = new byte[64];
        private int _start;
        private int _count;

        // the bytes read since the first mark not released, where the stream cannot seek: up to MaxRecorded of them, past
        // which they are let go, and no mark can be gone back to until all are released
        private byte[] _record = new byte[0];
        private int _recorded;
        private int _marks;
        private bool _overflowed;

        public PeekableStorage(IStorage inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <summary>The default of <see cref="MaxRecorded"/>: 16 MB.</summary>
        public const int DefaultMaxRecorded = 16 << 20;

        /// <summary>
        /// How many of the bytes read since a <see cref="Mark"/> are kept, where the stream cannot seek, to be read again: past
        /// it, none can be gone back to. Of <see cref="IsoStream"/>, the most of a box whose syntax does not fit what is in
        /// it that is kept as its bytes; the boxes are held in memory as they are read. 0 keeps none.
        /// </summary>
        public int MaxRecorded
        {
            get => _maxRecorded;
            set => _maxRecorded = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "A size, of 0 or more bytes.");
        }
        private int _maxRecorded = DefaultMaxRecorded;

        /// <summary>The storage looked ahead in.</summary>
        public IStorage Inner => _inner;

        public IMp4Logger Logger
        {
            get => _inner.Logger;
            set => _inner.Logger = value;
        }

        /// <summary>
        /// Up to <paramref name="count"/> of the next bytes, read ahead and kept, not read: fewer where the stream ends
        /// first. How many there are.
        /// </summary>
        public int Peek(byte[] buffer, int offset, int count)
        {
            if (_count < count)
            {
                Reserve(count);
                while (_count < count)
                {
                    int read = _inner.Read(_held, _start + _count, count - _count);
                    if (read <= 0)
                        break;
                    _count += read;
                }
            }

            int available = Math.Min(count, _count);
            Buffer.BlockCopy(_held, _start, buffer, offset, available);
            return available;
        }

        /// <summary>
        /// Where the bytes read from here on can be put back to be read again (<see cref="Rewind"/>): of a stream that
        /// cannot seek, they are kept until the mark is released. -1 of one that can, which seeks back instead.
        /// </summary>
        public int Mark()
        {
            if (_inner.CanStreamSeek())
                return -1;
            _marks++;
            return _recorded;
        }

        /// <summary>Whether the bytes read since a mark are all kept: not where more than 16 MB were read since the first.</summary>
        public bool CanRewind(int mark) => mark >= 0 && !_overflowed && mark <= _recorded;

        /// <summary>The bytes read since the mark put back, to be read again.</summary>
        public void Rewind(int mark)
        {
            if (!CanRewind(mark))
                throw new InvalidOperationException("No bytes are kept from that mark.");

            int count = _recorded - mark;
            // before the bytes looked at, which come after them
            if (_start < count)
            {
                var held = new byte[Math.Max(_held.Length, count + _count) + count];
                Buffer.BlockCopy(_held, _start, held, count, _count);
                _held = held;
                _start = count;
            }
            _start -= count;
            _count += count;
            Buffer.BlockCopy(_record, mark, _held, _start, count);
            _recorded = mark;
        }

        /// <summary>A mark done with: the bytes read since the first are let go when every mark is.</summary>
        public void Release(int mark)
        {
            if (mark < 0 || _marks == 0)
                return;
            if (--_marks == 0)
            {
                _recorded = 0;
                _overflowed = false;
            }
        }

        public int ReadByte()
        {
            int b;
            if (_count == 0)
            {
                b = _inner.ReadByte();
            }
            else
            {
                _count--;
                b = _held[_start++];
            }
            if (b >= 0 && _marks > 0)
            {
                if (_recorded >= MaxRecorded)
                    _overflowed = true;
                else
                {
                    if (_recorded == _record.Length)
                        Array.Resize(ref _record, Math.Max(256, _record.Length * 2));
                    _record[_recorded++] = (byte)b;
                }
            }
            return b;
        }

        public int Read(byte[] buffer, int offset, int length)
        {
            int taken;
            if (_count == 0)
            {
                taken = _inner.Read(buffer, offset, length);
            }
            else
            {
                taken = Math.Min(length, _count);
                Buffer.BlockCopy(_held, _start, buffer, offset, taken);
                _start += taken;
                _count -= taken;
            }
            if (taken > 0 && _marks > 0)
                Record(buffer, offset, taken);
            return taken;
        }

        public void ReadExactly(byte[] data, int offset, int length)
        {
            int taken = 0;
            while (taken < length)
            {
                int read = Read(data, offset + taken, length - taken);
                if (read <= 0)
                    throw new System.IO.EndOfStreamException();
                taken += read;
            }
        }

        public long GetPosition() => _inner.GetPosition() - _count;

        public long GetLength() => _inner.GetLength();

        public bool CanStreamSeek() => _inner.CanStreamSeek();

        public long SeekFromBeginning(long offset)
        {
            Drop();
            return _inner.SeekFromBeginning(offset);
        }

        public long SeekFromCurrent(long offset)
        {
            // from where the reads are, which is before the bytes looked at
            int held = _count;
            Drop();
            return _inner.SeekFromCurrent(offset - held);
        }

        public long SeekFromEnd(long offset)
        {
            Drop();
            return _inner.SeekFromEnd(offset);
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            Unread();
            _inner.Write(buffer, offset, length);
        }

        public void WriteByte(byte value)
        {
            Unread();
            _inner.WriteByte(value);
        }

        public void Flush() => _inner.Flush();

        public void Dispose() => _inner.Dispose();

        private void Reserve(int count)
        {
            if (_start + count <= _held.Length)
                return;
            var held = _held.Length >= count ? _held : new byte[Math.Max(count, _held.Length * 2)];
            Buffer.BlockCopy(_held, _start, held, 0, _count);
            _held = held;
            _start = 0;
        }

        private void Record(byte[] buffer, int offset, int count)
        {
            if (_overflowed || _recorded + count > MaxRecorded)
            {
                _overflowed = true;
                return;
            }
            if (_recorded + count > _record.Length)
                Array.Resize(ref _record, Math.Max(_recorded + count, Math.Max(256, _record.Length * 2)));
            Buffer.BlockCopy(buffer, offset, _record, _recorded, count);
            _recorded += count;
        }

        private void Drop()
        {
            _start = 0;
            _count = 0;
        }

        /// <summary>Before a write: the stream put back where the reads are, the bytes looked at given up.</summary>
        private void Unread()
        {
            if (_count == 0)
                return;
            if (!_inner.CanStreamSeek())
                throw new InvalidOperationException("Writing where bytes were looked ahead at, on a stream that cannot seek back to before them.");
            SeekFromCurrent(0);
        }
    }
}
