using System.Collections.Generic;

namespace GameKit.Toolbar
{
    public interface IObjectWrapperGUI
    {
        string CustomClassName { get; }

        string[] GroupNames { get; }

        object Target { get; }

        IEnumerable<IMethodWrapperGUI> GetMethods(string groupName);
    }
}