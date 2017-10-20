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
        public void ParseTable()
        {
            var state = StateFromString("\"stuff\":\n| Name | \n | Bob | \n | Ann | \n");
            CST.Table table = parser.ParseTable(state);

            Assert.That(table, Is.Not.Null);
        }

        [Test]
        public void ParseTableRow()
        {
            var state = StateFromString("| Fun | Games | Misc |");
            List<string> row = parser.ParseTableRow(state);
            Assert.That(row, Is.Not.Null);
        }


        //  In general, a table is not required at any particular point,
        //  so if it's not found, we get a null, not an abort.
        [Test]
        public void ParseTable_rolls_back_state_if_table_is_not_next()
        {
            Assert.Fail();
        }

        //  Since a table label doesn't look like anything else, finding one
        //  without a table will block us from parsing further.
        //[Test]
        //public void Parse_abort_on_label_with_no_column_name_row()
        //{
        //    var tokens = new List<Token> {
        //        new Token(TokenType.SectionHeader, "Olsen's Standard Book of British Birds"),
        //        new Token(TokenType.String, "The one without the Gannet"),
        //    };
        //    var state = new ParserState(tokens);
        //    var ex = Assert.Throws<Exception>(() => parser.ParseTable(state));
        //    Assert.That(ex.Message, Is.EqualTo("A table must have a row of column names"));
        //}

        //TODO: mull whether this table abort (and similar) are good signs or not.

        //TODO: figure out desired behaviors when header and row lengths don't match.  Several cases.

    }
}
