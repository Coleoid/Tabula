using NUnit.Framework;
using System.Linq;

namespace Tabula
{
    [TestFixture]
    public class TokenizerTests
    {

        //TODO:  Extend with other acceptable cases
        [Test]
        public void Scenario_label()
        {
            var tokenizer = new Tokenizer();

            var tokens = tokenizer.Tokenize("Scenario: What am I doing?");
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.ScenarioLabel));
            Assert.That(token.Text, Is.EqualTo("What am I doing?"));
        }

        //Q:  How do I differentiate a scenario label from a simple phrase?

        //[Test]
        //public void Scenario_label_empty_when_none_in_text()
        //{}

        [Test]
        public void String()
        {
            var tokenizer = new Tokenizer();

            var tokens = tokenizer.Tokenize("'Hello, World!'");
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo("Hello, World!"));
        }

    }
}
