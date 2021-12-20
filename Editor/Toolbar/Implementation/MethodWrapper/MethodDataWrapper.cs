using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameKit.Toolbar
{
    /// <summary>
    ///     реализация IMethodWrapperGUI
    /// </summary>
    public partial class MethodDataWrapper : IMethodWrapperGUI
    {
        private readonly MethodInfo _methodInfo;
        private readonly ParametrsWrapper[] _paramsWrapper;

        private readonly MethodStateInfo _state;

        private MethodDataSettings _settings;

        public MethodDataWrapper(MethodInfo info, MethodDataSettings settings, string group, int order)
        {
            _methodInfo = info;
            var parametrs = info.GetParameters();
            _paramsWrapper = new ParametrsWrapper[parametrs.Length];
            for (var i = 0; i < parametrs.Length; i++) _paramsWrapper[i] = new ParametrsWrapper(parametrs[i]);

            NameOfGroup = group;
            Order       = order;
            _settings   = settings;
            _state = new MethodStateInfo
            {
                Description = _settings.Strings.Description,
                Icon        = AssetDatabase.LoadAssetAtPath<Texture>(_settings.Strings.IconPath)
            };
        }

        public string NameOfGroup { get; }

        public bool HasParametr => _paramsWrapper.Length > 0;

        public int Order { get; }

        public void Invoke(object methodRepositoryTarget)
        {
            _methodInfo.Invoke(methodRepositoryTarget, _paramsWrapper.Select(o => o.Value).ToArray());
        }

        public IEnumerable<IParametrWrapperGUI> GetParametrs()
        {
            for (var i = 0; i < _paramsWrapper.Length; i++) yield return _paramsWrapper[i];
        }

        public IMethodStateInfo GetStateInfo()
        {
            // var isDisable = _settings.Validator == null || _settings.Validator();
            if (_settings.Validator == null)
            {
                _state.IsEnable = true;
                _state.Name     = _settings.Strings.AvalaibleName;
            }
            else
            {
                var isEnable = _settings.Validator();

                _state.IsEnable = _settings.IsEnableWhenValidatorFalse || isEnable;
                _state.Name     = isEnable ? _settings.Strings.AvalaibleName : _settings.Strings.NotAvalaibelName;
                //Debug.Log(_methodInfo.Name  + " " + isEnable + " " + _state.IsEnable + " " + _settings.IsEnableWhenValidatorFalse);
                //Debug.Log(_methodInfo.Name  + " " + isEnable + " " + _settings.Validator.Method.Name + " " + _settings.Validator.Target.GetType().Name);
            }

            return _state;
        }
    }
}