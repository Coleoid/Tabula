using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    [TestFixture]
    public class ParserTests_Table : TranspilerUnitTestBase
    {
        [Test]
        public void ParseSection_Table()
        {
            var state = StateFromString("[thistag] \n 'stuff':\n| Name | \n | Bob | \n | Ann | \n");
            CST.Section section = _parser.ParseSection(state);

            Assert.That(section, Is.Not.Null);
            var table = section as CST.Table;
            Assert.That(table, Is.Not.Null);
            Assert.That(table.Tags, Has.Count.EqualTo(1));
            Assert.That(table.Label, Is.EqualTo("stuff"));
            Assert.That(table.ColumnNames, Has.Count.EqualTo(1));
            Assert.That(table.ColumnNames[0], Is.EqualTo("Name"));
            Assert.That(table.Rows, Has.Count.EqualTo(2));
        }

        [Test]
        public void ParseSection_TableRow_line_numbers()
        {
            var state = StateFromString("[thistag] \n 'Friends':\n| Name | \n | Bob | \n | Ann | \n");

            var table = (CST.Table)_parser.ParseSection(state);

            var first = table.Rows.First();
            Assert.That(first.StartLine, Is.EqualTo(4));

            var last = table.Rows.Last();
            Assert.That(last.StartLine, Is.EqualTo(5));
        }

        [Test]
        public void ParseTable_MethodName_with_Tag_and_Label()
        {
            var state = StateFromString("[thistag] \n 'Friends':\n| Name | \n | Bob | \n | Ann | \n");

            var table = (CST.Table)_parser.ParseSection(state);

            Assert.That(table.MethodName, Is.EqualTo("table__003_to_005"));
        }

        [Test]
        public void Row_then_EOF()
        {
            var state = StateFromString("| Fun | Games | Misc |");
            var row = _parser.ParseTableRow(state);
            Assert.That(row, Is.Not.Null);
            Assert.That(row.Cells, Has.Count.EqualTo(3));
            Assert.That(row.Cells.Select(c => c[0]), Is.EquivalentTo(new [] { "Fun", "Games", "Misc" }));
        }

        [Test]
        public void Row_then_newline()
        {
            var state = StateFromString("| Fun | \n foo");
            var row = _parser.ParseTableRow(state);
            Assert.That(row, Is.Not.Null);
            Assert.That(row.Cells, Has.Count.EqualTo(1));
        }

        [Test]
        public void Row_with_multiple_symbols()
        {
            var state = StateFromString("| Fun and games | \n foo");
            var row = _parser.ParseTableRow(state);
            Assert.That(row, Is.Not.Null);
            Assert.That(row.Cells, Has.Count.EqualTo(1));
            Assert.That(row.Cells[0][0], Is.EqualTo("Fun and games"));
        }

        [Test]
        public void TableCell()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Word, "Say"),
                new Token(TokenType.Word, "Hello"),
                new Token(TokenType.TableCellSeparator, "|"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseTableCell(state);

            Assert.That(cst, Has.Count.EqualTo(1));
            Assert.That(cst[0], Is.EqualTo("Say Hello"));
        }

        [Test]
        public void TableCell_empty()
        {
            var tokens = new List<Token> {
                new Token(TokenType.TableCellSeparator, "|"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseTableCell(state);

            Assert.That(cst, Is.Not.Null);
            Assert.That(cst, Is.Empty);
        }

        [Test]
        public void TableCell_two_values_with_comma()
        {
            var tokens = new List<Token> {
                new Token(TokenType.Word, "Say"),
                new Token(TokenType.Word, "Say"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.Word, "Say"),
                new Token(TokenType.TableCellSeparator, "|"),
            };
            var state = new ParserState(tokens);
            var cst = _parser.ParseTableCell(state);

            Assert.That(cst, Has.Count.EqualTo(2));
            Assert.That(cst[0], Is.EqualTo("Say Say"));
            Assert.That(cst[1], Is.EqualTo("Say"));
        }


        //TODO:  work out desired behaviors when column counts don't match.  Several cases.

    }
}
