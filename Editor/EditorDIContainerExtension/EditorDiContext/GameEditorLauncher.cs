using VContainer;

namespace GameKit.CustomGameEditor
{
    public abstract class GameEditorLauncher<T> : AbstractGameEditorLauncher
        where T : class, ICustomGameEditor 
    {
        public override bool IsWork => CustomGameEditor != null;
        protected T CustomGameEditor;

        protected override void LunchEditor(IObjectResolver diContainer)
        {
            CustomGameEditor                      =  diContainer.Resolve<T>();
            CustomGameEditor.OnFinishWorkingEvent += () => CustomGameEditor = null;
            CustomGameEditor.StartWork();
        }

        protected override void PreBinding(IContainerBuilder diContainer)
        {
            base.PreBinding(diContainer);
            diContainer.Register<T>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        public override void Stop()
        {
            CustomGameEditor?.StopWork();
            CustomGameEditor = null;
        }
    }
}