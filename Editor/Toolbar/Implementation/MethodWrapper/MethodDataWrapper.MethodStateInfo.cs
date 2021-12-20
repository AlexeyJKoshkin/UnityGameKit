using UnityEngine;

namespace GameKit.Toolbar
{
    public partial class MethodDataWrapper
    {
        private class MethodStateInfo : IMethodStateInfo
        {
            public string Name { get; set; }
            public Texture Icon { get; set; }
            public string Description { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}