using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameKit.Editor
{
    /**
     * Provides helper methods to be used in different game editing situations.
     */
    public static class EditorUtils
    {
        // Provides a path which should be used to store a temporary data for long running operations.


        private static Dictionary<Type, Func<string, string>> _pathNormalizer =
            new Dictionary<Type, Func<string, string>>();

        static EditorUtils()
        {
            _pathNormalizer.Add(typeof(MonoBehaviour), Path.GetFileNameWithoutExtension);
            _pathNormalizer.Add(typeof(GameObject), Path.GetFileNameWithoutExtension);
        }

        /**
         * Creates unity asset object of the specified type saving it by the specified path.
         */
        public static ScriptableObject CreateAsset(Type type, string path, bool saveAssets = false)
        {
            if (!path.EndsWith(".asset"))
                path += ".asset";
            var dataClass = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(dataClass, path);
            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return dataClass;
        }


        public static ScriptableObject CreateSubAsset(Type type, string nameAsset, ScriptableObject parent)
        {
            var settings = ScriptableObject.CreateInstance(type);
            settings.name = nameAsset;
            AssetDatabase.AddObjectToAsset(settings, parent);
            return settings;
        }


        public static T CreateSubAsset<T>(string nameAsset, ScriptableObject parent) where T : ScriptableObject
        {
            var settings = ScriptableObject.CreateInstance<T>();
            settings.name = nameAsset;
            AssetDatabase.AddObjectToAsset(settings, parent);
            return settings;
        }

        public static T CreateAsset<T>(string path, bool saveAssets = false) where T : ScriptableObject
        {
            return CreateAsset(typeof(T), path, saveAssets) as T;
        }

        public static T LoadAsset<T>(string path, bool createIfNotExists, bool saveassets = false)
            where T : ScriptableObject
        {
            return LoadAsset(typeof(T), path, createIfNotExists, saveassets) as T;
        }
        
        public static ScriptableObject LoadAsset(Type type,string path, bool createIfNotExists, bool saveassets = false)
        {
            var asset = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
            if (asset == null && createIfNotExists) asset = CreateAsset(type,path, saveassets);

            return asset;
        }

        public static TextAsset SaveToFile(string path, string text)
        {
            var asset = LoadTextAsset(path, true);
            File.WriteAllText(path, text);
            return asset;
        }


        public static T SavePrefab<T>(T saveobject, string pathSaveFolder, params string[] lables) where T : Component
        {
            var pathPrefab = pathSaveFolder + "/" + saveobject.name + ".prefab";

            var prefabObject = PrefabUtility.SaveAsPrefabAsset(saveobject.gameObject, pathPrefab);
            if (lables != null)
                AssetDatabase.SetLabels(prefabObject, lables); // set label for objectpicker searching
            Object.DestroyImmediate(saveobject.gameObject);
            return prefabObject.GetComponent<T>();
        }

        public static TextAsset LoadTextAsset(string path, bool createIfNotExists)
        {
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
            if (asset == null && createIfNotExists)
            {
                File.Create(path).Dispose();
                AssetDatabase.Refresh();
                asset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
            }

            return asset;
        }

        /// <summary>
        ///     Генерирует текстуру из спрайта
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static Texture2D GenerateTextureFromSprite(Sprite sprite)
        {
            if (sprite == null) return null;
            if (!sprite.texture.isReadable) return null;
            Texture2D tex2d_image = null;
            if (sprite.packed)
            {
                var pixels = sprite.texture.GetPixels(
                    (int) sprite.rect.x,
                    (int) sprite.rect.y,
                    (int) sprite.rect.width,
                    (int) sprite.rect.height);
                tex2d_image = new Texture2D(
                    (int) sprite.rect.width,
                    (int) sprite.rect.height);
                tex2d_image.SetPixels(pixels);
                tex2d_image.Apply();
            }
            else
            {
                tex2d_image = sprite.texture;
            }

            return tex2d_image;
        }


        /// <summary>
        ///     Обрабатывает события мыши над объектов в рамке
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dropAarea"></param>
        /// <param name="prewieSprite"></param>
        /// <returns></returns>
        public static T MouseEventHandlerHelper<T>(Rect dropAarea, Object previewObject, Action OnDelete = null)
            where T : Object
        {
            var evt = Event.current;
            switch (evt.type)
            {
                case EventType.KeyDown:
                {
                    if (Event.current.keyCode == KeyCode.Delete)
                        OnDelete?.Invoke();
                    break;
                }

                case EventType.MouseDown:
                    if (dropAarea.Contains(evt.mousePosition)) EditorGUIUtility.PingObject(previewObject);

                    break;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropAarea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        if (typeof(T) == typeof(Sprite))
                        {
                            var res = DragAndDrop.objectReferences[0];
                            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GetAssetPath(res));
                        }

                        if (DragAndDrop.objectReferences != null)
                            return DragAndDrop.objectReferences.FirstOrDefault(o => o is T) as T;
                    }

                    break;
            }

            return previewObject as T;
        }

        /**
         * Creates a new scene. Used to create different editors.
         */
        public static Scene NewScene(bool destroyAll)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (destroyAll)
                DestroyAllObjects();
            return EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        }


        /**
         * Used to find all object of the secified type on the loaded scene.
         */
        public static T[] FindObjectsOfTypeAll<T>()
        {
            var results        = new List<T>();
            var s              = SceneManager.GetActiveScene();
            var allGameObjects = s.GetRootGameObjects();
            for (var j = 0; j < allGameObjects.Length; ++j)
                results.AddRange(allGameObjects[j].GetComponentsInChildren<T>(true));

            return results.ToArray();
        }


        public static IEnumerable<T> LoadAllAssetsAtPath<T>(string path, bool searchInSubDirs = false,
                                                            string searchPattern = null) where T : Object
        {
            return LoadAllAssetsAtPath(typeof(T), path, searchInSubDirs, searchPattern).Cast<T>();
        }

        public static IEnumerable<Object> LoadAllAssetsAtPath(Type type, string path, bool searchInSubDirs = false,
                                                              string searchPattern = null)
        {
            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1);
            searchPattern = string.IsNullOrEmpty(searchPattern) ? "*.*" : searchPattern;
            string[] fileEntries = searchInSubDirs
                ? Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                : Directory.GetFiles(path);

            Func<string, string> pathNormilizer = null;

            foreach (var pair in _pathNormalizer)
                if (type.IsSubclassOf(pair.Key) || type == pair.Key)
                {
                    pathNormilizer = pair.Value;
                    break;
                }

            foreach (var fileName in fileEntries.Where(e => !e.EndsWith(".meta")))
            {
                var localPath      = pathNormilizer == null ? fileName : pathNormilizer(fileName);
                var assetPathIndex = fileName.IndexOf("Assets");
                localPath = fileName.Substring(assetPathIndex);
                var asset = AssetDatabase.LoadAssetAtPath(localPath, type);
                if (asset != null)
                    yield return asset;
            }
        }

        /**
         * Returns true if asset specified by the path is exists
         */
        public static bool AssetExists(string assetPath)
        {
            return File.Exists(assetPath) || Directory.Exists(assetPath);
        }

        /**
         * Moves the specified asset from its current place to the specified one.
         */
        public static void MoveAsset(Object asset, string destinationFolder)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.MoveAsset(path, destinationFolder + "/" + Path.GetFileName(path));
            AssetDatabase.Refresh();
        }

        /**
         * Removes the specified asset from the project
         */
        public static void RemoveAsset(Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();
        }

        /**
         * Create asset directory specified by the path.
         */
        public static DefaultAsset CreateAssetFolder(string path)
        {
            path = path.Replace("//", "/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }

            return AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
        }

        /// <summary>
        ///     Load all files from folder
        /// </summary>
        /// <param name="SourceFolder"></param>
        /// <param name="Filter"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static string[] GetFilesFrom(string SourceFolder, string Filter, SearchOption searchOption)
        {
            var alFiles = new List<string>();
            // Create an array of filter string
            var MultipleFilters = Filter.Split('|');

            // for each filter find mathing file names
            foreach (var FileFilter in MultipleFilters)
                // add found file names to array list
                alFiles.AddRange(Directory.GetFiles(SourceFolder, FileFilter, searchOption));

            // returns string array of relevant file names
            return alFiles.ToArray();
        }


        /**
         * Returns padding which should be used with ui control to center a scaled image inside
         * its rectangle area.
         */
        public static RectOffset CenterImage(Texture image, int rectSize)
        {
            if (image == null)
                return new RectOffset();
            float max   = Mathf.Max(image.width, image.height);
            var   ratio = rectSize / max;

            var paddingH = rectSize - (int) (ratio * image.width);
            var paddingV = rectSize - (int) (ratio * image.height);

            return new RectOffset(paddingH >> 1, paddingH >> 1, paddingV >> 1, paddingV >> 1);
        }

        /**
         * Return array of T from reordable list
         */
        public static T[] ToArray<T>(this ReorderableList list)
        {
            if (list.count == 0)
                return new T[0];
            if (list.list[0] is T)
            {
                var res = new T[list.count];
                for (var i = 0; i < list.count; i++)
                    res[i] = (T) list.list[i];
                return res;
            }

            throw new ArrayTypeMismatchException();
        }


        // static Dictionary<Type, Object> _cashedFindAssets = new Dictionary<Type, Object>();

        public static T FindAsset<T>(params string[] searchinfolders) where T : Object
        {
            return FindAsset(typeof(T), searchinfolders) as T;
        }


        public static Object FindAsset(Type t, params string[] searchinfolders)
        {
            /*if (_cashedFindAssets.TryGetValue(t, out var findObject))
                return findObject;*/

            var guilds = searchinfolders == null
                ? AssetDatabase.FindAssets($"t:{t.Name}")
                : AssetDatabase.FindAssets($"t:{t.Name}", searchinfolders);
            if (guilds == null || guilds.Length == 0) return null;
            foreach (var guid in guilds) return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), t);

            return null;
        }


        public static string FindAssetPath<T>() where T : Object
        {
            var findObject = FindAsset<T>();
            if (findObject == null) return null;
            return AssetDatabase.GetAssetPath(findObject);
        }

        public static IEnumerable<T> LoadAllAssetsFrom<T>(Object target) where T : Object
        {
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target)))
                if (asset is T myAsset)
                    yield return myAsset;
        }


        public static IEnumerable<T> FindAssets<T>(params string[] searchinfolders) where T : Object
        {
            var guilds = searchinfolders == null
                ? AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                : AssetDatabase.FindAssets($"t:{typeof(T).Name}", searchinfolders);
            if (guilds == null || guilds.Length == 0) yield break;

            foreach (var guid in guilds)
            {
                var res = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) as T;
                if (res != null) yield return res;
            }
        }


        /// <summary>
        ///     Конвертим абсолютный путь в относительный
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string AbsolutePathToLocal(string absolutePath)
        {
            absolutePath = absolutePath.Replace(@"\", "/");
            if (absolutePath.StartsWith(Application.dataPath))
            {
                var relativepath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
                return relativepath;
            }

            return absolutePath;
        }


        public static void DestroyAllObjects()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
                foreach (var go in SceneManager.GetSceneAt(i).GetRootGameObjects())
                    Object.DestroyImmediate(go);
        }

        public static IEnumerable<T> CreateMissedSubAsset<T>(this ScriptableObject mainAsset) where T : ScriptableObject
        {
            HashSet<Type> hashSet = new HashSet<Type>(ReflectionHelper.GetAllTypesInSolution<T>());
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(mainAsset)))
            {
                if (asset is T subasset)
                    hashSet.Remove(subasset.GetType());

                if (asset == null)
                {
                    Object.DestroyImmediate(asset, true);
                }
            }

            foreach (var subAssetType in hashSet)
                yield return CreateSubAsset(subAssetType, subAssetType.Name, mainAsset) as T;
        }

        /// <summary>
        /// Function used to remove a sub-asset that is missing the script reference
        /// https://gitlab.com/RotaryHeart-UnityShare/subassetmissingscriptdelete/-/blob/master/FixMissingScript
        /// </summary>
        /// <param name="target">The main asset that holds the sub-asset</param>
        public static void FixMissingScript(Object target)
        {
            string targetPath = AssetDatabase.GetAssetPath(target);
            var    subAssets    = AssetDatabase.LoadAllAssetsAtPath(targetPath);
            if(subAssets.All(o=>o!=null)) return;
            
            //Create a new instance of the object to delete
            ScriptableObject newInstance = ScriptableObject.CreateInstance(target.GetType());

            //Copy the original content to the new instance
            EditorUtility.CopySerialized(target, newInstance);
            newInstance.name = target.name;

            string clonePath    = targetPath.Replace(".asset", "CLONE.asset");

            //Create the new asset on the project files
            AssetDatabase.CreateAsset(newInstance, clonePath);
            AssetDatabase.ImportAsset(clonePath);

            //Unhide sub-assets
            HideFlags[] flags     = new HideFlags[subAssets.Length];
            for (int i = 0; i < subAssets.Length; i++)
            {
                //Ignore the "corrupt" one
                if (subAssets[i] == null)
                    continue;

                //Store the previous hide flag
                flags[i]               = subAssets[i].hideFlags;
                subAssets[i].hideFlags = HideFlags.None;
                EditorUtility.SetDirty(subAssets[i]);
            }

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();

            //Reparent the subAssets to the new instance
            foreach (var subAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(targetPath))
            {
                //Ignore the "corrupt" one
                if (subAsset == null)
                    continue;

                //We need to remove the parent before setting a new one
                AssetDatabase.RemoveObjectFromAsset(subAsset);
                AssetDatabase.AddObjectToAsset(subAsset, newInstance);
            }

            //Import both assets back to unity
            AssetDatabase.ImportAsset(targetPath);
            AssetDatabase.ImportAsset(clonePath);

            //Reset sub-asset flags
            for (int i = 0; i < subAssets.Length; i++)
            {
                //Ignore the "corrupt" one
                if (subAssets[i] == null)
                    continue;

                subAssets[i].hideFlags = flags[i];
                EditorUtility.SetDirty(subAssets[i]);
            }

            EditorUtility.SetDirty(newInstance);
            AssetDatabase.SaveAssets();

            //Here's the magic. First, we need the system path of the assets
            string globalToDeletePath =
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), targetPath);
            string globalClonePath =
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), clonePath);

            //We need to delete the original file (the one with the missing script asset)
            //Rename the clone to the original file and finally
            //Delete the meta file from the clone since it no longer exists

            System.IO.File.Delete(globalToDeletePath);
            System.IO.File.Delete(globalClonePath + ".meta");
            System.IO.File.Move(globalClonePath, globalToDeletePath);

            AssetDatabase.Refresh();
        }


        public static void ShowErrorMessage(string errorMessage)
        {
            EditorUtility.DisplayDialog("ОШИБКА", errorMessage, "Ок");
        }

        public static void Ping(string folderPath, bool setSelected = false)
        {
            if (folderPath.EndsWith("/"))
                folderPath = folderPath.Substring(0, folderPath.Length - 1);

            Object obj = AssetDatabase.LoadAssetAtPath(folderPath, typeof(Object));
            if (setSelected)
                Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        public static void Ping<T>(bool setSelected) where T : Object
        {
            var asset = FindAsset<T>();
            EditorGUIUtility.PingObject(asset);
            if (setSelected)
                Selection.activeObject = asset;
        }


#region Renaming

        public static bool TryRename(Object target, string newname)
        {
            if (target == null)
            {
                ShowErrorMessage("Object is NULL");
                return false;
            }

            if (target.name == newname) return true;

            if (AssetDatabase.IsSubAsset(target))
            {
                RenameSubAsset(target, newname);
                return true;
            }

            return RenameAsset(target, newname);
        }

        private static bool RenameAsset(Object target, string newname)
        {
            var    targetPath = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrEmpty(targetPath)) return true;
            string newPath    = targetPath.Replace(Path.GetFileNameWithoutExtension(targetPath), newname);
            if (AssetExists(newPath))
            {
                ShowErrorMessage($"Нельзя задать имя {newname}\nУже существует {newPath}");
                return false;
            }

            string error = AssetDatabase.RenameAsset(targetPath, newname);
            if (!string.IsNullOrEmpty(error))
            {
                ShowErrorMessage(error);
                return false;
            }

            EditorUtility.SetDirty(target);

            return true;
        }

        private static void RenameSubAsset(Object target, string newname)
        {
            target.name = newname;
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

#endregion

        public static void SetInspectorLock(bool isLock)
        {
            var inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            var all           = Resources.FindObjectsOfTypeAll(inspectorType);
            if (all.Length == 0) return;
            var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
            // Invoke `isLocked` setter method passing 'true' to lock the inspector
            isLocked.GetSetMethod().Invoke(all[0] as EditorWindow, new object[] {isLock});
        }

        public static GUISkin GetSkinByName(string nameskin)
        {
            var guids = AssetDatabase.FindAssets($"{nameskin} t:GUISkin");
            if (guids.Length > 1)
                Debug.LogWarning("More than one guiskin with name " + nameskin);

            if (guids.Length == 0)
            {
                Debug.LogWarning("Not found skin " + nameskin);
                return null;
            }

            var guid = guids[0];
            return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GUISkin)) as GUISkin;
        }
    }
}