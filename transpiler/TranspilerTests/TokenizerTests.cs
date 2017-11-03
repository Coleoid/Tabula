using NUnit.Framework;
using System.Linq;

namespace Tabula
{
    [TestFixture]
    public class TokenizerTests : TranspilerUnitTestBase
    {
        [Test]
        public void Empty_input_produces_no_tokens_or_warnings()
        {
            var tokens = _tokenizer.Tokenize("");
            Assert.That(tokens, Has.Count.EqualTo(0));
            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(0));
        }

        [Test]
        public void Unrecognized_token_skips_remaining_input_and_warns()
        {
            var tokens = _tokenizer.Tokenize(" \n  %%% 'Hello, World!'");
            Assert.That(tokens, Has.Count.EqualTo(1));
            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(1));

            var warning = _tokenizer.Warnings[0];
            Assert.That(warning, Is.EqualTo("Text not tokenizable on line 2, at:\n%%% 'Hello, World!'\n"));
        }

        [Test]
        public void Skips_leading_whitespace()
        {
            var tokens = _tokenizer.Tokenize("   'Hello, World!'");
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
        }

        [TestCase("scenario: What am I doing?", "What am I doing?")]
        [TestCase("Scenario:", "")]
        [TestCase("Scenario: \"duuude.\"", "duuude.")]
        public void Scenario_label(string line, string expectedText)
        {
            var tokens = _tokenizer.Tokenize(line);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.ScenarioLabel));
            Assert.That(token.Text, Is.EqualTo(expectedText));
        }

        [TestCase("'What am I doing?':  ", "What am I doing?")]
        [TestCase(@"""This thing here"":", "This thing here")]
        public void Section_label(string line, string expectedText)
        {
            var tokens = _tokenizer.Tokenize(line);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.SectionLabel));
            Assert.That(token.Text, Is.EqualTo(expectedText));
        }

        [TestCase("'Hello, World!'", "Hello, World!")]
        [TestCase("\"Hello, World!\"", "Hello, World!")]
        public void String(string text, string expected)
        {
            var tokens = _tokenizer.Tokenize(text);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }
        //TODO: Strings containing escaped quotes

        [Test]
        public void Tag()
        {
            var tokens = _tokenizer.Tokenize("[serious business]");
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.Tag));
            Assert.That(token.Text, Is.EqualTo("serious business"));
        }

        [Test]
        public void Variable()
        {
            var tokens = _tokenizer.Tokenize("#business");
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.Variable));
            Assert.That(token.Text, Is.EqualTo("business"));
        }

        [TestCase("...", TokenType.BlockStart)]
        [TestCase(".", TokenType.BlockEnd)]
        public void Block(string text, TokenType type)
        {
            var tokens = _tokenizer.Tokenize(text);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(type));
            Assert.That(token.Text, Is.EqualTo(text));
        }

        [Test]
        public void Tags_in_single_set_of_brackets()
        {
            var tokens = _tokenizer.Tokenize("[serious,monkey,   risky]");
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
            var tokens = _tokenizer.Tokenize(input);
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
            var tokens = _tokenizer.Tokenize(input);
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
            var tokens = _tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Word));
            Assert.That(token.Text, Is.EqualTo(input));
        }

        [TestCase("use:Organization FNH Management", "Organization FNH Management")]
        [TestCase("use: Global Setting Management", "Global Setting Management")]
        [TestCase("use:  Employment  Action Edit ", "Employment  Action Edit ")]
        public void Command_use(string input, string expected)
        {
            var tokens = _tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.cmd_Use));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase("set: this => #that", "this", "that")]
        public void Command_Set(string input, string from, string to)
        {
            var tokens = _tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(2));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.cmd_Set));
            Assert.That(token.Text, Is.EqualTo(from));

            token = tokens[1];
            Assert.That(token.Type, Is.EqualTo(TokenType.Variable));
            Assert.That(token.Text, Is.EqualTo(to));
        }

        [TestCase("alias: this => #that", "this", "that")]
        public void Command_Alias(string input, string from, string to)
        {
            var tokens = _tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(2));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.cmd_Alias));
            Assert.That(token.Text, Is.EqualTo(from));

            token = tokens[1];
            Assert.That(token.Type, Is.EqualTo(TokenType.Variable));
            Assert.That(token.Text, Is.EqualTo(to));
        }

        [TestCase("|")]
        public void Table_cell_separator(string input)
        {
            var tokens = _tokenizer.Tokenize(input);
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.TableCellSeparator));
            Assert.That(token.Text, Is.EqualTo(input));
        }

        [Test]
        public void Words_and_NewLines()
        {
            var tokens = _tokenizer.Tokenize("one \n two");
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
            var tokens = _tokenizer.Tokenize("| Name | Age |");
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
            var tokens = _tokenizer.Tokenize("| Name | \n | Bob | \n | Ann | \n");
            Assert.That(tokens, Has.Count.EqualTo(12));
        }

    }
}
