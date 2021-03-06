﻿
using System.Collections.Generic;
using System.Linq;

namespace Tabula.CST
{
    /// <summary> Wrapper is for domain objects which are barely more than the token. </summary>
    public class Wrapper
    {
        private Token token;

        public Wrapper(Token token)
        {
            this.token = token;
        }

        public TokenType Type { get => token.Type; }
        public string Text { get => token.Text; }
        public int LineNumber { get => token.Line; }
    }

    public class Label : Wrapper
    {
        public Label(Token token)
            : base(token)
        { }

        internal static Label NewOrNull(Token token)
        {
            return token == null ? null : new Label(token);
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

        public bool IsStepArgument 
        { 
            get
            {
                return Type == TokenType.Date
                    || Type == TokenType.Number
                    || Type == TokenType.String
                    || Type == TokenType.Variable;
            }
        }

        internal static Symbol NewOrNull(Token token)
        {
            return token == null ? null : new Symbol(token);
        }
    }

    public class SymbolCollection : Symbol
    {
        public List<Symbol> Values { get; set; }

        public SymbolCollection()
            : base(new Token(TokenType.Collection, "Collection"))
        {
            Values = new List<Symbol>();
        }
    }
}
