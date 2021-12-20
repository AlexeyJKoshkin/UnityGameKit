using System;

namespace GameKit.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EnumDataBoxAttribute : Attribute
    {
        public Type Type;
        public object[] Exclude;

        public EnumDataBoxAttribute(Type type, params object[] exclude)
        {
            Type = type;
            Exclude = exclude;
        }
    }
}