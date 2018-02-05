using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tabula
{
    //  In Tabula, the generated code is a first-class result.
    //  Effort is spent to keep it readable.

    public class Generator
    {
        public static string CurrentVersion { get => "0.2"; }
        private Dictionary<string, string> Versions = new Dictionary<string, string>
        {
            { "0.1", "only generates this rudimentary paste" },
            { "0.2", "can barely generate a compilable class" },
            { "0.3", "doesn't always generate a compilable class" },
            { "0.4", "is not yet alpha (known missing 1.0 features)" },
            //  feels unproductive to project further at this time (29 jan '18)
        };

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
    
            executeMethodBody = new IndentingStringBuilder(12);  // 12 = namespace + class + method
            sectionsBody = new IndentingStringBuilder(8);  // 8 = namespace + class
        }

        public void Generate(CST.Scenario scenario, string inputFilePath, StringBuilder builder)
        {
            Scenario = scenario;
            Builder = builder;
            InputFilePath = inputFilePath;

            //  Several parts (e.g., the list of needed workflows) are built into
            //  different StringBuilders as the CST is walked by PrepareSections(),
            //  then sequenced into the main builder with Write{Xxx} methods.
            PrepareSections();

            WriteHeader();
            WriteNamespaceOpen();
            WriteClassOpen();

            WriteExecuteScenario();
            WriteSectionMethods();

            WriteDeclarations();
            WriteConstructor();

            WriteClassClose();
            WriteNamespaceClose();
        }

        #region Prepare Sections (Paragraphs and Tables)
        public void PrepareSections()
        {
            foreach (var section in Scenario.Sections)
            {
                if (section is CST.Paragraph paragraph)
                    PrepareParagraph(paragraph);
                else if (section is CST.Table table)
                    PrepareTable(table);
                else
                    throw new Exception($"Tabula needs code to prepare section type [{section.GetType().FullName}].");
            }

            ClearStage();
        }

        public void PrepareParagraph(CST.Paragraph paragraph)
        {
            sectionsBody.AppendLine($"public void {paragraph.MethodName}()");  //TODO: add label as a comment
            sectionsBody.AppendLine("{");
            sectionsBody.Indent();
            PrepareActions(paragraph.Actions);
            sectionsBody.Dedent();
            sectionsBody.AppendLine("}");
            sectionsBody.AppendLine();

            StageParagraph(paragraph.MethodName);
        }

        public void PrepareTable(CST.Table table)
        {
            sectionsBody.AppendLine($"public Table {table.MethodName}()");  //TODO: if label exists, add it as a comment
            sectionsBody.AppendLine("{");
            sectionsBody.Indent();
            sectionsBody.AppendLine("return new Table {");
            sectionsBody.Indent();
            PrepareRows(table);
            sectionsBody.Dedent();
            sectionsBody.AppendLine("};");
            sectionsBody.Dedent();

            StageTable(table.MethodName);
        }

        private void PrepareRows(CST.Table table)
        {
            //TODO: header row in CST.Table
            //sectionsBody.AppendLine("Header = new List<string> {");
            //string headerText = "{ \"" + string.Join("\", \"", table.Header.Select(c => Formatter.Reescape(c))) + "\" }";

            sectionsBody.AppendLine("Data = new List<List<string>> {");
            sectionsBody.Indent();
            foreach (var row in table.Rows)
            {
                sectionsBody.Append("new List<string>          {");
                string cellsText = "{ \"" + string.Join("\", \"", row.Cells.Select(c => Formatter.Reescape(c))) + "\" }";
                sectionsBody.AppendLine($"new List<string>          {cellsText}");
            }
            sectionsBody.Dedent();
            sectionsBody.AppendLine("}");
        }

        public void StageParagraph(string paragraphName)
        {
            ClearStage();
            stagedParagraph = paragraphName;
            paragraphPending = true;
        }

        public void StageTable(string tableName)
        {
            //if (stagedParagraph == null) throw new Exception("Tables must come after paragraphs.");

            executeMethodBody.AppendLine($"Run_para_over_table( {stagedParagraph}, {tableName} );");
            paragraphPending = false;
        }

        private bool paragraphPending = false;
        private string stagedParagraph;
        public void ClearStage()
        {
            if (!paragraphPending) return;

            executeMethodBody.AppendLine(stagedParagraph + "();");
            paragraphPending = false;
        }

        #endregion

        public void WriteExecuteScenario()
        {
            Builder.AppendLine("        public void ExecuteScenario()");
            Builder.AppendLine("        {");
            Builder.Append(executeMethodBody.Builder);
            Builder.AppendLine("        }");
        }

        public void WriteHeader()
        {
            Builder.AppendLine($"//  This file was generated by TabulaClassGenerator version {CurrentVersion}.");
            Builder.AppendLine($"//  To change this file, change the Tabula scenario at {InputFilePath}.");
            Builder.AppendLine($"//  Version {CurrentVersion} {Versions[CurrentVersion]}.  You have been warned.");
            Builder.AppendLine("using System;");
            Builder.AppendLine("using System.Collections.Generic;");
            Builder.AppendLine("using Acadis.Constants.Accounting;");
            Builder.AppendLine("using Acadis.Constants.Admin;");
            Builder.AppendLine("using Acadis.SystemUtilities;");
            Builder.AppendLine();
        }

        public void WriteNamespaceOpen()
        {
            Builder.AppendLine("namespace Tabula");
            Builder.AppendLine("{");
        }


        public void WriteClassOpen()
        {
            GeneratedClassName = GetGeneratedClassName();
            Builder.AppendLine($"    public class {GeneratedClassName}  //  {Scenario.Label}");
            Builder.AppendLine("        : GeneratedScenarioBase, IGeneratedScenario");
            Builder.AppendLine("    {");
        }

        public string GetGeneratedClassName()
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

        public void WriteDeclarations()
        {
            foreach (var type in GetNeededWorkflowTypes())
            {
                var instance = Formatter.InstanceName_from_TypeName(type.Name);
                Builder.AppendLine($"public {type.Namespace}.{type.Name} {instance};");
            }
        }

        public void WriteConstructor()
        {
            Builder.Append("        public ");
            Builder.Append(GeneratedClassName);
            Builder.AppendLine("()");
            Builder.AppendLine("            : base()");
            Builder.AppendLine("        {");

            WriteInstantiations();

            Builder.AppendLine("        }");
        }

        public void WriteInstantiations()
        {
            foreach (var type in GetNeededWorkflowTypes())
            {
                var instance = Formatter.InstanceName_from_TypeName(type.Name);
                Builder.AppendLine($"            {instance} = new {type.Namespace}.{type.Name}();");
            }
        }


        public void WriteSectionMethods()
        {
            Builder.Append(sectionsBody.Builder);
        }

        public void PrepareActions(List<CST.Action> actions)
        {
            foreach (var action in actions)
                PrepareAction(action);
        }

        public void PrepareAction(CST.Action action)
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
                throw new NotImplementedException("TODO: Prepare Alias Command");
            }
            else
            {
                throw new NotImplementedException(
                    $"Tabula needs code to prepare action type [{action.GetType().FullName}].");
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

            int argIndex = 0;
            foreach (var sym in step.Symbols.Where(s => s.Type != TokenType.Word))
            {
                Type argType = method.Args[argIndex].Type;
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
                        argsString += delim + $"var[\"{sym.Text}\"]";
                        if (argType == typeof(string))
                        { }
                        if (argType == typeof(int))
                        {
                            argsString += ".To<int>()";
                        }
                        if (argType == typeof(DateTime))
                        {
                            argsString += ".To<DateTime>()";
                        }

                        break;

                    default:
                        throw new Exception($"Did not expect to get a token type [{sym.Type}].");
                }
                delim = ", ";
                argIndex++;
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

        public void WriteClassClose()
        {
            Builder.AppendLine("    }");
        }

        public void WriteNamespaceClose()
        {
            Builder.AppendLine("}");
        }
    }
}
