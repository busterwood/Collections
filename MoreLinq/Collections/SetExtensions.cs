using System.Collections.Generic;
using System.Linq;

namespace BusterWood.Collections
{

    public static class SetExtensions
    {
        public static bool IsProperSubsetOf<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => set.IsSubsetOf(other) && other.Any(x => !set.Contains(x));

        public static bool IsProperSupersetOf<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => set.IsSupersetOf(other) && set.Any(x => !other.Contains(x, set.Equality));

        public static bool IsSubsetOf<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => set.All(x => other.Contains(x, set.Equality));

        public static bool IsSupersetOf<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => other.All(x => set.Contains(x));

        public static bool Overlaps<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => other.Any(x => set.Contains(x));

        public static bool SetEquals<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => set.IsSubsetOf(other) && set.IsSupersetOf(other);

    }
}