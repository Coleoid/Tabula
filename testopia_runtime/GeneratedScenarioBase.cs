using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tabula
{
    public class GeneratedScenarioBase
    {
        public string ScenarioLabel { get; set; }
        public ScenarioContext Context { get; set; }
        public ValueContext val { get; set; }
        private Stack<String> CallStack;

        protected List<string> Problems;
        private bool SkipRemainder = false;

        protected List<RunResult> RunResults;
        private RunResult _result;


        public GeneratedScenarioBase()
        {
            RunResults = new List<RunResult>();
            CallStack = new Stack<string>();
            Problems = new List<string>();
        }

        public void Do(Action action, string sourceLocation, string actionText)
        {
            _result = new RunResult {
                SourceLocation = sourceLocation,
                StackTrace = new List<string>(CallStack.ToList()),
                ActionText = actionText,
            };
            RunResults.Add(_result);

            if (SkipRemainder)
                _result.Skip(SkipMessage);
            else
                TimeAction(action);
        }

        public void Unfound(string actionText, string sourceLocation)
        {
            _result = new RunResult {
                SourceLocation = sourceLocation,
                Outcome = ActionOutcome.Unfound,
                Message = "Did not find method to match action.",
                StackTrace = new List<string>(CallStack.ToList()),
                TimeElapsed = TimeSpan.Zero,
                ActionText = actionText,
            };
            RunResults.Add(_result);
        }

        private void TimeAction(Action action)
        {
            Exception exception = null;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                stopwatch.Stop();
                _result.SetResult(stopwatch.Elapsed, exception);
                SkipRemainder = shouldSkipRemainder(exception);
                if (SkipRemainder)
                    SkipMessage = "Skipped due to error on line " + _result.LineNumber;
            }
        }
        public string SkipMessage;

        private bool shouldSkipRemainder(Exception ex)
        {
            bool should = false;

            if (ex != null && !(ex is NotImplementedException))
            {
                //  Testing these via reflection to avoid dependencies
                bool assertionFailure =
                    ex.GetType().Name.Contains("AssertionException") //NUnit
                    || ex.GetType().Name.Contains("AssertFailedException"); //MSTest

                should = !assertionFailure;
            }

            return should;
        }

        public void TableOverParagraph(Action paragraph, Func<Table> tableGenerator)
        {
            Table table = tableGenerator();
            foreach (var row in table.Rows)
            {
                Context.Push(row);
                paragraph();
                Context.Pop();
            }
        }

        public List<string> GetProblems()
        {
            return Problems;
        }

        public List<RunResult> GetResults()
        {
            return RunResults;
        }
    }

}
