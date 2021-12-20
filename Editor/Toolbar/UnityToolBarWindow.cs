using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameKit.Toolbar
{
    /// <summary>
    ///     Todo: дома допилить перерисовку меню  только в конце колбэка SceneView. иногда выскакивает ошибка EndLayoutGroup:
    ///     BeginLayoutGroup must be called first.
    /// </summary>
    [InitializeOnLoad]
    public static partial class UnityToolBarSceneView
    {
        private static ToolbarGUI _gui;
        private static readonly IObjectWithMethodsFactory _factory;

        static UnityToolBarSceneView()
        {
            _factory = new ObjectWithMethodsFactory();
        }

        public static bool IsOpen => _gui != null;

        public static void Hide()
        {
            _gui?.Close();
            _gui = null;
        }

        /// <summary>
        ///     Показать меню для инстанса класса
        /// </summary>
        /// <param name="instanceSource"></param>
        public static void Show(object[] instanceSource, string title)
        {
            var guiInfo = new List<IObjectWrapperGUI>();
            foreach (var instance in instanceSource) guiInfo.Add(_factory.CreateMethodRepository(instance));
            Show(guiInfo, title);
        }

        /// <summary>
        ///     Показать меню для класса, инстанс создаться через new()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Show<T>(string title) where T : new()
        {
            Show(new[] {typeof(T)}, title);
        }

        public static void Show(Type[] type, string title)
        {
            var guiInfo = new List<IObjectWrapperGUI>();
            foreach (var t in type)
            {
                var c = t.GetConstructor(Type.EmptyTypes);
                if (c != null && c.IsPublic && !t.IsAbstract)
                {
                    //You can create instance
                    var instance = Activator.CreateInstance(t);
                    var repa     = _factory.CreateMethodRepository(instance);
                    guiInfo.Add(repa);
                }
                else
                {
                    var repa = _factory.CreateMethodRepository(t);
                    guiInfo.Add(repa);
                }
            }

            Show(guiInfo, title);
        }

        private static void Show(List<IObjectWrapperGUI> data, string title)
        {
            if (_gui != null)
            {
                Debug.LogError("Menu is Active. Close current menu and try again");
                return;
            }

            _gui = new ToolbarGUI();
            _gui.Show(data, title);
        }
    }
}