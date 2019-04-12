using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Internal;
using System.Reflection.Emit;
using NUnit.Framework;

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

        //  skipping middle steps




        public (string, NUnitTestResult) ExecuteStep(CST.Step step)
        {
            if (skipSteps)
            {
                return ($"Step skipped: Due to error on line {skipLine}.", NUnitTestResult.Skipped);
            }

            var instance = Activator.CreateInstance(Workflow);
            Instance = instance;

            LearnMethods(Workflow);
            
            var parameters = new List<Object>();
            string searchName = string.Empty;
            string stepText = string.Empty;

            foreach (var symbol in step.Symbols)
            {
                switch (symbol.Type)
                {
                case TokenType.String:
                    parameters.Add(symbol.Text);
                    break;

                case TokenType.Number:
                    parameters.Add(int.Parse(symbol.Text));
                    break;

                case TokenType.Date:
                    parameters.Add(DateTime.Parse(symbol.Text));
                    break;

                case TokenType.Word:
                    searchName += symbol.Text.ToLower();
                    break;

                default:
                    break;
                }

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
                return ($"Couldn't find step '{stepText.Trim()}' on line {step.StartLine}", NUnitTestResult.Inconclusive);
            }

            try
            {
                using (var ic = new TestExecutionContext.IsolatedContext())
                {
                    methodInfo.Invoke(instance, parameters.ToArray());
                }
            }
            catch (Exception e)
            {


                Exception stepException = e.InnerException;

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

                return ($"Step {failureCatagory}: {stepException.Message.TrimStart()}", NUnitTestResult.Failed);

            }
            return ("Success", NUnitTestResult.Passed);
        }


        public Dictionary<string,MethodInfo> searchableMethods { get; set; }
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
