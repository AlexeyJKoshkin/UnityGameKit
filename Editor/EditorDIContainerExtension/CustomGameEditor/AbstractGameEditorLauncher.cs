
using UnityEngine;

namespace GameKit.CustomGameEditor
{
    public abstract class AbstractGameEditorLauncher : ScriptableObject
    {
        public string Description;
        public abstract bool IsWork { get; }
        public virtual string EditorName => name;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button, Sirenix.OdinInspector.HideIf("IsWork")]
#endif
        public abstract void Lunch();
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button, Sirenix.OdinInspector.ShowIf("IsWork")]
#endif
        public abstract void Stop();
    }
}