using UnityEngine;

namespace GameKit.Editor
{
    public interface IGUIContentHelper<in T>
    {
        GUIContent GetContentFor(T value, int index = -1);
    }

    public delegate GUIContent ContentMaker<in T>(T value, int index = -1);
}