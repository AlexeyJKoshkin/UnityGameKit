# if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColoredStyles
{
    private List<Color> _allColors;

    public ColoredStyles(int count)
    {
        Recreate(count);
    }

    public int Count => _allColors.Count;

    public GUIStyle GetNextStyle(float a = 0.3f)
    {
        var color = GetNextColor();
        color.a = a;
        return GetStyle(color);
    }


    public void Recreate(int count)
    {
        _allColors = new List<Color>(count);
        for (var i = 0; i < count; i++)
        {
            var hue            = i / (float) count;
            var componentColor = Color.HSVToRGB(hue, 0.7f, 1f);
            componentColor.a = 0.7f;
            _allColors.Add(componentColor);
        }
    }

    public Color GetNextColor(float alfa = 0.7f)
    {
        return GetColorIndex(0, alfa);
    }


    public Color GetColorIndex(int index, float alfa = 0.7f, bool settoend = true)
    {
        if (index < 0 || index >= _allColors.Count)
        {
            Debug.LogError(index);
            index = 0;
        }

        var res = _allColors[index];
        if (settoend)
        {
            _allColors.RemoveAt(index);
            _allColors.Add(res);
        }

        res.a = alfa;
        return res;
    }
    
    
    public static Texture2D CreateTexture(int width, int height, Color color)
    {
        var pixels = new Color[width * height];
        for (var i = 0; i < pixels.Length; ++i)
            pixels[i] = color;
        var result = new Texture2D(width, height);
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }

    public static GUIStyle GetStyle(Color color)
    {
        var style = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box;
        return new GUIStyle(style) {normal = {background = CreateTexture(2, 2, color)}};
    }
}
#endif