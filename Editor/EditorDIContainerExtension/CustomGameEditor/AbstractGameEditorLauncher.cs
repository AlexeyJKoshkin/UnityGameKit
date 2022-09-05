using GameKit.EditorContext;
using UnityEngine;
using VContainer;

namespace GameKit.CustomGameEditor
{
    public abstract class AbstractGameEditorLauncher : ScriptableObject
    {
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

        protected virtual void Binding(ContainerBuilder diContainer)
        {
            _installers.ForEach(e => e.InstallBindings(diContainer));
        }

        protected abstract void LunchEditor(IObjectResolver diContainer);

        protected virtual void PreBinding(ContainerBuilder diContainer) { }

        protected virtual void PostBinding(ContainerBuilder diContainer) { }

        protected virtual void PreLunch() { }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button, Sirenix.OdinInspector.ShowIf("IsWork")]
#endif
        public abstract void Stop();
    }
}