using System;
using UnityEditor;
using UnityEngine;

namespace Kitchen.EditorUtilityHelpers
{
    /// <summary>
    ///     Базовое окно инпута
    /// </summary>
    public class BaseInputWindow : EditorWindow
    {
        private const float WIDTH = 250;

        private InputSettings _settings;

        private string _errorMessage, _enterText;
        private  string LabelEnter => _settings.Label;
        private  string   Header     => _settings.Header;


        private Func<string, string> OnSelectChecker;

        public virtual float DefaultHeight => 40;

        private void OnGUI()
        {
            var height = string.IsNullOrEmpty(_errorMessage) ? DefaultHeight : DefaultHeight + 40;
            minSize = new Vector2(WIDTH, height);
            //  this.position = new Rect(this.position.position, new Vector2(WIDTH, height));;
            DrawContent();
        }

        private void OnLostFocus()
        {
            OnSelectChecker?.Invoke(string.IsNullOrEmpty(_errorMessage) ? _settings.DefautValue : GetResult(_enterText));
            if(this == null) return;
            Close();
        }


        //  float _height = 90;

        private void SetSetting(InputSettings inputSettings)
        {
            _settings = inputSettings;
            _enterText = inputSettings.DefautValue;

        }

        public static void ShowSimpleInput<T>(Func<string, string> onSelectItem,
                                              InputSettings settings = null)
            where T : BaseInputWindow
        {
            settings ??= new InputSettings();
            var window = GetWindow<T>(settings.Header);
            window.SetSetting(settings);

            window.OnSelectChecker = onSelectItem;
            window.maxSize         = new Vector2(WIDTH, window.DefaultHeight);
            window.minSize         = new Vector2(WIDTH, window.DefaultHeight);
            //  window.position = new Rect(Event.current.mousePosition,new Vector2(WIDTH, window.DefaultHeight) );
            window.ShowUtility();
        }


        private void DrawContent()
        {
            DrawErrorMessageIfNeed();

            EditorGUI.BeginChangeCheck();
            DrawInput();
            if (EditorGUI.EndChangeCheck())
                _errorMessage = null;

            DrawBtn();
        }


        private void DrawBtn()
        {
            using (new GUILayout.HorizontalScope())
            {
                var temp = GUI.color;
                GUI.color = new Color(0.0f, 0.0f, 1f, 0.7f);

                if (GUILayout.Button("Cancel")) Close();

                GUI.color = new Color(1f, 0.0f, 0.0f, 0.7f);
                if (GUILayout.Button("Select"))
                {
                    _errorMessage = OnSelectChecker?.Invoke(GetResult(_enterText));
                    if (string.IsNullOrEmpty(_errorMessage))
                    {
                        OnSelectChecker = null;
                        Close();
                    }

                    
                }

                GUI.color = temp;
            }
        }

        /// <summary>
        ///     преобразует введеный текст если нужно перед отдачей результата
        /// </summary>
        /// <param name="enterText"></param>
        /// <returns></returns>
        protected virtual string GetResult(string enterText)
        {
            return enterText;
        }


        private void DrawErrorMessageIfNeed()
        {
            if (string.IsNullOrEmpty(_errorMessage)) return;
            EditorGUILayout.HelpBox(_errorMessage, MessageType.Error);
        }

        protected virtual void DrawInput()
        {
            DrawStandardInput();
        }

        protected void DrawStandardInput()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(LabelEnter, GUILayout.Width(50));
                _enterText = GUILayout.TextField(_enterText, 50);
            }
        }

        public class InputSettings
        {
            public string DefautValue = "";
            public string Header = "Сделай свой выбор";
            public string Label = "Enter";
        }
    }

    /*public class EnumSelectWindow<T> : BaseSceneViewInputWindow where T: struct
    {
        public class EnumInputSettings: InputSettings 
        {
            public T[] DisplayValues;
        }


        private SelectionGUI<T> _selection;
        
        public EnumSelectWindow(EnumInputSettings inputSettings) : base(inputSettings)
        {
            _selection = new PopUpSelector<T>(inputSettings.DisplayValues, -1);
        }

        protected override void DrawInput()
        {
            _selection.DoSelectGUI();
        }
    }*/
}