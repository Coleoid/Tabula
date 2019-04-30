using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Internal;
using System.Reflection.Emit;
using NUnit.Framework;
using System.Globalization;
using System.Runtime.Remoting.Messaging;

namespace Tabula
{
    public class Interpreter
    {
        public Type Workflow { get; set; }
        public Object Instance { get; set; }
        public bool skipSteps { get; set; }
        public int skipLine { get; set; }


        public List<string> ExecuteScenario(CST.Scenario scenario, string fileName)
        {
            // type to change
            List<string> results = new List<string>();

            //???

            return results;
        }

        public NUnitReport.TestSuite ExecuteParagraph(CST.Paragraph paragraph)
        {
            var result = new NUnitReport.TestSuite();

            foreach (var action in paragraph.Actions)
            {
                if (action is CST.Step step)
                {
                    var stepResult = ExecuteStep(step);
                    result.TestCases.Add(stepResult);
                    result.TestCaseCount++;
                    if (stepResult.Result == NUnitTestResult.Passed) result.PassedTests++;
                    if (stepResult.Result == NUnitTestResult.Failed) result.FailedTests++;
                    if (stepResult.Result == NUnitTestResult.Skipped) result.SkippedTests++;
                    if (stepResult.Result == NUnitTestResult.Inconclusive) result.InconclusiveTests++;
                }
            }

            if (result.FailedTests + result.InconclusiveTests > 0)
            {
                result.Result = NUnitTestResult.Failed;
            }
            else if (result.SkippedTests == result.TestCaseCount)
            {
                result.Result = NUnitTestResult.Skipped;
            }
            else
            {
                result.Result = NUnitTestResult.Passed;
            }

            return result;
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

            //TODO: hoist this out to ExecuteParagraph
            var instance = Activator.CreateInstance(Workflow);
            Instance = instance;

            LearnMethods(Workflow);

            var arguments = new List<Object>();
            string searchName = string.Empty;
            string stepText = string.Empty;

            foreach (var symbol in step.Symbols)
            {
                if (symbol.Type == TokenType.Word)
                    searchName += symbol.Text.ToLower();

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
                foreach (var symbol in step.Symbols)
                {
                    switch (symbol.Type)
                    {
                    case TokenType.String:
                        TypeCheck(parameters, "String", paramIndex++, symbol);
                        arguments.Add(symbol.Text);
                        break;

                    case TokenType.Number:
                        TypeCheck(parameters, "Int32", paramIndex++, symbol);
                        arguments.Add(int.Parse(symbol.Text));
                        break;

                    case TokenType.Date:
                        TypeCheck(parameters, "DateTime", paramIndex++, symbol);
                        arguments.Add(DateTime.Parse(symbol.Text));
                        break;

                    default:
                        break;
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
