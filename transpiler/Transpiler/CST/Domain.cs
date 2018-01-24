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
        public string MethodName { get; set; }

        public Paragraph()
        {
            Actions = new List<Action>();
        }
    }

    public class Step : Action, ITaggable
    {
        public List<Symbol> Symbols { get; set; }

        public Step(List<Symbol> symbols)
        {
            Symbols = symbols;
        }

        public Step(int line, params (TokenType Word, string)[] syms)
        {
            Symbols = new List<Symbol>();
            foreach(var (type, text) in syms)
            {
                Symbols.Add(new Symbol(type, text, line));
            }
            StartLine = line;
        }

        public string GetMethodSearchName()
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

        //TODO:  Keep watch on string representations, eventual rework
        public string GetReadableText()
        {
            var stepText = "";
            string delim = "";
            foreach (var sym in Symbols)
            {
                if (sym.Type == TokenType.String)
                {
                    stepText += delim + "\"" + sym.Text + "\"";
                }
                else
                {
                    stepText += delim + sym.Text;
                }

                delim = " ";
            }
            return stepText;

        }

        //TODO:  Keep watch on string representations, eventual rework
        public object GetReadableString()
        {
            var readableText = GetReadableText();

            string readableString = readableText.Replace("\"", "\"\"");
            readableString = "@\"" + readableString + "\"";

            return readableString;
        }
    }

    public class Symbol : Wrapper
    {
        public Symbol(Token token)
            : base(token)
        { }

        public Symbol(TokenType type, string text)
            : base(new Token(type, text))
        { }

        public Symbol(TokenType type, string text, int line)
            : base(new Token(type, text) { Line = line })
        { }

        internal static Symbol Wrap(Token token)
        {
            return token == null ? null : new Symbol(token);
        }
    }
}
