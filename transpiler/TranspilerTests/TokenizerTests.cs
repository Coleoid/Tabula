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
            var tokens = tokenizer.Tokenize("[serious,monkey,   risky]");
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

        [TestCase("123")]
        [TestCase("-123")]
        [TestCase("12.34")]
        [TestCase("-12.34")]
        [TestCase("0.")]
        [TestCase(".2")]
        [TestCase("-.2")]
        public void Term_number(string input)
        {
            var tokens = tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Number));
            Assert.That(token.Text, Is.EqualTo(input));
        }


        [TestCase("12/25/2023")]
        [TestCase("1/2/99")]
        [TestCase("1/2/1999")]
        public void Term_date(string input)
        {
            var tokens = tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Date));
            Assert.That(token.Text, Is.EqualTo(input));
        }

        [TestCase("CamelCase")]
        [TestCase("Underscore_Case")]
        [TestCase("digitty_239")]
        public void Word(string input)
        {
            var tokens = tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Word));
            Assert.That(token.Text, Is.EqualTo(input));
        }

        //>use: Global Setting Management
        [TestCase(">use: Global Setting Management\n", "Global Setting Management")]
        [TestCase(">use:Organization FNH Management\n", "Organization FNH Management")]
        [TestCase(">use: Employment  Action Edit \n", "Employment  Action Edit ")]
        public void Use_command(string input, string expected)
        {
            var tokens = tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.UseCommand));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase("|")]
        public void Table_cell_separator(string input)
        {
            var tokens = tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.TableCellSeparator));
            Assert.That(token.Text, Is.EqualTo(input));
        }

        [Test]
        public void Words_and_NewLines()
        {
            var tokens = tokenizer.Tokenize("one \n two");
            Assert.That(tokens, Has.Count.EqualTo(3));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Word));
            Assert.That(token.Text, Is.EqualTo("one"));

            token = tokens[1];
            Assert.That(token.Type, Is.EqualTo(TokenType.NewLine));

            token = tokens[2];
            Assert.That(token.Type, Is.EqualTo(TokenType.Word));
            Assert.That(token.Text, Is.EqualTo("two"));
        }



        //TODO: require a boundary after some tokens, and not after others.
        //  specifically, this case currently finds two tokens, and it should complain instead.
        //[TestCase("3part")]
        //public void not_words(string input)
        //{
        //    var tokens = tokenizer.Tokenize(input);
        //    Assert.That(tokens, Has.Count.EqualTo(1));

        //    Token token = tokens[0];
        //    Assert.That(token.Type, Is.Not.EqualTo(TokenType.Word));
        //}


        [Test]
        public void TableRow()
        {
            var tokens = tokenizer.Tokenize("| Name | Age |");
            Assert.That(tokens, Has.Count.EqualTo(5));

            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.TableCellSeparator));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.TableCellSeparator));
            Assert.That(tokens[4].Type, Is.EqualTo(TokenType.TableCellSeparator));

            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Word));  //TODO: becomes phrase later
            Assert.That(tokens[1].Text, Is.EqualTo("Name"));

            Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Word));  //TODO: becomes phrase later
            Assert.That(tokens[3].Text, Is.EqualTo("Age"));
        }

        [Test]
        public void TableRows()
        {
            var tokens = tokenizer.Tokenize("| Name | \n | Bob | \n | Ann | \n");
            Assert.That(tokens, Has.Count.EqualTo(12));
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
