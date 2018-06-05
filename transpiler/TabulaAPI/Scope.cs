using System.Collections.Generic;

namespace Tabula.API
{
    public class Scope
    {
        public string Name;
        public Scope ParentScope = null;
        private Dictionary<string, string> _values = new Dictionary<string, string>();

        public string this[string key]
        {
            get
            {
                var lowKey = key.ToLower().Replace(" ", "");
                if (lowKey.StartsWith("#"))
                {
                    lowKey = this[lowKey.Substring(1)].ToLower().Replace(" ", "")];
                }
                if (_values.ContainsKey(lowKey))
                    return _values[lowKey];

                if (ParentScope == null)
                    return "";

                return ParentScope[lowKey];
            }

            set { _values[key.ToLower().Replace(" ", "")] = value; }
        }

    }
}
