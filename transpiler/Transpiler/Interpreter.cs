using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Internal;
using System.Reflection.Emit;
using NUnit.Framework;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using Tabula.CST;

namespace Tabula
{
    public class Interpreter
    {
        public Type Workflow { get; set; }
        public Object Instance { get; set; }
        public bool skipSteps { get; set; }
        public int skipLine { get; set; }


        public NUnitReport.TestSuite ExecuteScenario(CST.Scenario scenario)
        {
            var scenarioResult = new NUnitReport.TestSuite();

            // complete cheese
            var paragraph = scenario.Sections[0] as Paragraph;
            var table = scenario.Sections[1] as Table;

            NUnitReport.TestSuite paragraphResult;
            foreach (var row in table.Rows)
            {

                paragraphResult = ExecuteParagraph((paragraph));
                scenarioResult.TestSuites.Add(paragraphResult);
                scenarioResult.TestCaseCount += paragraphResult.TestCaseCount;
                scenarioResult.PassedTests += paragraphResult.PassedTests;
                scenarioResult.FailedTests += paragraphResult.FailedTests;
                scenarioResult.InconclusiveTests += paragraphResult.InconclusiveTests;
                scenarioResult.SkippedTests += paragraphResult.SkippedTests;
            }

            return scenarioResult;
        }

        public NUnitReport.TestSuite ExecuteParagraph(CST.Paragraph paragraph)
        {
            var paragraphResult = new NUnitReport.TestSuite();

            foreach (var action in paragraph.Actions)
            {
                if (action is CST.Step step)
                {
                    NUnitReport.TestCase stepResult = ExecuteStep(step);
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
                    if (symbol.IsStepArgument)
                    {
                        string variableValue = "";  //????  get value from symbol table
                        Type ParamType = GetParameterType(parameters[paramIndex]);
                        if (ParamType == typeof(String))
                        {
                            arguments.Add(variableValue);
                        }
                        if (ParamType == typeof(Int32))
                        {
                            int intValue;
                            if (int.TryParse(variableValue, out intValue))
                            {
                                arguments.Add(intValue);
                            }
                            else
                            {
                                throw new Exception("I died.");
                            }
                        }
                        if (ParamType == typeof(DateTime))
                        {
                            // validate?
                            arguments.Add(variableValue);
                        }
                    }
                    //    switch (symbol.Type)
                    //    {
                    //    case TokenType.String:
                    //        TypeCheck(parameters, "String", paramIndex++, symbol);
                    //        arguments.Add(symbol.Text);
                    //        break;

                    //    case TokenType.Number:
                    //        TypeCheck(parameters, "Int32", paramIndex++, symbol);
                    //        arguments.Add(int.Parse(symbol.Text));
                    //        break;

                    //    case TokenType.Date:
                    //        TypeCheck(parameters, "DateTime", paramIndex++, symbol);
                    //        arguments.Add(DateTime.Parse(symbol.Text));
                    //        break;

                    //    case TokenType.Variable:
                    //        Type ParamType = GetParameterType(parameters[paramIndex]);

                    //        string variableValue = "";  //????  get value from symbol table

                    //        if (ParamType == typeof(String))
                    //        {
                    //            arguments.Add(variableValue);
                    //        }
                    //        if (ParamType == typeof(Int32))
                    //        {

                    //            // can we convert our variableValue to an int?
                    //            arguments.Add(variableValue);
                    //        }
                    //        if (ParamType == typeof(DateTime))
                    //        {
                    //            // validate?
                    //            arguments.Add(variableValue);
                    //        }

                    //        ///!!!
                    //        break;

                    //    default:
                    //        break;
                    //    }
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
