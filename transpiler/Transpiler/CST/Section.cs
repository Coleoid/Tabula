using System.Collections.Generic;

namespace Tabula.CST
{
    /// <summary> A chunk of a scenario.  A paragraph or a table. </summary>
    public class Section : CST_Entity
    {
        public string MethodName { get; set; }
    }

    public class Paragraph : Section
    {
        public List<Action> Actions { get; internal set; }
        public List<WorkflowDetail> WorkflowsInScope { get; internal set; }

        public Paragraph()
        {
            Actions = new List<Action>();
            WorkflowsInScope = new List<WorkflowDetail>();
        }
    }

    public class Table : Section
    {
        public List<string> ColumnNames { get; set; }
        public List<TableRow> Rows { get; set; }

        public Table()
        {
            ColumnNames = new List<string>();
            Rows = new List<TableRow>();
        }
    }

    public class TableRow : Action
    {
        public List<List<string>> Cells { get; set; }

        public TableRow(List<List<string>> cells)
        {
            Cells = cells;
        }

        public TableRow(params string[] cellWords)
        {
            Cells = new List<List<string>>();

            foreach (var word in cellWords)
            {
                Cells.Add(new List<string> { word });
            }
        }
    }

}
