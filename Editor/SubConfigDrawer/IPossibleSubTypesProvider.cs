using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Editor
{
    public interface ISubScriptableFactory
    {
        void DrawCreateNewElementMenu(IEnumerable<ScriptableObject> currentItems,Action<ScriptableObject> onAddNewElement);
    }
}