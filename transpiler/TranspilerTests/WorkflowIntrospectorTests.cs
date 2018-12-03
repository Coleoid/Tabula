using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class WorkflowIntrospectorTests
    {
        private WorkflowIntrospector _introspector;
        private List<Type> _types;

        [SetUp]
        public void SetUp()
        {
            _introspector = new WorkflowIntrospector {
                Location = "k:\\code\\Tabula\\transpiler\\LibraryHoldingTestWorkflows\\bin\\debug",
                Library = "LibraryHoldingTestWorkflows.dll"
            };

            _types = _introspector.GetLoadedTypes();
        }

        //FUTURE:  Stop relying on Acadis types for these tests.
        [TestCase("CommentsModalWorkflow")]
        [TestCase("ListManagement")]
        [TestCase("MvcBaseWorkflow")]
        public void GetWorkflowDetail_gets_workflows_and_workflow_base_classes(string typeName)
        {
            var type = _types.SingleOrDefault(t => t.Name == typeName);
            Assert.That(type, Is.Not.Null, $"No type matching {typeName}");
            bool isWorkflow = _introspector.IsWorkflow(type);
            var detail = _introspector.GetWorkflowDetail(type);

            Assert.That(isWorkflow);
            Assert.That(detail, Is.Not.Null);
        }

        [TestCase("Workflow")]
        public void multiple_workflow_classes(string typeName)
        {
            var wfs = _types.Where(t => t.Name == typeName);
            foreach (var type in wfs)
            {
                bool isWorkflow = _introspector.IsWorkflow(type);
                var detail = _introspector.GetWorkflowDetail(type);
                Assert.That(isWorkflow);
                Assert.That(detail, Is.Not.Null);
            }
        }


        [TestCase("WorkflowProperty")]
        public void GetWorkflowDetail_skips_utility_classes(string typeName)
        {
            var type = _types.Single(t => t.Name == typeName);
            bool isWorkflow = _introspector.IsWorkflow(type);
            var detail = _introspector.GetWorkflowDetail(type);

            Assert.That(isWorkflow, Is.False);
            Assert.That(detail, Is.Null);
        }

        [TestCase(typeof(string))]
        [TestCase(typeof(object))]
        [TestCase(null)]
        public void GetWorkflowDetail_skips_outside_classes(Type type)
        {
            bool isWorkflow = _introspector.IsWorkflow(type);
            var detail = _introspector.GetWorkflowDetail(type);

            Assert.That(isWorkflow, Is.False);
            Assert.That(detail, Is.Null);
        }

        //FIXME:  this isn't checking parent right now, it's pasta of test below
        [Test]
        public void Parents_of_workflow_are_populated()
        {
            var myComments = _types.Single(t => t.Name == "CommentsModalWorkflow");

            var detail = _introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["addcomment"];
            Assert.That(method.Name == "Add_comment__");
        }

        [Test]
        public void Methods_of_workflow_are_populated()
        {
            var myComments = _types.Single(t => t.Name == "CommentsModalWorkflow");

            var detail = _introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["addcomment"];
            Assert.That(method.Name == "Add_comment__");
        }

        [Test]
        public void Arguments_of_method_are_populated()
        {
            Type myComments = _types.Single(t => t.Name == "CommentsModalWorkflow");

            var detail = _introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["verifycommenttextis"];

            Assert.That(method.Args.Count(), Is.EqualTo(2));
            Assert.That(method.Args[0].Name, Is.EqualTo("rowNum"));
            Assert.That(method.Args[1].Name, Is.EqualTo("text"));
        }

    }
}
