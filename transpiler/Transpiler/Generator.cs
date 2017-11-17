﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tabula
{
    public class ImplementationInfo
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public List<string> Args { get; set; }
    }

    public class Generator
    {
        private string GeneratorVersion = "0.1";

        public string GeneratedClassName { get; set; }
        public CST.Scenario Scenario { get; set; }
        public StringBuilder Builder { get; set; }
        public string InputFilePath { get; set; }
        public Dictionary<string, Dictionary<string, ImplementationInfo>> WorkflowImplementations { get; set; }

        public Generator()
        {
            WorkflowImplementations = new Dictionary<string, Dictionary<string, ImplementationInfo>>();
            WorkflowsInScope = new List<string>();
        }

        public void Generate(CST.Scenario scenario, string inputFilePath, StringBuilder builder)
        {
            Scenario = scenario;
            Builder = builder;
            InputFilePath = inputFilePath;

            BuildHeader();
            OpenNamespace();
            OpenClass();
            BuildConstructor();
            BuildClassBody();
            CloseClass();
            CloseNamespace();
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

        public void OpenNamespace()
        {
            Builder.AppendLine("namespace Tabula");
            Builder.AppendLine("{");
        }


        public void OpenClass()
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


        public void BuildClassBody()
        {
            var executeMethodBody = new IndentingStringBuilder(12);  // namespace + class + inside method
            var sectionsBody = new IndentingStringBuilder(8);  // namespace + class

            foreach (var section in Scenario.Sections)
            {

                var para = section as CST.Paragraph;
                if (para != null)
                {
                    WorkflowsInScope = new List<string>();

                    //TODO:  set Paragraph.MethodName (at end of paragraph parse?)
                    sectionsBody.AppendLine(para.MethodName + "()");
                    sectionsBody.AppendLine("{");
                    sectionsBody.Indent();
                    foreach (var action in para.Actions)
                    {
                        BuildAction(action);
                    }
                    sectionsBody.Dedent();
                    sectionsBody.AppendLine("}");

                    //TODO:  Handle called, uncalled, and final paragraph cases
                    executeMethodBody.AppendLine(para.MethodName + "();");
                }

                var table = section as CST.Table;
                if (table != null)
                {
                    //generate table (table generator method, technically)
                    //get staged paragraph name
                    //AppendLine RunParaOverTable( {paraName}, {tableName} );
                }
            }

            //TODO:  write final paragraph call unless a table has already run over it.


            //TODO:  Append the built text into the main Builder
        }

        public List<string> WorkflowsInScope { get; set; }

        public void BuildAction(CST.Action action)
        {
            if (action is CST.CommandUse useCommand)
            {
                foreach (var newWorkflowName in useCommand.Workflows)
                {
                    if (!WorkflowsInScope.Contains(newWorkflowName))
                    {
                        WorkflowsInScope.Add(newWorkflowName);
                    }
                }
            }
            else if (action is CST.Step step)
            {
                BuildStep(step);
            }
            else if (action is CST.CommandSet setCommand)
            {
                throw new NotImplementedException("TODO: Stash value in runtime context");
            }
            else if (action is CST.CommandAlias aliasCommand)
            {
                throw new NotImplementedException("TODO: Add the alias to the implementations");
            }
            else
                throw new NotImplementedException($"So {action.GetType().FullName} is an action now, huh?  Tell me what to do about that, please.");
        }

        public void BuildStep(CST.Step step)
        {
            string methodName = step.GetCanonicalMethodName();
            var implementation = FindImplementation(methodName);

            if (implementation == null)
            {
                var stepText = "hello world TODO";
                var lineNumber = "TODO"; //step.LineNumber;
                var sourceLocation = $"{InputFilePath}:{lineNumber}";

                //NOW: (divert down to get the source line number into our Steps)
                var unfound = $"            Unfound(      \"{stepText}\", \"{sourceLocation}\");";
                Builder.AppendLine(unfound);
            }
            //NEXT:
            //else
            //{
            //    List<string> args = new List<string> { "this", "that", "todo" };
            //    string delim = ", ";
            //    var argsText = string.Join(delim, args);

            //    Builder.AppendLine($"           {workflowName}.{methodName}({argsText});");
            //    //TODO:  rolling into .Do with lambda and text of line included
            //}
        }



        public ImplementationInfo FindImplementation(string lookupName)
        {
            foreach(var workflowName in WorkflowsInScope)
            {
                var implementations = WorkflowImplementations[workflowName];
                if (implementations.ContainsKey(lookupName))
                    return implementations[lookupName];
            }

            return null;
        }

        public List<string> GetNeededWorkflows()
        {
            List<string> nws = Scenario.NeededWorkflows;
            nws.Sort();
            var unws = nws.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

            //TODO:
            //  The 'peepEnrollment = new PeopleEnrollmentWorkflow();' lines are placed in
            //   each paragraph (or block) method, as the use command is encountered.
            //  And I need to think out not messing up the state of any workflows which rely
            //   on their state.  Perhaps we manually stash and replace workflow instances?
            //  Or, since I'm doing local initialization, I could switch to local declaration,
            //   also.  Then we need to find a way to know which workflows to pass as arguments,
            //   and do so with all consumers of the block.  More complex.
            //  I don't know where the real use cases will push us, so starting simple (and
            //   working to uncover the forces involved) seems like the plan.

            return unws;
        }

        //public void AddWorkflow(string workflowName)
        //{
        //    NeededWorkflows.Add(workflowName);
        //}

        public void BuildDeclarations()
        {
            foreach (var workflow in GetNeededWorkflows())
            {
                var varName = nameOfWorkflowInstance(workflow);
                Builder.AppendFormat("public {0} {1};{2}", workflow, varName, Environment.NewLine);
            }
        }

        //  Since workflow instantiation happens in each paragraph, this is (for now) a stub.
        public void BuildConstructor()
        {
            Builder.Append("        public ");
            Builder.Append(GeneratedClassName);
            Builder.AppendLine("()");
            Builder.AppendLine("            : base()");
            Builder.AppendLine("        {");
            Builder.AppendLine("        }");
        }

        public void CloseClass()
        {
            Builder.AppendLine("    }");
        }

        public void CloseNamespace()
        {
            Builder.AppendLine("}");
        }

        public string nameOfWorkflowInstance(string workflowName)
        {
            var lastDot = workflowName.LastIndexOf('.');
            return workflowName.Substring(lastDot + 1).Replace("Workflow", "");
        }
    }
}
