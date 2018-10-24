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
        public void PrepareAction_puts_workflows_in_paragraph_scope()
        {
            var paragraph = new CST.Paragraph();
            generator.CurrentParagraph = paragraph;
            var action = new CST.CommandUse(new List<string> { "FirstTestWorkflow" });
            generator.PrepareAction(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(paragraph.WorkflowsInScope[0], Is.SameAs(first));

            paragraph.WorkflowsInScope.Clear();
            generator.UseWorkflow(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(paragraph.WorkflowsInScope[0], Is.SameAs(first));
        }

        [TestCase("FirstTestWorkflow")]
        [TestCase("first test workflow")]
        [TestCase("FIRST test")]
        public void Use_command_finds_workflows_under_several_spellings(string requestedWorkflow)
        {
            var paragraph = new CST.Paragraph();
            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(0));
            generator.CurrentParagraph = paragraph;

            var action = new CST.CommandUse(new List<string> { requestedWorkflow });
            generator.UseWorkflow(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(1));
            var scopedWorkflow = paragraph.WorkflowsInScope[0];

            Assert.That(scopedWorkflow.Name, Is.EqualTo("FirstTestWorkflow"));
        }

        [Test]
        public void Use_command_Adds_workflows_to_scope()
        {
            var paragraph = new CST.Paragraph();
            generator.CurrentParagraph = paragraph;
            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(0));

            var action = new CST.CommandUse(new List<string> { "FirstTestWorkflow" });
            generator.UseWorkflow(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(paragraph.WorkflowsInScope[0], Is.SameAs(first));

            action = new CST.CommandUse(new List<string> { "SecondTestWorkflow", "AnotherTestWorkflow" });
            generator.UseWorkflow(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(3));
            Assert.That(paragraph.WorkflowsInScope[2], Is.SameAs(another));
        }

        //TODO: make sure that in-paragraph workflow instance names clip off 'workflow'

        //TODO: second and subsequent workflow usages don't cause a new 'var myfixture = ...' line

        //TODO: in-paragraph instantiations are of 'var xx' locals, and no declarations block at bottom of class.

        [Test]
        public void UseWorkflow_will_not_add_duplicate_workflows()
        {
            var paragraph = new CST.Paragraph();
            generator.CurrentParagraph = paragraph;
            var action = new CST.CommandUse(new List<string> { "FirstTestWorkflow" });
            generator.UseWorkflow(action);

            action = new CST.CommandUse(new List<string> { "First Test Workflow" });
            generator.UseWorkflow(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(paragraph.WorkflowsInScope[0], Is.SameAs(first));
        }

        [Test]
        public void BuildDeclarations_complains_sensibly_when_workflow_unfound()
        {
            scenario.SeenWorkflowRequests = new List<string> { "P equals NP workflow" };
            var ex = Assert.Throws<Exception>(() => generator.WriteDeclarations());

            Assert.That(ex.Message, Is.EqualTo("Tabula found no workflow matching [P equals NP workflow]."));
        }

        //  In normal use, we'll stop before we hit this exception, at BuildDeclarations()
        [Test]
        public void Use_command_complains_sensibly_when_workflow_unfound()
        {
            var paragraph = new CST.Paragraph();
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
            var paragraph = new CST.Paragraph();
            generator.CurrentParagraph = paragraph;
            paragraph.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(workflow, Is.Null);
            Assert.That(method, Is.Null);
        }

        [Test]
        public void FindWorkflowMethod_only_searches_impls_in_scope()
        {
            generator.CurrentParagraph = new CST.Paragraph();
            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");
            Assert.That(workflow, Is.Null);
            Assert.That(method, Is.Null);

            generator.CurrentParagraph.WorkflowsInScope.Add(first);

            (workflow, method) = generator.FindWorkflowMethod("helloworld");
            Assert.That(workflow, Is.Not.Null);
            Assert.That(method, Is.Not.Null);
        }

        [Test]
        public void FindWorkflowMethod_returns_implementation_when_lookup_matches()
        {
            var detail = new WorkflowDetail { Name = "GreetingWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "HelloWorld" });

            generator.CurrentParagraph = new CST.Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(workflow, Is.Not.Null);
            Assert.That(method, Is.Not.Null);
        }

        [Test]
        public void FindWorkflowMethod_returns_last_match_if_several_workflows_implement()
        {
            var detail = new WorkflowDetail { Name = "GreetingWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "Howdy_stranger" });
            generator.CurrentParagraph = new CST.Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            detail = new WorkflowDetail { Name = "SheriffWorkflow" };
            detail.AddMethod(new MethodDetail { Name = "Howdy_stranger" });
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("howdystranger");

            //  tie breaking on last added allows for overriding, sounds sensible at the moment
            Assert.That(workflow.Name, Is.EqualTo("SheriffWorkflow"));
        }


        [Test]
        public void declarations_empty_to_start()
        {
            generator.WriteDeclarations();
            var declarations = builder.ToString();

            Assert.That(declarations.Trim(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void one_declaration_per_needed_workflow()
        {
            scenario.SeenWorkflowRequests = new List<string> {
                "first test",
                "anothertestWorkflow",
                "FirstTest WORKFLOW"
            };

            generator.WriteDeclarations();

            var declarations = builder.ToString().Trim();
            var lines = Regex.Split(declarations, Environment.NewLine);

            Assert.That(lines[0].Trim(), Is.EqualTo("public Tabula.FirstTestWorkflow FirstTest;"));
            Assert.That(lines[1].Trim(), Is.EqualTo("public Tabula.extra.AnotherTestWorkflow AnotherTest;"));
            Assert.That(lines.Count(), Is.EqualTo(2));
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
