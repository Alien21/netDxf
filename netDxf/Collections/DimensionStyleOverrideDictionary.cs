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
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a dictionary of <see cref="DimensionStyleOverride">DimensionStyleOverrides</see>.
    /// </summary>
    public sealed class DimensionStyleOverrideDictionary :
        IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>
    {
        #region delegates and events

        public delegate void BeforeAddItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public event BeforeAddItemEventHandler BeforeAddItem;

        private bool OnBeforeAddItemEvent(DimensionStyleOverride item)
        {
            BeforeAddItemEventHandler ae = BeforeAddItem;
            if (ae != null)
            {
                DimensionStyleOverrideDictionaryEventArgs e = new DimensionStyleOverrideDictionaryEventArgs(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        public delegate void AddItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public event AddItemEventHandler AddItem;

        private void OnAddItemEvent(DimensionStyleOverride item)
        {
            AddItemEventHandler ae = AddItem;
            if (ae != null)
                ae(this, new DimensionStyleOverrideDictionaryEventArgs(item));
        }

        public delegate void BeforeRemoveItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public event BeforeRemoveItemEventHandler BeforeRemoveItem;

        private bool OnBeforeRemoveItemEvent(DimensionStyleOverride item)
        {
            BeforeRemoveItemEventHandler ae = BeforeRemoveItem;
            if (ae != null)
            {
                DimensionStyleOverrideDictionaryEventArgs e = new DimensionStyleOverrideDictionaryEventArgs(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        public delegate void RemoveItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public event RemoveItemEventHandler RemoveItem;

        private void OnRemoveItemEvent(DimensionStyleOverride item)
        {
            RemoveItemEventHandler ae = RemoveItem;
            if (ae != null)
                ae(this, new DimensionStyleOverrideDictionaryEventArgs(item));
        }

        #endregion

        #region private fields

        private readonly Dictionary<DimensionStyleOverrideType, DimensionStyleOverride> innerDictionary;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of <c>DimensionStyleOverrideDictionary</c>.
        /// </summary>
        public DimensionStyleOverrideDictionary()
        {
            innerDictionary = new Dictionary<DimensionStyleOverrideType, DimensionStyleOverride>();
        }

        /// <summary>
        /// Initializes a new instance of <c>DimensionStyleOverrideDictionary</c> and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of items the collection can initially store.</param>
        public DimensionStyleOverrideDictionary(int capacity)
        {
            innerDictionary = new Dictionary<DimensionStyleOverrideType, DimensionStyleOverride>(capacity);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> with the specified type.
        /// </summary>
        /// <param name="type">The type of the DimensionStyleOverride to get or set.</param>
        /// <returns>The <see cref="DimensionStyleOverride">DimensionStyleOverride</see> with the specified type.</returns>
        public DimensionStyleOverride this[DimensionStyleOverrideType type]
        {
            get { return innerDictionary[type]; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (type != value.Type)
                {
                    throw new ArgumentException(string.Format("The dictionary type: {0}, and the DimensionStyleOverride type: {1}, must be the same", type, value.Type));
                }

                DimensionStyleOverride remove = innerDictionary[type];
                if (OnBeforeRemoveItemEvent(remove))
                {
                    return;
                }

                if (OnBeforeAddItemEvent(value))
                {
                    return;
                }

                innerDictionary[type] = value;
                OnAddItemEvent(value);
                OnRemoveItemEvent(remove);
            }
        }

        /// <summary>
        /// Gets an ICollection containing the types of the current dictionary.
        /// </summary>
        public ICollection<DimensionStyleOverrideType> Types
        {
            get { return innerDictionary.Keys; }
        }

        /// <summary>
        /// Gets an ICollection containing the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> list of the current dictionary.
        /// </summary>
        public ICollection<DimensionStyleOverride> Values
        {
            get { return innerDictionary.Values; }
        }

        /// <summary>
        /// Gets the number of <see cref="DimensionStyleOverride">DimensionStyleOverride</see> contained in the current dictionary.
        /// </summary>
        public int Count
        {
            get { return innerDictionary.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the actual dictionary is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds a <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to the dictionary from its type and value.
        /// </summary>
        /// <param name="type">Dimension style override type.</param>
        /// <param name="value">Dimension style override value.</param>
        /// <remarks>A new DimensionStyleOverride will be created from the specified arguments.</remarks>
        public void Add(DimensionStyleOverrideType type, object value)
        {
            Add(new DimensionStyleOverride(type, value));
        }

        /// <summary>
        /// Adds an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to the dictionary.
        /// </summary>
        /// <param name="item">The <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to add.</param>
        public void Add(DimensionStyleOverride item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (OnBeforeAddItemEvent(item))
            {
                throw new ArgumentException(string.Format("The DimensionStyleOverride {0} cannot be added to the collection.", item), nameof(item));
            }

            innerDictionary.Add(item.Type, item);
            OnAddItemEvent(item);
        }

        /// <summary>
        /// Adds an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> list to the dictionary.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added.</param>
        public void AddRange(IEnumerable<DimensionStyleOverride> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            // we will make room for so the collection will fit without having to resize the internal array during the Add method
            foreach (DimensionStyleOverride item in collection)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Removes an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> of the specified type from the current dictionary.
        /// </summary>
        /// <param name="type">The type of the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to remove.</param>
        /// <returns>True if the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> is successfully removed; otherwise, false.</returns>
        public bool Remove(DimensionStyleOverrideType type)
        {
            if (!innerDictionary.TryGetValue(type, out DimensionStyleOverride remove))
            {
                return false;
            }

            if (OnBeforeRemoveItemEvent(remove))
            {
                return false;
            }

            innerDictionary.Remove(type);
            OnRemoveItemEvent(remove);
            return true;
        }

        /// <summary>
        /// Removes all <see cref="DimensionStyleOverride">DimensionStyleOverride</see> from the current dictionary.
        /// </summary>
        public void Clear()
        {
            DimensionStyleOverrideType[] types = new DimensionStyleOverrideType[innerDictionary.Count];
            innerDictionary.Keys.CopyTo(types, 0);
            foreach (DimensionStyleOverrideType tag in types)
            {
                Remove(tag);
            }
        }

        /// <summary>
        /// Determines whether current dictionary contains an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> of the specified type.
        /// </summary>
        /// <param name="type">The type to locate in the current dictionary.</param>
        /// <returns>True if the current dictionary contains an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> of the type; otherwise, false.</returns>
        public bool ContainsType(DimensionStyleOverrideType type)
        {
            return innerDictionary.ContainsKey(type);
        }

        /// <summary>
        /// Determines whether current dictionary contains a specified <see cref="DimensionStyleOverride">DimensionStyleOverride</see>.
        /// </summary>
        /// <param name="value">The <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to locate in the current dictionary.</param>
        /// <returns>True if the current dictionary contains the <see cref="DimensionStyleOverride">DimensionStyleOverride</see>; otherwise, false.</returns>
        public bool ContainsValue(DimensionStyleOverride value)
        {
            return innerDictionary.ContainsValue(value);
        }

        /// <summary>
        /// Gets the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> associated of the specified type.
        /// </summary>
        /// <param name="type">The type whose value to get.</param>
        /// <param name="value">When this method returns, the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> associated of the specified type,
        /// if the tag is found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>True if the current dictionary contains an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> of the specified type; otherwise, false.</returns>
        public bool TryGetValue(DimensionStyleOverrideType type, out DimensionStyleOverride value)
        {
            return innerDictionary.TryGetValue(type, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the dictionary.</returns>
        public IEnumerator<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>> GetEnumerator()
        {
            return innerDictionary.GetEnumerator();
        }

        #endregion

        #region private properties

        ICollection<DimensionStyleOverrideType> IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.Keys
        {
            get { return innerDictionary.Keys; }
        }

        #endregion

        #region private methods

        bool IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.ContainsKey(DimensionStyleOverrideType tag)
        {
            return innerDictionary.ContainsKey(tag);
        }

        void IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.Add(DimensionStyleOverrideType key, DimensionStyleOverride value)
        {
            Add(value);
        }

        void ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Add(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
        {
            Add(item.Value);
        }

        bool ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Remove(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
        {
            if (!ReferenceEquals(item.Value, innerDictionary[item.Key]))
            {
                return false;
            }

            return Remove(item.Key);
        }

        bool ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Contains(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
        {
            return ((IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>) innerDictionary).Contains(item);
        }

        void ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.CopyTo(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>[] array, int arrayIndex)
        {
            ((IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>) innerDictionary).CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}