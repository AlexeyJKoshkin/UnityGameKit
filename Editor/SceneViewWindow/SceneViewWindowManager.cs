using System;
using UnityEditor;
using UnityEngine;

namespace GameKit.SceneViewWindow
{
    public interface IEmbededEditor
    {
        string Capition { get; }
        IEmbededEditor OnSceneGUI(Rect windowRect);
    }

    public class SceneViewWindowManager : ScriptableObject
    {
        [SerializeField] private Rect _windowRect = new Rect(100, 300, 300, 500);

        // The current content of the window
        private IEmbededEditor _editor;

        // Holds window scroll position
        private Vector2 _scrollPosition = Vector2.zero;


        /**
         * Makes the window visible
         */
        public void Show(IEmbededEditor editor, Vector2 windowSize)
        {
            if (editor == null)
                throw new Exception("Editor error: editor cannot be null.");

#if UNITY_2020_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif

            _editor = editor;
            _windowRect = new Rect(new Vector2(SceneView.lastActiveSceneView.position.width - windowSize.x, 0),
                windowSize);
            if (_editor != null)
            {
#if UNITY_2020_1_OR_NEWER
                SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
            }
        }


        public void Hide()
        {
            _editor                      =  null;
#if UNITY_2020_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
        }

        /**
         * Draws the window with the unity scene view skin
         */
        private void OnSceneGUI(SceneView view)
        {
            //Prevent unity from resetting scene gui skin
            GUISkin oldSkin = GUI.skin;
            GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

            Handles.BeginGUI();
            var idPanel = GUIUtility.GetControlID(FocusType.Passive, _windowRect);
            _windowRect = GUI.Window(idPanel, LimiterPositionOnSceneView(_windowRect, view.position), DrawWindow,
                _editor.Capition);
            Handles.EndGUI();

            GUI.skin = oldSkin;
        }

        /**
         * Used to lock the window moving area inside the Scene View bounds.
         */
        private static Rect LimiterPositionOnSceneView(Rect windowRect, Rect sceneViewRect)
        {
            windowRect.x = Mathf.Clamp(windowRect.x, 0, sceneViewRect.width - windowRect.width);
            windowRect.y = Mathf.Clamp(windowRect.y, 0, sceneViewRect.height - windowRect.height);
            return windowRect;
        }


        /**
         * Draws the content of the window
         */
        private void DrawWindow(int windowID)
        {
            Color labelNormalColor  = EditorStyles.label.normal.textColor;
            Color labelFocusedColor = EditorStyles.label.focused.textColor;
            EditorStyles.label.normal.textColor  = Color.white;
            EditorStyles.label.focused.textColor = Color.white;

            Color foldoutActiveColor    = EditorStyles.foldout.active.textColor;
            Color foldoutFocusedColor   = EditorStyles.foldout.focused.textColor;
            Color foldoutNormalColor    = EditorStyles.foldout.normal.textColor;
            Color foldoutOnActiveColor  = EditorStyles.foldout.onActive.textColor;
            Color foldoutOnFocusedColor = EditorStyles.foldout.onFocused.textColor;
            Color foldoutOnNormalColor  = EditorStyles.foldout.onNormal.textColor;

            Texture2D foldoutActiveBack    = EditorStyles.foldout.active.background;
            Texture2D foldoutFocusedBack   = EditorStyles.foldout.focused.background;
            Texture2D foldoutNormalBack    = EditorStyles.foldout.normal.background;
            Texture2D foldoutOnActiveBack  = EditorStyles.foldout.onActive.background;
            Texture2D foldoutOnFocusedBack = EditorStyles.foldout.onFocused.background;
            Texture2D foldoutOnNormalBack  = EditorStyles.foldout.onNormal.background;

            GUIStyle sceneViewFoldout = GUI.skin.GetStyle("IN Foldout");
            EditorStyles.foldout.active.textColor    = sceneViewFoldout.active.textColor;
            EditorStyles.foldout.focused.textColor   = sceneViewFoldout.active.textColor;
            EditorStyles.foldout.normal.textColor    = sceneViewFoldout.active.textColor;
            EditorStyles.foldout.onActive.textColor  = sceneViewFoldout.onActive.textColor;
            EditorStyles.foldout.onFocused.textColor = sceneViewFoldout.onActive.textColor;
            EditorStyles.foldout.onNormal.textColor  = sceneViewFoldout.onActive.textColor;

            EditorStyles.foldout.active.background    = sceneViewFoldout.active.background;
            EditorStyles.foldout.focused.background   = sceneViewFoldout.active.background;
            EditorStyles.foldout.normal.background    = sceneViewFoldout.active.background;
            EditorStyles.foldout.onActive.background  = sceneViewFoldout.onActive.background;
            EditorStyles.foldout.onFocused.background = sceneViewFoldout.onActive.background;
            EditorStyles.foldout.onNormal.background  = sceneViewFoldout.onActive.background;

            try
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
                _editor         = _editor.OnSceneGUI(_windowRect);
                if (_editor == null)
                    Hide();
                EditorGUILayout.EndScrollView();
                GUI.DragWindow();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            EditorStyles.foldout.active.textColor    = foldoutActiveColor;
            EditorStyles.foldout.focused.textColor   = foldoutFocusedColor;
            EditorStyles.foldout.normal.textColor    = foldoutNormalColor;
            EditorStyles.foldout.onActive.textColor  = foldoutOnActiveColor;
            EditorStyles.foldout.onFocused.textColor = foldoutOnFocusedColor;
            EditorStyles.foldout.onNormal.textColor  = foldoutOnNormalColor;

            EditorStyles.foldout.active.background    = foldoutActiveBack;
            EditorStyles.foldout.focused.background   = foldoutFocusedBack;
            EditorStyles.foldout.normal.background    = foldoutNormalBack;
            EditorStyles.foldout.onActive.background  = foldoutOnActiveBack;
            EditorStyles.foldout.onFocused.background = foldoutOnFocusedBack;
            EditorStyles.foldout.onNormal.background  = foldoutOnNormalBack;

            EditorStyles.label.normal.textColor  = labelNormalColor;
            EditorStyles.label.focused.textColor = labelFocusedColor;
        }
    }
}