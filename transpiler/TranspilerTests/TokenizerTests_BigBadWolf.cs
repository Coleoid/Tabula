﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tabula.Tests
{
    [TestFixture]
    public class TokenizerTests_BigBadWolf : TranspilerUnitTestBase
    {
        [Test]
        public void Whos_afraid_to_tokenize_the_big_bad_wolf()
        {
            var tokens = _tokenizer.Tokenize(BigBadWolf);

            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(0),
                "Warnings: " + string.Join("\n", _tokenizer.Warnings)
            );
        }

        [Test]
        public void Scenario_header()
        {
            var tokens = _tokenizer.Tokenize(BigBadWolf);

            Assert_TokenSequenceMatches(tokens, 0,
                TokenType.NewLine,
                TokenType.Tag,
                TokenType.NewLine,
                TokenType.ScenarioLabel
            );

            Assert.That(tokens.Tokens[3].Text, Does.StartWith("Advanced person"));
        }

        [Test]
        public void para_1()
        {
            var output = _tokenizer.Tokenize(BigBadWolf);
            var tokens = output.Tokens;

            Assert_TokenSequenceMatches(output, 6,
                TokenType.SectionLabel,
                TokenType.NewLine,
                TokenType.cmd_Use,
                TokenType.NewLine,
                TokenType.Word,
                TokenType.Word,
                TokenType.Word,
                TokenType.NewLine
            );

            Assert.That(tokens[6].Text, Does.StartWith("We need duty locations active to search on them"));
            Assert.That(tokens[8].Text, Does.Match("Global Setting Management"));
            Assert.That(tokens[10].Text, Does.Match("Enable"));
            Assert.That(tokens[11].Text, Does.Match("Duty"));
            Assert.That(tokens[12].Text, Does.Match("Locations"));
        }

        [Test]
        public void para_2()
        {
            var output = _tokenizer.Tokenize(BigBadWolf);
            var tokens = output.Tokens;

            Assert_TokenSequenceMatches(output, 15,
                TokenType.SectionLabel,
                TokenType.NewLine,
                TokenType.cmd_Set,
                TokenType.Variable,
                TokenType.NewLine
            );

            Assert.That(tokens[15].Text, Does.Match("What we'll call our people in this scenario"));
            Assert.That(tokens[17].Text, Does.Match("#handle"));
            Assert.That(tokens[18].Text, Does.Match("fullnamelf"));
        }

        [Test]
        public void table_1()
        {
            var tokens = _tokenizer.Tokenize(BigBadWolf);

            Assert_TokenSequenceMatches(tokens, 20,
                TokenType.TableCellSeparator,
                TokenType.Word,
                TokenType.TableCellSeparator,
                TokenType.Word,
                TokenType.TableCellSeparator,
                TokenType.NewLine,
                TokenType.TableCellSeparator,
                TokenType.Word,
                TokenType.TableCellSeparator,
                TokenType.String,
                TokenType.TableCellSeparator,
                TokenType.NewLine
            );
        }
    }
}
