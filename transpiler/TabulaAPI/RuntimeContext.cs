using System;
using System.Collections.Generic;

namespace Tabula.API
{
    public class RuntimeContext
    {
        public string ParagraphName { get; internal set; }

        public string this[string key]
        {
            get => _currentScope[key];
        }

        public Scope CurrentScope { get => _currentScope; set => _currentScope = value; }
        protected Scope _currentScope;


        public void AddRowValues(List<string> labels, List<string> values)
        {
            for (var i = 0; i < labels.Count; i++)
            {
                _currentScope[labels[i]] = values[i];
            }
        }

        public List<string> BuildCallStack()
        {
            var calls = new List<string>();

            for (var scope = CurrentScope; scope != null; scope = scope.ParentScope)
            {
                calls.Add(scope.Name);
            }

            calls.Reverse();
            return calls;
        }

        public void OpenScope(string name)
        {
            OpenScope(new Scope(parent: null) { Name = name });
        }
        public void OpenScope(Scope newScope)
        {
            newScope.ParentScope = CurrentScope;
            CurrentScope = newScope;
        }

        public void CloseScope()
        {
            if (CurrentScope.ParentScope == null)
                throw new Exception("Closed too many scopes");

            CurrentScope = CurrentScope.ParentScope;
        }
    }
}
