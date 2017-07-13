using System;
using System.Collections;
using System.Collections.Generic;

namespace BusterWood.Collections
{

    /// <summary>
    /// Array backed list of unique elements, which acts like a set in that you cannot add duplicates.
    /// </summary>
    /// <remarks>
    /// Why use this over <see cref="HashSet{T}"/>? This is good for small number of items (50 or less) and it preserves order, and can be accessed like a list.
    /// </remarks>
    public class DistinctList<T> : IReadOnlySet<T>, ICollection<T>, IReadOnlyList<T>
    {
        readonly IEqualityComparer<T> equality;
        T[] items;

        public DistinctList(int capacity = 0, IEqualityComparer<T> equality = null)
        {
            this.equality = equality ?? EqualityComparer<T>.Default;
            items = new T[capacity];
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public IEqualityComparer<T> Equality => equality;

        public T this[int index] => items[index];

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Add(T item)
        {
            if (IndexOf(item) >= 0)
                return false;
            if (Count == items.Length)
                ReSize();
            items[Count] = item;
            Count += 1;
            return true;
        }

        void ReSize()
        {
            int newSize = items.Length == 0 ? 4 : items.Length * 2;
            Array.Resize(ref items, newSize);
        }

        internal void AddRange<U>(U other) where U : IEnumerable<T>
        {
            foreach (var item in other)
                Add(item);
        }

        public void Clear()
        {
            Array.Clear(items, 0, Count);
            Count = 0;
        }

        public bool Contains(T item) => IndexOf(item) >= 0;

        public void CopyTo(T[] array, int arrayIndex)
        {
            int len = Count - arrayIndex;
            Array.Copy(items, 0, array, arrayIndex, len);
        }

        public IEnumerator<T> GetEnumerator()
        {
            int i = 0;
            foreach (var x in items)
            {
                if (i == Count)
                    yield break;
                yield return x;
                i += 1;
            }
        }

        public bool Remove(T item)
        {
            int idx = IndexOf(item);
            if (idx < 0)
                return false;
            int shift = idx + 1;
            if (shift < Count)
            {
                Array.Copy(items, shift, items, idx, Count - shift);
                items[Count - 1] = default(T);
            }
            else
            {
                items[idx] = default(T);
            }
            Count -= 1;
            return true;
        }

        int IndexOf(T item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (i == Count)
                    break;
                if (equality.Equals(items[i], item))
                    return i;
            }
            return -1;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();        
    }
}