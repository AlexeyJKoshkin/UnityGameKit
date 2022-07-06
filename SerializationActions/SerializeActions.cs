using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Core.SerialisableAction
{
    [System.Serializable]
    public partial class AbstractSerialisableAction
    {
        public string MethodName;

        public string InspectorName;

        [SerializeField]
        public bool Show = false;

        // inspector cache
        public string[] candidatesMethods = { };

        public int index;
        public Delegate Sdelegate { get; protected set; }

        public AbstractSerialisableAction(string nameInInspector)
        {
            InspectorName = nameInInspector;
        }

        public virtual Type DelegateType
        {
            get { return typeof(Action); }
        }
    }

    [System.Serializable]
    public abstract class BaseSerialisableAction : AbstractSerialisableAction, ISerializationCallbackReceiver
    {
        [SerializeField]
        public byte[] _returnTypeData;

        [SerializeField]
        public byte[] _paramsTypeData;

        //[SerializeField]
        //public byte[] _DelegateTypeData;

        public abstract void CreateDelegate();

        public void OnBeforeSerialize()
        {
            //if (_delegateTypeData != null && _delegateTypeData.Length > 0) return;
            //_returnTypeData = DataHelper.Serialise(DelegateType);
            if (_returnTypeData != null && _returnTypeData.Length > 0) return;
            _returnTypeData = AssemblyReflectionHelper.Serialise(DelegateType.GetMethod("Invoke").ReturnType);

            if (_paramsTypeData != null && _paramsTypeData.Length > 0) return;
            _paramsTypeData = AssemblyReflectionHelper.Serialise(
                DelegateType
                    .GetMethod("Invoke").
                    GetParameters().
                    Select(e => e.ParameterType).ToArray());

            //if (_DelegateTypeData != null && _DelegateTypeData.Length > 0 && _delegateType == null)
            //    _delegateType = DataHelper.Deserialse<Type>(_DelegateTypeData);
        }

        public void OnAfterDeserialize()
        {
            if (_returnTypeData != null && _returnTypeData.Length > 0) return;
            _returnTypeData = AssemblyReflectionHelper.Serialise(DelegateType.GetMethod("Invoke").ReturnType);

            if (_paramsTypeData != null && _paramsTypeData.Length > 0) return;
            _paramsTypeData = AssemblyReflectionHelper.Serialise(
                DelegateType
                                                   .GetMethod("Invoke").
                                                   GetParameters().
                                                   Select(e => e.ParameterType).ToArray());
            //if (_DelegateTypeData != null && _DelegateTypeData.Length > 0 && _delegateType == null)
            //    _delegateType = DataHelper.Deserialse<Type>(_DelegateTypeData
        }

        protected BaseSerialisableAction(string inspectorName) : base(inspectorName)
        {
            OnAfterDeserialize();
        }
    }

    [System.Serializable]
    public class SAction : BaseSerialisableAction
    {
        // public settings
        public Object _target;

        //[SerializeField]
        //public byte[] _DelegateTypeData;

        public override void CreateDelegate()
        {
            if (_target != null && !string.IsNullOrEmpty(MethodName))
            {
                var methodInfo = _target.GetType().GetMethod(MethodName);
                if (methodInfo != null)
                    Sdelegate = Delegate.CreateDelegate(DelegateType, _target, methodInfo);
            }
        }

        public SAction(string inspectorName) : base(inspectorName)
        {
        }
    }

    [System.Serializable]
    public class SAction<T> : SAction where T : class
    {
        public override Type DelegateType
        {
            get { return typeof(T); }
        }

        public SAction(string inspectorName) : base(inspectorName)
        {
        }
    }

    [System.Serializable]
    public class LAction : AbstractSerialisableAction
    {
        public string NameLibrary;

        [SerializeField]
        public byte[] _paramsTypeData;

        public virtual Type LibraryType { get { return null; } }

        public LAction(string inspectorName) : base(inspectorName)
        {
        }

        public void CreateAction()
        {
            if (LibraryType == null) return;
            Reset(LibraryType);
        }

        public virtual void CreateAction(Type library)
        {
            Sdelegate = (MethodName == "None" || string.IsNullOrEmpty(MethodName))
               ? null
               : AssemblyReflectionHelper.FindAction<Action>(library, MethodName);
        }

        public virtual void Reset(Type library)
        {
            NameLibrary = library.Name;
            var temp = AssemblyReflectionHelper.GetInfoMethodsPack<Action>(library);
            if (!temp.ContainsKey("None"))
                temp.Add("None", "None");
            candidatesMethods = temp.Keys.Distinct().Reverse().ToArray();
        }
    }

    public partial class AbstractSerialisableAction
    {
        public virtual void Invoke<T1>(T1 t)
        {
        }

        public virtual void Invoke<T1, T2>(T1 t, T2 t2)
        {
        }
    }

    [System.Serializable]
    public class LAction<TAction, TLibrary> : LAction where TAction : class where TLibrary : class
    {
        public override Type DelegateType
        {
            get { return typeof(TAction); }
        }

        public override Type LibraryType
        {
            get
            {
                return typeof(TLibrary);
            }
        }

        public override void CreateAction(Type library)
        {
            //Debug.Log(MethodName);
            Sdelegate = (MethodName == "None" || string.IsNullOrEmpty(MethodName))
                 ? null
                 : AssemblyReflectionHelper.FindDelegate<TAction>(library, MethodName);
        }

        public override void Reset(Type library)
        {
            NameLibrary = library.Name;
            Debug.Log(NameLibrary);
            var temp = AssemblyReflectionHelper.GetInfoMethodsPack<TAction>(library);
            if (!temp.ContainsKey("None"))
                temp.Add("None", "None");
            candidatesMethods = temp.Keys.Distinct().Reverse().ToArray();
        }

        public LAction(string nameInInspector) : base(nameInInspector)
        {
        }
    }
}