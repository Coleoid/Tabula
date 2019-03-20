using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Tabula.API
{
    public class GeneratedScenarioBase
    {
        public string ScenarioLabel { get; set; }
        public string FileName { get; set; }

        public RuntimeContext Context { get; set; }

        protected List<NUnitReport.TestCase> RunResults;
        public NUnitReport.TestSuite GetResults()
        {
            return BuildReport(RunResults);
        }

        public NUnitReport.TestSuite BuildReport(List<NUnitReport.TestCase> cases)
        {
            var suite = new NUnitReport.TestSuite();

            suite.TestCases = cases;

            suite.TestCaseCount = cases.Count;
            suite.TotalTests = cases.Count;

            suite.ClassName = FileName;

            int failedTests = 0;
            int inconclusiveTests = 0;
            int passedTests = 0;
            int skippedTests = 0;
            
            foreach (var testCase in cases)
            {
                switch (testCase.Result)
                {
                case NUnitTestResult.Failed:
                    failedTests++;
                    break;
                case NUnitTestResult.Inconclusive:
                    inconclusiveTests++;
                    break;
                case NUnitTestResult.Passed:
                    passedTests++;
                    break;
                case NUnitTestResult.Skipped:
                    skippedTests++;
                    break;
                default:
                    break;
                }

            }

            suite.FailedTests = failedTests;
            suite.InconclusiveTests = inconclusiveTests;
            suite.PassedTests = passedTests;
            suite.SkippedTests = skippedTests;

            suite.FullName = ScenarioLabel;

            suite.Type = NUnitTestSuiteType.TestFixture;

            suite.Result = GetSuiteResult(suite);

            return suite;
        }

        private NUnitTestResult GetSuiteResult(NUnitReport.TestSuite suite)
        {
            if (suite.FailedTests > 0)
            {
                return NUnitTestResult.Failed;
            } else
            {
                return NUnitTestResult.Passed;
            }
        }

        protected List<string> Problems;
        public List<string> GetProblems() => Problems;

        protected Scope Var { get => Context.CurrentScope; }
        private Regex varReference;

        private bool SkippingRemainder = false;
        public string SkipMessage;

        public GeneratedScenarioBase()
        {
            Context = new RuntimeContext();
            RunResults = new List<NUnitReport.TestCase>();
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
            var testCase = new NUnitReport.TestCase
            {
                Result = NUnitTestResult.Failed,
                Name = actionText,
                FailureInfo = new NUnitReport.TestCaseFailure
                {
                    StackTrace = sourceLocation,
                    Message = "Did not find method to match step.",
                }
            };

            RunResults.Add(testCase);
        }


        public void Do(Action action, string sourceLocation, string actionText, [CallerMemberName] string callerName = "")
        {
            MethodBase callingMethod = new StackFrame(1).GetMethod();
            var result = new NUnitReport.TestCase
            {
                Name = actionText,
                Result = NUnitTestResult.Passed,
            };
            RunResults.Add(result);

            if (SkippingRemainder)
            {
                result.Result = NUnitTestResult.Skipped;
                result.FailureInfo = new NUnitReport.TestCaseFailure
                {
                    Message = SkipMessage,
                    StackTrace = sourceLocation
                };
                return;
            }

            string lineNumber = sourceLocation.Split(':')[1];
            PerformAction(action, result, lineNumber);  //  <--<<  The Action
            if (result.FailureInfo != null)
            {
                result.FailureInfo.StackTrace = sourceLocation;
            }
        }

        private void PerformAction(Action action, NUnitReport.TestCase result, string lineNumber)
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
                SetResult(result, exception);
                DetermineIfSkippingRemainder(exception, lineNumber);
            }
        }

        public void SetResult(NUnitReport.TestCase result, Exception ex = null)
        {
            if (ex == null)
            {
                result.Result = NUnitTestResult.Passed;
            }
            else if (ex is NotImplementedException)
            {
                result.Result = NUnitTestResult.Inconclusive;
                result.FailureInfo = new NUnitReport.TestCaseFailure
                {
                    StackTrace = string.Join(Environment.NewLine, Context.BuildCallStack()),
                    Message = "Step is not implemented"
                };
            }
            else
            {
                result.Result = NUnitTestResult.Failed;
                result.FailureInfo = new NUnitReport.TestCaseFailure
                {
                    Message = ex.Message
                };
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
