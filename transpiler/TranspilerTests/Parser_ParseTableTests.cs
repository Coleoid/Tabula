using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class Parser_ParseTableTests : ParserTestBase
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

        public ParserState StateFromString(string inputText)
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(inputText);
            return new ParserState(tokens);
        }

        [Test]
        public void ParseSection_Table()
        {
            var state = StateFromString("[thistag] \n 'stuff':\n| Name | \n | Bob | \n | Ann | \n");
            CST.Section section = parser.ParseSection(state);

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
            List<string> row = parser.ParseTableRow(state);
            Assert.That(row, Is.Not.Null);
            Assert.That(row, Has.Count.EqualTo(3));
            Assert.That(row, Is.EquivalentTo(new [] { "Fun", "Games", "Misc" }));
        }

        [Test]
        public void Row_then_newline()
        {
            var state = StateFromString("| Fun | \n foo");
            List<string> row = parser.ParseTableRow(state);
            Assert.That(row, Is.Not.Null);
            Assert.That(row, Has.Count.EqualTo(1));
        }

        [Test]
        public void Row_with_multiple_symbols()
        {
            var state = StateFromString("| Fun and games | \n foo");
            List<string> row = parser.ParseTableRow(state);
            Assert.That(row, Is.Not.Null);
            Assert.That(row, Has.Count.EqualTo(1));
            Assert.That(row[0], Is.EqualTo("Fun and games"));
        }


        //TODO:  work out desired behaviors when column counts don't match.  Several cases.

    }
}
