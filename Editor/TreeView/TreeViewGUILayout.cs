using UnityEditor;
using UnityEngine;

namespace GameKit.TreeView
{
    public class TreeViewGUILayout
    {
        public delegate bool CheckCanDo(ITreeViewItem item);


        public delegate void DrawTreeItemDelegate(ITreeViewItem item);

        private const int WIDTH_CONTROLL_ELEMENT = 15;
        private const int INDENT_SPACE = 10;
        private float _space;
        public CheckCanDo CheckCanEditCheckBox;


        public DrawTreeItemDelegate DrawItemCallback;

        public void DoLayout(ITreeViewItem root)
        {
            if (root == null) return;
            _space = 0;
            using (new GUILayout.VerticalScope("Box"))
            {
                DrawViewItem(root);
            }
        }

        private void DrawViewItem(ITreeViewItem treeLeaf)
        {
            if (treeLeaf == null) return;
            using (new GUILayout.HorizontalScope())
            {
                DrawLeafLine(treeLeaf);
            }

            if (treeLeaf.IsExpanded)
                using (new EditorGUI.DisabledScope(!treeLeaf.IsChecked))
                {
                    DrawChildren(treeLeaf);
                }
        }

        private void DrawChildren(ITreeViewItem treeLeafItems)
        {
            foreach (var leaf in treeLeafItems)
            {
                _space += INDENT_SPACE;
                DrawViewItem(leaf);
                _space -= INDENT_SPACE;
            }
        }

        private void DrawLeafLine(ITreeViewItem treeLeaf)
        {
            GUILayout.Space(_space);
            DrawCheckBox(treeLeaf);
            DrawExpand(treeLeaf);
            DrawHeader(treeLeaf);
        }

        private void DrawHeader(ITreeViewItem treeLeaf)
        {
            if (DrawItemCallback == null)
                EditorGUILayout.LabelField(treeLeaf.Header);
            else
                DrawItemCallback(treeLeaf);
        }

        private void DrawExpand(ITreeViewItem treeLeaf)
        {
            if (treeLeaf.ChildCount > 0)
                treeLeaf.IsExpanded = EditorGUILayout.Toggle(treeLeaf.IsExpanded, EditorStyles.foldout,
                    GUILayout.Width(WIDTH_CONTROLL_ELEMENT));
        }

        private void DrawCheckBox(ITreeViewItem treeLeaf)
        {
            bool canEditCheckbox = CheckCanEditCheckBox?.Invoke(treeLeaf) ?? true;
            using (new EditorGUI.DisabledScope(!canEditCheckbox))
            {
                treeLeaf.IsChecked =
                    EditorGUILayout.Toggle(treeLeaf.IsChecked, GUILayout.Width(WIDTH_CONTROLL_ELEMENT));
            }
        }

        public void DoLayoutStartChildren(ITreeViewItem root)
        {
            if (root == null) return;
            _space = 0;
            using (new GUILayout.VerticalScope("Box"))
            {
                foreach (var leaf in root)
                {
                    _space += INDENT_SPACE;
                    DrawViewItem(leaf);
                    _space -= INDENT_SPACE;
                }
            }
        }
    }
}