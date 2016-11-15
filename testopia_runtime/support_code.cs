using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{

    public class TabulaWorkflow
    {
        public ScenarioContext Context;
        public TabulaWorkflow( ScenarioContext context )
        {
            Context = context;
        }
    }

    public class Row : Dictionary<string,string> {}
    public class Table
    {
        public List<Row> Rows { get; set; }
        public List<string> Header;
        public List<List<string>> Data;
    }

    public class AssertionException : Exception
    {
        public AssertionException( string message )
            : base(message)
        { }
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


}
