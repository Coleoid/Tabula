using System;
using System.Collections.Generic;

namespace Tabula.CST
{
    /// <summary>
    /// Wrapper is for domain objects which are barely more than the token.
    /// </summary>
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

        internal static Label Wrap(Token token)
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

        internal static Symbol Wrap(Token token)
        {
            return token == null ? null : new Symbol(token);
        }
    }
}
