using System.Collections.Generic;
using System.Text;

namespace Tabula
{
    public class Transpiler
    {
        public void Transpile(string fileName, string scenarioText, StringBuilder builder)
        {
            var tokenizer = new Tokenizer();
            List<Token> tokenStream = tokenizer.Tokenize(scenarioText);
            var state = new ParserState(tokenStream);
            var parser = new Parser();
            CST.Scenario scenario = parser.ParseScenario(state);
            var generator = new Generator();
            generator.Generate(scenario, fileName, builder);
        }
    }
}
