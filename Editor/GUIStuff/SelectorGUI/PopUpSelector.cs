using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public class PopUpSelector<T> : SelectionGUI<T>
    {
        public string EmptyName = "SELECT";

        public PopUpSelector(int defaultIndex, ContentMaker<T> guicontentHelper = null) : base(null,
                                                                                               guicontentHelper,
                                                                                               defaultIndex) { }

        public PopUpSelector(params T[] values) : base(values, null, 0) { }

        public PopUpSelector(IEnumerable<T> data = null,
                             int defaultIndex = -1, ContentMaker<T> guicontentHelper = null) : base(data,
                                                                                                    guicontentHelper,
                                                                                                    defaultIndex) { }

        public PopUpSelector(IEnumerable<T> data, ContentMaker<T> guicontentHelper,
                             int defaultIndex = -1) : base(data, guicontentHelper, defaultIndex) { }

#if ODIN_INSPECTOR
        private int _odinDirty = -1;
        
        protected override int DrawSelectionGUI(int index, GUIContent label)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (label != GUIContent.none)
                    GUILayout.Label(label);
                if (GUILayout.Button(_valueBox.GetContent(CurrentValue), EditorStyles.popup))
                {
                   
                    var              customOdinDrawer = new GenericSelector<T>(this);
                    var              pos              = Event.current.mousePosition;
                    OdinEditorWindow odinEditorWindow = customOdinDrawer.ShowInPopup(pos);

                    {
                        odinEditorWindow.OnClose += () =>
                                                    {
                                                        if (customOdinDrawer?.SelectionTree?.Selection?.SelectedValue is T myItem)
                                                        {
                                                            _odinDirty = _valueBox.IndexOf(myItem); 
                                                            SetCurrentByIndex(_odinDirty);    
                                                        }
                                                    };
                    }

                    odinEditorWindow.OnClose += customOdinDrawer.SelectionTree.Selection.ConfirmSelection;
                    if (Application.platform == RuntimePlatform.LinuxEditor) GUIUtility.ExitGUI();
                }
            }

            if (_odinDirty > -1)
            {
                index      = _odinDirty;
                _odinDirty = -1;
            }

            return index;
        }
#else
        protected override int DrawSelectionGUI(int index, GUIContent label)
        {
            var selectionData = GetSelectionData(index);

            GUILayout.BeginHorizontal();

            if (label != GUIContent.none)
                GUILayout.Label(label);
            var res = EditorGUILayout.Popup(index, selectionData.names, GUILayout.ExpandHeight(Expand));
            GUILayout.EndHorizontal();
            return PostCheck(selectionData.hasEmpty, res);
        }
#endif

        protected override int DrawSelectionGUI(int index, Rect rect, GUIContent label = null)
        {
            var selectionData = GetSelectionData(index);
            label = label == null ? GUIContent.none : label;
            var res = EditorGUI.Popup(rect, label, index, selectionData.names, EditorStyles.popup);
            return PostCheck(selectionData.hasEmpty, res);
        }

        private int PostCheck(bool hasEmpty, int res)
        {
            return hasEmpty  //если надо добавить пустой объект
                ? (res == 0) // и выбран пустой . т.к. выбран 0 (самый первый)
                    ? -1     // то должны вернуть -1, чтобы в следущий раз опять добавить пустоту
                    : res - 1 // если выбран не пустой, то вернем res--, т.к. 0 элемент фейковый
                : res;
        }

        private (GUIContent[] names, int index, bool hasEmpty) GetSelectionData(int index)
        {
            var names      = _valueBox.NameContent;
            var needSelect = index == -1;
            if (needSelect)
            {
                index = 0;
                ArrayUtility.Insert(ref names, 0, new GUIContent(EmptyName));
            }

            return (names, index, needSelect);
        }

        protected override string DefaultGetName(T obj, int n)
        {
            return obj.ToString();
        }
    }
}