using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.OnOffSelection
{
    public class OnOffSelection
    {
        public bool IsSelected;
        private readonly string _label;

        public OnOffSelection(bool isSelected, string label = null)
        {
            IsSelected  = isSelected;
            _label = label;
        }

        public static bool DrawSelectionBtn(string text, bool isOn)
        {
            using (new GUILayout.HorizontalScope("Box"))
            {
                GUILayout.Label(text);
                return DrawSelectionBtn(isOn);
            }
        }


        public static bool DrawSelectionBtn(bool isselected, string onnText = "On", string offText = "Off",
                                            float width = 40)
        {
            var temp = GUI.backgroundColor;
            string leftBtntext = isselected ? onnText : "";
            string rigthBtntext = isselected ? "" : offText;
            GUI.backgroundColor = isselected ? Color.green : temp;
            isselected = GUILayout.Toggle(isselected, leftBtntext, EditorStyles.miniButtonLeft, GUILayout.Width(width));
            GUI.backgroundColor = isselected ? temp : Color.red;
            isselected = !GUILayout.Toggle(!isselected, rigthBtntext, EditorStyles.miniButtonRight,
                GUILayout.Width(width));
            GUI.backgroundColor = temp;
           
            return isselected;
        }
        
        public static bool DrawSelectionBtn(Rect rect,bool isselected, string onnText = "On", string offText = "Off")
        {
            var leftRec = new Rect(rect);
            leftRec.width  *= 0.5f;
            leftRec.height *= 0.8f;
            var rightRec = new Rect(leftRec) {position = new Vector2(leftRec.xMax, leftRec.y)};

            var temp = GUI.backgroundColor;
            string leftBtntext = isselected ? onnText : "";
            string rigthBtntext = isselected ? "" : offText;
            GUI.backgroundColor = isselected ? Color.green : temp;
            isselected          = GUI.Toggle(leftRec,isselected,leftBtntext, EditorStyles.miniButtonLeft);
            GUI.backgroundColor = isselected ? temp : Color.red;
            isselected          = !GUI.Toggle(rightRec,!isselected, rigthBtntext, EditorStyles.miniButtonRight);
            GUI.backgroundColor = temp;
           
            return isselected;
        }
        


        public bool DrawSelection(string onnText = "On", string offText = "Off", float width = 40)
        {
            using (new GUILayout.HorizontalScope())
            {
                if(!string.IsNullOrEmpty(_label))
                    GUILayout.Label(_label);
                EditorGUI.BeginChangeCheck();
                IsSelected = DrawSelectionBtn(IsSelected, onnText, offText, width);
                return EditorGUI.EndChangeCheck();    
            }
               
            
        }
    }
}