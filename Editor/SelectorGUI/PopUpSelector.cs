using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public class PopUpSelector<T> : SelectionGUI<T>
    {
        public string EmtyName = "SELECT";

        public PopUpSelector(int defaultIndex, ContentMaker<T> guicontentHelper = null) : base(null,
            guicontentHelper,
            defaultIndex)
        {
        }

        public PopUpSelector(params T[] values) : base(values, null, 0)
        {
        }

        public PopUpSelector(IEnumerable<T> data = null,
                             int defaultIndex = -1, ContentMaker<T> guicontentHelper = null) : base(data,
            guicontentHelper,
            defaultIndex)
        {
        }

        public PopUpSelector(IEnumerable<T> data, ContentMaker<T> guicontentHelper,
                             int defaultIndex = -1) : base(data, guicontentHelper, defaultIndex)
        {
        }


        protected override int SelecttionGUI(int index, GUIContent label, float labelwidth = 70)
        {
            //  var btnwidth = CalcBtnWidth();

            var names      = _valueBox.NameContent;
            var needSelect = index == -1;
            if (needSelect)
            {
                index = 0;
                ArrayUtility.Insert(ref names, 0, new GUIContent(EmtyName));
            }

            GUILayout.BeginHorizontal();

            if (label != GUIContent.none)
                GUILayout.Label(label, GUILayout.Width(labelwidth));
            var res = EditorGUILayout.Popup(index, names, GUILayout.ExpandHeight(Expand));
            if (needSelect)
                if (res == 0)
                    res = -1;
                else
                    res--;

            GUILayout.EndHorizontal();
            return res;
        }

        protected override int SelecttionGUI(int index, Rect rect, GUIContent label)
        {
            var names      = _valueBox.NameContent;
            var needSelect = index == -1;
            if (needSelect)
            {
                index = 0;
                ArrayUtility.Insert(ref names, 0, new GUIContent(EmtyName));
            }

            var res = EditorGUI.Popup(rect, new GUIContent(label), index, names, EditorStyles.popup);
            if (needSelect)
                if (res == 0)
                    res = -1;
                else
                    res--;
            return res;
        }

        protected override string DefaultGetName(T obj, int n)
        {
            return obj.ToString();
        }
    }
}