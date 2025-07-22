using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [System.Serializable]
    internal class SerializableKeyValuePair
    {
        public TKey Key;
        public TValue Value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [SerializeField] List<SerializableKeyValuePair> _list = new ();

    public virtual TKey DefaultKey => default;

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        this.Clear();

        foreach (var item in _list)
        {
            this[ContainsKey(item.Key) ? DefaultKey : item.Key] = item.Value;
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        _list.Clear();

        foreach (var pair in this)
        {
            _list.Add(new SerializableKeyValuePair(pair.Key, pair.Value));
        }
    }
}
