using System;

namespace GameKit.Toolbar
{
    /// <summary>
    ///     настройки для работы враппера методы
    /// </summary>
    public struct MethodDataSettings
    {
        public Func<bool> Validator;
        public bool IsEnableWhenValidatorFalse;
        public MethodButtonData Strings;
    }
}