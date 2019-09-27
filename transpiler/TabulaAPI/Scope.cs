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

        public Scope(Scope parent)
        {
            this.ParentScope = parent;
        }

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

        //BUG: make test where we have the variable in a parent scope
        public bool HasVariable(string text)
        {
            return _values.ContainsKey(norm(text));
        }

        public string NearHits(string unfoundName)
        {
            var near = NearMatchesOrdered(unfoundName).Select(x=> $"'{x}'").ToList();

            if (near.Any())
            {
                return $"Maybe you meant {Listify(near)}?";
            }
            else
            {
                return "This doesn't sound like any variable I know about.";
            }
        }

        public static double CloseEnoughThreshold = .5;
        private IEnumerable<string> NearMatchesOrdered(string unfoundName)
        {
            return OrderByFitness(unfoundName, _values.Keys)
                .Where(x => x.fitness > CloseEnoughThreshold)
                .Select(x => x.match);
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

        public static IOrderedEnumerable<(String match, double fitness)> 
            OrderByFitness(string goal, IEnumerable<String> candidates)
        {
            return candidates
                .Select(candidate => (candidate, MatchFitness(goal, candidate)))
                .OrderByDescending(x => x.Item2);
        }

        public static double MatchFitness(string goal, string candidate)
        {
            double sp = SubstringPercentage(goal, candidate);
            double ep = EditPercentage(goal, candidate);
            return (sp+ep)/2;  // first stab
        }

        public static double SubstringPercentage(string goal, string candidate)
        {
            double len = goal.Length < candidate.Length ? goal.Length : candidate.Length;
            return LongestCommonSubstring(goal, candidate).Length / len;
        }

        public static double EditPercentage(string goal, string candidate)
        {
            double len = 0.0;
            if (goal.Length > candidate.Length)
                len = goal.Length;
            else
                len = candidate.Length;

            return 1.0 - EditDistance(goal, candidate) / len;
        }

        public static string LongestCommonSubstring(string a, string b)
        {
            var lengths = new int[a.Length, b.Length];
            int greatestLength = 0;
            string output = "";
            for (int ax = 0; ax < a.Length; ax++)
            {
                for (int bx = 0; bx < b.Length; bx++)
                {
                    if (a[ax] == b[bx])
                    {
                        lengths[ax, bx] = ax == 0 || bx == 0 ? 1 : lengths[ax - 1, bx - 1] + 1;
                        if (lengths[ax, bx] > greatestLength)
                        {
                            greatestLength = lengths[ax, bx];
                            output = a.Substring(ax - greatestLength + 1, greatestLength);
                        }
                    }
                    else
                    {
                        lengths[ax, bx] = 0;
                    }
                }
            }

            return output;
        }

        public static int EditDistance(String goal, String candidate)
        {
            if (String.IsNullOrEmpty(goal) && String.IsNullOrEmpty(candidate))
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

            return distances[goal.Length, candidate.Length];
        }
    }
}
