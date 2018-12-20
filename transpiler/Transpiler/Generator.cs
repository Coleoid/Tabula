using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tabula.CST;

namespace Tabula
{
    //  Tabula has two major goals:
    //  1:  It replaces Testopia with minimal fuss.
    //  2:  The developer-user experience is smooth.
    //  Keeping the generated code clean and readable goes directly to #2.

    public class Generator
    {
        public static string CurrentVersion { get => "0.4"; }
        private readonly Dictionary<string, string> Versions = new Dictionary<string, string>
        {
            { "0.1", "only generates this rudimentary paste" },
            { "0.2", "can barely generate a compilable class" },
            { "0.3", "doesn't always generate a compilable class" },
            { "0.4", "is not yet alpha (known missing 1.0 features)" },
            { "0.5", "is a very lean initial alpha, start trying to use it!" },
            //  feels unproductive to project further at this time (29 jan '18)
        };

        public string GeneratedClassName { get; set; }

        public CST.Scenario Scenario { get; set; }

        public StringBuilder Builder { get; set; }
        public IndentingStringBuilder executeMethodBody { get; set; }
        public IndentingStringBuilder sectionsBody { get; set; }

        public string InputFilePath { get; set; }

        public Dictionary<string, WorkflowDetail> Workflows { get; private set; }

        public WorkflowIntrospector Library { get; private set; }
        public CST.Paragraph CurrentParagraph { get; set; }

        public Generator()
        {
            Workflows = new Dictionary<string, WorkflowDetail>();
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

            WriteExecuteMethod();
            WriteSectionMethods();

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
                    throw new Exception($"Extend Tabula to prepare section type [{section.GetType().FullName}].");
            }

            ClearStage();
        }

        public void PrepareParagraph(CST.Paragraph paragraph)
        {
            CurrentParagraph = paragraph;

            sectionsBody
                .AppendLine($"public void {paragraph.MethodName}()")
                .AppendLine("{")
                .Indent();

            var tagsArg = string.Join(", ", paragraph.Tags);
            if (!string.IsNullOrEmpty(tagsArg))
                sectionsBody.AppendLine($"Tags(     \"{tagsArg}\");");  //TODO:  Add test for tags on paragraph

            var useCommands = paragraph.Actions.Where(a => a is CST.CommandUse).Select(a => (CST.CommandUse) a);
            PrepareUses(useCommands);
            foreach (WorkflowDetail workflow in paragraph.WorkflowsInScope)
            {
                sectionsBody
                    .AppendLine($"var {workflow.InstanceName} = new {workflow.Namespace}.{workflow.Name}();");
            }
            if (paragraph.WorkflowsInScope.Count() > 0)
                sectionsBody.AppendLine();

            sectionsBody.AppendLine($"Label(     \"{paragraph.Label}\");");
            
            PrepareActions(paragraph.Actions);
            
            sectionsBody
                .Dedent()
                .AppendLine("}")
                .AppendLine();

            StageParagraph(paragraph.MethodName);
        }

        public void PrepareTable(CST.Table table)
        {
            sectionsBody
                .AppendLine($"public Table {table.MethodName}()")  //TODO: if label exists, add it as a comment
                .AppendLine("{")
                .Indent()
                .AppendLine("return new Table {")
                .Indent();

            PrepareMetadata(table);
            PrepareRows(table);

            sectionsBody
                .Dedent()
                .AppendLine("};")
                .Dedent()
                .AppendLine("}")
                .AppendLine();

            StageTable(table.MethodName);
        }

        public void PrepareMetadata(CST.Table table)
        {
            sectionsBody
                .If(table.Tags.Any())  //  Does .If() make me a bad person?
                .Append("Tags = new List<string> { ")
                .Append(string.Join(", ", table.Tags.Select(t => "\"" + Formatter.Reescape(t) + "\"")))
                .AppendLine(" },");

            sectionsBody
                .If(!string.IsNullOrEmpty(table.Label))  //  Perhaps it's a cry for help.
                .Append("Label = \"")
                .Append(table.Label)
                .AppendLine("\",");
        }

        private void PrepareRows(CST.Table table)
        {
            sectionsBody
                .Append("Header = new List<string>     { ")
                .Append(string.Join(", ", table.ColumnNames.Select(c => "\"" + Formatter.Reescape(c) + "\"" )))
                .AppendLine(" },");

            sectionsBody
                .AppendLine("Data = new List<List<string>> {")
                .Indent();

            foreach (var row in table.Rows)
            {
                sectionsBody.AppendLine(CodeTextFrom(row) + ",");
            }

            sectionsBody
                .Dedent()
                .AppendLine("}");
        }

        //TODO:  Should these go in the CST objects themselves? 
        public string CodeTextFrom(CST.TableRow row)
        {
            var cellStrings = row.Cells.Select(c => CodeTextFrom(c));

            return "new List<string>          { " + string.Join(", ", cellStrings) + " }";
        }

