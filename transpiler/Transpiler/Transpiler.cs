using System.CodeDom.Compiler;
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

    public class Engine
    {
        public void Execute(string fileName, string scenarioText)
        {
            var tokenizer = new Tokenizer();
            TokenizerOutput tokenStream = tokenizer.Tokenize(scenarioText);
            var state = new ParserState(tokenStream);
            var parser = new Parser();
            CST.Scenario scenario = parser.ParseScenario(state);
            var interpreter = new Interpreter();
            var results = interpreter.ExecuteScenario(scenario, fileName);
        }
    }
}
