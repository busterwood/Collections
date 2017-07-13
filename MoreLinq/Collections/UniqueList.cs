using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace BusterWood.Collections
{
    /// <summary>List of unique elements, which acts like a set in that you cannot add duplicates.</summary>
    /// <remarks>Performance is comparable to <see cref="HashSet{T}"/>.  Design was inspired by Python's 3.6 new dict</remarks>
    public class UniqueList<T> : IReadOnlyList<T>
    {
        const int Lower31BitMask = 0x7FFFFFFF;

        int[] indexes;
        int[] hashCodes;
        T[] values;
        int count;

        public UniqueList(IEqualityComparer<T> equality = null)
        {
            indexes = new int[4];
            hashCodes = new int[4];
            values = new T[4];
            count = 0;
            Equality = equality ?? EqualityComparer<T>.Default;
        }

        public IEqualityComparer<T> Equality { get; }

        public int Count => count;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException();
                return values[index];
            }
        }

        public bool Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException();

            int slot;
            var hc = PositiveHashCode(item);
            if (FindSlot(item, hc, out slot))
                return false; // already in list

            // if the list is full then resize
            if (slot < 0)
            {                
                Resize();
                FindSlot(item, hc, out slot);
            }

            // not found, add it
            Debug.Assert(slot >= 0);
            values[count] = item;
            hashCodes[count] = hc;
            indexes[slot] = ++count; // add one on here, taken off by FindSlot
            return true;
        }

        static int PositiveHashCode(T item) => item.GetHashCode() & Lower31BitMask;

        bool FindSlot(T item, int hc, out int slot)
        {
            var firstSlot = hc % indexes.Length;
            slot = firstSlot;
            for (;;)
            {
                int valueIdx = indexes[slot] - 1; // default value is 0, so always store one more than a real index
                if (valueIdx < 0)
                    return false; // not found and slot is index it can be added at

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
    }
}
