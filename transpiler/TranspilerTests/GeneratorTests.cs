using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        //TODO:  Report scenario errors in Visual Studio error list

        [TestCase("squangle_widgets_across_timezones.tab")]
        [TestCase("groplet_quality_within_tolerance.tab")]
        public void Header_warns_it_is_generated_and_tells_where_source_file_is(string fileName)
        {
            generator.InputFilePath = fileName;
            generator.BuildHeader();

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

            var className = generator.ClassNameFromInputFilePath();

            Assert.That(className, Is.EqualTo(expectedClassName));
        }

        [Test]
        public void Class_declaration_includes_base_and_interface()
        {
            generator.BuildClassOpen();

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
            generator.BuildClassOpen();

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
            var args = new List<string> {
                "name",
                "age",
                "birthday"
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
        public void BuildStep_Unfound_on_arg_count_mismatch()
        {
            var args = new List<string> {
                "name",
                "comment"
            };
            var detail = new WorkflowDetail { Name = "GreetingWorkflow", InstanceName = "myWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "user__made_comment__", Args = args });
            generator.WorkflowsInScope.Add(detail);

            var step = new CST.Step(222,
                (TokenType.Word, "user"),
                (TokenType.String, "Bob"),
                (TokenType.Word, "made"),
                (TokenType.Word, "comment") //,
                // (TokenType.String, "where am I?")  // if we add this token, we find the method
            );

            generator.BuildStep(step);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("Unfound"));
            Assert.That(result, Contains.Substring("@\"user \"\"Bob\"\" made comment\""));
        }

        
        //TODO:  Get the ClassName/ObjectName teased apart.  ClassName stays in ImplInfo, ObjectName goes to... Scope?
        //TODO:  Scoping of workflows

        class UnknownAction : CST.Action { }

        [Test]
        public void DispatchAction_explains_when_an_unknown_action_type_arrives()
        {
            var ex = Assert.Throws<NotImplementedException>(() => generator.DispatchAction(new UnknownAction()));
            Assert.That(ex.Message, Is.EqualTo(
                "The dispatcher doesn't have a case for [Tabula.GeneratorTests+UnknownAction].  Please extend it."));
        }

        [Test]
        public void BuildParagraph_gets_all_the_bits_together()
        {
            var args = new List<string> {
                "name",
                "comment"
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

            generator.BuildParagraph(paragraph);

            var result = generator.sectionsBody.ToString();
            Assert.That(result, Contains.Substring("public void paragraph_from_021_to_022()"));
            Assert.That(result, Contains.Substring("myWorkflow.user__made_comment__(\"Bob\", \"where am I?\")"));
        }
    }
}
