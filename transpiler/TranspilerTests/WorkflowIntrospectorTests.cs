using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class WorkflowIntrospectorTests
    {
        WorkflowIntrospector introspector;
        List<Type> types;

        [SetUp]
        public void SetUp()
        {
            introspector = new WorkflowIntrospector();
            types = introspector.GetLoadedTypes();
        }

        //FUTURE:  Stop relying on Acadis types for these tests.
        [TestCase("CommentsModalWorkflow")]
        [TestCase("ListManagement")]
        [TestCase("DivisionManagement")]
        [TestCase("Workflow")]
        [TestCase("MvcBaseWorkflow")]
        public void GetWorkflowDetail_gets_workflows_and_workflow_base_classes(string typeName)
        {
            var type = types.Single(t => t.Name == typeName);
            bool isWorkflow = introspector.IsWorkflow(type);
            var detail = introspector.GetWorkflowDetail(type);

            Assert.That(isWorkflow);
            Assert.That(detail, Is.Not.Null);
        }

        [TestCase("RuntimeContext")]
        [TestCase("WorkflowProperty")]
        [TestCase("MvcAction")]
        [TestCase("TuitionBilling_generated")]
        [TestCase("TextAttribute")]
        public void GetWorkflowDetail_skips_utility_classes(string typeName)
        {
            var type = types.Single(t => t.Name == typeName);
            bool isWorkflow = introspector.IsWorkflow(type);
            var detail = introspector.GetWorkflowDetail(type);

            Assert.That(isWorkflow, Is.False);
            Assert.That(detail, Is.Null);
        }

        [TestCase(typeof(string))]
        [TestCase(typeof(object))]
        [TestCase(null)]
        public void GetWorkflowDetail_skips_outside_classes(Type type)
        {
            bool isWorkflow = introspector.IsWorkflow(type);
            var detail = introspector.GetWorkflowDetail(type);

            Assert.That(isWorkflow, Is.False);
            Assert.That(detail, Is.Null);
        }

        [Test]
        public void Parents_of_workflow_are_populated()
        {
            var myComments = types.Single(t => t.Name == "CommentsModalWorkflow");

            var detail = introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["addcomment"];
            Assert.That(method.Name == "Add_comment__");
        }

        [Test]
        public void Methods_of_workflow_are_populated()
        {
            var myComments = types.Single(t => t.Name == "CommentsModalWorkflow");

            var detail = introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["addcomment"];
            Assert.That(method.Name == "Add_comment__");
        }

        [Test]
        public void Arguments_of_method_are_populated()
        {
            Type myComments = types.Single(t => t.Name == "CommentsModalWorkflow");

            var detail = introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["verifycommenttextis"];

            Assert.That(method.Args.Count(), Is.EqualTo(2));
            Assert.That(method.Args[0], Is.EqualTo("rowNum"));
            Assert.That(method.Args[1], Is.EqualTo("text"));
        }

    }
}
