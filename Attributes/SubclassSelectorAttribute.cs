using System;
using UnityEngine;

namespace GameKit
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SubclassSelectorAttribute : PropertyAttribute
    {
        Type _type;
        public SubclassSelectorAttribute(System.Type type)
        {
            _type = type;
        }
        public Type GetFieldType()
        {
            return _type;
        }
    }
}