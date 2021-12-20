using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Editor
{
    public class SubConfigReorderableList : ReorderableList, IEnumerable<ScriptableObject>
    {
        private readonly ScriptableObject _target;
        public event Action<ReorderableList> RemoveElementEvent;
   
       // protected readonly ISubScriptableFactory SubScriptableFactory;

        public ScriptableObject CurrentSelected {
            get
            {
                if (serializedProperty == null)
                {
                    if (this.index >= this.list.Count) return null;
                    return this.list[this.index] as ScriptableObject;
                }
                else
                {
                    if (this.index >= this.serializedProperty.arraySize) return null;
                    return this.serializedProperty.GetArrayElementAtIndex(this.index).objectReferenceValue as ScriptableObject;
                }
            }
        }

        public SubConfigReorderableList(SerializedObject serializedObject, SerializedProperty property) : base(serializedObject,
            property, true, true, true, true)
        {
            _target = serializedObject.targetObject as ScriptableObject;
      
            onRemoveCallback          = OnRemoveStageCallback;
            drawElementCallback       = DrawElementCallback;
           
        }
        
     
        private void DrawElementCallback(Rect rect, int i, bool isactive, bool isfocused)
        {
            if (serializedProperty != null)
            {
                GUI.enabled = false;
                EditorGUI.ObjectField(rect, serializedProperty.GetArrayElementAtIndex(i),GUIContent.none);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.ObjectField(rect, list[i] as ScriptableObject, typeof(ScriptableObject), false);
            }
        }

        private void OnRemoveStageCallback(ReorderableList reorderableList)
        {
            var item = GetCurrentElement();
            if (item != null)
            {
                Object.DestroyImmediate(item, true);
                RemoveElementEvent?.Invoke(this);
            }
            DoRemoveButton();
            EditorUtility.SetDirty(_target);
            AssetDatabase.SaveAssets();
          
        }
        
        void DoRemoveButton()
        {
            int index1 = this.index;
            if (index1 < 0 || index1 >= this.count)
                index1 = this.count - 1;
            if (this.serializedProperty != null)
            {
                this.serializedProperty.DeleteArrayElementAtIndex(index1);
                if (index1 < this.count - 1)
                {
                    SerializedProperty serializedProperty = this.serializedProperty.GetArrayElementAtIndex(index1);
                    for (int index2 = index1 + 1; index2 < this.count; ++index2)
                    {
                        SerializedProperty arrayElementAtIndex = this.serializedProperty.GetArrayElementAtIndex(index2);
                        serializedProperty.isExpanded = arrayElementAtIndex.isExpanded;
                        serializedProperty            = arrayElementAtIndex;
                    }
                }
                this.serializedProperty.serializedObject.ApplyModifiedProperties();
                if (this.index >= this.serializedProperty.arraySize - 1)
                    this.index = this.serializedProperty.arraySize - 1;
            }
            else
            {
                this.list.RemoveAt(index1);
                if (this.index >= this.list.Count - 1)
                    this.index = this.list.Count - 1;
            }
        }
        
    
        private Object GetCurrentElement()
        {
            if (serializedProperty != null)
                return serializedProperty.GetArrayElementAtIndex(this.index).objectReferenceValue;
            return list[this.index] as Object;
        }



      public void AddNewElement(ScriptableObject scriptableObject)
        {
            if(scriptableObject == null) return;
           
            if (serializedProperty != null)
            {
                serializedProperty.arraySize++;
                serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1).objectReferenceValue =
                    scriptableObject;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                list.Add(scriptableObject);
            }
          
            EditorUtility.SetDirty(_target);
            AssetDatabase.SaveAssets();
        }


        public IEnumerator<ScriptableObject> GetEnumerator()
        {
            if (serializedProperty != null)
            {
                for (int i = 0; i < serializedProperty.arraySize; i++)
                {
                    yield return serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as ScriptableObject;    
                }
           }
            else
            {
                foreach (var scriptableObject in list)
                {
                    yield return scriptableObject as ScriptableObject;
                }
            } 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}