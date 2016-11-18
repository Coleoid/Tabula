using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    public class GeneratedScenarioBase
    {
        public ScenarioContext Context { get; set; }
        public ValueContext val { get; set; }
        public TabulaStepRunner Runner { get; set; }

        public GeneratedScenarioBase(TabulaStepRunner runner)
        {
            Runner = runner;
        }

        public void Do(Action step, string sourceLocation, string stepText)
        {
            Runner.Do(step, sourceLocation, stepText);
        }

        public void Unfound(string stepText, string sourceLocation)
        {
            Runner.Unfound(stepText, sourceLocation);
        }

        public void TableOverParagraph(Action paragraph, Func<Table> tableGenerator)
        {
            Table table = tableGenerator();
            foreach (var row in table.Rows)
            {
                Context.Push(row);
                paragraph();
                Context.Pop();
            }
        }
    }
}
