using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public abstract class EditorSections<T> : IEditorSections<T>
    {
        public delegate bool CheckBoolDrawer(bool canChangeValue, bool currentValue, T keyValue);

        protected readonly T[] TempKeys;

        /// <summary>
        ///     Функтор отрисовки проверки була
        /// </summary>
        public CheckBoolDrawer DrawCheck;

        protected EditorSections(T[] availableKeys)
        {
            TempKeys = availableKeys;
        }

        protected Dictionary<T, bool> ShowSections { get; } = new Dictionary<T, bool>();

        public virtual bool this[T section]
        {
            get => ShowSections[section];
            set => ShowSections[section] = value;
        }

        public void Revert(T key)
        {
            this[key] = !this[key];
        }

        public bool CheckFoldout(T section, bool isEnable = true, GUIStyle foldoutStyle = null)
        {
            foldoutStyle = foldoutStyle ?? EditorStyles.foldout;
            var res = DrawCheck?.Invoke(isEnable, this[section], section) ??
                      DrawDefaultCheck(section, isEnable, foldoutStyle);
            if (isEnable)
                this[section] = res;
            return isEnable && this[section];
        }

        public void ShowAll(bool isEnable, GUIStyle foldoutStyle)
        {
            foreach (var key in TempKeys)
                CheckFoldout(key, isEnable, foldoutStyle);
        }

        private bool DrawDefaultCheck(T section, bool isEnable, GUIStyle foldoutStyle)
        {
            EditorGUI.BeginDisabledGroup(!isEnable);
            bool result = EditorGUILayout.Foldout(this[section], "Show" + section, true, foldoutStyle);
            EditorGUI.EndDisabledGroup();
            return result;
        }
    }
}