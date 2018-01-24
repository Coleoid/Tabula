﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tabula
{
    //  It is a truism that generated code is ugly.
    //  In Tabula, the generated code is a first-class result.  Effort is spent to keep it readable.

    //  Build{Xxx} methods primarily fill StringBuilders, which will finally be assembled into
    //  the StringBuilder sent to us by Visual Studio, which is waiting to receive our new
    //  C# code.

    //  IndentingStringBuilders factor out indenting each new line.

    public class Generator
    {
        private string GeneratorVersion = "0.2";

        public string GeneratedClassName { get; set; }

        public CST.Scenario Scenario { get; set; }

        public StringBuilder Builder { get; set; }
        public IndentingStringBuilder executeMethodBody { get; set; }
        public IndentingStringBuilder sectionsBody { get; set; }

        public string InputFilePath { get; set; }

        public Dictionary<string, WorkflowDetail> Workflows { get; private set; }
        public List<WorkflowDetail> WorkflowsInScope { get; set; }
        public WorkflowIntrospector Library { get; private set; }


        public Generator()
        {
            Workflows = new Dictionary<string, WorkflowDetail>();
            WorkflowsInScope = new List<WorkflowDetail>();
            Library = new WorkflowIntrospector();
            executeMethodBody = new IndentingStringBuilder(12);  // 12 = namespace + class + inside method
            sectionsBody = new IndentingStringBuilder(8);  // 8 = namespace + class
        }

        public void Generate(CST.Scenario scenario, string inputFilePath, StringBuilder builder)
        {
            Scenario = scenario;
            Builder = builder;
            InputFilePath = inputFilePath;

            BuildHeader();
            BuildNamespaceOpen();
            BuildClassOpen();
            BuildDeclarations();
            BuildConstructor();
            BuildClassBody();
            BuildClassClose();
            BuildNamespaceClose();
        }

        public void BuildHeader()
        {
            Builder.AppendLine($"//  This file was generated by TabulaClassGenerator version {GeneratorVersion}.");
            Builder.AppendLine($"//  To change this file, change the Tabula scenario at {InputFilePath}.");
            Builder.AppendLine($"//  Version {GeneratorVersion} only generates this rudimentary paste.  You have been warned.");
            Builder.AppendLine("using System;");
            Builder.AppendLine("using System.Collections.Generic;");
            Builder.AppendLine("using Acadis.Constants.Accounting;");
            Builder.AppendLine("using Acadis.Constants.Admin;");
            Builder.AppendLine("using Acadis.SystemUtilities;");
            Builder.AppendLine();
        }

        public void BuildNamespaceOpen()
        {
            Builder.AppendLine("namespace Tabula");
            Builder.AppendLine("{");
        }


        public void BuildClassOpen()
        {
            GeneratedClassName = ClassNameFromInputFilePath();
            Builder.AppendLine($"    public class {GeneratedClassName}  //  {Scenario.Label}");
            Builder.AppendLine("        : GeneratedScenarioBase, IGeneratedScenario");
            Builder.AppendLine("    {");
        }

        public string ClassNameFromInputFilePath()
        {
            //  remove everything before the last backslash
            int lastBackslash = InputFilePath.LastIndexOf("\\");
            if (lastBackslash != -1)
                InputFilePath = InputFilePath.Substring(lastBackslash + 1);

            //  remove everything after the last dot
            int lastDot = InputFilePath.LastIndexOf('.');
            if (lastDot != -1)
                InputFilePath = InputFilePath.Remove(lastDot);

            return InputFilePath.Replace(' ', '_').Replace('.', '_') + "_generated";
        }

        public void BuildDeclarations()
        {
            foreach (var type in GetNeededWorkflowTypes())
            {
                var instance = Formatter.InstanceName_from_TypeName(type.Name);
                Builder.AppendLine($"public {type.Namespace}.{type.Name} {instance};");
            }
        }

        //TODO:  Workflow instantiation.  I should have the 
        public void BuildConstructor()
        {
            Builder.Append("        public ");
            Builder.Append(GeneratedClassName);
            Builder.AppendLine("()");
            Builder.AppendLine("            : base()");
            Builder.AppendLine("        {");

            BuildInstantiations();

            Builder.AppendLine("        }");
        }

        public void BuildInstantiations()
        {
            foreach (var type in GetNeededWorkflowTypes())
            {
                var instance = Formatter.InstanceName_from_TypeName(type.Name);
                Builder.AppendLine($"            {instance} = new {type.Namespace}.{type.Name}();");
            }
        }


        public void BuildClassBody()
        {
            foreach (var section in Scenario.Sections)
            {
                if (section is CST.Paragraph paragraph)
                    BuildParagraph(paragraph);
                if (section is CST.Table table)
                    BuildTable(table);
            }

            //TODO:  write final paragraph call unless a table has already run over it.
            //like:  FinishScenario() or similar

            //TODO:  Append the built text into the main Builder
        }

        public void BuildParagraph(CST.Paragraph paragraph)
        {
            //TODO:  set Paragraph.MethodName (at end of paragraph parse?)
            sectionsBody.AppendLine( $"public void {paragraph.MethodName}()");
            sectionsBody.AppendLine("{");
            sectionsBody.Indent();
            DispatchActions(paragraph.Actions);
            sectionsBody.Dedent();
            sectionsBody.AppendLine("}");
            sectionsBody.AppendLine();

            //TODO:  Handle called, uncalled, and final paragraph cases
            executeMethodBody.AppendLine(paragraph.MethodName + "();");
        }

        public void BuildTable(CST.Table table)
        {
            //generate table (table generator method, technically)
            //get staged paragraph name
            //AppendLine RunParaOverTable( {paraName}, {tableName} );
        }

        public void DispatchActions(List<CST.Action> actions)
        {
            foreach (var action in actions)
                DispatchAction(action);
        }

        public void DispatchAction(CST.Action action)
        {
            if (action is CST.CommandUse useCommand)
            {
                UseWorkflow(useCommand);
            }
            else if (action is CST.Step step)
            {
                BuildStep(step);
            }
            else if (action is CST.CommandSet setCommand)
            {
                BuildSetCommand(setCommand);
            }
            else if (action is CST.CommandAlias aliasCommand)
            {
                throw new NotImplementedException("TODO: Add the alias to the implementations");
            }
            else
            {
                throw new NotImplementedException(
                    $"The dispatcher doesn't have a case for [{action.GetType().FullName}].  Please extend it.");
            }
        }

        public void UseWorkflow(CST.CommandUse useCommand)
        {
            foreach (var label in useCommand.Workflows)
            {
                var searchName = Formatter.SearchName_from_Use_label(label);
                if (!Library.TypeFromSearchName.ContainsKey(searchName))
                    throw new Exception($"Tabula found no workflow matching [{label}].");

                var type = Library.TypeFromSearchName[searchName];
                var workflow = Library.CachedWorkflows[type];
                WorkflowsInScope.Remove(workflow);
                WorkflowsInScope.Add(workflow);
            }
        }

        public void BuildStep(CST.Step step)
        {
            //FUTURE:  method search names should include argument count, and FindImplementation should include a count argument

            int stepArgCount = step.Symbols.Where(s => s.Type != TokenType.Word).Count();

            string searchName = step.GetMethodSearchName();
            (WorkflowDetail workflow, MethodDetail method) = FindWorkflowMethod(searchName);

            var lineNumber = step.StartLine;
            var sourceLocation = $"\"{InputFilePath}:{lineNumber}\"";

            if (method == null || stepArgCount != method.Args.Count())
            {
                var stepText = "\"" + step.GetReadableString() + "\"";
                var unfound = $"Unfound(      {stepText}, {sourceLocation});";
                sectionsBody.AppendLine(unfound);
            }
            else
            {
                var call = ComposeCall(step, workflow, method);
                var quotedCall = "@\"" + call.Replace("\"", "\"\"") + "\"";
                sectionsBody.AppendLine($"Do(() =>       {call}, {sourceLocation}, {quotedCall});");
            }
        }

        public string ComposeCall(CST.Step step, WorkflowDetail workflow, MethodDetail method)
        {
            string argsString = "";
            string delim = "";
            foreach (var sym in step.Symbols.Where(s => s.Type != TokenType.Word))
            {
                switch (sym.Type)
                {
                    case TokenType.String:
                        argsString += delim + $"\"{sym.Text}\"";
                        break;

                    case TokenType.Date:
                        argsString += delim + $"\"{sym.Text}\".To<DateTime>()";
                        break;

                    case TokenType.Number:
                        argsString += delim + sym.Text;
                        break;

                    case TokenType.Variable:
                        throw new Exception("Not composing variables into calls yet.");

                    default:
                        throw new Exception($"Did not expect to get a token type [{sym.Type}].");
                }
                delim = ", ";
            }

            var workflowName = workflow.InstanceName;
            var methodName = method.Name;
            var call = $"{workflowName}.{methodName}({argsString})";

            return call;
        }


        public (WorkflowDetail workflow, MethodDetail method) FindWorkflowMethod(string searchName)
        {
            foreach(var workflow in (WorkflowsInScope as IEnumerable<WorkflowDetail>).Reverse())
            {
                if (workflow.Methods.ContainsKey(searchName))
                    return (workflow, workflow.Methods[searchName]);
            }

            return (null, null);
        }

        /// <summary> All the workflow types requested by the scenario </summary>
        /// <returns> List sorted by namespace, then name </returns>
        public List<Type> GetNeededWorkflowTypes()
        {
            List<string> nws = Scenario.SeenWorkflowRequests.ToList();

            var types = new List<Type>();
            foreach (var request in Scenario.SeenWorkflowRequests)
            {
                var searchName = Formatter.SearchName_from_Use_label(request);

                if (!Library.TypeFromSearchName.ContainsKey(searchName))
                    throw new Exception($"Tabula found no workflow matching [{request}].");

                var type = Library.TypeFromSearchName[searchName];
                if (!types.Contains(type))
                    types.Add(type);
            }

            return types.OrderBy(t => t.Namespace).ThenBy(t => t.Name).ToList();
        }

        public void BuildSetCommand(CST.CommandSet commandSet)
        {
            var lineNumber = commandSet.StartLine;
            var sourceLocation = $"\"{InputFilePath}:{lineNumber}\"";

            var call = $"Var[\"{commandSet.Name}\"] = {commandSet.Term};";
            var quotedCall = "@\"" + call.Replace("\"", "\"\"") + "\"";
            sectionsBody.AppendLine($"Do(() =>       {call}, {sourceLocation}, {quotedCall});");
        }

        public void BuildClassClose()
        {
            Builder.AppendLine("    }");
        }

        public void BuildNamespaceClose()
        {
            Builder.AppendLine("}");
        }
    }
}
