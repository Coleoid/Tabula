using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Tabula.API;

namespace Tabula
{
    public class Interpreter
    {
        public Type Workflow { get; set; }
        public Object Instance { get; set; }
        public bool skipSteps { get; set; }
        public int skipLine { get; set; }

        public Scope Scope { get; set; } = new Scope(parent: null);

        public void OpenScope()
        {
            var scope = new Scope(this.Scope);
            this.Scope = scope;
        }

        public void CloseScope()
        {
            Scope = Scope.ParentScope;
        }

        public NUnitReport.TestSuite ExecuteScenario(CST.Scenario scenario)
        {
            var scenarioResult = new NUnitReport.TestSuite();         
            CST.Paragraph previousParagraph = null;
            bool hasExecuted = false;

            NUnitReport.TestSuite paragraphResult;
            foreach (var section in scenario.Sections)
            {                
                if (section is CST.Paragraph)
                {                   

                    if (previousParagraph != null && !hasExecuted)
                    {
                        paragraphResult = ExecuteParagraph((previousParagraph));
                        FoldParagraphResultsIn(scenarioResult, paragraphResult);
                    }

                    previousParagraph = (CST.Paragraph)section;
                    hasExecuted = false;

                }
                else if (section is CST.Table && previousParagraph != null)
                {
                    CST.Table table = (CST.Table) section;
                   
                    foreach (var row in table.Rows)
                    {
                        OpenScope();

                        var columnHeaders = table.ColumnNames;
                        for (int i = 0; i < columnHeaders.Count; i++)
                        {
                            //TODO: that hardcoded 0 will need to mature to handle lists
                            Scope[columnHeaders[i]] = row.Cells[i][0];
                        }


                        paragraphResult = ExecuteParagraph((previousParagraph));
                        FoldParagraphResultsIn(scenarioResult, paragraphResult);

                        CloseScope();
                    }

                    hasExecuted = true;

                }
                else
                {
                    var testCase = new NUnitReport.TestCase();
                    testCase.FailureInfo = new NUnitReport.TestCaseFailure
                    {
                        Message = "Cannot have a table as the first section of a scenario.",
                        StackTrace = "Interpreter.ExecuteScenario"
                    };
                    scenarioResult.TestCases.Add(testCase);
                    scenarioResult.Result = NUnitTestResult.Failed;
                    scenarioResult.TestCaseCount += 1;
                    scenarioResult.FailedTests += 1;
                }


            }

            if (!hasExecuted && previousParagraph != null)
            {
                paragraphResult = ExecuteParagraph((previousParagraph));
                FoldParagraphResultsIn(scenarioResult, paragraphResult);
            }

            return scenarioResult;
        }

        private void FoldParagraphResultsIn(NUnitReport.TestSuite scenarioResult, NUnitReport.TestSuite paragraphResult)
        {
            scenarioResult.TestSuites.Add(paragraphResult);
            scenarioResult.TestCaseCount += paragraphResult.TestCaseCount;
            scenarioResult.PassedTests += paragraphResult.PassedTests;
            scenarioResult.FailedTests += paragraphResult.FailedTests;
            scenarioResult.InconclusiveTests += paragraphResult.InconclusiveTests;
            scenarioResult.SkippedTests += paragraphResult.SkippedTests;
        }

        public NUnitReport.TestSuite ExecuteParagraph(CST.Paragraph paragraph)
        {
            var paragraphResult = new NUnitReport.TestSuite();

            foreach (var action in paragraph.Actions)
            {
                if (action is CST.Step step)
                {
                    var stepResult = ExecuteStep(step);
                    paragraphResult.TestCases.Add(stepResult);
                    paragraphResult.TestCaseCount++;
                    if (stepResult.Result == NUnitTestResult.Passed) paragraphResult.PassedTests++;
                    if (stepResult.Result == NUnitTestResult.Failed) paragraphResult.FailedTests++;
                    if (stepResult.Result == NUnitTestResult.Skipped) paragraphResult.SkippedTests++;
                    if (stepResult.Result == NUnitTestResult.Inconclusive) paragraphResult.InconclusiveTests++;
                }
            }

            if (paragraphResult.FailedTests + paragraphResult.InconclusiveTests > 0)
            {
                paragraphResult.Result = NUnitTestResult.Failed;
            }
            else if (paragraphResult.SkippedTests == paragraphResult.TestCaseCount)
            {
                paragraphResult.Result = NUnitTestResult.Skipped;
            }
            else
            {
                paragraphResult.Result = NUnitTestResult.Passed;
            }

            return paragraphResult;
        }

