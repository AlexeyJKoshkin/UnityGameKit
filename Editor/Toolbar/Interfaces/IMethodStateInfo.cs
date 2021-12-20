using UnityEngine;

namespace GameKit.Toolbar
{
    public interface IMethodStateInfo
    {
        string Name { get; }
        Texture Icon { get; }
        string Description { get; set; }
        bool IsEnable { get; }
    }
}