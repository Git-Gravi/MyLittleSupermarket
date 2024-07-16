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
    /// Only valid for SerializableSet's! Used to for display in the inspector
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class SetAttribute : PropertyAttribute { }

    /// <summary>
    /// A list without duplicates
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class SerializableSet<T> : ICollection, ICollection<T>, IEnumerable, IEnumerable<T>, IDeserializationCallback
    {
        [MessageBox("Use the [SetAttribute] attribute for displaying in the inspector", MessageBoxType.Error)]
        [SerializeField, HideField] bool info;


        [SerializeField, HideField]
        protected ListContainer<T> values = new();
        public int Count => values.Count;

        public SerializableSet()
        {
            values = new();
        }
        public SerializableSet(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new NullReferenceException("collection is null");
            values = new ListContainer<T>(collection);
        }


        /// <summary>
        /// Adds an element to the set
        /// </summary>
        /// <exception cref="ArgumentException">element already exists</exception>
        public void Add(T item)
        {
            if (!TryAdd(item))
            {
                if (item == null)
                    throw new NullReferenceException($"Item to add is null");
                else
                    throw new ArgumentException($"Item '{item}' already existed in set");
            }
        }

        /// <summary>
        /// Adds an element to the end of the set if not already exists
        /// </summary>
        /// <returns>true if item did not already existed</returns>
        public virtual bool TryAdd(T item)
        {
            if (item == null || values.Contains(item))
            {
                return false;
            }
            else
            {
                values.Add(item);
                return true;
            }
        }

        /// <returns>true if item was removed successfully</returns>
        public virtual bool Remove(T item)
        {
            return values.Remove(item);
        }

        /// <summary>
        /// Removes element at index
        /// </summary>
        public void RemoveAt(int index)
            => values.RemoveAt(index);

        /// <returns>
        /// The index of the item.
        /// <para> -1 if the item doesnt exist </para>
        /// </returns>
        public virtual int GetIndexOf(T item)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public T GetByIndex(int index)
            => values[index];

        public virtual T this[int index]
        {
            get => values[index];
            set
            {
                if (object.ReferenceEquals(values[index], value))
                {
                    return;
                }
                if (object.Equals(values[index], value))
                {
                    values[index] = value;
                    return;
                }
                if (values.Contains(value))
                    throw new ArgumentException($"Item '{value}' already existed in set");
                else
                    values[index] = value;
            }
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            Internal_OnDeserialization(sender);
        }
        protected virtual void Internal_OnDeserialization(object sender)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            //Check for duplicates
            for (int i = 0; i < values.Count; i++)
            {
                for (int j = i + 1; j < values.Count; j++)
                {
                    if (object.Equals(values[i], values[j]))
                    {
                        Debug.LogError($"DeserializationError: Duplicate items in Set found. Duplicates are discarded.\n{values[i]} at position {i} and {values[j]} at position {j}");
                        values = values.Distinct().ToList();
                        return;
                    }
                }
            }
#endif
        }

        public void Clear()
        {
            values.Clear();
        }

        /// <returns>If item exist in set</returns>
        public virtual bool Contains(T item)
        {
            return values.Contains(item);
        }

        public ReadOnlyCollection<T> AsReadOnly() => values.AsReadOnly();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T value in values)
            {
                yield return value;
            }
        }

        public static explicit operator List<T>(SerializableSet<T> s)
            => s.values;

        void ICollection.CopyTo(Array array, int index)
        {
            if (index + Count > array.Length)
                throw new ArgumentException("The number of elements in the source Dictionary is greater than the available space from index to the end of the destination array");

            for (int i = index + Count - 1; i >= index; i--)
            {
                array.SetValue(values[i], i);
            }
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("The number of elements in the source Dictionary is greater than the available space from index to the end of the destination array");

            for (int i = arrayIndex + Count - 1; i >= arrayIndex; i--)
            {
                array[i] = values[i];
            }
        }

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => false;

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// This is just for editorPurpose.
        /// </summary>
        [SerializeField]
        T editor_input;
    }
}