        public NUnitReport.TestCase ExecuteStep(CST.Step step)
        {
            var result = new NUnitReport.TestCase
            {
                FailureInfo = new NUnitReport.TestCaseFailure(),
                Name = step.GetReadableText()
            };

            if (skipSteps)
            {
                result.FailureInfo.Message = $"Step skipped: Due to error on line {skipLine}.";
                result.Result = NUnitTestResult.Skipped;
                return result;
            }

            //TODO: hoist this up to ExecuteParagraph
            var instance = Activator.CreateInstance(Workflow);
            Instance = instance;
            LearnMethods(Workflow);

            var arguments = new List<Object>();
            string searchName = string.Empty;
            string stepText = string.Empty;
            int argCount = 0;

            foreach (var symbol in step.Symbols)
            {
                if (symbol.Type == TokenType.Word)
                    searchName += symbol.Text.ToLower();

                if (symbol.IsStepArgument) argCount++;

                if (symbol.Type == TokenType.String)
                {
                    stepText += $"\"{symbol.Text}\" ";
                }
                else
                {
                    stepText += symbol.Text + " ";
                }
            }

            MethodInfo methodInfo = FindMethod(searchName);
            if (methodInfo == null)
            {
                result.FailureInfo.Message = $"Couldn't find step '{stepText.Trim()}' on line {step.StartLine}";
                result.Result = NUnitTestResult.Inconclusive;
                return result;
            }

            var parameters = methodInfo.GetParameters();
            int paramIndex = 0;
            try
            {
                if (parameters.Length != argCount)
                    throw new Exception($"{argCount} arguments were provided to a {parameters.Length} parameter method.");

                foreach (var symbol in step.Symbols)
                {
                    if (symbol.IsStepArgument)
                    {
                        string variableValue = symbol.Text;

                        //if token is variable
                        if (symbol.Type == TokenType.Variable)
                        {
                            if (Scope.HasVariable(symbol.Text))
                            {
                                variableValue = Scope[symbol.Text];
                            }
                            else
                            {
                                throw new Exception($"Expected variable \"{symbol.Text}\" but it was not passed in. {Scope.NearHits(symbol.Text)}");
                            }
                        }

                        Type ParamType = GetParameterType(parameters[paramIndex]);
                        if (ParamType == typeof(String))
                        {
                            arguments.Add(variableValue);
                        }

                        //TODO:  Handle other numeric types
                        if (ParamType == typeof(Int32))
                        {
                            if (int.TryParse(variableValue, out int intValue))
                            {
                                arguments.Add(intValue);
                            }
                            else
                            {
                                throw new Exception($"argument \"{symbol.Text}\" ({symbol.Type}) does not match parameter '{parameters[paramIndex].Name}' ({ParamType.Name}).");
                            }
                        }

                        if (ParamType == typeof(DateTime))
                        {
                            // validate?
                            if (DateTime.TryParse(variableValue, out DateTime dateValue))
                            {
                                arguments.Add(dateValue);
                            }
                            else
                            {
                                throw new Exception($"argument \"{symbol.Text}\" ({symbol.Type}) does not match parameter '{parameters[paramIndex].Name}' ({ParamType.Name}).");
                            }
                        }

                        paramIndex++;
                    }
                }

                if (arguments.Count > parameters.Length)
                {
                    throw new Exception(
                        $"{arguments.Count} arguments were provided to a {parameters.Length} parameter method.");
                }

                using (var ic = new TestExecutionContext.IsolatedContext())
                {
                    methodInfo.Invoke(instance, arguments.ToArray());
                }
            }
            catch (Exception e)
            {
                Exception stepException = e.InnerException;
                if (e.InnerException == null)
                {
                    stepException = e;
                }

                var failureCatagory = string.Empty;
                if (stepException is AssertionException)
                {
                    failureCatagory = "failed";
                }
                else
                {
                    failureCatagory = "threw exception";
                    skipLine = step.StartLine;
                    skipSteps = true;
                }

                result.FailureInfo.Message = $"Step {failureCatagory}: {stepException.Message.TrimStart()}";
                result.Result = NUnitTestResult.Failed;
            }

            return result;
        }

        public void SetVariable(string name, string value)
        {
            Scope[name] = value;
        }

        private static void TypeCheck(ParameterInfo[] parameters, string actualType, int paramIndex, CST.Symbol symbol)
        {
            if (paramIndex + 1 > parameters.Length)
            {
                return;
            }
            var param = parameters[paramIndex];
            var type = param.ParameterType.Name;
            if (type != actualType)
                throw new Exception(
                    $"argument \"{symbol.Text}\" ({actualType}) does not match parameter '{param.Name}' ({type}).");
        }

        public static Type GetParameterType(ParameterInfo parameter)
        {
            Type param = parameter.ParameterType;
            return param;
        }

        public Dictionary<string, MethodInfo> searchableMethods { get; set; }
        public void LearnMethods(Type workflowType)
        {
            searchableMethods = new Dictionary<string, MethodInfo>();
            foreach (MethodInfo mi in workflowType.GetMethods())
            {
                string searchName = mi.Name.Replace("_", "").ToLower();

                searchableMethods[searchName] = mi;
            }
        }

        private MethodInfo FindMethod(string searchName)
        {
            if (searchableMethods.ContainsKey(searchName))
            {
                return searchableMethods[searchName];
            }
            else
            {
                return null;
            }
        }
    }
}
