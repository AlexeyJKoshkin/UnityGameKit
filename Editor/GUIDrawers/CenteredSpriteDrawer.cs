using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    public class CenteredSpriteDrawer
    {
        private readonly GUIContent _foldoutContent;

        private static bool _show;

        public CenteredSpriteDrawer(string foldoutContent)
        {
            _foldoutContent = new GUIContent(foldoutContent);
            _show           = false;
        }


        public Sprite DrawWithFoldout(Sprite sprite, Vector2 size)
        {
            _show = EditorGUILayout.Foldout(_show, _foldoutContent);

            if (!_show) return sprite;

            DrawCenterBoxContent(size, sprite);

            return EditorGUILayout.ObjectField(sprite, typeof(Sprite), false) as Sprite;
        }

        public static Sprite Draw(Sprite sprite, Vector2 size)
        {
            DrawCenterBoxContent(size, sprite);

            return EditorGUILayout.ObjectField(sprite, typeof(Sprite), false) as Sprite;
        }

        public static void DrawCenterBoxContent(Vector2 size, Sprite sprite)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Box(GUIContent.none, GUILayout.Width(size.x), GUILayout.Height(size.y));

                var rect = GUILayoutUtility.GetLastRect();

                DrawTexturePreview(rect, sprite);

                GUILayout.FlexibleSpace();
            }
        }

        public static void DrawBoxContent(float size, Sprite sprite)
        {
            GUILayout.Box(GUIContent.none, GUILayout.Width(size), GUILayout.Height(size));

            var rect = GUILayoutUtility.GetLastRect();

            DrawTexturePreview(rect, sprite);
        }

        public static void DrawTexturePreview(Rect position, Sprite sprite)
        {
            if (sprite == null) return;
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size     = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.x      /= fullSize.x;
            coords.width  /= fullSize.x;
            coords.y      /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.width  = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
    }
}