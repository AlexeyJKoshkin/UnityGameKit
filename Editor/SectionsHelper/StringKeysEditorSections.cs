using System;
using UnityEditor;

namespace GameKit.Editor
{
    public class StringKeysEditorSections : EditorSections<string>
    {
        /// <summary>
        ///     Стандартаный функтор для получения ключа по типу.
        ///     Вполне возможно, что для одного и того же типа T - будет использоватся в разных местах.
        ///     поэтому мы должны хранить его по разным местам
        /// </summary>
        public Func<string, string> GetKey = key => key + "_section";

        public StringKeysEditorSections(string[] keys, bool hasValue) : base(keys)
        {
            foreach (var key in keys)
            {
                bool show = GetKey != null && EditorPrefs.GetBool(GetKey(key), hasValue);
                ShowSections.Add(key, show);
            }
        }
    }
}