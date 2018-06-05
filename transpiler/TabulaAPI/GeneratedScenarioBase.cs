using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TestopiaAPI;

namespace Tabula.API
{
    public class GeneratedScenarioBase
    {
        public string ScenarioLabel { get; set; }
        public string FileName { get; set; }

        public RuntimeContext Context { get; set; }
        protected List<RunResult> RunResults;
        protected List<string> Problems;
        public List<RunResult> GetResults()
        {
            return RunResults;
        }
        public List<string> GetProblems()
        {
            return Problems;
        }

        protected Scope Var { get => Context.CurrentScope; }
        private Regex varReference;

        private bool SkippingRemainder = false;
        public string SkipMessage;

        public GeneratedScenarioBase()
        {
            Context = new RuntimeContext();
            RunResults = new List<RunResult>();
            Problems = new List<string>();

            varReference = new Regex(@"(Var\[""([^]]+)""\])");
        }

        public void Start_Paragraph(string labelText)  //  speculative
        {
            Context.OpenScope($"Paragraph {labelText}");
            Label(labelText);
        }
        public void Label(string labelText)
        {
            Context.ParagraphName = labelText;
        }
        public void End_Paragraph()  //  speculative
        {
            Context.CloseScope();
        }

        public void Foreach_Row_in(Func<Table> tableGenerator, Action paragraph)
        {
            Table table = tableGenerator();
            int rowNum = 0;
            int rowCount = table.Data.Count;
            foreach (var row in table.Data)
            {
                Context.OpenScope($"Row {++rowNum} of {rowCount} in table {table.Name}");
                Context.AddRowValues(table.Header, row);
                paragraph();  //  <--<<  The Actions
                Context.CloseScope();
            }
        }

        public void Unfound(string actionText, string sourceLocation)
        {
            var result = new RunResult
            {
                SourceLocation = sourceLocation,
                StackTrace = Context.BuildCallStack(),
                ActionText = actionText,
                Outcome = ActionOutcome.Unfound,
                Message = "Did not find method to match action.",
                TimeElapsed = TimeSpan.Zero,
            };
            RunResults.Add(result);
        }

        public void Do(Action action, string sourceLocation, string actionText, [CallerMemberName] string callerName = "")
        {
            MethodBase callingMethod = new StackFrame(1).GetMethod();
            var result = new RunResult
            {
                SourceLocation = sourceLocation,
                StackTrace = Context.BuildCallStack(),
                ActionText = varReference.Replace(actionText, (m) => $"\"{Context[m.Groups[2].Value]}\""),
            };
            RunResults.Add(result);

            if (SkippingRemainder)
            {
                result.Skip(SkipMessage);
                return;
            }

            PerformAction(action, result);  //  <--<<  The Action
        }

        private void PerformAction(Action action, RunResult result)
        {
            Exception exception = null;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                action();  //  <--<<  The Action
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                stopwatch.Stop();
                result.SetResult(stopwatch.Elapsed, exception);
                DetermineIfSkippingRemainder(exception, result.LineNumber);
            }
        }

        private void DetermineIfSkippingRemainder(Exception ex, string lineNumber)
        {
            if (ex == null || ex is NotImplementedException) return;

            //  Checking these exception types via text, to avoid .dll dependencies
            if (Regex.IsMatch(ex.GetType().Name, "Assert(ion|Failed)Exception")) return;

            SkippingRemainder = true;
            SkipMessage = "Skipped due to error on line " + lineNumber;
        }
    }
}
