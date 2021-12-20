using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameKit.Editor
{
    public static class ReflectionHelper
    {
        private static readonly List<string> _avalaibleNameSpaces;

        public static IEnumerable<MethodInfo> GetAllMethods<T>(
            Type type, BindingFlags bindingFlags = BindingFlags.Default) where T : class
        {
            var methods = type.GetMethods();
            foreach (MethodInfo method in methods)
                if (IsMethodCompatibleWithDelegate<T>(method))
                    yield return method;
        }

        public static bool IsMethodCompatibleWithDelegate<T>(MethodInfo method) where T : class
        {
            Type       delegateType      = typeof(T);
            MethodInfo delegateSignature = delegateType.GetMethod("Invoke");

            bool parametersEqual = delegateSignature
                                   .GetParameters()
                                   .Select(x => x.ParameterType)
                                   .SequenceEqual(method.GetParameters()
                                                        .Select(x => x.ParameterType));

            return delegateSignature.ReturnType == method.ReturnType &&
                   parametersEqual;
        }

        public static IEnumerable<MethodInfo> GetAllMethods(Type classtype, Type methodType,
                                                            BindingFlags bindingFlags = BindingFlags.Default)
        {
            var methods = classtype.GetMethods();
            foreach (MethodInfo method in methods)
                if (IsMethodCompatibleWithDelegate(method, methodType))
                    yield return method;
        }

        public static bool IsMethodCompatibleWithDelegate(MethodInfo method, Type delegateType)
        {
            MethodInfo delegateSignature = delegateType.GetMethod("Invoke");

            bool parametersEqual = delegateSignature
                                   .GetParameters()
                                   .Select(x => x.ParameterType)
                                   .SequenceEqual(method.GetParameters()
                                                        .Select(x => x.ParameterType));

            return delegateSignature.ReturnType == method.ReturnType &&
                   parametersEqual;
        }

        public static IEnumerable<Type> AllTypeInSolution(bool IncludingAbstract = false)
        {
            var scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            if (!scriptAssemblies.Contains(Assembly.GetExecutingAssembly()))
                scriptAssemblies.Add(Assembly.GetExecutingAssembly());
            return from assembly in scriptAssemblies
                from type in assembly.GetTypes()
                where IncludingAbstract || !type.IsAbstract
                select type;
        }

        public static Dictionary<Type, TAtr> GetAllTypeWithAttribute<TType, TAtr>(bool IncludingAbstract = false)
            where TAtr : Attribute
        {
            var res = new Dictionary<Type, TAtr>();
            foreach (var t in GetAllTypesInSolution<TType>(IncludingAbstract))
            {
                if (!Attribute.IsDefined(t, typeof(TAtr))) continue;
                var atr = t.GetCustomAttributes(typeof(TAtr), false)[0] as TAtr;
                res.Add(t, atr);
            }

            return res;
        }

        public static Dictionary<Type, T> GetAllTypeWithAttribute<T>(bool IncludingAbstract = false) where T : Attribute
        {
            var res = new Dictionary<Type, T>();
            foreach (var t in AllTypeInSolution(IncludingAbstract))
            {
                if (!Attribute.IsDefined(t, typeof(T))) continue;
                var atr = t.GetCustomAttributes(typeof(T), false)[0] as T;
                res.Add(t, atr);
            }

            return res;
        }

        /// <summary>
        ///     Все не абстрактные типы классов реализующие T в решении
        /// </summary>
        /// <returns></returns>
        public static IList<Type> GetAllTypesInSolution<T>(bool IncludingAbstract = false)
        {
            return AllTypeInSolution(IncludingAbstract).Where(t => typeof(T).IsAssignableFrom(t)).ToList();
        }

        /// <summary>
        ///     Все не абстрактные типы классов реализующие T в решении
        /// </summary>
        /// <returns></returns>
        public static IList<Type> GetAllTypesInSolution(Type type, bool IncludingAbstract = false)
        {
            if (type.IsGenericType)
                return AllTypeInSolution().Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                                                      t.BaseType.GetGenericTypeDefinition() == type).ToList();
            return AllTypeInSolution(IncludingAbstract).Where(type.IsAssignableFrom).ToList();
        }

        /// <summary>
        ///     Все не абстрактные типы классов реализующие T в решении
        /// </summary>
        /// <returns></returns>
        public static IList<Type> GetAllTypesInSolutionWithInterface<T>()
        {
            var interfaceType = typeof(T);
            return GetAllTypesInSolutionWithInterface(interfaceType);
        }

        public static IList<Type> GetAllTypesInSolutionWithInterface(Type interfaceType)
        {
            if (!interfaceType.IsInterface) return new List<Type>();
            return AllTypeInSolution().Where(t => t.Implements(interfaceType)).ToList();
        }

        public static bool Implements(this Type candidateType, Type interfaceType)
        {
            if (interfaceType.IsInterface && !candidateType.IsInterface && !candidateType.IsAbstract)
                return interfaceType.IsAssignableFrom(candidateType);
            return false;
        }

        public static bool HasAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), inherit).Length > 0;
        }

        public static T GetCustomAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            var attr = type.GetCustomAttributes(typeof(T), inherit);
            if (attr.Length == 0) return null;
            return attr[0] as T;
        }
    }
}