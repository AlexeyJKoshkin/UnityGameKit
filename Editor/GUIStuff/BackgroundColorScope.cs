using UnityEngine;

namespace GameKit.Editor {
    public class BackgroundColorScope : GUI.Scope
    {
        private readonly Color _originColor;

        public BackgroundColorScope(Color color)
        {
            _originColor        = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        protected override void CloseScope()
        {
            GUI.backgroundColor = _originColor;
        }
    }
}