        public string CodeTextFrom(List<string> cell)
        {
            return StringFrom(string.Join(" ", cell));
        }

        public string StringFrom(string input)
        {
            return "\"" + input.Replace("\\", "\\\\").Replace("\"", "\\") + "\"";
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

            executeMethodBody.AppendLine($"Foreach_Row_in( {tableName}, {stagedParagraph} );");
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

        public void WriteExecuteMethod()
        {
            Builder
                .AppendLine("        public void ExecuteScenario()")
                .AppendLine("        {")
                .Append(executeMethodBody.Builder)
                .AppendLine("        }")
                .AppendLine();
        }

        public void WriteHeader()
        {
            Builder
                .AppendLine($"//  This file was generated by TabulaClassGenerator version {CurrentVersion}.")
                .AppendLine($"//  To change this file, change the Tabula scenario at {InputFilePath}.")
                .AppendLine($"//  Version {CurrentVersion} {Versions[CurrentVersion]}.  You have been warned.")
                .AppendLine("using System;")
                .AppendLine("using System.Collections.Generic;")
                .AppendLine("using Acadis.Constants.Accounting;")
                .AppendLine("using Acadis.Constants.Admin;")
                .AppendLine("using Acadis.SystemUtilities;")
                .AppendLine("using Tabula.API;")
                .AppendLine();
        }

        public void WriteNamespaceOpen()
        {
            Builder.AppendLine("namespace Tabula");
            Builder.AppendLine("{");
        }


        public void WriteClassOpen()
        {
            GeneratedClassName = GetGeneratedClassName();
            Builder
                .AppendLine($"    //  \"{Scenario.Label}\"")
                .AppendLine($"    public class {GeneratedClassName}")
                .AppendLine("        : GeneratedScenarioBase, IGeneratedScenario")
                .AppendLine("    {");
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

        public void WriteConstructor()
        {
            Builder
                .Append("        public ")
                .Append(GeneratedClassName)
                .AppendLine("()")
                .AppendLine("            : base()")
                .AppendLine("        {");

            Builder.AppendLine("        }");
        }


        public void WriteSectionMethods()
        {
            Builder.Append(sectionsBody.Builder);
        }

        public void PrepareUses(IEnumerable<CST.CommandUse> useCommands)
        {
            foreach (var useCommand in useCommands)
                UseWorkflow(useCommand);
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
                BuildAliasCommand(aliasCommand);
            }
            else
            {
                throw new NotImplementedException(
                    $"Extend Tabula to prepare action type [{action.GetType().FullName}].");
            }
        }

        public void UseWorkflow(CST.CommandUse useCommand)
        {
            foreach (var label in useCommand.Workflows)
            {
                var searchName = Formatter.SearchName_from_Use_label(label);
                if (!Library.TypeFromSearchName.ContainsKey(searchName))
                    throw new Exception($"Tabula found no workflow matching [{label}].");

                Type type = Library.TypeFromSearchName[searchName];
                var workflow = Library.CachedWorkflows[type];
                CurrentParagraph.WorkflowsInScope.Remove(workflow);
                CurrentParagraph.WorkflowsInScope.Add(workflow);
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
                var stepText = step.GetReadableString();
                var unfound = $"Unfound(  {stepText}, @{sourceLocation});";
                sectionsBody.AppendLine(unfound);
            }
            else
            {
                (var call, var declarations) = ComposeCall(step, workflow, method);
                var quotedCall = "@\"" + call.Replace("\"", "\"\"") + "\"";
                foreach (var declaration in declarations)
                {
                    sectionsBody.AppendLine(declaration);
                }
                sectionsBody.AppendLine($"Do(() =>    {call}, @{sourceLocation}, {quotedCall});");
            }
        }


        public (string, List<string>) ComposeCall(CST.Step step, WorkflowDetail workflow, MethodDetail method)
        {
            List<string> declarations = new List<string>( );
            string argsString = "";
            string delim = "";
            string text = "";

            int argIndex = 0;
            foreach (var sym in step.Symbols.Where(s => s.Type != TokenType.Word))
            {
                Type paramType = method.Args[argIndex].Type;
                (string newArg, string newDeclaration) = Prepare_input_for_param(sym, paramType, step.StartLine, argIndex);
                if (newDeclaration != null)
                    declarations.Add(newDeclaration);
                argsString += delim + newArg;
                delim = ", ";
                argIndex++;
            }

            var workflowName = workflow.InstanceName;
            var methodName = method.Name;
            var call = $"{workflowName}.{methodName}({argsString})";

            return (call, declarations);
        }

        public string TokenToType(TokenType tokenType, Type paramType, string input)
        {
            var targetType = (paramType == typeof(List<string>) ? typeof(string) : paramType);
            bool typesNeedCast = true;
            var result = input;

            if (tokenType == TokenType.Number)
            {
                if (targetType == typeof(Int32) || 
                    targetType == typeof(float) || 
                    targetType == typeof(double) ||
                    targetType == typeof(decimal))
                {
                    typesNeedCast = false;
                }

                if (targetType == typeof(decimal))
                {
                    result += "m";
                }
            }

            if (targetType == typeof(string) && (tokenType == TokenType.String || tokenType == TokenType.Variable))
            {
                typesNeedCast = false;
            }

            if (tokenType == TokenType.Variable)
            {
                result = $"Var[\"{input}\"]";
            }

            if (tokenType == TokenType.String)
            {
                var replaced = Regex.Replace(input, "#(\\w+)", "{Var[\"$1\"]}");
                bool hasInterp = !input.Equals(replaced);
                string din = hasInterp ? "$" : "";  // dollar if needed
                result = $"{din}\"{replaced}\"";

                if (!hasInterp && targetType == typeof(int))
                {
                    int int_out;
                    if (paramType == typeof(int) && int.TryParse(input, out int_out))
                    {
                        result = input;
                        typesNeedCast = false;
                    }
                }
            }

            if (tokenType == TokenType.Number)
            {
                if (targetType == typeof(string))
                {
                    result = "\"" + input + "\"";
                    typesNeedCast = false;
                }
            }

            if (tokenType == TokenType.Date)
            {
                result = "\"" + input + "\"";
            }

            return result + (typesNeedCast ? CastToType(targetType) : "");
        }

        private string CastToType(Type argType)
        {
            var ns = argType.Namespace == "System" ? "" : argType.Namespace + ".";
            return $".To<{ns}{argType.Name}>()";
        }

        public (string, string) Prepare_input_for_param(Symbol sym, Type paramType, int lineNumber, int argIndex)
        {
            string text = string.Empty;
            string declaration = null;

            Type collectedType = null;
            if (paramType.GenericTypeArguments.Length > 0)
                collectedType = paramType.GenericTypeArguments[0];

            switch (sym.Type)
            {

                case TokenType.Collection:
                    string arg_name = $"arg_{lineNumber}_{argIndex}";

                    string comma = "";
                    string elements = string.Empty;
                    foreach (var value in (sym as SymbolCollection).Values)
                    {
                        elements += comma + TokenToType(value.Type, collectedType, value.Text);
                        comma = ", ";
                    }

                    declaration = $"var {arg_name} = new List<{collectedType.Name}> {{ {elements} }};";

                    text = arg_name;
                    break;

                case TokenType.String:
                    text = TokenToType(TokenType.String, paramType, sym.Text);

                    if (paramType == typeof(List<string>))
                    {
                        arg_name = $"arg_{lineNumber}_{argIndex}";
                        declaration = $"var {arg_name} = new List<string> {{ {text} }};";
                        text = arg_name;
                    }
                    break;

                case TokenType.Date:
                case TokenType.Number:
                case TokenType.Variable:
                    text = TokenToType(sym.Type, paramType, sym.Text);
                    break;

                default:
                    throw new Exception($"Did not expect to get a token type [{sym.Type}].");
            }

            return (text, declaration);
        }


        public (WorkflowDetail workflow, MethodDetail method) FindWorkflowMethod(string searchName)
        {
            foreach(var workflow in (CurrentParagraph.WorkflowsInScope as IEnumerable<WorkflowDetail>).Reverse())
            {
                if (workflow.Methods.ContainsKey(searchName))
                    return (workflow, workflow.Methods[searchName]);
            }

            return (null, null);
        }

        public void BuildSetCommand(CST.CommandSet commandSet)
        {
            var lineNumber = commandSet.StartLine;
            var sourceLocation = $"@\"{InputFilePath}:{lineNumber}\"";

            string varStart = "Var[";
            string varEnd = "]";
            string call = varStart + "\"" + commandSet.Name.Replace("#", "") + "\"" + varEnd;
            
            if (commandSet.Name.Contains('#'))
            {
                call = varStart + call + varEnd;
            }

            call += " = ";

            if (commandSet.Term.Type == TokenType.Variable)
            {
                call += $"Var[\"{commandSet.Term.Text}\"]";
            }
            else
            {
                call += $"\"{commandSet.Term.Text}\"";
            }

            var quotedCall = "@\"" + call.Replace("\"", "\"\"") + "\"";
            sectionsBody.AppendLine($"Do(() =>    {call}, {sourceLocation}, {quotedCall});");
        }

        //TODO: Alias POI
        private void BuildAliasCommand(CST.CommandAlias aliasCommand)
        {
            sectionsBody.AppendLine($"Alias(     {aliasCommand.Name}, {aliasCommand.Action} ); //TODO: Actually implement");
        }

        //TODO:  block__XXX_to_YYY()

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
