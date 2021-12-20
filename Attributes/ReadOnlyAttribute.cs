using UnityEngine;

namespace GameKit
{
    /// <summary>
    ///     Used to deny editing serializefield in Inspector
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

    public class CenteredSpriteDrawerAttribute : PropertyAttribute
    {
        public Vector2 Size;

        public CenteredSpriteDrawerAttribute(float x, float y)
        {
            Size = new Vector2(x,y);
        }
    }
}