using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tabula
{
    [TestFixture]
    public class Generator_WorkflowTests
    {
        StringBuilder builder;
        CST.Scenario scenario;
        Generator generator;

        WorkflowDetail first;
        WorkflowDetail second;
        WorkflowDetail another;

        [SetUp]
        public void SetUp()
        {
            builder = new StringBuilder();
            scenario = new CST.Scenario();
            generator = new Generator { Builder = builder, Scenario = scenario, InputFilePath = "scenario_source.tab" };
            first = generator.Library.GetWorkflowDetail(typeof(FirstTestWorkflow));
            second = generator.Library.GetWorkflowDetail(typeof(SecondTestWorkflow));
            another = generator.Library.GetWorkflowDetail(typeof(extra.AnotherTestWorkflow));
        }


        [Test]
        public void DispatchAction_sends_Use_commands_to_UseWorkflow()
        {
            var action = new CST.CommandUse(new List<string> { "FirstTestWorkflow" });
            generator.DispatchAction(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowsInScope[0], Is.SameAs(first));

            generator.WorkflowsInScope.Clear();
            generator.UseWorkflow(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowsInScope[0], Is.SameAs(first));
        }

        [TestCase("FirstTestWorkflow")]
        [TestCase("first test workflow")]
        [TestCase("FIRST test")]
        public void Use_command_finds_workflows_under_several_spellings(string requestedWorkflow)
        {
            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(0));

            var action = new CST.CommandUse(new List<string> { requestedWorkflow });
            generator.UseWorkflow(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            var scopedWorkflow = generator.WorkflowsInScope[0];

            Assert.That(scopedWorkflow.Name, Is.EqualTo("FirstTestWorkflow"));
        }

        [Test]
        public void Use_command_Adds_workflows_to_scope()
        {
            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(0));

            var action = new CST.CommandUse(new List<string> { "FirstTestWorkflow" });
            generator.UseWorkflow(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowsInScope[0], Is.SameAs(first));

            action = new CST.CommandUse(new List<string> { "SecondTestWorkflow", "AnotherTestWorkflow" });
            generator.UseWorkflow(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(3));
            Assert.That(generator.WorkflowsInScope[2], Is.SameAs(another));
        }

        [Test]
        public void UseWorkflow_will_not_add_duplicate_workflows()
        {
            var action = new CST.CommandUse(new List<string> { "FirstTestWorkflow" });
            generator.UseWorkflow(action);

            action = new CST.CommandUse(new List<string> { "First Test Workflow" });
            generator.UseWorkflow(action);

            Assert.That(generator.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(generator.WorkflowsInScope[0], Is.SameAs(first));
        }

        [Test]
        public void BuildDeclarations_complains_sensibly_when_workflow_unfound()
        {
            scenario.SeenWorkflowRequests = new List<string> { "P equals NP workflow" };
            var ex = Assert.Throws<Exception>(() => generator.BuildDeclarations());

            Assert.That(ex.Message, Is.EqualTo("Tabula found no workflow matching [P equals NP workflow]."));
        }

        //  In normal use, we'll stop before we hit this exception, at BuildDeclarations()
        [Test]
        public void Use_command_complains_sensibly_when_workflow_unfound()
        {
            var action = new CST.CommandUse(new List<string> { "P equals NP workflow" });
            var ex = Assert.Throws<Exception>(() => generator.UseWorkflow(action));

            Assert.That(ex.Message, Is.EqualTo("Tabula found no workflow matching [P equals NP workflow]."));
        }

        [Test]
        public void FindWorkflowMethod_returns_nulls_if_no_workflow_has_method_matching_step()
        {
            var detail = new WorkflowDetail { Name = "GreetingWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "Howdy_stranger" });
            detail.AddMethod(new MethodDetail { Name = "Hello_Everybody" });

            generator.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(workflow, Is.Null);
            Assert.That(method, Is.Null);
        }

        [Test]
        public void FindWorkflowMethod_only_searches_impls_in_scope()
        {
            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");
            Assert.That(workflow, Is.Null);
            Assert.That(method, Is.Null);

            generator.WorkflowsInScope.Add(first);

            (workflow, method) = generator.FindWorkflowMethod("helloworld");
            Assert.That(workflow, Is.Not.Null);
            Assert.That(method, Is.Not.Null);
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

            //  tie breaking on last added allows for overriding, sounds sensible at the moment
            Assert.That(workflow.Name, Is.EqualTo("SheriffWorkflow"));
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
            scenario.SeenWorkflowRequests = new List<string> {
                "first test",
                "anothertestWorkflow",
                "FirstTest WORKFLOW"
            };

            generator.BuildDeclarations();

            var declarations = builder.ToString();
            var lines = Regex.Split(declarations, Environment.NewLine);

            Assert.That(lines[0], Is.EqualTo("public Tabula.FirstTestWorkflow FirstTest;"));
            Assert.That(lines[1], Is.EqualTo("public Tabula.extra.AnotherTestWorkflow AnotherTest;"));
            Assert.That(lines.Count(), Is.EqualTo(3));
            Assert.That(lines[2], Is.EqualTo(string.Empty));
        }
    }

    public class TestWorkflowBase
    { }

    public class FirstTestWorkflow : TestWorkflowBase
    {
        public void Hello_World() { }
    }

    public class SecondTestWorkflow : TestWorkflowBase
    { }

    namespace extra
    {
        public class AnotherTestWorkflow : TestWorkflowBase
        { }
    }
}
