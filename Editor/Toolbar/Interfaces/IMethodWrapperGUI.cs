using System.Collections.Generic;

namespace GameKit.Toolbar
{
    public interface IMethodWrapperGUI
    {
        string NameOfGroup { get; }
        bool HasParametr { get; }
        int Order { get; }

        void Invoke(object methodRepositoryTarget);

        IEnumerable<IParametrWrapperGUI> GetParametrs();

        IMethodStateInfo GetStateInfo();
    }
}