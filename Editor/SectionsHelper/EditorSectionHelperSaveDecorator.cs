using System;
using UnityEngine;

namespace GameKit.Editor
{
    public class EditorSectionHelperSaveDecorator<T> : IEditorSections<T>
    {
        private EditorSectionHelperSaveDecorator(IEditorSections<T> sections)
        {
        }

        public bool this[T key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool CheckFoldout(T key, bool isEnable, GUIStyle foldoutStyle = null)
        {
            throw new NotImplementedException();
        }

        public void ShowAll(bool isEnable, GUIStyle foldoutStyle)
        {
            throw new NotImplementedException();
        }

        public void Revert(T key)
        {
            throw new NotImplementedException();
        }
    }
}