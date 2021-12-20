using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public static class CustomEditorStyles
    {
        static CustomEditorStyles()
        {
            if (CustomEditorSkin == null)
                CustomEditorSkin = EditorUtils.GetSkinByName("KitchenEditorSkin");

            CenteredLabel = CustomEditorSkin.GetStyle("centeredLabel");
            LeftPanel     = CustomEditorSkin.GetStyle("leftPanel");
            MainPanel     = CustomEditorSkin.GetStyle("mainBackground");
            BtnPanel      = CustomEditorSkin.GetStyle("BtnUpPanel");
            DownPanel     = CustomEditorSkin.GetStyle("DownArea");

            var blank = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("label");
            RichTextLabel = new GUIStyle(blank) {richText = true, wordWrap = true};

            blank         = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("boldLabel");
            BoldCentered  = new GUIStyle(blank) {alignment = TextAnchor.MiddleCenter};
            ToolBarEditor = CustomEditorSkin.GetStyle("toolbarButton");

            InfoIcon    = EditorGUIUtility.FindTexture("console.infoicon");
            WarningIcon = EditorGUIUtility.FindTexture("console.warnicon");
            ErrorIcon   = EditorGUIUtility.FindTexture("console.erroricon");

            var foldOut = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("Foldout");

            LeftTextButton = new GUIStyle(EditorStyles.miniButton) {alignment = TextAnchor.MiddleLeft};

            BoldBigBtn = new GUIStyle("Button") {fontStyle = FontStyle.Bold};


            WhiteFoldout = new GUIStyle(foldOut)
            {
                active    = {textColor = Color.white},
                normal    = {textColor = Color.white},
                focused   = {textColor = Color.white},
                onActive  = {textColor = Color.white},
                onFocused = {textColor = Color.white},
                onHover   = {textColor = Color.white},
                hover     = {textColor = Color.white},
                onNormal  = {textColor = Color.white}
            };
        }

        public static GUIStyle BoldBigBtn { get; }

        public static GUIStyle WhiteFoldout { get; }

        public static GUIStyle CenteredLabel { get; }

        public static GUIStyle RichTextLabel { get; }

        public static GUIStyle LeftPanel { get; }
        public static GUIStyle MainPanel { get; }
        public static GUIStyle BtnPanel { get; }
        public static GUIStyle DownPanel { get; }

        public static GUIStyle ToolBarEditor { get; }

        public static GUIStyle box => CustomEditorSkin.box;

        public static GUIStyle LeftTextButton { get; }

        public static GUIStyle BoldCentered { get; }

        public static Texture2D InfoIcon { get; }

        public static Texture2D WarningIcon { get; }

        public static Texture2D ErrorIcon { get; }

        /// <summary>
        ///     Скин
        /// </summary>
        public static GUISkin CustomEditorSkin { get; }

      
    }
}