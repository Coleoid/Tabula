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
            generator.BuildClassOpen();

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($"public class {expectedClassName}"));
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
            string instanceName = generator.GetNameOfWorkflowInstance(workflowName);

            Assert.That(instanceName, Is.EqualTo(expectedInstanceName));
        }

        [Test]
        public void BuildAction_remembers_Use_command_workflows()
        {
            var action = new CST.CommandUse(new List<string> { "FirstWorkflow" });
            generator.BuildAction(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowsInScope[0], Is.EqualTo("FirstWorkflow"));

            action = new CST.CommandUse(new List<string> { "AnotherWorkflow", "AThirdWorkflow" });
            generator.BuildAction(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(3));
            Assert.That(generator.WorkflowsInScope[2], Is.EqualTo("AThirdWorkflow"));
        }

        [Test]
        public void BuildAction_will_not_add_duplicate_workflows()
        {
            var action = new CST.CommandUse(new List<string> { "FirstWorkflow" });
            generator.BuildAction(action);

            action = new CST.CommandUse(new List<string> { "FirstWorkflow" });
            generator.BuildAction(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowsInScope[0], Is.EqualTo("FirstWorkflow"));
        }


        [Test]
        public void FindImplementation_returns_null_if_no_impl_has_match_for_step()
        {
            var impls = new List<KeyValuePair<string, ImplementationInfo>>
            {
                new KeyValuePair<string, ImplementationInfo>("howdystranger", new ImplementationInfo { ClassName = "GreetingWorkflow", MethodName = "Howdy_stranger" }),
                new KeyValuePair<string, ImplementationInfo>("helloeverybody", new ImplementationInfo { ClassName = "GreetingWorkflow", MethodName = "Hello_Everybody" }),
            };

            generator.WorkflowImplementations["GreetingWorkflow"] = impls;
            generator.WorkflowsInScope.Add("GreetingWorkflow");

            var implementation = generator.FindImplementation("helloworld");

            Assert.That(implementation, Is.Null);
        }

        [Test]
        public void FindImplementation_only_searches_impls_in_scope()
        {
            var impls = new List<KeyValuePair<string, ImplementationInfo>>
            {
                new KeyValuePair<string, ImplementationInfo>("helloworld", new ImplementationInfo { ClassName = "GreetingWorkflow", MethodName = "HelloWorld" }),
            };
            generator.WorkflowImplementations["GreetingWorkflow"] = impls;

            var implementation = generator.FindImplementation("helloworld");

            Assert.That(implementation, Is.Null);
        }

        [Test]
        public void FindImplementation_returns_implementation_when_lookup_matches()
        {
            var impls = new List<KeyValuePair<string, ImplementationInfo>>
            {
                new KeyValuePair<string, ImplementationInfo>("howdystranger", new ImplementationInfo { ClassName = "GreetingWorkflow", MethodName = "Howdy_stranger" }),
                new KeyValuePair<string, ImplementationInfo>("helloeverybody", new ImplementationInfo { ClassName = "GreetingWorkflow", MethodName = "Hello_Everybody" }),
                new KeyValuePair<string, ImplementationInfo>("helloworld", new ImplementationInfo { ClassName = "GreetingWorkflow", MethodName = "HelloWorld" }),
            };
            generator.WorkflowImplementations["GreetingWorkflow"] = impls;
            generator.WorkflowsInScope.Add("GreetingWorkflow");

            var implementation = generator.FindImplementation("helloworld");

            Assert.That(implementation, Is.Not.Null);
            Assert.That(implementation.ClassName, Is.EqualTo("GreetingWorkflow"));
        }

        //TODO: Use command will complain sensibly if we try to use a workflow which does not exist...

        [Test]
        public void FindImplementation_returns_last_match_if_several_workflows_implement()
        {
            var impls = new List<KeyValuePair<string, ImplementationInfo>>
            {
                new KeyValuePair<string, ImplementationInfo>("howdystranger", new ImplementationInfo { ClassName = "GreetingWorkflow", MethodName = "Howdy_stranger" }),
                new KeyValuePair<string, ImplementationInfo>("howdystranger", new ImplementationInfo { ClassName = "SheriffWorkflow", MethodName = "Howdy_stranger" })
            };
            generator.WorkflowImplementations["GreetingWorkflow"] = impls;

            generator.WorkflowsInScope.Add("GreetingWorkflow");
            generator.WorkflowsInScope.Add("SheriffWorkflow");

            var workflow = generator.FindImplementation("howdystranger");

            //  last added being returned allows for overriding, sounds more sensible at the moment
            Assert.That(workflow.ClassName, Is.EqualTo("SheriffWorkflow"));
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
            generator.AddImplementation(new ImplementationInfo { ObjectName = "myWorkflow", MethodName = "MyMethod_correctly_spelled" } );
            generator.WorkflowsInScope.Add("myWorkflow");

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
            generator.AddImplementation(new ImplementationInfo { ObjectName = "myWorkflow", MethodName = "My_friend__turned__on__" });
            generator.WorkflowsInScope.Add("myWorkflow");

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
            Assert.That(result, Contains.Substring(expectedArguments));
            Assert.That(result, Contains.Substring(requotedArguments));
        }


        [Test]
        public void AddImplementation_stores_implementation_under_object_name()
        {
            var ii = new ImplementationInfo { ObjectName = "myWorkflow", MethodName = "My_friend__turned__on__" };

            generator.AddImplementation(ii);

            Assert.That(generator.WorkflowImplementations, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowImplementations.ContainsKey("myWorkflow"));
        }

        [Test]
        public void AddImplementation_puts_methods_from_same_class_into_same_collection()
        {
            generator.AddImplementation(new ImplementationInfo { ObjectName = "myWorkflow", MethodName = "foo_bar" });
            generator.AddImplementation(new ImplementationInfo { ObjectName = "myWorkflow", MethodName = "bar_barian" });

            Assert.That(generator.WorkflowImplementations, Has.Count.EqualTo(1));
            var impl = generator.WorkflowImplementations["myWorkflow"];
            Assert.That(impl, Has.Count.EqualTo(2));
        }

        [Test]
        public void AddImplementation_puts_methods_from_different_classes_into_different_collection()
        {
            generator.AddImplementation(new ImplementationInfo { ObjectName = "myWorkflow", MethodName = "foo_bar" });
            generator.AddImplementation(new ImplementationInfo { ObjectName = "yourWorkflow", MethodName = "bar_barian" });

            Assert.That(generator.WorkflowImplementations, Has.Count.EqualTo(2));
            var impl = generator.WorkflowImplementations["myWorkflow"];
            Assert.That(impl, Has.Count.EqualTo(1));
        }

        [Test]
        public void AddImplementation_resolves_ClassName_to_ObjectName()
        {
            var ii = new ImplementationInfo { ClassName = "Namespace.Of.App.MyWorkflow", MethodName = "HelloWorld" };

            //TODO:  The mapping of "Namespace.Of.App.MyWorkflow" to "myWorkflow"

            generator.AddImplementation(ii);

            var impl = generator.WorkflowImplementations["myWorkflow"];
            Assert.That(impl, Has.Count.EqualTo(1));
            Assert.That(impl[0].Key, Is.EqualTo("helloworld"));
        }

        //TODO:  Get the ClassName/ObjectName teased apart.  ClassName stays in ImplInfo, ObjectName goes to... Scope?
        //TODO:  Scoping of workflows

        [TestCase("My_friend__turned__on__", "myfriendturnedon")]
        [TestCase("hello___WORLD", "helloworld")]
        public void AddImplementation_stores_info_under_method_search_key(string methodName, string searchKey)
        {
            var ii = new ImplementationInfo { ObjectName = "myWorkflow", MethodName = methodName };

            generator.AddImplementation(ii);

            var impl = generator.WorkflowImplementations["myWorkflow"];
            Assert.That(impl, Has.Count.EqualTo(1));
            Assert.That(impl[0].Key, Is.EqualTo(searchKey));
        }

        class UnknownAction : CST.Action { }

        [Test]
        public void BuildAction_explains_when_an_unknown_action_type_arrives()
        {
            var ex = Assert.Throws<NotImplementedException>(() => generator.BuildAction(new UnknownAction()));
            Assert.That(ex.Message, Is.EqualTo("So Tabula.GeneratorTests+UnknownAction is an action now, huh?  Tell me what to do about that, please."));
        }
    }
}
