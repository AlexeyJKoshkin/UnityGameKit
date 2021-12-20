using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Editor
{
    public class CustomReorderableList
    {
        public delegate void AddCallbackDelegate(CustomReorderableList list);

        public delegate void AddDropdownCallbackDelegate(Rect buttonRect, CustomReorderableList list);

        public delegate bool CanAddCallbackDelegate(CustomReorderableList list);

        public delegate bool CanRemoveCallbackDelegate(CustomReorderableList list);

        public delegate void ChangedCallbackDelegate(CustomReorderableList list);

        public delegate void DragCallbackDelegate(CustomReorderableList list);

        public delegate void DrawNoneElementCallback(Rect rect);

        public delegate void ElementCallbackDelegate(
            Rect rect,
            int index,
            bool isActive,
            bool isFocused);

        public delegate float ElementHeightCallbackDelegate(int index);

        public delegate void FooterCallbackDelegate(Rect rect);

        public delegate void HeaderCallbackDelegate(Rect rect);

        public delegate void RemoveCallbackDelegate(CustomReorderableList list);

        public delegate void ReorderCallbackDelegate(CustomReorderableList list);

        public delegate void ReorderCallbackDelegateWithDetails(
            CustomReorderableList list,
            int oldIndex,
            int newIndex);

        public delegate void SelectCallbackDelegate(CustomReorderableList list);

        private static Defaults s_Defaults;
        public bool displayAdd;
        public bool displayRemove;
        public ElementCallbackDelegate drawElementBackgroundCallback;
        public ElementCallbackDelegate drawElementCallback;
        public FooterCallbackDelegate drawFooterCallback = null;
        public HeaderCallbackDelegate drawHeaderCallback;
        public DrawNoneElementCallback drawNoneElementCallback = null;
        public float elementHeight = 21f;
        public ElementHeightCallbackDelegate elementHeightCallback;
        public float footerHeight = 13f;
        public float headerHeight = 18f;
        private int id = -1;
        private int m_ActiveElement = -1;
        private bool m_DisplayHeader;
        private bool m_Draggable;
        private float m_DraggedY;
        private bool m_Dragging;
        private float m_DragOffset;
        private IList m_ElementList;
        private SerializedProperty m_Elements;
        private List<int> m_NonDragTargetIndices;

        private SerializedObject m_SerializedObject;
        public AddCallbackDelegate onAddCallback;
        public AddDropdownCallbackDelegate onAddDropdownCallback;
        public CanAddCallbackDelegate onCanAddCallback;
        public CanRemoveCallbackDelegate onCanRemoveCallback;
        public ChangedCallbackDelegate onChangedCallback;
        public DragCallbackDelegate onMouseDragCallback;
        public SelectCallbackDelegate onMouseUpCallback;
        public RemoveCallbackDelegate onRemoveCallback;
        public ReorderCallbackDelegate onReorderCallback;
        public ReorderCallbackDelegateWithDetails onReorderCallbackWithDetails;
        public SelectCallbackDelegate onSelectCallback;
        public bool showDefaultBackground = true;

        public CustomReorderableList(IList elements, Type elementType)
        {
            InitList(null, null, elements, true, true, true, true);
        }

        public CustomReorderableList(
            IList elements,
            Type elementType,
            bool draggable,
            bool displayHeader,
            bool displayAddButton,
            bool displayRemoveButton)
        {
            InitList(null, null, elements, draggable, displayHeader, displayAddButton, displayRemoveButton);
        }

        public CustomReorderableList(SerializedObject serializedObject, SerializedProperty elements)
        {
            InitList(serializedObject, elements, null, true, true, true, true);
        }

        public CustomReorderableList(
            SerializedObject serializedObject,
            SerializedProperty elements,
            bool draggable,
            bool displayHeader,
            bool displayAddButton,
            bool displayRemoveButton)
        {
            InitList(serializedObject, elements, null, draggable, displayHeader, displayAddButton, displayRemoveButton);
        }

        public static Defaults defaultBehaviours => s_Defaults;

        public SerializedProperty serializedProperty
        {
            get => m_Elements;
            set => m_Elements = value;
        }

        public IList list
        {
            get => m_ElementList;
            set => m_ElementList = value;
        }

        public int index
        {
            get => m_ActiveElement;
            set => m_ActiveElement = value;
        }

        public bool draggable
        {
            get => m_Draggable;
            set => m_Draggable = value;
        }

        public int count
        {
            get
            {
                if (m_Elements == null)
                    return m_ElementList.Count;
                if (!m_Elements.hasMultipleDifferentValues)
                    return m_Elements.arraySize;
                int val2 = m_Elements.arraySize;
                foreach (Object targetObject in m_Elements.serializedObject.targetObjects)
                    val2 = Math.Min(new SerializedObject(targetObject).FindProperty(m_Elements.propertyPath).arraySize,
                        val2);
                return val2;
            }
        }

        private void InitList(
            SerializedObject serializedObject,
            SerializedProperty elements,
            IList elementList,
            bool draggable,
            bool displayHeader,
            bool displayAddButton,
            bool displayRemoveButton)
        {
            id                 = GUIUtility.GetControlID(FocusType.Keyboard);
            m_SerializedObject = serializedObject;
            m_Elements         = elements;
            m_ElementList      = elementList;
            m_Draggable        = draggable;
            m_Dragging         = false;
            displayAdd         = displayAddButton;
            m_DisplayHeader    = displayHeader;
            displayRemove      = displayRemoveButton;
            if (m_Elements != null && !m_Elements.editable)
                m_Draggable = false;
            if (m_Elements == null || m_Elements.isArray)
                return;
            Debug.LogError("Input elements should be an Array SerializedProperty");
        }

        private Rect GetContentRect(Rect rect)
        {
            Rect rect1 = rect;
            if (draggable)
                rect1.xMin += 20f;
            else
                rect1.xMin += 6f;
            rect1.xMax -= 6f;
            return rect1;
        }

        private float GetElementYOffset(int index)
        {
            return GetElementYOffset(index, -1);
        }

        private float GetElementYOffset(int index, int skipIndex)
        {
            if (elementHeightCallback == null)
                return index * elementHeight;
            float num = 0.0f;
            for (int index1 = 0; index1 < index; ++index1)
                if (index1 != skipIndex)
                    num += elementHeightCallback(index1);
            return num;
        }

        private float GetElementHeight(int index)
        {
            if (elementHeightCallback == null)
                return elementHeight;
            return elementHeightCallback(index);
        }

        private Rect GetRowRect(int index, Rect listRect)
        {
            return new Rect(listRect.x, listRect.y + GetElementYOffset(index), listRect.width, GetElementHeight(index));
        }

        public void DoLayoutList()
        {
            if (s_Defaults == null)
                s_Defaults = new Defaults();
            Rect rect1 = GUILayoutUtility.GetRect(0.0f, headerHeight, GUILayout.ExpandWidth(true));
            Rect rect2 = GUILayoutUtility.GetRect(10f, GetListElementHeight(), GUILayout.ExpandWidth(true));
            Rect rect3 = GUILayoutUtility.GetRect(4f, footerHeight, GUILayout.ExpandWidth(true));
            DoListHeader(rect1);
            DoListElements(rect2);
            DoListFooter(rect3);
        }

        public void DoList(Rect rect)
        {
            if (s_Defaults == null)
                s_Defaults = new Defaults();
            Rect headerRect = new Rect(rect.x, rect.y, rect.width, headerHeight);
            Rect listRect   = new Rect(rect.x, headerRect.y + headerRect.height, rect.width, GetListElementHeight());
            Rect footerRect = new Rect(rect.x, listRect.y + listRect.height, rect.width, footerHeight);
            DoListHeader(headerRect);
            DoListElements(listRect);
            DoListFooter(footerRect);
        }

        public float GetHeight()
        {
            return 0.0f + GetListElementHeight() + headerHeight + footerHeight;
        }

        private float GetListElementHeight()
        {
            int count = this.count;
            if (count == 0)
                return elementHeight + 7f;
            if (elementHeightCallback != null)
                return (float) (GetElementYOffset(count - 1) + (double) GetElementHeight(count - 1) + 7.0);
            return (float) (elementHeight * (double) count + 7.0);
        }

        private void DoListElements(Rect listRect)
        {
            int count = this.count;
            if (showDefaultBackground && Event.current.type == EventType.Repaint)
                s_Defaults.boxBackground.Draw(listRect, false, false, false, false);
            listRect.yMin += 2f;
            listRect.yMax -= 5f;
            Rect rect1 = listRect;
            rect1.height = elementHeight;
            if ((m_Elements != null && m_Elements.isArray || m_ElementList != null) && count > 0)
            {
                if (IsDragging() && Event.current.type == EventType.Repaint)
                {
                    int rowIndex = CalculateRowIndex();
                    m_NonDragTargetIndices.Clear();
                    for (int index = 0; index < count; ++index)
                        if (index != m_ActiveElement)
                            m_NonDragTargetIndices.Add(index);
                    m_NonDragTargetIndices.Insert(rowIndex, -1);
                    bool flag = false;
                    for (int index = 0; index < m_NonDragTargetIndices.Count; ++index)
                        if (m_NonDragTargetIndices[index] != -1)
                        {
                            rect1.height = GetElementHeight(index);
                            if (elementHeightCallback == null)
                            {
                                rect1.y = listRect.y + GetElementYOffset(index, m_ActiveElement);
                            }
                            else
                            {
                                rect1.y = listRect.y +
                                          GetElementYOffset(m_NonDragTargetIndices[index], m_ActiveElement);
                                if (flag)
                                    rect1.y += elementHeightCallback(m_ActiveElement);
                            }

                            //rect1 = this.m_SlideGroup.GetRect(this.m_NonDragTargetIndices[index], rect1);
                            if (drawElementBackgroundCallback == null)
                                s_Defaults.DrawElementBackground(rect1, index, false, false, m_Draggable);
                            else
                                drawElementBackgroundCallback(rect1, index, false, false);
                            s_Defaults.DrawElementDraggingHandle(rect1, index, false, false, m_Draggable);
                            Rect contentRect = GetContentRect(rect1);
                            if (drawElementCallback == null)
                            {
                                if (m_Elements != null)
                                    s_Defaults.DrawElement(contentRect,
                                        m_Elements.GetArrayElementAtIndex(m_NonDragTargetIndices[index]), null, false,
                                        false, m_Draggable);
                                else
                                    s_Defaults.DrawElement(contentRect, null,
                                        m_ElementList[m_NonDragTargetIndices[index]], false, false, m_Draggable);
                            }
                            else
                            {
                                drawElementCallback(contentRect, m_NonDragTargetIndices[index], false, false);
                            }
                        }
                        else
                        {
                            flag = true;
                        }

                    rect1.y = m_DraggedY - m_DragOffset + listRect.y;
                    if (drawElementBackgroundCallback == null)
                        s_Defaults.DrawElementBackground(rect1, m_ActiveElement, true, true, m_Draggable);
                    else
                        drawElementBackgroundCallback(rect1, m_ActiveElement, true, true);
                    s_Defaults.DrawElementDraggingHandle(rect1, m_ActiveElement, true, true, m_Draggable);
                    Rect contentRect1 = GetContentRect(rect1);
                    if (drawElementCallback == null)
                    {
                        if (m_Elements != null)
                            s_Defaults.DrawElement(contentRect1, m_Elements.GetArrayElementAtIndex(m_ActiveElement),
                                null, true, true, m_Draggable);
                        else
                            s_Defaults.DrawElement(contentRect1, null, m_ElementList[m_ActiveElement], true, true,
                                m_Draggable);
                    }
                    else
                    {
                        drawElementCallback(contentRect1, m_ActiveElement, true, true);
                    }
                }
                else
                {
                    for (int index = 0; index < count; ++index)
                    {
                        bool flag1 = index == m_ActiveElement;
                        bool flag2 = index == m_ActiveElement && HasKeyboardControl();
                        rect1.height = GetElementHeight(index);
                        rect1.y      = listRect.y + GetElementYOffset(index);
                        if (drawElementBackgroundCallback == null)
                            s_Defaults.DrawElementBackground(rect1, index, flag1, flag2, m_Draggable);
                        else
                            drawElementBackgroundCallback(rect1, index, flag1, flag2);
                        s_Defaults.DrawElementDraggingHandle(rect1, index, flag1, flag2, m_Draggable);
                        Rect contentRect = GetContentRect(rect1);
                        if (drawElementCallback == null)
                        {
                            if (m_Elements != null)
                                s_Defaults.DrawElement(contentRect, m_Elements.GetArrayElementAtIndex(index), null,
                                    flag1, flag2, m_Draggable);
                            else
                                s_Defaults.DrawElement(contentRect, null, m_ElementList[index], flag1, flag2,
                                    m_Draggable);
                        }
                        else
                        {
                            drawElementCallback(contentRect, index, flag1, flag2);
                        }
                    }
                }

                DoDraggingAndSelection(listRect);
            }
            else
            {
                rect1.y = listRect.y;
                if (drawElementBackgroundCallback == null)
                    s_Defaults.DrawElementBackground(rect1, -1, false, false, false);
                else
                    drawElementBackgroundCallback(rect1, -1, false, false);
                s_Defaults.DrawElementDraggingHandle(rect1, -1, false, false, false);
                Rect rect2 = rect1;
                rect2.xMin += 6f;
                rect2.xMax -= 6f;
                if (drawNoneElementCallback == null)
                    s_Defaults.DrawNoneElement(rect2, m_Draggable);
                else
                    drawNoneElementCallback(rect2);
            }
        }

        private void DoListHeader(Rect headerRect)
        {
            if (showDefaultBackground && Event.current.type == EventType.Repaint)
                s_Defaults.DrawHeaderBackground(headerRect);
            headerRect.xMin   += 6f;
            headerRect.xMax   -= 6f;
            headerRect.height -= 2f;
            ++headerRect.y;
            if (drawHeaderCallback != null)
            {
                drawHeaderCallback(headerRect);
            }
            else
            {
                if (!m_DisplayHeader)
                    return;
                s_Defaults.DrawHeader(headerRect, m_SerializedObject, m_Elements, m_ElementList);
            }
        }

        private void DoListFooter(Rect footerRect)
        {
            if (drawFooterCallback != null)
            {
                drawFooterCallback(footerRect);
            }
            else
            {
                if (!displayAdd && !displayRemove)
                    return;
                s_Defaults.DrawFooter(footerRect, this);
            }
        }

        private void DoDraggingAndSelection(Rect listRect)
        {
            Event current        = Event.current;
            int   activeElement1 = m_ActiveElement;
            bool  flag           = false;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (listRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        EditorGUIUtility.editingTextField = false;
                        m_ActiveElement                   = GetRowIndex(Event.current.mousePosition.y - listRect.y);
                        if (m_Draggable)
                        {
                            m_DragOffset = Event.current.mousePosition.y - listRect.y -
                                           GetElementYOffset(m_ActiveElement);
                            UpdateDraggedY(listRect);
                            GUIUtility.hotControl = id;
                            //   this.m_SlideGroup.Reset();
                            m_NonDragTargetIndices = new List<int>();
                        }

                        GrabKeyboardFocus();
                        current.Use();
                        flag = true;
                    }

                    break;
                case EventType.MouseUp:
                    if (!m_Draggable)
                    {
                        if (onMouseUpCallback != null && IsMouseInsideActiveElement(listRect)) onMouseUpCallback(this);
                        break;
                    }

                    if (GUIUtility.hotControl == id)
                    {
                        current.Use();
                        m_Dragging = false;
                        try
                        {
                            int rowIndex = CalculateRowIndex();
                            if (m_ActiveElement != rowIndex)
                            {
                                if (m_SerializedObject != null && m_Elements != null)
                                {
                                    m_Elements.MoveArrayElement(m_ActiveElement, rowIndex);
                                    m_SerializedObject.ApplyModifiedProperties();
                                    m_SerializedObject.Update();
                                }
                                else if (m_ElementList != null)
                                {
                                    object element = m_ElementList[m_ActiveElement];
                                    for (int index = 0; index < m_ElementList.Count - 1; ++index)
                                        if (index >= m_ActiveElement)
                                            m_ElementList[index] = m_ElementList[index + 1];
                                    for (int index = m_ElementList.Count - 1; index > 0; --index)
                                        if (index > rowIndex)
                                            m_ElementList[index] = m_ElementList[index - 1];
                                    m_ElementList[rowIndex] = element;
                                }

                                int activeElement2 = m_ActiveElement;
                                int newIndex       = rowIndex;
                                m_ActiveElement = rowIndex;
                                if (onReorderCallbackWithDetails != null)
                                    onReorderCallbackWithDetails(this, activeElement2, newIndex);
                                else
                                    onReorderCallback?.Invoke(this);

                                onChangedCallback?.Invoke(this);
                                break;
                            }

                            onMouseUpCallback?.Invoke(this);
                            break;
                        }
                        finally
                        {
                            GUIUtility.hotControl  = 0;
                            m_NonDragTargetIndices = null;
                        }
                    }
                    else
                    {
                        break;
                    }
                case EventType.MouseDrag:
                    if (m_Draggable && GUIUtility.hotControl == id)
                    {
                        m_Dragging = true;
                        onMouseDragCallback?.Invoke(this);
                        UpdateDraggedY(listRect);
                        current.Use();
                    }

                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl != id)
                        return;
                    if (current.keyCode == KeyCode.DownArrow)
                    {
                        ++m_ActiveElement;
                        current.Use();
                    }

                    if (current.keyCode == KeyCode.UpArrow)
                    {
                        --m_ActiveElement;
                        current.Use();
                    }

                    if (current.keyCode == KeyCode.Escape && GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        m_Dragging            = false;
                        current.Use();
                    }

                    m_ActiveElement = Mathf.Clamp(m_ActiveElement, 0,
                        m_Elements == null ? m_ElementList.Count - 1 : m_Elements.arraySize - 1);
                    break;
            }

            if (m_ActiveElement == activeElement1 && !flag || onSelectCallback == null)
                return;
            onSelectCallback(this);
        }

        private bool IsMouseInsideActiveElement(Rect listRect)
        {
            int rowIndex = GetRowIndex(Event.current.mousePosition.y - listRect.y);
            return rowIndex == m_ActiveElement && GetRowRect(rowIndex, listRect).Contains(Event.current.mousePosition);
        }

        private void UpdateDraggedY(Rect listRect)
        {
            m_DraggedY = Mathf.Clamp(Event.current.mousePosition.y - listRect.y, m_DragOffset,
                listRect.height - (GetElementHeight(m_ActiveElement) - m_DragOffset));
        }

        private int CalculateRowIndex()
        {
            return GetRowIndex(m_DraggedY);
        }

        private int GetRowIndex(float localY)
        {
            if (elementHeightCallback == null)
                return Mathf.Clamp(Mathf.FloorToInt(localY / elementHeight), 0, count - 1);
            float num1 = 0.0f;
            for (int index = 0; index < count; ++index)
            {
                float num2 = elementHeightCallback(index);
                float num3 = num1 + num2;
                if (localY >= (double) num1 && localY < (double) num3)
                    return index;
                num1 += num2;
            }

            return count - 1;
        }

        private bool IsDragging()
        {
            return m_Dragging;
        }

        public void GrabKeyboardFocus()
        {
            GUIUtility.keyboardControl = id;
        }

        public void ReleaseKeyboardFocus()
        {
            if (GUIUtility.keyboardControl != id)
                return;
            GUIUtility.keyboardControl = 0;
        }

        public bool HasKeyboardControl()
        {
            return GUIUtility.keyboardControl == id;
        }

        public class Defaults
        {
            public const int padding = 6;
            public const int dragHandleWidth = 20;
            private static GUIContent s_ListIsEmpty = EditorGUIUtility.TrTextContent("List is Empty");
            public readonly GUIStyle boxBackground = "RL Background";
            public readonly GUIStyle draggingHandle = "RL DragHandle";
            public readonly GUIStyle elementBackground = "RL Element";
            public readonly GUIStyle footerBackground = "RL Footer";
            public readonly GUIStyle headerBackground = "RL Header";
            public readonly GUIStyle preButton = "RL FooterButton";

            public GUIContent iconToolbarMinus =
                EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");

            public GUIContent iconToolbarPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");

            public GUIContent iconToolbarPlusMore =
                EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");

            public void DrawFooter(Rect rect, CustomReorderableList list)
            {
                float xMax = rect.xMax;
                float x    = xMax - 8f;
                if (list.displayAdd)
                    x -= 25f;
                if (list.displayRemove)
                    x -= 25f;
                rect = new Rect(x, rect.y, xMax - x, rect.height);
                Rect rect1    = new Rect(x + 4f, rect.y - 3f, 25f, 13f);
                Rect position = new Rect(xMax - 29f, rect.y - 3f, 25f, 13f);
                if (Event.current.type == EventType.Repaint)
                    footerBackground.Draw(rect, false, false, false, false);
                if (list.displayAdd)
                    using (new EditorGUI.DisabledScope(list.onCanAddCallback != null && !list.onCanAddCallback(list)))
                    {
                        if (GUI.Button(rect1,
                            list.onAddDropdownCallback == null ? iconToolbarPlus : iconToolbarPlusMore, preButton))
                        {
                            if (list.onAddDropdownCallback != null)
                                list.onAddDropdownCallback(rect1, list);
                            else if (list.onAddCallback != null)
                                list.onAddCallback(list);
                            else
                                DoAddButton(list);
                            if (list.onChangedCallback != null)
                                list.onChangedCallback(list);
                        }
                    }

                if (!list.displayRemove)
                    return;
                using (new EditorGUI.DisabledScope(list.index < 0 || list.index >= list.count ||
                                                   list.onCanRemoveCallback != null && !list.onCanRemoveCallback(list)))
                {
                    if (GUI.Button(position, iconToolbarMinus, preButton))
                    {
                        if (list.onRemoveCallback == null)
                            DoRemoveButton(list);
                        else
                            list.onRemoveCallback(list);
                        if (list.onChangedCallback != null)
                            list.onChangedCallback(list);
                    }
                }
            }

            public void DoAddButton(CustomReorderableList list)
            {
                if (list.serializedProperty != null)
                {
                    ++list.serializedProperty.arraySize;
                    list.index = list.serializedProperty.arraySize - 1;
                }
                else
                {
                    Type elementType = list.list.GetType().GetElementType();
                    if (elementType == typeof(string))
                        list.index = list.list.Add("");
                    else if (elementType != null && elementType.GetConstructor(Type.EmptyTypes) == null)
                        Debug.LogError("Cannot add element. Type " + elementType +
                                       " has no default constructor. Implement a default constructor or implement your own add behaviour.");
                    else if (list.list.GetType().GetGenericArguments()[0] != null)
                        list.index =
                            list.list.Add(Activator.CreateInstance(list.list.GetType().GetGenericArguments()[0]));
                    else if (elementType != null)
                        list.index = list.list.Add(Activator.CreateInstance(elementType));
                    else
                        Debug.LogError("Cannot add element of type Null.");
                }
            }

            public void DoRemoveButton(CustomReorderableList list)
            {
                if (list.serializedProperty != null)
                {
                    list.serializedProperty.DeleteArrayElementAtIndex(list.index);
                    if (list.index < list.serializedProperty.arraySize - 1)
                        return;
                    list.index = list.serializedProperty.arraySize - 1;
                }
                else
                {
                    list.list.RemoveAt(list.index);
                    if (list.index >= list.list.Count - 1)
                        list.index = list.list.Count - 1;
                }
            }

            public void DrawHeaderBackground(Rect headerRect)
            {
                if (Event.current.type != EventType.Repaint)
                    return;
                headerBackground.Draw(headerRect, false, false, false, false);
            }

            public void DrawHeader(
                Rect headerRect,
                SerializedObject serializedObject,
                SerializedProperty element,
                IList elementList)
            {
                EditorGUI.LabelField(headerRect,
                    EditorGUIUtility.TrTempContent(element == null ? "IList" : "Serialized Property"));
            }

            public void DrawElementBackground(
                Rect rect,
                int index,
                bool selected,
                bool focused,
                bool draggable)
            {
                if (Event.current.type != EventType.Repaint)
                    return;
                elementBackground.Draw(rect, false, selected, selected, focused);
            }

            public void DrawElementDraggingHandle(
                Rect rect,
                int index,
                bool selected,
                bool focused,
                bool draggable)
            {
                if (Event.current.type != EventType.Repaint || !draggable)
                    return;
                draggingHandle.Draw(new Rect(rect.x + 5f, rect.y + 7f, 10f, rect.height - (rect.height - 7f)), false,
                    false, false, false);
            }

            public void DrawElement(
                Rect rect,
                SerializedProperty element,
                object listItem,
                bool selected,
                bool focused,
                bool draggable)
            {
                EditorGUI.LabelField(rect,
                    EditorGUIUtility.TrTempContent(element == null ? listItem.ToString() : element.displayName));
            }

            public void DrawNoneElement(Rect rect, bool draggable)
            {
                EditorGUI.LabelField(rect, s_ListIsEmpty);
            }
        }
    }
}