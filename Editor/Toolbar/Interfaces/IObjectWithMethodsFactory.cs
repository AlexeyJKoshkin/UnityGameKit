using System;

namespace GameKit.Toolbar
{
    public interface IObjectWithMethodsFactory
    {
        IObjectWrapperGUI CreateMethodRepository(Type type);

        IObjectWrapperGUI CreateMethodRepository(object target);
    }
}