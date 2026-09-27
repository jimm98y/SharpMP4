using System;

namespace SharpAVX
{
    /// <summary>An AomArray whatever its element type, for copying one inside another.</summary>
    public interface ICloneableArray
    {
        object CloneArray();
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

        /// <summary>A copy of the array and of the arrays in it, as a process that saves one keeps it.</summary>
        public AomArray<T> Clone()
        {
            var copy = new AomArray<T>(_create) { _items = (T[])_items.Clone(), Length = Length };
            for (int i = 0; i < copy._items.Length; i++)
            {
                if (copy._items[i] is ICloneableArray inner)
                    copy._items[i] = (T)inner.CloneArray();
                else if (copy._items[i] is byte[] bytes)
                    copy._items[i] = (T)(object)bytes.Clone();
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
