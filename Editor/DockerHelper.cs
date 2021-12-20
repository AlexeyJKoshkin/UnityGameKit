#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameKit
{
    /*/// <summary>
    ///     Честно спижженно отсюда. https://gist.github.com/Thundernerd/5085ec29819b2960f5ff2ee32ad57cbb#file-docker-cs
    ///     позволяет упростить работу с расположением окон относительно друг друга
    /// </summary>
    public static class Docker
    {
        public enum DockPosition
        {
            Left,
            Top,
            Right,
            Bottom
        }

        /// <summary>
        ///     Docks the second window to the first window at the given position
        /// </summary>
        public static void DockTo(this EditorWindow wnd, EditorWindow other, DockPosition position)
        {
            var mousePosition = GetFakeMousePosition(wnd, position);

            var parent          = new _EditorWindow(wnd);
            var child           = new _EditorWindow(other);
            var dockArea        = new _DockArea(parent.m_Parent);
            var containerWindow = new _ContainerWindow(dockArea.window);
            var splitView       = new _SplitView(containerWindow.rootSplitView);
            var dropInfo        = splitView.DragOver(other, mousePosition);
            if(dropInfo == null) return;
            dockArea.s_OriginalDragSource = child.m_Parent;
            splitView.PerformDrop(other, dropInfo, mousePosition);
        }

        private static Vector2 GetFakeMousePosition(EditorWindow wnd, DockPosition position)
        {
            Vector2 mousePosition = Vector2.zero;

            float magic = 80;

            // The 20 is required to make the docking work.
            // Smaller values might not work when faking the mouse position.
            switch (position)
            {
                case DockPosition.Left:
                    mousePosition = new Vector2(magic, wnd.position.size.y / 2);
                    break;
                case DockPosition.Top:
                    mousePosition = new Vector2(wnd.position.size.x / 2, magic);
                    break;
                case DockPosition.Right:
                    mousePosition = new Vector2(wnd.position.size.x - magic, wnd.position.size.y / 2);
                    break;
                case DockPosition.Bottom:
                    mousePosition = new Vector2(wnd.position.size.x / 2, wnd.position.size.y - magic);
                    break;
            }

            return new Vector2(wnd.position.x + mousePosition.x, wnd.position.y + mousePosition.y);
        }

#region Reflection Types

        private class _EditorWindow
        {
            private EditorWindow instance;
            private Type type;

            public _EditorWindow(EditorWindow instance)
            {
                this.instance = instance;
                type          = instance.GetType();
            }

            public object m_Parent
            {
                get
                {
                    var field = type.GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
                    return field.GetValue(instance);
                }
            }
        }

        private class _DockArea
        {
            private object instance;
            private Type type;

            public _DockArea(object instance)
            {
                this.instance = instance;
                type          = instance.GetType();
            }

            public object window
            {
                get
                {
                    var property = type.GetProperty("window", BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(instance, null);
                }
            }

            public object s_OriginalDragSource
            {
                set
                {
                    var field = type.GetField("s_OriginalDragSource", BindingFlags.Static | BindingFlags.NonPublic);
                    field.SetValue(null, value);
                }
            }
        }

        private class _ContainerWindow
        {
            private object instance;
            private Type type;

            public _ContainerWindow(object instance)
            {
                this.instance = instance;
                type          = instance.GetType();
            }


            public object rootSplitView
            {
                get
                {
                    var property = type.GetProperty("rootSplitView", BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(instance, null);
                }
            }
        }

        private class _SplitView
        {
            private object instance;
            private Type type;

            public _SplitView(object instance)
            {
                this.instance = instance;
                type          = instance.GetType();
            }

            public object DragOver(EditorWindow child, Vector2 screenPoint)
            {
                var method = type.GetMethod("DragOver", BindingFlags.Instance | BindingFlags.Public);
                return method.Invoke(instance, new object[] {child, screenPoint});
            }

            public void PerformDrop(EditorWindow child, object dropInfo, Vector2 screenPoint)
            {
                var method = type.GetMethod("PerformDrop", BindingFlags.Instance | BindingFlags.Public);
                method.Invoke(instance, new[] {child, dropInfo, screenPoint});
            }
        }

#endregion
    }*/

    public static class Docker
    {
        public enum DockPosition
        {
            Left,
            Top,
            Right,
            Bottom
        }

        /// <summary>
        /// Docks the second window to the first window at the given position
        /// </summary>
        public static void DockTo(this EditorWindow thiz, EditorWindow target, DockPosition position)
        {
            var mousePosition = GetFakeMousePosition(target, position);

            // Translated from Editor/Mono/GUI/DockArea.cs:537
            var assembly          = typeof(EditorWindow).Assembly;
            var __ContainerWindow = assembly.GetType("UnityEditor.ContainerWindow");
            var __DockArea        = assembly.GetType("UnityEditor.DockArea");
            var __IDropArea       = assembly.GetType("UnityEditor.IDropArea");

            object dropInfo   = null;
            object targetView = null;

            var windows = __ContainerWindow.GetProperty("windows", BindingFlags.Static | BindingFlags.Public)
                                           .GetValue(null, null) as object[];

            if (windows != null)
            {
                foreach (var window in windows)
                {
                    var rootSplitView = window.GetType()
                                              .GetProperty("rootSplitView", BindingFlags.Instance | BindingFlags.Public)
                                              .GetValue(window, null);
                    if (rootSplitView != null)
                    {
                        var method = rootSplitView.GetType()
                                                  .GetMethod("DragOverRootView",
                                                      BindingFlags.Instance | BindingFlags.Public);
                        dropInfo   = method.Invoke(rootSplitView, new object[] {mousePosition});
                        targetView = rootSplitView;
                    }

                    if (dropInfo == null)
                    {
                        var rootView = window.GetType()
                                             .GetProperty("rootView", BindingFlags.Instance | BindingFlags.Public)
                                             .GetValue(window, null);
                        var allChildren =
                            rootView.GetType().GetProperty("allChildren", BindingFlags.Instance | BindingFlags.Public)
                                    .GetValue(rootView, null) as object[];
                        foreach (var view in allChildren)
                        {
                            if (__IDropArea.IsAssignableFrom(view.GetType()))
                            {
                                var method = view.GetType().GetMethod("DragOver",
                                    BindingFlags.Instance | BindingFlags.Public);
                                dropInfo = method.Invoke(view, new object[] {target, mousePosition});
                                if (dropInfo != null)
                                {
                                    targetView = view;
                                    break;
                                }
                            }
                        }
                    }

                    if (dropInfo != null)
                    {
                        break;
                    }
                }
            }

            if (dropInfo != null && targetView != null)
            {
                var otherParent = thiz.GetType().GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic)
                                      .GetValue(thiz);
                __DockArea.GetField("s_OriginalDragSource", BindingFlags.Static | BindingFlags.NonPublic)
                          .SetValue(null, otherParent);
                var method = targetView.GetType().GetMethod("PerformDrop", BindingFlags.Instance | BindingFlags.Public);
                method.Invoke(targetView, new object[] {thiz, dropInfo, mousePosition});
            }
        }

        private static Vector2 GetFakeMousePosition(EditorWindow wnd, DockPosition position)
        {
            Vector2 mousePosition = Vector2.zero;

            // The 80 is required to make the docking work.
            // Smaller values might not work when faking the mouse position.
            var padding = 80;
            switch (position)
            {
                case DockPosition.Left:
                    mousePosition = new Vector2(padding, wnd.position.size.y / 2);
                    break;
                case DockPosition.Top:
                    mousePosition = new Vector2(wnd.position.size.x / 2, padding);
                    break;
                case DockPosition.Right:
                    mousePosition = new Vector2(wnd.position.size.x - padding, wnd.position.size.y / 2);
                    break;
                case DockPosition.Bottom:
                    mousePosition = new Vector2(wnd.position.size.x / 2, wnd.position.size.y - padding);
                    break;
            }

            return new Vector2(wnd.position.x + mousePosition.x, wnd.position.y + mousePosition.y);
        }
    }
}
#endif