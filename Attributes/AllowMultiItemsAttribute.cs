using System;
using System.Diagnostics;

namespace GameKit
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class)]
    public class AllowMultiItemsAttribute : Attribute
    {
    }
}