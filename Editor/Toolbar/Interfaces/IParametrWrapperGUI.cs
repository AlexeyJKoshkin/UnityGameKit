using System;

namespace GameKit.Toolbar
{
    public interface IParametrWrapperGUI
    {
        string Name { get; }
        object Value { get; set; }
        Type ParameterType { get; }
    }
}