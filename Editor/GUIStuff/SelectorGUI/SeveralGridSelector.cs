using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public interface ISeveralSelector<T>
    {
        int CountSelectedElements { get; }

        IEnumerable<T> All();
        IEnumerable<T> Selected();
        IEnumerable<T> NonSelected();

        bool IsSelected(T kitchenLevelModel);

        bool IsSelected(Func<T, bool> checker);
    }

    public class SeveralGridSelector<T> : ISeveralSelector<T>
    {
        public int CountSelectedElements { get; private set; }
        
        public delegate bool ElemntDrawerDelegate(T item, bool isSelected, GUIContent content, int index,
                                                  params GUILayoutOption[] options);

        private readonly ValueNameBox<T> _valueBox;

        protected readonly Type CurrentTypeSelected = typeof(T);

        /// <summary>
        ///     Делегат который выполняет отрисовку гуя
        /// </summary>
        private Action _drawingAction;

        private Vector2 _scrollPosition;

        private bool[] _selected;

        private bool _showScroll;
        private GUILayoutOption[] _tempOptions;

        private GUIStyle _tempStyle;

        private bool _wasChange;
        private int _xCount = 4;

        public ElemntDrawerDelegate ItemElementDrawCallback;


        public int PixelSpaceBetweenItems = 2;

        public SeveralGridSelector(bool showScroll, ContentMaker<T> guicontentHelper) : this(showScroll, null,
            guicontentHelper)
        {
        }

        public SeveralGridSelector(bool showScroll, IEnumerable<T> allValues = null,
                                   ContentMaker<T> guicontentHelper = null)
        {
            ShowScroll = showScroll;
            if (allValues == null) allValues = new T[0];
            _valueBox = new ValueNameBox<T>(guicontentHelper);
            InitValues(allValues);
        }

        public int XCount
        {
            get => _xCount;
            set => _xCount = Mathf.Clamp(value, 1, 6);
        }

        /// <summary>
        ///     надо ли показывать скролл
        /// </summary>
        public bool ShowScroll
        {
            get => _showScroll;
            set
            {
                _drawingAction = null;
                _showScroll     = value;
                if (value)
                {
                    _drawingAction =  PrepareScroll;
                    _drawingAction += Drawing;
                    _drawingAction += EndScroll;
                }
                else
                {
                    _drawingAction += Drawing;
                }
            }
        }

       

        public bool DoSelectGUI(GUIStyle style = null, params GUILayoutOption[] options)
        {
            _tempStyle   = style ?? GUI.skin.toggle;
            _tempOptions = options;
            // EditorGUI.BeginChangeCheck();
            _drawingAction?.Invoke();
            return _wasChange;
        }


        public void DoSelectGUI(float fullWidth)
        {
            if (_tempStyle == null)
                _tempStyle = GUI.skin.button;
            bool isOpen = false;

            float elemWidth = fullWidth / _xCount;
            _tempOptions = new[] {GUILayout.Width(elemWidth)};
            using (new EditorGUILayout.VerticalScope())
            {
                int n = 0;
                for (int i = 0; i < _valueBox.Count; i++)
                {
                    if (n == 0)
                    {
                        isOpen = true;
                        EditorGUILayout.BeginHorizontal();
                    }

                    DrawElement(i);
                    n++;
                    if (n == _xCount)
                    {
                        isOpen = false;
                        EditorGUILayout.EndHorizontal();
                        n = 0;
                    }
                }

                if (isOpen) EditorGUILayout.EndHorizontal();
            }
        }

        public IEnumerable<T> All()
        {
            return _valueBox;
        }

        public IEnumerable<T> Selected()
        {
            for (var i = 0; i < _selected.Length; i++)
                if (_selected[i])
                    yield return _valueBox[i];
        }

        public IEnumerable<T> NonSelected()
        {
            for (var i = 0; i < _selected.Length; i++)
                if (!_selected[i])
                    yield return _valueBox[i];
        }

        /// <summary>
        ///     Выбран ли данный элемент
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsSelected(T element)
        {
            for (var i = 0; i < _valueBox.Count; i++)
                if (_valueBox[i].Equals(element))
                    return _selected[i];
            return false;
        }


        public bool IsSelected(Func<T, bool> checker)
        {
            for (var i = 0; i < _valueBox.Count; i++)
                if (checker(_valueBox[i]))
                    return _selected[i];
            return false;
        }

        public void InitValues(IEnumerable<T> allItems, Comparison<T> comparison = null)
        {
            if (allItems == null)
            {
                Clear();
                return;
            }

            _valueBox.InitValues(allItems);
            if (comparison != null)
                _valueBox.Sort(comparison);
            _selected = new bool[_valueBox.Count];
        }
        
        public void AddValue(T value, bool isSelected)
        {
            if(_valueBox.Contains(value)) return;
            _valueBox.Add(value);
            if (isSelected)
                CountSelectedElements++;
            ArrayUtility.Add(ref _selected, isSelected);
        }


        /// <summary>
        ///     очистить все значения
        /// </summary>
        public void Clear()
        {
            _valueBox.InitValues(new T[0]);
            CountSelectedElements = 0;
            _selected             = new bool[0];
        }

        public void SetCurrent(IEnumerable<T> currentSelectedItems)
        {
            _selected             = new bool[_valueBox.Count]; // сброс всех выделенных
            CountSelectedElements = 0;
            if (currentSelectedItems == null) return;
            foreach (var item in currentSelectedItems)
                for (var i = 0; i < _valueBox.Count; i++)
                    if (_valueBox[i].Equals(item))
                    {
                        _selected[i] = true;
                        CountSelectedElements++;
                    }
        }

        public void Reset()
        {
            CountSelectedElements = 0;
            if (_selected != null)
                for (var i = 0; i < _selected.Length; i++)
                    _selected[i] = false;
        }

        private void PrepareScroll()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false, GUIStyle.none,
                GUI.skin.verticalScrollbar, GUI.skin.scrollView, GUILayout.ExpandHeight(false));
        }

        protected virtual void Drawing()
        {
            _wasChange = false;
            EditorGUI.BeginChangeCheck();
            for (var i = 0; i < _valueBox.Count; i++)
                DrawElement(i);
            _wasChange = EditorGUI.EndChangeCheck();
        }

        private void EndScroll()
        {
            EditorGUILayout.EndScrollView();
        }

        private void DrawElement(int i)
        {
            var wasSelected = _selected[i];
            if (ItemElementDrawCallback == null)
            {
                _selected[i] = GUILayout.Toggle(_selected[i], _valueBox.GetContent(i), _tempStyle);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                _selected[i] =
                    ItemElementDrawCallback(_valueBox[i], _selected[i], _valueBox.GetContent(i), i, _tempOptions);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(PixelSpaceBetweenItems);
            if (wasSelected != _selected[i])
                if (wasSelected) CountSelectedElements--;
                else CountSelectedElements++;
        }

        public void SetSelected(T element, bool value = true)
        {
            for (var i = 0; i < _valueBox.Count; i++)
            {
                var oldElement = _valueBox[i];

                bool isEqual = CheckEqual(element, oldElement);


                if (isEqual)
                {
                    // если выставляем в тру и текущий элемент не выбран, то увеличиваем счетчик
                    if (value && !_selected[i])
                        CountSelectedElements++;

                    //если снимаем флаг, и текущий выбран, то уменьшаем счетчик
                    if (!value && _selected[i])
                        CountSelectedElements--;

                    _selected[i] = value;
                    break;
                }
            }
        }

        public void SetSelected(Predicate<T> predicate, bool value = true)
        {
            for (var i = 0; i < _valueBox.Count; i++)
            {
                var oldElement = _valueBox[i];

                bool isEqual = predicate(oldElement);

                if (isEqual)
                {
                    // если выставляем в тру и текущий элемент не выбран, то увеличиваем счетчик
                    if (value && !_selected[i])
                        CountSelectedElements++;

                    //если снимаем флаг, и текущий выбран, то уменьшаем счетчик
                    if (!value && _selected[i])
                        CountSelectedElements--;

                    _selected[i] = value;
                    break;
                }
            }
        }

        private bool CheckEqual(T element, T oldElement)
        {
            return EqualityComparer<T>.Default.Equals(element, oldElement);
        }

        public void SetSelected(bool isSelected)
        {
            for (int i = 0; i < _selected.Length; i++) _selected[i] = isSelected;
            CountSelectedElements = isSelected ? _selected.Length : 0;
        }

        
    }
}