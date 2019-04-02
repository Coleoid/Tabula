using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Tabula
{
    public class Interpreter
    {
        public Type Workflow { get; set; }
        public Object Instance { get; set; }


        public List<string> ExecuteScenario(CST.Scenario scenario, string fileName)
        {
            // type to change
            List<string> results = new List<string>();

            //???

            return results;
        }

        //  skipping middle steps

        public string ExecuteStep(CST.Step step)
        {
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
                    searchName += symbol.Text;
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

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, parameters.ToArray());
            }

            return $"Couldn't find step '{stepText.Trim()}' on line {step.StartLine}";
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
