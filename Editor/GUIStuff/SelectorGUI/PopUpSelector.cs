using System.Collections.Generic;
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
                             int defaultIndex = -1,
                             ContentMaker<T> guicontentHelper = null) : base(data,
                                                                             guicontentHelper,
                                                                             defaultIndex) { }

        public PopUpSelector(IEnumerable<T> data,
                             ContentMaker<T> guicontentHelper,
                             int defaultIndex = -1) : base(data, guicontentHelper, defaultIndex) { }
#if ODIN_INSPECTOR

        private int _odinDirty = -1;

        protected override int DrawSelectionGUI(int index, GUIContent label, float labelwidth = 70)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (label != GUIContent.none) GUILayout.Label(label, GUILayout.Width(labelwidth));

                if (GUILayout.Button(_valueBox.GetContent(CurrentValue), EditorStyles.popup))
                {
                    var genericSelector = new GenericSelector<T>(this);
                    var pos = Event.current.mousePosition;
                    Sirenix.OdinInspector.Editor.OdinEditorWindow odinEditorWindow = genericSelector.ShowInPopup(pos);

                    odinEditorWindow.OnClose += () =>
                    {
                        var item = (T) genericSelector.SelectionTree.Selection.SelectedValue;
                        _odinDirty = _valueBox.IndexOf(item);
                        this.SetCurrentByIndex(_odinDirty);
                    };
                    odinEditorWindow.OnClose += genericSelector.SelectionTree.Selection.ConfirmSelection;
                    if (Application.platform == RuntimePlatform.LinuxEditor) GUIUtility.ExitGUI();
                }
            }

            if (_odinDirty > -1)
            {
                index = _odinDirty;
                _odinDirty = -1;
            }

            return index;
        }

#else
        protected override int SelecttionGUI(int index, GUIContent label, float labelwidth = 70)
        {
            var info = PrepareValues(index);

            using (new GUILayout.HorizontalScope())
            {
                if (label != GUIContent.none) GUILayout.Label(label, GUILayout.Width(labelwidth));
                var res = EditorGUILayout.Popup(index, info.names, GUILayout.ExpandHeight(Expand));
                return PostCheck(info.hasEmpty, res);
            }
        }

#endif

        protected override int DrawSelectionGUI(int index, Rect rect, GUIContent label)
        {
            var info = PrepareValues(index);
            var res = EditorGUI.Popup(rect, new GUIContent(label), index, info.names, EditorStyles.popup);
            return PostCheck(info.hasEmpty, res);
        }

        (GUIContent[] names, int index, bool hasEmpty) PrepareValues(int index)
        {
            var names = _valueBox.NameContent;
            var hasEmpty = index == -1;

            if (hasEmpty)
            {
                index = 0;
                ArrayUtility.Insert(ref names, 0, new GUIContent(EmptyName));
            }

            return (names, index, hasEmpty);
        }

        int PostCheck(bool hasEmpty, int resultIndex)
        {
            return hasEmpty
                ? resultIndex == 0
                    ? -1
                    : resultIndex - 1
                : resultIndex;
        }

        protected override string DefaultGetName(T obj, int n)
        {
            return obj.ToString();
        }
    }
}
