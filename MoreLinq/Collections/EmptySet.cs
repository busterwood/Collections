using System.Collections;
using System.Collections.Generic;

namespace BusterWood.Collections.Immutable
{
    public struct EmptySet<T> : IReadOnlySet<T>
    {
        public int Count => 0;

        public IEqualityComparer<T> Equality => EqualityComparer<T>.Default; // does not matter

        public bool Contains(T item) => false;

        public IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public U Union<U>(U other) where U : IReadOnlySet<T> => other;

        public EmptySet<T> Intersect<U>(U other) where U : IReadOnlySet<T> => this;
    }
}