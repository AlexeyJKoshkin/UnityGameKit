using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public class GenericPrefabPathLayout<T> where T : Object
    {
        private bool _needPrefab;
        private T _prefabCandidate;

        private T _prefabObject;
        private Texture _prefabPreview;

        private float _size = 40;

        public bool ClearPrefabWhenFoldout = false;

        //  private readonly ImportToAddressables _addressablesImporter;

        public string LabelPreview;

        public GenericPrefabPathLayout(string key, float size = 40)
        {
            LabelPreview = typeof(T).Name;

            /*string groupName = string.IsNullOrEmpty(addGroup)
                ? AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.Name
                : addGroup;*/

            // _addressablesImporter = new ImportToAddressables(groupName);
            _size = size;
            //  _prefabObject = FindPrefabByKey(key);
            _needPrefab = _prefabObject != null;
            UpdatePrefabPreview();
        }

        public bool IsShowPrefab => _needPrefab;

        /*private T FindPrefabByKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            foreach (var gr in AddressableAssetSettingsDefaultObject.Settings.groups)
            {
                foreach (var en in gr.entries)
                {
                    if (en.address == key)
                    {
                        return AssetDatabase.LoadAssetAtPath(en.AssetPath, typeof(T)) as T;
                    }
                }
            }

            return null;
        }*/

        private void UpdatePrefabPreview()
        {
            if (_prefabPreview == null && _prefabObject != null)
                _prefabPreview = AssetPreview.GetAssetPreview(_prefabObject);
        }


        public bool DrawPrefabSelection()
        {
            _needPrefab = EditorGUILayout.Foldout(_needPrefab, LabelPreview);
            if (!_needPrefab)
            {
                if (ClearPrefabWhenFoldout)
                {
                    _prefabObject  = null;
                    _prefabPreview = null;
                }

                return true;
            }

            UpdatePrefabPreview();
            using (new GUILayout.VerticalScope("Box"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(new GUIContent(_prefabPreview), GUILayout.Width(_size), GUILayout.Height(_size));
                    GUILayout.FlexibleSpace();
                }

                bool wasChange = false;
                EditorGUI.BeginChangeCheck();
                _prefabCandidate = EditorGUILayout.ObjectField(_prefabObject, typeof(T), false) as T;
                if (EditorGUI.EndChangeCheck())
                    if (_prefabCandidate != _prefabObject)
                    {
                        _prefabPreview = null;
                        UpdatePrefabPreview();
                        wasChange = true;
                    }

                return wasChange;
            }
        }


        /*private bool DrawPrefabField()
        {
            bool wasChange = false;
            var newObject = EditorUtils.MouseEventHandlerHelper<T>(GUILayoutUtility.GetLastRect(), _prefabPreview,
                () =>
                {
                    _prefabCandidate = null;
                    UserSetNewPrefab();
                    wasChange = true;
                });
            if (newObject != _prefabObject && newObject != null)
            {
                _prefabCandidate = newObject;
                UserSetNewPrefab();
                wasChange = true;
            }

            return wasChange;
        }*/
    }
}