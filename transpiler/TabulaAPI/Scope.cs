using System;
using System.Collections.Generic;
using System.Linq;

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
            var near = Levenshtein(unfoundName).Select(x=> $"'{x}'").ToList();

            if (near.Any())
            {
                return $"Maybe you meant {Listify(near)}?";
            }
            else
            {
                return "This doesn't sound like any variable I know about.";
            }
        }

        private IEnumerable<string> Levenshtein(string unfoundName)
        {
            return OrderByMatchLevel(_values.Keys, unfoundName).Where(x => x.level > HeuristicMatch).Select(x => x.match);
        }

        public bool HasVariable(string text)
        {
            return _values.ContainsKey(norm(text));
        }

        private static String Listify(IEnumerable<String> vals)
        {
            var values = vals is ICollection<String> valsl ? valsl : vals.ToList();

            if (values.Count == 0)
            {
                return "";
            }
            else if (values.Count == 1)
            {
                return values.First();
            }
            else if (values.Count == 2)
            {
                return $"{values.First()} or {values.Last()}";
            }
            else
            {
                return $"{String.Join(", ", values.Take(values.Count - 1))}, or {values.Last()}";
            }
        }

        private static IOrderedEnumerable<(String match, double level)> OrderByMatchLevel(IEnumerable<String> candidates, String goal)
        {
            return candidates.Select(x => (x, LevenshteinishDistance(goal, x))).OrderByDescending(x => x.Item2);
        }

        public static double HeuristicMatch = 1;

        private static double LevenshteinishDistance(String goal, String candidate)
        {
            if (String.IsNullOrEmpty(goal) || String.IsNullOrEmpty(candidate))
            {
                return 0;
            }

            goal = goal.ToUpper();
            candidate = candidate.ToUpper();

            var distances = new int[goal.Length + 1, candidate.Length + 1];

            for (int i = 0; i <= goal.Length; i++)
            {
                distances[i, 0] = i;
            }
            for (int j = 0; j <= candidate.Length; j++)
            {
                distances[0, j] = j;
            }

            for (int i = 1; i <= goal.Length; i++)
            {
                for (int j = 1; j <= candidate.Length; j++)
                {
                    int cost = candidate[j - 1] == goal[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min(Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1), distances[i - 1, j - 1] + cost);
                }
            }

            return goal.Length / (double)distances[goal.Length, candidate.Length];
        }
    }
}
