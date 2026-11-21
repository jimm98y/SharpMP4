// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpMP4.Readers
{
    /// <summary>
    /// A table of the samples of a track's current fragment - their addresses, times and runs - of exactly as many entries as
    /// the fragment has samples. Each track's is made once and filled again of each fragment: of an array of each, four arrays
    /// of each fragment of each track. Its entries are those of the fragment being read, until the next is read.
    /// </summary>
    public sealed class FragmentTable<T> : IReadOnlyList<T>
    {
        // as large as the largest fragment read: grown, not made again, of a larger
        private T[] _items = Array.Empty<T>();

        internal FragmentTable()
        {
        }

        /// <summary>How many samples the fragment has.</summary>
        public int Count { get; private set; }

        /// <summary>The entry of the fragment's sample of the index.</summary>
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
            internal set
            {
                if ((uint)index >= (uint)Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _items[index] = value;
            }
        }

        // of a fragment of so many samples: every entry of it written before it is read
        internal void Reset(int count)
        {
            if (_items.Length < count)
                _items = new T[count];
            Count = count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return _items[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
