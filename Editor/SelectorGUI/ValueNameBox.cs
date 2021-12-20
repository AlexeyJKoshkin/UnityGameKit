using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    /// <summary>
    ///     Вспомогательный класс, держит в себе значения и GUIContent для отображеия
    /// </summary>
    public class ValueNameBox<T> : IEnumerable<T>, IGUIContentHelper<T>, IList
    {
        private readonly ContentMaker<T> _contentGetter;

        private readonly GUIContent _ErrorContent = new GUIContent("Wrong Index");

        private T[] _items;

        private GUIContent[] _names;

        public ValueNameBox(ContentMaker<T> contentHelper = null)
        {
            _contentGetter = contentHelper ?? GetContentFor;
            _names         = new GUIContent[0];
            _items         = new T[0];
        }

        public GUIContent[] NameContent => _names;


        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _items.Length) return default;
                return _items[index];
            }
            set
            {
                _items[index] = value;
                var content = _contentGetter(value, index);
                _names[index] = content;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var v in _items)
                yield return v;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<(T item, GUIContent content)> IterateItemWithContent()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                yield return (_items[i], _names[i]);
            }
        }

        public GUIContent GetContentFor(T value, int index = -1)
        {
            return value.GetContentForEnum();
        }

        public int Count => _items.Length;

        public void RefreshContent()
        {
            for (var i = 0; i < _items.Length; i++)
                this[i] = _items[i];
        }

        public void RefreshContent(int index)
        {
            if (index < 0 || index >= _items.Length) return;
            this[index] = _items[index];
        }

        public void InitValues(IEnumerable<T> values)
        {
            if (values == null) return;
            _items = values.ToArray();
            _names = new GUIContent[_items.Length];
            for (int i = 0; i < _items.Length; i++) _names[i] = _contentGetter(_items[i], i);
        }

        /// <summary>
        ///     Попытка добавить еще одно значение в список
        /// </summary>
        /// <param name="value">значение</param>
        /// <returns>true если добавился</returns>
        public void Add(T value)
        {
            ArrayUtility.Add(ref _items, value);
            var content = _contentGetter(value, _names.Length);
            ArrayUtility.Add(ref _names, content);
        }
        
        public void SwapElements(int oldIndex, int newIndex)
        {
            var element = _items[oldIndex];
            _items[oldIndex] = _items[newIndex];
            _items[newIndex] = element;
            
            var elementName = _names[oldIndex];
            _names[oldIndex] = _names[newIndex];
            _names[newIndex] = elementName;
        }


        public GUIContent GetContent(int index)
        {
            if (index < 0 || index > _items.Length) return _ErrorContent;
            return _names[index];
        }
        
        public GUIContent GetContent(T item)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].Equals(item)) return _names[i];
            }
            return GUIContent.none;
        }

        public void Sort(Comparison<T> comprasion = null)
        {
            if (_items.Length < 2) return;
            var list = new List<T>(_items);
            var temp = new Dictionary<T, GUIContent>();
            for (var i = 0; i < Count; i++)
                temp.Add(_items[i], _names[i]);
            if (comprasion == null)
                list.Sort();
            else
                list.Sort(comprasion);
            _items = list.ToArray();
            for (var i = 0; i < _items.Length; i++)
                _names[i] = temp[_items[i]];
        }

        public bool Contains(T value)
        {
            return ArrayUtility.FindIndex(_items, o => o.Equals(value)) > -1;
        }

        public bool Contains(Predicate<T> predicate)
        {
            return ArrayUtility.FindIndex(_items, predicate) > -1;
        }

        public int TryRemove(T value)
        {
            var index = ArrayUtility.FindIndex(_items, o => o.Equals(value));
            if (index > -1)
            {
                ArrayUtility.RemoveAt(ref _items, index);
                ArrayUtility.RemoveAt(ref _names, index);
                return index;
            }

            return -1;
        }

        /*public static class DefaultContentGetter
        {
            private static string GetTooltip(T value)
            {
                var itemType = typeof(T);

                return itemType.IsEnum ? GetToolTipForEnum(itemType, value) : null;
            }

            public static GUIContent GetContentFor(T value, int index = -1)
            {
                var itemType = typeof(T);

                return itemType.IsEnum ? value.GetContentForEnum() :
                new GUIContent
                {
                    text = value.ToString(),
                    tooltip = GetTooltip(value)
                };
            }

            private static string GetToolTipForEnum(Type t, T ie)
            {
                try
                {
                    var memInfo = t.GetMember(ie.ToString());
                    if (memInfo.Length == 0) return null;
                    var attributes = memInfo[0].GetCustomAttributes(typeof(TooltipAttribute), false);
                    if (attributes.Length == 0)
                        return ie.ToString();
                    var attr = (TooltipAttribute) attributes[0];
                    return attr.tooltip;
                }
                catch (Exception e)
                {
                    Debug.LogError(t.Name + " " + ie);
                    Debug.LogException(e);
                    return null;
                }
            }
        }*/

#region IList

        bool IList.IsReadOnly => false;

        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is T)
                    this[index] = (T) value;
            }
        }

        int IList.Add(object value)
        {
            Add((T) value);
            return ArrayUtility.FindIndex(_items, o => o.Equals(value));
        }

        public void Clear()
        {
            _items = new T[0];
            _names = new GUIContent[0];
        }

        bool IList.Contains(object value)
        {
            return ArrayUtility.FindIndex(_items, o => o.Equals(value)) > -1;
        }

        public int IndexOf(object value)
        {
            return ArrayUtility.FindIndex(_items, o => o.Equals(value));
        }

        void IList.Insert(int index, object value)
        {
            if (!(value is T)) return;
            if (_items.Any(o => o.Equals(value))) return;
            ArrayUtility.Insert(ref _items, index, (T) value);
            var content = _contentGetter((T) value, _names.Length);
            ArrayUtility.Insert(ref _names, index, content);
        }

        void IList.Remove(object value)
        {
            int index = IndexOf(value);

            if (index > -1) RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            ArrayUtility.RemoveAt(ref _items, index);
            ArrayUtility.RemoveAt(ref _names, index);
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, Count);
        }


        bool ICollection.IsSynchronized => false;
        private object _syncRoot;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                return _syncRoot;
            }
        }

        bool IList.IsFixedSize => false;

#endregion;

       
    }
}