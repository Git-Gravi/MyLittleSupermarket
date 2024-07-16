using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace CustomInspector
{
    /// <summary>
    /// Only valid for dictionary's! Used for display in the inspector
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class DictionaryAttribute : PropertyAttribute
    {
        /// <summary>
        /// Only on SerializableDictionary: Overrides the key-column-header with a custom label
        /// </summary>
        public string keyLabel = null;
        /// <summary>
        /// Only on SerializableDictionary: Overrides the value-column-header with a custom label
        /// </summary>
        public string valueLabel = null;

        public readonly float keySize;
        public const float defaultKeySize = .4f;
        public DictionaryAttribute(float keySize = defaultKeySize)
        {
            if (keySize <= 0)
            {
                Debug.LogWarning($"{nameof(keySize)} has to be greater zero");
                keySize = defaultKeySize;
            }
            else if (keySize >= 1)
            {
                Debug.LogWarning($"{nameof(keySize)} has to be between 0 and 1, because it defines the proportion of space for keys and values");
                keySize = defaultKeySize;
            }
            this.keySize = keySize;
        }
    }

    /// <summary>
    /// A serializable dictionary.
    /// Mostly you want to use SerializableSortedDictionary for better performance
    /// Time complexity: O(n)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : IEnumerable, IDictionary, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDeserializationCallback
    {
        [MessageBox("Use the [Dictionary]-attribute for displaying in the inspector", MessageBoxType.Error)]
        [SerializeField, HideField] bool info;


        [SerializeField, HideField]
        protected SerializableSet<TKey> keys;
        public ReadOnlyCollection<TKey> Keys => keys.ToList().AsReadOnly();


        /// <summary>
        /// same sorting & amount as keys
        /// </summary>
        [SerializeField, HideField]
        protected ListContainer<TValue> values;
        public ReadOnlyCollection<TValue> Values => values.AsReadOnly();


        public int Count => keys.Count;

        public SerializableDictionary()
        {
            this.keys = new();
            this.values = new();
        }
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
                throw new NullReferenceException("Dictionary is null");
            keys = new SerializableSet<TKey>(dictionary.Select(_ => _.Key));
            values = dictionary.Select(_ => _.Value).ToList();
        }
        public SerializableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            if (collection == null)
                throw new NullReferenceException("IEnumerable is null");
            keys = new SerializableSet<TKey>(collection.Select(_ => _.Key));
            values = collection.Select(_ => _.Value).ToList();
        }
        protected SerializableDictionary(SerializableSet<TKey> keys, List<TValue> values)
        {
            if (keys == null)
                throw new NullReferenceException("Keys is null");
            if (values == null)
                throw new NullReferenceException("Values value is null");
            if (keys.Count != values.Count)
                throw new ArgumentException($"{nameof(keys)} and {nameof(values)} must have the same amount of elements");
            this.keys = keys;
            this.values = values;
        }

        /// <summary> Adds an element to the dictionary </summary>
        /// <exception cref="ArgumentException">If key already exists</exception>
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException($"key/value pair with the same key '{key}' already existed in the Dictionary");
        }


        /// <summary> Adds an element to the dictionary </summary>
        /// <returns>True, if key/value pair got added at key. False if key already exists</returns>
        public virtual bool TryAdd(TKey key, TValue value)
        {
            if (keys.TryAdd(key))
            {
                values.Add(value);
                return true;
            }
            else
                return false;
        }

        /// <returns>True if key/value pair was deleted. False if key was not found</returns>
        public bool Remove(TKey key)
        {
            return TryRemove(key, out TValue _);
        }

        /// <summary>
        /// If key/value pair was removed, it stores the removed value in out TValue value
        /// </summary>
        /// <returns>if key/value pair was removed</returns>
        public bool TryRemove(TKey key, out TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException("Key is null");
            }

            int ind = keys.GetIndexOf(key);
            if (ind != -1)
            {
                value = values[ind];
                keys.RemoveAt(ind);
                values.RemoveAt(ind);
                return true;
            }
            value = default;
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException($"Index ({index}) has to be greater zero");
            if (index >= Count)
                throw new ArgumentOutOfRangeException($"Index ({index}) has to be smaller than the size of the collection ({Count})");

            keys.RemoveAt(index);
            values.RemoveAt(index);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int ind = keys.GetIndexOf(key);
            if (ind == -1)
            {
                value = default;
                return false;
            }
            else
            {
                try
                {
                    value = values[ind];
                }
                catch (ArgumentOutOfRangeException ae)
                {
                    if (keys.Count != values.Count)
                    {
                        throw new InvalidOperationException($"Internal Dictionary Error: Element with index '{ind}' cannot be accessed due to corrupted dictionary. " +
                                                            $"Keys.Count='{keys.Count}' differ from Values.Count='{values.Count}'.\n\nOriginal Exception: {ae.Message}");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Internal Dictionary Error: Element with index '{ind}' is invalid on dictionary.Count='{Count}'." +
                                                            $"\n\nOriginal Exception: {ae.Message}");
                    }
                }
                return true;
            }
        }

        public TValue GetValue(TKey key)
        {
            if (TryGetValue(key, out TValue value))
                return value;
            else
                throw new KeyNotFoundException($"key '{key}' not found");
        }

        public TValue this[TKey key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                int ind = keys.GetIndexOf(key);
                if (ind == -1)
                    throw new KeyNotFoundException($"key '{key}' not found");
                values[ind] = value;
            }
        }

        public bool ContainsKey(TKey key)
            => keys.Contains(key);

        public bool Contains(TKey key, TValue value)
        {
            if (TryGetValue(key, out TValue returnedValue))
                return returnedValue.Equals(value);
            else
                return false;
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public IEnumerator<(TKey key, TValue value)> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return (keys.GetByIndex(i), values[i]);
            }
        }

        public static explicit operator (List<TKey> keys, List<TValue> values)(SerializableDictionary<TKey, TValue> d)
            => ((List<TKey>)d.keys, d.values);
        public static explicit operator List<(TKey, TValue)>(SerializableDictionary<TKey, TValue> d)
        {
            List<(TKey, TValue)> res = new();
            for (int i = 0; i < d.Count; i++)
            {
                res.Add((d.keys.GetByIndex(i), d.values[i]));
            }
            return res;
        }
        public static explicit operator Dictionary<TKey, TValue>(SerializableDictionary<TKey, TValue> d)
        {
            Dictionary<TKey, TValue> dict = new();
            var e = d.GetEnumerator();
            while (e.MoveNext())
                dict.Add(e.Current.key, e.Current.value);
            return dict;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(keys.GetByIndex(i), values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();


        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        object ICollection.SyncRoot => null;

        ICollection IDictionary.Keys => keys;
        ICollection IDictionary.Values => values;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => keys.AsReadOnly();
        ICollection<TValue> IDictionary<TKey, TValue>.Values => values.AsReadOnly();
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => keys.AsReadOnly();
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => values.AsReadOnly();

        object IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => this[(TKey)key] = (TValue)value;
        }


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

        protected class DictionaryEnumerator : IDictionaryEnumerator
        {
            // A copy of the SimpleDictionary object's key/value pairs.
            readonly DictionaryEntry[] items;
            int index = -1;

            public DictionaryEnumerator(SerializableDictionary<TKey, TValue> sd)
            {
                // Make a copy of the dictionary entries currently in the SimpleDictionary object.
                items = sd.Select(_ => new DictionaryEntry(_.Key, _.Value)).ToArray();
            }

            // Return the current item.
            public object Current { get { ValidateIndex(); return items[index]; } }

            // Return the current dictionary entry.
            public DictionaryEntry Entry
            {
                get { return (DictionaryEntry)Current; }
            }

            // Return the key of the current item.
            public object Key { get { ValidateIndex(); return items[index].Key; } }

            // Return the value of the current item.
            public object Value { get { ValidateIndex(); return items[index].Value; } }

            // Advance to the next item.
            public bool MoveNext()
            {
                if (index < items.Length - 1) { index++; return true; }
                return false;
            }

            // Validate the enumeration index and throw an exception if the index is out of range.
            private void ValidateIndex()
            {
                if (index < 0 || index >= items.Length)
                    throw new InvalidOperationException("Enumerator is before or after the collection.");
            }

            // Reset the index to restart the enumeration.
            public void Reset()
            {
                index = -1;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (index + Count > array.Length)
                throw new ArgumentException("The number of elements in the source Dictionary is greater than the available space from index to the end of the destination array");

            for (int i = index + Count - 1; i >= index; i--)
            {
                array.SetValue(new KeyValuePair<TKey, TValue>(keys.GetByIndex(i), values[i]), i);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("The number of elements in the source Dictionary is greater than the available space from index to the end of the destination array");

            for (int i = arrayIndex + Count - 1; i >= arrayIndex; i--)
            {
                array[i] = new KeyValuePair<TKey, TValue>(keys.GetByIndex(i), values[i]);
            }
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            Internal_OnDeserialization(sender);
        }
        protected virtual void Internal_OnDeserialization(object sender)
        {
            if (keys.Count != values.Count)
            {
                if (keys.Count > values.Count)
                {
                    int tooMuch = keys.Count - values.Count;
                    for (int i = 0; i < tooMuch; i++)
                    {
                        keys.RemoveAt(keys.Count - 1);
                    }

                    Debug.LogError(nameof(SerializableDictionary<TKey, TValue>) + ": Deserialized 'key's amount was greater than 'value's amount. " +
                                    $"{tooMuch} 'key's were discarded.");
                }
                else
                {
                    int tooMuch = values.Count - keys.Count;
                    for (int i = 0; i < tooMuch; i++)
                    {
                        values.RemoveAt(values.Count - 1);
                    }

                    Debug.LogError(nameof(SerializableDictionary<TKey, TValue>) + ": Deserialized 'value's amount was greater than 'key' amount. " +
                                    $"{tooMuch} 'value's were discarded.");
                }
            }
        }

        /// <summary>
        /// This is just for editorPurpose.
        /// </summary>
        [SerializeField]
        TKey editor_keyInput;
        /// <summary>
        /// This is just for editorPurpose.
        /// </summary>
        [SerializeField]
        TValue editor_valueInput;
    }
}
