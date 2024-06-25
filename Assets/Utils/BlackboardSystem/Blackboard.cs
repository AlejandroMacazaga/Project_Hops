using System;
using System.Collections.Generic;
using Utils.Extensions;

namespace Utils.BlackboardSystem
{
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        private readonly string _name;
        private readonly int _hashedKey;

        public BlackboardKey(string name)
        {
            this._name = name;
            _hashedKey = name.ComputeFNV1aHash();
        }
        public bool Equals(BlackboardKey other) => _hashedKey == other._hashedKey;
        
        public override bool Equals(object other) => other is BlackboardKey key && Equals(key);
        public override int GetHashCode() => _hashedKey;
        public override string ToString() => _name;
        
        public static bool operator ==(BlackboardKey a, BlackboardKey b) => a._hashedKey == b._hashedKey;
        public static bool operator !=(BlackboardKey a, BlackboardKey b) => !(a == b);
    }

    [Serializable]
    public class BlackboardEntry<T>
    {
        public BlackboardKey Key { get;  }
        public T Value { get; }
        public Type ValueType { get; }
        
        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }
        
        public override bool Equals(object obj) => obj is BlackboardEntry<T> entry && Key == entry.Key;
        public override int GetHashCode() => Key.GetHashCode();
    }

    [Serializable]
    public class Blackboard
    {
        private Dictionary<string, BlackboardKey> _keyCache = new();
        private Dictionary<BlackboardKey, object> _entries = new();

        public List<Action> PassedActions { get; } = new();

        public void AddAction(Action action)
        {
            Preconditions.CheckNotNull(action);
            PassedActions.Add(action);    
        }
        
        public void ClearActions() => PassedActions.Clear();
        
        public void Debug()
        {
            foreach (var entry in _entries)
            {
                var entryType = entry.Value.GetType();
                
                if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
                {
                    var valueProperty = entryType.GetProperty("Value");
                    if (valueProperty == null) continue;
                    var value = valueProperty.GetValue(entry.Value);
                    UnityEngine.Debug.Log($"Key: {entry.Key}, Value: {value}");
                }
            }
        }
        
        public BlackboardKey GetOrRegisterKey(string name)
        {
            Preconditions.CheckNotNull(name);
            
            if (!_keyCache.TryGetValue(name, out var key))
            {
                key = new BlackboardKey(name);
                _keyCache[name] = key;
            }

            return key;
        }
        
        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            if (_entries.TryGetValue(key, out var obj) && obj is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }
        
        public void SetValue<T>(BlackboardKey key, T value)
        {
            _entries[key] = new BlackboardEntry<T>(key, value);
        }
        
        public bool ContainsKey(BlackboardKey key) => _entries.ContainsKey(key);
        
        public void Remove(BlackboardKey key) => _entries.Remove(key);
        
        
    }
}
