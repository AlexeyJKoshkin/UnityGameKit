using GameKit.EditorContext;
using UnityEngine;
using VContainer;

namespace GameKit.CustomGameEditor
{
    public abstract class AbstractGameEditorLauncher : ScriptableObject
    {
        [ReadOnly]
        public string Description;
        public abstract bool IsWork { get; }
        public virtual string EditorName => name;
        [SerializeField] private BaseGameEditorInstallers[] _installers;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button, Sirenix.OdinInspector.HideIf("IsWork")]
#endif
        public void Lunch()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            PreBinding(containerBuilder);
            Binding(containerBuilder);
            PostBinding(containerBuilder);
            var resolver = containerBuilder.Build();
            PreLunch();
            LunchEditor(resolver);
        }

        protected virtual void Binding(IContainerBuilder diContainer)
        {
            _installers.ForEach(e => e.Install(diContainer));
        }

        protected abstract void LunchEditor(IObjectResolver diContainer);

        protected virtual void PreBinding(IContainerBuilder diContainer) { }

        protected virtual void PostBinding(IContainerBuilder diContainer) { }

        protected virtual void PreLunch() { }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button, Sirenix.OdinInspector.ShowIf("IsWork")]
#endif
        public abstract void Stop();
    }
}
