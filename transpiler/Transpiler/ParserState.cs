using System;
using System.Collections.Generic;

namespace Tabula
{
    public class ParserState
    {
        public List<Token> Tokens;
        public int Position;

        public ParserState(List<Token> tokens, int position = 0)
        {
            Tokens = tokens;
            Position = position;
        }

        public bool AtEnd
        {
            get => Position == Tokens.Count;
        }

        public Token Peek()
        {
            return AtEnd ? null : Tokens[Position];
        }

        public Token Take()
        {
            return AtEnd ? null : Tokens[Position++];
        }

        public bool NextIs(string text)
        {
            return Peek().Text == text;
        }
        public bool NextIs(TokenType tokenType)
        {
            return Peek().Type == tokenType;
        }

        internal Token TakeNextIs(TokenType tokenType, string exceptionMessage)
        {
            if (NextIs(tokenType)) return Take();
            throw new Exception(exceptionMessage);
        }

        internal string TextIfType(TokenType tokenType)
        {
            return NextIs(tokenType)
                ? Take().Text
                : null;
        }

        internal List<string> PartsIfType(TokenType tokenType)
        {
            return NextIs(tokenType)
                ? Take().Parts
                : null;
        }
    }
}
