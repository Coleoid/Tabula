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
                key = norm(key);

                if (key.StartsWith("#"))
                {
                    key = norm(this[key.Substring(1)]);
                }

                if (_values.ContainsKey(key))
                {
                    var innerValue = _values[key];
                    if (innerValue.StartsWith("#"))
                    {
                        innerValue = this[innerValue.Substring(1)];
                    }

                    return innerValue;
                }

                if (ParentScope == null)
                    return "";

                return ParentScope[key];
            }

            set { _values[norm(key)] = value; }
        }

        public string norm(string input)
        {
            return input.ToLower().Replace(" ", "");
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
