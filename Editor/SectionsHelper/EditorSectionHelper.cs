using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace GameKit.Editor
{
    public class EditorSectionHelper<T> : EditorSections<T> where T : struct
    {
        /// <summary>
        ///     Стандартаный функтор для получения ключа по типу.
        ///     Вполне возможно, что для одного и того же типа T - будет использоватся в разных местах.
        ///     поэтому мы должны хранить его по разным местам
        /// </summary>
        public static Func<T, string> GetKey = key => key + "_section";

        public EditorSectionHelper(bool hasValue = false) : base(Enum.GetValues(typeof(T)).Cast<T>().ToArray())
        {
            LoadKeys(hasValue);
        }

        public EditorSectionHelper(Func<T, string> getKey = null) : base(Enum.GetValues(typeof(T)).Cast<T>().ToArray())
        {
            if (getKey != null)
                GetKey = getKey;

            LoadKeys(false);
        }

        public override bool this[T section]
        {
            get => base[section];
            set
            {
                base[section] = value;
                if (GetKey == null) return;
                EditorPrefs.SetBool(GetKey(section), value);
            }
        }

        public float CountKeys => TempKeys.Length;

        public static IEnumerable<KeyValuePair<T, bool>> LoadKeys(Func<T, string> getter = null)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"T is not Enum T is {typeof(T).Name}");
            var keyGetter = getter ?? GetKey;
            foreach (T key in Enum.GetValues(typeof(T)).Cast<T>().ToArray())
                yield return new KeyValuePair<T, bool>(key, EditorPrefs.GetBool(keyGetter(key)));
        }

        private void LoadKeys(bool hasValue)
        {
            foreach (T section in TempKeys)
                ShowSections.Add(section, GetKey != null && EditorPrefs.GetBool(GetKey(section), hasValue));
        }
    }
}