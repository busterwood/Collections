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

        public int Capacity => values.Length;
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
                        return Add(item, hc, reuseThis < 0 ? slot : reuseThis);

                    // remember first slot we can reuse due to deletion
                    if (reuseThis == DELETED)
                        reuseThis = valueIdx;
                }
                else if (hc == hashCodes[valueIdx] && Equality.Equals(item, values[valueIdx]))
                    // return if item already there
                    return false;

                // another value is in that slot, try the next index
                slot += 1;
                if (slot == indexes.Length)
                    slot = 0;

                // searched all possible entries and returned back to original slot
                if (slot == firstSlot)
                {
                    // we searched all entries and now can reuse the first DELETED slot we found
                    if (reuseThis > DELETED)
                        return Add(item, hc, reuseThis);

                    // must be full, so resize
                    Resize();
                    slot = hc % indexes.Length;
                }
            }
        }

        private bool Add(T item, int hc, int slot)
        {
            values[count] = item;
            hashCodes[count] = hc;
            indexes[slot] = ++count; // add one on here, taken off above
            return true;
        }

        static int PositiveHashCode(T item) => item.GetHashCode() & Lower31BitMask;

        int GetIndex(int slot) => indexes[slot] - 1; // default value is 0, so always store one more than a real index

        void SetIndex(int slot, int idx) { indexes[slot] = idx + 1; }

        FindResult FindSlot(T item, int hc)
        {
            const int UNSET = -1;
            var firstSlot = hc % indexes.Length;
            int slot = firstSlot;
            int firstFree = UNSET;
            for (;;)
            {
                int valueIdx = GetIndex(slot); 
                if (valueIdx == FREE)
                {
                    return new FindResult(-1, firstFree);
                }
                else if (valueIdx == DELETED)
                {
                    if (firstFree == UNSET)
                        firstFree = slot;
                }
                else if (hc == hashCodes[valueIdx] && Equality.Equals(item, values[valueIdx]))
                    return new FindResult(slot, firstFree);

                // another value is in that slot, try the next index
                slot += 1;
                if (slot == indexes.Length)
                    slot = 0;

                // searched all possible entries and returned back to original slot, must be full
                if (slot == firstSlot)
                    return new FindResult(-1, firstFree); // not found after searching all slots
            }
        }

        struct FindResult
        {
            public readonly int Slot;
            public readonly int FirstFreeSlot;
            public bool Found => Slot >= 0;

            public FindResult(int slot, int firstFree)
            {
                Slot = slot;
                FirstFreeSlot = firstFree;
            }
        }

        void Resize()
        {
            int newSize = (indexes.Length * 2) + 1;
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
                int valueIdx = GetIndex(slot);
                if (valueIdx < 0)
                {
                    SetIndex(slot, valueIndex);
                    break;
                }
                // another value is in that slot, try the next index
                if (++slot == indexes.Length)
                    slot = 0;

                // searched all possible entries and returned back to original slot, must be full, so resize
                if (slot == firstSlot)
                    throw new InvalidOperationException("all slots are full after resizing, this should not happen!");
            }
        }

        public bool Contains(T item)
        {
            int slot;
            var hc = PositiveHashCode(item);
            var result = FindSlot(item, hc);
            return result.Found;
        }

        public int IndexOf(T item)
        {
            int slot;
            var hc = PositiveHashCode(item);
            var result = FindSlot(item, hc);
            return result.Found ? GetIndex(result.Slot) : -1;
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
            int slot;
            var hc = PositiveHashCode(item);
            var found = FindSlot(item, hc);
            if (!found.Found)
                return false; // item not found in dictionary

            // it was found in index[slot]
            var idx = indexes[found.Slot] - 1;
            indexes[found.Slot] = DELETED + 1; // slot now free but mark it as deleted so probing will skip over it when finding values

            // shift values and hash codes down one, and clear last entry
            int newCount = count - 1;
            if (idx+1 < count)
            {
                Array.Copy(hashCodes, idx + 1, hashCodes, idx, newCount - idx);
                Array.Copy(values, idx + 1, values, idx, newCount - idx);
            }
            hashCodes[newCount] = 0;
            values[newCount] = default(T);

            // the slow bit, reduce by one each entry of the indexes array whose value is greater than the index we just deleted
            int search = idx + 1;
            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] > search)
                    indexes[i] -= 1;
            }
            count -= 1;
            return true;
        }

        public void RemoveAt(int index)
        {
            var item  = this[index];
            Remove(item);
        }
    }
}
