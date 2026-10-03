using SharpISOBMFF;
using SharpMP4.Common;
using System;
using System.Collections.Generic;

namespace SharpMP4.Builders
{
    /// <summary>
    /// The samples of an 'mdat', as one storage: runs of bytes of the tracks' own storages, one after another, in the order
    /// the chunks are interleaved in - so the 'mdat' is written as a box, its data a marker over them. It is only read, and
    /// the storages are the builder's: disposing it disposes none of them.
    /// </summary>
    internal sealed class InterleavedStorage : IStorage
    {
        /// <summary>A run of bytes of one storage, and where in the 'mdat' it starts.</summary>
        public readonly struct Run
        {
            public Run(IStorage storage, long start, long length)
            {
                Storage = storage;
                Start = start;
                Length = length;
            }

            public IStorage Storage { get; }
            public long Start { get; }
            public long Length { get; }
        }

        private readonly List<Run> _runs;
        private readonly long[] _runOffsets;
        private readonly long _length;
        private long _position;

        public InterleavedStorage(List<Run> runs, IMp4Logger logger = null)
        {
            _runs = runs;
            _runOffsets = new long[runs.Count];
            for (int i = 0; i < runs.Count; i++)
            {
                _runOffsets[i] = _length;
                _length += runs[i].Length;
            }
            Logger = logger ?? DefaultMp4Logger.Instance;
        }

        public IMp4Logger Logger { get; set; }

        public bool CanStreamSeek() => true;

        public long GetLength() => _length;

        public long GetPosition() => _position;

        public long SeekFromBeginning(long offset)
        {
            if (offset < 0 || offset > _length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            _position = offset;
            return _position;
        }

        public long SeekFromCurrent(long offset) => SeekFromBeginning(_position + offset);

        public long SeekFromEnd(long offset) => SeekFromBeginning(_length + offset);

        public int Read(byte[] buffer, int offset, int length)
        {
            int total = 0;
            while (length > 0 && _position < _length)
            {
                // the run the position is in: the last that starts at it or before
                int index = Array.BinarySearch(_runOffsets, _position);
                if (index < 0)
                    index = ~index - 1;
                while (_runs[index].Length == 0)
                    index++;

                var run = _runs[index];
                long within = _position - _runOffsets[index];
                int count = (int)Math.Min(length, run.Length - within);
                run.Storage.SeekFromBeginning(run.Start + within);
                run.Storage.ReadExactly(buffer, offset, count);

                _position += count;
                offset += count;
                length -= count;
                total += count;
            }
            return total;
        }

        public void ReadExactly(byte[] data, int offset, int length)
        {
            if (Read(data, offset, length) != length)
                throw new IsoEndOfStreamException();
        }

        public int ReadByte()
        {
            var one = new byte[1];
            return Read(one, 0, 1) == 1 ? one[0] : -1;
        }

        public void Flush()
        {
        }

        public void Write(byte[] buffer, int offset, int length) => throw new NotSupportedException("The samples of an 'mdat' are only read.");

        public void WriteByte(byte value) => throw new NotSupportedException("The samples of an 'mdat' are only read.");

        public void Dispose()
        {
            // the storages are the builder's, which disposes them once the file is written
        }
    }
}
