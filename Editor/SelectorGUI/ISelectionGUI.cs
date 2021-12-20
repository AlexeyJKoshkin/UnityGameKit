using System.Collections.Generic;

namespace GameKit.Editor
{
    public interface ISelectionGUI<T> : IEnumerable<T>
    {
        bool IsSelected { get; }
        T CurrentValue { get; }

        int Count { get; }
    }
}