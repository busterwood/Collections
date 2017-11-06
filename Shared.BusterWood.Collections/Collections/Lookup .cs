using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BusterWood.Collections
{
    /// <summary>A key to multiple value dictionary, duplicate values are allowed. <see cref="UniqueLookup{TKey, TValue}"/> for a version of this that does not allow duplicate values.</summary>
#if List_INTERNAL
    internal
#else
    public
#endif
    class Lookup<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, List<TValue>>>
    {
        readonly Dictionary<TKey, List<TValue>> _map;

        public Lookup(IEqualityComparer<TKey> keyEquality = null)
        {
            _map = new Dictionary<TKey, List<TValue>>(keyEquality ?? EqualityComparer<TKey>.Default);
        }

        /// <summary>The number of  keys</summary>
        public int Count => _map.Count;

        public bool Add(TKey key, TValue value)
        {
            if (_map.TryGetValue(key, out var list))
                list.Add(value);
            else
                _map.Add(key, new List<TValue> { value });
            return true;
        }

        public List<TValue> this[TKey key]
        {
            get => _map.TryGetValue(key, out var list) ? list : null;
            set
            {
                if (value == null)
                    _map.Remove(key);
                else
                    _map[key] = value;
            }
        }

        public static List<TValue> GetValueOrDefault<TKey, TValue>(TKey key, TValue @default = default(T))
        {
            return _map.TryGetValue(key, out var list) ? list : @default;
        }

        public bool RemoveAll(TKey key) => _map.Remove(key);
        
        public bool Remove(TKey key, TValue value) => _map.TryGetValue(key, out var list) ? list.Remove(value) : false;

        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator() =>_map.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static partial class Lookups
    {
        public static Lookup<TKey, TValue> ToLookup<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyEquality = null)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var lookup = new Lookup<TKey, TValue>(keyEquality);
            foreach (var v in values)
            {
                lookup.Add(keySelector(v), v);
            }
            return lookup;
        }
    }
}
