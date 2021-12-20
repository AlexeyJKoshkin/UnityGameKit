using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{

    public class DefaultSubScriptableFactory :ISubScriptableFactory
    {
        private readonly Type _type;
        private Dictionary<Type, bool> _allowMultitype = new Dictionary<Type, bool>();

        private readonly ValueNameBox<Type> _possibleTypes;

        public DefaultSubScriptableFactory(Type type)
        {
            _type = type;
            _possibleTypes = new ValueNameBox<Type>(GetContentFor);
            _possibleTypes.InitValues(ReflectionHelper.GetAllTypesInSolution(_type,false));
            foreach (var t in _possibleTypes)
            {
                _allowMultitype.Add(t, t.HasAttribute<AllowMultiItemsAttribute>());
            }
        }
        
        GUIContent GetContentFor(Type type, int index = -1)
        {
            var attribute = type.GetCustomAttribute<CustomGUIContentAttribute>();
            return attribute != null
                ? attribute.EnumType == null
                    ? new GUIContent(attribute.Name, attribute.Tooltip)
                    : attribute.Enum.GetContentForEnum(attribute.EnumType, false)
                : new GUIContent(type.Name);
        }

        IEnumerable<(Type item, GUIContent content)> IterateItems(IEnumerable<ScriptableObject> currentItems)
        {
            List<ScriptableObject> temp = currentItems?.ToList() ?? new List<ScriptableObject>();
            foreach (var pair in _possibleTypes.IterateItemWithContent())
            {
                if (temp.Any(o => o.GetType() == pair.item))
                {
                    if (_allowMultitype[pair.item]) yield return pair;
                    else continue;
                }
                yield return pair;
            }
        }

        public void DrawCreateNewElementMenu(IEnumerable<ScriptableObject> currentItems,Action<ScriptableObject> onAddNewElement)
        {
            var menu = new GenericMenu();
            foreach (var pair in IterateItems(currentItems))
            {
                menu.AddItem(pair.content, false, (userdata) =>
                {
                    
                    var type = userdata as Type;
                    if (type == null) return;
                    
                    onAddNewElement?.Invoke(ScriptableObject.CreateInstance(type));
                   
                }, pair.item);
            }
            menu.ShowAsContext();
        }
    }
    
}