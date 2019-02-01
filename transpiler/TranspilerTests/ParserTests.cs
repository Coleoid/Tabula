using NUnit.Framework;
using System;
using System.Collections.Generic;
using Tabula.Parse;
using Tabula.CST;

namespace Tabula
{
    [TestFixture]
    public class ParserTests : TranspilerUnitTestBase
    {
        [Test]
        public void Scenario_gets_tags()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Tag, "Laughing Loudly"),
                new Token(TokenType.Tag, "Jumping"),
                new Token(TokenType.ScenarioLabel, "Get active and make noise"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseScenario(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Tags, Has.Count.EqualTo(2));
            Assert.That(cst.Tags[0], Is.EqualTo("Laughing Loudly"));
            Assert.That(cst.Tags[1], Is.EqualTo("Jumping"));
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
                new Token(TokenType.cmd_Use, "Student Enrollment", 10),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.cmd_Use, "Person Search Criteria Workflow"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search"),
                new Token(TokenType.Word, "here"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search", 13),
                new Token(TokenType.Word, "there"),
                new Token(TokenType.NewLine, "\n"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseSection(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.MethodName, Is.EqualTo("paragraph__010_to_013"));
            Assert.That(cst.Label, Is.EqualTo("paragraph__010_to_013"));
            var paragraph = (Paragraph)cst;
            Assert.That(paragraph.Actions, Has.Count.EqualTo(3));
            Assert.That(paragraph.Actions[0], Is.TypeOf<CST.CommandUse>());
        }

        [Test]
        public void Paragraph_with_extra_blank_lines()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Use, "Student Enrollment", 22),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.cmd_Use, "Person Search Criteria Workflow", 23),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search", 24),
                new Token(TokenType.Word, "here", 24),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search", 27),
                new Token(TokenType.Word, "there", 27),
                new Token(TokenType.NewLine, "\n"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseParagraph(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Actions, Has.Count.EqualTo(3));
            Assert.That(cst.Actions[0], Is.TypeOf<CST.CommandUse>());
            Assert.That(cst.Actions[1], Is.TypeOf<CST.Step>());
            Assert.That(cst.Actions[2], Is.TypeOf<CST.Step>());
            Assert.That(cst.MethodName, Is.EqualTo("paragraph__022_to_027"));
        }

