using System.Collections.Generic;

namespace Tabula.CST
{
    public interface Taggable
    {
        List<string> Tags { get; }
        string Label { get; }
    }

    public class CST_Entity : Taggable
    {
        public List<string> Tags { get; set; }
        public string Label { get; set; }

    }

    public class Scenario: CST_Entity
    {
        public List<Section> Sections { get; set; }
    }

    public class Section: CST_Entity
    {
    }

    public class Paragraph : Section
    {
        public List<string> Workflows { get; internal set; }
        public List<Step> Steps { get; internal set; }

    }

    public class Table : Section
    {
        public List<string> ColumnNames { get; set; }
        public List<List<string>> Rows { get; set; }
    }

    public class Step : CST_Entity
    {
        public List<Symbol> Symbols { get; }
    }

    public class Symbol
    {
        //  starting down a slippery slope!  EEK and such.
        private Token token;

        public Symbol(Token token)
        {
            this.token = token;
        }

        public TokenType Type { get => token.Type; }
        public string Text { get => token.Text; }
    }
}
