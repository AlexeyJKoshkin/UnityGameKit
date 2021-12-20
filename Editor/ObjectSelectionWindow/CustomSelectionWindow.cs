using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Editor
{
    public class CustomSelectionWindow : EditorWindow
    {
        private List<ScriptableObjectTranslation> _all = new List<ScriptableObjectTranslation>();

        private List<ScriptableObjectTranslation> _filterNode = new List<ScriptableObjectTranslation>();

        private int _previousSearchStringCount;

        private Vector2 _scroll;
        private Action<Object> OnSelectNodeAction;

        private string SearchString = "";
        private bool _closeWhenSelect;

        private void OnGUI()
        {
            try
            {
                EditorGUI.BeginChangeCheck();
                SearchString = EditorGUILayout.TextField(SearchString);
                if (EditorGUI.EndChangeCheck()) OnChangeSearchString(SearchString.Length);

                if (SearchString.Length > 2)
                    ShowObjects(_filterNode);
                else
                    ShowObjects(_all);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Close();
            }
        }

        private void OnLostFocus()
        {
            Close();
        }


        public static void ShowSelectionWindow<T>(IObjectSelectionInfrastructure<T> selectionInfrastructure, bool closeWhenSelect = true)
            where T : Object
        {
            var window = GetWindow<CustomSelectionWindow>();

            window.InitWindow(selectionInfrastructure,closeWhenSelect);
            window.ShowUtility();
            window.Focus();
        }

        private void InitWindow<T>(IObjectSelectionInfrastructure<T> selectionInfrastructure, bool closeWhenSelect) where T : Object
        {
            _closeWhenSelect = closeWhenSelect;
            _all.Clear();
            _filterNode.Clear();
            OnSelectNodeAction = node => selectionInfrastructure.SelectObject(node as T);
            foreach (var questNode in selectionInfrastructure)
                _all.Add(new ScriptableObjectTranslation(questNode, selectionInfrastructure.GetObjectName(questNode)));


            _all.Sort((o1, o2) =>
                HumanStringSorter.InnerCompare(o1.RussianName, o2.RussianName));
        }

        private void ShowObjects(List<ScriptableObjectTranslation> all)
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var metaQuest in all)
                if (GUILayout.Button(new GUIContent(metaQuest.RussianName, metaQuest.Node.name)))
                {
                    OnSelectNodeAction?.Invoke(metaQuest.Node);
                    if(_closeWhenSelect)
                        Close();
                }

            EditorGUILayout.EndScrollView();
        }

        private void OnChangeSearchString(int searchStringLength)
        {
            if (searchStringLength > 2)
            {
                if (_previousSearchStringCount == 0 || _previousSearchStringCount > searchStringLength)
                {
                    _previousSearchStringCount = searchStringLength;
                    SearchIn(_all);
                    return;
                }

                if (searchStringLength > _previousSearchStringCount)
                {
                    _previousSearchStringCount = searchStringLength;
                    SearchIn(_filterNode);
                }
            }
            else
            {
                _previousSearchStringCount = 0;
            }
        }

        private void SearchIn(List<ScriptableObjectTranslation> all)
        {
            var list = new List<ScriptableObjectTranslation>();
            foreach (var translation in all)
                if (CheckRussianName(translation) || CheckNodeName(translation))
                    list.Add(translation);

            _filterNode = list;
        }

        private bool CheckNodeName(ScriptableObjectTranslation translation)
        {
            return translation.Node.name.Contains(SearchString);
        }

        private bool CheckRussianName(ScriptableObjectTranslation translation)
        {
            return !string.IsNullOrEmpty(translation.RussianName) && translation.RussianName.Contains(SearchString);
        }

        private class ScriptableObjectTranslation
        {
            public readonly Object Node;
            public readonly string RussianName;

            public ScriptableObjectTranslation(Object node, string russianName)
            {
                Node        = node;
                RussianName = russianName;
            }
        }
    }
}