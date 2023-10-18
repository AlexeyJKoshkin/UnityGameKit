#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     Scene auto loader.
/// </summary>
/// <description>
///     This class adds a File &gt; Scene Autoload menu containing options to select a "master scene"
///     enable it to be auto-loaded when the user presses play in the editor. When enabled, the selected
///     scene will be loaded on play, then the original scene will be reloaded on stop. Based on an idea
///     on this thread:
///     http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
/// </description>
[InitializeOnLoad]
internal static class SceneAutoLoader
{
    // Properties are remembered as editor preferences.
    private const string cEditorPrefLoadMasterOnPlay = "SceneAutoLoader.LoadMasterOnPlay";

    private const string cEditorPrefMasterScene = "SceneAutoLoader.MasterScene";

    private const string cEditorPrefPreviousScene = "SceneAutoLoader.PreviousScene";

    // Static constructor binds a playmode-changed callback. [InitializeOnLoad] above makes sure this
    // gets executed.
    static SceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorPrefs.SetBool("TreeViewExpansionAnimation", false);
    }

    private static bool LoadMasterOnPlay
    {
        get => EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, false);
        set => EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value);
    }

    private static string MasterScene
    {
        get => EditorPrefs.GetString(Application.productName + cEditorPrefMasterScene, "Master.unity");
        set => EditorPrefs.SetString(Application.productName + cEditorPrefMasterScene, value);
    }

    private static string PreviousScene
    {
        get => EditorPrefs.GetString(Application.productName + cEditorPrefPreviousScene,
            SceneManager.GetActiveScene().name);
        set => EditorPrefs.SetString(Application.productName + cEditorPrefPreviousScene, value);
    }

    // Menu items to select the "master" scene and control whether or not to load it.
    [MenuItem("File/Scene Autoload/Select Master Scene...")]
    private static void SelectMasterScene()
    {
        var masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
        if (!string.IsNullOrEmpty(masterScene))
        {
            MasterScene      = masterScene;
            LoadMasterOnPlay = true;
            Menu.SetChecked("File/Scene Autoload/Load Master On Play", true);
        }
    }

    [MenuItem("File/Scene Autoload/Load Master On Play")]
    private static void EnableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = !LoadMasterOnPlay;
        Menu.SetChecked("File/Scene Autoload/Load Master On Play", LoadMasterOnPlay);
    }

    [MenuItem("File/Scene Autoload/Load Master On Play", true)]
    private static bool validationEnableLoadMasterOnPlay()
    {
        Menu.SetChecked("File/Scene Autoload/Load Master On Play", LoadMasterOnPlay);
        return true;
    }

    // Play mode change callback handles the scene load/reload.
    private static void OnPlayModeChanged(PlayModeStateChange playModeStateChange)
    {
        if (!LoadMasterOnPlay)
            return;

        switch (playModeStateChange)
        {
            case PlayModeStateChange.ExitingEditMode:
            {
                // User pressed play -- autoload master scene.
                PreviousScene = SceneManager.GetActiveScene().path;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    if (EditorSceneManager.OpenScene(MasterScene) == default)
                    {
                        Debug.LogError($"error: scene not found: {MasterScene}");
                        EditorApplication.isPlaying = false;
                    }
                }
                else
                {
                    // User cancelled the save operation -- cancel play as well.
                    EditorApplication.isPlaying = false;
                }

                break;
            }

            case PlayModeStateChange.EnteredEditMode:
            {
                if (!string.IsNullOrEmpty(PreviousScene))
                    EditorSceneManager.OpenScene(PreviousScene, OpenSceneMode.Single);
                break;
            }
        }
    }
}

#endif