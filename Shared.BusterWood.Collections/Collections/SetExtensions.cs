using System.Collections.Generic;
using System.Linq;

namespace BusterWood.Collections
{
#if UNIQUELIST_INTERNAL
    internal
#else
    public
#endif
    static class SetExtensions
    {
        public static bool IsProperSubsetOf<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => set.IsSubsetOf(other) && other.Any(x => !set.Contains(x));

        public static bool IsProperSupersetOf<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => set.IsSupersetOf(other) && set.Any(x => !other.Contains(x, set.Equality));

        public static bool IsSubsetOf<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => set.All(x => other.Contains(x, set.Equality));

        public static bool IsSupersetOf<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => other.All(x => set.Contains(x));

        public static bool Overlaps<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => other.Any(x => set.Contains(x));

        public static bool SetEquals<S, T>(this S set, IEnumerable<T> other) where S : IReadOnlySet<T> => set.IsSubsetOf(other) && set.IsSupersetOf(other);

        public static UniqueList<T> ToUniqueList<T>(this IEnumerable<T> items, IEqualityComparer<T> equality = null)
        {
            equality  = equality ?? EqualityComparer<T>.Default;
            var ul = items as UniqueList<T>;
            if (ul != null && ul.Equality == equality)
                return ul.Copy();
            
            var result = new UniqueList<T>(equality);
            foreach (var item in items)
                result.Add(item);
            return result;
        }
    }
}