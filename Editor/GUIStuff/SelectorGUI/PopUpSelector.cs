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
        
        protected override int SelecttionGUI(int index, GUIContent label, float labelwidth = 70)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (label != GUIContent.none)
                    GUILayout.Label(label, GUILayout.Width(labelwidth));
                if (GUILayout.Button(_valueBox.GetContent(CurrentValue), EditorStyles.popup))
                {
                   
                    var              customOdinDrawer = new GenericSelector<T>(this);
                    var              pos              = Event.current.mousePosition;
                    OdinEditorWindow odinEditorWindow = customOdinDrawer.ShowInPopup(pos);

                    {
                        odinEditorWindow.OnClose += () =>
                                                    {
                                                        var item = (T) customOdinDrawer.SelectionTree.Selection.SelectedValue;
                                                        _odinDirty = _valueBox.IndexOf(item); 
                                                        SetCurrentByIndex(_odinDirty);
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
        protected override int SelecttionGUI(int index, GUIContent label, float labelwidth = 70)
        {
            var selectionData = GetSelectionData(index);

            GUILayout.BeginHorizontal();

            if (label != GUIContent.none)
                GUILayout.Label(label, GUILayout.Width(labelwidth));
            var res = EditorGUILayout.Popup(index, selectionData.names, GUILayout.ExpandHeight(Expand));
            GUILayout.EndHorizontal();
            return PostCheck(selectionData.hasEmpty, res);
        }
#endif

        protected override int SelecttionGUI(int index, Rect rect, GUIContent label)
        {
            var selectionData = GetSelectionData(index);
            var res = EditorGUI.Popup(rect, new GUIContent(label), index, selectionData.names, EditorStyles.popup);
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