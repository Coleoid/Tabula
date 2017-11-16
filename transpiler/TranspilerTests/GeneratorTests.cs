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
            generator = new Generator { Builder = builder, Scenario = scenario };
        }

        //TODO:  Report scenario errors in Visual Studio error list

        [TestCase("squangle_widgets_across_timezones.tab")]
        [TestCase("groplet_quality_within_tolerance.tab")]
        public void Header_warns_it_is_generated_and_tells_where_source_file_is(string fileName)
        {
            generator.BuildHeader(fileName);

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
            generator.OpenClass(fileName);

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($"public class {expectedClassName}"));
        }

        [Test]
        public void Class_declaration_includes_base_and_interface()
        {
            generator.OpenClass("TuitionBilling");

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($": GeneratedScenarioBase, IGeneratedScenario"));
        }

        [Test]
        public void Class_declaration_includes_scenario_label_as_comment()
        {
            var label = "AC-39393: Snock the cubid foratically";
            var scenario = new CST.Scenario();
            scenario.Label = label;
            generator.Scenario = scenario;
            generator.OpenClass("TuitionBilling");

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
            string instanceName = generator.nameOfWorkflowInstance(workflowName);

            Assert.That(instanceName, Is.EqualTo(expectedInstanceName));
        }

        [Test, Ignore("until after handling no-arg version")]
        public void BuildAction_without_arguments()
        {
            //TODO: prepare a fixture class DoerWorkflow with instance name Doer and the method .Do_this()

            //  Step:  do this
            var symbols = new List<CST.Symbol> {
                new CST.Symbol(new Token(TokenType.Word, "do")),
                new CST.Symbol(new Token(TokenType.Word, "this")),
            };
            var action = new CST.Step(symbols);

            generator.BuildAction(action);

            Assert.That(generator.Builder.ToString(), Is.EqualTo("            Doer.Do_this();\r\n"));
        }

        [Test, Ignore("until after handling no-arg version")]
        public void BuildAction_includes_arguments()
        {
            //TODO: prepare a fixture class LoopyWorkflow with instance name Loopy and the method .Do_this__times(int count)

            //  Step:  do this 5 times
            var symbols = new List<CST.Symbol> {
                new CST.Symbol(new Token(TokenType.Word, "do")),
                new CST.Symbol(new Token(TokenType.Word, "this")),
                new CST.Symbol(new Token(TokenType.Number, "5")),
                new CST.Symbol(new Token(TokenType.Word, "times")),
            };
            var action = new CST.Step(symbols);

            generator.BuildAction(action);

            Assert.That(generator.Builder.ToString(), Is.EqualTo("            Loopy.Do_this__times(5);\r\n"));
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
        public void FindWorkflowImplenting_returns_null_if_none_found()
        {
            generator.WorkflowMethods["GreetingWorkflow"] = new List<string> { "howdystranger", "helloeverybody" };
            generator.WorkflowsInScope.Add("GreetingWorkflow");

            string workflow = generator.FindWorkflowImplementing("helloworld");

            Assert.That(workflow, Is.Null);
        }

        [Test]
        public void FindWorkflowImplenting_returns_null_if_workflow_not_in_scope()
        {
            generator.WorkflowMethods["GreetingWorkflow"] = new List<string> { "helloworld" };

            string workflow = generator.FindWorkflowImplementing("helloworld");

            Assert.That(workflow, Is.Null);
        }

        [Test]
        public void FindWorkflowImplenting_returns_name_if_method_matched()
        {
            generator.WorkflowMethods["GreetingWorkflow"] = new List<string> { "howdystranger", "helloworld" };
            generator.WorkflowsInScope.Add("GreetingWorkflow");

            string workflowName = generator.FindWorkflowImplementing("helloworld");

            Assert.That(workflowName, Is.EqualTo("GreetingWorkflow"));
        }

        //TODO: Use command will complain sensibly if we try to use a workflow which does not exist...

        //[Test]
        //public void FindWorkflowImplenting_returns_first_match_if_several_workflows_implement()
        //{
        //    //  This is true now, but we may need a different decision later
        //    generator.WorkflowMethods["GreetingWorkflow"] = new List<string> { "howdystranger", "helloworld" };
        //    generator.WorkflowsInScope.Add("GreetingWorkflow");
        //    generator.WorkflowsInScope.Add("GreetingWorkflow");

        //    string workflowName = generator.FindWorkflowImplementing("helloworld");

        //    Assert.That(workflowName, Is.EqualTo("GreetingWorkflow"));
        //}

    }
}
