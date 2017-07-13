using System.Collections;
using System.Collections.Generic;

namespace BusterWood.Collections.Immutable
{

    /// <summary>
    /// Set that can contain zero or one items
    /// </summary>
    public struct BinarySet<T> : IReadOnlySet<T>
    {
        readonly IEqualityComparer<T> equality;
        readonly T item;

        public BinarySet(IEqualityComparer<T> equality)
        {
            this.equality = equality;
            Count = 0;
            item = default(T);
        }

        public BinarySet(T item, IEqualityComparer<T> equality = null)
        {
            this.equality = equality;
#pragma warning disable RECS0017 // Possible compare of value type with 'null'
            Count = item == null ? 0 : 1;
#pragma warning restore RECS0017 // Possible compare of value type with 'null'
            this.item = item;
        }

        public int Count { get; }

        public IEqualityComparer<T> Equality => equality ?? EqualityComparer<T>.Default; // cannot set in ctor due to being a struct

        public bool Contains(T item) => Count == 1 && Equality.Equals(this.item, item);

        public IEnumerator<T> GetEnumerator()
        {
            if (Count == 1)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // avoid boxing using generic contraint
        public bool Equals<U>(U other) where U : IReadOnlySet<T> => this.SetEquals(other);

        public override bool Equals(object obj) => obj != null && Equals((IReadOnlySet<T>)obj);

        public override int GetHashCode() => item?.GetHashCode() ?? 0;

        public IReadOnlySet<T> Union<U>(U other) where U : IReadOnlySet<T>
        {
            if (Count == 0) return other;
            if (other.Count == 0) return this;
            var b = ArraySet<T>.Builder(Count + other.Count, Equality);
            b.Add(item);
            b.AddRange(other);
            return b.Build();
        }
    }
}