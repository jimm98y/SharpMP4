using System;
using System.Collections.Generic;

namespace SharpAVX
{
    /// <summary>
    /// One occurrence of a syntax element as it was read: its value, and the bits it took - a leb128()
    /// may take more bytes than its value needs, and written again it must take as many.
    /// </summary>
    public readonly struct AomSyntaxValue
    {
        public AomSyntaxValue(long value, int bits, byte[] bytes = null)
        {
            Value = value;
            Bits = bits;
            Bytes = bytes;
        }

        public long Value { get; }
        public int Bits { get; }

        /// <summary>
        /// The bytes of an element read as bytes (f(n) past 32 bits, le(n) past 4 bytes, tile data), or those
        /// a leb128() was coded in.
        /// </summary>
        public byte[] Bytes { get; }

        public override string ToString() => Bytes != null ? $"byte[{Bytes.Length}]" : $"{Value} ({Bits} bits)";
    }

    /// <summary>
    /// The syntax elements of an OBU, every occurrence of each in the order they were read. An encoder
    /// writes each element from its own state, which the specification's derived variables mirror; what
    /// an encoder chose that its state does not say - a found_ref of 0 where a reference matched, a
    /// leb128() longer than it needs - only the stream says. Written from this record, an OBU is
    /// written back as it was read, bit for bit.
    /// </summary>
    public sealed class AomSyntaxRecord
    {
        // In the order they were read: an OBU written again takes them in the same order
        private readonly Entries _entries = new Entries();

        // Each element's occurrences, where they are in _entries: made when first asked for by name
        private Dictionary<string, List<int>> _index;

        /// <summary>How many occurrences were read, of all the elements.</summary>
        public int Count => _entries.Count;

        /// <summary>The element read at a position, in the order they were read.</summary>
        public string NameAt(int position) => _entries[position].Name;

        public AomSyntaxValue ValueAt(int position) => _entries[position].Value;

        /// <summary>The names of the elements read, in the order each was first read.</summary>
        public IReadOnlyCollection<string> Names
        {
            get
            {
                var names = new List<string>();
                var seen = new HashSet<string>();
                for (int i = 0; i < _entries.Count; i++)
                {
                    string name = _entries[i].Name;
                    if (seen.Add(name))
                        names.Add(name);
                }
                return names;
            }
        }

        public void Add(string name, AomSyntaxValue value)
        {
            _entries.Add((name, value));
            if (_index != null)
                Occurrences(name, create: true).Add(_entries.Count - 1);
        }

        private List<int> Occurrences(string name, bool create)
        {
            if (_index == null)
            {
                _index = new Dictionary<string, List<int>>();
                for (int i = 0; i < _entries.Count; i++)
                {
                    if (!_index.TryGetValue(_entries[i].Name, out var positions))
                        _index.Add(_entries[i].Name, positions = new List<int>());
                    positions.Add(i);
                }
            }
            if (!_index.TryGetValue(name, out var found) && create)
                _index.Add(name, found = new List<int>());
            return found;
        }

        /// <summary>Every occurrence of an element, in the order they were read; none if it was not read.</summary>
        public IReadOnlyList<AomSyntaxValue> this[string name]
        {
            get
            {
                var positions = Occurrences(name, create: false);
                if (positions == null)
                    return Array.Empty<AomSyntaxValue>();
                var values = new AomSyntaxValue[positions.Count];
                for (int i = 0; i < values.Length; i++)
                    values[i] = _entries[positions[i]].Value;
                return values;
            }
        }

        public bool TryGet(string name, int occurrence, out AomSyntaxValue value)
        {
            var positions = Occurrences(name, create: false);
            if (positions != null && occurrence < positions.Count)
            {
                value = _entries[positions[occurrence]].Value;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>Sets an occurrence to be written with another value, in as few bits as it takes.</summary>
        public void Set(string name, int occurrence, long value)
        {
            int position = Occurrences(name, create: false)[occurrence];
            _entries[position] = (name, new AomSyntaxValue(value, 0));
        }

        /// <summary>Sets an occurrence of an element read as bytes to other bytes.</summary>
        public void Set(string name, int occurrence, byte[] bytes)
        {
            int position = Occurrences(name, create: false)[occurrence];
            _entries[position] = (name, new AomSyntaxValue(0, bytes.Length * 8, bytes));
        }

        /// <summary>
        /// The entries, in blocks of a fixed size: a VP9 frame's compressed header has thousands, and a list grown to hold
        /// them copied them over as it grew, frame after frame, into arrays past what the large object heap takes.
        /// </summary>
        private sealed class Entries
        {
            private const int BlockBits = 8;
            private const int BlockSize = 1 << BlockBits;

            private readonly List<(string Name, AomSyntaxValue Value)[]> _blocks = new List<(string, AomSyntaxValue)[]>();

            public int Count { get; private set; }

            public ref (string Name, AomSyntaxValue Value) this[int index]
            {
                get
                {
                    if ((uint)index >= (uint)Count)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    return ref _blocks[index >> BlockBits][index & (BlockSize - 1)];
                }
            }

            public void Add((string Name, AomSyntaxValue Value) entry)
            {
                if ((Count & (BlockSize - 1)) == 0)
                    _blocks.Add(new (string, AomSyntaxValue)[BlockSize]);
                _blocks[Count >> BlockBits][Count & (BlockSize - 1)] = entry;
                Count++;
            }
        }
    }
}
