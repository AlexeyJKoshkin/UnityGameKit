using GameKit.CustomGameEditor;

namespace GameKit.EditorContext
{
    public abstract class GameEditorLauncher<TIWrapper> : AbstractGameEditorLauncher where TIWrapper : IDIWrapper,new()
    {
        public sealed override void Lunch()
        {
            TIWrapper diContainer = new TIWrapper();
            PreBinding();
            Binding(diContainer);
            PostBinding();
            PreLunch();
            LunchEditor(diContainer);
            diContainer.FlushBindings();
        }

        protected abstract void Binding(TIWrapper diContainer);

        protected abstract void LunchEditor(TIWrapper diContainer);
        
        protected virtual void PreBinding()
        {
        }

        protected virtual void PostBinding()
        {
        }

        protected virtual void PreLunch()
        {
        }
    }

    public abstract class GameEditorLauncher<T,TIWrapper> : GameEditorLauncher<TIWrapper> where T : class, ICustomGameEditor  where TIWrapper : IDIWrapper,new()
    {
        public override bool IsWork => CustomGameEditor != null;
        protected T CustomGameEditor;

        protected override void LunchEditor(TIWrapper diContainer)
        {
            diContainer.BindAsSingle<T>();
            diContainer.ResolveRoots();
            CustomGameEditor                      =  diContainer.Resolve<T>();
            CustomGameEditor.OnFinishWorkingEvent += () => CustomGameEditor = null;
            CustomGameEditor.StartWork();
        }

        public override void Stop()
        {
            CustomGameEditor?.StopWork();
            CustomGameEditor = null;
        }
    }
}