using BusterWood.MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BusterWood.Collections
{
    /// <summary>List of unique elements, which acts like a set in that you cannot add duplicates.</summary>
    /// <remarks>Performance is comparable to <see cref="HashSet{T}"/>.  Design was inspired by Python's 3.6 new dict</remarks>
    public class UniqueList<T> : IReadOnlyList<T>, ISet<T>, IReadOnlySet<T>, IList<T>
    {
        const int Lower31BitMask = 0x7FFFFFFF;
        const int FREE = -1;    // index is free, terminate probing when looking for an item
        const int DELETED = -2; // mark index with this to indicate a value WAS at this index, and continue probing when looking for an item
        int[] indexes;
        int[] hashCodes;
        T[] values;
        int count;

        public UniqueList(IEqualityComparer<T> equality = null)
        {
            const int InitialSize = 3;
            indexes = new int[InitialSize];
            hashCodes = new int[InitialSize];
            values = new T[InitialSize];
            count = 0;
            Equality = equality ?? EqualityComparer<T>.Default;
        }

        public IEqualityComparer<T> Equality { get; }

        public int Count => count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException();
                return values[index];
            }
            set
            {
                //TODO: remove and add with value and hashcode slot reuses
                throw new NotImplementedException();
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        // inline the code from FindSlot to optimize performance
        public bool Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException();

            var hc = PositiveHashCode(item);
            var firstSlot = hc % indexes.Length;
            int slot = firstSlot;
            int reuseThis = DELETED;
            for (;;)
            {
                int valueIdx = indexes[slot] - 1; // default value is 0, so always store one more than a real index
                if (valueIdx < 0)
                {
                    // not found, add it now
                    if (valueIdx == FREE)
                    {
                        values[count] = item;
                        hashCodes[count] = hc;
                        indexes[reuseThis < 0 ? slot : reuseThis] = ++count; // add one on here, taken off above
                        return true;
                    }

                    // remember first slot we can reuse due to deletion
                    if (reuseThis == DELETED)
                        reuseThis = valueIdx;
                }

                // return if item already there
                if (hc == hashCodes[valueIdx] && Equality.Equals(item, values[valueIdx]))
                    return false; 

                // another value is in that slot, try the next index
                slot += 1;
                if (slot == indexes.Length)
                    slot = 0;

                // searched all possible entries and returned back to original slot, must be full, so resize
                if (slot == firstSlot)
                {
                    Resize();
                    slot = hc % indexes.Length;
                }
            }
        }

        static int PositiveHashCode(T item) => item.GetHashCode() & Lower31BitMask;

        bool FindSlot(T item, int hc, out int slot)
        {
            var firstSlot = hc % indexes.Length;
            slot = firstSlot;
            for (;;)
            {
                int valueIdx = indexes[slot] - 1; // default value is 0, so always store one more than a real index
                if (valueIdx == FREE)
                    return false; 

                if (hc == hashCodes[valueIdx] && Equality.Equals(item, values[valueIdx]))
                    return true; // found

                // another value is in that slot, try the next index
                slot += 1;
                if (slot == indexes.Length)
                    slot = 0;

                // searched all possible entries and returned back to original slot, must be full
                if (slot == firstSlot)
                {
                    slot = -1; // not found and cannot be inserted
                    return false;
                }
            }
        }

        void Resize()
        {
            int newSize = (indexes.Length * 2);
            Array.Resize(ref hashCodes, newSize);
            Array.Resize(ref values, newSize);
            
            // recreate indexes from existing values and hashcodes
            indexes = new int[newSize];
            for (int i = 0; i < values.Length; i++)
            {
                if (i == count) break;
                var hc = hashCodes[i];
                SetIndexForValue(i, hc);
            }
        }

        void SetIndexForValue(int valueIndex, int hc)
        {
            int firstSlot = hc % indexes.Length;
            int slot = firstSlot;
            for (;;)
            {
                int valueIdx = indexes[slot] - 1; // default value is 0, so always store one more than a real index
                if (valueIdx < 0)
                {
                    indexes[slot] = valueIndex + 1; // add one on here, taken of then searching, avoids initialising indexes
                    break;
                }
                // another value is in that slot, try the next index
                if (++slot == indexes.Length)
                    slot = 0;

                // searched all possible entries and returned back to original slot, must be full, so resize
                if (slot == firstSlot)
                {
                    throw new InvalidOperationException("all slots are full after resizing, this should not happen!");
                }
            }
        }

        public bool Contains(T item)
        {
            int slot;
            var hc = PositiveHashCode(item);
            return FindSlot(item, hc, out slot);
        }

        public int IndexOf(T item)
        {
            int slot;
            var hc = PositiveHashCode(item);
            return FindSlot(item, hc, out slot) ? slot : -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int i = 0;
            foreach (var v in values)
            {
                if (i++ > count) yield break;
                yield return v;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // AddRange
        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                Add(item);
        }

        // in this AND other
        public void IntersectWith(IEnumerable<T> other)
        {
            var notInBoth = other.ToList(x => !Contains(x));
            ExceptWith(notInBoth);
        }

        // RemoveRange
        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                Remove(item);
        }

        // NOT in either this or other
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var inBoth = other.ToList(x => Contains(x));
            ExceptWith(inBoth);
        }

        public bool IsSubsetOf(IEnumerable<T> other) => SetExtensions.IsSubsetOf(this, other);

        public bool IsSupersetOf(IEnumerable<T> other) => SetExtensions.IsSupersetOf(this, other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => SetExtensions.IsProperSupersetOf(this, other);

        public bool IsProperSubsetOf(IEnumerable<T> other) => SetExtensions.IsProperSubsetOf(this, other);

        public bool Overlaps(IEnumerable<T> other) => SetExtensions.Overlaps(this, other);

        public bool SetEquals(IEnumerable<T> other) => SetExtensions.SetEquals(this, other);

        public void Clear()
        {
            Array.Clear(indexes, 0, indexes.Length);
            Array.Clear(hashCodes, 0, count);
            Array.Clear(values, 0, count);
            count = 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int toCopy = Math.Min(count, array.Length - arrayIndex);
            Array.Copy(values, 0, array, arrayIndex, toCopy);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            var idx = IndexOf(item);
            var found = idx >= 0;
            if (found)
                RemoveAt(idx);
            return found;
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}
