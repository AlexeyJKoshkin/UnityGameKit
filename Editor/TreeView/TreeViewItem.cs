using System;
using System.Collections.Generic;

namespace GameKit.TreeView
{
    public interface ITreeViewItem : IEnumerable<ITreeViewItem>
    {
        bool IsChecked { get; set; }
        bool IsExpanded { get; set; }
        int ChildCount { get; }
        string Header { get; }

        void UpdateStateByContext(bool recursive);
    }

    [Serializable]
    public abstract class TreeViewItem
    {
        private ItemNodeData _nodeData = new ItemNodeData {IsChecked = true};

        public string Header { get; protected set; }

        public abstract int ChildCount { get; }
        public abstract TreeViewItem this[int index] { get; }

        public virtual bool IsChecked
        {
            get => _nodeData.IsChecked;
            set => _nodeData.IsChecked = value;
        }

        public virtual bool IsExpanded
        {
            get => _nodeData.IsExpanded;
            set => _nodeData.IsExpanded = value;
        }

        [Serializable]
        public class ItemNodeData
        {
            public bool IsExpanded = true;
            public bool IsChecked;
        }
    }
}