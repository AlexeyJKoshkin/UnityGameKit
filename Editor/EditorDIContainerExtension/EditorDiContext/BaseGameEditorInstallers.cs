using UnityEngine;

namespace GameKit.EditorContext {
    public abstract class BaseGameEditorInstallers : ScriptableObject
    {
        public abstract void InstallBindings(ContainerBuilder diContainer);
    }
}