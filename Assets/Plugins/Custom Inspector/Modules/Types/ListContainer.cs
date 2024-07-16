using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace CustomInspector
{
    /// <summary>
    /// An equivalent to the System.Collections.Generic.List<T>
    /// </summary>
    [System.Serializable]
    public class ListContainer<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        [MessageBox("ListContainer is missing the [ListContainer]-attribute to be correctly displayed")]
        [SerializeField, HideField] bool _;

        // CONTENT
        [SerializeField]
        List<T> values = new();


        // CONSTRUCTORS
        public ListContainer() { }
        public ListContainer(int capacity) : base()
        {
            values = new List<T>(capacity);
        }
        public ListContainer(List<T> collection) : base()
        {
            values = collection;
        }
        public ListContainer(IEnumerable<T> collection) : base()
        {
            values = collection.ToList();
        }

        // COMPATIBILITY
        public static implicit operator ListContainer<T>(List<T> list) => new(list);
        public static implicit operator List<T>(ListContainer<T> container) => container.values;

        // LIST METHODS
        public T this[int index] { get => values[index]; set => values[index] = value; }
        public int Count => values.Count;
        public int Capacity => values.Capacity;
        public void Add(T item) => values.Add(item);
        public void AddRange(IEnumerable<T> collection) => values.AddRange(collection);
        public ReadOnlyCollection<T> AsReadOnly() => values.AsReadOnly();
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) => values.BinarySearch(index, count, item, comparer);
        public int BinarySearch(T item) => values.BinarySearch(item);
        public int BinarySearch(T item, IComparer<T> comparer) => values.BinarySearch(item, comparer);
        public void Clear() => values.Clear();
        public bool Contains(T item) => values.Contains(item);
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => values.ConvertAll(converter);
        public void CopyTo(T[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
        public void CopyTo(T[] array) => values.CopyTo(array);
        public void CopyTo(int index, T[] array, int arrayIndex, int count) => values.CopyTo(index, array, arrayIndex, count);
        public bool Exists(Predicate<T> match) => values.Exists(match);
        public T Find(Predicate<T> match) => values.Find(match);
        public List<T> FindAll(Predicate<T> match) => values.FindAll(match);
        public int FindIndex(int startIndex, int count, Predicate<T> match) => values.FindIndex(startIndex, count, match);
        public int FindIndex(int startIndex, Predicate<T> match) => values.FindIndex(startIndex, match);
        public int FindIndex(Predicate<T> match) => values.FindIndex(match);
        public T FindLast(Predicate<T> match) => values.FindLast(match);
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) => values.FindLastIndex(startIndex, count, match);
        public int FindLastIndex(int startIndex, Predicate<T> match) => values.FindLastIndex(startIndex, match);
        public int FindLastIndex(Predicate<T> match) => values.FindLastIndex(match);
        public void ForEach(Action<T> action) => values.ForEach(action);
        public List<T>.Enumerator GetEnumerator() => values.GetEnumerator();
        public List<T> GetRange(int index, int count) => values.GetRange(index, count);
        public int IndexOf(T item, int index, int count) => values.IndexOf(item, index, count);
        public int IndexOf(T item, int index) => values.IndexOf(item, index);
        public int IndexOf(T item) => values.IndexOf(item);
        public void Insert(int index, T item) => values.Insert(index, item);
        public void InsertRange(int index, IEnumerable<T> collection) => values.InsertRange(index, collection);
        public int LastIndexOf(T item) => values.LastIndexOf(item);
        public int LastIndexOf(T item, int index) => values.LastIndexOf(item, index);
        public int LastIndexOf(T item, int index, int count) => values.LastIndexOf(item, index, count);
        public bool Remove(T item) => values.Remove(item);
        public int RemoveAll(Predicate<T> match) => values.RemoveAll(match);
        public void RemoveAt(int index) => values.RemoveAt(index);
        public void RemoveRange(int index, int count) => values.RemoveRange(index, count);
        public void Reverse(int index, int count) => values.Reverse(index, count);
        public void Reverse() => values.Reverse();
        public void Sort(Comparison<T> comparison) => values.Sort(comparison);
        public void Sort(int index, int count, IComparer<T> comparer) => values.Sort(index, count, comparer);
        public void Sort() => values.Sort();
        public void Sort(IComparer<T> comparer) => values.Sort(comparer);
        public T[] ToArray() => values.ToArray();
        public void TrimExcess() => values.TrimExcess();
        public bool TrueForAll(Predicate<T> match) => values.TrueForAll(match);


        // INTERFACES
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)values).IsReadOnly;

        public bool IsSynchronized => ((ICollection)values).IsSynchronized;

        public object SyncRoot => ((ICollection)values).SyncRoot;

        public bool IsFixedSize => ((IList)values).IsFixedSize;

        public bool IsReadOnly => ((IList)values).IsReadOnly;

        object IList.this[int index] { get => ((IList)values)[index]; set => ((IList)values)[index] = value; }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)values).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)values).GetEnumerator();
        }
        public void CopyTo(Array array, int index)
        {
            ((ICollection)values).CopyTo(array, index);
        }

        public int Add(object value)
        {
            return ((IList)values).Add(value);
        }

        public bool Contains(object value)
        {
            return ((IList)values).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)values).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList)values).Insert(index, value);
        }

        public void Remove(object value)
        {
            ((IList)values).Remove(value);
        }
    }

    /// <summary>
    /// Only valid for ListContainer! Used to fix overriding of other attributes
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class ListContainerAttribute : PropertyAttribute { }
}
