using NUnit.Framework;
using System.Linq;
using System.Text;
using Tabula.Parse;

namespace Tabula
{
    [TestFixture]
    public class Generator_FullFileTests : TranspilerUnitTestBase
    {
        StringBuilder builder;
        Generator generator;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            builder = new StringBuilder();
            generator = new Generator();
        }

        [Test]
        public void generating_from_an_empty_scenario_creates_runnable_class()
        {
            generator.Generate(new CST.Scenario(), "path\\scenario_source.tab", builder);

            var fullClass = builder.ToString();
            Assert.That(fullClass, Contains.Substring("class scenario_source_generated"));
            Assert.That(fullClass, Contains.Substring("public void ExecuteScenario()"));
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
