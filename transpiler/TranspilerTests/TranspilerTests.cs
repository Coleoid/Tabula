using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace Tabula
{
    [TestFixture]
    public class TranspilerTests : TranspilerUnitTestBase
    {
        [Test]
        public void Transpiler_generates_class_in_supplied_StringBuilder()
        {
            var builder = new StringBuilder();
            var transpiler = new Transpiler();
            var scenarioText = @"
Scenario: really rather silly
";

            transpiler.Transpile("silly file_name.txt", scenarioText, builder);

            var output = builder.ToString();
            Assert.That(output, Contains.Substring("public class silly_file_name_generated"));
        }
    }
}
