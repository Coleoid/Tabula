using System;
using System.Collections.Generic;

namespace Tabula.CST
{
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
        public string MethodName { get; internal set; }

        public Paragraph()
        {
            Actions = new List<Action>();
        }
    }

    public class Step : Action, ITaggable
    {
        public Step(List<Symbol> symbols)
        {
            Symbols = symbols;
        }

        public List<Symbol> Symbols { get; set; }

        public string GetCanonicalMethodName()
        {
            string name = string.Empty;

            foreach(var symbol in Symbols)
            {
                if (symbol.Type == TokenType.Word)
                {
                    name += symbol.Text.ToLower();
                }
            }

            return name;
        }
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
}
