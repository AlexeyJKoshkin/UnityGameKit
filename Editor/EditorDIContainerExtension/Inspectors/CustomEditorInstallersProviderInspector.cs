using GameKit.Editor;
using UnityEditor;

namespace GameKit.EditorContext.Inspectors
{
    [CustomEditor(typeof(CustomEditorInstallersProvider))]
    public class CustomEditorInstallersProviderInspector : UnityEditor.Editor
    {
        private SubConfigsEditorFacade<BaseCustomEditorInstaller> _installersDrawer;

        private void OnEnable()
        {
            EditorUtils.FixMissingScript(target);
            _installersDrawer = new SubConfigsEditorFacade<BaseCustomEditorInstaller>(serializedObject, serializedObject.FindProperty("_allAvailableInstallers"), "Инастраллеры");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _installersDrawer.OnInspectorGUI();
        }
    }
}