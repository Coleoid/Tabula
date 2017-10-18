using NUnit.Framework;
using System.Collections.Generic;

namespace Tabula
{
    public class ParserTestBase
    {
        protected Parser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new Parser();
        }
    }

    [TestFixture]
    public class ParserTests : ParserTestBase
    {
        [Test]
        public void Term_date()
        {
            var tokens = new List<Token> { new Token(TokenType.Date, "09/30/2016") };
            var state = new ParserState(tokens);
            var cst = parser.ParseTerm(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Type, Is.EqualTo(TokenType.Date));
            Assert.That(cst.Text, Is.EqualTo("09/30/2016"));
        }

        [Test]
        public void Term_number()
        {
            var tokens = new List<Token> { new Token(TokenType.Number, "23") };
            var state = new ParserState(tokens);
            var cst = parser.ParseTerm(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Type, Is.EqualTo(TokenType.Number));
            Assert.That(cst.Text, Is.EqualTo("23"));
        }

        [Test]
        public void Scenario_gets_tags()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Tag, "Laughing"),
                new Token(TokenType.Tag, "Jumping"),
                new Token(TokenType.ScenarioLabel, "Get active and make noise"),
            };
            var state = new ParserState(tokens);
            var cst = parser.ParseScenario(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Tags, Has.Count.EqualTo(2));
            Assert.That(cst.Sections, Is.Empty);
        }

        [Test]
        public void Section_gets_tags()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Tag, "Enrollment"),
                new Token(TokenType.Tag, "AC-98989"),
                new Token(TokenType.SectionHeader, ""),
                new Token(TokenType.UseCommand, "Student Enrollment"),
            };
            var state = new ParserState(tokens);
            var cst = parser.ParseSection(state);
            Assert.That(cst, Is.Not.Null);

            var paragraph = cst as CST.Paragraph;
            Assert.That(cst.Tags, Has.Count.EqualTo(2));
        }

        [Test]
        public void Section_gets_label()
        {
            string label = "Enroll Bob in Early Structure Fire Detection";
            var tokens = new List<Token> {
                new Token(TokenType.SectionHeader, label),
                new Token(TokenType.UseCommand, "Student Enrollment"),
            };
            var state = new ParserState(tokens);
            var cst = parser.ParseSection(state);
            Assert.That(cst, Is.Not.Null);

            var paragraph = cst as CST.Paragraph;
            Assert.That(cst.Label, Is.EqualTo(label));
        }
    }
}
