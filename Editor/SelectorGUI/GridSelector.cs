using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public class GridSelector<T> : SelectionGUI<T>
    {
        /// <summary>
        ///     Делегат отрисовки одноо элемента
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="isSelected"></param>
        /// <returns></returns>
        private const int MINUS_BTN_SIZE = 15;

        private bool _canDelete;

        private Func<int, int> ElementsDrawer;
        private Func<int,Rect, int> ElementsDrawerRect;

        public ItemSelectDrawCallback ItemDrawerCallback;

        public GUIStyle MinusBtnStyle {
            get => _minusStyle ?? EditorStyles.miniButton;
            set => _minusStyle = value;
        }
        private GUIStyle _minusStyle;
        
        public GUIStyle BtnStyle {
            get => _btnStyle ?? EditorStyles.miniButton;
            set => _btnStyle = value;
        }
        private GUIStyle _btnStyle;
        

        public Action OnDrawHeader;
        public Action<T, int> OnRemoveCallback;
        public Func<T, int, bool> AskBeforeDeletingCallback;

        public GridSelector(bool canDelete, ContentMaker<T> guicontent, int defaulindex = -1) : this(null,
            guicontent,
            defaulindex, canDelete)
        {
        }

        public GridSelector(bool canDelete, int defaulindex = -1) : this(null, null, defaulindex, canDelete)
        {
        }

        public GridSelector(IEnumerable<T> data = null, ContentMaker<T> guicontent = null,
                            bool canDelete = false) : this(data, guicontent, -1, canDelete)
        {
        }


        public GridSelector(IEnumerable<T> data, ContentMaker<T> guicontent, int index, bool candelete) : base(
            data,
            guicontent, index)
        {
            CanDelete  = candelete;
        }

        public bool CanDelete
        {
            get => _canDelete;
            set
            {
                _canDelete = value;
                if (value)
                {
                    ElementsDrawer     = DeleteElementDrawer;
                    ElementsDrawerRect = DefaultDrawerRect;
                }

                else
                {
                    ElementsDrawerRect = DefaultDrawerRect;
                    ElementsDrawer     = DefaultDrawer;
                }
            }
        }

      


        protected override int SelecttionGUI(int selectionIndex, GUIContent label, float labelwidth = 70)
        {
            //  var btnwidth = CalcBtnWidth();
            if (label != GUIContent.none)
                GUILayout.Label(label, GUILayout.Width(labelwidth));
            OnDrawHeader?.Invoke();
            selectionIndex = ElementsDrawer(selectionIndex);
            return selectionIndex;
        }

        protected override int SelecttionGUI(int index, Rect rect, GUIContent label = null)
        {
            if (label != null)
            {
                
            }

            return  ElementsDrawerRect(index, rect);
        }


        /// <summary>
        ///     Рисуем кнопки элементов
        /// </summary>
        /// <param name="selectionIndex"></param>
        /// <param name="names"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private int DefaultDrawer(int selectionIndex)
        {
            /*bool isOpen = false;
            
            float elemWidth = (fullWidth - num2) / _xcount;
            _tempOptions = new GUILayoutOption[]{GUILayout.Width(elemWidth)};
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
                    if (n == _xcount)
                    {
                        isOpen = false;
                        EditorGUILayout.EndHorizontal();
                        n = 0;
                    }
                }

                if (isOpen)
                {
                    EditorGUILayout.EndHorizontal();
                }
            }*/

            for (var i = 0; i < _valueBox.Count; i++)
                if (ItemDrawerCallback == null)
                {
                    if (GUILayout.Toggle(i == selectionIndex, _valueBox.NameContent[i], BtnStyle))
                        selectionIndex = i;
                }
                else
                {
                    if (ItemDrawerCallback.Invoke(_valueBox[i], i, _valueBox.NameContent[i], i == selectionIndex))
                        selectionIndex = i;
                }

            return selectionIndex;
        }
        
        private int DefaultDrawerRect(int selectionIndex, Rect fullRect)
        {
            float widthOne = fullRect.width / _valueBox.Count;
            
            for (var i = 0; i < _valueBox.Count; i++)
            {
                Rect rect = new Rect()
                {
                    size     = new Vector2(widthOne, fullRect.height),
                    position = new Vector2(fullRect.xMin + widthOne * i, fullRect.y)
                };    
                if (GUI.Toggle(rect,i == selectionIndex, _valueBox.NameContent[i], EditorStyles.miniButton))
                    selectionIndex = i;
            }

            return selectionIndex;
        }

        private bool DrawItem(int index, bool isSelected)
        {
            EditorGUI.BeginChangeCheck();
            if (ItemDrawerCallback == null)
                GUILayout.Toggle(isSelected, _valueBox.NameContent[index], EditorStyles.miniButton);
            else
                ItemDrawerCallback.Invoke(_valueBox[index], index, _valueBox.NameContent[index], isSelected);

            if (EditorGUI.EndChangeCheck())
                isSelected = !isSelected;
            return isSelected;
        }

        /// <summary>
        ///     рисуем кнопки с возможностью удаления
        /// </summary>
        /// <param name="selectionIndex"></param>
        /// <param name="names"></param>
        /// <param name="width"></param>
        private int DeleteElementDrawer(int selectionIndex)
        {
            var deletedIndex = -1;
            for (var i = 0; i < _valueBox.NameContent.Length; i++)
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("-", MinusBtnStyle, GUILayout.Width(MINUS_BTN_SIZE)))
                        deletedIndex = i;
                    if (ItemDrawerCallback == null)
                    {
                        if (GUILayout.Toggle(i == selectionIndex, _valueBox.NameContent[i], MinusBtnStyle))
                            selectionIndex = i;
                    }
                    else
                    {
                        if (ItemDrawerCallback.Invoke(_valueBox[i], i, _valueBox.NameContent[i],
                            i == selectionIndex))
                            selectionIndex = i;
                    }
                }

            if (deletedIndex > -1)
            {
                var res     = _valueBox[deletedIndex];
                if (AskBeforeDeletingCallback != null)
                {
                    if (!AskBeforeDeletingCallback.Invoke(res, deletedIndex)) return selectionIndex;
                }
    
                if (deletedIndex == selectionIndex)
                    selectionIndex = -1;
                else if (deletedIndex < selectionIndex)
                    selectionIndex--;
                
                var newList = new List<T>(_valueBox);
                newList.RemoveAt(deletedIndex);
                InitValues(newList);
                OnRemoveCallback?.Invoke(res, deletedIndex);
            }

            return selectionIndex;
        }


        protected override string DefaultGetName(T obj, int n)
        {
            return n + " " + obj;
        }


      
    }
}