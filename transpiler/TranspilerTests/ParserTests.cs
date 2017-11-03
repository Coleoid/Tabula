using NUnit.Framework;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class ParserTests : TranspilerUnitTestBase
    {
        [Test]
        public void Scenario_gets_tags()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Tag, "Laughing"),
                new Token(TokenType.Tag, "Jumping"),
                new Token(TokenType.ScenarioLabel, "Get active and make noise"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseScenario(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Tags, Has.Count.EqualTo(2));
        }

        [Test]
        public void Section_gets_tags()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Tag, "Enrollment"),
                new Token(TokenType.Tag, "AC-98989"),
                new Token(TokenType.SectionLabel, ""),
                new Token(TokenType.cmd_Use, "Student Enrollment"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseSection(state);
            Assert.That(cst, Is.Not.Null);

            Assert.That(cst.Tags, Has.Count.EqualTo(2));
        }

        [Test]
        public void Section_gets_label()
        {
            string label = "Enroll Bob in Early Structure Fire Detection";
            var tokens = new List<Token> {
                new Token(TokenType.SectionLabel, label),
                new Token(TokenType.cmd_Use, "Student Enrollment"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseSection(state);
            Assert.That(cst, Is.Not.Null);

            Assert.That(cst.Label, Is.EqualTo(label));
        }

        [Test]
        public void Paragraph()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Use, "Student Enrollment"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.cmd_Use, "Person Search Criteria Workflow"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search"),
                new Token(TokenType.Word, "here"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search"),
                new Token(TokenType.Word, "there"),
                new Token(TokenType.NewLine, "\n"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseParagraph(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Actions, Has.Count.EqualTo(3));
            Assert.That(cst.Actions[0], Is.TypeOf<CST.CommandUse>());
        }

        [Test]
        public void Paragraph_with_set_and_alias()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Use, "Student Enrollment"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.cmd_Set, "#Fred"),
                new Token(TokenType.String, "Boxley, Frederick"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.cmd_Alias, "Search"),
                new Token(TokenType.BlockStart, "..."),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search"),
                new Token(TokenType.Word, "there"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.BlockEnd, "."),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseParagraph(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Actions, Has.Count.EqualTo(3));
        }


        [Test]
        public void Step_two_words()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Word, "Say"),
                new Token(TokenType.Word, "Hello"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseStep(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Symbols, Has.Count.EqualTo(2));
        }

        [Test]
        public void Step_with_number_and_date()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Word, "this"),
                new Token(TokenType.Word, "number"),
                new Token(TokenType.Number, "22"),
                new Token(TokenType.Word, "this"),
                new Token(TokenType.Word, "date"),
                new Token(TokenType.Date, "11/22/2044"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseStep(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Symbols, Has.Count.EqualTo(6));
        }

        [Test]
        public void Step_with_variable()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Word, "this"),
                new Token(TokenType.Word, "person"),
                new Token(TokenType.Variable, "#nextPerson"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseStep(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Symbols, Has.Count.EqualTo(3));
            Assert.That(cst.Symbols[2].Type, Is.EqualTo(TokenType.Variable));
        }

        [Test]
        public void Section_header()
        {
            var tokens = new List<Token> {
                new Token(TokenType.SectionLabel, "Search people with many different duty assignment criteria"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.SectionLabel, "Search people with 5 duty assignment criteria"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseSectionLabel(state);

            Assert.That(cst.Text, Is.EqualTo("Search people with many different duty assignment criteria"));

            cst = _parser.ParseSectionLabel(state);
            Assert.That(cst.Text, Is.EqualTo("Search people with 5 duty assignment criteria"));
        }

        [Test]
        public void Steps()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Word, "Search"),
                new Token(TokenType.Word, "here"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search"),
                new Token(TokenType.Word, "there"),
                new Token(TokenType.NewLine, "\n"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseSteps(state);

            Assert.That(cst, Is.Not.Null);

            Assert.That(cst, Has.Count.EqualTo(2));
        }

        [Test]
        public void Term_date()
        {
            var tokens = new List<Token> { new Token(TokenType.Date, "09/30/2016") };
            var state = new ParserState(tokens);
            var cst = _parser.ParseTerm(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Type, Is.EqualTo(TokenType.Date));
            Assert.That(cst.Text, Is.EqualTo("09/30/2016"));
        }

        [Test]
        public void Term_number()
        {
            var tokens = new List<Token> { new Token(TokenType.Number, "23") };
            var state = new ParserState(tokens);
            var cst = _parser.ParseTerm(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Type, Is.EqualTo(TokenType.Number));
            Assert.That(cst.Text, Is.EqualTo("23"));
        }
    }
}
