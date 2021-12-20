using System;

namespace GameKit.Toolbar
{
    /// <summary>
    ///     Этим атрибутом помечаем класс который автоматом подхватитсья в тулбар
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolbarMethodRepositoryAttribute : Attribute
    {
        public string CustomName;

        public ToolbarMethodRepositoryAttribute(string CustomName)
        {
            this.CustomName = CustomName;
        }
    }
}