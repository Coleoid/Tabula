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

        public Paragraph()
        {
            Actions = new List<Action>();
        }
    }

    public class Table : Section
    {
        public List<string> ColumnNames { get; set; }
        public List<TableRow> Rows { get; set; }

        public Table()
        {
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
    }

}
