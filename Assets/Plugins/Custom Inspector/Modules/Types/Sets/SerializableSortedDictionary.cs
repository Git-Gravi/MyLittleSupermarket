using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace CustomInspector
{
    /// <summary>
    /// A serializable implementation of System.SortedDictionary
    /// Time complexity: access = O(log(n)) , add/remove = O(n)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class SerializableSortedDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue>, IEnumerable, IDictionary, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
                                                                where TKey : IComparable
    {
        readonly SerializableSortedSet<TKey> keys_casted;

        public SerializableSortedDictionary()
        : base(new SerializableSortedSet<TKey>(), new List<TValue>())
        {
            keys_casted = (SerializableSortedSet<TKey>)base.keys;
        }
        public SerializableSortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this()
        {
            foreach (var item in collection)
            {
                this.Add(item.Key, item.Value);
            }
        }


        /// <returns>True, if key/value pair got added at key. False if key already exists</returns>
        public override bool TryAdd(TKey key, TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException("Key is null");
            }

            int ind = keys_casted.AddAndGetIndex(key);
            if (ind != -1)
            {
                values.Insert(ind, value);
                return true;
            }
            return false;
        }

        protected override void Internal_OnDeserialization(object sender)
        {
            base.Internal_OnDeserialization(sender);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (keys_casted.Count != base.keys.Count)
            {
                Debug.LogError("Deserialized keys were out of sync. Keys may lose their order");
                base.keys = keys_casted;
            }
            else
            {
                for (int i = 0; i < base.keys.Count; i++)
                {
                    if (!keys_casted[i].Equals(base.keys[i]))
                    {
                        Debug.LogError("Deserialized keys were out of sync. Keys may lose their order");
                        base.keys = keys_casted;
                        break;
                    }
                }
            }
#endif
            base.keys = keys_casted;
        }

        ICollection IDictionary.Keys => keys_casted.AsReadOnly();
        ICollection IDictionary.Values => values.AsReadOnly();



        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            => Contains(item.Key, item.Value);

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        bool IDictionary.Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator(this);
        }
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(keys_casted.GetByIndex(i), values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
        void ICollection.CopyTo(Array array, int index)
        {
            if (index + Count > array.Length)
                throw new ArgumentException("The number of elements in the source Dictionary is greater than the available space from index to the end of the destination array");

            for (int i = index + Count - 1; i >= index; i--)
            {
                array.SetValue(new KeyValuePair<TKey, TValue>(keys_casted.GetByIndex(i), values[i]), i);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("The number of elements in the source Dictionary is greater than the available space from index to the end of the destination array");

            for (int i = arrayIndex + Count - 1; i >= arrayIndex; i--)
            {
                array[i] = new KeyValuePair<TKey, TValue>(keys_casted.GetByIndex(i), values[i]);
            }
        }
    }
}