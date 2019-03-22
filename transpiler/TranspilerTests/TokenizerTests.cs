using NUnit.Framework;
using System.Linq;
using Tabula.CST;

namespace Tabula
{
    [TestFixture]
    public class TokenizerTests : TranspilerUnitTestBase
    {
        [Test]
        public void Empty_input_produces_no_tokens_or_warnings()
        {
            var output = _tokenizer.Tokenize("");
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(0));
            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(0));
        }

        [TestCase("'What am I doing?':  ")]
        [TestCase("'Hello, World!' ")]
        public void Whitespace_before_EOF_not_a_problem(string text)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));
            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(0));
        }

        [Test]
        public void Unrecognized_token_is_added_to_unregognized_collection_and_parse_resumes()
        {
            var output = _tokenizer.Tokenize(" \n  % 'Hello, World!'");
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(2));
            Assert.That(tokens[1].Text, Is.EqualTo("Hello, World!"));
            Assert.That(output.UnrecognizedTokens, Has.Count.EqualTo(1));
            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(1));

            var warning = _tokenizer.Warnings[0];
            Assert.That(warning, Is.EqualTo("Text not tokenizable on line 2 column 3, at:\n% 'Hello, World!'\n"));
            Assert.That(output.UnrecognizedTokens[0].Text, Is.EqualTo("%"));
            Assert.That(output.UnrecognizedTokens[0].Type, Is.EqualTo(TokenType.Unrecognized));
        }

        [Test]
        public void Unrecognized_token_length_correctly_recorded()
        {
            var output = _tokenizer.Tokenize(" \n  %$#*^%#&%^( 'Hello, World!'");
            var tokens = output.Tokens;
            Assert.That(tokens[1].Text, Is.EqualTo("Hello, World!"));
            Assert.That(output.UnrecognizedTokens, Has.Count.EqualTo(1));
            Assert.That(output.UnrecognizedTokens[0].Text, Is.EqualTo("%$#*^%#&%^("));
            Assert.That(output.UnrecognizedTokens[0].FullLength, Is.EqualTo(11));
        }

        [Test]
        public void Skips_leading_whitespace()
        {
            var output = _tokenizer.Tokenize("   'Hello, World!'");
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo("Hello, World!"));
        }

        [TestCase("scenario: What am I doing?", "What am I doing?")]
        [TestCase("Scenario:", "")]
        [TestCase("Scenario: \"duuude.\"", "duuude.")]
        public void Scenario_label(string line, string expectedText)
        {
            var output = _tokenizer.Tokenize(line);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.ScenarioLabel));
            Assert.That(token.Text, Is.EqualTo(expectedText));
        }

        [TestCase("'What am I doing?':  ", "What am I doing?")]
        [TestCase(@"""This thing here"":", "This thing here")]
        public void Section_label(string line, string expectedText)
        {
            var output = _tokenizer.Tokenize(line);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.SectionLabel));
            Assert.That(token.Text, Is.EqualTo(expectedText));
        }

        [TestCase("\"Hello, World!\"", "Hello, World!")]
        [TestCase("\"Hello, #location!\"", "Hello, #location!")]
        [TestCase("\"#itemType\"", "#itemType")]
        public void String_dq(string text, string expected)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase("'Hello, World!'", "Hello, World!")]
        [TestCase("'\\t'", "\\t")]
        [TestCase("'\\n'", "\\n")]
        [TestCase("'\\\\'", "\\\\")]
        public void String_sq_never_escapes_with_backslash(string text, string expected)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase(@"""I contain 'single' quotes.""", "I contain 'single' quotes.")]
        [TestCase(@"'I contain ""double"" quotes.'", @"I contain ""double"" quotes.")]
        [TestCase(@"""I contain `backticks`.""", @"I contain `backticks`.")]
        public void String_including_other_quotes(string text, string expected)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase(@"'unescaped backslash: \ '", @"unescaped backslash: \ ")]
        [TestCase(@"'escaped single quote: '' '", @"escaped single quote: ' ")]
        [TestCase(@"'Angus O''Brady'", @"Angus O'Brady")]
        [TestCase(@"""escaped double quote: \"" """, @"escaped double quote: "" ")]
        public void String_Escaping(string text, string expected)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase(@"""no final dq")]
        [TestCase(@"'no final sq")]
        [TestCase(@"`no final bt")]
        [TestCase(@"""final dq is escaped: \""")]
        [TestCase(@"'final sq is escaped: ''")]
        [TestCase(@"`final bt is escaped: ``")]
        public void Plausible_but_not_strings(string text)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;

            Assert.That(tokens[0].Type, Is.Not.EqualTo(TokenType.String));
            Assert.That(output.UnrecognizedTokens.Count, Is.GreaterThan(0));
        }

        [TestCase("newline", @"""\n""", "\n")]
        [TestCase("carriage return", @"""\r""", "\r")]
        [TestCase("tab", @"""\t""", "\t")]
        [TestCase("backslash", @""" \ """, " \\ ")]
        public void Special_escape_sequences_generate_special_characters(string name, string text, string expected)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase("empty string", "``", "")]
        [TestCase("empty string", "`\n`", "\n")]
        [TestCase("escaped backtick", "` `` `", " ` ")]
        [TestCase("empty string", "`\n\n stuff \n`", "\n\n stuff \n")]
        public void Multi_Line_Strings(string name, string text, string expected)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase(@"""\a""", "\\a")]
        [TestCase(@"""\A""", "\\A")]
        public void Most_backslashed_characters_are_just_the_character_and_a_backslash(string text, string expected)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase(@"""\#""", "\\#")]
        public void Backslashed_number_sign_retains_literal_backslash(string text, string expected)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [Test]
        public void Tag()
        {
            var output = _tokenizer.Tokenize("[serious business]");
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.Tag));
            Assert.That(token.Text, Is.EqualTo("serious business"));
        }

        [TestCase("#BIZ","biz")]
        public void Variable(string rawText, string varName)
        {
            var output = _tokenizer.Tokenize(rawText);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(TokenType.Variable));
            Assert.That(token.Text, Is.EqualTo(varName));
        }

        [TestCase("...", TokenType.BlockStart)]
        [TestCase(".", TokenType.BlockEnd)]
        public void Block(string text, TokenType type)
        {
            var output = _tokenizer.Tokenize(text);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens.First();
            Assert.That(token.Type, Is.EqualTo(type));
            Assert.That(token.Text, Is.EqualTo(text));
        }

        [Test]
        public void Tags_in_single_set_of_brackets()
        {
            var output = _tokenizer.Tokenize("[serious,monkey,   risky]");
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Tag));

            Assert.That(token.Parts[0], Is.EqualTo("serious"));
            Assert.That(token.Parts[1], Is.EqualTo("monkey"));
            Assert.That(token.Parts[2], Is.EqualTo("risky"));
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
            var output = _tokenizer.Tokenize(input);
            var tokens = output.Tokens;
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
            var output = _tokenizer.Tokenize(input);
            var tokens = output.Tokens;
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
            var output = _tokenizer.Tokenize(input);
            var tokens = output.Tokens;
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
            var output = _tokenizer.Tokenize(input);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.cmd_Use));
            Assert.That(token.Text, Is.EqualTo(expected));
        }

        [TestCase("set: this => #that", "this", "that")]
        public void Command_Set(string input, string from, string to)
        {
            var output = _tokenizer.Tokenize(input);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(2));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.cmd_Set));
            Assert.That(token.Text, Is.EqualTo(from));
            Assert.That(token.Line, Is.EqualTo(1));

            token = tokens[1];
            Assert.That(token.Type, Is.EqualTo(TokenType.Variable));
            Assert.That(token.Text, Is.EqualTo(to));
            Assert.That(token.Line, Is.EqualTo(1));
        }

        [TestCase("alias: this => #that", "this", "that")]
        public void Command_Alias(string input, string from, string to)
        {
            var output = _tokenizer.Tokenize(input);
            var tokens = output.Tokens;
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
            var output = _tokenizer.Tokenize(input);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(1));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.TableCellSeparator));
            Assert.That(token.Text, Is.EqualTo(input));
        }

        [Test]
        public void Words_and_NewLines()
        {
            var output = _tokenizer.Tokenize("one \n two");
            var tokens = output.Tokens;
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


        [TestCase(@"Create ""#itemType"" list item ""Testopia #item"" with description ""Testopia #item description"" with usage ""Available for new records""")]
        public void Full_Step(string step)
        {
            var output = _tokenizer.Tokenize(step);
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(11));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Word));
            Assert.That(token.Text, Is.EqualTo("Create"));

            token = tokens[1];
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo("#itemType"));
        }

        [Test]
        public void multiple_strings()
        {
            string step = @"one ""string one"" two ""string two"" three";
            var output = _tokenizer.Tokenize(step);
            var tokens = output.Tokens;
            //Assert.That(tokens, Has.Count.EqualTo(5));

            Token token = tokens[0];
            Assert.That(token.Type, Is.EqualTo(TokenType.Word));
            Assert.That(token.Text, Is.EqualTo("one"));

            token = tokens[1];
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo("string one"));

            token = tokens[2];
            Assert.That(token.Type, Is.EqualTo(TokenType.Word));
            Assert.That(token.Text, Is.EqualTo("two"));

            token = tokens[3];
            Assert.That(token.Type, Is.EqualTo(TokenType.String));
            Assert.That(token.Text, Is.EqualTo("string two"));
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
            var output = _tokenizer.Tokenize("| Name | Age |");
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(5));

            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.TableCellSeparator));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.TableCellSeparator));
            Assert.That(tokens[4].Type, Is.EqualTo(TokenType.TableCellSeparator));

            Assert.That(tokens[2].FullLength, Is.EqualTo(1));
            Assert.That(tokens[2].StartPosition, Is.EqualTo(7));  //  Note, position is zero-based
            Assert.That(tokens[2].Line, Is.EqualTo(1));           //  while line and column are one-based
            Assert.That(tokens[2].Column, Is.EqualTo(8));

            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Word));  //TODO: becomes phrase later
            Assert.That(tokens[1].Text, Is.EqualTo("Name"));

            Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Word));  //TODO: becomes phrase later
            Assert.That(tokens[3].Text, Is.EqualTo("Age"));
        }

        [Test]
        public void TableRows()
        {
            var output = _tokenizer.Tokenize("| Name | \n | Bob | \n | Ann | \n");
            var tokens = output.Tokens;
            Assert.That(tokens, Has.Count.EqualTo(12));
        }

    }
}
