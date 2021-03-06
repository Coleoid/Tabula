﻿using NUnit.Framework;
using System.Collections.Generic;
using Tabula.Parse;

namespace Tabula
{
    [TestFixture]
    public class ParserTests_BigBadWolf : TranspilerUnitTestBase
    {
        [Test]
        public void Whos_afraid_to_parse_the_big_bad_wolf()
        {
            var tokens = _tokenizer.Tokenize(BigBadWolf);
            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(0),
                "Warnings: " + string.Join("\n", _tokenizer.Warnings)
            );
            var state = new ParserState(tokens);

            var cst = _parser.ParseScenario(state);

            //  [Person Search, Duty Assignments, AC-16629]
            Assert.That(cst.Tags, Has.Count.EqualTo(3));
            Assert.That(cst.Tags[0], Is.EqualTo("Person Search"));
            Assert.That(cst.Tags[1], Is.EqualTo("Duty Assignments"));
            Assert.That(cst.Tags[2], Is.EqualTo("AC-16629"));

            //  Scenario: ""Advanced person search with duty assignments""
            Assert.That(cst.Label, Is.EqualTo("Advanced person search with duty assignments"));

            Assert.That(cst.Sections, Has.Count.EqualTo(29));
            Assert.That(cst.Sections[0].Label, Is.EqualTo("We need duty locations active to search on them"));
            Assert.That(cst.Sections[1].Label, Is.EqualTo("What we'll call our people in this scenario"));
            Assert.That(cst.Sections[2].Label, Is.EqualTo("table__011_to_015"));
            Assert.That(cst.Sections[3].Label, Is.EqualTo("What we'll call the organizations they work for"));
            Assert.That(cst.Sections[5].Label, Is.EqualTo("Create our people"));
            Assert.That(cst.Sections[7].Label, Is.EqualTo("Create our organizations"));
            Assert.That(cst.Sections[9].Label, Is.EqualTo("Create list items"));
            Assert.That(cst.Sections[11].Label, Is.EqualTo("Add employments"));
            Assert.That(cst.Sections[14].Label, Is.EqualTo("Add employment actions"));
            Assert.That(cst.Sections[20].Label, Is.EqualTo("Add a comment and duty assignment"));
            Assert.That(cst.Sections[22].Label, Is.EqualTo("Add temporary duty assignments"));
            Assert.That(cst.Sections[23].Label, Is.EqualTo("Search people with many different duty assignment criteria"));
            Assert.That(cst.Sections[24].Label, Is.EqualTo("Search Assignment regular/temporary, location, and org"));
            Assert.That(cst.Sections[25].Label, Is.EqualTo("Assignment status, before, and after"));
            Assert.That(cst.Sections[26].Label, Is.EqualTo("Assignment date range, location and org"));
            Assert.That(cst.Sections[27].Label, Is.EqualTo("Assignment date range, location, and undated"));
            Assert.That(cst.Sections[28].Label, Is.EqualTo("Assignment date range, location/org, and employment type"));
        }

        [Test]
        public void Undecorated_alias_to_a_block()
        {
            var tokens = _tokenizer.Tokenize(BigBadWolf);
            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(0),
                "Warnings: " + string.Join("\n", _tokenizer.Warnings)
            );
            var state = new ParserState(tokens);

            var cst = _parser.ParseScenario(state);

            //  alias: ""Add employment actions for #employeeName at #orgName"" => ...
            Assert.That(cst.Sections[13].Label, Is.EqualTo("paragraph__068_to_080"));
            var para = cst.Sections[13] as CST.Paragraph;
            Assert.That(para, Is.Not.Null);
            Assert.That(para.Actions, Has.Count.EqualTo(1));

            var alias = para.Actions[0] as CST.CommandAlias;
            Assert.That(alias, Is.Not.Null);
            var block = alias.Action as CST.Block;
            Assert.That(block, Is.Not.Null);

            Assert.That(block.Actions, Has.Count.EqualTo(10));
        }

    }
}
