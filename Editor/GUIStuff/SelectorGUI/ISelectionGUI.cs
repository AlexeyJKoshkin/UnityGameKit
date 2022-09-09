using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Editor
{
    public interface ISelectionGUI<T> : IEnumerable<T>
    {
        bool IsSelected { get; }
        T CurrentValue { get; }

        int Count { get; }
        IEnumerable<(T item, GUIContent content)> GetItemsWithContent();
        void SetCurrent(T selected);
    }
}