        [Test]
        public void Paragraph_gets_suitable_name()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Use, "Student Enrollment", 42),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.cmd_Set, "#Fred", 43),
                new Token(TokenType.String, "Boxley, Frederick"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search", 44),
                new Token(TokenType.Word, "here"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search", 45),
                new Token(TokenType.Word, "there"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search", 46),
                new Token(TokenType.Word, "elsewhere"),
                new Token(TokenType.NewLine, "\n"),
            };
            var state = new ParserState(tokens);

            var para = _parser.ParseParagraph(state);

            Assert.That(para.MethodName, Is.EqualTo("paragraph__042_to_046"));
        }

        [Test]
        public void Paragraph_name_correct_with_block_at_end()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Use, "Student Enrollment", 42),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.cmd_Set, "#Fred", 43),
                new Token(TokenType.String, "Boxley, Frederick"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.cmd_Alias, "Search", 44),
                new Token(TokenType.BlockStart, "...", 44),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.Word, "Search", 45),
                new Token(TokenType.Word, "there"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.BlockEnd, ".", 46),
            };
            var state = new ParserState(tokens);

            var para = _parser.ParseParagraph(state);

            var alias = para.Actions[2] as CST.CommandAlias;
            var block = alias.Action;

            Assert.That(block.StartLine, Is.EqualTo(44));
            Assert.That(block.EndLine, Is.EqualTo(46));

            Assert.That(para.MethodName, Is.EqualTo("paragraph__042_to_046"));
        }

        //TODO:  block_gets_proper_start_and_end_line_numbers()
        //TODO:  alias_gets_proper_start_and_end_line_numbers()


        //TODO:  Paragraph label is mandatory.
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
                new Token(TokenType.Word, "Say") { Line = 7 },
                new Token(TokenType.Word, "Hello"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseStep(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Symbols, Has.Count.EqualTo(2));
            Assert.That(cst.StartLine, Is.EqualTo(7));
            Assert.That(cst.EndLine, Is.EqualTo(7));
        }

        [Test]
        public void Step_with_number_and_date()
        {
            // this number 22 this date ..
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
        public void Step_with_numbers_in_a_list()
        {
            // my lucky numbers 22, 28, 42
            var tokens = new List<Token> {
                new Token(TokenType.Word, "my"),
                new Token(TokenType.Word, "lucky"),
                new Token(TokenType.Word, "numbers"),
                new Token(TokenType.Number, "22"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.Number, "28"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.Number, "42"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseStep(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Symbols, Has.Count.EqualTo(4));
            var list = cst.Symbols[3];
            Assert.That(list is SymbolCollection);
        }

        [Test]
        public void Collection_with_numbers_in_a_list()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Number, "22"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.Number, "28"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.String, "Howdy!"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseCollection(state);

            Assert.That(cst, Is.Not.Null);
            var collection = cst as SymbolCollection;
            Assert.That(collection, Is.Not.Null);

            Assert.That(collection.Values, Has.Count.EqualTo(3));
            Assert.That(collection.Values[0].Type, Is.EqualTo(TokenType.Number));
            Assert.That(collection.Values[2].Type, Is.EqualTo(TokenType.String));
        }

        //FUTURE: Consider treatment of collections of unlike types

        [Test]
        public void Collection_trailing_comma_error()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Number, "22"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.Word, "balloons"),
            };
            var state = new ParserState(tokens);
            var ex = Assert.Throws<Exception>(() => _parser.ParseCollection(state));
            Assert.That(ex.Message, Is.EqualTo("Can only collect terms"));
        }

        [Test]
        public void Collection_trailing_comma_atEnd_error()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Number, "22"),
                new Token(TokenType.Comma, ","),
            };
            var state = new ParserState(tokens);
            var ex = Assert.Throws<Exception>(() => _parser.ParseCollection(state));
            Assert.That(ex.Message, Is.EqualTo("Can only collect terms"));
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

        [TestCase("23")]
        [TestCase("44.4")]
        public void Term_number(string number)
        {
            var tokens = new List<Token> { new Token(TokenType.Number, number) };
            var state = new ParserState(tokens);
            var cst = _parser.ParseTerm(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst.Type, Is.EqualTo(TokenType.Number));
            Assert.That(cst.Text, Is.EqualTo(number));
        }


        [Test]
        public void ParseBlock_returns_null_when_no_match()
        {
            var tokens = new List<Token> { new Token(TokenType.Number, "23") };

            var state = new ParserState(tokens);
            var block = _parser.ParseBlock(state);

            Assert.That(block, Is.Null);
        }

        [Test]
        public void ParseBlock_works_with_an_empty_block()
        {
            var tokens = new List<Token> { new Token(TokenType.BlockStart, "...") { Line = 5 }, new Token(TokenType.BlockEnd, ".") { Line = 6 } };

            var state = new ParserState(tokens);
            var block = _parser.ParseBlock(state);

            Assert.That(block, Is.Not.Null);
            Assert.That(block.StartLine, Is.EqualTo(5));
            Assert.That(block.EndLine, Is.EqualTo(6));
            Assert.That(block.Actions, Has.Count.EqualTo(0));
        }

        [Test]
        public void ParseBlock_includes_actions()
        {
            var tokens = new List<Token> {
                new Token(TokenType.BlockStart, "..."),
                new Token(TokenType.cmd_Use, "Employment Action Edit"),
                new Token(TokenType.BlockEnd, ".")
            };

            var state = new ParserState(tokens);
            var block = _parser.ParseBlock(state);

            Assert.That(block.Actions, Has.Count.EqualTo(1));
        }

        [Test]
        public void ParseBlock_throws_descriptively_when_unclosed()
        {
            var tokens = new List<Token> {
                new Token(TokenType.BlockStart, "..."),
                new Token(TokenType.cmd_Use, "Employment Action Edit"),
            };

            var state = new ParserState(tokens);

            var ex = Assert.Throws<Exception>(() => _parser.ParseBlock(state));
            Assert.That(ex.Message, Is.EqualTo("After the actions in a block, we need a block end, a period."));
        }



        //  alias: "Do the magic" => |
        [Test]
        public void ParseAlias_throws_descriptively_when_not_followed_by_Action()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Alias, "Do the magic"),
                new Token(TokenType.TableCellSeparator, "|"),
            };

            var state = new ParserState(tokens);

            var ex = Assert.Throws<Exception>(() => _parser.ParseCommand_Alias(state));
            Assert.That(ex.Message, Is.EqualTo("The target of an Alias command must be a step or a block of steps."));
        }

        [Test]
        public void ParseAlias_null_case()
        {
            var tokens = new List<Token> { new Token(TokenType.Number, "23") };
            var state = new ParserState(tokens);
            var alias = _parser.ParseCommand_Alias(state);

            Assert.That(alias, Is.Null);

        }

        [Test]
        public void ParseAlias_correct_case()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Alias, "Test should Work"),
                new Token(TokenType.cmd_Use, "Employment Action Edit") { Line = 18 }
            };

            var state = new ParserState(tokens);

            var alias = _parser.ParseCommand_Alias(state);
            Assert.That(alias, Is.Not.Null);
            Assert.That(alias.Name, Is.EqualTo("Test should Work"));
            Assert.That(alias.StartLine, Is.EqualTo(18));
            Assert.That(alias.EndLine, Is.EqualTo(18));

            var useCommand = (CST.CommandUse)alias.Action;
            Assert.That(useCommand, Is.Not.Null);

            Assert.That(useCommand.Workflows, Has.Count.EqualTo(1));
            Assert.That(useCommand.Workflows[0], Is.EqualTo("Employment Action Edit"));
        }

        [Test]
        public void ParseSet_gets_proper_start_and_end_line_numbers()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Set, "foo", 18),
                new Token(TokenType.String, "Blargh", 18)
            };

            var state = new ParserState(tokens);

            var cmdSet = _parser.ParseCommand_Set(state);
            Assert.That(cmdSet, Is.Not.Null);
            Assert.That(cmdSet.Name, Is.EqualTo("foo"));
            Assert.That(cmdSet.Term.Text, Is.EqualTo("Blargh"));
            Assert.That(cmdSet.StartLine, Is.EqualTo(18));
            Assert.That(cmdSet.EndLine, Is.EqualTo(18));
        }


        [Test]
        public void ParseSet_returns_null_when_not_()
        {
            var tokens = new List<Token> {
                new Token(TokenType.String, "foo", 18),
                new Token(TokenType.cmd_Set, "bar", 19),
            };

            var state = new ParserState(tokens);

            var ex = Assert.Throws<Exception>(() => _parser.ParseCommand_Set(state));
            Assert.That(ex, Has.Message.EqualTo("Needed a Set command, got a [String]."));
        }

        [Test]
        public void ParseSet_throws_descriptively_when_not_followed_by_Action()
        {
            var tokens = new List<Token> {
                new Token(TokenType.cmd_Set, "foo", 18),
                new Token(TokenType.cmd_Set, "bar", 19),
            };

            var state = new ParserState(tokens);

            var ex = Assert.Throws<Exception>(() => _parser.ParseCommand_Set(state));
            Assert.That(ex, Has.Message.StartWith("The target of a Set command must be a term"));
        }



        [Test]
        public void Parse_simplest_table()
        {
            var tokens = new List<Token> {
                new Token(TokenType.TableCellSeparator, "|"),
                new Token(TokenType.Word, "January"),
                new Token(TokenType.TableCellSeparator, "|"),
                new Token(TokenType.NewLine, "\n"),
            };

            var state = new ParserState(tokens);

            var table = _parser.ParseTable(state);
            Assert.That(table, Is.Not.Null);
            Assert.That(table.ColumnNames.Count, Is.EqualTo(1));
            Assert.That(table.ColumnNames[0], Is.EqualTo("January"));
            Assert.That(table.Rows.Count, Is.EqualTo(0));
        }

        [Test]
        public void Parse_less_simplest_table()
        {
            var tokens = new List<Token> {
                new Token(TokenType.TableCellSeparator, "|"),
                new Token(TokenType.Word, "January"),
                new Token(TokenType.TableCellSeparator, "|"),
                new Token(TokenType.NewLine, "\n"),
                new Token(TokenType.TableCellSeparator, "|"),
                new Token(TokenType.Variable, "another_month"),
                new Token(TokenType.TableCellSeparator, "|"),
                new Token(TokenType.NewLine, "\n"),
            };

            var state = new ParserState(tokens);

            var table = _parser.ParseTable(state);
            Assert.That(table, Is.Not.Null);
            Assert.That(table.Rows.Count, Is.EqualTo(1));
            var row = table.Rows[0];
            Assert.That(row.Cells.Count, Is.EqualTo(1));
            Assert.That(row.Cells[0].Count, Is.EqualTo(1));
            Assert.That(row.Cells[0][0], Is.EqualTo("#another_month"));
        }
    }
}
