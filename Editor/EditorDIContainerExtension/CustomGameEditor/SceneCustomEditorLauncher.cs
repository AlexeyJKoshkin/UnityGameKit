using System;
using System.Collections.Generic;
using GameKit.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameKit.CustomGameEditor
{
    public class SceneCustomEditorLauncher : ScriptableObject
    {
        public List<SceneEditorBinding> Bindings = new List<SceneEditorBinding>();

        [SerializeField]
        private bool _isStartOnRecompile;
        
        [Serializable]
        public struct SceneEditorBinding
        {
            public SceneAsset SceneAsset;
            public AbstractGameEditorLauncher Launcher;
        }

        [SerializeField, ReadOnly]
        private AbstractGameEditorLauncher _сurrentLuncher;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
        #endif
        void Start()
        {
            EditorSceneManager.sceneOpened   += SceneOpenHandler;
            EditorSceneManager.sceneOpening  += SceneOpeningHandler;
            EditorSceneManager.sceneClosing  += SceneClosingHandler;
        }

        [InitializeOnLoadMethod]
        static void InitOnLoad()
        {
            var instance = EditorUtils.FindAsset<SceneCustomEditorLauncher>();
            if(instance == null) return;
            if(!instance._isStartOnRecompile) return;
            instance.Start();
        }

        private void SceneClosingHandler(Scene scene, bool removingscene)
        {
            _сurrentLuncher?.Stop();
        }

        private void SceneOpeningHandler(string path, OpenSceneMode mode)
        {
            _сurrentLuncher?.Stop();
        }

        private void SceneOpenHandler(Scene scene, OpenSceneMode mode)
        {
            var binding = Bindings.Find(o => AssetDatabase.GetAssetPath(o.SceneAsset) == scene.path);
            if(binding.Launcher == null) return;
            if (EditorUtility.DisplayDialog("Open custom Editor Scene",
                $"Запустить редактор {binding.Launcher.EditorName}?", "Да", "Нет"))
            {
                _сurrentLuncher = binding.Launcher;
                _сurrentLuncher?.Lunch();
            }
        }
    }
}