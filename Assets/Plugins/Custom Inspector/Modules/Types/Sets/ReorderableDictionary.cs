using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CustomInspector
{
    [Serializable]
    public class ReorderableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        #region Inspector

        [MessageBox("Please add the [Dictionary]-attribute to this Dictionary for correct displaying")]
        [HideField, SerializeField] bool _;

        [SerializeField] List<SerializableKeyValuePair> keyValuePairs = new(); //HideInInspector caused bug on classes: could not aufklappen classes
        #endregion

        #region Constructors
        public ReorderableDictionary() : base()
        { }
        public ReorderableDictionary(ReorderableDictionary<TKey, TValue> dict) : base()
        {
            foreach (KeyValuePair<TKey, TValue> item in dict)
                this.Add(item.Key, item.Value);
            ApplyValuesToList();
        }
        public ReorderableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
            ApplyValuesToList();
        }
        public ReorderableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection)
        {
            ApplyValuesToList();
        }
        public ReorderableDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        { }
        public ReorderableDictionary(int capacity) : base(capacity)
        { }
        public ReorderableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        {
            ApplyValuesToList();
        }
        public ReorderableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) : base(collection, comparer)
        {
            ApplyValuesToList();
        }
        public ReorderableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
        { }
        protected ReorderableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        #endregion

        [Conditional("UNITY_EDITOR")]
        void ApplyValuesToList()
        {
            //take code changes
            keyValuePairs = this.Select(_ => new SerializableKeyValuePair(_.Key, _.Value, true)).ToList();
        }

        #region Serialization
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {

        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // From serialized to object
            // Set values to inner Dictionary
            base.Clear();
            foreach (var item in keyValuePairs)
            {
                if (item.isValid)
                {
                    if (base.TryAdd(item.key, item.value))
                        item.isValid = true;
                    else
                        item.isValid = false;
                }
            }
        }
        #endregion

        #region Changes
        public new void Clear()
        {
            base.Clear();
            keyValuePairs.Clear();
        }
        public new bool TryAdd(TKey key, TValue value)
        {
            if (base.TryAdd(key, value))
            {
                keyValuePairs.Add(new(key, value, true));
                return true;
            }
            return false;
        }
        public new void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("Key cannot be null.");
            if (!this.TryAdd(key, value))
                throw new ArgumentException($"An item with the same key ({key}) has already been added.");
        }
        public new bool Remove(TKey key) => this.Remove(key, out TValue _);
        public new bool Remove(TKey key, out TValue value)
        {
            if (base.Remove(key, out value))
            {
                int index = keyValuePairs.FindIndex(kvp => kvp.key.Equals(key));
                Debug.Assert(index != -1, "Internal Error: Serialized representation was not syncronised with dictionaries values.");
                keyValuePairs.RemoveAt(index);
            }
            return false;
        }
        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                keyValuePairs.Find(kvp => kvp.key.Equals(key)).value = value;
            }
        }
        #endregion

        [System.Serializable]
        public class SerializableKeyValuePair
        {
            public TKey key;
            public TValue value;

            public bool isValid;

            public SerializableKeyValuePair(TKey key, TValue value, bool isValid)
            {
                this.key = key;
                this.value = value;

                this.isValid = isValid;
            }
        }
    }
}
