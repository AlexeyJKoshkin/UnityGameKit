using System;

namespace GameKit.Toolbar
{
    /// <summary>
    ///     Помечаем этим атрибутом тултипы для кнопок методов
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AddionalValidayionAttribute : Attribute
    {
        public string AvalaibleName;
        public string NotAvalaibleName;

        public AddionalValidayionAttribute(string notAvalaibleName, string avalablename)
        {
            NotAvalaibleName = notAvalaibleName;

            AvalaibleName = avalablename;
        }
    }
}