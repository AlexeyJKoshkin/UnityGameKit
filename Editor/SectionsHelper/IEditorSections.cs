using UnityEngine;

namespace GameKit.Editor
{
    public interface IEditorSections<in T>
    {
        bool this[T key] { get; set; }
        bool CheckFoldout(T section, bool isEnable, GUIStyle foldoutStyle = null);

        void ShowAll(bool isEnable, GUIStyle foldoutStyle);
        void Revert(T playAnimation);
    }
}