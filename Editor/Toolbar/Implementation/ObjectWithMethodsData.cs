using System.Collections.Generic;
using System.Linq;

namespace GameKit.Toolbar
{
    /// <summary>
    ///     Тут хранятся мета данные по классу(методы для вызоа, имя группы)  и сам инстанс класса(если не статик)
    /// </summary>
    public class ObjectWithMethodsData : IObjectWrapperGUI
    {
        private readonly Dictionary<string, List<IMethodWrapperGUI>> _groupMethods;
        private int _index;

        private ObjectWithMethodsData(IEnumerable<IMethodWrapperGUI> buttonInformationCollection)
        {
            _groupMethods = buttonInformationCollection.GroupBy(o => o.NameOfGroup)
                                                       .ToDictionary(o => o.Key, o => o.ToList());

            foreach (var e in _groupMethods.Values) e.Sort((e1, e2) => e1.Order.CompareTo(e2.Order));
            GroupNames = _groupMethods.Keys.ToArray();
        }

        public string CustomClassName { get; private set; }
        public string[] GroupNames { get; }

        public IEnumerable<IMethodWrapperGUI> GetMethods(string groupName)
        {
            List<IMethodWrapperGUI> list = null;
            if (_groupMethods.TryGetValue(groupName, out list))
                for (_index = 0; _index < list.Count; _index++)
                    yield return list[_index];
        }

        public object Target { get; private set; }

        public static IObjectWrapperGUI Create(string name, IEnumerable<IMethodWrapperGUI> buttonInformationCollection,
                                               object target)
        {
            return new ObjectWithMethodsData(buttonInformationCollection)
            {
                CustomClassName = name,
                Target          = target
            };
        }
    }
}