using GameKit.CustomGameEditor;
using UnityEngine;

namespace GameKit.EditorContext
{
    public abstract class GameEditorLauncher<TIWrapper> : AbstractGameEditorLauncher where TIWrapper : IDIWrapper, new()
    {
        [SerializeField]
        private BaseGameEditorInstallers<TIWrapper>[] _installers;
        
        public sealed override void Lunch()
        {
            TIWrapper diContainer = new TIWrapper();
            PreBinding(diContainer);
            Binding(diContainer);
            PostBinding(diContainer);
            diContainer.ResolveRoots();
            PreLunch();
            LunchEditor(diContainer);
            diContainer.FlushBindings();
        }

        protected virtual void Binding(TIWrapper diContainer)
        {
            _installers.ForEach(e=> e.InstallBindings(diContainer));
        }

        protected abstract void LunchEditor(TIWrapper diContainer);

        protected virtual void PreBinding(TIWrapper diContainer) { }

        protected virtual void PostBinding(TIWrapper diContainer) { }

        protected virtual void PreLunch() { }
    }

    public abstract class GameEditorLauncher<T, TIWrapper> : GameEditorLauncher<TIWrapper>
        where T : class, ICustomGameEditor where TIWrapper : IDIWrapper, new()
    {
        public override bool IsWork => CustomGameEditor != null;
        protected T CustomGameEditor;

        protected override void LunchEditor(TIWrapper diContainer)
        {
            CustomGameEditor = diContainer.Resolve<T>();
            CustomGameEditor.OnFinishWorkingEvent += () => CustomGameEditor = null;
            CustomGameEditor.StartWork();
        }

        protected override void PreBinding(TIWrapper diContainer)
        {
            base.PreBinding(diContainer);
            diContainer.BindAsSingle<T>();
        }

        public override void Stop()
        {
            CustomGameEditor?.StopWork();
            CustomGameEditor = null;
        }
    }

    public abstract class BaseGameEditorInstallers<TIWrapper> : ScriptableObject where TIWrapper : IDIWrapper
    {
        public virtual void InstallBindings(TIWrapper diContainer)
        {
            
        }
    }
}
