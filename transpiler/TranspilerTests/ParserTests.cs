using NUnit.Framework;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class ParserTests
    {
        Parser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new Parser();
        }

        [Test]
        public void Term()
        {
            var tokens = new List<Token> { new Token(TokenType.Date, "09/30/1966") };
            var state = new ParserState(tokens);
            var cst = parser.ParseTerm(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Type, Is.EqualTo(TokenType.Date));
            Assert.That(cst.Text, Is.EqualTo("09/30/1966"));
        }

    }
}
