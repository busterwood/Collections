using System;
using System.Collections;
using System.Collections.Generic;

namespace BusterWood.Collections.Immutable
{
    public struct ArraySet<T> : IReadOnlySet<T>, IReadOnlyList<T>
    {
        readonly T[] items;

        private ArraySet(T[] items, IEqualityComparer<T> equality)
        {
            this.items = items;
            Equality = equality;
        }

        public int Count => items.Length;

        public IEqualityComparer<T> Equality { get; }

        public T this[int index] => items[index];

        public bool Contains(T item)
        {
            foreach (var x in items)
            {
                if (Equality.Equals(x, item))
                    return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var x in items)
                yield return x;
        }

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public ArraySet<T> Union<U>(U other) where U : IReadOnlySet<T>
        {
            if (other.Count == 0) return this;
            var b = Builder(Count + other.Count, Equality);
            b.Copy(items);
            b.AddRange(other);
            return b.Build();
        }

        public static SetBuilder Builder(int capacity = 0, IEqualityComparer<T> equality = null) => new SetBuilder(capacity, equality);

        public class SetBuilder 
        {
            readonly IEqualityComparer<T> equality;
            T[] items;

            public SetBuilder(int capacity, IEqualityComparer<T> equality)
            {
                this.equality = equality ?? EqualityComparer<T>.Default;
                items = new T[capacity];
            }

            public int Count { get; private set; }

            public IEqualityComparer<T> Equality => equality;

            public bool Add(T item)
            {
                int idx = IndexOf(item);
                if (idx >= 0)
                    return false;
                EnsureCapacity(Count + 1);
                items[Count] = item;
                Count += 1;
                return true;
            }

            int IndexOf(T item)
            {
                int i = 0;
                foreach (var x in items)
                {
                    if (i == Count)
                        break;
                    if (equality.Equals(x, item))
                        return i;
                    i += 1;
                }
                return -1;
            }

            void EnsureCapacity(int requiredSize)
            {
                if (requiredSize > items.Length)
                {
                    int newSize = items.Length == 0 ? 4 : items.Length * 2;
                    Array.Resize(ref items, newSize);
                }
            }

            internal void AddRange<U>(U other) where U : IEnumerable<T>
            {
                foreach (var item in other)
                    Add(item);
            }

            public ArraySet<T> Build()
            {
                if (items.Length != Count)
                    Array.Resize(ref items, Count);
                return new ArraySet<T>(items, Equality);
            }

            internal void Copy(T[] items)
            {
                Array.Copy(items, this.items, 0);
            }
        }
    }

}
