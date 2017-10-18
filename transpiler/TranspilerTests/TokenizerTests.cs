using NUnit.Framework;
using System.Linq;

namespace Tabula
{
    public class TokenizerTestBase
    {
        protected Tokenizer tokenizer;

        [SetUp]
        public void SetUp()
        {
            tokenizer = new Tokenizer();
        }
    }

    [TestFixture]
    public class TokenizerTests : TokenizerTestBase
    {
        [Test]
        public void Empty_input_produces_no_tokens_or_warnings()
        {
            var tokens = tokenizer.Tokenize("");
            Assert.That(tokens, Has.Count.EqualTo(0));
            Assert.That(tokenizer.Warnings, Has.Count.EqualTo(0));
        }

        [Test]
        public void Unrecognized_token_skips_remaining_input_and_warns()
        {
            var tokens = tokenizer.Tokenize(" \n  %%% 'Hello, World!'");
            Assert.That(tokens, Has.Count.EqualTo(0));
            Assert.That(tokenizer.Warnings, Has.Count.EqualTo(1));

            var warning = tokenizer.Warnings[0];
            Assert.That(warning, Is.EqualTo("Unrecognized token at position 4."));
            //TODO: Assert.That(warning, Is.EqualTo("Unrecognized token [%%%] at line 2, column 3."));
        }

        [Test]
        public void Skips_leading_whitespace()
        {
            var tokens = tokenizer.Tokenize("   'Hello, World!'");
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
        }

        [TestCase("scenario: What am I doing?", "What am I doing?")]
        [TestCase("Scenario:", "")]
        public void Scenario_label(string line, string expectedText)
        {
            var tokens = tokenizer.Tokenize(line);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.ScenarioLabel));
            Assert.That(token.Text, Is.EqualTo(expectedText));
        }

        [Test]
        public void String()
        {
            var tokens = tokenizer.Tokenize("'Hello, World!'");
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo("Hello, World!"));
        }
        //TODO: Double-quoted strings
        //TODO: Strings containing escaped quotes

        [Test]
        public void Tag()
        {
            var tokens = tokenizer.Tokenize("[serious business]");
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.Tag));
            Assert.That(token.Text, Is.EqualTo("serious business"));
        }

        [Test]
        public void Tags_in_single_set_of_brackets()
        {
            var tokens = tokenizer.Tokenize("[serious, monkey, risky]");
            Assert.That(tokens, Has.Count.EqualTo(3));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Tag));
            Assert.That(token.Text, Is.EqualTo("serious"));

            token = tokens[1];
            Assert.That(token.Type, Is.EqualTo(TokenType.Tag));
            Assert.That(token.Text, Is.EqualTo("monkey"));

            token = tokens[2];
            Assert.That(token.Type, Is.EqualTo(TokenType.Tag));
            Assert.That(token.Text, Is.EqualTo("risky"));
        }

        //[Test]
        //public void TableRow()
        //{
        //    var tokens = tokenizer.Tokenize("| Name | Age |");
        //    Assert.That(tokens, Has.Count.EqualTo(1));

        //    Token token = tokens[0];
        //    Assert.That(token.Type, Is.EqualTo(TokenType.TableRow));
        //    Assert.That(token.Parts.Count, Is.EqualTo(2));
        //    Assert.That(token.Parts[0], Is.EqualTo("Name"));
        //    Assert.That(token.Parts[1], Is.EqualTo("Age"));
        //}
        //...the row tests may demonstrate that I'm hoping for too much from my tokenizer.
        //...especially once I'm testing rows containing lists and variables.

        //TODO:  Differentiate a scenario label from a steplike phrase...  Start and end anchors?  Colon?
    }
}
