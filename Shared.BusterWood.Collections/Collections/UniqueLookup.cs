using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BusterWood.Collections
{
    /// <summary>
    /// A key to multiple value dictionary, where each value is only included once, i.e. duplicate values are not added.
    /// <see cref="UniqueLookup{TKey, TValue}"/> for a version of this that does not allow duplicate values.
    /// </summary>
#if UNIQUELIST_INTERNAL
    internal
#else
    public
#endif
    class UniqueLookup<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, UniqueList<TValue>>>
    {
        readonly Dictionary<TKey, UniqueList<TValue>> _map;
        readonly IEqualityComparer<TValue> _valueEquality;

        public UniqueLookup(IEqualityComparer<TKey> keyEquality = null, IEqualityComparer<TValue> valueEquality = null)
        {
            _map = new Dictionary<TKey, UniqueList<TValue>>(keyEquality ?? EqualityComparer<TKey>.Default);
            _valueEquality = valueEquality;
        }

        /// <summary>The number of unique keys, NOT the number of values</summary>
        public int Count => _map.Count;

        public bool Add(TKey key, TValue value)
        {
            if (_map.TryGetValue(key, out var list))
                return list.Add(value);

            _map.Add(key, new UniqueList<TValue>(_valueEquality) { value });
            return true;
        }

        public UniqueList<TValue> this[TKey key]
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

        public static UniqueList<TValue> GetValueOrDefault<TKey, TValue>(TKey key, TValue @default = default(T))
        {
            return _map.TryGetValue(key, out var list) ? list : @default;
        }


        public bool RemoveAll(TKey key) => _map.Remove(key);

        public bool Remove(TKey key, TValue value) => _map.TryGetValue(key, out var list) ? list.Remove(value) : false;

        public IEnumerator<KeyValuePair<TKey, UniqueList<TValue>>> GetEnumerator() =>_map.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static partial class Lookups
    {
        public static UniqueLookup<TKey, TValue> ToUniqueLookup<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyEquality = null, IEqualityComparer<TValue> valueEquality = null)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var lookup = new UniqueLookup<TKey, TValue>(keyEquality, valueEquality);
            foreach (var v in values)
            {
                lookup.Add(keySelector(v), v);
            }
            return lookup;
        }
    }
}
