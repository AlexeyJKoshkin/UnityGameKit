using UnityEditor;

namespace GameKit.SceneViewWindow
{
    [CustomEditor(typeof(SceneViewWindowManager))]
    public class SceneViewWindowManagerInspector : UnityEditor.Editor
    {
        private SceneViewWindowManager Manager => target as SceneViewWindowManager;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}