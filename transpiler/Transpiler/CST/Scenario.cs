using System;
using System.Collections.Generic;

namespace Tabula.CST
{
    public interface Taggable
    {
        List<string> Tags { get; }
    }

    public class CST_Entity : Taggable
    {
        public List<string> Tags { get; set; }
        public string Label { get; set; }
    }

    public class Wrapper
    {
        private Token token;

        public Wrapper(Token token)
        {
            this.token = token;
        }

        public TokenType Type { get => token.Type; }
        public string Text { get => token.Text; }
    }


    //  ===================


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

    public class Step : Taggable
    {
        public Step(List<Symbol> symbols)
        {
            Symbols = symbols;
        }

        public List<string> Tags { get; set; }
        public List<Symbol> Symbols { get; set; }
    }

    public class Symbol : Wrapper
    {
        public Symbol(Token token)
            : base(token)
        { }

        internal static Symbol Wrap(Token token)
        {
            return token == null ? null : new Symbol(token);
        }
    }

    public class Label : Wrapper
    {
        public Label(Token token)
            : base(token)
        { }

        internal static Label Wrap(Token token)
        {
            return token == null ? null : new Label(token);
        }
    }
}
