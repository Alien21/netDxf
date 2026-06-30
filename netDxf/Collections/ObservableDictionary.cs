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
    public sealed class ObservableDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>
    {
        #region delegates and events

        public delegate void AddItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void BeforeAddItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void RemoveItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void BeforeRemoveItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public event BeforeAddItemEventHandler BeforeAddItem;
        public event AddItemEventHandler AddItem;
        public event BeforeRemoveItemEventHandler BeforeRemoveItem;
        public event RemoveItemEventHandler RemoveItem;

        private bool BeforeAddItemEvent(KeyValuePair<TKey, TValue> item)
        {
            BeforeAddItemEventHandler ae = BeforeAddItem;
            if (ae != null)
            {
                ObservableDictionaryEventArgs<TKey, TValue> e = new ObservableDictionaryEventArgs<TKey, TValue>(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        private void AddItemEvent(KeyValuePair<TKey, TValue> item)
        {
            AddItemEventHandler ae = AddItem;
            if (ae != null)
                ae(this, new ObservableDictionaryEventArgs<TKey, TValue>(item));
        }

        private bool BeforeRemoveItemEvent(KeyValuePair<TKey, TValue> item)
        {
            BeforeRemoveItemEventHandler ae = BeforeRemoveItem;
            if (ae != null)
            {
                ObservableDictionaryEventArgs<TKey, TValue> e = new ObservableDictionaryEventArgs<TKey, TValue>(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        private void RemoveItemEvent(KeyValuePair<TKey, TValue> item)
        {
            RemoveItemEventHandler ae = RemoveItem;
            if (ae != null)
                ae(this, new ObservableDictionaryEventArgs<TKey, TValue>(item));
        }

        #endregion

        #region private fields

        private readonly Dictionary<TKey, TValue> innerDictionary;

        #endregion

        #region constructor

        public ObservableDictionary()
        {
            innerDictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(int capacity)
        {
            innerDictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            innerDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            innerDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        #endregion

        #region public properties

        public TValue this[TKey key]
        {
            get { return innerDictionary[key]; }
            set
            {
                KeyValuePair<TKey, TValue> remove = new KeyValuePair<TKey, TValue>(key, innerDictionary[key]);
                KeyValuePair<TKey, TValue> add = new KeyValuePair<TKey, TValue>(key, value);

                if (BeforeRemoveItemEvent(remove))
                {
                    return;
                }

                if (BeforeAddItemEvent(add))
                {
                    return;
                }

                innerDictionary[key] = value;
                AddItemEvent(add);
                RemoveItemEvent(remove);
            }
        }

        public ICollection<TKey> Keys
        {
            get { return innerDictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return innerDictionary.Values; }
        }

        public int Count
        {
            get { return innerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region public methods

        public void Add(TKey key, TValue value)
        {
            KeyValuePair<TKey, TValue> add = new KeyValuePair<TKey, TValue>(key, value);
            if (BeforeAddItemEvent(add))
            {
                throw new ArgumentException("The item cannot be added to the dictionary.", nameof(value));
            }
            innerDictionary.Add(key, value);
            AddItemEvent(add);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            if (!innerDictionary.ContainsKey(key))
            {
                return false;
            }

            KeyValuePair<TKey, TValue> remove = new KeyValuePair<TKey, TValue>(key, innerDictionary[key]);
            if (BeforeRemoveItemEvent(remove))
            {
                return false;
            }

            innerDictionary.Remove(key);
            RemoveItemEvent(remove);

            return true;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!ReferenceEquals(item.Value, innerDictionary[item.Key]))
            {
                return false;
            }

            return Remove(item.Key);
        }

        public void Clear()
        {
            TKey[] keys = new TKey[innerDictionary.Count];
            innerDictionary.Keys.CopyTo(keys, 0);
            foreach (TKey key in keys)
            {
                Remove(key);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>) innerDictionary).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return innerDictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return innerDictionary.ContainsValue(value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return innerDictionary.TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>) innerDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return innerDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}