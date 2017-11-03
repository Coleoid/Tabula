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

    public class Action : Taggable
    {
        public List<string> Tags { get; set; }
    }

    public class Block : Action
    {
        public List<Action> Actions { get; set; }
        public Block(List<Action> actions)
        {
            Actions = actions;
        }
    }

    public class CommandAlias : Action
    {
        public string Name { get; set; }
        public Action Action { get; set; }

        public CommandAlias(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }

    public class CommandSet : Action
    {
        public string Name { get; set; }
        public Symbol Term { get; set; }

        public CommandSet(string name, Symbol term)
        {
            Name = name;
            Term = term;
        }
    }

    public class CommandUse : Action
    {
        public List<string> Workflows { get; set; }
        public CommandUse(List<string> workflows)
        {
            Workflows = workflows;
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

    public class Paragraph : Section
    {
        public List<Action> Actions { get; internal set; }
    }

    public class Scenario: CST_Entity
    {
        public List<Section> Sections { get; set; }
    }

    public class Section: CST_Entity
    {
    }

    public class Step : Action, Taggable
    {
        public Step(List<Symbol> symbols)
        {
            Symbols = symbols;
        }

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

    public class Table : Section
    {
        public List<string> ColumnNames { get; set; }
        public List<TableRow> Rows { get; set; }
    }

    public class TableRow
    {
        public List<List<string>> Cells { get; set; }

        public TableRow(List<List<string>> cells)
        {
            Cells = cells;
        }
    }
}
