using Kitchen.EditorUtilityHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public class SubConfigsEditorFacade<T> : IEnumerable<T>  where T : ScriptableObject
    {
        private readonly SubConfigReorderableList _list;
        private UnityEditor.Editor _currentEditor;

        public SubConfigsEditorFacade(SerializedObject serializedObject, SerializedProperty property,string label = "Items", ISubScriptableFactory factory = null)
        {
            var provider = factory ?? new DefaultSubScriptableFactory(typeof(T));
            _list = CreateReorderable(serializedObject, property, label);
            _list.onAddCallback += (list) => provider.DrawCreateNewElementMenu(this, OnAddCallbackHandler);
            FixReference(serializedObject);
        }

        private void OnAddCallbackHandler(ScriptableObject asset)
        {
            try
            {
                asset.name = "NewItem";
                AssetDatabase.AddObjectToAsset(asset,_list.serializedProperty.serializedObject.targetObject);
                _list.AddNewElement(asset);
                BaseInputWindow.ShowSimpleInput<BaseInputWindow>(aName =>
                                                                 {
                                                                     if (string.IsNullOrEmpty(aName)) return "Should not be Null";
                                                                     asset.name = aName;
                                                                     EditorUtility.SetDirty(asset);
                                                                     return null;
                                                                 },new BaseInputWindow.InputSettings(){DefautValue = asset.GetType().Name, Header = "Name"});
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
        }

        private void FixReference(SerializedObject serializedObject)
        {
            var child = EditorUtils.LoadAllAssetsFrom<T>(serializedObject.targetObject).ToList();
            _list.ForEach(e =>
                          {
                              if (e is T t) child.Remove(t);
                          });
            child.ForEach(e =>
                          {
                              this._list.serializedProperty.InsertArrayElementAtIndex(0);
                              this._list.serializedProperty.GetArrayElementAtIndex(0).objectReferenceValue = e;
                          });
            if (child.Count > 0)
                serializedObject.ApplyModifiedProperties();
        }


        private SubConfigReorderableList CreateReorderable(SerializedObject serializedObject,
                                                              SerializedProperty property,
                                                              string label)
        {
            var res = new SubConfigReorderableList(serializedObject, property)
            {
                drawHeaderCallback           = rect => EditorGUI.LabelField(rect, label),
                onSelectCallback = list =>
                {
                    var currentElement = _list.CurrentSelected;
                    _currentEditor = UnityEditor.Editor.CreateEditor(currentElement);
                },
            };
            res.RemoveElementEvent += list =>
            {
                if (_currentEditor == null) return;
                if (_currentEditor.target == null)
                {
                    _currentEditor = null;
                }
            }; 
            return res;
        }

        public void OnInspectorGUI()
        {
            _list.DoLayoutList();
            _currentEditor?.OnInspectorGUI();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _list)
            {
                yield return item as T;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}