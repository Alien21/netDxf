#region netDxf library licensed under the MIT License
// 
//                       netDxf library
// Copyright (c) Daniel Carvajal (haplokuon@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace netDxf.Collections
{
    /// <summary>
    /// Represent a collection of items that fire events when it is modified. 
    /// </summary>
    /// <typeparam name="T">Type of items.</typeparam>
    public class ObservableCollection<T> :
        IList<T>
    {
        #region delegates and events

        public delegate void AddItemEventHandler(ObservableCollection<T> sender, ObservableCollectionEventArgs<T> e);

        public delegate void BeforeAddItemEventHandler(ObservableCollection<T> sender, ObservableCollectionEventArgs<T> e);

        public delegate void RemoveItemEventHandler(ObservableCollection<T> sender, ObservableCollectionEventArgs<T> e);

        public delegate void BeforeRemoveItemEventHandler(ObservableCollection<T> sender, ObservableCollectionEventArgs<T> e);

        public event BeforeAddItemEventHandler BeforeAddItem;
        public event AddItemEventHandler AddItem;
        public event BeforeRemoveItemEventHandler BeforeRemoveItem;
        public event RemoveItemEventHandler RemoveItem;

        protected virtual void OnAddItemEvent(T item)
        {
            AddItemEventHandler ae = AddItem;
            if (ae != null)
            {
                ae(this, new ObservableCollectionEventArgs<T>(item));
            }
        }

        protected virtual bool OnBeforeAddItemEvent(T item)
        {
            BeforeAddItemEventHandler ae = BeforeAddItem;
            if (ae != null)
            {
                ObservableCollectionEventArgs<T> e = new ObservableCollectionEventArgs<T>(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        protected virtual bool OnBeforeRemoveItemEvent(T item)
        {
            BeforeRemoveItemEventHandler ae = BeforeRemoveItem;
            if (ae != null)
            {
                ObservableCollectionEventArgs<T> e = new ObservableCollectionEventArgs<T>(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        protected virtual void OnRemoveItemEvent(T item)
        {
            RemoveItemEventHandler ae = RemoveItem;
            if (ae != null)
            {
                ae(this, new ObservableCollectionEventArgs<T>(item));
            }
        }

        #endregion

        #region private fields

        private readonly List<T> innerArray;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of <c>ObservableCollection</c>.
        /// </summary>
        public ObservableCollection()
        {
            innerArray = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of <c>ObservableCollection</c> and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of items the collection can initially store.</param>
        public ObservableCollection(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "The collection capacity cannot be negative.");
            }
            innerArray = new List<T>(capacity);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set.</param>
        /// <returns>The object at the specified index.</returns>
        public T this[int index]
        {
            get { return innerArray[index]; }
            set
            {
                T remove = innerArray[index];
                T add = value;

                if (OnBeforeRemoveItemEvent(remove))
                {
                    return;
                }

                if (OnBeforeAddItemEvent(add))
                {
                    return;
                }

                innerArray[index] = value;
                OnAddItemEvent(add);
                OnRemoveItemEvent(remove);
            }
        }

        /// <summary>
        /// Gets the number of object contained in the collection.
        /// </summary>
        public int Count
        {
            get { return innerArray.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Reverses the order of the elements in the entire list.
        /// </summary>
        public void Reverse()
        {
            innerArray.Reverse();
        }

        /// <summary>
        /// Sorts the elements in the entire System.Collections.Generic.List&lt;T&gt; using the specified System.Comparison&lt;T&gt;.
        /// </summary>
        /// <param name="comparision">The System.Comparison&lt;T&gt; to use when comparing elements.</param>
        public void Sort(Comparison<T> comparision)
        {
            innerArray.Sort(comparision);
        }

        /// <summary>
        /// Sorts the elements in a range of elements in System.Collections.Generic.List&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            innerArray.Sort(index, count, comparer);
        }

        /// <summary>
        /// Sorts the elements in a range of elements in System.Collections.Generic.List&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        public void Sort(IComparer<T> comparer)
        {
            innerArray.Sort(comparer);
        }

        /// <summary>
        /// Sorts the elements in the entire System.Collections.Generic.List&lt;T&gt; using the default comparer.
        /// </summary>
        public void Sort()
        {
            innerArray.Sort();
        }

        /// <summary>
        /// Adds an object to the collection.
        /// </summary>
        /// <param name="item"> The object to add to the collection.</param>
        /// <returns>True if the object has been added to the collection, or false otherwise.</returns>
        public void Add(T item)
        {
            if (OnBeforeAddItemEvent(item))
            {
                throw new ArgumentException("The item cannot be added to the collection.", nameof(item));
            }
            innerArray.Add(item);
            OnAddItemEvent(item);
        }

        /// <summary>
        /// Adds an object list to the end of the collection.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (T item in collection)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Inserts an object into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can not be null.</param>
        /// <returns>True if the object has been inserted to the collection; otherwise, false.</returns>
        public void Insert(int index, T item)
        {
            if (index < 0 || index >= innerArray.Count)
            {
                throw new ArgumentOutOfRangeException(string.Format("The parameter index {0} must be in between {1} and {2}.", index, 0, innerArray.Count));
            }

            if (OnBeforeRemoveItemEvent(innerArray[index]))
            {
                return;
            }

            if (OnBeforeAddItemEvent(item))
            {
                throw new ArgumentException("The item cannot be added to the collection.", nameof(item));
            }

            OnRemoveItemEvent(innerArray[index]);
            innerArray.Insert(index, item);
            OnAddItemEvent(item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>True if object is successfully removed; otherwise, false.</returns>
        public bool Remove(T item)
        {
            if (!innerArray.Contains(item))
            {
                return false;
            }

            if (OnBeforeRemoveItemEvent(item))
            {
                return false;
            }

            innerArray.Remove(item);
            OnRemoveItemEvent(item);
            return true;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection
        /// </summary>
        /// <param name="items">The list of objects to remove from the collection.</param>
        public void Remove(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (T item in items)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Removes the object at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the object to remove.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= innerArray.Count)
            {
                throw new ArgumentOutOfRangeException(string.Format("The parameter index {0} must be in between {1} and {2}.", index, 0, innerArray.Count));
            }

            T remove = innerArray[index];
            if (OnBeforeRemoveItemEvent(remove))
            {
                return;
            }

            innerArray.RemoveAt(index);
            OnRemoveItemEvent(remove);
        }

        /// <summary>
        /// Removes all object from the collection.
        /// </summary>
        public void Clear()
        {
            T[] items = new T[innerArray.Count];
            innerArray.CopyTo(items, 0);
            foreach (T item in items)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire collection, if found; otherwise, –1.</returns>
        public int IndexOf(T item)
        {
            return innerArray.IndexOf(item);
        }

        /// <summary>
        /// Determines whether an object is in the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>True if item is found in the collection; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return innerArray.Contains(item);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array"> The one-dimensional System.Array that is the destination of the elements copied from the collection. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            innerArray.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return innerArray.GetEnumerator();
        }

        #endregion

        #region private fields

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            Insert(index, item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}