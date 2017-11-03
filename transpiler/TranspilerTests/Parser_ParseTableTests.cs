using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    [TestFixture]
    public class Parser_ParseTableTests : TranspilerTestBase
    {
        ////  Label is optional, data rows are optional.
        //[Test]
        //public void Minimal_table_only_has_only_a_row_of_column_names()
        //{
        //    var tokens = new List<Token> {
        //        new Token(TokenType.TableRow, new List<string> {
        //            "Name",
        //            "Age",
        //        }),
        //    };
        //    var state = new ParserState(tokens);
        //    var table = parser.ParseTable(state);

        //    Assert.That(table, Is.Not.Null);
        //    Assert.That(table.ColumnNames[0], Is.EqualTo("Name"));
        //    Assert.That(table.ColumnNames[1], Is.EqualTo("Age"));
        //    Assert.That(table.Label, Is.Null);
        //    Assert.That(table.Rows.Count, Is.Zero);
        //}

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
            Assert.That(table.Rows, Has.Count.EqualTo(2));
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
