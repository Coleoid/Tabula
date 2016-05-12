using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using TestopiaAPI;

namespace Tabula
{
    public class TabulaStepRunner
    {
        Stack<string> CallStack;
        List<RunResult> RunResults;
        bool SkipRemainder = false;
        private ILog _logger;
        private RunResult Result;

        public TabulaStepRunner(ILog logger)
        {
            CallStack = new Stack<string>();
            RunResults = new List<RunResult>();
            _logger = logger;
        }

        public void Do(Action action, string sourceLocation, string actionText)
        {
            CallStack.Push(sourceLocation);
            Result = new RunResult {
                StackTrace = new List<string>( CallStack.ToList() ),
                ActionText = actionText,
            };
            RunResults.Add(Result);

            var stopwatch = new Stopwatch();
            try
            {
                if (SkipRemainder)
                {
                    SetResultSkipped();
                    return;
                }

                try
                {
                    stopwatch.Start();
                    action();
                }
                finally
                {
                    stopwatch.Stop();
                }

                SetResult(stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                SetResult(stopwatch.Elapsed, ex);
            }
            finally
            {
                CallStack.Pop();
            }
        }

        public void Unfound(string sourceLocation, string actionText)
        {
            CallStack.Push(sourceLocation);

            Result = new RunResult {
                Outcome = ActionOutcome.Unfound,
                Message = "Did not find method to match action.",
                StackTrace = new List<string>( CallStack.ToList() ),
                TimeElapsed = TimeSpan.Zero,
                ActionText = actionText,
            };
            RunResults.Add(Result);

            CallStack.Pop();
        }

        private void SetResult(TimeSpan elapsed, Exception ex = null)
        {
            ActionOutcome outcome;
            string errorMessage = string.Empty;

            if (ex == null)
            {
                outcome = ActionOutcome.Passed;
            }
            else
            {
                errorMessage = ex.Message;
                if (ex is StepNotImplementedException)
                {
                    outcome = ActionOutcome.Waived;
                }
                else
                {
                    outcome = ActionOutcome.Failed;

                    //  Testing these via reflection to avoid dependencies
                    bool assertionFailure =
                        ex.GetType().Name.Contains("AssertionException") //NUnit
                     || ex.GetType().Name.Contains("AssertFailedException"); //MSTest

                    SkipRemainder = !assertionFailure;
                }
            }

            Result.Outcome = outcome;
            Result.Message = errorMessage;
            Result.TimeElapsed = elapsed;

            if (SkipRemainder)
            {
                _logger.ErrorFormat("Step [{0}] \nthrew exception at {1}:  {2}",
                    Result.ActionText, CallStack.Peek(), ex.Message);
            }
        }

        private void SetResultSkipped()
        {
            Result.Outcome = ActionOutcome.Waived;
            Result.Message = "Skipped due to earlier error";
            Result.TimeElapsed = TimeSpan.Zero;
        }
    }

    public enum ActionOutcome {
        Unknown = 0,
        Passed,
        Failed,
        Waived,
        Unfound,
    }

    public class RunResult
    {
        public ActionOutcome Outcome { get; set; }
        public string Message { get; set; }
        public List<string> StackTrace { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public string ActionText { get; set; }
    }
}
