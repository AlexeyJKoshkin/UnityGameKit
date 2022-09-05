using System;
using GameKit;
using GameKit.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Kitchen.CustomInspectors
{
    #if ODIN_INSPECTOR
    public class UDateTimeOdinDrawer : OdinValueDrawer<UDateTime>
    {
        private static System.Globalization.DateTimeFormatInfo _info = System.Globalization.DateTimeFormatInfo.CurrentInfo;
        private GUIContent _plus,
                           _minus;
        private PopUpSelector<int> _month;
        private int _monthInt,
                    _daysInt,
                    _hourInt;

        private int _maxDays;

        protected override void Initialize()
        {
            base.Initialize();
            _plus = new GUIContent((Texture) EditorGUIUtility.Load("scrollup"));
            _minus = new GUIContent((Texture) EditorGUIUtility.Load("scrolldown"));
            var currentValue = this.ValueEntry.SmartValue.dateTime;

            _month = new PopUpSelector<int>(new[]
            {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10,
                11,
                12
            }, (value, index) => new GUIContent(_info.GetMonthName(value)))
            {
                OnSelectNewItemCallback = UpdateDaysList
            };

            UpdateDaysList(currentValue.Month);
            _daysInt = currentValue.Day;
        }

        private void UpdateDaysList(int month)
        {
            _maxDays = DateTime.DaysInMonth(this.ValueEntry.SmartValue.dateTime.Year, month);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            DateTime dateTime = this.ValueEntry.SmartValue.dateTime;
            int year = dateTime.Year;
            int minutes = dateTime.Minute;

            _month.SetCurrent(dateTime.Month);
            _hourInt = dateTime.Hour;

            EditorGUI.BeginChangeCheck();

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(label, GUILayout.Width(40));
                _daysInt = Mathf.Clamp(DrawSelectionInt(_daysInt, _maxDays, SelectDays), 1, _maxDays);
                _month.DoSelectGUI();
                year = EditorGUILayout.IntField(year, GUILayout.Width(45));
                GUILayout.Space(5);
                _hourInt = Mathf.Clamp(DrawSelectionInt(_hourInt, 24, SelectHours), 0, 23);
                GUILayout.Label(":");
                minutes = DrawInt(minutes, 25);
            }

            if (EditorGUI.EndChangeCheck())
            {
                UpdateValue(year, minutes, dateTime);
            }
        }

        int DrawSelectionInt(int currentValue, int maxValue, GenericMenu.MenuFunction2 selectionHandler)
        {
            using (new GUILayout.HorizontalScope(GUILayout.Width(15)))
            {
                currentValue =  EditorGUILayout.IntField(currentValue, GUILayout.Width(25));

                if (GUILayout.Button(_minus, GUILayout.Width(20)))
                {
                    ShowIntSelector(maxValue, selectionHandler);
                }
            }

            return currentValue;
        }

        private void ShowIntSelector(int maxValue, GenericMenu.MenuFunction2 selectHours)
        {
            GenericMenu genericMenu = new GenericMenu();

            for (int i = 0; i < maxValue; i++)
            {
                genericMenu.AddItem(new GUIContent(i.ToString()), false, selectHours, i);
            }

            GUI.FocusControl(null);
            genericMenu.ShowAsContext();
        }

        void SelectHours(object selectedHour)
        {
            _hourInt = (int) selectedHour;
            this.ValueEntry.SmartValue.dateTime = this.ValueEntry.SmartValue.dateTime.AddHours(_hourInt - this.ValueEntry.SmartValue.dateTime.Hour);
        }

        void SelectDays(object selectedDays)
        {
            _daysInt = (int) selectedDays;
            this.ValueEntry.SmartValue.dateTime = this.ValueEntry.SmartValue.dateTime.AddDays(_daysInt - this.ValueEntry.SmartValue.dateTime.Day);
        }

        void UpdateValue(int year, int minutes, DateTime dateTime)
        {
            year = Mathf.Clamp(year, DateTime.Now.Year, DateTime.Now.Year + 1);
            dateTime = dateTime.AddHours(_hourInt - dateTime.Hour);
            dateTime = dateTime.AddMinutes(minutes - dateTime.Minute);
            dateTime = dateTime.AddDays(_daysInt - dateTime.Day);
            dateTime = dateTime.AddMonths(_month.CurrentValue - dateTime.Month);
            dateTime = dateTime.AddYears(year - dateTime.Year);
            this.ValueEntry.SmartValue.dateTime = dateTime;
        }

        private int DrawInt(int val, float width, int delta= 5)
        {
            int result = val;

            using (new GUILayout.HorizontalScope())
            {
                result = EditorGUILayout.IntField(result, GUILayout.Width(width));

                if (GUILayout.Button(_plus, EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    result += delta;
                }

                if (GUILayout.Button(_minus, EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    result -= delta;
                }

                return result;
            }
        }
    }
    #endif
}
