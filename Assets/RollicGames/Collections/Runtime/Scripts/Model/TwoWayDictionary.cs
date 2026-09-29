using System.Collections.Generic;

namespace RollicGames.Collections.Runtime.Model
{
    public class TwoWayDictionary<TKey, TValue>
    {
        public int Count => _forward.Count;
        public IReadOnlyDictionary<TKey, TValue> Forward => _forward;
        public IReadOnlyDictionary<TValue, TKey> Reverse => _reverse;

        private readonly Dictionary<TKey, TValue> _forward = new();
        private readonly Dictionary<TValue, TKey> _reverse = new();

        public void Add(TKey key, TValue value)
        {
            if(_forward.ContainsKey(key) || _reverse.ContainsKey(value))
            {
                throw new System.ArgumentException("Key or value already exists.");
            }

            _forward.Add(key, value);
            _reverse.Add(value, key);
        }

        public bool TryGetValue(TKey key, out TValue value) => _forward.TryGetValue(key, out value);

        public bool TryGetKey(TValue value, out TKey key) => _reverse.TryGetValue(value, out key);

        public bool ContainsKey(TKey key) => _forward.ContainsKey(key);

        public bool ContainsValue(TValue value) => _reverse.ContainsKey(value);

        public bool RemoveByKey(TKey key)
        {
            if(!_forward.Remove(key, out var value)) return false;

            _reverse.Remove(value);
            return true;
        }

        public bool RemoveByValue(TValue value)
        {
            if(!_reverse.Remove(value, out var key)) return false;

            _forward.Remove(key);
            return true;
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }
    }
}
