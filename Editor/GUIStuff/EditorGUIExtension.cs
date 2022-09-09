using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Editor
{
    public static class EditorGUIExtensions
    {
        public static void DoButton(string content, Action action,params GUILayoutOption[] options)
        {
            using (new EditorGUI.DisabledScope(action == null))
            {
                if (GUILayout.Button(content,options))
                {
                    action.Invoke();
                }    
            }
        }
        public static void DoButton(string content, Action action, GUIStyle style, params GUILayoutOption[] options )
        {
            using (new EditorGUI.DisabledScope(action == null))
            {
                if (GUILayout.Button(content,style, options))
                {
                    action.Invoke();
                }    
            }
        }
        
        public static void DoButton(GUIContent content, Action action, GUIStyle style, params GUILayoutOption[] options )
        {
            using (new EditorGUI.DisabledScope(action == null))
            {
                if (GUILayout.Button(content,style, options))
                {
                    action.Invoke();
                }    
            }
        }
        
        public static void DoButton(GUIContent content, Action action,  params GUILayoutOption[] options )
        {
            using (new EditorGUI.DisabledScope(action == null))
            {
                if (GUILayout.Button(content, options))
                {
                    action.Invoke();
                }    
            }
        }


        public static void DrawHeaderScript(this UnityEditor.Editor editor)
        {
            if(editor == null) return;
            
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:",  GetScriptObject(editor.target),editor.target.GetType(), false);
            GUI.enabled = true;
        }

        private static Object GetScriptObject(Object editorTarget)
        {
            if (editorTarget == null) return null;
            if (editorTarget is ScriptableObject scriptableObject)
                return MonoScript.FromScriptableObject(scriptableObject);
            if (editorTarget is MonoBehaviour monoBehaviour)
                return MonoScript.FromMonoBehaviour(monoBehaviour);
            Debug.LogError("Unknown Type " + editorTarget.GetType().Name);
            return null;
        }
    }
}