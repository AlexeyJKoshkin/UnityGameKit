using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    /// <summary>
    ///     Вспомогательный класс для отрисовки в редакторe Popup Selection.
    ///     довольно часто приходится  выбирать из чего либо, и влом постоянно реализовывать одно и тоже в каждом классе.
    ///     При  инициализации элементов создается новый массив элементов и массив имен.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SelectionGUI<T> : ISelectionGUI<T>
    {
        public delegate bool ItemSelectDrawCallback(T element, int index, GUIContent name, bool isSelected);

        protected readonly Type CurrentTypeSelected = typeof(T);

        private int _currentIndex;

        private bool _isDirty;

        protected ValueNameBox<T> _valueBox;

        public Comparison<T> ComparisonDelegate;

        public T DefaultValue = default;

        public bool Expand = false;

        public string NoElementsMessage = "No Elements " + typeof(T).Name;

        /// <summary>
        ///     Пользователь выбрал новый активый элемент
        /// </summary>
        public Action<T> OnSelectNewItemCallback;


        protected SelectionGUI(IEnumerable<T> data = null, ContentMaker<T> contentHelper = null, int defaultIndex = -1)
        {
            _valueBox = contentHelper == null ? new ValueNameBox<T>() : new ValueNameBox<T>(contentHelper);
            InitValues(data ?? new T[] { });
            _currentIndex = defaultIndex;
        }

        public bool IsSelected => Count > 0 && _currentIndex > -1;

        public int Count => _valueBox.Count;
        public IEnumerable<(T item, GUIContent content)> GetItemsWithContent()
        {
            return _valueBox.IterateItemWithContent();
        }

        /// <summary>
        ///     Текущее выбранное значение
        /// </summary>
        public T CurrentValue
        {
            get => _currentIndex > -1 ? _valueBox[_currentIndex] : DefaultValue;
            set
            {
                _isDirty      = true;
                _currentIndex = -1;
                for (var i = 0; i < _valueBox.Count; i++)
                {
                    if (!_valueBox[i].Equals(value)) continue;
                    _currentIndex = i;
                    break;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _valueBox.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SetCurrent(T value)
        {
            _currentIndex = -1;
            for (var i = 0; i < _valueBox.Count; i++)
            {
                if (!CheckEqual(_valueBox[i], value)) continue;
                _currentIndex = i;
                break;
            }
        }

        private bool CheckEqual(T element, T oldElement)
        {
            return EqualityComparer<T>.Default.Equals(element, oldElement);
        }

        public void SetCurrentByIndex(int index)
        {
            _currentIndex = index >= _valueBox.Count ? -1 : index;
            OnSelectNewItemCallback?.Invoke(CurrentValue);
        }

        public void Sort(Comparison<T> comprasion = null)
        {
            var currentValue = CurrentValue;
            _valueBox.Sort(comprasion);
            for (var i = 0; i < _valueBox.Count; i++)
            {
                if (!_valueBox[i].Equals(currentValue)) continue;
                _currentIndex = i;
                break;
            }
        }

        public void RefreshContent()
        {
            _valueBox.RefreshContent();
        }

        public void RefreshContent(int index)
        {
            _valueBox.RefreshContent(index);
        }

        /// <summary>
        ///     Добавить елемент в списку выбора, если такой элемент уже присутствует, то добавление игнорируется
        /// </summary>
        /// <param name="value"></param>
        /// <param name="setCurrent"></param>
        public void AddValue(T value, bool setCurrent = false)
        {
            if (_valueBox.Any(o => o.Equals(value))) return;
            _valueBox.Add(value);
            if (setCurrent)
                _currentIndex = _valueBox.Count - 1;
        }

        public void RemoveAll(Predicate<T> func)
        {
            List<T> deletedValues = new List<T>();

            foreach (var value in _valueBox)
                if (func(value))
                    deletedValues.Add(value);

            foreach (var deletedValue in deletedValues)
            {
                var deletedIndex = _valueBox.TryRemove(deletedValue);
                if (deletedIndex == _currentIndex)
                    _currentIndex = -1;
            }

            _isDirty = true;
        }
        
        public void RemoveAt(int listIndex)
        {
            _valueBox.RemoveAt(listIndex);
            if (_currentIndex == listIndex)
            {
                _currentIndex = -1;
            }
        }
        
        public void SwapElements(int oldIndex, int newIndex)
        {
            _valueBox.SwapElements(oldIndex, newIndex);
        }

        public void Remove(T value)
        {
            var deletedIndex = _valueBox.TryRemove(value);
            if (deletedIndex == _currentIndex)
                _currentIndex = -1;
            _isDirty = true;
        }

        public bool Contains(T value)
        {
            return _valueBox.Contains(value);
        }

        public bool Contains(Predicate<T> predicate)
        {
            return _valueBox.Contains(predicate);
        }

        /// <summary>
        ///     Сбросить выбор. активным становится "никакое" значение. Событие выбора нового не срабатывает
        /// </summary>
        public void DropSelection()
        {
            _currentIndex = -1;
        }

        /// <summary>
        ///     Заменить значение по индексу
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Change(int index, T value)
        {
            if (index >= _valueBox.Count) throw new ArgumentException("Index is bigger than array of values");
            _valueBox[index] = value;
        }

        public void InitValues(IEnumerable<T> data, int selectedIndex = -1)
        {
            if (data == null) return;
            _valueBox.InitValues(data);
            if (ComparisonDelegate != null)
                Sort(ComparisonDelegate);
            _currentIndex = Mathf.Min(selectedIndex, _valueBox.Count - 1);
        }

        public void DoSelectGUI(Rect rect, string label)
        {
            if (_valueBox.Count == 0)
            {
                if (!string.IsNullOrEmpty(NoElementsMessage))
                    EditorGUI.HelpBox(rect, NoElementsMessage, MessageType.Warning);
                _currentIndex = -1;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var index = DrawSelectionGUI(_currentIndex, rect,
                                             string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label));
                if (EditorGUI.EndChangeCheck())
                    if (OnSelectNewItemCallback != null && index != _currentIndex)
                    {
                        var newCurrent = index > -1 ? _valueBox[index] : DefaultValue;
                        //  Debug.Log("new selected " + newCurrent + " " + index + " " + typeof(T).Name);
                        OnSelectNewItemCallback(newCurrent);
                    }

                // возможно мы выставили новое текущее значение в коллбэке. в таком случае не меняем текущий индекс выбранного
                if (_isDirty)
                    _isDirty = false;
                else
                    _currentIndex = index;
            }
        }


        public void DoSelectGUI(string label = null)
        {
            DoSelectGUI(string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label));
        }
        
        
        public void DoSelectGUI(GUIContent content)
        {
           
            if (_valueBox.Count == 0)
            {
                if (!string.IsNullOrEmpty(NoElementsMessage))
                    EditorGUILayout.HelpBox(NoElementsMessage, MessageType.Warning);
                _currentIndex = -1;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var index = DrawSelectionGUI(_currentIndex, content);
                if (EditorGUI.EndChangeCheck())
                    if (OnSelectNewItemCallback != null && index != _currentIndex)
                    {
                        var newCurrent = index > -1 ? _valueBox[index] : DefaultValue;
                        //  Debug.Log("new selected " + newCurrent + " " + index + " " + typeof(T).Name);
                        OnSelectNewItemCallback(newCurrent);
                    }

                // возможно мы выставили новое текущее значение в коллбэке. в таком случае не меняем текущий индекс выбранного
                if (_isDirty)
                    _isDirty = false;
                else
                    _currentIndex = index;
            }
        }


        public T DoSelectGUI(Rect rect, T currentValue, string label = null)
        {
            SetCurrent(currentValue);
            DoSelectGUI(rect, label);
            return CurrentValue;
        }


        /*public readonly GUIStyle footerBackground = (GUIStyle) "RL Footer";
        public GUIContent iconToolbarPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
        public GUIContent iconToolbarPlusMore = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
        public GUIContent iconToolbarMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
        public readonly GUIStyle preButton = (GUIStyle) "RL FooterButton";

        private Action OnAdd;

        private void DrawFOOTER(Rect rect)
        {
             float xMax = rect.xMax;
                    float x = xMax - 8f;
                   // if (list.displayAdd)
                      x -= 25f;
                    /*if (list.displayRemove)
                      x -= 25f;#1#
                    rect = new Rect(x, rect.y, xMax - x, rect.height);
                    Rect rect1 = new Rect(x + 4f, rect.y - 3f, 25f, 13f);
                    Rect position = new Rect(xMax - 29f, rect.y - 3f, 25f, 13f);
                    if (Event.current.type == EventType.Repaint)
                      this.footerBackground.Draw(rect, false, false, false, false);
                    if (GUI.Button(rect1, OnAdd == null ? this.iconToolbarPlus : this.iconToolbarPlusMore, this.preButton))
                    {
                        Debug.LogError("Добавляй");
                        if (list.onAddDropdownCallback != null)
                            list.onAddDropdownCallback(rect1, list);
                        else if (list.onAddCallback != null)
                            list.onAddCallback(list);
                        else
                            this.DoAddButton(list);
                        if (list.onChangedCallback != null)
                            list.onChangedCallback(list);
                    }
                    if (!list.displayRemove)
                      return;
                    using (new EditorGUI.DisabledScope(list.index < 0 || list.index >= list.count || list.onCanRemoveCallback != null && !list.onCanRemoveCallback(list)))
                    {
                      if (GUI.Button(position, this.iconToolbarMinus, this.preButton))
                      {
                        if (list.onRemoveCallback == null)
                          this.DoRemoveButton(list);
                        else
                          list.onRemoveCallback(list);
                        if (list.onChangedCallback != null)
                          list.onChangedCallback(list);
                      }
                    }
        }*/

        protected abstract int DrawSelectionGUI(int index, GUIContent label);

        protected abstract int DrawSelectionGUI(int index, Rect rect, GUIContent label = null);

        protected abstract string DefaultGetName(T obj, int n);

        public void Clear()
        {
            _currentIndex = -1;
            _valueBox.Clear();
        }
    }
}