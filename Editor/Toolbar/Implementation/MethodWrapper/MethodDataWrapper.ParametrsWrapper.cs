using System;
using System.Reflection;

namespace GameKit.Toolbar
{
    public partial class MethodDataWrapper
    {
        private class ParametrsWrapper : IParametrWrapperGUI
        {
            public ParametrsWrapper(ParameterInfo parameterInfo)
            {
                Name          = parameterInfo.Name;
                ParameterType = parameterInfo.ParameterType;
                Value         = parameterInfo.DefaultValue;
            }

            public string Name { get; }
            public object Value { get; set; }

            public Type ParameterType { get; }
        }
    }
}