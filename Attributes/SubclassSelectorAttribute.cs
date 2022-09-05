using System;
using System.Diagnostics;
using UnityEngine;

namespace GameKit
{
    [Conditional("UNITY_EDITOR")]
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