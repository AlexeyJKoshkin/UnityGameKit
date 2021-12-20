using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Editor
{
    public interface IObjectSelectionInfrastructure<T> : IEnumerable<T> where T : Object
    {
        void SelectObject(T selectionObject);

        string GetObjectName(T objectCandidate);
    }
}