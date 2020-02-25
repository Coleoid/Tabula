using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Tabula.API;
using System.IO;

namespace Tabula
{
    public class Interpreter
    {
       // public Type Workflow { get; set; }
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

        public Interpreter()
        {
            LoadWorkflowDLL();
        }

        List<Type> MyTypes { get; set; }
        public void LoadWorkflowDLL()
        {
            AppPath = @"D:\code\coleoid\Tabula\transpiler";
            var dllLocation = Path.Combine(AppPath, @"LibraryHoldingTestWorkflows\bin\Debug\LibraryHoldingTestWorkflows.dll");

            Assembly asm2 = Assembly.LoadFrom(dllLocation);

            AppDomain curDomain = AppDomain.CurrentDomain;
            curDomain.ReflectionOnlyAssemblyResolve += resolveAssembly;
            var asms = curDomain.GetAssemblies();

            MyTypes = new List<Type>();
            MyTypes.AddRange(asm2.ExportedTypes);
        }



        public NUnitReport.TestSuite ExecuteScenario(CST.Scenario scenario)
        {
            var scenarioResult = new NUnitReport.TestSuite();         
            CST.Paragraph previousParagraph = null;
            bool hasExecuted = false;

            scenarioResult.Name = scenario.Label;
            scenarioResult.ClassName = scenario.FileName;

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
                else if (section is CST.Table)
                {
                    if (previousParagraph == null)
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
                        return scenarioResult;
                    }

                    CST.Table table = (CST.Table) section;
                   
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        var row = table.Rows[i];
                        OpenScope();

                        var columnHeaders = table.ColumnNames;
                        for (int j = 0; j < columnHeaders.Count; j++)
                        {
                            //TODO: that hardcoded 0 will need to mature to handle lists
                            Scope[columnHeaders[j]] = row.Cells[j][0];
                        }

                        paragraphResult = ExecuteParagraph((previousParagraph));
                        if (table.Label != null)
                        {
                            paragraphResult.ClassName = $"Row {i} of table {table.Label}";
                        }
                        else
                        {
                            paragraphResult.ClassName = $"Row {i} of table at lines {table.StartLine}-{table.EndLine}";
                        }
                        FoldParagraphResultsIn(scenarioResult, paragraphResult);

                        CloseScope();
                    }

                    hasExecuted = true;
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
            scenarioResult.TotalTests += paragraphResult.TestCaseCount;
            scenarioResult.PassedTests += paragraphResult.PassedTests;
            scenarioResult.FailedTests += paragraphResult.FailedTests;
            scenarioResult.InconclusiveTests += paragraphResult.InconclusiveTests;
            scenarioResult.SkippedTests += paragraphResult.SkippedTests;

            if (paragraphResult.Result == NUnitTestResult.Failed)
            {
                scenarioResult.Result = NUnitTestResult.Failed;
            }
        }



        //TODO:  Get location(s) from config and/or command line args
        private Assembly resolveAssembly(object sender, ResolveEventArgs args)
        {
            string libName = args.Name.Substring(0, args.Name.IndexOf(","));

            string libPath = Path.Combine(AppPath, libName + ".dll");
            if (File.Exists(libPath))
                return Assembly.ReflectionOnlyLoadFrom(libPath);

            string exePath = AppPath + libName + ".exe";
            if (File.Exists(exePath))
                return Assembly.ReflectionOnlyLoadFrom(exePath);

            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        private string AppPath { get; set; }
        public void UseWorkflow(string name)
        {
            Type workflow = MyTypes.Find(t => t.Name == name);

            var instance = Activator.CreateInstance(workflow);
            Instance = instance;
            LearnMethods(workflow);
        }

        public NUnitReport.TestSuite ExecuteParagraph(CST.Paragraph paragraph)
        {
            var paragraphResult = new NUnitReport.TestSuite();

            paragraphResult.Name = paragraph.Label;

            foreach (var action in paragraph.Actions)
            {
                if (action is CST.CommandSet cmdSet)
                {
                    Scope[cmdSet.Name] = cmdSet.Term.Text;
                }
                if (action is CST.CommandUse cmd)
                {
                    foreach (var workflowName in cmd.Workflows)
                    {
                        try
                        {
                            UseWorkflow(workflowName);
                        }
                        catch (Exception ex)
                        {
                            var result = new NUnitReport.TestCase
                            {
                                FailureInfo = new NUnitReport.TestCaseFailure(),
                                Name = "use: " + workflowName  //TODO: Test this is what we want
                            };

                            Exception stepException = ex.InnerException;
                            if (ex.InnerException == null)
                            {
                                stepException = ex;
                            }

                            var failureCatagory = string.Empty;
                            if (stepException is AssertionException)
                            {
                                failureCatagory = "failed";
                            }
                            else
                            {
                                failureCatagory = "threw exception";
                                skipLine = cmd.StartLine;
                                skipSteps = true;
                            }

                            result.FailureInfo.Message = $"Tried to use workflow \"{workflowName}\" on line {cmd.StartLine}, and did not find it.";
                            result.Result = NUnitTestResult.Failed;
                            FoldLineResultsIn(paragraphResult, result);
                        }
                    }
                }
                else if (action is CST.Step step)
                {
                    var stepResult = ExecuteStep(step);
                    FoldLineResultsIn(paragraphResult, stepResult);
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

        public void FoldLineResultsIn(NUnitReport.TestSuite paragraphResult, NUnitReport.TestCase lineResult )
        {
            paragraphResult.TestCases.Add(lineResult);
            paragraphResult.TestCaseCount++;
            if (lineResult.Result == NUnitTestResult.Passed) paragraphResult.PassedTests++;
            if (lineResult.Result == NUnitTestResult.Failed) paragraphResult.FailedTests++;
            if (lineResult.Result == NUnitTestResult.Skipped) paragraphResult.SkippedTests++;
            if (lineResult.Result == NUnitTestResult.Inconclusive) paragraphResult.InconclusiveTests++;
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
                    methodInfo.Invoke(Instance, arguments.ToArray());
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

        public MethodInfo FindMethod(string searchName)
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
