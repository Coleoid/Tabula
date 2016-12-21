using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    public interface IGeneratedScenario
    {
        void ExecuteScenario();
        List<string> GetProblems();
        List<RunResult> GetResults();
        string ScenarioLabel { get; set; }
    }
}
