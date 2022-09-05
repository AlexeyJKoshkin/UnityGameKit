using System;
using System.Diagnostics;

namespace GameKit
{
    [Conditional("UNITY_EDITOR")]
    /// <summary>
    ///     Маркируем этим атрибутом енума, в редакторе  гуи подтянет отображение
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomEnumNameAttribute : Attribute
    {
        public string Name;
        public string Tooltip;

        public CustomEnumNameAttribute()
        {
            Name    = null;
            Tooltip = null;
        }

        public CustomEnumNameAttribute(string name)
        {
            Name    = name;
            Tooltip = null;
        }

        public CustomEnumNameAttribute(string name, string tooltip)
        {
            Name    = name;
            Tooltip = tooltip;
        }
    }
}