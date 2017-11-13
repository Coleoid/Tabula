using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class ParserStateTests : TranspilerUnitTestBase
    {
        [Test]
        public void Take_returns_null_at_EOF()
        {
            var tagToken = new Token(TokenType.Tag, "Laughing");
            var tokens = new List<Token> { tagToken };
            var state = new ParserState(tokens);

            var taken = state.Take();
            Assert.That(taken, Is.SameAs(tagToken));

            taken = state.Take();
            Assert.That(taken, Is.Null);
            Assert.That(state.AtEnd);
            taken = state.Take();
            Assert.That(taken, Is.Null);
            Assert.That(state.AtEnd);
        }

        [Test]
        public void TakeExpectingType_throws_with_user_message()
        {
            var tagToken = new Token(TokenType.Tag, "Laughing");
            var tokens = new List<Token> { tagToken };
            var state = new ParserState(tokens);

            var effMsg = "DateEffective command requires a date";
            var ex = Assert.Throws<Exception>(() => state.Take(TokenType.Date, effMsg));
            Assert.That(ex.Message, Is.SameAs(effMsg));
        }
    }
}
