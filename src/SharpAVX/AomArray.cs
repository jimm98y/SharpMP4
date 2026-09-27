using System;

namespace SharpAVX
{
    /// <summary>An AomArray whatever its element type, for copying one inside another.</summary>
    public interface ICloneableArray
    {
        object CloneArray();
    }

    /// <summary>Where a value is in a table: what an encoder writes for an index the table was looked up with.</summary>
    public static class AomArray
    {
        /// <summary>The first index of the value in the table; 0 if it is not there, the index a decoder would take for none.</summary>
        public static int IndexOf(int[] table, int value)
        {
            int index = Array.IndexOf(table, value);
            return index < 0 ? 0 : index;
        }

        public static int IndexOf(AomArray<int> table, int value)
        {
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] == value)
                    return i;
            }
            return 0;
        }
    }

    /// <summary>
    /// An array that grows to whatever index is used, AV2's arrays being sized by the values in the
    /// stream. Its indexer returns the element itself, so that it can be read into, and an element
    /// that is an array of its own is made, by the function given for it, when it is first reached.
    /// </summary>
    public sealed class AomArray<T> : ICloneableArray
    {
        private readonly Func<T> _create;
        private T[] _items = new T[4];

        /// <param name="create">What makes an element when it is first reached; null, it starts as its default.</param>
        public AomArray(Func<T> create = null)
        {
            _create = create;
        }

        public int Length { get; private set; }

        public ref T this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException($"Index {index} is negative");

                if (index >= _items.Length)
                {
                    int size = _items.Length;
                    while (size <= index)
                        size *= 2;
                    Array.Resize(ref _items, size);
                }

                if (index >= Length)
                    Length = index + 1;

                if (_create != null && _items[index] == null)
                    _items[index] = _create();

                return ref _items[index];
            }
        }

        // Whether an element can be an array of its own, to be copied too: an int cannot.
        private static readonly bool ElementsAreArrays = !typeof(T).IsValueType;

        /// <summary>A copy of the array and of the arrays in it, as a process that saves one keeps it.</summary>
        public AomArray<T> Clone()
        {
            // As many elements as have been reached: past them all are as they started
            var items = new T[Math.Max(4, Length)];
            Array.Copy(_items, items, Length);
            var copy = new AomArray<T>(_create) { _items = items, Length = Length };
            if (ElementsAreArrays)
            {
                for (int i = 0; i < Length; i++)
                {
                    if (items[i] is ICloneableArray inner)
                        items[i] = (T)inner.CloneArray();
                    else if (items[i] is byte[] bytes)
                        items[i] = (T)(object)bytes.Clone();
                }
            }
            return copy;
        }

        object ICloneableArray.CloneArray() => Clone();

        /// <summary>A table's row as an array: AV2's variables are AomArrays, its tables arrays.</summary>
        public static implicit operator AomArray<T>(T[] items)
        {
            var array = new AomArray<T>();
            for (int i = items.Length - 1; i >= 0; i--)
                array[i] = items[i];
            return array;
        }
    }
}
