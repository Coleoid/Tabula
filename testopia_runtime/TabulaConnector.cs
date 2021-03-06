using System.Linq;
using Testopia.Runner;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tabula
{
    //  Called by the Testopia framework, Program.cs:Main().
    public class TabulaConnector : TestopiaConnector
    {
        public override void StartTestRun()
        {
            _suite = new Testopia.Suite();
            SuiteResults = new List<RunResult>();

            // SPIKE-CHEAT: Generated scenario added to the Testopia project,
            //  newed up and fired off right here.
            // NOTE:  FlowScenario_generated.cs was generated by the
            //  Tabula compiler by reading the files
            //  FlowScenario.tab and SampleWorkflows.cs
            IGeneratedScenario scenario = new FlowScenario_generated();
            scenario.ExecuteScenario();

            ReportProblems(scenario);
            ReportResults(scenario);
        }

        private void ReportProblems(IGeneratedScenario scenario)
        {
            var problems = scenario.GetProblems();
            if (problems.Any())
            {
                Console.Out.WriteLine("Scenario [{0}] problems:", scenario.ScenarioLabel);
                foreach (var problem in problems)
                {
                    Console.Out.WriteLine(problem);
                }
                Console.Out.WriteLine("");
            }
        }

        private void ReportResults(IGeneratedScenario scenario)
        {
            var results = scenario.GetResults();
            Console.Out.WriteLine("Scenario [{0}] results:", scenario.ScenarioLabel);
            foreach (var result in results)
            {
                Console.Out.WriteLine(result);
            }
        }


        //  **********************************************************************************
        //  *** Code below is closer to production-complexity, but out o' scope for the spike.

        public List<IGeneratedScenario> Scenarios { get; set; }
        private List<RunResult> SuiteResults;

        public void eventually_StartTestRun()
        {
            var problems = LoadSuite();
            if (problems.Any())
            {
                //TODO: Nonmoronic error reporting
                throw new Exception("Moof!");
            }

            _logger.Debug("Running Suite");
            problems = RunSuite();
            if (problems.Any())
            {
                //TODO: Nonmoronic error reporting
                throw new Exception("Crob!");
            }

            _logger.Debug("Suite run finished.");
        }

        public List<string> LoadSuite()
        {
            var _assembly = Assembly.LoadFile(_arguments.AssemblyPath);

            var scenarios = new List<string>();
            if (!string.IsNullOrEmpty(_arguments.Scenario))
                scenarios.Add(_arguments.Scenario);

            //TODO: loading scenarios from a folder/subtree

            Scenarios = new List<IGeneratedScenario>();
            var problems = new List<string>();
            foreach (var scenarioName in scenarios)
            {
                bool foundScenarioClass = _assembly.ExportedTypes.Any(t => t.Name.Equals(scenarioName, StringComparison.CurrentCultureIgnoreCase));
                if (foundScenarioClass)
                {
                    var scenario = (IGeneratedScenario)_assembly.CreateInstance(scenarioName, true);
                    Scenarios.Add(scenario);
                }
                else
                {
                    var problem = String.Format("Can't find scenario [{0}] in assembly [{1}].",
                        scenarioName, _assembly.FullName);
                    problems.Add(problem);
                }
            }

            return problems;
        }

        public List<string> RunSuite()
        {
            var suiteProblems = new List<string>();
            foreach (var scenario in Scenarios)
            {
                scenario.ExecuteScenario();
                var results = scenario.GetResults();
                SuiteResults.AddRange(results);
                var problems = scenario.GetProblems();
                suiteProblems.AddRange(problems);
            }
            return suiteProblems;
        }

    }

}
