using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameKit {
    public abstract class BaseGameEditorInstaller : ScriptableObject, IInstaller
    {
        public abstract void Install(IContainerBuilder diContainer);
    }
}