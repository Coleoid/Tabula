using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{

    public class Row : Dictionary<string,string> {}
    public class Table
    {
        public List<Row> Rows { get; set; }
        public List<string> Header;
        public List<List<string>> Data;
    }


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

        public void ActOnTable(Action paragraph, Table table)
        {
            foreach (var row in table.Rows)
            {
                Context.Push(row);
                paragraph();
                Context.Pop();
            }
        }

    }

    public class ScenarioContext
    {
        public void Push(Row row)
        {}

        public void Pop()
        {}
    }

    public class ValueContext
    {
        public Dictionary<string,string> values = new Dictionary<string,string>();
        public string this[string key]
        {
            get { return values[key]; }
            set { values[key] = value; }
        }
    }

    public class FunkyWorkflow : TabulaWorkflow
    {
        public FunkyWorkflow(ScenarioContext context)
            : base(context)
        {
        }

        public void I_do__(string action)
        { }

        public void Then_I_create__(string result)
        { }
    }

    public class EvaluateResultsWorkflow : TabulaWorkflow
    {
        public EvaluateResultsWorkflow(ScenarioContext context)
            : base(context)
        {
        }

        public void Both__and__should_show__results(string first, string second, string resultQuality)
        { }
    }

    public class TabulaWorkflow
    {
        public ScenarioContext Context;
        public TabulaWorkflow( ScenarioContext context )
        {
            Context = context;
        }
    }
}
