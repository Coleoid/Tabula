using System.Collections.Generic;
using System.Text;
using Tabula.Parse;
using Tabula.CST;

namespace Tabula
{
    public class Transpiler
    {
        public void Transpile(string fileName, string scenarioText, StringBuilder builder)
        {
            var tokenizer = new Tokenizer();
            TokenizerOutput tokenStream = tokenizer.Tokenize(scenarioText);
            var state = new ParserState(tokenStream);
            var parser = new Parser();
            CST.Scenario scenario = parser.ParseScenario(state);
            var generator = new Generator();
            generator.Generate(scenario, fileName, builder);
        }
    }
}
