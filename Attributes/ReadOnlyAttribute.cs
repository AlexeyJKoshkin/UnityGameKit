using System.Diagnostics;
using UnityEngine;

namespace GameKit
{
    [Conditional("UNITY_EDITOR")]
    /// <summary>
    ///     Used to deny editing serializefield in Inspector
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class CenteredSpriteDrawerAttribute : PropertyAttribute
    {
        public Vector2 Size;

        public CenteredSpriteDrawerAttribute(float x, float y)
        {
            Size = new Vector2(x,y);
        }
    }
}