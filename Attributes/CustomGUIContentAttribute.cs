using System;

namespace GameKit
{
    /// <summary>
    ///     Маркируем эьим атрибутов класс, чтобы в редакторе при выборе типа печатолось читаемое имя
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomGUIContentAttribute : Attribute
    {
        public Enum Enum;

        public Type EnumType;
        public string Name;
        public string Tooltip;

        public CustomGUIContentAttribute(string name)
        {
            Name    = name;
            Tooltip = null;
        }

        public CustomGUIContentAttribute(object eEnum)
        {
            if (eEnum is Enum)
            {
                Enum     = eEnum as Enum;
                EnumType = eEnum.GetType();
            }
            else
            {
                Name    = eEnum.ToString();
                Tooltip = null;
            }
        }

        public CustomGUIContentAttribute(string name, string tooltip)
        {
            Name    = name;
            Tooltip = tooltip;
        }
    }
}