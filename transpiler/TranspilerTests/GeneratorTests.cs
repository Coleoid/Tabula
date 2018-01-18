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
            generator.InputFilePath = "TuitionBilling";
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

        [Test]
        public void declarations_empty_to_start()
        {
            generator.BuildDeclarations();
            var declarations = builder.ToString();

            Assert.That(declarations, Is.EqualTo(string.Empty));
        }

        [Test]
        public void one_declaration_per_needed_workflow()
        {
            var wfs = new List<string> {
                "ScenarioContext.Implementations.Administration.TaskRunnerWorkflow",
                "ScenarioContext.Implementations.Curriculum.AddEnrollmentWorkflow"
            };
            scenario.NeededWorkflows = wfs;

            generator.BuildDeclarations();

            var declarations = builder.ToString();
            var lines = Regex.Split(declarations, Environment.NewLine);

            Assert.That(lines.Count(), Is.EqualTo(3));
            Assert.That(lines[0], Is.EqualTo("public ScenarioContext.Implementations.Administration.TaskRunnerWorkflow TaskRunner;"));
            Assert.That(lines[1], Is.EqualTo("public ScenarioContext.Implementations.Curriculum.AddEnrollmentWorkflow AddEnrollment;"));
            Assert.That(lines[2], Is.EqualTo(string.Empty));
        }

        [TestCase("ScenarioContext.Implementations.Administration.TaskRunnerWorkflow", "TaskRunner")]
        [TestCase("ScenarioContext.Implementations.Curriculum.AddEnrollmentWorkflow", "AddEnrollment")]
        public void instance_name_known_from_workflow(string workflowName, string expectedInstanceName)
        {
            string instanceName = Formatter.ClassName_to_InstanceName(workflowName);

            Assert.That(instanceName, Is.EqualTo(expectedInstanceName));
        }

        [Test]
        public void BuildAction_Use_command_Adds_workflows_to_scope()
        {
            var first = new WorkflowDetail { Name = "FirstWorkflow" };
            var another = new WorkflowDetail { Name = "AnotherWorkflow" };
            var third = new WorkflowDetail { Name = "AThirdWorkflow" };
            generator.Library.KnownWorkflows["firstworkflow"] = first;
            generator.Library.KnownWorkflows["anotherworkflow"] = another;
            generator.Library.KnownWorkflows["athirdworkflow"] = third;

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(0));

            var action = new CST.CommandUse(new List<string> { "FirstWorkflow" });
            generator.BuildAction(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowsInScope[0], Is.SameAs(first));

            action = new CST.CommandUse(new List<string> { "AnotherWorkflow", "AThirdWorkflow" });
            generator.BuildAction(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(3));
            Assert.That(generator.WorkflowsInScope[2], Is.SameAs(third));
        }

        [Test]
        public void BuildAction_will_not_add_duplicate_workflows()
        {
            var first = new WorkflowDetail { Name = "FirstWorkflow" };
            generator.Library.KnownWorkflows["firstworkflow"] = first;

            var action = new CST.CommandUse(new List<string> { "FirstWorkflow" });
            generator.BuildAction(action);

            action = new CST.CommandUse(new List<string> { "FirstWorkflow" });
            generator.BuildAction(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowsInScope[0], Is.SameAs(first));
        }


        [Test]
        public void FindWorkflowMethod_returns_nulls_if_no_workflow_has_method_matching_step()
        {
            var detail = new WorkflowDetail { Name = "GreetingWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "Howdy_stranger" });
            detail.AddMethod(new MethodDetail { Name = "Hello_Everybody" });

            generator.Library.KnownWorkflows["GreetingWorkflow"] = detail;
            generator.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(workflow, Is.Null);
            Assert.That(method, Is.Null);
        }

        [Test]
        public void FindWorkflowMethod_only_searches_impls_in_scope()
        {
            var detail = new WorkflowDetail { Name = "GreetingWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "HelloWorld" });
            generator.Library.KnownWorkflows["GreetingWorkflow"] = detail;

            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(workflow, Is.Null);
            Assert.That(method, Is.Null);
        }

        [Test]
        public void FindWorkflowMethod_returns_implementation_when_lookup_matches()
        {
            var detail = new WorkflowDetail { Name = "GreetingWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "HelloWorld" });

            generator.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(workflow, Is.Not.Null);
            Assert.That(method, Is.Not.Null);
        }

        //TODO: Use command will complain sensibly if we try to use a workflow which does not exist...

        [Test]
        public void FindWorkflowMethod_returns_last_match_if_several_workflows_implement()
        {
            var detail = new WorkflowDetail { Name = "GreetingWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "Howdy_stranger" });
            generator.WorkflowsInScope.Add(detail);

            detail = new WorkflowDetail { Name = "SheriffWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "Howdy_stranger" });
            generator.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("howdystranger");

            //  last added being returned allows for overriding, sounds more sensible at the moment
            Assert.That(workflow.Name, Is.EqualTo("SheriffWorkflow"));
        }


        [Test]
        public void BuildStep_Unfound_includes_step_text()
        {
            var step = new CST.Step(12,
                (TokenType.Word, "hello"),
                (TokenType.Word, "world")
            );

            generator.BuildStep(step);

            var result = generator.Builder.ToString();
            Assert.That(result, Contains.Substring("@\"hello world\""));
        }

        [Test]
        public void BuildStep_Unfound_includes_source_file_and_location()
        {
            var step = new CST.Step(12,
                (TokenType.Word, "hello"),
                (TokenType.Word, "world")
            );

            generator.BuildStep(step);

            var result = generator.Builder.ToString();
            Assert.That(result, Contains.Substring("\"scenario_source.tab:12\""));
        }

        [Test]
        public void Do_Call_includes_numerous_parts()
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

            var result = generator.Builder.ToString();
            Assert.That(result, Contains.Substring("Do(() =>     "));
            Assert.That(result, Contains.Substring("myWorkflow.MyMethod_correctly_spelled()"));
            Assert.That(result, Contains.Substring("\"scenario_source.tab:34\""));
        }

        [Test]
        public void Do_Call_includes_arguments()
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

            var result = generator.Builder.ToString();
            var expectedArguments = @"(""Bob"", 28, ""07/15/2017"".To<DateTime>())";
            var requotedArguments = expectedArguments.Replace("\"", "\"\"");
            Assert.That(result, Contains.Substring("myWorkflow.My_friend__turned__on__"));
            Assert.That(result, Contains.Substring(expectedArguments));
            Assert.That(result, Contains.Substring(requotedArguments));
        }


        [Test]
        public void BuilStep_will_complain_on_arg_count_mismatch()
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

            var result = generator.Builder.ToString();
            Assert.That(result, Contains.Substring("Unfound"));
            Assert.That(result, Contains.Substring("@\"user \"\"Bob\"\" made comment\""));
        }

        
        //TODO:  Get the ClassName/ObjectName teased apart.  ClassName stays in ImplInfo, ObjectName goes to... Scope?
        //TODO:  Scoping of workflows

        class UnknownAction : CST.Action { }

        [Test]
        public void BuildAction_explains_when_an_unknown_action_type_arrives()
        {
            var ex = Assert.Throws<NotImplementedException>(() => generator.BuildAction(new UnknownAction()));
            Assert.That(ex.Message, Is.EqualTo("So Tabula.GeneratorTests+UnknownAction is an action now, huh?  Tell me what to do about that, please."));
        }
    }
}
