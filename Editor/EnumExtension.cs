using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameKit.Editor
{
    public static class EnumExtension
    {
        public static IEnumerable<T> GetAllEnumElements<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<T> GetAllEnumElements<T>(params T[] except) where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Where(o => !except.Contains(o));
        }

        /// <summary>
        ///     Для Grida с енумами возвращает итоговый енам
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TEnum GetSelectedEnum<TEnum>(this IEnumerable<TEnum> selector) where TEnum : struct
        {
            var t = typeof(TEnum); // тип енума
            if (!t.HasAttribute<FlagsAttribute>()) throw new ArgumentException("В селекторе не Flags енам");
            var result = 0;

            foreach (var c in selector)
                result = result | (int) Enum.ToObject(t, c);
            return (TEnum) Enum.ToObject(typeof(TEnum), result);
        }

        public static string GetTooltipEnum<TEnum>(this TEnum ie) where TEnum : struct
        {
            var t = typeof(TEnum); // тип енума
            if (!t.IsEnum) return ie.ToString(); // если это не енам, то просто тустринг
            var memInfo = t.GetMember(ie.ToString());
            if (ie.ToString() == "Equals")
                Debug.LogError("Имя значения енума == Equals. Шарп не умеет искать у таких значений атрибуты :-0");
            if (memInfo.Length == 0) return ie.ToString();
            var attributes = memInfo[0].GetCustomAttributes(typeof(TooltipAttribute), false);
            if (attributes.Length == 0)
                return ie.ToString();
            return ((TooltipAttribute) attributes[0]).tooltip;
        }

        public static string GetEnumName<TEnum>(this TEnum ie) where TEnum : struct
        {
            var t = typeof(TEnum); // тип енума
            if (!t.IsEnum) return ie.ToString(); // если это не енам, то просто тустринг
            var memInfo = t.GetMember(ie.ToString());
            if (ie.ToString() == "Equals")
                Debug.LogError("Имя значения енума == Equals. Шарп не умеет искать у таких значений атрибуты :-0");
            if (memInfo.Length == 0) return ie.ToString();
            var attributes = memInfo[0].GetCustomAttributes(typeof(CustomEnumNameAttribute), false);
            if (attributes.Length == 0)
                return ie.ToString();
            string enumName = ((CustomEnumNameAttribute) attributes[0]).Name;
            return string.IsNullOrEmpty(enumName) ? ie.ToString() : enumName;
        }

        public static GUIContent GetContentForEnum(this Enum enumValue, Type enumType, bool revertTooltip)
        {
            var result = new GUIContent(enumValue.ToString(), "");
            // var t = enumType; // тип енума
            if (!enumType.IsEnum) return result; // если это не енам, то просто тустринг
            return FillGuiContetnWithEnum(enumValue, enumType, result, revertTooltip);
        }


        public static GUIContent GetContentForEnum<TEnum>(this TEnum ie, bool revertTooltip = false)
        {
            var result = new GUIContent(ie.ToString(), "");
            var t      = typeof(TEnum); // тип енума
            if (!t.IsEnum) return result; // если это не енам, то просто тустринг
            return FillGuiContetnWithEnum(ie as Enum, t, result, revertTooltip);
        }

        private static GUIContent FillGuiContetnWithEnum(Enum enumValue, Type enumType, GUIContent result,
                                                         bool revertTooltip)
        {
            var memInfo = enumType.GetMember(enumValue.ToString());
            if (memInfo.Length == 0) return result;

            var attributes = memInfo[0].GetCustomAttributes(typeof(CustomEnumNameAttribute), false);
            if (attributes.Length > 0)
            {
                var concreate = (CustomEnumNameAttribute) attributes[0];
                result.text    = concreate.Name;
                result.tooltip = concreate.Tooltip;
            }

            if (string.IsNullOrEmpty(result.tooltip))
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(TooltipAttribute), false);
                if (attrs.Length > 0)
                    result.tooltip = ((TooltipAttribute) attrs[0]).tooltip;
            }

            if (!string.IsNullOrEmpty(result.tooltip) && revertTooltip)
            {
                string name = result.tooltip;
                result.tooltip = result.text;
                result.text    = name;
            }

            if (string.IsNullOrEmpty(result.text))
                result.text = enumValue.ToString();
            return result;
        }
    }
}