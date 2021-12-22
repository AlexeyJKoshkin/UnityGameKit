// 25-02-2020 13:45
// /////

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
///     Продвинутый инспектор трансформа.
///     Должен работать только в редакторе.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(Transform), true)]
public class TransformInspector : Editor
{
    private static GUIStyle[] _axisStyle;

    private static bool ShowWorldPosition;
    private SerializedProperty mPos;
    private SerializedProperty mRot;
    private SerializedProperty mScale;

    private Transform CurrentTransform => target as Transform;

    private void OnEnable()
    {
        if (this)
        {
            try
            {
                var so = serializedObject;
                mPos   = so.FindProperty("m_LocalPosition");
                mRot   = so.FindProperty("m_LocalRotation");
                mScale = so.FindProperty("m_LocalScale");
            }
            catch { }
        }
    }

    /// <summary>
    ///     Draw the inspector widget.
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 15;

        serializedObject.Update();

        DrawPosition();
        DrawRotation(false);
        DrawScale(false);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPosition()
    {
        bool reset = false;
        using (new GUILayout.HorizontalScope())
        {
            reset             = GUILayout.Button("P", EditorStyles.miniButtonLeft, GUILayout.Width(20));
            ShowWorldPosition = GUILayout.Toggle(ShowWorldPosition, "W", EditorStyles.miniButtonRight, GUILayout.Width(22));

            if (ShowWorldPosition)
            {
                CurrentTransform.position = EditorGUILayout.Vector3Field(GUIContent.none, CurrentTransform.position);
            }
            else
            {
                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("x"));
                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("y"));
                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("z"));
            }
        }

        if (reset)
        {
            mPos.vector3Value = Vector3.zero;
        }
    }

    private void DrawProp(string s)
    {
        //  GUIStyle style = _showStepSettings ? _axisStyle[styleIndex] : GUIStyle.none;
        using (new GUILayout.VerticalScope()) { }
    }

    private void DrawScale(bool isWidget)
    {
        GUILayout.BeginHorizontal();
        {
            bool reset = GUILayout.Button("S", GUILayout.Width(20f));

            if (isWidget)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
            }

            EditorGUILayout.PropertyField(mScale.FindPropertyRelative("x"));
            EditorGUILayout.PropertyField(mScale.FindPropertyRelative("y"));
            EditorGUILayout.PropertyField(mScale.FindPropertyRelative("z"));
            if (isWidget)
            {
                GUI.color = Color.white;
            }

            if (reset)
            {
                mScale.vector3Value = Vector3.one;
            }
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    ///     Create an undo point for the specified objects.
    /// </summary>
    public static void RegisterUndo(string name, params Object[] objects)
    {
        if (objects != null && objects.Length > 0)
        {
            Undo.RecordObjects(objects, name);

            foreach (Object obj in objects)
            {
                if (obj == null)
                {
                    continue;
                }

                EditorUtility.SetDirty(obj);
            }
        }
    }

    public static float WrapAngle(float angle)
    {
        while (angle > 180f)
        {
            angle -= 360f;
        }

        while (angle < -180f)
        {
            angle += 360f;
        }

        return angle;
    }

#region Rotation is ugly as hell... since there is no native support for quaternion property drawing

    private enum Axes
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        All = 7
    }

    private Axes CheckDifference(Transform t, Vector3 original)
    {
        Vector3 next = t.localEulerAngles;

        Axes axes = Axes.None;

        if (Differs(next.x, original.x))
        {
            axes |= Axes.X;
        }

        if (Differs(next.y, original.y))
        {
            axes |= Axes.Y;
        }

        if (Differs(next.z, original.z))
        {
            axes |= Axes.Z;
        }

        return axes;
    }

    private Axes CheckDifference(SerializedProperty property)
    {
        Axes axes = Axes.None;

        if (property.hasMultipleDifferentValues)
        {
            Vector3 original = property.quaternionValue.eulerAngles;

            foreach (Object obj in serializedObject.targetObjects)
            {
                axes |= CheckDifference(obj as Transform, original);
                if (axes == Axes.All)
                {
                    break;
                }
            }
        }

        return axes;
    }

    /// <summary>
    ///     Draw an editable float field.
    /// </summary>
    /// <param name="hidden">Whether to replace the value with a dash</param>
    /// <param name="greyedOut">Whether the value should be greyed out or not</param>
    private static bool FloatField(string name, ref float value, bool hidden, bool greyedOut, GUILayoutOption opt)
    {
        float newValue = value;
        GUI.changed = false;

        if (!hidden)
        {
            if (greyedOut)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                newValue  = EditorGUILayout.FloatField(name, newValue, opt);
                GUI.color = Color.white;
            }
            else
            {
                newValue = EditorGUILayout.FloatField(name, newValue, opt);
            }
        }
        else if (greyedOut)
        {
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
            GUI.color = Color.white;
        }
        else
        {
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
        }

        if (GUI.changed && Differs(newValue, value))
        {
            value = newValue;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Because Mathf.Approximately is too sensitive.
    /// </summary>
    private static bool Differs(float a, float b)
    {
        return Mathf.Abs(a - b) > 0.0001f;
    }

    private void DrawRotation(bool isWidget)
    {
        using (new GUILayout.HorizontalScope())
        {
            bool reset = GUILayout.Button("R", GUILayout.Width(20f));

            Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;

            visible.x = WrapAngle(visible.x);
            visible.y = WrapAngle(visible.y);
            visible.z = WrapAngle(visible.z);

            Axes changed = CheckDifference(mRot);
            Axes altered = Axes.None;

            GUILayoutOption opt = GUILayout.MinWidth(30f);

            if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, isWidget, opt))
            {
                altered |= Axes.X;
            }

            if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, isWidget, opt))
            {
                altered |= Axes.Y;
            }

            if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, false, opt))
            {
                altered |= Axes.Z;
            }

            if (reset)
            {
                mRot.quaternionValue = Quaternion.identity;
            }
            else if (altered != Axes.None)
            {
                RegisterUndo("Change Rotation", serializedObject.targetObjects);

                foreach (Object obj in serializedObject.targetObjects)
                {
                    Transform t = obj as Transform;
                    Vector3   v = t.localEulerAngles;

                    if ((altered & Axes.X) != 0)
                    {
                        v.x = visible.x;
                    }

                    if ((altered & Axes.Y) != 0)
                    {
                        v.y = visible.y;
                    }

                    if ((altered & Axes.Z) != 0)
                    {
                        v.z = visible.z;
                    }

                    t.localEulerAngles = v;
                }
            }
        }
    }

#endregion
}
#endif