using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameKit.EditorContext {
    public abstract class BaseGameEditorInstallers : ScriptableObject,IInstaller
    {
        public abstract void Install(IContainerBuilder diContainer);
    }
}
