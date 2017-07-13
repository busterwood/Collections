using System;
using System.Collections.Generic;
using System.Linq;

namespace BusterWood.MoreLinq
{
    public static class Enumerables
    {
        /// <summary>returns up to <paramref cref="count"/> values from the <paramref cref="source"/>, padding the result with <paramref cref="@default"/> values</summary>
        public static T[] ToArrayOf<T>(this IEnumerable<T> source, int count, T @default = default(T))
        {
            // short cut if passed an array of the correct size
            var a = source as T[];
            if (a != null && a.Length == count)
                return a;

            // create a new array and populate it
            var result = new T[count];
            var e = source.GetEnumerator();
            int i = 0;
            for (; i < count && e.MoveNext(); i++)
                result[i] = e.Current;
            for (;  i < count; i++)
                result[i] = @default;
            return result;
        }

        /// <summary>returns up to <paramref cref="count"/> values from the <paramref cref="source"/>, padding the result with <paramref cref="@default"/> values</summary>
        public static List<T> ToListOf<T>(this IEnumerable<T> source, int count, T @default = default(T))
        {
            // short cut if passed an list of the correct size
            var lst = source as List<T>;
            if (lst != null && lst.Count == count)
                return lst;

            // create a new list and populate it
            var result = new List<T>(count);
            result.AddRange(source.Take(count));
            while (result.Count < count)
                result.Add(@default);
            return result;
        }

        public static IEnumerable<IEnumerable<T>> SplitOn<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var group = new List<T>();
            foreach (var item in items)
            {
                if (predicate(item) && group.Count > 0)
                {
                    yield return group.ToArray();
                    group.Clear();
                }
                group.Add(item);
            }
            if (group.Count > 0)
                yield return group.ToArray();
        }

        public static IEnumerable<T> Concat<T>(this T head, IEnumerable<T> rest)
        {
            yield return head;
            foreach (var i in rest)
                yield return i;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> rest, T tail)
        {
            foreach (var i in rest)
                yield return i;
            yield return tail;
        }

        /// <summary>Returns a sequence of integers from <paramref name="start"/> up to and including <paramref name="end"/></summary>
        public static IEnumerable<int> To(this int start, int end)
        {
            for (int i = start; i <= end; i++)
                yield return i;
        }

        /// <summary>Returns a sequence of <paramref name="count"/> integers starting with <paramref name="start"/></summary>
        public static IEnumerable<int> For(this int start, int count)
        {
            int end = start + count;
            for (int i = start; i < end; i++)
                yield return i;
        }

        /// <summary>Returns the input items without the first one</summary>
        public static IEnumerable<T> Head<T>(this IEnumerable<T> items, int count = 1) => items.Take(count);

        /// <summary>Returns the input items without the first one</summary>
        public static IEnumerable<T> Rest<T>(this IEnumerable<T> items, int skip = 1) => items.Skip(skip);

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> items, int times) => Enumerable.Repeat(items, times).SelectMany(x => x);

        /// <summary>Returns results of the <paramref name="chooser"/> function that return a value (the functions result is not null)</summary>
        public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<T> items, Func<T, TResult> chooser) where TResult : class
            => items.Select(chooser).Where(res => res != null);

        /// <summary>Returns results of the <paramref name="chooser"/> function that return a value (the functions result is not null)</summary>
        public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<T> items, Func<T, TResult?> chooser) where TResult : struct
            => items.Select(chooser).Where(res => res.HasValue).Select(res => res.Value);

    }

    
}
