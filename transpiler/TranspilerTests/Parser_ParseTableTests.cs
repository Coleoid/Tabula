﻿using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class Parser_ParseTableTests : ParserTestBase
    {
        //  Label is optional, data rows are optional.
        [Test]
        public void Minimal_table_only_has_only_a_row_of_column_names()
        {
            var tokens = new List<Token> {
                new Token(TokenType.TableRow, new List<string> {
                    "Name",
                    "Age",
                }),
            };
            var state = new ParserState(tokens);
            var table = parser.ParseTable(state);

            Assert.That(table, Is.Not.Null);
            Assert.That(table.ColumnNames[0], Is.EqualTo("Name"));
            Assert.That(table.ColumnNames[1], Is.EqualTo("Age"));
            Assert.That(table.Label, Is.Null);
            Assert.That(table.Rows.Count, Is.Zero);
        }

        [Test]
        public void Table_gets_label_when_present()
        {
            Assert.Fail();
        }

        [Test]
        public void Table_reads_several_rows()
        {
            Assert.Fail();
        }

        //  In general, a table is not required at any particular point,
        //  so if it's not found, we get a null, not an abort.
        [Test]
        public void ParseTable_does_not_advance_parse_position_if_table_is_not_next()
        {
            Assert.Fail();
        }

        //  Since a table label doesn't look like anything else, finding one
        //  without a table will block us from parsing further.
        [Test]
        public void Parse_abort_on_label_with_no_header_row()
        {
            var tokens = new List<Token> {
                new Token(TokenType.TableLabel, "Olsen's Standard Book of British Birds"),
                new Token(TokenType.String, "The one without the Gannet"),
            };
            var state = new ParserState(tokens);
            var ex = Assert.Throws<Exception>(() => parser.ParseTable(state));
            Assert.That(ex.Message, Is.EqualTo("A table must have a row of column names"));
        }

        //TODO: mull whether this table abort (and similar) are good signs or not.

        //TODO: figure out desired behaviors when header and row lengths don't match.  Several cases.

    }
}
