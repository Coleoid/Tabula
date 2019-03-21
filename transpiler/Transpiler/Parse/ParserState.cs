using System;
using System.Collections.Generic;
using System.Linq;
using Tabula.CST;

namespace Tabula.Parse
{
    public class ParserState
    {
        public TokenizerOutput Output;
        public int Position;

        public ParserState(TokenizerOutput output, int position = 0)
        {
            Output = output;
            Position = position;
        }

        public bool AtEnd
        {
            get => Position == Output.Tokens.Count;
        }
        public bool LineComplete
        {
            get => AtEnd || NextIs(TokenType.NewLine);
        }

        public Token Peek()
        {
            return AtEnd ? null : Output.Tokens[Position];
        }

        public bool NextIs(TokenType tokenType)
        {
            return Peek()?.Type == tokenType;
        }

        public bool NextIsIn(params TokenType[] tokenTypes)
        {
            var nextType = Peek()?.Type;
            return tokenTypes.Any(t => t == nextType);
        }

        public Token Take()
        {
            return AtEnd ? null : Output.Tokens[Position++];
        }

        internal Token Take(TokenType tokenType)
        {
            return NextIs(tokenType) ? Take() : null;
        }

        public Token Take(TokenType tokenType, string exceptionMessage)
        {
            if (NextIs(tokenType)) return Take();
            throw new Exception(exceptionMessage);
        }

        internal Token Take(params TokenType [] types)
        {
            return NextIsIn(types) ? Take() : null;
        }

        internal void AdvanceLines()
        {
            while (Take(TokenType.NewLine) != null) { }
        }
    }
}
