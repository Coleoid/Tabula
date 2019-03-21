using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
            generator = new Generator {Builder = builder, Scenario = scenario, InputFilePath = "scenario_source.tab"};
            first = generator.Library.GetWorkflowDetail(typeof(FirstTestWorkflow));
            second = generator.Library.GetWorkflowDetail(typeof(SecondTestWorkflow));
            another = generator.Library.GetWorkflowDetail(typeof(extra.AnotherTestWorkflow));
        }


        [Test]
        public void PrepareAction_puts_workflows_in_paragraph_scope()
        {
            var paragraph = new CST.Paragraph();
            generator.CurrentParagraph = paragraph;
            var action = new CST.CommandUse(new List<string> {"FirstTestWorkflow"});
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

            var action = new CST.CommandUse(new List<string> {requestedWorkflow});
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

            var action = new CST.CommandUse(new List<string> {"FirstTestWorkflow"});
            generator.UseWorkflow(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(paragraph.WorkflowsInScope[0], Is.SameAs(first));

            action = new CST.CommandUse(new List<string> {"SecondTestWorkflow", "AnotherTestWorkflow"});
            generator.UseWorkflow(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(3));
            Assert.That(paragraph.WorkflowsInScope[2], Is.SameAs(another));
        }

        //TODO: make sure that in-paragraph workflow instance names clip off 'workflow'

        //TODO: second and subsequent workflow usages don't cause a new 'var myfixture = ...' line

        [Test]
        public void UseWorkflow_will_not_add_duplicate_workflows()
        {
            var paragraph = new CST.Paragraph();
            generator.CurrentParagraph = paragraph;
            var action = new CST.CommandUse(new List<string> {"FirstTestWorkflow"});
            generator.UseWorkflow(action);

            action = new CST.CommandUse(new List<string> {"First Test Workflow"});
            generator.UseWorkflow(action);

            Assert.That(paragraph.WorkflowsInScope, Has.Count.EqualTo(1));
            Assert.That(paragraph.WorkflowsInScope[0], Is.SameAs(first));
        }

        [Test]
        public void Use_command_complains_sensibly_when_workflow_unfound()
        {
            var paragraph = new CST.Paragraph();
            var action = new CST.CommandUse(new List<string> {"P equals NP workflow"});
            var ex = Assert.Throws<Exception>(() => generator.UseWorkflow(action));

            Assert.That(ex.Message, Is.EqualTo("Tabula found no workflow matching [P equals NP workflow]."));
        }

        [Test]
        public void FindWorkflowMethod_returns_nulls_if_no_workflow_has_method_matching_step()
        {
            var detail = new WorkflowDetail {Name = "GreetingWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "Howdy_stranger"});
            detail.AddMethod(new MethodDetail {Name = "Hello_Everybody"});
            var paragraph = new CST.Paragraph();
            generator.CurrentParagraph = paragraph;
            paragraph.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(workflow, Is.Null);
            Assert.That(method, Is.Null);
        }

        [Test]
        public void FindWorkflowMethod_finds_in_all_workflows_in_scope()
        {
            var workflow = new WorkflowDetail { Name = "GreetingWorkflow" };
            workflow.AddMethod(new MethodDetail { Name = "Howdy_stranger" });
            workflow.AddMethod(new MethodDetail { Name = "Hello_Everybody" });
            var secondWorkflow = new WorkflowDetail { Name = "CoderWorkflow" };
            secondWorkflow.AddMethod(new MethodDetail { Name = "hello_world" });
            var paragraph = new CST.Paragraph();
            generator.CurrentParagraph = paragraph;
            paragraph.WorkflowsInScope.Add(workflow);
            paragraph.WorkflowsInScope.Add(secondWorkflow);

            (var resultWorkflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(resultWorkflow, Is.SameAs(secondWorkflow));
            Assert.That(method, Is.Not.Null);
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
            var detail = new WorkflowDetail {Name = "GreetingWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "HelloWorld"});

            generator.CurrentParagraph = new CST.Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("helloworld");

            Assert.That(workflow, Is.Not.Null);
            Assert.That(method, Is.Not.Null);
        }

        [Test]
        public void FindWorkflowMethod_returns_last_match_if_several_workflows_implement()
        {
            var detail = new WorkflowDetail {Name = "GreetingWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "Howdy_stranger"});
            generator.CurrentParagraph = new CST.Paragraph();
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            detail = new WorkflowDetail {Name = "SheriffWorkflow"};
            detail.AddMethod(new MethodDetail {Name = "Howdy_stranger"});
            generator.CurrentParagraph.WorkflowsInScope.Add(detail);

            (var workflow, var method) = generator.FindWorkflowMethod("howdystranger");

            //  tie breaking on last added allows for overriding, sounds sensible at the moment
            Assert.That(workflow.Name, Is.EqualTo("SheriffWorkflow"));
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
