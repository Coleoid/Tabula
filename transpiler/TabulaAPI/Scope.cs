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
                    lowKey = this[lowKey.Substring(1)].ToLower().Replace(" ", "");
                }
                if (_values.ContainsKey(lowKey))
                    return _values[lowKey];

                if (ParentScope == null)
                    return "";

                return ParentScope[lowKey];
            }

            set { _values[key.ToLower().Replace(" ", "")] = value; }
        }

        //speculation:
        public string NearHits(string unfoundName)
        {
            List<string> near = Levenshtein(unfoundName);

            var message = $"Did not find {unfoundName}.  Did you mean {near[0]}, {near[1]}, or {near[2]}?";
            return message;
        }

        public List<string> Levenshtein(string unfoundName)
        {
            var result = new List<string>();

            return result;
        }
    }
}
