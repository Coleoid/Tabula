using NUnit.Framework;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class Parser_BigBadWolfTests : TranspilerTestBase
    {
        [Test]
        public void Whos_afraid_to_parse_the_big_bad_wolf()
        {
            var tokens = _tokenizer.Tokenize(BigBadWolf);
            Assert.That(_tokenizer.Warnings, Has.Count.EqualTo(0),
                () => "Warnings: " + string.Join("\n", _tokenizer.Warnings)
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

            //TODO: current (31 Oct 17) failure point, no sections in scenario after ParseScenario.
            Assert.That(cst.Sections, Has.Count.EqualTo(29));

            //  ""Enable duty locations"":
            //  use: Global Setting Management
            //  Enable Duty Locations
            var para = cst.Sections[0] as CST.Paragraph;
            Assert.That(para, Is.Not.Null);

            Assert.That(cst.Sections[0].Label, Is.EqualTo("Enable duty locations"));
            Assert.That(cst.Sections[1].Label, Is.EqualTo("What we'll call our people in this scenario"));
            Assert.That(cst.Sections[2].Label, Is.Null);
            Assert.That(cst.Sections[3].Label, Is.EqualTo("What we'll call the organizations they work for"));
            Assert.That(cst.Sections[4].Label, Is.Null);
            Assert.That(cst.Sections[5].Label, Is.EqualTo("Create our people"));
            Assert.That(cst.Sections[6].Label, Is.Null);
            Assert.That(cst.Sections[7].Label, Is.EqualTo("Create our organizations"));
            Assert.That(cst.Sections[8].Label, Is.Null);
            Assert.That(cst.Sections[9].Label, Is.EqualTo("Create list items"));
            Assert.That(cst.Sections[10].Label, Is.Null);
            Assert.That(cst.Sections[11].Label, Is.EqualTo("Add employments"));
            Assert.That(cst.Sections[12].Label, Is.Null);
            Assert.That(cst.Sections[13].Label, Is.Null);  // alias
            Assert.That(cst.Sections[14].Label, Is.EqualTo("Add employment actions"));
            Assert.That(cst.Sections[15].Label, Is.Null);
            Assert.That(cst.Sections[16].Label, Is.Null);
            Assert.That(cst.Sections[17].Label, Is.Null);
            Assert.That(cst.Sections[18].Label, Is.Null);
            Assert.That(cst.Sections[19].Label, Is.Null);
            Assert.That(cst.Sections[20].Label, Is.EqualTo("Add a comment and duty assignment"));
            Assert.That(cst.Sections[21].Label, Is.Null);
            Assert.That(cst.Sections[22].Label, Is.Null);
            Assert.That(cst.Sections[23].Label, Is.EqualTo("Search people with many different duty assignment criteria"));
            Assert.That(cst.Sections[24].Label, Is.EqualTo("Search Assignment regular/temporary, location, and org"));
            Assert.That(cst.Sections[25].Label, Is.EqualTo("Assignment status, before, and after"));
            Assert.That(cst.Sections[26].Label, Is.EqualTo("Assignment date range, location and org"));
            Assert.That(cst.Sections[27].Label, Is.EqualTo("Assignment date range, location, and undated"));
            Assert.That(cst.Sections[28].Label, Is.EqualTo("Assignment date range, location/org, and employment type"));
        }

        //[Test]
        //public void 

    }
}
