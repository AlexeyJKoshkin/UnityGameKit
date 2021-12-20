using System.Collections.Generic;
using System.Linq;
using GameKit.CustomGameEditor;
using UnityEngine;
using Zenject;

namespace GameKit.EditorContext
{
    public abstract class GameEditorLauncher : AbstractGameEditorLauncher
    {
        [SerializeField] private List<BaseCustomEditorInstaller> _editorInstallers;
        
        public sealed override void Lunch()
        {
            DiContainer diContainer = new DiContainer();
            PreBinding();
            Binding(diContainer);
            PostBinding();
            PreLunch();
            LunchEditor(diContainer);
            diContainer.FlushBindings();
        }
        
        private void Binding(DiContainer diContainer)
        {
            foreach (var installer in _editorInstallers.Where(o => o != null))
            {
                installer.Initialize(diContainer);
                installer.InstallBindings();
            }
        }

        protected abstract void LunchEditor(DiContainer diContainer);
        
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

    public abstract class GameEditorLauncher<T> : GameEditorLauncher where T : class, ICustomGameEditor
    {
        public override bool IsWork => CustomGameEditor != null;
        protected T CustomGameEditor;

        protected override void LunchEditor(DiContainer diContainer)
        {
            diContainer.Bind<T>().AsSingle();
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