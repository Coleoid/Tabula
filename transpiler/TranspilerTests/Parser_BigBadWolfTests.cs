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


        }

    }
}
