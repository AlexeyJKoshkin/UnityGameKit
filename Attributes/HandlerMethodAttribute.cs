using System;

namespace GameKit {
    /// <summary>
    /// Атрибут для физических детектов
    /// </summary>
    public class HandlerMethodAttribute : Attribute
    {
        public string LayerName;

        public string Key
        {
            get { return _key; }
        }

        private string _key;

        public HandlerMethodAttribute(string layer = "Default", string key = "")
        {
            _key      = key;
            LayerName = layer;
        }
    }
}