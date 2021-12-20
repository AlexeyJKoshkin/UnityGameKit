using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Toolbar
{
    /**
     * Partial contain GUI elements, operations
     */
    public static partial class UnityToolBarSceneView
    {
        /*private class DisabeleScope : GUI.Scope
        {
            public DisabeleScope(bool disabled)
            {
                EditorGUI.BeginDisabledGroup(disabled);
            }

            protected override void CloseScope()
            {
                EditorGUI.EndDisabledGroup();
            }
        }*/

        private class ToolbarGUI
        {
            // Window closet size
            private readonly Vector2 closedWindowSize = new Vector2(150, 35);

            private IObjectWrapperGUI _hotObject;

            private IMethodWrapperGUI _hotWraper;

            /// <summary>
            ///     Нужен этот стиль для отрисовки нормальной читаемой надписи
            /// </summary>
            private GUIStyle _labelParamStyle;

            /**
             * Makes toolbar visible on the active scene
             */
            private string _title;

            // Flag to display the toolbar
            private bool drawToolbarItems;

            //Collection in which information is stored by classes and their methods that have data on the purpose of the call and the arguments
            private List<IObjectWrapperGUI> methodRepositories = new List<IObjectWrapperGUI>();

            private string[] namesOfClasses = { };

            // Current selected index of selected executable class
            private int numberOfSelectedMethodRepository;

            private int[] orderOfClasses = { };

            /**
             * Current selected index of method group number
             * Used to determine the current selected group to draw the buttons
             */
            private int selectedIndexOfMethodGroup;

            // Used to process toolbar window drag
            private Rect windowRect = new Rect(30, 30, 150, 35);

            // Button position on tool window
            private Rect minimizeButtonRect => new Rect(windowRect.width - 43, 0, 42, 15);

            public void Show(IEnumerable<IObjectWrapperGUI> classesData, string title = "tools")
            {
                _title = title;
                Close();
                drawToolbarItems = true;
                _labelParamStyle =
                    new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("label"))
                    {
                        normal = {textColor = Color.white}
                    };
                methodRepositories         = classesData.ToList();
                selectedIndexOfMethodGroup = 0;
                CollectMethodRepositioriesInfo();
#if UNITY_2020_1_OR_NEWER
                SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
               SceneView.RepaintAll();
            }

            /**
             * Removes toolbar from the scene view.
             */
            public void Close()
            {
                methodRepositories.Clear();
#if UNITY_2020_1_OR_NEWER
                SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
            }

            /**
             * Event updates the GUI in the scene view
             */
            private void OnSceneGUI(SceneView sceneView)
            {
                /** если так не сделать, то блять при переключении режима в 3d все полетит к хуям!
                 * Есть Проблема, что окно можно перетащить за пределы SceneView, оно становиться на место,то есть этотакая фича! хз. гугление ничего не дало.
                 */
                //  if (Event.current.type == EventType.Repaint) return;
                if (EditorApplication.isCompiling)
                {
                    Close();
                    return;
                }

                var oldSkin = GUI.skin;
                GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
                // Handles.BeginGUI();
                var limitiedrect = LimiterPositionOnSceneView(windowRect, sceneView.position);
                var id           = GUIUtility.GetControlID(FocusType.Passive);
                windowRect = GUILayout.Window(id, limitiedrect, DrawWindow, _title);
                //    Handles.EndGUI();
                GUI.skin = oldSkin;

                if (Event.current.type == EventType.Used && _hotObject != null && _hotWraper != null)
                {
                    var tempO = _hotObject;
                    var tempw = _hotWraper;
                    _hotObject = null;
                    _hotWraper = null;
                    tempw.Invoke(tempO.Target);
                }
            }

            /**
             * Draws toolbar window
             */
            private void DrawWindow(int windowID)
            {
                try
                {
                    DrawMinimizeButton();
                    if (drawToolbarItems)
                        ShowToolbarItems();
                    else
                        GUILayout.Label("Click To Open ->");

                    // GUI.DragWindow();
                }
                catch (ArgumentException e)
                {
                    Debug.LogException(e);
                    Debug.LogError(Event.current.type);
                }
            }

            /**
             * Used to draw the caps of the window where is the minimize button
             */
            private void DrawMinimizeButton()
            {
                var old = GUI.contentColor;
                GUI.contentColor = Color.red;
                if (GUI.Button(minimizeButtonRect, new GUIContent(drawToolbarItems ? "hide" : "show")))
                {
                    drawToolbarItems = !drawToolbarItems;
                    if (!drawToolbarItems)
                        windowRect.size = closedWindowSize;
                }

                GUI.contentColor = old;
            }

            /**
             * Used for draw content from class info,
             * Draw class selector (can change selected class)
             * Button group by group name
             * Executable buttons
             */
            private void ShowToolbarItems()
            {
                if (methodRepositories.Count > 1) DrawMethodRepositories();

                var wrapper = methodRepositories[numberOfSelectedMethodRepository];
                using (new EditorGUILayout.HorizontalScope("Box"))
                {
                    selectedIndexOfMethodGroup = GUILayout.Toolbar(selectedIndexOfMethodGroup, wrapper.GroupNames);
                }

                DrawButtons(wrapper);
            }

            /**
             * Used for draw the selection of the executable class
             */
            private void DrawMethodRepositories()
            {
                using (new EditorGUILayout.HorizontalScope("Box"))
                {
                    EditorGUI.BeginChangeCheck();
                    numberOfSelectedMethodRepository = EditorGUILayout.IntPopup(numberOfSelectedMethodRepository,
                        namesOfClasses, orderOfClasses);
                    if (EditorGUI.EndChangeCheck()) selectedIndexOfMethodGroup = 0;
                }
            }

            /**
             * Uses for drawing buttons in Scene view
             */
            private void DrawButtons(IObjectWrapperGUI methodRepository)
            {
                var group = methodRepository.GroupNames[selectedIndexOfMethodGroup];
                foreach (var methodWrapper in methodRepository.GetMethods(group))
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        var methodState = methodWrapper.GetStateInfo();
                        var content = new GUIContent
                        {
                            text    = methodState.Name,
                            tooltip = methodState.Description,
                            image   = methodState.Icon
                        };
                        //   EditorGUI.BeginDisabledGroup(!methodState.IsEnable);
                        using (new EditorGUI.DisabledScope(!methodState.IsEnable))
                        {
                            GUILayoutOption[] options = null;
                            if (methodState.Icon != null)
                            {
                                options = new[] {GUILayout.Height(50)};
                                GUILayout.Box(new GUIContent(methodState.Icon), GUILayout.Width(50),
                                    GUILayout.Height(50));
                            }

                            if (GUILayout.Button(content, options))
                            {
                                _hotWraper = methodWrapper;
                                _hotObject = methodRepository;
                            }
                            else
                            {
                                if (methodWrapper.HasParametr)
                                    DrawParameterMethod(methodWrapper);
                            }
                        }

                        //    EditorGUI.EndDisabledGroup();
                    }
            }

            private void DrawParameterMethod(IMethodWrapperGUI methodData)
            {
                foreach (var parametrInfo in methodData.GetParametrs())
                {
                    var labelContent = new GUIContent(parametrInfo.Name);
                    var size         = _labelParamStyle.CalcSize(labelContent);

                    EditorGUILayout.LabelField(labelContent, _labelParamStyle, GUILayout.Width(size.x));
                    parametrInfo.Value = DrawFieldByType(parametrInfo.Value, parametrInfo.ParameterType);
                }
            }

            /**
             * Because unity does not allow to draw a field of indeterminate type,
             * We will determine type by property
             */
            private object DrawFieldByType(object value, Type parameterType)
            {
                if (parameterType == typeof(Object) || parameterType.IsSubclassOf(typeof(Object)))
                    return EditorGUILayout.ObjectField(value as Object,
                        parameterType, true, GUILayout.ExpandWidth(true));
                if (parameterType.IsSubclassOf(typeof(Enum)))
                    return EditorGUILayout.EnumPopup(value as Enum);

                switch (parameterType.Name)
                {
                    case "Boolean":
                        return EditorGUILayout.Toggle((bool) value, GUILayout.ExpandWidth(true));

                    case "Int32":
                        return EditorGUILayout.IntField(Convert.ToInt32(value), GUILayout.ExpandWidth(true));

                    case "Single":
                        return EditorGUILayout.FloatField(Convert.ToSingle(value), GUILayout.ExpandWidth(true));

                    case "String":
                        return EditorGUILayout.TextField((string) value, GUILayout.ExpandWidth(true));

                    case "Color":
                    {
                        var color = (Color?) value ?? Color.white;
                        return EditorGUILayout.ColorField(color);
                    }

                    default:
                        return null;
                }
            }

            /**
             * Used to block the exit from the window in the scene viewing space
             */
            private Rect LimiterPositionOnSceneView(Rect windowRect, Rect sceneViewRect)
            {
                windowRect.x      = 15; //Mathf.Clamp(windowRect.x, 0, windowRect.width);
                windowRect.y      = 15; //Mathf.Clamp(windowRect.y, 0, windowRect.height);
                windowRect.height = 15;
                windowRect.width  = 15;
                //  Debug.Log(windowRect);
                return windowRect;
            }

            /**
             * Сreates an array of custom class names
             */
            private void CollectMethodRepositioriesInfo()
            {
                orderOfClasses = CreateArrayFromCount(methodRepositories.Count);
                namesOfClasses = methodRepositories.Select(cc => cc.CustomClassName).ToArray();
            }

            /**
             * Used to fill in sequence numbers
             */
            private int[] CreateArrayFromCount(int count)
            {
                var array = new int[count];
                for (var i = 0; i < count; i++)
                    array[i] = i;
                return array;
            }
        }
    }
}