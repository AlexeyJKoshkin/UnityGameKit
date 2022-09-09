/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public abstract class SearchSelectEnumDrawer<T> : ISelectionGUI<T>
    {
        private readonly Dictionary<string, T> _nameToItemDictionary;
        private int _currentItemIndex;
        private string[] _filteredItems;

        private string _filterText = "";


        public SearchSelectEnumDrawer()
        {
            _nameToItemDictionary = new Dictionary<string, T>();
            _filteredItems        = new[] {"NONE"};
            _currentItemIndex     = 0;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _nameToItemDictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsSelected => _currentItemIndex > 0 && _filteredItems.Length > 0;

        public T CurrentValue { get; private set; }

        public int Count => _nameToItemDictionary.Count;

        public void SetSelected(T element)
        {
            _filterText = "";
            UpdateFilterItems();
            _currentItemIndex = _nameToItemDictionary.Values.ToList().FindIndex(o => o.Equals(element)) + 1;
        }

        protected void InitValues(IEnumerable<T> items, Func<T, string> GetName)
        {
            _nameToItemDictionary.Clear();
            Func<T, string> func                                          = GetName ?? DefaultGetName;
            foreach (var item in items) _nameToItemDictionary[func(item)] = item;
            UpdateFilterItems();
        }

        private string DefaultGetName(T arg)
        {
            return arg.ToString();
        }

        public T DoSelectGUI(Rect rect)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();

                Rect textRect = new Rect(rect);
                textRect.width *= 0.45f;
                _filterText    =  GUI.TextField(textRect, _filterText);
                if (EditorGUI.EndChangeCheck()) UpdateFilterItems();
                Rect popUpRect = new Rect(rect);
                popUpRect.width    *= 0.51f;
                popUpRect.position =  new Vector2(textRect.xMax + 2, rect.y);
                _currentItemIndex  =  EditorGUI.Popup(popUpRect, _currentItemIndex, _filteredItems);
            }

            return CurrentValue;
        }


        public void DoSelectGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                _filterText = GUILayout.TextField(_filterText);
                if (EditorGUI.EndChangeCheck()) UpdateFilterItems();
                _currentItemIndex = EditorGUILayout.Popup(_currentItemIndex, _filteredItems);
            }
        }

        private void UpdateFilterItems()
        {
            _filterText = _filterText.Replace(" ", null);
            List<string> list = list = new List<string>(_nameToItemDictionary.Count + 1) {"NONE"};
            if (string.IsNullOrEmpty(_filterText))
                list.AddRange(_nameToItemDictionary.Keys);
            else
                foreach (var key in _nameToItemDictionary.Keys)
                    if (key.Contains(_filterText))
                        list.Add(key);
            _filteredItems    = list.ToArray();
            _currentItemIndex = _filteredItems.Length == 1 ? _currentItemIndex = 0 : 1;
        }
    }

    public class EnumFilterSearchPopup<T> : SearchSelectEnumDrawer<T> where T : struct
    {
        public EnumFilterSearchPopup()
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"{typeof(T).Name} is Not ENUM");

            InitValues(Enum.GetValues(typeof(T)).Cast<T>(), null);
        }
    }
}*/