using NUnit.Framework;
using System.Linq;
using System.Text;

namespace Tabula
{
    [TestFixture]
    public class Generator_FullFileTests : TranspilerUnitTestBase
    {
        StringBuilder builder;
        Generator generator;

        [SetUp]
        public void SetUp()
        {
            builder = new StringBuilder();
            generator = new Generator();
        }

        [Test]
        public void generating_from_an_empty_scenario_is_not_insane()
        {
            generator.Generate(new CST.Scenario(), "path\\scenario_source.tab", builder);

            var fullClass = builder.ToString();
            Assert.That(fullClass, Contains.Substring("class scenario_source_generated"));
        }

        [Test]
        public void Big_Bad_Wolf_generates()
        {
            var tokens = _tokenizer.Tokenize(BigBadWolf);
            var state = new ParserState(tokens);
            var scenario = _parser.ParseScenario(state);

            generator.Library.DetailLoadedTypes();
            var types = generator.Library.CachedWorkflows.Keys;
            var globals = types.Where(t => t.Name.Contains("Global")).ToList();
            var settings = types.Where(t => t.Name.Contains("Setting")).ToList();
            generator.Generate(scenario, "path\\person_search_duty_locations.tab", builder);

            var fullClass = builder.ToString();
            Assert.That(fullClass, Contains.Substring("class person_search_duty_locations_generated"));
        }
    }
}
