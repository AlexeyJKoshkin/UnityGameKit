using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameKit.Toolbar
{
    public class ObjectWithMethodsFactory : IObjectWithMethodsFactory
    {
        private readonly BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static |
            BindingFlags.Instance;

        /**
         * Converts type to class info.
         */
        public IObjectWrapperGUI CreateMethodRepository(Type type)
        {
            var customName = type.GetCustomAttribute<ToolbarMethodRepositoryAttribute>()?.CustomName ?? type.Name;
            var methodData = CreateMethodData(type, customName);
            return methodData;
        }

        public IObjectWrapperGUI CreateMethodRepository(object target)
        {
            var type       = target.GetType();
            var customName = type.GetCustomAttribute<ToolbarMethodRepositoryAttribute>()?.CustomName ?? type.Name;
            var methodData = CreateMethodData(type, customName, target);
            return methodData;
        }

        /**
         * Converts type methods to buttons info.
         */
        private IObjectWrapperGUI CreateMethodData(Type type, string customName, object target = null)
        {
            var methodData = new List<MethodDataWrapper>();
            foreach (var methodInfo in type.GetMethods(bindingFlags))
            {
                var attribute = methodInfo.GetCustomAttribute<ToolbarMethodDataAttribute>();
                if (attribute == null) continue;

                Func<bool> validator = null;
                if (!string.IsNullOrEmpty(attribute.NameOfValidationMethod))
                {
                    var validMethod = type.GetMethod(attribute.NameOfValidationMethod, bindingFlags);
                    if (validMethod != null)
                    {
                        if (target == null || validMethod.IsStatic)
                            validator = Delegate.CreateDelegate(typeof(Func<bool>), validMethod) as Func<bool>;
                        else
                            validator =
                                Delegate.CreateDelegate(typeof(Func<bool>), target, validMethod.Name) as Func<bool>;
                    }
                }

                var tooltips       = methodInfo.GetCustomAttribute<ToolBarTooltipAttribute>();
                var additionalAttr = methodInfo.GetCustomAttribute<AddionalValidayionAttribute>();
                var settings = new MethodDataSettings
                {
                    Validator                  = validator,
                    IsEnableWhenValidatorFalse = additionalAttr != null,
                    Strings = new MethodButtonData
                    {
                        AvalaibleName = additionalAttr == null ? methodInfo.Name : additionalAttr.AvalaibleName,
                        Description   = tooltips?.Tooltip,
                        NotAvalaibelName = additionalAttr == null
                            ? methodInfo.Name + " Not avalaible"
                            : additionalAttr.NotAvalaibleName,
                        IconPath = tooltips?.IconPath
                    }
                };
                var data = new MethodDataWrapper(methodInfo, settings, attribute.NameOfGroup, attribute.SortingOrder);

                methodData.Add(data);
            }

            return ObjectWithMethodsData.Create(customName, methodData, target);
        }
    }
}