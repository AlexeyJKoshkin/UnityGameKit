using System;

namespace GameKit.Toolbar
{
    /**
     * Use this PropertyAttribute to add the command method to a scene view toolbar as a button.
     */
    [AttributeUsage(AttributeTargets.Method)]
    public class ToolbarMethodDataAttribute : Attribute
    {
        public string NameOfGroup;
        public string NameOfValidationMethod;

        public int SortingOrder;

        public ToolbarMethodDataAttribute(object group, string NameOfValidationMethod = null, int sorting = 0)
        {
            NameOfGroup                 = group.ToString();
            this.NameOfValidationMethod = NameOfValidationMethod;
            SortingOrder                = sorting;
        }

        public ToolbarMethodDataAttribute(object group, int sorting)
        {
            NameOfGroup            = group.ToString();
            NameOfValidationMethod = null;
            SortingOrder           = sorting;
        }

        public ToolbarMethodDataAttribute(string NameOfGroup = "", int sorting = 0)
        {
            this.NameOfGroup       = NameOfGroup;
            NameOfValidationMethod = null;
            SortingOrder           = sorting;
        }
        // method button. public string NameOfMethod { get; set; }

        // Tool bar consists of several columns. This parameter specifies what column to put the
    }
}