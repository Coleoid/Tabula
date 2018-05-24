using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tabula
{
    [TestFixture]
    public class GeneratorTests
    {
        StringBuilder builder;
        CST.Scenario scenario;
        Generator generator;

        [SetUp]
        public void SetUp()
        {
            builder = new StringBuilder();
            scenario = new CST.Scenario();
            generator = new Generator { Builder = builder, Scenario = scenario, InputFilePath = "scenario_source.tab" };
        }

        //FUTURE:  Report scenario errors in Visual Studio error list
        //FUTURE:  Levenshtein suggestions for unfound workflows and steps

        [TestCase("squangle_widgets_across_timezones.tab")]
        [TestCase("groplet_quality_within_tolerance.tab")]
        public void Header_warns_it_is_generated_and_tells_where_source_file_is(string fileName)
        {
            generator.InputFilePath = fileName;
            generator.WriteHeader();

            var result = builder.ToString();
            var lines = Regex.Split(result, Environment.NewLine);

            Assert.That(lines.Count(), Is.GreaterThan(2));
            Assert.That(lines[0], Does.Contain("generated"));
            Assert.That(lines[0], Does.Contain("TabulaClassGenerator"));
            Assert.That(lines[1], Does.Contain(fileName));
        }

        [TestCase("my_scenario.tab", "my_scenario_generated")]
        [TestCase("c:\\proj\\scenarios\\my.tab", "my_generated")]
        [TestCase("scenarios\\my_tests.v4.tab", "my_tests_v4_generated")]
        public void scenario_class_name_matches_file_name(string fileName, string expectedClassName)
        {
            generator.InputFilePath = fileName;

            var className = generator.GetGeneratedClassName();

            Assert.That(className, Is.EqualTo(expectedClassName));
        }

        [Test]
        public void Class_declaration_includes_base_and_interface()
        {
            generator.WriteClassOpen();

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($": GeneratedScenarioBase, IGeneratedScenario"));
        }

        [Test]
        public void Class_declaration_includes_scenario_label_as_comment()
        {
            var label = "AC-39393: Snock the cubid foratically";
            var scenario = new CST.Scenario { Label = label };
            generator.Scenario = scenario;
            generator.InputFilePath = "TuitionBilling";
            generator.WriteClassOpen();

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($"  //  {label}"));
        }

        [TestCase("ScenarioContext.Implementations.Administration.TaskRunnerWorkflow", "TaskRunner")]
        [TestCase("ScenarioContext.Implementations.Curriculum.AddEnrollmentWorkflow", "AddEnrollment")]
        public void instance_name_known_from_workflow(string workflowName, string expectedInstanceName)
        {
            string instanceName = Formatter.InstanceName_from_TypeName(workflowName);

            Assert.That(instanceName, Is.EqualTo(expectedInstanceName));
        }

        [Test]
        public void BuildStep_Unfound_includes_source_text_and_location()
        {
            var step = new CST.Step(12,
                (TokenType.Word, "hello"),
                (TokenType.Word, "world")
            );

            generator.BuildStep(step);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("@\"hello world\""));
            Assert.That(result, Contains.Substring("\"scenario_source.tab:12\""));
        }


        [TestCase("Do call with lambda", "Do(() =>")]
        [TestCase("source location", "\"scenario_source.tab:34\"")]
        [TestCase("workflow instance and method name", "myWorkflow.MyMethod_correctly_spelled()")]
        public void Step_Call_includes_(string description, string part)
        {
            var detail = new WorkflowDetail { Name = "GreetingWorkflow", InstanceName = "myWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "MyMethod_correctly_spelled" });
            generator.WorkflowsInScope.Add(detail);

            var step = new CST.Step(34,
                (TokenType.Word, "my"),
                (TokenType.Word, "method"),
                (TokenType.Word, "correctly"),
                (TokenType.Word, "spelled")
            );

            generator.BuildStep(step);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring(part), description);
        }

        [Test]
        public void Step_Call_includes_arguments()
        {
            var args = new List<ArgDetail>() {
                new ArgDetail { Name = "name", Type = typeof(string) },
                new ArgDetail { Name = "age", Type = typeof(int) },
                new ArgDetail { Name = "birthday", Type = typeof(DateTime) }
            };

            var detail = new WorkflowDetail { Name = "GreetingWorkflow", InstanceName = "myWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "My_friend__turned__on__", Args = args });
            generator.WorkflowsInScope.Add(detail);

            var step = new CST.Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.String, "Bob"),
                (TokenType.Word, "turned"),
                (TokenType.Number, "28"),
                (TokenType.Word, "on"),
                (TokenType.Date, "07/15/2017")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friend__turned__on__"));

            //  string arguments are quoted, ints aren't, dates go through conversion
            var expectedArguments = @"(""Bob"", 28, ""07/15/2017"".To<DateTime>())";
            //  to aid debugging, the Do() call includes a string copy of the args
            var requotedArguments = expectedArguments.Replace("\"", "\"\"");
            Assert.That(result, Contains.Substring(expectedArguments));
            Assert.That(result, Contains.Substring(requotedArguments));
        }

        [Test]
        public void BuildStep_sets_a_variable_on_CommandSet()
        {
            var term = new CST.Symbol(TokenType.String, "bar", 1);
            var command = new CST.CommandSet("foo", term) { StartLine = 12 };

            generator.BuildSetCommand(command);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Var[\"foo\"] = \"bar\","));
        }

        [Test]
        public void BuildStep_Unfound_on_arg_count_mismatch()
        {
            var args = new List<ArgDetail>() {
                new ArgDetail { Name = "name", Type = typeof(string) },
                new ArgDetail { Name = "comment", Type = typeof(string) }
            };


            var detail = new WorkflowDetail { Name = "GreetingWorkflow", InstanceName = "myWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "user__made_comment__", Args = args });
            generator.WorkflowsInScope.Add(detail);

            var step = new CST.Step(222,
                (TokenType.Word, "user"),
                (TokenType.String, "Bob"),
                (TokenType.Word, "made"),
                (TokenType.Word, "comment")
            );

            generator.BuildStep(step);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Unfound"));
            Assert.That(result, Contains.Substring("@\"user \"\"Bob\"\" made comment\""));
        }


        [Test]
        public void BuildStep_complaint_when_TokenType_unrecognized()
        {
            var args = new List<ArgDetail>() {
                new ArgDetail { Name = "name", Type = typeof(string) },
            };

            var detail = new WorkflowDetail { Name = "GreetingWorkflow", InstanceName = "myWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "user__thing", Args = args });
            generator.WorkflowsInScope.Add(detail);

            var step = new CST.Step(222,
                (TokenType.Word, "user"),
                (((TokenType)(-1)), "mystery"),
                (TokenType.Word, "thing")
            );

            var ex = Assert.Throws<Exception>(() => generator.BuildStep(step));
            Assert.That(ex.Message, Contains.Substring("token type [-1]"));
        }


        [Test]
        public void Step_Call_handles_variables()
        {
            var args = new List<ArgDetail>() {
                new ArgDetail { Name = "name", Type = typeof(string) },
                new ArgDetail { Name = "age", Type = typeof(int) },
                new ArgDetail { Name = "birthday", Type = typeof(DateTime) }
            };

            var detail = new WorkflowDetail { Name = "GreetingWorkflow", InstanceName = "myWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "My_friend__turned__on__", Args = args });
            generator.WorkflowsInScope.Add(detail);

            var step = new CST.Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, "friendname"),
                (TokenType.Word, "turned"),
                (TokenType.Number, "21"),
                (TokenType.Word, "on"),
                (TokenType.Date, "1/1/2001")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friend__turned__on__"));

            var expectedArguments = @"(var[""friendname""], 21, ""1/1/2001"".To<DateTime>())";
            Assert.That(result, Contains.Substring(expectedArguments));
        }

        [Test]
        public void Step_Call_variables_get_types()
        {
            var args = new List<ArgDetail>() {
                new ArgDetail { Name = "name", Type = typeof(string) },
                new ArgDetail { Name = "age", Type = typeof(int) },
                new ArgDetail { Name = "birthday", Type = typeof(DateTime) }
            };

            var detail = new WorkflowDetail { Name = "GreetingWorkflow", InstanceName = "myWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "My_friend__turned__on__", Args = args });
            generator.WorkflowsInScope.Add(detail);

            var step = new CST.Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, "friendname"),
                (TokenType.Word, "turned"),
                (TokenType.Variable, "newage"),
                (TokenType.Word, "on"),
                (TokenType.Variable, "birthday")
            );

            generator.BuildStep(step);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring("myWorkflow.My_friend__turned__on__"));

            //  variables going into an argument of type int need to get a .To<int>()
            //  variables going into an argument of type DateTime need to get a .To<DateTime>()
            var expectedArguments = @"(var[""friendname""], var[""newage""].To<int>(), var[""birthday""].To<DateTime>())";
            Assert.That(result, Contains.Substring(expectedArguments));
        }

        //TODO:  Args which are list or array types

        //TODO:  Scoping of workflows

        class UnknownAction : CST.Action { }

        [Test]
        public void DispatchAction_explains_when_an_unknown_action_type_arrives()
        {
            var ex = Assert.Throws<NotImplementedException>(() => generator.PrepareAction(new UnknownAction()));
            Assert.That(ex.Message, Is.EqualTo(
                "Extend Tabula to prepare action type [Tabula.GeneratorTests+UnknownAction]."));
        }

        [Test]
        public void BuildParagraph_gets_all_the_bits_together()
        {
            var args = new List<ArgDetail>() {
                new ArgDetail { Name = "name", Type = typeof(string) },
                new ArgDetail { Name = "comment", Type = typeof(string) }
            };
            var detail = new WorkflowDetail { Name = "GreetingWorkflow", InstanceName = "myWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "user__made_comment__", Args = args });
            generator.WorkflowsInScope.Add(detail);

            var step = new CST.Step(22,
                (TokenType.Word, "user"),
                (TokenType.String, "Bob"),
                (TokenType.Word, "made"),
                (TokenType.Word, "comment"),
                (TokenType.String, "where am I?")
            );

            var paragraph = new CST.Paragraph
            {
                Label = "short paragraph",
                MethodName = "paragraph_from_021_to_022"
            };
            paragraph.Actions.Add(step);

            generator.PrepareParagraph(paragraph);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("public void paragraph_from_021_to_022()"));
            Assert.That(result, Contains.Substring("myWorkflow.user__made_comment__(\"Bob\", \"where am I?\")"));
        }

        [TestCase("public Table table_from_030_to_035()", "method name")]
        [TestCase("Tags = new List<string> { \"Crucial\", \"AC-22222\" }", "tags")]
        [TestCase("Label = \"This is my label\",", "label")]
        [TestCase("Header = new List<string>     { \"First\", \"Second\" },", "header")]
        [TestCase("\"Blood\", \"Guessing\"", "row one")]
        public void BuildTable_gets_all_the_bits_together(string substring, string part)
        {
            /*
            Source:
            [Crucial] [AC-22222]
            == This is my label ==
            | First       | Second   |
            | Blood       | Guessing |
            | Time        | Chance   |
            | Impressions | Thoughts |

        Target:
        public Table table_from_030_to_035()
        {
            return new Table {
                Tags = new List<string> { "Crucial", "AC-22222" },
                Label = "This is my label",
                Header = new List<string>     { "First", "Second" },
                Data = new List<List<string>> {
                    new List<string>          { "Blood", "Guessing" },
                    new List<string>          { "Time", "Chance" },
                    new List<string>          { "Impressions", "Thoughts" },
                }
            };
        };
 */

            var cells = new List<List<string>> { new List<string> { "Blood" }, new List<string> { "Guessing" } };
            var row1 = new CST.TableRow(cells);
            cells = new List<List<string>> { new List<string> { "Time" }, new List<string> { "Chance" } };
            var row2 = new CST.TableRow(cells);
            cells = new List<List<string>> { new List<string> { "Impressions" }, new List<string> { "Thoughts" } };
            var row3 = new CST.TableRow(cells);

            var table = new CST.Table {
                MethodName = "table_from_030_to_035",
                Tags = new List<string> { "Crucial", "AC-22222" },
                Label = "This is my label",
                ColumnNames = new List<string> { "First", "Second" },
                Rows = new List<CST.TableRow> { row1, row2, row3 }
            };

            generator.PrepareTable(table);
            var result = generator.sectionsBody.ToString();

            Assert.That(result, Contains.Substring(substring), $"generated table is missing {part}.");
        }


        [Test]
        public void Row_ToCodeText_returns_text_for_List_of_string()
        {
            var cells = new List<List<string>>();
            var cell = new List<string> { "Fluffy" };
            cells.Add(cell);
            var row = new CST.TableRow(cells);

            var result = generator.Row_ToCodeText(row);

            Assert.That(result, Contains.Substring("new List<string>"));
            Assert.That(result, Contains.Substring("{ \"Fluffy\" }"));
        }

        [Test]
        public void WriteSectionMethods_puts_built_sections_into_main_StringBuilder()
        {
            generator.sectionsBody.AppendLine("all my sections");

            generator.WriteSectionMethods();

            string built = generator.Builder.ToString();
            Assert.That(built, Contains.Substring("all my sections"));
        }

        [Test]
        public void WriteExecuteMethod_put_built_required_interface_methods_into_main_StringBuilder()
        {
            generator.executeMethodBody.AppendLine("my required method");

            generator.WriteExecuteMethod();

            string built = generator.Builder.ToString();
            Assert.That(built, Contains.Substring("my required method"));
        }

        private class UnrecognizedSection : CST.Section { }

        [Test]
        public void Unrecognized_Section_type_complaint()
        {
            scenario.Sections.Add(new UnrecognizedSection { });
            var ex = Assert.Throws<Exception>(() => generator.PrepareSections());
            Assert.That(ex.Message, Contains.Substring("Extend Tabula"));
            Assert.That(ex.Message, Contains.Substring("UnrecognizedSection"));
        }

        [Test]
        public void Single_paragraph_scenario_gets_one_paragraph_call()
        {
            scenario.Sections.Add(new CST.Paragraph { MethodName = "paragraph_one" });

            generator.PrepareSections();

            string built = generator.executeMethodBody.ToString();
            Assert.That(built, Contains.Substring("paragraph_one();"));
        }

        [Test]
        public void two_paragraph_scenario_gets_both_calls()
        {
            scenario.Sections.Add(new CST.Paragraph { MethodName = "paragraph_one" });
            scenario.Sections.Add(new CST.Paragraph { MethodName = "paragraph_two" });

            generator.PrepareSections();

            string built = generator.executeMethodBody.ToString();
            Assert.That(built, Contains.Substring("paragraph_one();"));
            Assert.That(built, Contains.Substring("paragraph_two();"));
        }

        [Test]
        public void Single_paragraph_followed_by_table_gets_run_x_over_y_call()
        {
            scenario.Sections.Add(new CST.Paragraph { MethodName = "paragraph_one" });
            scenario.Sections.Add(new CST.Table { MethodName = "table_one" });

            generator.PrepareSections();

            string built = generator.executeMethodBody.ToString();
            Assert.That(built, Contains.Substring("Run_para_over_table( paragraph_one, table_one );"));
        }

        [Test]
        public void Single_paragraph_followed_by_two_tables_gets_two_calls()
        {
            scenario.Sections.Add(new CST.Paragraph { MethodName = "paragraph_one" });
            scenario.Sections.Add(new CST.Table { MethodName = "table_one" });
            scenario.Sections.Add(new CST.Table { MethodName = "table_two" });

            generator.PrepareSections();

            string built = generator.executeMethodBody.ToString();
            Assert.That(built, Contains.Substring("Run_para_over_table( paragraph_one, table_one );"));
            Assert.That(built, Contains.Substring("Run_para_over_table( paragraph_one, table_two );"));
        }
    }
}
