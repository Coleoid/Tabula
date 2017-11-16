using NUnit.Framework;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class CSTTests : TranspilerUnitTestBase
    {

        [TestCase("Hello", "hello")]
        [TestCase("Save", "save")]
        public void GetCanonicalMethodName_simple_case(string word, string expectedAnswer)
        {
            var step = new CST.Step(new List<CST.Symbol> { new CST.Symbol(new Token(TokenType.Word, word)) });

            string canonicalName = step.GetCanonicalMethodName();

            Assert.That(canonicalName, Is.EqualTo(expectedAnswer));
        }

        [Test]
        public void GetCanonicalMethodName_multiple_words()
        {
            var step = new CST.Step(new List<CST.Symbol> {
                new CST.Symbol(new Token(TokenType.Word, "Hello")),
                new CST.Symbol(new Token(TokenType.Word, "world")),
            });

            string canonicalName = step.GetCanonicalMethodName();

            Assert.That(canonicalName, Is.EqualTo("helloworld"));
        }


        [Test]
        public void GetCanonicalMethodName_avoids_arguments()
        {
            var step = new CST.Step(new List<CST.Symbol> {
                new CST.Symbol(new Token(TokenType.Word, "Hello")),
                new CST.Symbol(new Token(TokenType.Date, "11/16/2017")),
                new CST.Symbol(new Token(TokenType.Word, "today")),
                new CST.Symbol(new Token(TokenType.Number, "11")),
                new CST.Symbol(new Token(TokenType.Word, "times")),
            });

            string canonicalName = step.GetCanonicalMethodName();

            Assert.That(canonicalName, Is.EqualTo("hellotodaytimes"));
        }

    }
}
