using System;

namespace GameKit.Toolbar
{
    /// <summary>
    ///     Помечаем этим атрибутом тултипы для кнопок методов
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ToolBarTooltipAttribute : Attribute
    {
        public string IconPath;
        public string Tooltip;

        public ToolBarTooltipAttribute(string tooltoip)
        {
            Tooltip = tooltoip;
        }

        public ToolBarTooltipAttribute(string tooltoip, string ioconPath)
        {
            Tooltip  = tooltoip;
            IconPath = ioconPath;
        }
    }
}