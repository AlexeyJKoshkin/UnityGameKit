using UnityEngine;
using VContainer;

namespace GameKit.EditorContext {
    public abstract class BaseGameEditorInstallers : ScriptableObject,IInstaller
    {
        public abstract void Install(IContainerBuilder diContainer);
    }
}
