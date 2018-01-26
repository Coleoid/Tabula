using System.Collections.Generic;

namespace Tabula.CST
{
    public class Table : Section
    {
        public string MethodName { get; set; }
        public List<string> ColumnNames { get; set; }
        public List<TableRow> Rows { get; set; }
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
