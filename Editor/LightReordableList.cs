using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameKit.Editor
{
    /// <summary>
    ///     Спец. класс для установки нормального скина для всех реордэбл лист.
    /// </summary>
    public class LightReorderableList : ReorderableList
    {
        private static bool _isSet;

        public LightReorderableList(IList elements, Type elementType) : base(elements, elementType)
        {
            KostulInitDefaults();
        }

        public LightReorderableList(IList elements, Type elementType, bool draggable, bool displayHeader,
                                    bool displayAddButton, bool displayRemoveButton) : base(elements, elementType,
            draggable, displayHeader,
            displayAddButton, displayRemoveButton)
        {
            KostulInitDefaults();
        }

        public LightReorderableList(SerializedObject serializedObject, SerializedProperty elements) : base(
            serializedObject, elements)
        {
            KostulInitDefaults();
        }

        public LightReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable,
                                    bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(
            serializedObject, elements,
            draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            KostulInitDefaults();
        }

        private static void SetLigthSkin()
        {
            var skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            var defaults = new Defaults();

            DoAdd(defaults, "boxBackground", skin, "RL Background");
            DoAdd(defaults, "draggingHandle", skin, "RL DragHandle");
            DoAdd(defaults, "headerBackground", skin, "RL Header");
            DoAdd(defaults, "footerBackground", skin, "RL Footer");
            DoAdd(defaults, "preButton", skin, "RL FooterButton");
            DoAdd(defaults, "elementBackground", skin, "RL Element");

            var fieldInfo =
                typeof(ReorderableList).GetField("s_Defaults", BindingFlags.NonPublic | BindingFlags.Static);

            fieldInfo.SetValue(null, defaults);
        }

        private static void DoAdd(Defaults defaults, string boxbackground, GUISkin skin, string rlBackground)
        {
            var field = defaults.GetType().GetField(boxbackground);
            field.SetValue(defaults, skin.FindStyle(rlBackground));
        }

        private void KostulInitDefaults()
        {
            if (_isSet) return;
            try
            {
                if (GUI.skin == null) return;
                SetLigthSkin();
                _isSet = true;
            }
            catch
            {
            }
        }
    }
}